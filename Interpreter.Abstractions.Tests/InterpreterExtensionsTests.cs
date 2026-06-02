using Esolang.Processor;
using static Esolang.Processor.IOEvent;

namespace Esolang.Interpreter.Tests;

[TestClass]
public class InterpreterExtensionsTests(TestContext TestContext)
{
    CancellationToken CancellationToken => TestContext.CancellationToken;

    [TestMethod]
    [Timeout(2000, CooperativeCancellation = true)]
    public async Task RunToConsoleAsync_ProcessesAllEvents()
    {
        var output = new StringWriter();
        Console.SetOut(output);

        var capturedChar = ' ';
        var capturedInt = 0;

        var processor = new MockEventProcessor([
            OutputChar('A'),
            OutputInt(123),
            InputChar(c => capturedChar = c),
            InputInt(i => capturedInt = i),
            End(0)
        ]);

        // Input simulation for interactive mode
        using var input = new StringReader("B" + "456" + Environment.NewLine);
        Console.SetIn(input);

        var exitCode = await processor.RunToConsoleAsync(cancellationToken: CancellationToken);

        Assert.AreEqual(0, exitCode);
        Assert.AreEqual("A123", output.ToString());
        Assert.AreEqual('B', capturedChar);
        Assert.AreEqual(456, capturedInt);
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
