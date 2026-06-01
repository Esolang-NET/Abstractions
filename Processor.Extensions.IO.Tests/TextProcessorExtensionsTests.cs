using Esolang.Processor;
using Esolang.Processor.Extensions.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using static Esolang.Processor.IOEvent;

namespace Esolang.Processor.Tests;

[TestClass]
public class TextProcessorExtensionsTests(TestContext TestContext)
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
    public async Task RunToEndAsync_HandlesOutputCharEvent()
    {
        var processor = new MockEventProcessor([OutputChar('A'), End(0)]);
        using var output = new StringWriter();

        var exitCode = await TextProcessorExtensions.RunToEndAsync(processor, null, output, CancellationToken);

        Assert.AreEqual("A", output.ToString());
        Assert.AreEqual(0, exitCode);
    }

    [TestMethod]
    [Timeout(2000, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesOutputIntEvent()
    {
        var processor = new MockEventProcessor([OutputInt(123), End(0)]);
        using var output = new StringWriter();

        var exitCode = await TextProcessorExtensions.RunToEndAsync(processor, null, output, CancellationToken);

        Assert.AreEqual("123", output.ToString());
        Assert.AreEqual(0, exitCode);
    }

    [TestMethod]
    [Timeout(2000, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesInputCharEvent()
    {
        var capturedChar = ' ';
        var processor = new MockEventProcessor([
            InputChar(c => capturedChar = c),
            End(0)
        ]);
        using var input = new StringReader("X");

        await TextProcessorExtensions.RunToEndAsync(processor, input, null, CancellationToken);

        Assert.AreEqual('X', capturedChar);
    }

    [TestMethod]
    [Timeout(2000, CooperativeCancellation = true)]
    public async Task RunToEndAsync_HandlesInputIntEvent()
    {
        var capturedInt = 0;
        var processor = new MockEventProcessor([
            InputInt(i => capturedInt = i),
            End(0)
        ]);
        using var input = new StringReader("123");

        await TextProcessorExtensions.RunToEndAsync(processor, input, null, CancellationToken);

        Assert.AreEqual(123, capturedInt);
    }

    [TestMethod]
    [Timeout(2000, CooperativeCancellation = true)]
    public void RunToEnd_HandlesEndEvent()
    {
        var processor = new MockEventProcessor([End(88)]);
#pragma warning disable CS0618
        var exitCode = TextProcessorExtensions.RunToEnd(processor, null, null, CancellationToken);
#pragma warning restore CS0618
        Assert.AreEqual(88, exitCode);
    }
}
