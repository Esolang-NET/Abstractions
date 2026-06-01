# Esolang.Processor.Abstractions

Unified processor abstractions for esolang execution across the Esolang.NET ecosystem.

## Installation

```bash
dotnet add package Esolang.Processor.Abstractions
```

## Overview

This package provides common interfaces and extension methods for implementing esolang processors (interpreters) based on an event-driven I/O model.

## Core Interfaces

### IProcessor\<TProgram\>

Base interface that holds a parsed program.

```csharp
public interface IProcessor<TProgram> : IProcessor
{
    /// <summary>The parsed program.</summary>
    TProgram Program { get; }
}
```

### IEventProcessor

Execution interface based on a stream of I/O events.

```csharp
public interface IEventProcessor : IProcessor
{
    /// <summary>
    /// Executes the processor and returns an asynchronous stream of I/O events.
    /// </summary>
    IAsyncEnumerable<IOEvent> RunAsyncEnumerable(CancellationToken cancellationToken = default);
}
```

## Extension Methods

To facilitate running processors, common extension methods are provided in separate packages:

- **`Esolang.Processor.Extensions.IO`**: Contains `TextProcessorExtensions` (for `TextReader`/`TextWriter`), `StringProcessorExtensions` (for `string`/`StringBuilder`), and `PipeProcessorExtensions` (for `PipeReader`/`PipeWriter`).

```csharp
// Example using TextReader/TextWriter
await processor.RunToEndAsync(inputReader, outputWriter, cancellationToken);

// Example using PipeReader/PipeWriter
await processor.RunToEndAsync(inputPipe, outputPipe, cancellationToken);

// Example using string input/output
var result = await processor.RunToStringAsync(input: "your_input", cancellationToken);
```

## Usage Example

Implement `IEventProcessor` in your processor:

```csharp
using Esolang.Processor;

public class MyEsolangProcessor : IEventProcessor
{
    public MyProgram Program { get; }

    public IAsyncEnumerable<IOEvent> RunAsyncEnumerable(CancellationToken cancellationToken = default)
    {
        // Implement the execution logic yielding IOEvents (InputCharEvent, OutputCharEvent, etc.)
    }
}
```

## Target Framework

- **netstandard2.0** — Compatible with .NET Framework 4.6.1+ and .NET Core 2.0+

## See Also

- [Esolang.Funge](https://github.com/Esolang-NET/Funge) — Funge-98 implementation
- [Esolang.Brainfuck](https://github.com/Esolang-NET/Brainfuck) — Brainfuck implementation
- [Esolang.Piet](https://github.com/Esolang-NET/Piet) — Piet implementation
