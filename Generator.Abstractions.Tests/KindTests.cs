using System.Diagnostics.CodeAnalysis;

namespace Esolang.Generator.Tests;

public class KindTests
{
    [Test]
    [SuppressMessage("Usage", "TUnitAssertions0005:Assert.That(...) should not be used with a constant value")]
    public async Task MethodInputKind_HasExpectedValues()
    {
        await Assert.That((int)MethodInputKind.None).IsEqualTo(0);
        await Assert.That((int)MethodInputKind.String).IsEqualTo(1);
        await Assert.That((int)MethodInputKind.TextReader).IsEqualTo(2);
        await Assert.That((int)MethodInputKind.PipeReader).IsEqualTo(3);
    }

    [Test]
    [SuppressMessage("Usage", "TUnitAssertions0005:Assert.That(...) should not be used with a constant value")]
    public async Task MethodOutputKind_HasExpectedValues()
    {
        await Assert.That((int)MethodOutputKind.None).IsEqualTo(0);
        await Assert.That((int)MethodOutputKind.TextWriter).IsEqualTo(1);
        await Assert.That((int)MethodOutputKind.PipeWriter).IsEqualTo(2);
    }

    [Test]
    [SuppressMessage("Usage", "TUnitAssertions0005:Assert.That(...) should not be used with a constant value")]
    public async Task MethodReturnKind_HasExpectedValues()
    {
        await Assert.That((int)MethodReturnKind.Invalid).IsEqualTo(0);
        await Assert.That((int)MethodReturnKind.Void).IsEqualTo(1);
        await Assert.That((int)MethodReturnKind.Int32).IsEqualTo(2);
        await Assert.That((int)MethodReturnKind.String).IsEqualTo(3);
        await Assert.That((int)MethodReturnKind.NullableString).IsEqualTo(4);
        await Assert.That((int)MethodReturnKind.Task).IsEqualTo(5);
        await Assert.That((int)MethodReturnKind.TaskInt32).IsEqualTo(6);
        await Assert.That((int)MethodReturnKind.TaskString).IsEqualTo(7);
        await Assert.That((int)MethodReturnKind.TaskNullableString).IsEqualTo(8);
        await Assert.That((int)MethodReturnKind.ValueTask).IsEqualTo(9);
        await Assert.That((int)MethodReturnKind.ValueTaskInt32).IsEqualTo(10);
        await Assert.That((int)MethodReturnKind.ValueTaskString).IsEqualTo(11);
        await Assert.That((int)MethodReturnKind.ValueTaskNullableString).IsEqualTo(12);
        await Assert.That((int)MethodReturnKind.IEnumerableByte).IsEqualTo(13);
        await Assert.That((int)MethodReturnKind.IAsyncEnumerableByte).IsEqualTo(14);
    }
}
