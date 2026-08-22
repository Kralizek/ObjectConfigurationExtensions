using System.Text.Json;
using Kralizek.Extensions.Configuration.Internal;
using NUnit.Framework;

namespace Tests.Internal;

[TestFixture]
public class JsonConfigurationFlattenerTests
{
    [Test]
    public void Null_value_is_preserved()
    {
        var result = Flatten("""{ "Value": null }""");

        Assert.That(result, Contains.Key("Value"));
        Assert.That(result["Value"], Is.Null);
    }

    [Test]
    public void Empty_string_is_preserved()
    {
        var result = Flatten("""{ "Value": "" }""");

        Assert.That(result["Value"], Is.EqualTo(string.Empty));
    }

    [Test]
    public void Empty_array_is_represented_by_an_empty_value()
    {
        var result = Flatten("""{ "Values": [] }""");

        Assert.That(result["Values"], Is.EqualTo(string.Empty));
    }

    [Test]
    public void Empty_object_is_represented_by_a_null_value()
    {
        var result = Flatten("""{ "Value": {} }""");

        Assert.That(result, Contains.Key("Value"));
        Assert.That(result["Value"], Is.Null);
    }

    [Test]
    public void Null_array_elements_are_preserved()
    {
        var result = Flatten("""{ "Values": [null, "one"] }""");

        Assert.That(result, Contains.Key("Values:0"));
        Assert.That(result["Values:0"], Is.Null);
        Assert.That(result["Values:1"], Is.EqualTo("one"));
    }

    [Test]
    public void Root_section_is_applied_to_semantic_values()
    {
        var result = Flatten("""{ "Null": null, "Empty": [], "Text": "" }""", "Defaults");

        Assert.That(result["Defaults:Null"], Is.Null);
        Assert.That(result["Defaults:Empty"], Is.EqualTo(string.Empty));
        Assert.That(result["Defaults:Text"], Is.EqualTo(string.Empty));
    }

    private static IDictionary<string, string?> Flatten(string json, string rootSectionName = "")
    {
        using var document = JsonDocument.Parse(json);

        return JsonConfigurationFlattener.Flatten(document.RootElement, rootSectionName);
    }
}
