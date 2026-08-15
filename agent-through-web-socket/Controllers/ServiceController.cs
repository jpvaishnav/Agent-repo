using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AgentThroughWebSocket.Controllers
{
    [ApiController]
    [Route("service")]
    public class ServiceController : ControllerBase
    {
        // Endpoint for Service S to ask Agent X (via Relay / WebSocket tunnel)
        [HttpGet("check_with_x")]
        public async Task<IActionResult> CheckWithX([FromQuery] string q)
        {
            // Placeholder behavior: a real implementation would use an Azure Relay client or WebSocket tunnel proxy            
            await Task.CompletedTask;
            return Ok(new { query = q, note = "Placeholder: forward this via Azure Relay / WebSocket to AgentX" });
        }
    }
}
