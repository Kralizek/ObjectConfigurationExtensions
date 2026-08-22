using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Kralizek.Extensions.Configuration.Internal;

internal static class JsonConfigurationFlattener
{
    public static IDictionary<string, string?> Flatten(JsonElement element, string rootSectionName)
    {
        var visitor = new JsonVisitor();

        return visitor.Flatten(element, rootSectionName);
    }

    private sealed class JsonVisitor
    {
        private readonly IDictionary<string, string?> _data = new SortedDictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new();
        private string _currentPath = string.Empty;

        public IDictionary<string, string?> Flatten(JsonElement element, string rootSectionName)
        {
            if (!string.IsNullOrEmpty(rootSectionName))
            {
                EnterContext(rootSectionName);
            }

            VisitElement(element);

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
                    VisitObject(element);
                    break;

                case JsonValueKind.Array:
                    VisitArray(element);
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

        private void VisitObject(JsonElement element)
        {
            var isEmpty = true;

            foreach (var property in element.EnumerateObject())
            {
                isEmpty = false;
                EnterContext(property.Name);
                VisitElement(property.Value);
                ExitContext();
            }

            if (isEmpty)
            {
                AddCurrentValue(null);
            }
        }

        private void VisitArray(JsonElement element)
        {
            var index = 0;

            foreach (var item in element.EnumerateArray())
            {
                EnterContext(index.ToString(CultureInfo.InvariantCulture));
                VisitElement(item);
                ExitContext();
                index++;
            }

            if (index == 0)
            {
                AddCurrentValue(string.Empty);
            }
        }

        private void VisitPrimitive(JsonElement element)
        {
            var value = element.ValueKind switch
            {
                JsonValueKind.Null => null,
                JsonValueKind.String => element.GetString(),
                _ => element.GetRawText()
            };

            AddCurrentValue(value);
        }

        private void AddCurrentValue(string? value)
        {
            if (string.IsNullOrEmpty(_currentPath))
            {
                return;
            }

            if (_data.ContainsKey(_currentPath))
            {
                throw new FormatException($"A duplicate key '{_currentPath}' was found.");
            }

            _data[_currentPath] = value;
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
