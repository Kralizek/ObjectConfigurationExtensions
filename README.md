# ObjectConfigurationExtensions

[![CI](https://github.com/Kralizek/ObjectConfigurationExtensions/actions/workflows/ci.yml/badge.svg)](https://github.com/Kralizek/ObjectConfigurationExtensions/actions/workflows/ci.yml)
[![GitHub Release](https://img.shields.io/github/v/release/Kralizek/ObjectConfigurationExtensions)](https://github.com/Kralizek/ObjectConfigurationExtensions/releases/latest)
[![Stable](https://img.shields.io/nuget/v/Kralizek.Extensions.Configuration.Objects?label=stable)](https://www.nuget.org/packages/Kralizek.Extensions.Configuration.Objects)
[![Latest](https://img.shields.io/nuget/vpre/Kralizek.Extensions.Configuration.Objects?label=latest)](https://www.nuget.org/packages/Kralizek.Extensions.Configuration.Objects)
[![Downloads](https://img.shields.io/nuget/dt/Kralizek.Extensions.Configuration.Objects?label=downloads)](https://www.nuget.org/packages/Kralizek.Extensions.Configuration.Objects)

ObjectConfigurationExtensions lets you add a concrete object directly to `Microsoft.Extensions.Configuration`.

The library supports object graphs containing scalar values, nested objects, collections, and dictionaries, and targets both `netstandard2.0` and `net10.0`.

## Install

```bash
dotnet add package Kralizek.Extensions.Configuration.Objects
```

## Add an object to configuration

`AddObject` follows the normal configuration-provider convention: the object is added with higher precedence than providers registered before it.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddObject(new
{
    Value = 123,
    ManyValues = new ComplexObject[]
    {
        new("New value", 234),
        new("Another value", 345)
    },
    Flag = true,
    Text = "Something"
}, "Test");

var app = builder.Build();

app.MapGet("/", (IConfiguration configuration) => configuration
    .GetSection("Test")
    .AsEnumerable()
    .OrderBy(c => c.Key)
    .ToDictionary(c => c.Key, v => v.Value));

app.Run();

public record ComplexObject(string Text, int Number);
```

## Root section name

The root section name is optional. To add the properties directly to the root configuration:

```csharp
builder.Configuration.AddObject(new
{
    IsEnabled = false
});
```

Configuration paths can also be used as root section names:

```csharp
builder.Configuration.AddObject(
    new { Enabled = true },
    "Features:Payments");
```

The value is then available as `Features:Payments:Enabled`.

## Use an object as fallback configuration

Use `AddObjectAsFallback` when the object contains defaults that should be overridden by the configuration already registered by the host:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddObjectAsFallback(new
{
    FeatureEnabled = false,
    RetryCount = 3
});
```

The fallback is inserted at the lowest precedence, so `appsettings.json`, environment variables, command-line arguments, and any other higher-precedence providers can override those values. This is true even when `AddObjectAsFallback` is called after those providers have already been registered.

## Source-generated System.Text.Json metadata

Both `AddObject` and `AddObjectAsFallback` have overloads accepting `JsonTypeInfo<T>`. Use them when reflection-based System.Text.Json serialization is not appropriate, including trimming and Native AOT scenarios.

```csharp
[JsonSerializable(typeof(MySettings))]
internal partial class AppJsonContext : JsonSerializerContext;

builder.Configuration.AddObject(
    new MySettings { FeatureEnabled = true },
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

## Versioning and prereleases

The library follows Semantic Versioning. Stable releases and public prereleases are published to NuGet.org.

## Building

The repository uses the .NET SDK directly.

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet pack --configuration Release
```
