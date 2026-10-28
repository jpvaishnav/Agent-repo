namespace AgentThroughWebSocket.Shared;

public sealed class AzureRelayOptions
{
    public const string SectionName = "AzureRelay";

    public string Namespace { get; init; } = "";
    public string HybridConnectionName { get; init; } = "agent-x";
    public string Authentication { get; init; } = "EntraId";
    public string SasKeyName { get; init; } = "";
    public string SasKey { get; init; } = "";

    public Uri GetConnectionUri()
    {
        if (string.IsNullOrWhiteSpace(Namespace))
        {
            throw new InvalidOperationException("AzureRelay:Namespace is required.");
        }

        if (string.IsNullOrWhiteSpace(HybridConnectionName))
        {
            throw new InvalidOperationException("AzureRelay:HybridConnectionName is required.");
        }

        var relayNamespace = Namespace
            .Replace("sb://", "", StringComparison.OrdinalIgnoreCase)
            .Trim().TrimEnd('/');

        return new Uri($"sb://{relayNamespace}/{Uri.EscapeDataString(HybridConnectionName)}");
    }
}
