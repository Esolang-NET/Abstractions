# Esolang.Processor.Extensions.IO

Provides extension methods for running `IEventProcessor` using various I/O abstractions.

## Installation

```bash
dotnet add package Esolang.Processor.Extensions.IO
```

## Overview

This package provides extension methods to facilitate running processors based on an event-driven I/O model. It bridges the gap between the core `IOEvent` stream and common .NET I/O types.

## Features

- **Text I/O**: Run processors using `TextReader` for input and `TextWriter` for output.
- **String I/O**: Convenient methods for executing with `string` input and capturing output as a `string` or `StringBuilder`.
- **Pipe I/O**: High-performance I/O using `PipeReader` and `PipeWriter` from `System.IO.Pipelines`.

## Usage Examples

### String I/O

```csharp
using Esolang.Processor.Extensions.IO;

// Run and get the output as a string
string? result = await processor.RunToStringAsync(input: "your_input");

// Run and write output to a StringBuilder
var sb = new StringBuilder();
int exitCode = await processor.RunToEndAsync(input: "your_input", output: sb);
```

### Text I/O

```csharp
using Esolang.Processor.Extensions.IO;

// Run using TextReader and TextWriter
int exitCode = await processor.RunToEndAsync(Console.In, Console.Out);
```

### Pipe I/O

```csharp
using Esolang.Processor.Extensions.IO;

// Run using System.IO.Pipelines
int exitCode = await processor.RunToEndAsync(pipeReader, pipeWriter);
```
