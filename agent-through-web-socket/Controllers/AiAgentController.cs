using Microsoft.AspNetCore.Mvc;

namespace AgentThroughWebSocket.Controllers
{
    [ApiController]
    [Route("/")]
    public class AiAgentController : ControllerBase
    {
        // Dummy AI Agent X endpoint (simulates the agent running on machine M)
        [HttpGet("get_ai_tools")]
        public IActionResult GetAiTools()
        {
            var tools = new[] { "search", "calculator", "summarizer" };
            return Ok(new { agent = "AgentX", tools });
        }
    }
}
