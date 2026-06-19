namespace Esolang.Generator.Tests;

public class MethodSignatureBindingTests
{
    [Test]
    public async Task Properties_CheckCorrectly()
    {
        // IsAsync: Task 系
        var bindingTask = new MethodSignatureBinding(MethodReturnKind.Task, MethodInputKind.None, MethodOutputKind.None, "", "", null, null, false, []);
        await Assert.That(bindingTask.IsAsync).IsTrue();

        // IsAsync: ValueTask 系
        var bindingValueTask = new MethodSignatureBinding(MethodReturnKind.ValueTask, MethodInputKind.None, MethodOutputKind.None, "", "", null, null, false, []);
        await Assert.That(bindingValueTask.IsAsync).IsTrue();

        // IsEnumerable
        var bindingEnum = new MethodSignatureBinding(MethodReturnKind.IEnumerableByte, MethodInputKind.None, MethodOutputKind.None, "", "", null, null, false, []);
        await Assert.That(bindingEnum.IsEnumerable).IsTrue();
        await Assert.That(bindingEnum.IsAsync).IsFalse();

        // IsAsyncEnumerable
        var bindingAsyncEnum = new MethodSignatureBinding(MethodReturnKind.IAsyncEnumerableByte, MethodInputKind.None, MethodOutputKind.None, "", "", null, null, false, []);
        await Assert.That(bindingAsyncEnum.IsAsyncEnumerable).IsTrue();
        await Assert.That(bindingAsyncEnum.IsAsync).IsTrue();

        // HasExplicitInput
        var bindingInput = new MethodSignatureBinding(MethodReturnKind.Void, MethodInputKind.String, MethodOutputKind.None, "s", "", null, null, false, []);
        await Assert.That(bindingInput.HasExplicitInput).IsTrue();

        // HasExplicitOutput
        var bindingOutput = new MethodSignatureBinding(MethodReturnKind.Void, MethodInputKind.None, MethodOutputKind.TextWriter, "", "w", null, null, false, []);
        await Assert.That(bindingOutput.HasExplicitOutput).IsTrue();
    }
}
