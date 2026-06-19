using Esolang.Processor;
using Esolang.Processor.Extensions.IO;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using static Esolang.Processor.IOEvent;

namespace Esolang.Processor.Tests;

public class PipeProcessorExtensionsTests
{

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

    [Test]
    [Timeout(Constants.Timeout)]
    public async Task RunToEndAsync_HandlesEndEvent(CancellationToken CancellationToken)
    {
        var processor = new MockEventProcessor([End(42)]);
        var exitCode = await PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken);
        await Assert.That(exitCode).IsEqualTo(42);
    }

    [Test]
    [Timeout(Constants.Timeout)]
    public async Task RunToEndAsync_HandlesOutputCharEvent(CancellationToken CancellationToken)
    {
        var processor = new MockEventProcessor([OutputChar('A'), End(0)]);
        var pipe = new Pipe();

        var readTask = Task.Run(async () =>
        {
            var result = await pipe.Reader.ReadAsync(CancellationToken);
            var content = Encoding.UTF8.GetString(result.Buffer.ToArray());
            await Assert.That(content).IsEqualTo("A");
            pipe.Reader.AdvanceTo(result.Buffer.End);
            pipe.Reader.Complete();
        }, CancellationToken);

        var exitCode = await PipeProcessorExtensions.RunToEndAsync(processor, null, pipe.Writer, CancellationToken);

        await readTask;
        await Assert.That(exitCode).IsEqualTo(0);
    }

    [Test]
    [Timeout(Constants.Timeout)]
    public async Task RunToEndAsync_HandlesOutputIntEvent(CancellationToken CancellationToken)
    {
        var processor = new MockEventProcessor([OutputInt(123), End(0)]);
        var pipe = new Pipe();

        var readTask = Task.Run(async () =>
        {
            var result = await pipe.Reader.ReadAsync(CancellationToken);
            var content = Encoding.UTF8.GetString(result.Buffer.ToArray());
            await Assert.That(content).IsEqualTo("123");
            pipe.Reader.AdvanceTo(result.Buffer.End);
            pipe.Reader.Complete();
        }, CancellationToken);

        var exitCode = await PipeProcessorExtensions.RunToEndAsync(processor, null, pipe.Writer, CancellationToken);

        await readTask;
        await Assert.That(exitCode).IsEqualTo(0);
    }

    [Test]
    [Timeout(Constants.Timeout)]
    public async Task RunToEndAsync_HandlesInputCharEvent(CancellationToken CancellationToken)
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

        await Assert.That(capturedChar).IsEqualTo('X');
    }

    [Test]
    [Timeout(Constants.Timeout)]
    public async Task RunToEndAsync_HandlesInputIntEvent(CancellationToken CancellationToken)
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

        await Assert.That(capturedInt).IsEqualTo(123);
    }

    [Test]
    [Timeout(Constants.Timeout)]
    public async Task RunToEnd_HandlesEndEvent(CancellationToken CancellationToken)
    {
        var processor = new MockEventProcessor([End(99)]);
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
        var exitCode = PipeProcessorExtensions.RunToEnd(processor, null, null, CancellationToken);
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
        await Assert.That(exitCode).IsEqualTo(99);
    }

    [Test]
    [Timeout(Constants.Timeout)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullReader(CancellationToken CancellationToken)
    {
        var capturedChar = ' ';
        var processor = new MockEventProcessor([
            InputChar(c => capturedChar = c)
        ]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [Test]
    [Timeout(Constants.Timeout)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullWriter(CancellationToken CancellationToken)
    {
        var processor = new MockEventProcessor([OutputChar('A')]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [Test]
    [Timeout(Constants.Timeout)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullReader_InputInt(CancellationToken CancellationToken)
    {
        var capturedInt = 0;
        var processor = new MockEventProcessor([
            InputInt(i => capturedInt = i)
        ]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }

    [Test]
    [Timeout(Constants.Timeout)]
    public async Task RunToEndAsync_ThrowsArgumentNullExceptionOnNullWriter_OutputInt(CancellationToken CancellationToken)
    {
        var processor = new MockEventProcessor([OutputInt(123)]);
        await Assert.ThrowsAsync<ArgumentNullException>(() => PipeProcessorExtensions.RunToEndAsync(processor, null, null, CancellationToken).AsTask());
    }
}

file static class Constants
{
    public const int Timeout = 2000;
}
