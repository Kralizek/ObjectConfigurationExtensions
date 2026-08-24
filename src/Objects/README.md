# Kralizek.Extensions.Configuration.Objects

Add strongly typed objects directly to a `Microsoft.Extensions.Configuration` pipeline.

The library supports object graphs containing scalar values, nested objects, collections, and dictionaries.

## Install

```bash
dotnet add package Kralizek.Extensions.Configuration.Objects
```

## Add an object

```csharp
builder.Configuration.AddObject(new
{
    FeatureEnabled = true,
    RetryCount = 3
});
```

`AddObject` appends the object configuration source, so it has higher precedence than configuration providers already registered.

## Root section name

You can place the object under a root section:

```csharp
builder.Configuration.AddObject(settings, "MySettings");
```

The root section can also be a configuration path:

```csharp
builder.Configuration.AddObject(settings, "Features:Payments");
```

This makes the object available under `Features:Payments`.

## Add fallback defaults

Use `AddObjectAsFallback` when the object should provide defaults that the rest of the configuration pipeline can override:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddObjectAsFallback(new
{
    FeatureEnabled = false,
    RetryCount = 3
});
```

The fallback source is inserted at the lowest precedence, even when other providers are already registered. Values from `appsettings.json`, environment variables, command-line arguments, and other higher-precedence providers can override the defaults.

## Source-generated System.Text.Json

Both APIs have overloads accepting `JsonTypeInfo<T>`. Use them with System.Text.Json source generation when reflection-based serialization is not appropriate, including trimming and Native AOT scenarios.

```csharp
[JsonSerializable(typeof(MySettings))]
internal partial class AppJsonContext : JsonSerializerContext;

builder.Configuration.AddObject(
    new MySettings { FeatureEnabled = true },
    AppJsonContext.Default.MySettings);
```

The same overload is available for fallback configuration:

```csharp
builder.Configuration.AddObjectAsFallback(
    defaults,
    AppJsonContext.Default.MySettings);
```

## API

```csharp
IConfigurationBuilder AddObject<T>(
    T? value,
    string? rootSectionName = "");

IConfigurationBuilder AddObject<T>(
    T? value,
    JsonTypeInfo<T> jsonTypeInfo,
    string? rootSectionName = "");

IConfigurationBuilder AddObjectAsFallback<T>(
    T? value,
    string? rootSectionName = "");

IConfigurationBuilder AddObjectAsFallback<T>(
    T? value,
    JsonTypeInfo<T> jsonTypeInfo,
    string? rootSectionName = "");
```

For source code, examples, and release notes, see the GitHub repository: https://github.com/Kralizek/ObjectConfigurationExtensions
