using System.Threading.Tasks;

namespace AgentThroughWebSocket.Relay
{
    public class ServiceSClient
    {
        // Placeholder for a client that calls the Azure Relay or WebSocket endpoint.
        // In production this would create a HybridConnectionClient and send an HttpRequestMessage through it.
        // For example:
        //
        // var client = new HybridConnectionClient(new Uri(relayUri), tokenProvider);
        // using (var relayStream = await client.CreateConnectionAsync())
        // using (var httpClient = new HttpClient(new StreamHttpHandler(relayStream))) { /* send request */ }
        //
        // This project intentionally keeps the implementation light-weight for illustration.
        public Task<string> CheckAsync(string q) => Task.FromResult($"Simulated response for q={q}");
    }
}
