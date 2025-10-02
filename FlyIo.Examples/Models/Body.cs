using System.Text.Json.Serialization;

namespace FlyIo.Examples.Models;

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

    [JsonPropertyName("value")]
    public int? Value { get; init; }

    [JsonPropertyName("delta")]
    public int? Delta { get; init; }
}