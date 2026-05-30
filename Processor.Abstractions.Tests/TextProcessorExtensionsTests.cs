namespace Esolang.Processor.Tests;

[TestClass]
public class TextProcessorExtensionsTests(TestContext TestContext)
{
    CancellationToken CancellationToken => TestContext.CancellationToken;

    private class MockEventProcessor(List<IOEvent> events) : IEventProcessor
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
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesOutputCharEvent()
    {
        var processor = new MockEventProcessor([new OutputCharEvent('A'), new EndEvent(0)]);
        using var writer = new StringWriter();

        var exitCode = await TextProcessorExtensions.RunToEndAsync(processor, null, writer, CancellationToken);

        Assert.AreEqual("A", writer.ToString());
        Assert.AreEqual(0, exitCode);
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesOutputIntEvent()
    {
        var processor = new MockEventProcessor([new OutputIntEvent(123), new EndEvent(0)]);
        using var writer = new StringWriter();

        var exitCode = await TextProcessorExtensions.RunToEndAsync(processor, null, writer, CancellationToken);

        Assert.AreEqual("123" + Environment.NewLine, writer.ToString());
        Assert.AreEqual(0, exitCode);
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesInputCharEvent()
    {
        var capturedChar = ' ';
        var processor = new MockEventProcessor([
            new TestInputCharEvent(c => capturedChar = c),
            new EndEvent(0)
        ]);

        using var reader = new StringReader("X");
        await TextProcessorExtensions.RunToEndAsync(processor, reader, null, CancellationToken);

        Assert.AreEqual('X', capturedChar);
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesInputIntEvent()
    {
        var capturedInt = 0;
        var processor = new MockEventProcessor([
            new TestInputIntEvent(i => capturedInt = i),
            new EndEvent(0)
        ]);

        using var reader = new StringReader("123");
        await TextProcessorExtensions.RunToEndAsync(processor, reader, null, CancellationToken);

        Assert.AreEqual(123, capturedInt);
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullInput()
    {
        var processor = new MockEventProcessor([new InputCharEventMock()]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => TextProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullOutput()
    {
        var processor = new MockEventProcessor([new OutputCharEvent('A')]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => TextProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullInput_InputInt()
    {
        var processor = new MockEventProcessor([new InputIntEventMock()]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => TextProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullOutput_OutputInt()
    {
        var processor = new MockEventProcessor([new OutputIntEvent(123)]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => TextProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public void RunToEnd_HandlesEndEvent()
    {
        var processor = new MockEventProcessor([new EndEvent(88)]);
#pragma warning disable CS0618
        var exitCode = TextProcessorExtensions.RunToEnd(processor, null, null, CancellationToken);
#pragma warning restore CS0618
        Assert.AreEqual(88, exitCode);
    }

    private class TestInputCharEvent(Action<char> write) : InputCharEvent
    {
        public override void Write(char c) => write(c);
    }

    private class TestInputIntEvent(Action<int> write) : InputIntEvent
    {
        public override void Write(int i) => write(i);
    }

    private class InputCharEventMock : InputCharEvent
    {
        public override void Write(char c) { }
    }

    private class InputIntEventMock : InputIntEvent
    {
        public override void Write(int i) { }
    }
}

file static class Constants
{
    public const int Timeout = 2000;
}
