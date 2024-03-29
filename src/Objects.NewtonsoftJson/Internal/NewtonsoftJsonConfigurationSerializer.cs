using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kralizek.Extensions.Configuration.Internal;

public class NewtonsoftJsonConfigurationSerializer : IConfigurationSerializer
{
    public IDictionary<string, string?> Serialize(object source, string rootSectionName)
    {
        var json = JsonConvert.SerializeObject(source);
        var jsonConfig = JObject.Parse(json);

        var visitor = new JsonVisitor();

        return visitor.ParseObject(jsonConfig, rootSectionName);
    }

    private class JsonVisitor
    {
        private readonly IDictionary<string, string?> _data = new SortedDictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new ();
        private string _currentPath = null!;

        public IDictionary<string, string?> ParseObject(JObject jsonObject, string rootSectionName)
        {
            if (rootSectionName != "")
            {
                EnterContext(rootSectionName);
            }

            VisitJObject(jsonObject);

            if (rootSectionName != "")
            {
                ExitContext();
            }

            return _data;
        }

        private void VisitJObject(JObject? jObject)
        {
            foreach (var property in jObject?.Properties() ?? [])
            {
                EnterContext(property.Name);
                VisitProperty(property);
                ExitContext();
            }
        }

        private void VisitProperty(JProperty property)
        {
            VisitToken(property.Value);
        }

        private void VisitToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    VisitJObject(token.Value<JObject>());
                    break;

                case JTokenType.Array:
                    VisitArray(token.Value<JArray>());
                    break;

                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Bytes:
                case JTokenType.Raw:
                case JTokenType.Null:
                    VisitPrimitive(token.Value<JValue>());
                    break;

                case JTokenType.None:
                case JTokenType.Constructor:
                case JTokenType.Property:
                case JTokenType.Comment:
                case JTokenType.Undefined:
                case JTokenType.Date:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                default:
                    throw new NotSupportedException($"Unsupported JSON token '{token.Type}' was found");
            }
        }

        private void VisitArray(JArray? array)
        {
            if (array is null) return;
                
            for (var index = 0; index < array.Count; index++)
            {
                EnterContext(index.ToString());
                VisitToken(array[index]);
                ExitContext();
            }
        }

        private void VisitPrimitive(JValue? data)
        {
            if (data is null) return;
                
            var key = _currentPath;

            if (_data.ContainsKey(key))
            {
                throw new FormatException($"A duplicate key '{key}' was found.");
            }

            var stringValue = data.ToString(CultureInfo.InvariantCulture);

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