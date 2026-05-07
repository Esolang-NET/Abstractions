# Esolang.Processor.Abstractions

Unified processor abstractions for esolang execution across the Esolang.NET ecosystem.

## Installation

```bash
dotnet add package Esolang.Processor.Abstractions
```

## Overview

This package provides common interfaces for implementing esolang processors (interpreters) with consistent execution patterns.

## Interfaces

### IProcessor\<TProgram\>

Base interface that holds a parsed program.

```csharp
public interface IProcessor<TProgram>
{
    /// <summary>Parsed program instance.</summary>
    TProgram Program { get; }
}
```

### ITextProcessor\<TProgram\>

Execution interface using `TextReader` and `TextWriter` for input/output.

```csharp
public interface ITextProcessor<TProgram> : IProcessor<TProgram>
{
    /// <summary>Execute the program synchronously and return exit code.</summary>
    int RunToEnd(
        TextReader? input = null,
        TextWriter? output = null,
        CancellationToken cancellationToken = default);

    /// <summary>Execute the program asynchronously and return exit code.</summary>
    ValueTask<int> RunToEndAsync(
        TextReader? input = null,
        TextWriter? output = null,
        CancellationToken cancellationToken = default);
}
```

### IPipeProcessor\<TProgram\>

Execution interface using `PipeReader` and `PipeWriter` for high-performance I/O.

```csharp
public interface IPipeProcessor<TProgram> : IProcessor<TProgram>
{
    /// <summary>Execute the program synchronously and return exit code.</summary>
    int RunToEnd(
        PipeReader input,
        PipeWriter output,
        CancellationToken cancellationToken = default);

    /// <summary>Execute the program asynchronously and return exit code.</summary>
    ValueTask<int> RunToEndAsync(
        PipeReader input,
        PipeWriter output,
        CancellationToken cancellationToken = default);
}
```

## Usage Example

Implement these interfaces in your processor:

```csharp
using Esolang.Processor;
using System.IO.Pipelines;

public class MyEsolangProcessor : ITextProcessor<MyProgram>, IPipeProcessor<MyProgram>
{
    public MyProgram Program { get; }

    public MyEsolangProcessor(MyProgram program)
    {
        Program = program;
    }

    // Text-based I/O
    public int RunToEnd(TextReader? input = null, TextWriter? output = null, CancellationToken cancellationToken = default)
    {
        // Execute program with text I/O, return exit code
        return 0;
    }

    public ValueTask<int> RunToEndAsync(TextReader? input = null, TextWriter? output = null, CancellationToken cancellationToken = default)
    {
        // Async variant
        return new ValueTask<int>(0);
    }

    // Pipe-based I/O
    public int RunToEnd(PipeReader input, PipeWriter output, CancellationToken cancellationToken = default)
    {
        // Execute program with pipe I/O, return exit code
        return 0;
    }

    public ValueTask<int> RunToEndAsync(PipeReader input, PipeWriter output, CancellationToken cancellationToken = default)
    {
        // Async variant
        return new ValueTask<int>(0);
    }
}
```

## Target Framework

- **netstandard2.0** — Compatible with .NET Framework 4.6.1+ and .NET Core 2.0+

## Dependencies

- **System.IO.Pipelines** — High-performance I/O primitives

## See Also

- [Esolang.Funge](https://github.com/Esolang-NET/Funge) — Funge-98 implementation
- [Esolang.Brainfuck](https://github.com/Esolang-NET/Brainfuck) — Brainfuck implementation
- [Esolang.Piet](https://github.com/Esolang-NET/Piet) — Piet implementation
