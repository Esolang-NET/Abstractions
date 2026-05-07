# Changelog

All notable changes to this repository are documented in this file.

The format is based on Keep a Changelog.

## [Unreleased]

## [1.0.0] - 2026-05-07

### Added

- Initial release of Esolang.Processor.Abstractions.
- `IProcessor<TProgram>` — Base interface holding a parsed program.
- `ITextProcessor<TProgram>` — Execution interface using `TextReader` and `TextWriter` for text-based I/O.
- `IPipeProcessor<TProgram>` — Execution interface using `PipeReader` and `PipeWriter` for high-performance pipe-based I/O.
- Both text and pipe processors return exit codes (`int`) from execution.
- Support for optional `CancellationToken` on all execution methods.
