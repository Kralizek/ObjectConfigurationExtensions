using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Kralizek.Extensions.Configuration.Internal;

internal static class SystemTextJsonConfigurationSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    public static JsonElement Serialize<T>(T source)
    {
        return JsonSerializer.SerializeToElement(source, JsonOptions);
    }

    public static JsonElement Serialize<T>(T source, JsonTypeInfo<T> jsonTypeInfo)
    {
        return JsonSerializer.SerializeToElement(source, jsonTypeInfo);
    }
}
