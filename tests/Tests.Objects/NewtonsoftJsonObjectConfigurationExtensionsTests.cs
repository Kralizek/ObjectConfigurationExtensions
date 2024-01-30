using System;
using Kralizek.Extensions.Configuration.Internal;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class NewtonsoftJsonObjectConfigurationExtensionsTests
{
    [Test]
    [CustomInlineAutoData((object)null)]
    [CustomInlineAutoData]
    public void AddObject_returns_ConfigurationBuilder(object testSource, IConfigurationBuilder configurationBuilder, string rootSectionName)
    {
        var output = NewtonsoftJsonObjectConfigurationExtensions.AddObjectWithNewtonsoftJson(configurationBuilder, testSource, rootSectionName);

        Assert.That(output, Is.SameAs(configurationBuilder));
    }

    [Test, CustomAutoData]
    public void AddObject_configure_ConfigurationBuilder(IConfigurationBuilder configurationBuilder, object testSource, string rootSectionName)
    {
        NewtonsoftJsonObjectConfigurationExtensions.AddObjectWithNewtonsoftJson(configurationBuilder, testSource, rootSectionName);

        Mock.Get(configurationBuilder).Verify(p => p.Add(It.IsAny<ObjectConfigurationSource>()));
    }

    [Test, CustomAutoData]
    public void AddObject_does_nothing_when_source_is_null(IConfigurationBuilder configurationBuilder, string rootSectionName)
    {
        NewtonsoftJsonObjectConfigurationExtensions.AddObjectWithNewtonsoftJson(configurationBuilder, null!, rootSectionName);

        Mock.Get(configurationBuilder).Verify(p => p.Add(It.IsAny<IConfigurationSource>()), Times.Never);
    }

    [Test, CustomAutoData]
    public void AddObject_throws_if_rootSectionName_is_null(IConfigurationBuilder configurationBuilder, object testSource)
    {
        Assert.Throws<ArgumentNullException>(() => NewtonsoftJsonObjectConfigurationExtensions.AddObjectWithNewtonsoftJson(configurationBuilder, testSource, null));
    }
}