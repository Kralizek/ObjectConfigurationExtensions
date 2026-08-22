using System.Text.Json.Serialization;

namespace Tests;

[JsonSerializable(typeof(ObjectWithStringList))]
[JsonSerializable(typeof(ObjectWithTwoScalars))]
internal partial class TestJsonContext : JsonSerializerContext;
