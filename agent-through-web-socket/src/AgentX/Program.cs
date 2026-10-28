var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/get_ai_tools", (string? q) => Results.Ok(new
{
    agent = "Agent X",
    query = q ?? "",
    tools = new[] { "search", "summarize", "reason" },
    generatedAtUtc = DateTimeOffset.UtcNow
}));

app.Run();
