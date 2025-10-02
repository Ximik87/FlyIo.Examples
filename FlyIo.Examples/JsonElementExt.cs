using System.Text.Json;

namespace FlyIo.Examples;

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