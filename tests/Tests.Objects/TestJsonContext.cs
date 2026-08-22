using System.Text.Json.Serialization;

namespace Tests;

[JsonSerializable(typeof(ObjectWithSimpleProperties))]
[JsonSerializable(typeof(ObjectWithNullableValues))]
internal partial class TestJsonContext : JsonSerializerContext;
