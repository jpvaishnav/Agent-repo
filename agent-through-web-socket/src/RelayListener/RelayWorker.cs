using AgentThroughWebSocket.Shared;
using Microsoft.Azure.Relay;
using Microsoft.Extensions.Options;

namespace RelayListener;

public sealed class RelayWorker(
    IOptions<AzureRelayOptions> relayOptions,
    HttpClient agentClient,
    ILogger<RelayWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = relayOptions.Value;
        var listener = new HybridConnectionListener(
            options.GetConnectionUri(),
            RelayTokenProviderFactory.Create(options));

        listener.Connecting += (_, _) => logger.LogInformation("Connecting to Azure Relay");
        listener.Online += (_, _) => logger.LogInformation("Azure Relay listener is online");
        listener.Offline += (_, _) => logger.LogWarning("Azure Relay listener is offline");

        await listener.OpenAsync(stoppingToken);
        using var registration = stoppingToken.Register(
            () => _ = listener.CloseAsync(CancellationToken.None));

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var connection = await listener.AcceptConnectionAsync();
                if (connection is null)
                {
                    break;
                }

                try
                {
                    await HandleConnectionAsync(connection, stoppingToken);
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    logger.LogWarning(exception, "Relay connection closed before a response was sent");
                }
            }
        }
        finally
        {
            await listener.CloseAsync(CancellationToken.None);
        }
    }

    private async Task HandleConnectionAsync(
        HybridConnectionStream connection,
        CancellationToken cancellationToken)
    {
        using (connection)
        {
            try
            {
                var request = await RelayProtocol.ReadAsync<AgentRequest>(connection, cancellationToken);
                var path = $"/get_ai_tools?q={Uri.EscapeDataString(request.Query)}";
                using var response = await agentClient.GetAsync(path, cancellationToken);
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                var contentType = response.Content.Headers.ContentType?.ToString()
                    ?? "application/json";

                await RelayProtocol.WriteAsync(
                    connection,
                    new AgentResponse((int)response.StatusCode, contentType, body),
                    cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "Failed to proxy a Relay request to Agent X");
                await RelayProtocol.WriteAsync(
                    connection,
                    new AgentResponse(502, "application/problem+json", "{\"title\":\"Agent X is unavailable\"}"),
                    cancellationToken);
            }
        }
    }
}
