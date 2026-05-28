using Esolang.Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis;

namespace Esolang.Generator.Tests;

[TestClass]
public class MethodSignatureBindingTests
{
    [TestMethod]
    public void Properties_CheckCorrectly()
    {
        // IsAsync: Task 系
        var bindingTask = new MethodSignatureBinding(true, MethodReturnKind.Task, MethodInputKind.None, MethodOutputKind.None, "", "", null, null, false, []);
        Assert.IsTrue(bindingTask.IsAsync);
        
        // IsAsync: ValueTask 系
        var bindingValueTask = new MethodSignatureBinding(true, MethodReturnKind.ValueTask, MethodInputKind.None, MethodOutputKind.None, "", "", null, null, false, []);
        Assert.IsTrue(bindingValueTask.IsAsync);

        // IsEnumerable
        var bindingEnum = new MethodSignatureBinding(true, MethodReturnKind.IEnumerableByte, MethodInputKind.None, MethodOutputKind.None, "", "", null, null, false, []);
        Assert.IsTrue(bindingEnum.IsEnumerable);
        Assert.IsFalse(bindingEnum.IsAsync);

        // IsAsyncEnumerable
        var bindingAsyncEnum = new MethodSignatureBinding(true, MethodReturnKind.IAsyncEnumerableByte, MethodInputKind.None, MethodOutputKind.None, "", "", null, null, false, []);
        Assert.IsTrue(bindingAsyncEnum.IsAsyncEnumerable);
        Assert.IsTrue(bindingAsyncEnum.IsAsync);

        // HasExplicitInput
        var bindingInput = new MethodSignatureBinding(true, MethodReturnKind.Void, MethodInputKind.String, MethodOutputKind.None, "s", "", null, null, false, []);
        Assert.IsTrue(bindingInput.HasExplicitInput);

        // HasExplicitOutput
        var bindingOutput = new MethodSignatureBinding(true, MethodReturnKind.Void, MethodInputKind.None, MethodOutputKind.TextWriter, "", "w", null, null, false, []);
        Assert.IsTrue(bindingOutput.HasExplicitOutput);
    }
}
