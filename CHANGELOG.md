# Changelog

All notable changes to this repository are documented in this file.

The format is based on Keep a Changelog.

## [2.0.0] - 2026-06-01

### Added
- **Esolang.Generator.Abstractions**: Added comprehensive abstractions for method signature binding and type resolution.
- **Esolang.Interpreter.Abstractions**: Added new common abstractions for esolang interpreters.
- **Esolang.Processor.Extensions.IO**: Extracted and standardized IO extension methods (supporting `TextReader`, `TextWriter`, and `System.IO.Pipelines`) into a dedicated project.
- Enhanced testing infrastructure across all abstraction projects, significantly improving code coverage.

### Changed
- Refactored `Generator.Abstractions` to utilize type-safe `BindingError` record hierarchy instead of string-based error IDs.
- Standardized `KnownTypes` implementation and nullable handling in generator abstractions.
- Refactored processor abstractions for improved architectural cleanliness.
- Updated `.editorconfig` with stricter C# style and MSTest diagnostic rules.

### Fixed
- Improved cooperative cancellation support in `RunToConsoleAsync`.
- Addressed various issues in `MethodSignatureBinder` and `MethodSignatureBinding`.

## [1.0.0] - 2026-05-07

### Added

- Initial release of Esolang.Processor.Abstractions.
- `IProcessor<TProgram>` — Base interface holding a parsed program.
- `ITextProcessor<TProgram>` — Execution interface using `TextReader` and `TextWriter` for text-based I/O.
- `IPipeProcessor<TProgram>` — Execution interface using `PipeReader` and `PipeWriter` for high-performance pipe-based I/O.
- Both text and pipe processors return exit codes (`int`) from execution.
- Support for optional `CancellationToken` on all execution methods.
