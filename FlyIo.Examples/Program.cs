using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FlyIo.Examples;

internal static class Program
{
    private static async Task Main()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        var node = new MaelstromNode(new ConsoleProcessor(), new DebugLogger(false));
        node.On("init", Handlers.HandleInit(node));
        node.On("echo", Handlers.HandleEcho(node));
        node.On("broadcast", Handlers.HandleBroadcast(node));
        //node.On("broadcast_ok", Handlers.HandleBroadcastOk(node));
        node.On("topology", Handlers.HandleTopology(node));
        node.On("read", Handlers.HandleRead(node));
        node.On("error", Handlers.HandleError(node));
        node.On("generate", Handlers.HandleGenerate(node));

        await node.RunAsync();
    }
}

internal readonly struct Envelope
{
    [JsonPropertyName("src")]
    public string Src { get; init; }

    [JsonPropertyName("dest")]
    public string Dest { get; init; }

    [JsonPropertyName("body")]
    public Body Body { get; init; }
}

internal struct Body
{
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; init; }

    [JsonPropertyName("node_ids")]
    public string[] NodeIds { get; init; }

    [JsonPropertyName("msg_id")]
    public int? MsgId { get; init; }

    [JsonPropertyName("in_reply_to")]
    public int? InReplyTo { get; init; }

    [JsonPropertyName("echo")]
    public string? Echo { get; init; }

    [JsonPropertyName("message")]
    public int? Message { get; init; }

    [JsonPropertyName("messages")]
    public int[] Messages { get; init; }

    [JsonPropertyName("topology")]
    public Topology? Topology { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }
}

public struct Topology
{
    public string[] n0 { get; set; }
    public string[] n1 { get; set; }
    public string[] n2 { get; set; }
    public string[] n3 { get; set; }
    public string[] n4 { get; set; }
    public string[] n5 { get; set; }
}

internal static class JsonElementExt
{
    public static JsonElement? GetPropertyOrNull(this JsonElement el, string name)
        => el.TryGetProperty(name, out var v) ? v : null;

    public static Dictionary<string, JsonElement> ToMutableDictionary(this JsonElement el)
    {
        var dict = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        foreach (var p in el.EnumerateObject())
            dict[p.Name] = p.Value.Clone();
        return dict;
    }

    public static JsonElement Clone(this JsonElement el)
    {
        using var doc = JsonDocument.Parse(el.GetRawText());
        return doc.RootElement.Clone();
    }
}