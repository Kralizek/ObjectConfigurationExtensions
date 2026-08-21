[![Stable](https://img.shields.io/nuget/v/Kralizek.Extensions.Configuration.Objects.svg?label=stable)](https://www.nuget.org/packages/Kralizek.Extensions.Configuration.Objects) [![Latest](https://img.shields.io/nuget/vpre/Kralizek.Extensions.Configuration.Objects.svg?label=latest)](https://www.nuget.org/packages/Kralizek.Extensions.Configuration.Objects) [![NuGet downloads](https://img.shields.io/nuget/dt/Kralizek.Extensions.Configuration.Objects.svg)](https://www.nuget.org/packages/Kralizek.Extensions.Configuration.Objects) [![CI](https://github.com/Kralizek/ObjectConfigurationExtensions/actions/workflows/ci.yml/badge.svg)](https://github.com/Kralizek/ObjectConfigurationExtensions/actions/workflows/ci.yml)

# ObjectConfigurationExtensions

ObjectConfigurationExtensions is a configuration provider for `Microsoft.Extensions.Configuration` that lets you add a concrete object directly to the configuration pipeline.

The library supports primitive values, complex objects, and sequences, and targets both `netstandard2.0` and `net10.0`.

## Install

```bash
dotnet add package Kralizek.Extensions.Configuration.Objects
```

## Add an object to configuration

`AddObject` follows the normal configuration-provider convention: the object provider is appended to the pipeline, so it has higher precedence than providers registered before it.

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

## Use an object as fallback configuration

Use `AddObjectAsFallback` when the object contains defaults that should be overridden by the rest of the configuration pipeline:

```csharp
builder.Configuration
    .AddObjectAsFallback(new
    {
        FeatureEnabled = false,
        RetryCount = 3
    })
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();
```

The fallback provider is inserted at the beginning of the provider chain, so later providers win.

## Source-generated System.Text.Json metadata

Both `AddObject` and `AddObjectAsFallback` have overloads accepting `JsonTypeInfo<T>`. Use them when reflection-based System.Text.Json serialization is not appropriate, including trimming and Native AOT scenarios.

```csharp
[JsonSerializable(typeof(MySettings))]
internal partial class AppJsonContext : JsonSerializerContext;

builder.Configuration.AddObject(
    new MySettings { FeatureEnabled = true },
    AppJsonContext.Default.MySettings);
```

The reflection and `JsonTypeInfo<T>` overloads use the same configuration-flattening implementation.

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

The library follows Semantic Versioning. Stable releases are published from GitHub Releases. Maintainer-triggered prereleases use `alpha`, `beta`, and `rc` channels; alpha packages remain on GitHub Packages while beta and RC packages are also published to NuGet.org.

## Building

The repository uses the .NET SDK directly.

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet pack --configuration Release
```
