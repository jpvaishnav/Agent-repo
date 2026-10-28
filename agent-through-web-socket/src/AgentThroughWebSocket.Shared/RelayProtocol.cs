using System.Text.Json;

namespace AgentThroughWebSocket.Shared;

public sealed record AgentRequest(string Query);

public sealed record AgentResponse(int StatusCode, string ContentType, string Body);

public static class RelayProtocol
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static async Task WriteAsync<T>(Stream stream, T message, CancellationToken cancellationToken)
    {
        var writer = new StreamWriter(stream, leaveOpen: true) { AutoFlush = true };
        var json = JsonSerializer.Serialize(message, JsonOptions);
        await writer.WriteLineAsync(json.AsMemory(), cancellationToken);
    }

    public static async Task<T> ReadAsync<T>(Stream stream, CancellationToken cancellationToken)
    {
        var reader = new StreamReader(stream, leaveOpen: true);
        var line = await reader.ReadLineAsync(cancellationToken);
        if (line is null)
        {
            throw new EndOfStreamException("The relay peer closed the connection before sending a message.");
        }

        return JsonSerializer.Deserialize<T>(line, JsonOptions)
            ?? throw new InvalidDataException("The relay peer sent an invalid message.");
    }
}
