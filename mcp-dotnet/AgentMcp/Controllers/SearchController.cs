using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Collections.Generic;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace AgentMcp.Controllers
{
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly HttpClient _http = new HttpClient();
        private readonly AgentMcp.Services.McpFormatter _mcp;

        public SearchController(AgentMcp.Services.McpFormatter mcp) { _mcp = mcp; }

        [HttpGet("/keepalive")]
        public IActionResult KeepAlive() => Ok(_mcp.Format("keepalive", new { status = "ok" }));

        [HttpGet("/web_search")]
        public async Task<IActionResult> WebSearch([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest(new { error = "query is required" });

            var engines = new[] {
                new { name = "google", url = $"https://www.google.com/search?q={Uri.EscapeDataString(query)}" },
                new { name = "bing", url = $"https://www.bing.com/search?q={Uri.EscapeDataString(query)}" },
                new { name = "duckduckgo", url = $"https://html.duckduckgo.com/html?q={Uri.EscapeDataString(query)}" },
                new { name = "yahoo", url = $"https://search.yahoo.com/search?p={Uri.EscapeDataString(query)}" },
                new { name = "startpage", url = $"https://www.startpage.com/sp/search?q={Uri.EscapeDataString(query)}" }
            };

            var results = new List<object>();

            _http.DefaultRequestHeaders.UserAgent.Clear();
            _http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AgentMcp", "1.0"));

            foreach (var engine in engines)
            {
                try
                {
                    var html = await _http.GetStringAsync(engine.url);
                    var items = await ExtractTopResultsAsync(html, 3);
                    results.Add(new { engine = engine.name, searchUrl = engine.url, topResults = items });
                }
                catch (Exception ex)
                {
                    results.Add(new { engine = engine.name, searchUrl = engine.url, error = ex.Message });
                }
            }

            var envelope = _mcp.Format("web_search", new { query, results });
            return Ok(envelope);
        }

        private async Task<List<object>> ExtractTopResultsAsync(string html, int max)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var list = new List<object>();
            var anchors = doc.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>();

            foreach (var a in anchors)
            {
                if (list.Count >= max) break;
                var href = a.GetAttributeValue("href", "");
                if (string.IsNullOrWhiteSpace(href)) continue;

                // normalize possible relative URLs or engine trackers
                if (!href.StartsWith("http://") && !href.StartsWith("https://"))
                {
                    var qIdx = href.IndexOf("q=");
                    if (qIdx >= 0)
                    {
                        var part = href.Substring(qIdx + 2);
                        var amp = part.IndexOf('&');
                        if (amp >= 0) part = part.Substring(0, amp);
                        try { href = Uri.UnescapeDataString(part); }
                        catch { }
                    }
                }

                if (!href.StartsWith("http")) continue;

                var text = HtmlEntity.DeEntitize(a.InnerText ?? string.Empty).Trim();
                if (text.Length < 3) continue;

                string snippet = string.Empty;
                var parent = a.ParentNode;

                // Try to extract snippet from nearby nodes (same as before)
                if (parent != null)
                {
                    var candidates = parent.SelectNodes(".//p|.//span|.//div") ?? Enumerable.Empty<HtmlNode>();
                    foreach (var c in candidates)
                    {
                        var s = HtmlEntity.DeEntitize(c.InnerText ?? string.Empty).Trim();
                        if (s.Length >= 30 && !s.Equals(text, StringComparison.OrdinalIgnoreCase))
                        {
                            snippet = s;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(snippet))
                    {
                        var sib = a.NextSibling;
                        int steps = 0;
                        while (sib != null && steps < 6)
                        {
                            var stext = HtmlEntity.DeEntitize(sib.InnerText ?? string.Empty).Trim();
                            if (!string.IsNullOrEmpty(stext) && stext.Length >= 20 && !stext.Equals(text, StringComparison.OrdinalIgnoreCase))
                            {
                                snippet = stext;
                                break;
                            }
                            sib = sib.NextSibling;
                            steps++;
                        }
                    }

                    if (string.IsNullOrEmpty(snippet))
                    {
                        var gp = parent.ParentNode;
                        if (gp != null)
                        {
                            var gpP = gp.SelectSingleNode(".//p|.//div[contains(@class,'snippet')]|.//span[contains(@class,'snippet')]");
                            if (gpP != null) snippet = HtmlEntity.DeEntitize(gpP.InnerText ?? string.Empty).Trim();
                        }
                    }
                }

                // Fallback meta description
                if (string.IsNullOrEmpty(snippet))
                {
                    var meta = doc.DocumentNode.SelectSingleNode("//meta[@name='description']") ?? doc.DocumentNode.SelectSingleNode("//meta[@property='og:description']");
                    if (meta != null) snippet = HtmlEntity.DeEntitize(meta.GetAttributeValue("content", "")).Trim();
                }

                // If still empty, fetch the target page and attach its HTML body text as the snippet (trimmed)
                if (string.IsNullOrEmpty(snippet))
                {
                    try
                    {
                        var pageHtml = await _http.GetStringAsync(href);
                        var pageDoc = new HtmlDocument();
                        pageDoc.LoadHtml(pageHtml);
                        var body = pageDoc.DocumentNode.SelectSingleNode("//body");
                        var bodyText = body != null ? HtmlEntity.DeEntitize(body.InnerText ?? string.Empty).Trim() : HtmlEntity.DeEntitize(pageDoc.DocumentNode.InnerText ?? string.Empty).Trim();
                        if (!string.IsNullOrEmpty(bodyText))
                        {
                            snippet = bodyText.Length > 2000 ? bodyText.Substring(0, 2000) + "..." : bodyText;
                        }
                    }
                    catch
                    {
                        // leave snippet empty if fetch fails
                    }
                }

                list.Add(new { title = text, link = href, snippet });
            }

            return list;
        }
    }
}
