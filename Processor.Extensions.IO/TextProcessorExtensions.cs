using System.Buffers;
using static Esolang.Processor.IOEvent;

namespace Esolang.Processor.Extensions.IO;

/// <summary>
/// Provides extension methods for running <see cref="IEventProcessor"/> using <see cref="TextReader"/> and <see cref="TextWriter"/>.
/// </summary>
public static class TextProcessorExtensions
{
    /// <summary>
    /// Executes the processor until it reaches an <see cref="EndEvent"/>.
    /// </summary>
    /// <param name="processor">The event processor.</param>
    /// <param name="input">The input text reader.</param>
    /// <param name="output">The output text writer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The exit code.</returns>
    /// <exception cref="ArgumentNullException">Thrown when input or output is null depending on the event.</exception>
    public static async ValueTask<int> RunToEndAsync(
        this IEventProcessor processor,
        TextReader? input = null,
        TextWriter? output = null,
        CancellationToken cancellationToken = default)
    {
        await foreach (var ioEvent in processor.RunAsyncEnumerable(cancellationToken))
        {
            switch (ioEvent)
            {
                case InputCharEvent charInput:
                    if (input is null)
                        throw new ArgumentNullException(nameof(input));
                    {
                        var buffer = ArrayPool<char>.Shared.Rent(1);
                        try
                        {
                            int read;
                            do
                            {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                                read = await input.ReadAsync(buffer.AsMemory(0, 1), cancellationToken).ConfigureAwait(false);
#else
                                read = await input.ReadAsync(buffer, 0, 1).ConfigureAwait(false);
#endif
                                if (read < 0) continue;
                                charInput.Write(buffer[0]);
                                break;
                            } while (read < 0 && !cancellationToken.IsCancellationRequested);
                        }
                        finally
                        {
                            ArrayPool<char>.Shared.Return(buffer);
                        }
                    }
                    break;
                case InputIntEvent intInput:
                    if (input is null)
                        throw new ArgumentNullException(nameof(input));
                    {
#if NET7_0_OR_GREATER
                        var inputString = await input.ReadLineAsync(cancellationToken).ConfigureAwait(false);
#else
                        var inputString = await input.ReadLineAsync().ConfigureAwait(false);
#endif
                        if (int.TryParse(inputString, out var i))
                        {
                            intInput.Write(i);
                        }
                    }
                    break;
                case OutputCharEvent charOutput:
                    if (output is null)
                        throw new ArgumentNullException(nameof(output));
                    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                        var buffer =ArrayPool<char>.Shared.Rent(1);
                        buffer.AsSpan(0, 1)[0] = charOutput.Output;
                        try {
                        await output.WriteAsync(buffer.AsMemory(0, 1), cancellationToken).ConfigureAwait(false);
                        } finally
                        {
                            ArrayPool<char>.Shared.Return(buffer);
                        }
#else
                        await output.WriteAsync(charOutput.Output).ConfigureAwait(false);
#endif
#if NET8_0_OR_GREATER
                        await output.FlushAsync(cancellationToken).ConfigureAwait(false);
#else
                        await output.FlushAsync().ConfigureAwait(false);
#endif

                    }
                    break;
                case OutputIntEvent intOutput:
                    if (output is null)
                        throw new ArgumentNullException(nameof(output));
                    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                        var outputString = intOutput.Output.ToString();
                        var span = intOutput.Output.ToString().AsSpan();
                        var buffer =ArrayPool<char>.Shared.Rent(span.Length);
                        var memory = buffer.AsMemory(0, span.Length);
                        span.CopyTo(memory.Span);
                        try {
                            await output.WriteAsync(memory, cancellationToken).ConfigureAwait(false);
                        } finally
                        {
                            ArrayPool<char>.Shared.Return(buffer);
                        }
#else
                        await output.WriteAsync(intOutput.Output.ToString()).ConfigureAwait(false);
#endif
#if NET8_0_OR_GREATER
                        await output.FlushAsync(cancellationToken).ConfigureAwait(false);
#else 
                        await output.FlushAsync().ConfigureAwait(false);
#endif
                    }
                    break;
                case EndEvent endEvent:
                    return endEvent.ExitCode;
            }
        }
        return 0;
    }

    /// <summary>
    /// Executes the processor synchronously until it reaches an <see cref="EndEvent"/>.
    /// </summary>
    /// <param name="processor">The event processor.</param>
    /// <param name="input">The input text reader.</param>
    /// <param name="output">The output text writer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The exit code.</returns>
    [Obsolete($"Use {nameof(RunToEndAsync)} instead.")]
    public static int RunToEnd(
        this IEventProcessor processor,
        TextReader? input = null,
        TextWriter? output = null,
        CancellationToken cancellationToken = default)
    {
        var result = RunToEndAsync(processor, input, output, cancellationToken);
        if (result.IsCompleted)
            return result.GetAwaiter().GetResult();
        return result.AsTask().GetAwaiter().GetResult();
    }
}
