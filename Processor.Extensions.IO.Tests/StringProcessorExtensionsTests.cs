using Esolang.Processor;
using Esolang.Processor.Extensions.IO;
using System.Text;

namespace Esolang.Processor.Tests;

public class StringProcessorExtensionsTests
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
    public async Task RunToStringAsync_ReturnsCorrectOutput(CancellationToken CancellationToken)
    {
        var processor = new MockEventProcessor([
            IOEvent.OutputChar('H'),
            IOEvent.OutputChar('i'),
            IOEvent.End(0)
        ]);

        var result = await StringProcessorExtensions.RunToStringAsync(processor, null, CancellationToken);

        await Assert.That(result).IsEqualTo("Hi");
    }

    [Test]
    [Timeout(2000)]
    public async Task RunToString_ReturnsCorrectOutput(CancellationToken CancellationToken)
    {
        var processor = new MockEventProcessor([
            IOEvent.OutputChar('A'),
            IOEvent.OutputChar('B'),
            IOEvent.End(0)
        ]);

#pragma warning disable CS0618
        var result = StringProcessorExtensions.RunToString(processor, null, CancellationToken);
#pragma warning restore CS0618
        await Assert.That(result).IsEqualTo("AB");
    }

    [Test]
    [Timeout(2000)]
    public async Task RunToEndAsync_HandlesOutputCharEvent(CancellationToken CancellationToken)
    {
        var processor = new MockEventProcessor([IOEvent.OutputChar('A'), IOEvent.End(0)]);
        var output = new StringBuilder();

        var exitCode = await StringProcessorExtensions.RunToEndAsync(processor, null, output, CancellationToken);

        await Assert.That(output.ToString()).IsEqualTo("A");
        await Assert.That(exitCode).IsEqualTo(0);
    }
}
