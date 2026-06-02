# Esolang.Interpreter.Abstractions

Common abstractions for esolang interpreters.

## Installation

```bash
dotnet add package Esolang.Interpreter.Abstractions
```

## Overview

This package provides common interfaces and extensions for implementing esolang interpreters within the Esolang.NET ecosystem. It focuses on facilitating the execution of any `IEventProcessor` using standard console I/O.

## Usage

### Run to Console

The `RunToConsoleAsync` extension method allows you to execute an `IEventProcessor` directly using `Console.In` and `Console.Out`.

```csharp
using Esolang.Interpreter;

// Run your processor using standard I/O
int exitCode = await processor.RunToConsoleAsync();
```
