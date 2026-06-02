using Esolang.Processor;
using Esolang.Processor.Extensions.IO;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using static Esolang.Processor.IOEvent;

namespace Esolang.Processor.Tests;

[TestClass]
public class PipeProcessorExtensionsTests(TestContext TestContext)
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
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesEndEvent()
    {
        var processor = new MockEventProcessor([End(42)]);
        var exitCode = await PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken);
        Assert.AreEqual(42, exitCode);
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesOutputCharEvent()
    {
        var processor = new MockEventProcessor([OutputChar('A'), End(0)]);
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
        var processor = new MockEventProcessor([OutputInt(123), End(0)]);
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
        var processor = new MockEventProcessor([
            InputChar(c => capturedChar = c),
            End(0)
        ]);

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
            InputInt(i => capturedInt = i),
            End(0)
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
        var processor = new MockEventProcessor([End(99)]);
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
        var exitCode = PipeProcessorExtensions.RunToEnd(processor, null, null, CancellationToken);
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
        Assert.AreEqual(99, exitCode);
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullReader()
    {
        var capturedChar = ' ';
        var processor = new MockEventProcessor([
            InputChar(c => capturedChar = c)
        ]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullWriter()
    {
        var processor = new MockEventProcessor([OutputChar('A')]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullReader_InputInt()
    {
        var capturedInt = 0;
        var processor = new MockEventProcessor([
            InputInt(i => capturedInt = i)
        ]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [TestMethod]
    [Timeout(Constants.Timeout, CooperativeCancellation = true)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullWriter_OutputInt()
    {
        var processor = new MockEventProcessor([OutputInt(123)]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }
}

file static class Constants
{
    public const int Timeout = 2000;
}
