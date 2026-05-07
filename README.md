# Esolang.Abstractions

Shared abstraction interfaces for Esolang.NET projects (Funge-98, Brainfuck, Piet).

## Overview

This repository provides common abstractions used across multiple esolang interpreter and code generator projects:

- **Esolang.Funge** — Funge-98 parser, processor, and generator
- **Esolang.Brainfuck** — Brainfuck interpreter and generator
- **Esolang.Piet** — Piet parser, processor, and generator

## Packages

### Esolang.Processor.Abstractions

Unified processor abstractions for esolang execution.

```bash
dotnet add package Esolang.Processor.Abstractions
```

Provides:

- **`IProcessor<TProgram>`** — Base interface holding a parsed program
- **`ITextProcessor<TProgram>`** — Execution via `TextReader`/`TextWriter`
- **`IPipeProcessor<TProgram>`** — Execution via `PipeReader`/`PipeWriter`

#### Usage

```csharp
using Esolang.Processor;

// Implement in your processor
public class MyProcessor : ITextProcessor<MyProgram>
{
    public MyProgram Program { get; }

    public int RunToEnd(TextReader? input = null, TextWriter? output = null, CancellationToken ct = default)
    {
        // Execute program and return exit code
    }

    public ValueTask<int> RunToEndAsync(TextReader? input = null, TextWriter? output = null, CancellationToken ct = default)
    {
        // Async variant
    }
}
```

## Contributing

Contributions are welcome. Please ensure code follows the project's `.editorconfig` and coding standards.

## License

See [LICENSE](LICENSE) for details.
