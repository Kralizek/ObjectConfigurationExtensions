# Kralizek.Extensions.Configuration.Objects

Add strongly typed objects directly to a `Microsoft.Extensions.Configuration` pipeline.

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

You can place the object under a root section:

```csharp
builder.Configuration.AddObject(settings, "MySettings");
```

`AddObject` appends the provider, so it has higher precedence than configuration providers already registered.

## Add fallback defaults

Use `AddObjectAsFallback` when the object should provide defaults that other providers can override:

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

The object provider is inserted at the beginning of the provider chain, so later providers win.

## Source-generated System.Text.Json

Both APIs have overloads accepting `JsonTypeInfo<T>`, which can be used with System.Text.Json source generation for trimming and Native AOT scenarios.

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
