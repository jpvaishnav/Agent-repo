# Agents Global Context

This file defines the repository-wide context for agents operating in this repository.

- This is the global context for agents: treat this repository as agent-friendly.
- Load skills from: `.claude/skills/` (each skill in its own subfolder with `skill.md`).
- Load instructions from: `.claude/instructions/` and `.github/instructions/`.
- Use `.claude/skills.json` manifest for explicit skill registration when present.
- Keep per-skill outputs inside each skill's `output/` directory to avoid global state.

Agents should respect these conventions when adding skills, reading instructions, or operating on code in this repository.

## Further reading

- Official Claude Agents documentation: https://code.claude.com/docs/en

## Remote MCP tools (dotnet example)

A sample minimal .NET MCP server has been added at `mcp-dotnet/AgentMcp`. It exposes two endpoints useful for agents configured as remote MCP tools:

- GET /keepalive
  - Returns 200 OK with JSON: { "status": "ok" }
  - Use this for health checks and keepalive pings.

- GET /web_search?query=...
  - Searches the given query across 5 search engines (google, bing, duckduckgo, yahoo, startpage) and returns a structured JSON object with per-engine topResults (title, link, snippet).
  - The implementation uses HtmlAgilityPack to extract result anchors when official API keys are not provided. For production, configure and use official search APIs (e.g., Bing Search API, Google Custom Search, SerpAPI) and update the code to call them.

How to run (local):

1. cd mcp-dotnet\AgentMcp
2. dotnet run (requires .NET 10 SDK)

To configure as a remote MCP tool, expose the service (ngrok or hosted URL) and register the base URL in the agents' configuration so the agent can call /keepalive and /web_search. The project is a starting point and should be hardened for production use (rate limits, API keys, robust HTML parsing, retries, and caching).

