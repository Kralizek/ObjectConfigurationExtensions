using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class ObjectConfigurationIntegrationTests
{
    [Test]
    public void Added_object_exposes_expected_configuration_keys()
    {
        var source = new IntegrationSource
        {
            Count = 42,
            Name = "example",
            Timeout = TimeSpan.FromSeconds(15),
            Numbers = [1, 2, 3],
            Names = ["one", null, ""],
            Scores = new Dictionary<string, int>
            {
                ["first"] = 10,
                ["second"] = 20
            },
            Labels = new Dictionary<string, string?>
            {
                ["first"] = "alpha",
                ["second"] = null,
                ["third"] = ""
            }
        };

        var configuration = new ConfigurationBuilder()
            .AddObject(source, "Settings")
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(configuration["Settings:Count"], Is.EqualTo("42"));
            Assert.That(configuration["Settings:Name"], Is.EqualTo("example"));
            Assert.That(configuration["Settings:Timeout"], Is.EqualTo("00:00:15"));
            Assert.That(configuration["Settings:Numbers:0"], Is.EqualTo("1"));
            Assert.That(configuration["Settings:Numbers:1"], Is.EqualTo("2"));
            Assert.That(configuration["Settings:Numbers:2"], Is.EqualTo("3"));
            Assert.That(configuration["Settings:Names:0"], Is.EqualTo("one"));
            Assert.That(configuration["Settings:Names:1"], Is.Null);
            Assert.That(configuration["Settings:Names:2"], Is.EqualTo(string.Empty));
            Assert.That(configuration["Settings:Scores:first"], Is.EqualTo("10"));
            Assert.That(configuration["Settings:Scores:second"], Is.EqualTo("20"));
            Assert.That(configuration["Settings:Labels:first"], Is.EqualTo("alpha"));
            Assert.That(configuration["Settings:Labels:second"], Is.Null);
            Assert.That(configuration["Settings:Labels:third"], Is.EqualTo(string.Empty));
        });
    }

    [Test]
    public void Added_object_can_bind_to_an_equivalent_different_type()
    {
        var source = new IntegrationSource
        {
            Count = 7,
            Name = "source",
            Timeout = TimeSpan.FromMinutes(2),
            Numbers = [3, 5, 8],
            Names = ["three", null, "eight"],
            Scores = new Dictionary<string, int>
            {
                ["three"] = 3,
                ["five"] = 5
            },
            Labels = new Dictionary<string, string?>
            {
                ["primary"] = "blue",
                ["secondary"] = null
            }
        };

        var configuration = new ConfigurationBuilder()
            .AddObject(source, "Settings")
            .Build();

        var result = configuration
            .GetSection("Settings")
            .Get<IntegrationTarget>();

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Count, Is.EqualTo(source.Count));
            Assert.That(result.Name, Is.EqualTo(source.Name));
            Assert.That(result.Timeout, Is.EqualTo(source.Timeout));
            Assert.That(result.Numbers, Is.EqualTo(source.Numbers));
            Assert.That(result.Names, Is.EqualTo(source.Names));
            Assert.That(result.Scores, Is.EqualTo(source.Scores));
            Assert.That(result.Labels, Is.EqualTo(source.Labels));
        });
    }

    [Test]
    public void Multi_level_root_section_exposes_expected_configuration_keys()
    {
        var source = new ObjectWithTwoScalars
        {
            Count = 3,
            Name = "payments"
        };

        var configuration = new ConfigurationBuilder()
            .AddObject(source, "Features:Payments")
            .Build();

        Assert.Multiple(() =>
        {
            Assert.That(configuration["Features:Payments:Count"], Is.EqualTo("3"));
            Assert.That(configuration["Features:Payments:Name"], Is.EqualTo("payments"));
        });
    }

    [Test]
    public void Multi_level_root_section_with_json_type_info_binds_from_the_expected_section()
    {
        var source = new ObjectWithTwoScalars
        {
            Count = 5,
            Name = "source-generated"
        };

        var configuration = new ConfigurationBuilder()
            .AddObject(source, TestJsonContext.Default.ObjectWithTwoScalars, "Features:Payments")
            .Build();

        var result = configuration
            .GetSection("Features:Payments")
            .Get<ObjectWithTwoScalars>();

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Count, Is.EqualTo(source.Count));
            Assert.That(result.Name, Is.EqualTo(source.Name));
        });
    }

    [Test]
    public void Nested_object_can_bind_to_an_equivalent_different_type()
    {
        var source = new IntegrationNestedSource
        {
            Child = new IntegrationSource
            {
                Count = 5,
                Name = "nested",
                Timeout = TimeSpan.FromSeconds(30),
                Numbers = [1, 1, 2, 3, 5],
                Names = ["one", "two"],
                Scores = new Dictionary<string, int> { ["score"] = 99 },
                Labels = new Dictionary<string, string?> { ["label"] = "value" }
            }
        };

        var configuration = new ConfigurationBuilder()
            .AddObject(source)
            .Build();

        var result = configuration.Get<IntegrationNestedTarget>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Child, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Child!.Count, Is.EqualTo(source.Child!.Count));
            Assert.That(result.Child.Name, Is.EqualTo(source.Child.Name));
            Assert.That(result.Child.Timeout, Is.EqualTo(source.Child.Timeout));
            Assert.That(result.Child.Numbers, Is.EqualTo(source.Child.Numbers));
            Assert.That(result.Child.Names, Is.EqualTo(source.Child.Names));
            Assert.That(result.Child.Scores, Is.EqualTo(source.Child.Scores));
            Assert.That(result.Child.Labels, Is.EqualTo(source.Child.Labels));
        });
    }

    [Test]
    public void Fallback_object_is_overridden_by_later_configuration()
    {
        var fallback = new IntegrationTarget
        {
            Count = 1,
            Name = "fallback"
        };

        var configuration = new ConfigurationBuilder()
            .AddObjectAsFallback(fallback)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [nameof(IntegrationTarget.Name)] = "configured"
            })
            .Build();

        var result = configuration.Get<IntegrationTarget>();

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Count, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("configured"));
        });
    }

    private sealed class IntegrationSource
    {
        public int Count { get; set; }

        public string? Name { get; set; }

        public TimeSpan Timeout { get; set; }

        public int[]? Numbers { get; set; }

        public string?[]? Names { get; set; }

        public Dictionary<string, int>? Scores { get; set; }

        public Dictionary<string, string?>? Labels { get; set; }
    }

    private sealed class IntegrationTarget
    {
        public int Count { get; set; }

        public string? Name { get; set; }

        public TimeSpan Timeout { get; set; }

        public int[]? Numbers { get; set; }

        public string?[]? Names { get; set; }

        public Dictionary<string, int>? Scores { get; set; }

        public Dictionary<string, string?>? Labels { get; set; }
    }

    private sealed class IntegrationNestedSource
    {
        public IntegrationSource? Child { get; set; }
    }

    private sealed class IntegrationNestedTarget
    {
        public IntegrationTarget? Child { get; set; }
    }
}
