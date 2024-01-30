<a href="https://www.buymeacoffee.com/rengol"><img src="https://img.buymeacoffee.com/button-api/?text=Buy me a pizza&emoji=🍕&slug=rengol&button_colour=FFDD00&font_colour=000000&font_family=Cookie&outline_colour=000000&coffee_colour=ffffff" /></a>

[![Build status](https://ci.appveyor.com/api/projects/status/5l2nwpit63j19rhd/branch/master?svg=true)](https://ci.appveyor.com/project/Kralizek/objectconfigurationextensions/branch/master) [![NuGet version](https://img.shields.io/nuget/vpre/Kralizek.Extensions.Configuration.Objects.svg)](https://www.nuget.org/packages/Kralizek.Extensions.Configuration.Objects)

# ObjectConfigurationExtensions

This repository contains a provider for [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/) that allows the insertion of a concrete object into the configuration pipeline.

The library supports all primitive types, complex objects and sequences of both.

## How to use it

Let's see it in action. Here is a simple ASP.NET Core application that loads an object in the configuration pipeline, specifically in the `Test` section.

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

You can install the package using the .NET CLI

```bash
$ dotnet add package Kralizek.Extensions.Configuration.Objects
```

When accessed at the root page, the application prints all the configuration values found in the `Test` section.

```bash
$ curl http://localhost:5003
{
  "Test":null,
  "Test:Flag":"true",
  "Test:ManyValues":null,
  "Test:ManyValues:0":null,
  "Test:ManyValues:0:Number":"234",
  "Test:ManyValues:0:Text":"New value",
  "Test:ManyValues:1":null,
  "Test:ManyValues:1:Number":"345",
  "Test:ManyValues:1:Text":"Another value",
  "Test:Text":"Something",
  "Test:Value":"123"
}
```

## Root section name

The root section name used in the sample above is optional. If you prefer so, you can add the properties of the object directly to the root of the configuration.

```csharp
builder.Configuration.AddObject(new
{
    IsEnabled = false
});
```

## Newtonsoft.Json serializer

The library uses by default the JSON serializer available in the `System.Text.Json` namespace.

If you need to use the JSON serializer from Newtonsoft, you can install the specific package and use the `AddObjectWithNewtonsoftJson` method.

```bash
dotnet add package Kralizek.Extensions.Configuration.Objects.NewtonsoftJson
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddObjectWithNewtonsoftJson(new
{
    Text = "Something"
}, "Test");

var app = builder.Build();

app.MapGet("/", (IConfiguration configuration) => configuration
    .GetSection("Test")
    .AsEnumerable()
    .OrderBy(c => c.Key)
    .ToDictionary(c => c.Key, v => v.Value));


app.Run();
```

## Custom serializer

The serialization strategy doesn't play an important role in the library. 

Yet, in case of need, a custom serializer implementing the interface `Kralizek.Extensions.Configuration.IConfigurationSerializer` can be provided.

The interface is very minimal and requires to write the object as a `Dictionary<string, string?>` where the key is the path to the property and the value is the value of the property.

```csharp
public interface IConfigurationSerializer
{
    IDictionary<string, string?> Serialize(object source, string rootSectionName);
}
```

Let's assume we have a custom implementation

```csharp
public class FailingConfigurationSerializer : IConfigurationSerializer
{
    public IDictionary<string, string?> Serialize(object source, string rootSectionName) => throw new NotImplementedException();
}
```

We can use it by passing an instance of the custom serializer to the `AddObject` method.

```csharp
var builder = WebApplication.CreateBuilder(args);

var serializer = new FailingConfigurationSerializer();

builder.Configuration.AddObject(new
{
    Text = "Something"
}, serializer, "Test");

var app = builder.Build();

app.MapGet("/", (IConfiguration configuration) => configuration
    .GetSection("Test")
    .AsEnumerable()
    .OrderBy(c => c.Key)
    .ToDictionary(c => c.Key, v => v.Value));


app.Run();
```

## Versioning

This library follows [Semantic Versioning 2.0.0](http://semver.org/spec/v2.0.0.html) for the public releases (published to the [nuget.org](https://www.nuget.org/)).

## How to build

This project uses [Cake](https://cakebuild.net/) as a build engine.

If you would like to build this project locally, just execute the `build.cake` script.

You can do it by using the .NET tool created by CAKE authors and use it to execute the build script.

```powershell
dotnet tool install -g Cake.Tool
dotnet cake
```
