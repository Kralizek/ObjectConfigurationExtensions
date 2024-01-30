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

        Assert.That(result.Text, Is.EqualTo(testSource.Text));
        Assert.That(result.Value, Is.EqualTo(testSource.Value));
    }

    [Test, CustomAutoData]
    public void Object_can_be_retrieved_from_configuration(ConfigurationBuilder configurationBuilder, ObjectWithInnerObject testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        var result = configuration.Get<ObjectWithInnerObject>();

        Assert.That(result.InnerObject.Text, Is.EqualTo(testSource.InnerObject.Text));
        Assert.That(result.InnerObject.Value, Is.EqualTo(testSource.InnerObject.Value));
    }

    [Test, CustomAutoData]
    public void Object_can_be_retrieved_from_configuration(ConfigurationBuilder configurationBuilder, ObjectWithSimpleStringArray testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        var result = configuration.Get<ObjectWithSimpleStringArray>();

        Assert.That(result.Texts, Is.EquivalentTo(testSource.Texts));
    }

    [Test, CustomAutoData]
    public void Object_can_be_retrieved_from_configuration(ConfigurationBuilder configurationBuilder, ObjectWithSimpleIntArray testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        var result = configuration.Get<ObjectWithSimpleIntArray>();

        Assert.That(result.Values, Is.EquivalentTo(testSource.Values));
    }

    [Test, CustomAutoData]
    public void Object_can_be_retrieved_from_configuration(ConfigurationBuilder configurationBuilder, ObjectWithComplexArray testSource)
    {
        configurationBuilder.AddObject(testSource);

        var configuration = configurationBuilder.Build();

        var result = configuration.Get<ObjectWithComplexArray>();

        Assert.That(result.Items, Is.EquivalentTo(testSource.Items).Using((Func<ObjectWithSimpleProperties, ObjectWithSimpleProperties, bool>)Comparison));
    }

    bool Comparison(ObjectWithSimpleProperties first, ObjectWithSimpleProperties second) => first.Text == second.Text && first.Value == second.Value;

        
    [Test, CustomAutoData]
    [Property("Issue", "3")]
    public void Null_values_should_not_override_existing_values(ConfigurationBuilder configurationBuilder, ObjectWithSimpleProperties testSource)
    {
        configurationBuilder.AddObject(testSource);

        configurationBuilder.AddObject(new ObjectWithSimpleProperties{ Text = null, Value = testSource.Value });

        var configuration = configurationBuilder.Build();

        var result = configuration.Get<ObjectWithSimpleProperties>();

        Assert.That(result.Text, Is.EqualTo(testSource.Text));
        Assert.That(result.Value, Is.EqualTo(testSource.Value));
    }

    [Test, CustomAutoData]
    [Property("Issue", "3")]
    public void Null_values_should_not_override_existing_values(ConfigurationBuilder configurationBuilder, ObjectWithSimpleIntArray testSource)
    {
        configurationBuilder.AddObject(testSource);

        configurationBuilder.AddObject(new ObjectWithSimpleIntArray { Values = new int[0] });

        var configuration = configurationBuilder.Build();

        var result = configuration.Get<ObjectWithSimpleIntArray>();

        Assert.That(result.Values, Is.EquivalentTo(testSource.Values));
    }
}