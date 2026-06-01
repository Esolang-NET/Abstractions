using Esolang.Processor;
using Esolang.Processor.Extensions.IO;

namespace Esolang.Interpreter;

/// <summary>
/// Provides extension methods for running <see cref="IEventProcessor"/> in an interpreter context.
/// </summary>
public static class InterpreterExtensions
{
    /// <summary>
    /// Executes the processor using standard I/O (Console.In, Console.Out).
    /// </summary>
    /// <param name="processor">The event processor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The exit code.</returns>
    public static async ValueTask<int> RunToConsoleAsync(
        this IEventProcessor processor,
        CancellationToken cancellationToken = default)
    {
        // リダイレクトされていれば標準のReaderをそのまま使い、そうでなければカスタムリーダーを使う
        using var reader = Console.IsInputRedirected ? Console.In : new InteractiveConsoleReader(cancellationToken);
        return await processor.RunToEndAsync(reader, Console.Out, cancellationToken);
    }
}

sealed class InteractiveConsoleReader(CancellationToken cancellationToken) : TextReader
{
    public override int Read()
    {
        // キーが押されるまでループ待機
        while (!Console.KeyAvailable)
        {
            if (cancellationToken.IsCancellationRequested) return -1;
            Thread.Sleep(10);
        }
        return Console.ReadKey(true).KeyChar;
    }

    public override async Task<int> ReadAsync(char[] buffer, int index, int count)
    {
        // 非同期コンテキストでも同様に待機
        while (!Console.KeyAvailable)
        {
            if (cancellationToken.IsCancellationRequested) return -1;
            await Task.Delay(10, cancellationToken);
        }
        buffer[index] = Console.ReadKey(true).KeyChar;
        return 1;
    }
}
