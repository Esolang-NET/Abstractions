using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Esolang.Generator.Tests;

public class NullableTest
{
    [Test]
    public async Task CheckNullableContext(CancellationToken CancellationToken)
    {
        var compilation = CSharpCompilation.Create("Test",
            [CSharpSyntaxTree.ParseText("class C {}", cancellationToken: CancellationToken)],
            null,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // デフォルトは Disable であるはず
        await Assert.That(compilation.Options.NullableContextOptions).IsEqualTo(NullableContextOptions.Disable);
    }
}
