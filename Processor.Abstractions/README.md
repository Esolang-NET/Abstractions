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

## IO Events

The `IEventProcessor` communicates with the outside world through a stream of `IOEvent` objects.

| Event Type | Purpose | Factory Method |
| --- | --- | --- |
| `InputCharEvent` | Requests a single character from the input. | `IOEvent.InputChar(Action<char> write)` |
| `InputIntEvent` | Requests a single integer from the input. | `IOEvent.InputInt(Action<int> write)` |
| `OutputCharEvent` | Sends a single character to the output. | `IOEvent.OutputChar(char output)` |
| `OutputIntEvent` | Sends a single integer to the output. | `IOEvent.OutputInt(int output)` |
| `EndEvent` | Signals the end of execution and provides an exit code. | `IOEvent.End(int exitCode)` |

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

    public async IAsyncEnumerable<IOEvent> RunAsyncEnumerable(CancellationToken cancellationToken = default)
    {
        // Implement the execution logic yielding IOEvents
        yield return IOEvent.OutputChar('H');
        yield return IOEvent.OutputChar('i');
        yield return IOEvent.End(0);
    }
}
```

## Target Framework

- **netstandard2.0** — Compatible with .NET Framework 4.6.1+ and .NET Core 2.0+
- **netstandard2.1** — Compatible with .NET Core 3.0+ and .NET 5+

## See Also

- [Esolang.Funge](https://github.com/Esolang-NET/Funge) — Funge-98 implementation
- [Esolang.Brainfuck](https://github.com/Esolang-NET/Brainfuck) — Brainfuck implementation
- [Esolang.Piet](https://github.com/Esolang-NET/Piet) — Piet implementation
