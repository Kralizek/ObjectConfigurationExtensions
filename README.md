[![CI](https://github.com/Kralizek/ObjectConfigurationExtensions/actions/workflows/ci.yml/badge.svg)](https://github.com/Kralizek/ObjectConfigurationExtensions/actions/workflows/ci.yml) [![NuGet version](https://img.shields.io/nuget/vpre/Kralizek.Extensions.Configuration.Objects.svg)](https://www.nuget.org/packages/Kralizek.Extensions.Configuration.Objects)

# ObjectConfigurationExtensions

This repository contains a provider for [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/) that allows the insertion of a concrete object into the configuration pipeline.

The library supports all primitive types, complex objects and sequences of both.

## How to use it

Here is a simple ASP.NET Core application that loads an object in the configuration pipeline, specifically in the `Test` section.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddObject(new
{
    Value = 123,
    ManyValues = new ComplexObject[]
    {
        new ("New value", 234),
        new ("Another value", 345)
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

## Newtonsoft.Json serializer

The library uses System.Text.Json by default. Consumers that need Newtonsoft.Json can install the companion package and use `AddObjectWithNewtonsoftJson`.

```bash
dotnet add package Kralizek.Extensions.Configuration.Objects.NewtonsoftJson
```

```csharp
builder.Configuration.AddObjectWithNewtonsoftJson(new
{
    Text = "Something"
}, "Test");
```

## Custom serializer

A custom serializer can implement `Kralizek.Extensions.Configuration.IConfigurationSerializer` and provide configuration keys and values directly.

```csharp
public interface IConfigurationSerializer
{
    IDictionary<string, string?> Serialize(object source, string rootSectionName);
}
```

Pass it to `AddObject`:

```csharp
var serializer = new FailingConfigurationSerializer();
builder.Configuration.AddObject(serializer, new { Text = "Something" }, "Test");
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
