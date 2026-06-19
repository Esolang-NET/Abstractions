using Esolang.Processor;
using static Esolang.Processor.IOEvent;

namespace Esolang.Interpreter.Tests;

public class InterpreterExtensionsTests
{

    [Test]
    [Timeout(2000)]
    public async Task RunToConsoleAsync_ProcessesAllEvents(CancellationToken CancellationToken)
    {
        var oldout = Console.Out;
        var oldin = Console.In;
        try
        {
            var output = new StringWriter();
            MockEventProcessor? processor;
            var capturedChar = ' ';
            var capturedInt = 0;
            var capturedLine = string.Empty;
            var capturedString = string.Empty;

#pragma warning disable TUnit0055 // Do not overwrite the Console writer
            Console.SetOut(output);
#pragma warning restore TUnit0055 // Do not overwrite the Console writer


            processor = new MockEventProcessor([
                OutputChar('A'),
                OutputInt(123),
                OutputLine("piyo piyo"),
                OutputString("""
                neko neko
                inu inu
                """),
                InputChar(c => capturedChar = c),
                InputInt(i => capturedInt = i),
                InputLine(l => capturedLine = l),
                InputString(s => capturedString = s),
                End(0)
            ]);

            // Input simulation for interactive mode
            using var input = new StringReader(
                "B" + "456" + Environment.NewLine
                + "neko neko" + Environment.NewLine
                + "inu inu" + Environment.NewLine
            );
            Console.SetIn(input);

            var exitCode = await processor.RunToConsoleAsync(cancellationToken: CancellationToken);

            await Assert.That(exitCode).IsEqualTo(0);
            await Assert.That($"{output}".Replace("\r\n", "\n").Replace("\r", "\n")).IsEqualTo($"A123piyo piyo\nneko neko\ninu inu");
            await Assert.That(capturedChar).IsEqualTo('B');
            await Assert.That(capturedInt).IsEqualTo(456);
            await Assert.That(capturedLine).IsEqualTo("neko neko");
            await Assert.That(capturedString).IsEqualTo("inu inu");

        }
        finally
        {
#pragma warning disable TUnit0055 // Do not overwrite the Console writer
            Console.SetOut(oldout);
#pragma warning restore TUnit0055 // Do not overwrite the Console writer
            Console.SetIn(oldin);
        }
    }

    class MockEventProcessor(List<IOEvent> events) : IEventProcessor
    {
        public async IAsyncEnumerable<IOEvent> RunAsyncEnumerable([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var ev in events)
            {
                yield return ev;
            }
            await Task.CompletedTask;
        }
    }
}
