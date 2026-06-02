# Esolang.Generator.Abstractions

Common abstractions for esolang code generators.

## Installation

```bash
dotnet add package Esolang.Generator.Abstractions
```

## Overview

This package provides common interfaces, types, and binder utilities for implementing esolang code generators and Roslyn-based source generators within the Esolang.NET ecosystem.

## Key Components

### MethodSignatureBinder

The `MethodSignatureBinder` is a core utility that facilitates mapping esolang source code to C# partial method signatures. It handles the identification of input, output, and return patterns to generate appropriate boilerplate.

### KnownTypes

The `KnownTypes` struct provides a standardized way to resolve and compare common esolang types (like `PipeReader`, `TextWriter`, etc.) within a Roslyn `Compilation`.

### Binding Kinds

To support diverse esolang execution models, several "Kind" enums are provided to classify method signatures:

- **MethodInputKind**: Classifies how the esolang receives input (e.g., `TextReader`, `PipeReader`, `byte[]`, or none).
- **MethodOutputKind**: Classifies how the esolang sends output (e.g., `TextWriter`, `PipeWriter`, `StringBuilder`, or none).
- **MethodReturnKind**: Determines the method's return pattern (e.g., `void`, `string`, `int`, `Task`, `IEnumerable`, `IAsyncEnumerable`).

### Diagnostic Handling

- **BindingError**: A type-safe record hierarchy representing diagnostic errors encountered during the binding process (e.g., `UnsupportedReturnType`, `DuplicateInput`, `ReturnOutputConflict`).
