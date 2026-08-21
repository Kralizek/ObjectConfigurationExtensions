using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Configuration;

namespace Kralizek.Extensions.Configuration.Internal;

internal static class SystemTextJsonConfigurationSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static IDictionary<string, string?> Serialize<T>(T source, string rootSectionName)
    {
        var json = JsonSerializer.Serialize(source, JsonOptions);

        return Flatten(json, rootSectionName);
    }

    public static IDictionary<string, string?> Serialize<T>(T source, JsonTypeInfo<T> jsonTypeInfo, string rootSectionName)
    {
        var json = JsonSerializer.Serialize(source, jsonTypeInfo);

        return Flatten(json, rootSectionName);
    }

    private static IDictionary<string, string?> Flatten(string json, string rootSectionName)
    {
        using var document = JsonDocument.Parse(json);
        var visitor = new JsonVisitor();

        return visitor.ParseObject(document.RootElement, rootSectionName);
    }

    private sealed class JsonVisitor
    {
        private readonly IDictionary<string, string?> _data = new SortedDictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new();
        private string _currentPath = string.Empty;

        public IDictionary<string, string?> ParseObject(JsonElement jsonObject, string rootSectionName)
        {
            if (!string.IsNullOrEmpty(rootSectionName))
            {
                EnterContext(rootSectionName);
            }

            VisitElement(jsonObject);

            if (!string.IsNullOrEmpty(rootSectionName))
            {
                ExitContext();
            }

            return _data;
        }

        private void VisitElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        EnterContext(property.Name);
                        VisitElement(property.Value);
                        ExitContext();
                    }
                    break;

                case JsonValueKind.Array:
                    var index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        EnterContext(index.ToString());
                        VisitElement(item);
                        ExitContext();
                        index++;
                    }
                    break;

                case JsonValueKind.Number:
                case JsonValueKind.String:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    VisitPrimitive(element);
                    break;

                case JsonValueKind.Undefined:
                default:
                    throw new NotSupportedException($"Unsupported JSON token '{element.ValueKind}' was found");
            }
        }

        private void VisitPrimitive(JsonElement data)
        {
            var key = _currentPath;

            if (data.ValueKind == JsonValueKind.Null)
            {
                return;
            }

            if (_data.ContainsKey(key))
            {
                throw new FormatException($"A duplicate key '{key}' was found.");
            }

            var stringValue = data.ValueKind switch
            {
                JsonValueKind.String => data.GetString(),
                _ => data.GetRawText()
            };

            if (!string.IsNullOrEmpty(stringValue))
            {
                _data[key] = stringValue;
            }
        }

        private void EnterContext(string context)
        {
            _context.Push(context);
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private void ExitContext()
        {
            _context.Pop();
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }
    }
}
