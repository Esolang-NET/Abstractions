using Esolang.Processor;

namespace Esolang.Interpreter.Tests;

[TestClass]
public class InterpreterExtensionsTests(TestContext TestContext)
{
    CancellationToken CancellationToken => TestContext.CancellationToken;

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

    [TestMethod]
    [Timeout(2000, CooperativeCancellation = true)]
    public async Task RunToConsoleAsync_HandlesEndEvent()
    {
        var processor = new MockEventProcessor([IOEvent.End(0)]);

        // Redirect Console
        using var stringReader = new StringReader("");
        using var stringWriter = new StringWriter();
        var originalIn = Console.In;
        var originalOut = Console.Out;
        Console.SetIn(stringReader);
        Console.SetOut(stringWriter);

        try
        {
            var exitCode = await processor.RunToConsoleAsync(CancellationToken);
            Assert.AreEqual(0, exitCode);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }
}
