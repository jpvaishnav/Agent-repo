using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Relay;

namespace AgentThroughWebSocket.Shared;

public static class RelayTokenProviderFactory
{
    public static TokenProvider Create(AzureRelayOptions options)
    {
        if (options.Authentication.Equals("Sas", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(options.SasKeyName) || string.IsNullOrWhiteSpace(options.SasKey))
            {
                throw new InvalidOperationException(
                    "AzureRelay:SasKeyName and AzureRelay:SasKey are required for SAS authentication.");
            }

            return TokenProvider.CreateSharedAccessSignatureTokenProvider(
                options.SasKeyName,
                options.SasKey);
        }

        if (!options.Authentication.Equals("EntraId", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("AzureRelay:Authentication must be EntraId or Sas.");
        }

        var credential = new DefaultAzureCredential();
        return TokenProvider.CreateAzureActiveDirectoryTokenProvider(
            async (audience, _, _) =>
            {
                var scope = $"{audience.TrimEnd('/')}/.default";
                var token = await credential.GetTokenAsync(
                    new TokenRequestContext([scope]),
                    CancellationToken.None);
                return token.Token;
            },
            "https://login.microsoftonline.com/common");
    }
}
