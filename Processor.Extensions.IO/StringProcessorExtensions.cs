using System.Text;
using static Esolang.Processor.IOEvent;

namespace Esolang.Processor.Extensions.IO;

/// <summary>
/// Provides extension methods for running <see cref="IEventProcessor"/> using <see cref="string"/> and <see cref="StringBuilder"/>.
/// </summary>
public static class StringProcessorExtensions
{
    /// <summary>
    /// Executes the processor until it reaches an <see cref="EndEvent"/> and returns the output as a string.
    /// </summary>
    /// <param name="processor">The event processor.</param>
    /// <param name="input">The input string.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The output string.</returns>
    public static async ValueTask<string?> RunToStringAsync(
        this IEventProcessor processor,
        string? input = null,
        CancellationToken cancellationToken = default)
    {
        var output = new StringBuilder();
        await processor.RunToEndAsync(input, output, cancellationToken);
        return output.ToString();
    }

    /// <summary>
    /// Executes the processor synchronously until it reaches an <see cref="EndEvent"/> and returns the output as a string.
    /// </summary>
    /// <param name="processor">The event processor.</param>
    /// <param name="input">The input string.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The output string.</returns>
    [Obsolete($"Use {nameof(RunToStringAsync)} instead.")]
    public static string? RunToString(
        this IEventProcessor processor,
        string? input = null,
        CancellationToken cancellationToken = default)
    {
        var result = processor.RunToStringAsync(input, cancellationToken);
        if (result.IsCompleted)
            return result.GetAwaiter().GetResult();
        return result.AsTask().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Executes the processor until it reaches an <see cref="EndEvent"/>.
    /// </summary>
    /// <param name="processor">The event processor.</param>
    /// <param name="input">The input string.</param>
    /// <param name="output">The output string builder.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The exit code.</returns>
    public static async ValueTask<int> RunToEndAsync(
        this IEventProcessor processor,
        string? input = null,
        StringBuilder? output = null,
        CancellationToken cancellationToken = default)
    {
        using var reader = input != null ? new StringReader(input) : null;
        using var writer = output != null ? new StringWriter(output) : null;
        return await processor.RunToEndAsync(reader, writer, cancellationToken);
    }

    /// <summary>
    /// Executes the processor synchronously until it reaches an <see cref="EndEvent"/>.
    /// </summary>
    /// <param name="processor">The event processor.</param>
    /// <param name="input">The input string.</param>
    /// <param name="output">The output string builder.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The exit code.</returns>
    [Obsolete($"Use {nameof(RunToEndAsync)} instead.")]
    public static int RunToEnd(
        this IEventProcessor processor,
        string? input = null,
        StringBuilder? output = null,
        CancellationToken cancellationToken = default)
    {
        var result = processor.RunToEndAsync(input, output, cancellationToken);
        if (result.IsCompleted)
            return result.GetAwaiter().GetResult();
        return result.AsTask().GetAwaiter().GetResult();
    }
}
