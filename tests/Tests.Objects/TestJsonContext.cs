using System.Text.Json.Serialization;

namespace Tests;

[JsonSerializable(typeof(ObjectWithSimpleProperties))]
internal partial class TestJsonContext : JsonSerializerContext;
