using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class SystemTextJsonObjectConfigurationExtensionsTests
{
    [Test]
    public void AddObject_returns_same_builder_when_source_is_null()
    {
        var builder = new ConfigurationBuilder();

        var result = builder.AddObject<ObjectWithSimpleProperties>(null);

        Assert.That(result, Is.SameAs(builder));
        Assert.That(builder.Sources, Is.Empty);
    }

    [Test]
    public void AddObject_appends_provider_with_highest_precedence()
    {
        var builder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [nameof(ObjectWithSimpleProperties.Text)] = "existing"
            });

        builder.AddObject(new ObjectWithSimpleProperties { Text = "object", Value = 42 });

        var configuration = builder.Build();

        Assert.That(configuration[nameof(ObjectWithSimpleProperties.Text)], Is.EqualTo("object"));
    }

    [Test]
    public void AddObjectAsFallback_inserts_provider_with_lowest_precedence()
    {
        var builder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [nameof(ObjectWithSimpleProperties.Text)] = "existing"
            });

        builder.AddObjectAsFallback(new ObjectWithSimpleProperties { Text = "fallback", Value = 42 });

        var configuration = builder.Build();

        Assert.That(configuration[nameof(ObjectWithSimpleProperties.Text)], Is.EqualTo("existing"));
        Assert.That(configuration[nameof(ObjectWithSimpleProperties.Value)], Is.EqualTo("42"));
    }

    [Test]
    public void JsonTypeInfo_overload_matches_reflection_overload()
    {
        var source = new ObjectWithSimpleProperties { Text = "hello", Value = 42 };

        var reflectionConfiguration = new ConfigurationBuilder()
            .AddObject(source, "Root")
            .Build();

        var sourceGeneratedConfiguration = new ConfigurationBuilder()
            .AddObject(source, TestJsonContext.Default.ObjectWithSimpleProperties, "Root")
            .Build();

        Assert.That(sourceGeneratedConfiguration["Root:Text"], Is.EqualTo(reflectionConfiguration["Root:Text"]));
        Assert.That(sourceGeneratedConfiguration["Root:Value"], Is.EqualTo(reflectionConfiguration["Root:Value"]));
    }

    [Test]
    public void Fallback_JsonTypeInfo_overload_matches_reflection_overload()
    {
        var source = new ObjectWithSimpleProperties { Text = "fallback", Value = 42 };
        var values = new Dictionary<string, string?>
        {
            [nameof(ObjectWithSimpleProperties.Text)] = "existing"
        };

        var reflectionConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .AddObjectAsFallback(source)
            .Build();

        var sourceGeneratedConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .AddObjectAsFallback(source, TestJsonContext.Default.ObjectWithSimpleProperties)
            .Build();

        Assert.That(sourceGeneratedConfiguration[nameof(ObjectWithSimpleProperties.Text)], Is.EqualTo(reflectionConfiguration[nameof(ObjectWithSimpleProperties.Text)]));
        Assert.That(sourceGeneratedConfiguration[nameof(ObjectWithSimpleProperties.Value)], Is.EqualTo(reflectionConfiguration[nameof(ObjectWithSimpleProperties.Value)]));
    }

    [Test]
    public void Serialization_is_deferred_until_configuration_is_built()
    {
        var source = new ObjectWithSimpleProperties { Text = "before", Value = 42 };
        var builder = new ConfigurationBuilder().AddObject(source);

        source.Text = "after";

        var configuration = builder.Build();

        Assert.That(configuration[nameof(ObjectWithSimpleProperties.Text)], Is.EqualTo("after"));
    }
}
