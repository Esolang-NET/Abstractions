using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Esolang.Generator;
using Basic.Reference.Assemblies;
using System.Text;

namespace Esolang.Generator.Tests;

[TestClass]
public class MethodSignatureBinderTests(TestContext TestContext)
{
    private Compilation baseCompilation = default!;

    void WriteLine(string message) => TestContext.WriteLine(message);

    [TestInitialize]
    public void InitializeCompilation()
    {
        IEnumerable<PortableExecutableReference> references =
#if NET10_0_OR_GREATER
            Net100.References.All;
#elif NET9_0_OR_GREATER
            Net90.References.All;
#elif NET472_OR_GREATER
            Net472.References.All;
#else
            throw new InvalidOperationException("Unsupported target framework for generator tests.");
#endif

        var referenceList = references.ToList();
        var extraTypes = new[] {
            typeof(System.IO.Pipelines.PipeReader),
            typeof(Microsoft.Extensions.Logging.ILogger),
            typeof(ValueTask),
            typeof(IAsyncEnumerable<>)
        };

        foreach (var type in extraTypes)
        {
            var location = type.Assembly.Location;
            if (!string.IsNullOrWhiteSpace(location) && !referenceList.Any(r => Path.GetFileName(r.FilePath) == Path.GetFileName(location)))
            {
                referenceList.Add(MetadataReference.CreateFromFile(location));
            }
        }

        baseCompilation = CSharpCompilation.Create("generatortest",
            references: referenceList,
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable));
    }

    private (IMethodSymbol, Compilation) GetMethodAndCompilation(string code, string methodName)
    {
        // コード全体に対して nullable enable を適用する
        var tree = CSharpSyntaxTree.ParseText("#nullable enable\n" + code, cancellationToken: TestContext.CancellationToken);
        var compilation = baseCompilation.AddSyntaxTrees(tree);
        var classC = compilation.GetTypeByMetadataName("C");
        return (classC!.GetMembers(methodName).OfType<IMethodSymbol>().First(), compilation);
    }
    static string ToString(ITypeSymbol? symbol)
    {
        var builder = new StringBuilder();
        if (symbol is null) return string.Empty;
        builder.Append('(');
        builder.Append(symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        builder.Append(", ");
        builder.Append(nameof(symbol.NullableAnnotation));
        builder.Append(':');
        builder.Append(symbol.NullableAnnotation);
        builder.Append(')');
        return builder.ToString();
    }

    [TestMethod]
    public void Bind_VoidMethod_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("class C { void M() {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        try
        {
            Assert.IsTrue(binding.IsValid);
            Assert.AreEqual(MethodReturnKind.Void, binding.ReturnKind);
        }
        catch (AssertFailedException)
        {
            WriteLine($"binding: {binding}");
            WriteLine($"knownTypes: {knownTypes}");
            WriteLine($"returnType: {ToString(method.ReturnType)}");
            throw;
        }
    }

    [TestMethod]
    public void Bind_TaskMethod_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading.Tasks; class C { Task M() => Task.CompletedTask; }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        try
        {
            Assert.IsTrue(binding.IsValid);
            Assert.AreEqual(MethodReturnKind.Task, binding.ReturnKind);
        }
        catch (AssertFailedException)
        {
            WriteLine($"binding: {binding}");
            WriteLine($"knownTypes: {knownTypes}");
            WriteLine($"returnType: {ToString(method.ReturnType)}");
            throw;
        }
    }

    [TestMethod]
    public void Bind_ValueTaskMethod_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading.Tasks; class C { ValueTask M() => default; }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        try
        {
            Assert.IsTrue(binding.IsValid);
            Assert.AreEqual(MethodReturnKind.ValueTask, binding.ReturnKind);
        }
        catch (AssertFailedException)
        {
            WriteLine($"binding: {binding}");
            WriteLine($"knownTypes: {knownTypes}");
            WriteLine($"returnType: {ToString(method.ReturnType)}");
            throw;
        }
    }

    [TestMethod]
    public void Bind_TaskTMethod_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading.Tasks; class C { Task<int> M() => Task.FromResult(0); }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        try
        {
            Assert.IsTrue(binding.IsValid, $"binding: {binding}");
            Assert.AreEqual(MethodReturnKind.TaskInt32, binding.ReturnKind, $"binding: {binding}");
        }
        catch (AssertFailedException)
        {
            WriteLine($"binding: {binding}");
            WriteLine($"knownTypes: {knownTypes}");
            WriteLine($"returnType: {ToString(method.ReturnType)}");
            throw;
        }
    }

    [TestMethod]
    public void Bind_ValueTaskTMethod_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading.Tasks; class C { ValueTask<int> M() => default; }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        try
        {
            Assert.IsTrue(binding.IsValid, $"binding: {binding}");
            Assert.AreEqual(MethodReturnKind.ValueTaskInt32, binding.ReturnKind, $"binding: {binding}");
        }
        catch (AssertFailedException)
        {
            WriteLine($"binding: {binding}");
            WriteLine($"knownTypes: {knownTypes}");
            WriteLine($"returnType: {ToString(method.ReturnType)}");
            throw;
        }
    }
}
