using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Configuration;

namespace Kralizek.Extensions.Configuration.Internal;

internal static class SystemTextJsonConfigurationSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    public static MemoryStream Serialize<T>(T source, string rootSectionName)
    {
        return Serialize(rootSectionName, writer => JsonSerializer.Serialize(writer, source, JsonOptions));
    }

    public static MemoryStream Serialize<T>(T source, JsonTypeInfo<T> jsonTypeInfo, string rootSectionName)
    {
        return Serialize(rootSectionName, writer => JsonSerializer.Serialize(writer, source, jsonTypeInfo));
    }

    private static MemoryStream Serialize(string rootSectionName, Action<Utf8JsonWriter> serialize)
    {
        var stream = new MemoryStream();

        using (var writer = new Utf8JsonWriter(stream))
        {
            var rootSections = GetRootSections(rootSectionName);

            foreach (var rootSection in rootSections)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(rootSection);
            }

            serialize(writer);

            for (var index = 0; index < rootSections.Length; index++)
            {
                writer.WriteEndObject();
            }
        }

        stream.Position = 0;

        return stream;
    }

    private static string[] GetRootSections(string rootSectionName)
    {
        return string.IsNullOrEmpty(rootSectionName)
            ? Array.Empty<string>()
            : rootSectionName.Split(new[] { ConfigurationPath.KeyDelimiter }, StringSplitOptions.None);
    }
}
