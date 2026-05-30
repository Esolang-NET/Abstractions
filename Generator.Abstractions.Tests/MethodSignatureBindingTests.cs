namespace Esolang.Generator.Tests;

[TestClass]
public class MethodSignatureBindingTests
{
    [TestMethod]
    public void Properties_CheckCorrectly()
    {
        // IsAsync: Task 系
        var bindingTask = new MethodSignatureBinding(MethodReturnKind.Task, MethodInputKind.None, MethodOutputKind.None, "", "", null, null, false, []);
        Assert.IsTrue(bindingTask.IsAsync);

        // IsAsync: ValueTask 系
        var bindingValueTask = new MethodSignatureBinding(MethodReturnKind.ValueTask, MethodInputKind.None, MethodOutputKind.None, "", "", null, null, false, []);
        Assert.IsTrue(bindingValueTask.IsAsync);

        // IsEnumerable
        var bindingEnum = new MethodSignatureBinding(MethodReturnKind.IEnumerableByte, MethodInputKind.None, MethodOutputKind.None, "", "", null, null, false, []);
        Assert.IsTrue(bindingEnum.IsEnumerable);
        Assert.IsFalse(bindingEnum.IsAsync);

        // IsAsyncEnumerable
        var bindingAsyncEnum = new MethodSignatureBinding(MethodReturnKind.IAsyncEnumerableByte, MethodInputKind.None, MethodOutputKind.None, "", "", null, null, false, []);
        Assert.IsTrue(bindingAsyncEnum.IsAsyncEnumerable);
        Assert.IsTrue(bindingAsyncEnum.IsAsync);

        // HasExplicitInput
        var bindingInput = new MethodSignatureBinding(MethodReturnKind.Void, MethodInputKind.String, MethodOutputKind.None, "s", "", null, null, false, []);
        Assert.IsTrue(bindingInput.HasExplicitInput);

        // HasExplicitOutput
        var bindingOutput = new MethodSignatureBinding(MethodReturnKind.Void, MethodInputKind.None, MethodOutputKind.TextWriter, "", "w", null, null, false, []);
        Assert.IsTrue(bindingOutput.HasExplicitOutput);
    }
}
