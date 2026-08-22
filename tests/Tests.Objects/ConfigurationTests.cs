using System;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class ConfigurationTests
{
    [Test, CustomAutoData]
    public void Object_is_added_to_configuration(ConfigurationBuilder configurationBuilder, ObjectWithSimpleProperties testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        Assert.That(configuration[nameof(testSource.Text)], Is.EqualTo($"{testSource.Text}"));
        Assert.That(configuration[nameof(testSource.Value)], Is.EqualTo($"{testSource.Value}"));
    }

    [Test, CustomAutoData]
    public void Object_is_added_to_configuration(ConfigurationBuilder configurationBuilder, ObjectWithInnerObject testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        Assert.That(configuration[$"{nameof(testSource.InnerObject)}:{nameof(testSource.InnerObject.Text)}"], Is.EqualTo($"{testSource.InnerObject.Text}"));
        Assert.That(configuration[$"{nameof(testSource.InnerObject)}:{nameof(testSource.InnerObject.Value)}"], Is.EqualTo($"{testSource.InnerObject.Value}"));
    }

    [Test, CustomAutoData]
    public void Object_is_added_to_configuration(ConfigurationBuilder configurationBuilder, ObjectWithSimpleStringArray testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        Assert.That(configuration[$"{nameof(testSource.Texts)}:0"], Is.EqualTo($"{testSource.Texts[0]}"));
    }

    [Test, CustomAutoData]
    public void Object_is_added_to_configuration(ConfigurationBuilder configurationBuilder, ObjectWithSimpleIntArray testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        Assert.That(configuration[$"{nameof(testSource.Values)}:0"], Is.EqualTo($"{testSource.Values[0]}"));
    }

    [Test, CustomAutoData]
    public void Object_is_added_to_configuration(ConfigurationBuilder configurationBuilder, ObjectWithComplexArray testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        Assert.That(configuration[$"{nameof(testSource.Items)}:0:Text"], Is.EqualTo($"{testSource.Items[0].Text}"));
        Assert.That(configuration[$"{nameof(testSource.Items)}:0:Value"], Is.EqualTo($"{testSource.Items[0].Value}"));
    }

    [Test, CustomAutoData]
    public void Object_can_be_retrieved_from_configuration(ConfigurationBuilder configurationBuilder, ObjectWithSimpleProperties testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        var result = configuration.Get<ObjectWithSimpleProperties>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Text, Is.EqualTo(testSource.Text));
        Assert.That(result.Value, Is.EqualTo(testSource.Value));
    }

    [Test, CustomAutoData]
    public void Object_can_be_retrieved_from_configuration(ConfigurationBuilder configurationBuilder, ObjectWithInnerObject testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        var result = configuration.Get<ObjectWithInnerObject>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.InnerObject.Text, Is.EqualTo(testSource.InnerObject.Text));
        Assert.That(result.InnerObject.Value, Is.EqualTo(testSource.InnerObject.Value));
    }

    [Test, CustomAutoData]
    public void Object_can_be_retrieved_from_configuration(ConfigurationBuilder configurationBuilder, ObjectWithSimpleStringArray testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        var result = configuration.Get<ObjectWithSimpleStringArray>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Texts, Is.EquivalentTo(testSource.Texts));
    }

    [Test, CustomAutoData]
    public void Object_can_be_retrieved_from_configuration(ConfigurationBuilder configurationBuilder, ObjectWithSimpleIntArray testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        var result = configuration.Get<ObjectWithSimpleIntArray>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Values, Is.EquivalentTo(testSource.Values));
    }

    [Test, CustomAutoData]
    public void Object_can_be_retrieved_from_configuration(ConfigurationBuilder configurationBuilder, ObjectWithComplexArray testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        var result = configuration.Get<ObjectWithComplexArray>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Items, Is.EquivalentTo(testSource.Items).Using((Func<ObjectWithSimpleProperties, ObjectWithSimpleProperties, bool>)Comparison));
    }

    [Test]
    public void Null_values_override_existing_values()
    {
        var configuration = new ConfigurationBuilder()
            .AddObject(new ObjectWithNullableValues
            {
                Text = "Initial",
                Number = 123
            })
            .AddObject(new ObjectWithNullableValues
            {
                Text = null,
                Number = null
            })
            .Build();

        var result = configuration.Get<ObjectWithNullableValues>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Text, Is.Null);
        Assert.That(result.Number, Is.Null);
    }

    [Test]
    public void Empty_arrays_are_bound_as_empty()
    {
        var configuration = new ConfigurationBuilder()
            .AddObject(new ObjectWithNullableValues
            {
                EmptyValues = []
            })
            .Build();

        var result = configuration.Get<ObjectWithNullableValues>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.EmptyValues, Is.Not.Null);
        Assert.That(result.EmptyValues, Is.Empty);
    }

    [Test]
    public void Empty_arrays_follow_configuration_child_key_merge_semantics()
    {
        var configuration = new ConfigurationBuilder()
            .AddObject(new ObjectWithNullableValues
            {
                EmptyValues = ["Initial"]
            })
            .AddObject(new ObjectWithNullableValues
            {
                EmptyValues = []
            })
            .Build();

        var result = configuration.Get<ObjectWithNullableValues>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.EmptyValues, Is.EqualTo(new[] { "Initial" }));
    }

    [Test]
    public void Null_array_elements_are_bound()
    {
        var configuration = new ConfigurationBuilder()
            .AddObject(new ObjectWithNullableValues
            {
                Values = [null, "one"]
            })
            .Build();

        var result = configuration.Get<ObjectWithNullableValues>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Values, Is.EqualTo(new string?[] { null, "one" }));
    }

    [Test]
    public void Source_generated_serialization_preserves_modern_semantics()
    {
        var source = new ObjectWithNullableValues
        {
            Text = null,
            Number = null,
            Values = [null, "one"],
            EmptyValues = []
        };

        var configuration = new ConfigurationBuilder()
            .AddObject(source, TestJsonContext.Default.ObjectWithNullableValues)
            .Build();

        var result = configuration.Get<ObjectWithNullableValues>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Text, Is.Null);
        Assert.That(result.Number, Is.Null);
        Assert.That(result.Values, Is.EqualTo(new string?[] { null, "one" }));
        Assert.That(result.EmptyValues, Is.Not.Null);
        Assert.That(result.EmptyValues, Is.Empty);
    }

    [Test]
    public void Fallback_semantic_values_do_not_override_later_providers()
    {
        var configuration = new ConfigurationBuilder()
            .AddObjectAsFallback(new ObjectWithNullableValues
            {
                Text = null,
                EmptyValues = []
            })
            .AddObject(new ObjectWithNullableValues
            {
                Text = "Configured",
                EmptyValues = ["Configured"]
            })
            .Build();

        var result = configuration.Get<ObjectWithNullableValues>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Text, Is.EqualTo("Configured"));
        Assert.That(result.EmptyValues, Is.EqualTo(new[] { "Configured" }));
    }

    private static bool Comparison(ObjectWithSimpleProperties first, ObjectWithSimpleProperties second) => first.Text == second.Text && first.Value == second.Value;
}
