# Esolang.Abstractions

[![.NET](https://github.com/Esolang-NET/Abstractions/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Esolang-NET/Abstractions/actions/workflows/dotnet.yml)

Unified abstractions and interfaces for the Esolang.NET ecosystem.

## Overview

This repository provides common abstractions used across multiple esolang interpreter and code generator projects such as Funge-98, Brainfuck, and Piet. It defines a unified model for execution, I/O processing, and source generation.

## Choose Package

| Want to do | Package |
| --- | --- |
| Create code generators or binders | [Esolang.Generator.Abstractions](./Generator.Abstractions/README.md) |
| Implement a new esolang interpreter | [Esolang.Interpreter.Abstractions](./Interpreter.Abstractions/README.md) |
| Define core execution and I/O models | [Esolang.Processor.Abstractions](./Processor.Abstractions/README.md) |
| Add I/O extensions (Text, Pipelines, etc.) | [Esolang.Processor.Extensions.IO](./Processor.Extensions.IO/README.md) |

## Install

```bash
dotnet add package Esolang.Generator.Abstractions
dotnet add package Esolang.Interpreter.Abstractions
dotnet add package Esolang.Processor.Abstractions
dotnet add package Esolang.Processor.Extensions.IO
```

## NuGet

| Project | NuGet | Summary |
| --- | --- | --- |
| [Esolang.Generator.Abstractions](./Generator.Abstractions/README.md) | [![NuGet: Esolang.Generator.Abstractions](https://img.shields.io/nuget/v/Esolang.Generator.Abstractions?logo=nuget&label=2.0.0)](https://www.nuget.org/packages/Esolang.Generator.Abstractions/) | Code generator and Roslyn binder abstractions. |
| [Esolang.Interpreter.Abstractions](./Interpreter.Abstractions/README.md) | [![NuGet: Esolang.Interpreter.Abstractions](https://img.shields.io/nuget/v/Esolang.Interpreter.Abstractions?logo=nuget&label=2.0.0)](https://www.nuget.org/packages/Esolang.Interpreter.Abstractions/) | Base abstractions for interpreters. |
| [Esolang.Processor.Abstractions](./Processor.Abstractions/README.md) | [![NuGet: Esolang.Processor.Abstractions](https://img.shields.io/nuget/v/Esolang.Processor.Abstractions?logo=nuget&label=2.0.0)](https://www.nuget.org/packages/Esolang.Processor.Abstractions/) | Core processor and I/O event abstractions. |
| [Esolang.Processor.Extensions.IO](./Processor.Extensions.IO/README.md) | [![NuGet: Esolang.Processor.Extensions.IO](https://img.shields.io/nuget/v/Esolang.Processor.Extensions.IO?logo=nuget&label=2.0.0)](https://www.nuget.org/packages/Esolang.Processor.Extensions.IO/) | I/O extensions for event processors. |

## Framework Support

| Project | Target frameworks |
| --- | --- |
| Esolang.Generator.Abstractions | netstandard2.0, netstandard2.1 |
| Esolang.Interpreter.Abstractions | net10.0 |
| Esolang.Processor.Abstractions | netstandard2.0, netstandard2.1 |
| Esolang.Processor.Extensions.IO | netstandard2.0, netstandard2.1 |

## Changelog

- [CHANGELOG](./CHANGELOG.md)

## See also

- [Esolang.Funge](https://github.com/Esolang-NET/Funge) — Funge-98 implementation
- [Esolang.Brainfuck](https://github.com/Esolang-NET/Brainfuck) — Brainfuck implementation
- [Esolang.Piet](https://github.com/Esolang-NET/Piet) — Piet implementation

## License

This project is licensed under the MIT License - see the [LICENSE](./LICENSE) file for details.
