# Esolang.Processor.Extensions.IO

Provides extension methods for running `IEventProcessor` using various I/O abstractions.

## Features

- **Text I/O**: Extension methods for `TextReader` and `TextWriter`.
- **String I/O**: Convenient extension methods for handling `string` input and `StringBuilder` output.
- **Pipe I/O**: Extension methods for high-performance `PipeReader` and `PipeWriter` from `System.IO.Pipelines`.

## Usage

Depending on your needs, you can use these extensions to run processors seamlessly:

```csharp
// Example using string input/output
var exitCode = await processor.RunToEndAsync(input: "your_input", output: stringBuilder);

// Example using string result
var result = await processor.RunToStringAsync(input: "your_input");
```

For more advanced I/O, utilize the text or pipe extensions.
