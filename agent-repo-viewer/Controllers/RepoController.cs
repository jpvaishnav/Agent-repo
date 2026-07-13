using AgentRepoViewer.Models;
using AgentRepoViewer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgentRepoViewer.Controllers;

[ApiController]
[Route("api")]
public class RepoController : ControllerBase
{
    private readonly RepoAnalyzerService _analyzer;

    public RepoController(RepoAnalyzerService analyzer)
    {
        _analyzer = analyzer;
    }

    [HttpGet("repo")]
    public async Task<IActionResult> GetRepoContext()
    {
        var context = await _analyzer.AnalyzeRepoAsync();
        return Ok(context);
    }
}
