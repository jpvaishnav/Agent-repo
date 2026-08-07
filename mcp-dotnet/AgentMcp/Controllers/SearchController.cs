using System.Net.Http.Headers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace AgentMcp.Controllers
{
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly HttpClient _http = new HttpClient();

        [HttpGet("/keepalive")]
        public IActionResult KeepAlive() => Ok(new { status = "ok" });

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
                    var items = ExtractTopResults(html, 3);
                    results.Add(new { engine = engine.name, searchUrl = engine.url, topResults = items });
                }
                catch (Exception ex)
                {
                    results.Add(new { engine = engine.name, searchUrl = engine.url, error = ex.Message });
                }
            }

            return Ok(new { query, results });
        }

        private List<object> ExtractTopResults(string html, int max)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var list = new List<object>();

            // Find anchors with http(s) links and meaningful text as a fallback for many engines.
            var anchors = doc.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>();

            foreach (var a in anchors)
            {
                if (list.Count >= max) break;
                var href = a.GetAttributeValue("href", "");
                if (string.IsNullOrWhiteSpace(href)) continue;

                // normalize possible relative URLs or engine trackers
                if (!href.StartsWith("http://") && !href.StartsWith("https://"))
                {
                    // Some search engines embed links like "/url?q=..." (Google) or redirect wrappers.
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

                // snippet: try to find a sibling paragraph or title-like text
                var snippet = string.Empty;
                var parent = a.ParentNode;
                if (parent != null)
                {
                    var p = parent.SelectSingleNode(".//p");
                    if (p != null) snippet = HtmlEntity.DeEntitize(p.InnerText).Trim();
                }

                list.Add(new { title = text, link = href, snippet });
            }

            return list;
        }
    }
}
