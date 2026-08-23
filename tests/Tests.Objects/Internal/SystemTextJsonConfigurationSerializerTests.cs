using System.IO;
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
        using var stream = SystemTextJsonConfigurationSerializer.Serialize(new ObjectWithStringArray { Values = [] }, string.Empty);
        var result = Parse(stream);

        Assert.That(result.ValueKind, Is.EqualTo(JsonValueKind.Object));
        Assert.That(result.GetProperty(nameof(ObjectWithStringArray.Values)).ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(result.GetProperty(nameof(ObjectWithStringArray.Values)).GetArrayLength(), Is.Zero);
    }

    [Test]
    public void Serialize_preserves_null_and_empty_string_values()
    {
        using var stream = SystemTextJsonConfigurationSerializer.Serialize(new ObjectWithStringList { Values = [null, ""] }, string.Empty);
        var result = Parse(stream);
        var values = result.GetProperty(nameof(ObjectWithStringList.Values));

        Assert.That(values[0].ValueKind, Is.EqualTo(JsonValueKind.Null));
        Assert.That(values[1].GetString(), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Serialize_wraps_object_in_root_section()
    {
        using var stream = SystemTextJsonConfigurationSerializer.Serialize(new ObjectWithTwoScalars { Count = 2, Name = "rooted" }, "Settings");
        var result = Parse(stream);
        var settings = result.GetProperty("Settings");

        Assert.That(settings.GetProperty(nameof(ObjectWithTwoScalars.Count)).GetInt32(), Is.EqualTo(2));
        Assert.That(settings.GetProperty(nameof(ObjectWithTwoScalars.Name)).GetString(), Is.EqualTo("rooted"));
    }

    [Test]
    public void Serialize_wraps_object_in_multi_level_root_section()
    {
        using var stream = SystemTextJsonConfigurationSerializer.Serialize(new ObjectWithTwoScalars { Count = 3, Name = "payments" }, "Features:Payments");
        var result = Parse(stream);
        var payments = result.GetProperty("Features").GetProperty("Payments");

        Assert.That(payments.GetProperty(nameof(ObjectWithTwoScalars.Count)).GetInt32(), Is.EqualTo(3));
        Assert.That(payments.GetProperty(nameof(ObjectWithTwoScalars.Name)).GetString(), Is.EqualTo("payments"));
    }

    [Test]
    public void JsonTypeInfo_serialization_matches_reflection_serialization()
    {
        var source = new ObjectWithStringList { Values = ["one", null, ""] };

        using var reflectionStream = SystemTextJsonConfigurationSerializer.Serialize(source, "Root:Nested");
        using var sourceGeneratedStream = SystemTextJsonConfigurationSerializer.Serialize(source, TestJsonContext.Default.ObjectWithStringList, "Root:Nested");

        var reflection = Parse(reflectionStream);
        var sourceGenerated = Parse(sourceGeneratedStream);

        Assert.That(JsonElement.DeepEquals(sourceGenerated, reflection), Is.True);
    }

    private static JsonElement Parse(MemoryStream stream)
    {
        using var document = JsonDocument.Parse(stream);

        return document.RootElement.Clone();
    }
}
