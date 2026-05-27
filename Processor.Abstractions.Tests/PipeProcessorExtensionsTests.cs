using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace Esolang.Processor.Tests;

[TestClass]
public class PipeProcessorExtensionsTests(TestContext TestContext)
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
    public async Task RunToEndAsync_HandlesEndEvent()
    {
        var processor = new MockEventProcessor(new List<IOEvent> { new EndEvent(42) });
        var exitCode = await PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken);
        Assert.AreEqual(42, exitCode);
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesOutputCharEvent()
    {
        var processor = new MockEventProcessor(new List<IOEvent> { new OutputCharEvent('A'), new EndEvent(0) });
        var pipe = new Pipe();

        var readTask = Task.Run(async () =>
        {
            var result = await pipe.Reader.ReadAsync(CancellationToken);
            var content = Encoding.UTF8.GetString(result.Buffer.ToArray());
            Assert.AreEqual("A", content);
            pipe.Reader.AdvanceTo(result.Buffer.End);
            pipe.Reader.Complete();
        }, CancellationToken);

        var exitCode = await PipeProcessorExtensions.RunToEndAsync(processor, null, pipe.Writer, CancellationToken);

        await readTask;
        Assert.AreEqual(0, exitCode);
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesOutputIntEvent()
    {
        var processor = new MockEventProcessor([new OutputIntEvent(123), new EndEvent(0)]);
        var pipe = new Pipe();

        var readTask = Task.Run(async () =>
        {
            var result = await pipe.Reader.ReadAsync(CancellationToken);
            var content = Encoding.UTF8.GetString(result.Buffer.ToArray());
            Assert.AreEqual("123", content);
            pipe.Reader.AdvanceTo(result.Buffer.End);
            pipe.Reader.Complete();
        }, CancellationToken);

        var exitCode = await PipeProcessorExtensions.RunToEndAsync(processor, null, pipe.Writer, CancellationToken);

        await readTask;
        Assert.AreEqual(0, exitCode);
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesInputCharEvent()
    {
        var capturedChar = ' ';
        var processor = new MockEventProcessor(new List<IOEvent> {
            new TestInputCharEvent(c => capturedChar = c),
            new EndEvent(0)
        });

        var pipe = new Pipe();
        await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("X"), CancellationToken);
        pipe.Writer.Complete();

        await PipeProcessorExtensions.RunToEndAsync(processor, pipe.Reader, null, CancellationToken);

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

        var pipe = new Pipe();
        await pipe.Writer.WriteAsync(BitConverter.GetBytes(123).AsMemory(), CancellationToken);
        pipe.Writer.Complete();

        await PipeProcessorExtensions.RunToEndAsync(processor, pipe.Reader, null, CancellationToken);

        Assert.AreEqual(123, capturedInt);
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public void RunToEnd_HandlesEndEvent()
    {
        var processor = new MockEventProcessor([new EndEvent(99)]);
        var exitCode = PipeProcessorExtensions.RunToEnd(processor, null, null, CancellationToken);
        Assert.AreEqual(99, exitCode);
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullReader()
    {
        var processor = new MockEventProcessor([new InputCharEventMock()]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullWriter()
    {
        var processor = new MockEventProcessor([new OutputCharEvent('A')]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullReader_InputInt()
    {
        var processor = new MockEventProcessor([new InputIntEventMock()]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullWriter_OutputInt()
    {
        var processor = new MockEventProcessor([new OutputIntEvent(123)]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
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
