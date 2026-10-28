using AgentThroughWebSocket.Shared;

namespace AgentThroughWebSocket.Tests;

public sealed class RelayProtocolTests
{
    [Fact]
    public async Task RoundTrip_PreservesQueryWithNewlineAndUnicode()
    {
        await using var stream = new MemoryStream();
        var expected = new AgentRequest("first line\nsecond line ✓");

        await RelayProtocol.WriteAsync(stream, expected, CancellationToken.None);
        stream.Position = 0;
        var actual = await RelayProtocol.ReadAsync<AgentRequest>(
            stream,
            CancellationToken.None);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetConnectionUri_NormalizesNamespace()
    {
        var options = new AzureRelayOptions
        {
            Namespace = "sb://example.servicebus.windows.net/",
            HybridConnectionName = "agent x"
        };

        Assert.Equal(
            "sb://example.servicebus.windows.net/agent%20x",
            options.GetConnectionUri().AbsoluteUri);
    }
}
