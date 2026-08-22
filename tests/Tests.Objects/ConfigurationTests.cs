using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class ConfigurationTests
{
    [Test]
    public void Scalar_integer_is_bound()
    {
        var source = new ObjectWithScalarInt { Value = 42 };

        var result = Bind(source);

        Assert.That(result.Value, Is.EqualTo(42));
    }

    [Test]
    public void Nullable_integer_null_is_preserved()
    {
        var configuration = new ConfigurationBuilder()
            .AddObject(new ObjectWithScalarNullableInt { Value = 42 })
            .AddObject(new ObjectWithScalarNullableInt { Value = null })
            .Build();

        var result = configuration.Get<ObjectWithScalarNullableInt>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Value, Is.Null);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("hello")]
    public void String_scalar_is_bound(string? value)
    {
        var result = Bind(new ObjectWithScalarString { Value = value });

        Assert.That(result.Value, Is.EqualTo(value));
    }

    [Test]
    public void TimeSpan_scalar_is_bound()
    {
        var source = new ObjectWithScalarTimeSpan { Value = TimeSpan.FromMinutes(2.5) };

        var result = Bind(source);

        Assert.That(result.Value, Is.EqualTo(source.Value));
    }

    [TestCaseSource(nameof(IntListCases))]
    public void Integer_list_supports_multiple_arities(int[] values)
    {
        var result = Bind(new ObjectWithIntList { Values = [.. values] });

        Assert.That(result.Values, Is.EqualTo(values));
    }

    [Test]
    public void Nullable_integer_list_preserves_null_elements()
    {
        var result = Bind(new ObjectWithNullableIntList { Values = [1, null, 3] });

        Assert.That(result.Values, Is.EqualTo(new int?[] { 1, null, 3 }));
    }

    [Test]
    public void String_list_preserves_null_and_empty_elements()
    {
        var result = Bind(new ObjectWithStringList { Values = ["one", null, "", "four"] });

        Assert.That(result.Values, Is.EqualTo(new string?[] { "one", null, "", "four" }));
    }

    [Test]
    public void Integer_map_supports_multiple_entries()
    {
        var source = new ObjectWithIntMap
        {
            Values = new Dictionary<string, int>
            {
                ["one"] = 1,
                ["two"] = 2,
                ["three"] = 3
            }
        };

        var result = Bind(source);

        Assert.That(result.Values, Is.EqualTo(source.Values));
    }

    [Test]
    public void Nullable_integer_map_preserves_null_values()
    {
        var source = new ObjectWithNullableIntMap
        {
            Values = new Dictionary<string, int?>
            {
                ["one"] = 1,
                ["two"] = null,
                ["three"] = 3
            }
        };

        var result = Bind(source);

        Assert.That(result.Values, Is.EqualTo(source.Values));
    }

    [Test]
    public void String_map_preserves_null_and_empty_values()
    {
        var source = new ObjectWithStringMap
        {
            Values = new Dictionary<string, string?>
            {
                ["one"] = "first",
                ["two"] = null,
                ["three"] = ""
            }
        };

        var result = Bind(source);

        Assert.That(result.Values, Is.EqualTo(source.Values));
    }

    [Test]
    public void Empty_maps_are_bound_as_empty()
    {
        var result = Bind(new ObjectWithStringMap { Values = [] });

        Assert.That(result.Values, Is.Not.Null);
        Assert.That(result.Values, Is.Empty);
    }

    [Test]
    public void Two_scalars_are_bound_together()
    {
        var source = new ObjectWithTwoScalars { Count = 2, Name = "two" };

        var result = Bind(source);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Name, Is.EqualTo("two"));
    }

    [Test]
    public void Three_scalars_are_bound_together()
    {
        var source = new ObjectWithThreeScalars
        {
            Count = 3,
            Name = "three",
            Timeout = TimeSpan.FromSeconds(30)
        };

        var result = Bind(source);

        Assert.That(result.Count, Is.EqualTo(source.Count));
        Assert.That(result.Name, Is.EqualTo(source.Name));
        Assert.That(result.Timeout, Is.EqualTo(source.Timeout));
    }

    [Test]
    public void Two_collections_are_bound_together()
    {
        var source = new ObjectWithTwoCollections
        {
            Numbers = [1, 2, 3],
            Names = ["one", null, "three"]
        };

        var result = Bind(source);

        Assert.That(result.Numbers, Is.EqualTo(source.Numbers));
        Assert.That(result.Names, Is.EqualTo(source.Names));
    }

    [Test]
    public void Collection_and_map_are_bound_together()
    {
        var source = new ObjectWithCollectionAndMap
        {
            Names = ["one", "two"],
            Scores = new Dictionary<string, int>
            {
                ["one"] = 1,
                ["two"] = 2
            }
        };

        var result = Bind(source);

        Assert.That(result.Names, Is.EqualTo(source.Names));
        Assert.That(result.Scores, Is.EqualTo(source.Scores));
    }

    [Test]
    public void Nested_object_is_bound()
    {
        var source = new ObjectWithNestedObject
        {
            Nested = new ObjectWithThreeScalars
            {
                Count = 3,
                Name = "nested",
                Timeout = TimeSpan.FromSeconds(15)
            }
        };

        var result = Bind(source);

        Assert.That(result.Nested, Is.Not.Null);
        Assert.That(result.Nested!.Count, Is.EqualTo(source.Nested.Count));
        Assert.That(result.Nested.Name, Is.EqualTo(source.Nested.Name));
        Assert.That(result.Nested.Timeout, Is.EqualTo(source.Nested.Timeout));
    }

    [Test]
    public void Empty_list_is_bound_as_empty()
    {
        var result = Bind(new ObjectWithStringList { Values = [] });

        Assert.That(result.Values, Is.Not.Null);
        Assert.That(result.Values, Is.Empty);
    }

    [Test]
    public void Empty_list_does_not_remove_lower_precedence_child_keys()
    {
        var configuration = new ConfigurationBuilder()
            .AddObject(new ObjectWithStringList { Values = ["Initial"] })
            .AddObject(new ObjectWithStringList { Values = [] })
            .Build();

        var result = configuration.Get<ObjectWithStringList>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Values, Is.EqualTo(new[] { "Initial" }));
    }

    [Test]
    public void Fallback_values_do_not_override_later_providers()
    {
        var configuration = new ConfigurationBuilder()
            .AddObjectAsFallback(new ObjectWithTwoScalars { Count = 1, Name = null })
            .AddObject(new ObjectWithTwoScalars { Count = 2, Name = "configured" })
            .Build();

        var result = configuration.Get<ObjectWithTwoScalars>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Count, Is.EqualTo(2));
        Assert.That(result.Name, Is.EqualTo("configured"));
    }

    [Test]
    public void Source_generated_serialization_uses_the_same_semantics()
    {
        var source = new ObjectWithStringList { Values = ["one", null, ""] };

        var configuration = new ConfigurationBuilder()
            .AddObject(source, TestJsonContext.Default.ObjectWithStringList)
            .Build();

        var result = configuration.Get<ObjectWithStringList>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Values, Is.EqualTo(source.Values));
    }

    private static IEnumerable<int[]> IntListCases()
    {
        yield return [];
        yield return [1];
        yield return [1, 2];
        yield return [1, 2, 3];
    }

    private static T Bind<T>(T source)
        where T : class
    {
        var configuration = new ConfigurationBuilder()
            .AddObject(source)
            .Build();

        var result = configuration.Get<T>();

        Assert.That(result, Is.Not.Null);

        return result!;
    }
}
