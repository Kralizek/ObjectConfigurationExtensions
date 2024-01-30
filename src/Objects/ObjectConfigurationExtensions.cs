using System;
using Kralizek.Extensions.Configuration.Internal;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.Configuration;

public static class ObjectConfigurationExtensions
{
    public static IConfigurationBuilder AddObject(this IConfigurationBuilder configurationBuilder, IConfigurationSerializer serializer, object? objectToAdd, string? rootSectionName = "")
    {
        if (objectToAdd is null)
        {
            return configurationBuilder;
        }
            
        if (rootSectionName == null)
        {
            throw new ArgumentNullException(nameof(rootSectionName));
        }

        configurationBuilder.Add(new ObjectConfigurationSource(serializer, objectToAdd, rootSectionName));

        return configurationBuilder;
    }
        
    public static IConfigurationBuilder AddObject(this IConfigurationBuilder configurationBuilder, object? objectToAdd, string? rootSectionName = "")
    {
        var serializer = new SystemTextJsonConfigurationSerializer();

        return AddObject(configurationBuilder, serializer, objectToAdd, rootSectionName);
    }
}