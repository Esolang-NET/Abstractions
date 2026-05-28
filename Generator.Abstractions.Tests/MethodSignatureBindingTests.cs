using Esolang.Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis;

namespace Esolang.Generator.Tests;

[TestClass]
public class MethodSignatureBindingTests
{
    [TestMethod]
    public void PropertiesWorkAsExpected()
    {
        var binding = new MethodSignatureBinding(
            IsValid: true,
            ReturnKind: MethodReturnKind.Void,
            InputKind: MethodInputKind.None,
            OutputKind: MethodOutputKind.None,
            InputExpression: "",
            OutputExpression: "",
            CancellationTokenName: null,
            LoggerExpression: null,
            IsLoggerFromParameter: false,
            UnhandledParameters: []
        );

        Assert.IsTrue(binding.IsValid);
        Assert.IsFalse(binding.HasExplicitInput);
        Assert.IsFalse(binding.HasExplicitOutput);
        Assert.IsFalse(binding.IsAsync);
        Assert.IsFalse(binding.IsEnumerable);
        Assert.IsFalse(binding.IsAsyncEnumerable);
    }
}
