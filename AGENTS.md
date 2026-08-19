# Agent guidance

## Repository purpose

ObjectConfigurationExtensions provides a Microsoft.Extensions.Configuration provider that adds concrete objects to the configuration pipeline.

## Structure

- `src/Objects`: core package using System.Text.Json.
- `src/Objects.NewtonsoftJson`: Newtonsoft.Json companion package.
- `tests/Tests.Objects`: unit tests for both packages.
- `samples`: small ASP.NET Core usage examples.
- `eng/Package.props`: shared NuGet and MinVer settings.

## Build and validation

Run the same core checks as CI:

```bash
dotnet restore
dotnet format --verify-no-changes --no-restore
dotnet build --configuration Release --no-restore --warnaserror
dotnet test --configuration Release --no-build
dotnet pack --configuration Release --no-build
```

## Change discipline

Preserve backward compatibility unless a change is explicitly scoped to a major version. Keep the public API small, prefer platform conventions, and avoid adding abstractions unless they solve a demonstrated library requirement.
