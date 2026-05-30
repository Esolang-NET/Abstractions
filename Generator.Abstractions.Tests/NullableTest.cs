using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Esolang.Generator.Tests;

[TestClass]
public class NullableTest(TestContext TestContext)
{
#pragma warning disable MSTEST0054 // TestContext.CancellationTokenSource.Token の代わりに TestContext.CancellationToken を使用する
    CancellationToken CancellationToken => TestContext.CancellationTokenSource.Token;
#pragma warning restore MSTEST0054 // TestContext.CancellationTokenSource.Token の代わりに TestContext.CancellationToken を使用する
    [TestMethod]
    public void CheckNullableContext()
    {
        var compilation = CSharpCompilation.Create("Test",
            [CSharpSyntaxTree.ParseText("class C {}", cancellationToken: CancellationToken)],
            null,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // デフォルトは Disable であるはず
        Assert.AreEqual(NullableContextOptions.Disable, compilation.Options.NullableContextOptions);
    }
}
