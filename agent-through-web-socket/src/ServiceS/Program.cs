using AgentThroughWebSocket.Shared;
using Microsoft.Azure.Relay;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<AzureRelayOptions>(
    builder.Configuration.GetSection(AzureRelayOptions.SectionName));
builder.Services.AddSingleton(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IOptions<AzureRelayOptions>>().Value;
    return new HybridConnectionClient(
        options.GetConnectionUri(),
        RelayTokenProviderFactory.Create(options));
});

var app = builder.Build();

app.MapGet("/check_with_x", async (
    string? q,
    HybridConnectionClient relayClient,
    ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    try
    {
        using var connection = await relayClient.CreateConnectionAsync().WaitAsync(cancellationToken);
        await RelayProtocol.WriteAsync(
            connection,
            new AgentRequest(q ?? ""),
            cancellationToken);
        var response = await RelayProtocol.ReadAsync<AgentResponse>(
            connection,
            cancellationToken);

        return Results.Content(response.Body, response.ContentType, statusCode: response.StatusCode);
    }
    catch (Exception exception) when (exception is not OperationCanceledException)
    {
        logger.LogError(exception, "Could not reach Agent X through Azure Relay");
        return Results.Problem(
            title: "Agent X is unavailable",
            detail: "Service S could not establish the outbound Relay connection.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

await app.RunAsync();
