using System.Text.Json.Serialization;

namespace FlyIo.Examples.Models;

internal readonly struct Envelope
{
    [JsonPropertyName("src")]
    public string Src { get; init; }

    [JsonPropertyName("dest")]
    public string Dest { get; init; }

    [JsonPropertyName("body")]
    public Body Body { get; init; }
}