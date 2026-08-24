using System;
using System.IO;
using System.Text.Json.Serialization.Metadata;
using Kralizek.Extensions.Configuration.Internal;
using Microsoft.Extensions.Configuration.Json;

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
        if (jsonTypeInfo is null)
        {
            throw new ArgumentNullException(nameof(jsonTypeInfo));
        }

        return AddObject(configurationBuilder, objectToAdd, rootSectionName, (value, rootSection) => SystemTextJsonConfigurationSerializer.Serialize(value, jsonTypeInfo, rootSection));
    }

    public static IConfigurationBuilder AddObjectAsFallback<T>(this IConfigurationBuilder configurationBuilder, T? objectToAdd, string? rootSectionName = "")
    {
        return AddObjectAsFallback(configurationBuilder, objectToAdd, rootSectionName, static (value, rootSection) => SystemTextJsonConfigurationSerializer.Serialize(value, rootSection));
    }

    public static IConfigurationBuilder AddObjectAsFallback<T>(this IConfigurationBuilder configurationBuilder, T? objectToAdd, JsonTypeInfo<T> jsonTypeInfo, string? rootSectionName = "")
    {
        if (jsonTypeInfo is null)
        {
            throw new ArgumentNullException(nameof(jsonTypeInfo));
        }

        return AddObjectAsFallback(configurationBuilder, objectToAdd, rootSectionName, (value, rootSection) => SystemTextJsonConfigurationSerializer.Serialize(value, jsonTypeInfo, rootSection));
    }

    private static IConfigurationBuilder AddObject<T>(IConfigurationBuilder configurationBuilder, T? objectToAdd, string? rootSectionName, Func<T, string, Stream> serialize)
    {
        if (configurationBuilder is null)
        {
            throw new ArgumentNullException(nameof(configurationBuilder));
        }

        if (objectToAdd is null)
        {
            return configurationBuilder;
        }

        configurationBuilder.Sources.Add(CreateSource(objectToAdd, rootSectionName, serialize));

        return configurationBuilder;
    }

    private static IConfigurationBuilder AddObjectAsFallback<T>(IConfigurationBuilder configurationBuilder, T? objectToAdd, string? rootSectionName, Func<T, string, Stream> serialize)
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

    private static JsonStreamConfigurationSource CreateSource<T>(T objectToAdd, string? rootSectionName, Func<T, string, Stream> serialize)
    {
        return new JsonStreamConfigurationSource
        {
            Stream = serialize(objectToAdd, rootSectionName ?? string.Empty)
        };
    }
}
