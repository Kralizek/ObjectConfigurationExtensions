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

        var result = builder.AddObject<ObjectWithTwoScalars>(null);

        Assert.That(result, Is.SameAs(builder));
        Assert.That(builder.Sources, Is.Empty);
    }

    [Test]
    public void AddObject_appends_provider_with_highest_precedence()
    {
        var builder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [nameof(ObjectWithTwoScalars.Name)] = "existing"
            });

        builder.AddObject(new ObjectWithTwoScalars { Name = "object", Count = 42 });

        var configuration = builder.Build();

        Assert.That(configuration[nameof(ObjectWithTwoScalars.Name)], Is.EqualTo("object"));
    }

    [Test]
    public void AddObjectAsFallback_inserts_provider_with_lowest_precedence()
    {
        var builder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [nameof(ObjectWithTwoScalars.Name)] = "existing"
            });

        builder.AddObjectAsFallback(new ObjectWithTwoScalars { Name = "fallback", Count = 42 });

        var configuration = builder.Build();

        Assert.That(configuration[nameof(ObjectWithTwoScalars.Name)], Is.EqualTo("existing"));
        Assert.That(configuration[nameof(ObjectWithTwoScalars.Count)], Is.EqualTo("42"));
    }

    [Test]
    public void JsonTypeInfo_overload_matches_reflection_overload()
    {
        var source = new ObjectWithTwoScalars { Name = "hello", Count = 42 };

        var reflectionConfiguration = new ConfigurationBuilder()
            .AddObject(source, "Root")
            .Build();

        var sourceGeneratedConfiguration = new ConfigurationBuilder()
            .AddObject(source, TestJsonContext.Default.ObjectWithTwoScalars, "Root")
            .Build();

        Assert.That(sourceGeneratedConfiguration["Root:Name"], Is.EqualTo(reflectionConfiguration["Root:Name"]));
        Assert.That(sourceGeneratedConfiguration["Root:Count"], Is.EqualTo(reflectionConfiguration["Root:Count"]));
    }

    [Test]
    public void Fallback_JsonTypeInfo_overload_matches_reflection_overload()
    {
        var source = new ObjectWithTwoScalars { Name = "fallback", Count = 42 };
        var values = new Dictionary<string, string?>
        {
            [nameof(ObjectWithTwoScalars.Name)] = "existing"
        };

        var reflectionConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .AddObjectAsFallback(source)
            .Build();

        var sourceGeneratedConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .AddObjectAsFallback(source, TestJsonContext.Default.ObjectWithTwoScalars)
            .Build();

        Assert.That(sourceGeneratedConfiguration[nameof(ObjectWithTwoScalars.Name)], Is.EqualTo(reflectionConfiguration[nameof(ObjectWithTwoScalars.Name)]));
        Assert.That(sourceGeneratedConfiguration[nameof(ObjectWithTwoScalars.Count)], Is.EqualTo(reflectionConfiguration[nameof(ObjectWithTwoScalars.Count)]));
    }

    [Test]
    public void Serialization_is_deferred_until_configuration_is_built()
    {
        var source = new ObjectWithTwoScalars { Name = "before", Count = 42 };
        var builder = new ConfigurationBuilder().AddObject(source);

        source.Name = "after";

        var configuration = builder.Build();

        Assert.That(configuration[nameof(ObjectWithTwoScalars.Name)], Is.EqualTo("after"));
    }
}
