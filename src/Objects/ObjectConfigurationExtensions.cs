using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Kralizek.Extensions.Configuration.Internal;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.Configuration;

public static class ObjectConfigurationExtensions
{
    public static IConfigurationBuilder AddObject<T>(this IConfigurationBuilder configurationBuilder, T? objectToAdd, string? rootSectionName = "")
    {
        return AddObject(configurationBuilder, objectToAdd, rootSectionName, static value => SystemTextJsonConfigurationSerializer.Serialize(value));
    }

    public static IConfigurationBuilder AddObject<T>(this IConfigurationBuilder configurationBuilder, T? objectToAdd, JsonTypeInfo<T> jsonTypeInfo, string? rootSectionName = "")
    {
        if (jsonTypeInfo is null)
        {
            throw new ArgumentNullException(nameof(jsonTypeInfo));
        }

        return AddObject(configurationBuilder, objectToAdd, rootSectionName, value => SystemTextJsonConfigurationSerializer.Serialize(value, jsonTypeInfo));
    }

    public static IConfigurationBuilder AddObjectAsFallback<T>(this IConfigurationBuilder configurationBuilder, T? objectToAdd, string? rootSectionName = "")
    {
        return AddObjectAsFallback(configurationBuilder, objectToAdd, rootSectionName, static value => SystemTextJsonConfigurationSerializer.Serialize(value));
    }

    public static IConfigurationBuilder AddObjectAsFallback<T>(this IConfigurationBuilder configurationBuilder, T? objectToAdd, JsonTypeInfo<T> jsonTypeInfo, string? rootSectionName = "")
    {
        if (jsonTypeInfo is null)
        {
            throw new ArgumentNullException(nameof(jsonTypeInfo));
        }

        return AddObjectAsFallback(configurationBuilder, objectToAdd, rootSectionName, value => SystemTextJsonConfigurationSerializer.Serialize(value, jsonTypeInfo));
    }

    private static IConfigurationBuilder AddObject<T>(IConfigurationBuilder configurationBuilder, T? objectToAdd, string? rootSectionName, Func<T, JsonElement> serialize)
    {
        if (configurationBuilder is null)
        {
            throw new ArgumentNullException(nameof(configurationBuilder));
        }

        if (objectToAdd is null)
        {
            return configurationBuilder;
        }

        configurationBuilder.Add(CreateSource(objectToAdd, rootSectionName, serialize));

        return configurationBuilder;
    }

    private static IConfigurationBuilder AddObjectAsFallback<T>(IConfigurationBuilder configurationBuilder, T? objectToAdd, string? rootSectionName, Func<T, JsonElement> serialize)
    {
        if (configurationBuilder is null)
        {
            throw new ArgumentNullException(nameof(configurationBuilder));
        }

        if (objectToAdd is null)
        {
            return configurationBuilder;
        }

        configurationBuilder.Sources.Insert(0, CreateSource(objectToAdd, rootSectionName, serialize));

        return configurationBuilder;
    }

    private static ObjectConfigurationSource CreateSource<T>(T objectToAdd, string? rootSectionName, Func<T, JsonElement> serialize)
    {
        var rootSection = rootSectionName ?? string.Empty;

        return new ObjectConfigurationSource(() =>
        {
            var json = serialize(objectToAdd);

            return JsonConfigurationFlattener.Flatten(json, rootSection);
        });
    }
}
