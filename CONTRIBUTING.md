# Contributing

Contributions are welcome through GitHub issues and pull requests.

## Development

The repository requires the .NET SDK selected by `global.json`.

```bash
dotnet restore
dotnet format --verify-no-changes --no-restore
dotnet build --configuration Release --no-restore --warnaserror
dotnet test --configuration Release --no-build
dotnet pack --configuration Release --no-build
```

Keep changes focused and add or update tests for behavioral changes.
