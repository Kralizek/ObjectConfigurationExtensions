[![CI](https://github.com/Kralizek/ObjectConfigurationExtensions/actions/workflows/ci.yml/badge.svg)](https://github.com/Kralizek/ObjectConfigurationExtensions/actions/workflows/ci.yml) [![NuGet version](https://img.shields.io/nuget/vpre/Kralizek.Extensions.Configuration.Objects.svg)](https://www.nuget.org/packages/Kralizek.Extensions.Configuration.Objects)

# ObjectConfigurationExtensions

This repository contains a provider for [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/) that allows the insertion of a concrete object into the configuration pipeline.

The library supports primitive types, complex objects, and sequences of both.

## How to use it

Here is a simple ASP.NET Core application that loads an object in the configuration pipeline, specifically in the `Test` section.

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

Install the package using the .NET CLI:

```bash
dotnet add package Kralizek.Extensions.Configuration.Objects
```

## Root section name

The root section name is optional. To add the properties directly to the root configuration:

```csharp
builder.Configuration.AddObject(new
{
    IsEnabled = false
});
```

## Using an object as fallback configuration

`AddObject` follows the normal configuration-provider convention and appends the object provider, giving it higher precedence than providers already registered.

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

Both `AddObject` and `AddObjectAsFallback` have overloads accepting `JsonTypeInfo<T>`. Use these overloads when reflection-based System.Text.Json serialization is not appropriate, including trimming and Native AOT scenarios.

```csharp
[JsonSerializable(typeof(MySettings))]
internal partial class AppJsonContext : JsonSerializerContext;

builder.Configuration.AddObject(
    new MySettings { FeatureEnabled = true },
    AppJsonContext.Default.MySettings);
```

The reflection and `JsonTypeInfo<T>` overloads use the same configuration flattening implementation.

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
