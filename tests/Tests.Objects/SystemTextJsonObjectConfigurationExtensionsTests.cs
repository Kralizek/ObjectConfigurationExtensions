using System;
using Kralizek.Extensions.Configuration.Internal;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod

namespace Tests;

[TestFixture]
public class SystemTextJsonObjectConfigurationExtensionsTests
{
    [Test]
    [CustomInlineAutoData((object)null)]
    [CustomInlineAutoData()]
    public void AddObject_returns_ConfigurationBuilder(object testSource, IConfigurationBuilder configurationBuilder, string rootSectionName)
    {
        var output = ObjectConfigurationExtensions.AddObject(configurationBuilder, testSource, rootSectionName);

        Assert.That(output, Is.SameAs(configurationBuilder));
    }

    [Test, CustomAutoData]
    public void AddObject_configure_ConfigurationBuilder(IConfigurationBuilder configurationBuilder, object testSource, string rootSectionName)
    {
        ObjectConfigurationExtensions.AddObject(configurationBuilder, testSource, rootSectionName);

        Mock.Get(configurationBuilder).Verify(p => p.Add(It.IsAny<ObjectConfigurationSource>()));
    }

    [Test, CustomAutoData]
    public void AddObject_does_nothing_when_source_is_null(IConfigurationBuilder configurationBuilder, string rootSectionName)
    {
        ObjectConfigurationExtensions.AddObject(configurationBuilder, null!, rootSectionName);

        Mock.Get(configurationBuilder).Verify(p => p.Add(It.IsAny<IConfigurationSource>()), Times.Never);
    }
}