using System;
using System.Collections.Generic;

namespace AgentMcp.Services
{
    // Minimal MCP-compatible envelope formatter.
    // Replace with the official Model Context Protocol NuGet package implementation when available.
    public class McpFormatter
    {
        public object Format(string toolName, object payload)
        {
            return new {
                mcp_version = "1.0",
                tool = toolName,
                timestamp = DateTime.UtcNow.ToString("o"),
                payload
            };
        }
    }
}
