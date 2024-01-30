using System;
using Kralizek.Extensions.Configuration.Internal;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.Configuration;

public static class NewtonsoftJsonObjectConfigurationExtensions
{
    public static IConfigurationBuilder AddObjectWithNewtonsoftJson(this IConfigurationBuilder configurationBuilder, object? objectToAdd, string? rootSectionName = "")
    {
        var serializer = new NewtonsoftJsonConfigurationSerializer();

        return configurationBuilder.AddObject(serializer, objectToAdd, rootSectionName);
    }
}