using System.Text.Json;
using Kralizek.Extensions.Configuration.Internal;
using NUnit.Framework;

namespace Tests.Internal;

[TestFixture]
public class SystemTextJsonConfigurationSerializerTests
{
    [Test]
    public void Serialize_preserves_empty_array_as_json_array()
    {
        var result = SystemTextJsonConfigurationSerializer.Serialize(new ObjectWithStringArray { Values = [] });

        Assert.That(result.ValueKind, Is.EqualTo(JsonValueKind.Object));
        Assert.That(result.GetProperty(nameof(ObjectWithStringArray.Values)).ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(result.GetProperty(nameof(ObjectWithStringArray.Values)).GetArrayLength(), Is.Zero);
    }

    [Test]
    public void Serialize_preserves_null_and_empty_string_values()
    {
        var result = SystemTextJsonConfigurationSerializer.Serialize(new ObjectWithStringList { Values = [null, ""] });
        var values = result.GetProperty(nameof(ObjectWithStringList.Values));

        Assert.That(values[0].ValueKind, Is.EqualTo(JsonValueKind.Null));
        Assert.That(values[1].GetString(), Is.EqualTo(string.Empty));
    }

    [Test]
    public void JsonTypeInfo_serialization_matches_reflection_serialization()
    {
        var source = new ObjectWithStringList { Values = ["one", null, ""] };

        var reflection = SystemTextJsonConfigurationSerializer.Serialize(source);
        var sourceGenerated = SystemTextJsonConfigurationSerializer.Serialize(source, TestJsonContext.Default.ObjectWithStringList);

        Assert.That(sourceGenerated.GetRawText(), Is.EqualTo(reflection.GetRawText()));
    }
}
