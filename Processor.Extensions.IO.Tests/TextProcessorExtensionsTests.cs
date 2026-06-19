using Esolang.Processor;
using Esolang.Processor.Extensions.IO;
using System.IO;
using System.Text;
using static Esolang.Processor.IOEvent;

namespace Esolang.Processor.Tests;

public class TextProcessorExtensionsTests
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
    [Timeout(2000)]
    public async Task RunToEndAsync_HandlesOutputCharEvent(CancellationToken CancellationToken)
    {
        var processor = new MockEventProcessor([OutputChar('A'), End(0)]);
        using var output = new StringWriter();

        var exitCode = await TextProcessorExtensions.RunToEndAsync(processor, null, output, CancellationToken);

        await Assert.That(output.ToString()).IsEqualTo("A");
        await Assert.That(exitCode).IsEqualTo(0);
    }

    [Test]
    [Timeout(2000)]
    public async Task RunToEndAsync_HandlesOutputIntEvent(CancellationToken CancellationToken)
    {
        var processor = new MockEventProcessor([OutputInt(123), End(0)]);
        using var output = new StringWriter();

        var exitCode = await TextProcessorExtensions.RunToEndAsync(processor, null, output, CancellationToken);

        await Assert.That(output.ToString()).IsEqualTo("123");
        await Assert.That(exitCode).IsEqualTo(0);
    }

    [Test]
    [Timeout(2000)]
    public async Task RunToEndAsync_HandlesInputCharEvent(CancellationToken CancellationToken)
    {
        var capturedChar = ' ';
        var processor = new MockEventProcessor([
            InputChar(c => capturedChar = c),
            End(0)
        ]);
        using var input = new StringReader("X");

        await TextProcessorExtensions.RunToEndAsync(processor, input, null, CancellationToken);

        await Assert.That(capturedChar).IsEqualTo('X');
    }

    [Test]
    [Timeout(2000)]
    public async Task RunToEndAsync_HandlesInputIntEvent(CancellationToken CancellationToken)
    {
        var capturedInt = 0;
        var processor = new MockEventProcessor([
            InputInt(i => capturedInt = i),
            End(0)
        ]);
        using var input = new StringReader("123");

        await TextProcessorExtensions.RunToEndAsync(processor, input, null, CancellationToken);

        await Assert.That(capturedInt).IsEqualTo(123);
    }

    [Test]
    [Timeout(2000)]
    public async Task RunToEnd_HandlesEndEvent(CancellationToken CancellationToken)
    {
        var processor = new MockEventProcessor([End(88)]);
#pragma warning disable CS0618
        var exitCode = TextProcessorExtensions.RunToEnd(processor, null, null, CancellationToken);
#pragma warning restore CS0618
        await Assert.That(exitCode).IsEqualTo(88);
    }
}
