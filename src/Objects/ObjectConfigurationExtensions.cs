using System;
using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;
using Kralizek.Extensions.Configuration.Internal;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.Configuration;

public static class ObjectConfigurationExtensions
{
    public static IConfigurationBuilder AddObject<T>(this IConfigurationBuilder configurationBuilder, T? objectToAdd, string? rootSectionName = "")
    {
        return AddObject(configurationBuilder, objectToAdd, rootSectionName, static (value, rootSection) => SystemTextJsonConfigurationSerializer.Serialize(value, rootSection));
    }

    public static IConfigurationBuilder AddObject<T>(this IConfigurationBuilder configurationBuilder, T? objectToAdd, JsonTypeInfo<T> jsonTypeInfo, string? rootSectionName = "")
    {
        ArgumentNullException.ThrowIfNull(jsonTypeInfo);

        return AddObject(configurationBuilder, objectToAdd, rootSectionName, (value, rootSection) => SystemTextJsonConfigurationSerializer.Serialize(value, jsonTypeInfo, rootSection));
    }

    public static IConfigurationBuilder AddObjectAsFallback<T>(this IConfigurationBuilder configurationBuilder, T? objectToAdd, string? rootSectionName = "")
    {
        return AddObjectAsFallback(configurationBuilder, objectToAdd, rootSectionName, static (value, rootSection) => SystemTextJsonConfigurationSerializer.Serialize(value, rootSection));
    }

    public static IConfigurationBuilder AddObjectAsFallback<T>(this IConfigurationBuilder configurationBuilder, T? objectToAdd, JsonTypeInfo<T> jsonTypeInfo, string? rootSectionName = "")
    {
        ArgumentNullException.ThrowIfNull(jsonTypeInfo);

        return AddObjectAsFallback(configurationBuilder, objectToAdd, rootSectionName, (value, rootSection) => SystemTextJsonConfigurationSerializer.Serialize(value, jsonTypeInfo, rootSection));
    }

    private static IConfigurationBuilder AddObject<T>(IConfigurationBuilder configurationBuilder, T? objectToAdd, string? rootSectionName, Func<T, string, IDictionary<string, string?>> serialize)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        if (objectToAdd is null)
        {
            return configurationBuilder;
        }

        configurationBuilder.Add(new ObjectConfigurationSource(() => serialize(objectToAdd, rootSectionName ?? string.Empty)));

        return configurationBuilder;
    }

    private static IConfigurationBuilder AddObjectAsFallback<T>(IConfigurationBuilder configurationBuilder, T? objectToAdd, string? rootSectionName, Func<T, string, IDictionary<string, string?>> serialize)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        if (objectToAdd is null)
        {
            return configurationBuilder;
        }

        configurationBuilder.Sources.Insert(0, new ObjectConfigurationSource(() => serialize(objectToAdd, rootSectionName ?? string.Empty)));

        return configurationBuilder;
    }
}
