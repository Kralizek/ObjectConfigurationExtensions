using System.Text.Json.Serialization;

namespace Tests;

[JsonSerializable(typeof(ObjectWithStringList))]
internal partial class TestJsonContext : JsonSerializerContext;
