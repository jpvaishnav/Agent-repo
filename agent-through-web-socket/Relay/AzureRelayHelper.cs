using System;
using System.Threading.Tasks;
// using Microsoft.Azure.Relay; // uncomment when package available

namespace AgentThroughWebSocket.Relay
{
    public static class AzureRelayHelper
    {
        // Conceptual helper and sample snippets showing how to use Azure Relay Hybrid Connections.
        // See: https://learn.microsoft.com/en-us/azure/azure-relay/relay-what-is-it
        //
        // Example conceptual code (not compiled here):
        //
        // var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(keyName, key);
        // var listener = new HybridConnectionListener(new Uri($"sb://{namespaceName}.servicebus.windows.net/{hybridConnectionName}"), tokenProvider);
        // listener.RequestHandler += async (s, e) => { /* read e.Request and forward to local agent */ };
        // await listener.OpenAsync();
        //
        // The machine M (AgentX) would open the listener (outbound connection to Azure Relay).
        // The service S would create a HybridConnectionClient and send requests to the same hybrid connection.
        //
        // Keep this file as documentation and starting point for code to implement and test.

        public static Task PlaceholderAsync() => Task.CompletedTask;
    }
}
