using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Kralizek.Extensions.Configuration.Internal;

internal static class SystemTextJsonConfigurationSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    public static IDictionary<string, string?> Serialize<T>(T source, string rootSectionName)
    {
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(source, JsonOptions));

        return JsonConfigurationFlattener.Flatten(document.RootElement, rootSectionName);
    }

    public static IDictionary<string, string?> Serialize<T>(T source, JsonTypeInfo<T> jsonTypeInfo, string rootSectionName)
    {
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(source, jsonTypeInfo));

        return JsonConfigurationFlattener.Flatten(document.RootElement, rootSectionName);
    }
}
