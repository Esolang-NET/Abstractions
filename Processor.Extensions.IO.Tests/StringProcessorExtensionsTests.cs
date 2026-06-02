using Esolang.Processor;
using Esolang.Processor.Extensions.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Esolang.Processor.Tests;

[TestClass]
public class StringProcessorExtensionsTests(TestContext TestContext)
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
    public async Task RunToStringAsync_ReturnsCorrectOutput()
    {
        var processor = new MockEventProcessor([
            IOEvent.OutputChar('H'),
            IOEvent.OutputChar('i'),
            IOEvent.End(0)
        ]);

        var result = await StringProcessorExtensions.RunToStringAsync(processor, null, CancellationToken);

        Assert.AreEqual("Hi", result);
    }

    [TestMethod]
    [Timeout(2000, CooperativeCancellation = true)]
    public void RunToString_ReturnsCorrectOutput()
    {
        var processor = new MockEventProcessor([
            IOEvent.OutputChar('A'),
            IOEvent.OutputChar('B'),
            IOEvent.End(0)
        ]);

#pragma warning disable CS0618
        var result = StringProcessorExtensions.RunToString(processor, null, CancellationToken);
#pragma warning restore CS0618
        Assert.AreEqual("AB", result);
    }

    [TestMethod]
    [Timeout(2000, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesOutputCharEvent()
    {
        var processor = new MockEventProcessor([IOEvent.OutputChar('A'), IOEvent.End(0)]);
        var output = new StringBuilder();

        var exitCode = await StringProcessorExtensions.RunToEndAsync(processor, null, output, CancellationToken);

        Assert.AreEqual("A", output.ToString());
        Assert.AreEqual(0, exitCode);
    }
}
