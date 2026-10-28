using AgentThroughWebSocket.Shared;
using RelayListener;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<AzureRelayOptions>(
    builder.Configuration.GetSection(AzureRelayOptions.SectionName));
builder.Services.AddSingleton(new HttpClient
{
    BaseAddress = new Uri(
        builder.Configuration["AgentX:BaseUrl"] ?? "https://localhost:5001"),
    Timeout = TimeSpan.FromSeconds(30)
});
builder.Services.AddHostedService<RelayWorker>();

await builder.Build().RunAsync();
