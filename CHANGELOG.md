# Changelog

All notable changes to this repository are documented in this file.

The format is based on Keep a Changelog.

## [Unreleased]

## [2.0.2] - 2026-06-02

### Fix

- **Esolang.Processor.Abstractions**: remove unnecessary PackageReference for System.Buffers

## [2.0.1] - 2026-06-02

### Fix

- **Esolang.Interpreter.Abstractions** / **Esolang.Processor.Abstractions** / **Esolang.Processor.Extensions.IO** : 
   - add AOT compatibility settings for net10.0 target framework

## [2.0.0] - 2026-06-02

### Added

- **Esolang.Processor.Abstractions**:
    - Introduced `IEventProcessor` for a unified, event-driven execution model.
    - Added `IOEvent` and its subtypes (`InputChar`, `InputInt`, `OutputChar`, `OutputInt`, `End`) to represent I/O operations.
    - Added `IProcessor` as a non-generic base interface.
- **Esolang.Generator.Abstractions**:
    - Added comprehensive abstractions for method signature binding and type resolution.
    - Introduced `MethodSignatureBinder` for mapping esolang source to C# partial methods.
    - Added `KnownTypes` for standardized Roslyn type resolution.
    - Added `BindingError` record hierarchy for type-safe diagnostic reporting.
    - Added `MethodInputKind`, `MethodOutputKind`, and `MethodReturnKind` for signature classification.
- **Esolang.Interpreter.Abstractions**:
    - Added new project for common interpreter utilities.
    - Introduced `RunToConsoleAsync` extension method for running processors with standard console I/O.
- **Esolang.Processor.Extensions.IO**:
    - Extracted and standardized I/O extension methods into a dedicated project.
    - Added `RunToEndAsync` overloads for `TextReader`/`TextWriter`, `string`/`StringBuilder`, and `PipeReader`/`PipeWriter`.
    - Added `RunToStringAsync` for capturing output as a string.
- Enhanced testing infrastructure across all abstraction projects, significantly improving code coverage.

### Changed

- **Esolang.Processor.Abstractions** (Breaking Changes):
    - Removed `ITextProcessor<TProgram>` and `IPipeProcessor<TProgram>` interfaces in favor of the event-driven `IEventProcessor`.
    - Modernized interfaces to use `IAsyncEnumerable<IOEvent>` for execution.
    - Standardized naming and namespaces.
- Updated `.editorconfig` with stricter C# style and MSTest diagnostic rules.

## [1.0.0] - 2026-05-07

### Added

- Initial release of Esolang.Processor.Abstractions.
- `IProcessor<TProgram>` — Base interface holding a parsed program.
- `ITextProcessor<TProgram>` — Execution interface using `TextReader` and `TextWriter` for text-based I/O.
- `IPipeProcessor<TProgram>` — Execution interface using `PipeReader` and `PipeWriter` for high-performance pipe-based I/O.
- Both text and pipe processors return exit codes (`int`) from execution.
- Support for optional `CancellationToken` on all execution methods.

[Unreleased]: https://github.com/Esolang-NET/Abstractions/compare/v2.0.1...HEAD
[2.0.1]: https://github.com/Esolang-NET/Abstractions/tree/v2.0.1
[2.0.0]: https://github.com/Esolang-NET/Abstractions/tree/v2.0.0
[1.0.0]: https://github.com/Esolang-NET/Abstractions/tree/v1.0.0
