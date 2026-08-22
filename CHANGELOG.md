# Changelog

All notable changes to this project will be documented in this file.

## Unreleased

- Repository modernization for the next major version.
- Removed the Newtonsoft.Json companion package and custom serializer abstraction.
- Made `AddObject` generic and added `JsonTypeInfo<T>` overloads for source-generated System.Text.Json metadata.
- Added `AddObjectAsFallback` for registering object configuration at the lowest precedence.
- Aligned object flattening with .NET 10 configuration semantics for null values, empty strings, empty objects, empty arrays, and null array elements.

## 3.0.1 - 2024-07-12

- Updated System.Text.Json.

## 3.0.0

- Added System.Text.Json as the default serializer while retaining the Newtonsoft.Json companion package.
