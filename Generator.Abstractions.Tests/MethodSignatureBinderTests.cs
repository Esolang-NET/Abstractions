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
#if NET472_OR_GREATER
            typeof(ValueTask),
            typeof(IAsyncEnumerable<>)
#endif
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
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
    }

    private (IMethodSymbol, Compilation) GetMethodAndCompilation(string code, string methodName)
    {
        var tree = CSharpSyntaxTree.ParseText("#nullable enable\n" + code, cancellationToken: TestContext.CancellationToken);
        var compilation = baseCompilation.AddSyntaxTrees(tree);
        var classC = compilation.GetTypeByMetadataName("C");
        return (classC!.GetMembers(methodName).OfType<IMethodSymbol>().First(), compilation);
    }

    [TestMethod]
    public void Bind_VoidMethod_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("class C { void M() {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid, $"binding: {binding}");
        Assert.AreEqual(MethodReturnKind.Void, binding.ReturnKind, $"binding: {binding}");
    }

    [TestMethod]
    public void Bind_TaskMethod_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading.Tasks; class C { Task M() => Task.CompletedTask; }", "M");
        var knownTypes = new KnownTypes(compilation);
        
        Assert.IsNotNull(knownTypes.Task, "KnownTypes.Task is null");
        
        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid, $"binding: {binding}");
        Assert.AreEqual(MethodReturnKind.Task, binding.ReturnKind, $"binding: {binding}");
    }

    [TestMethod]
    public void Bind_ValueTaskMethod_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading.Tasks; class C { ValueTask M() => default; }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid, $"binding: {binding}");
        Assert.AreEqual(MethodReturnKind.ValueTask, binding.ReturnKind, $"binding: {binding}");
    }

    [TestMethod]
    public void Bind_TaskTMethod_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading.Tasks; class C { Task<int> M() => Task.FromResult(0); }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid, $"binding: {binding}");
        Assert.AreEqual(MethodReturnKind.TaskInt32, binding.ReturnKind, $"binding: {binding}");
    }

    [TestMethod]
    public void Bind_ValueTaskTMethod_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading.Tasks; class C { ValueTask<int> M() => default; }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid, $"binding: {binding}");
        Assert.AreEqual(MethodReturnKind.ValueTaskInt32, binding.ReturnKind, $"binding: {binding}");
    }

    [TestMethod]
    public void Bind_StringInput_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("class C { void M(string s) {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid, $"binding: {binding}");
        Assert.AreEqual(MethodInputKind.String, binding.InputKind);
        Assert.AreEqual("s", binding.InputExpression);
    }

    [TestMethod]
    public void Bind_TextReaderInput_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO; class C { void M(TextReader r) {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid, $"binding: {binding}");
        Assert.AreEqual(MethodInputKind.TextReader, binding.InputKind);
        Assert.AreEqual("r", binding.InputExpression);
    }

    [TestMethod]
    public void Bind_TextWriterOutput_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO; class C { void M(TextWriter w) {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid, $"binding: {binding}");
        Assert.AreEqual(MethodOutputKind.TextWriter, binding.OutputKind);
        Assert.AreEqual("w", binding.OutputExpression);
    }

    [TestMethod]
    public void Bind_LoggerInField_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("""
            using Microsoft.Extensions.Logging;
            class C {
                ILogger _logger;
                C(ILogger logger) => _logger = logger;
                void M() {}
            }
            """, "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid);
        Assert.IsFalse(binding.IsLoggerFromParameter);
        Assert.AreEqual("_logger", binding.LoggerExpression);
    }

    [TestMethod]
    public void Bind_LoggerInBaseClass_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("""
            using Microsoft.Extensions.Logging;
            class B { protected ILogger _logger; }
            class C : B { void M() {} }
            """, "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid);
        Assert.IsFalse(binding.IsLoggerFromParameter);
        Assert.AreEqual("_logger", binding.LoggerExpression);
    }

    [TestMethod]
    public void Bind_LoggerInConstructorParameter_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("""
            using Microsoft.Extensions.Logging;
            class C {
                C(ILogger logger) { }
                void M() {}
            }
            """, "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid);
        Assert.IsTrue(binding.IsLoggerFromParameter);
        Assert.AreEqual("logger", binding.LoggerExpression);
    }

    [TestMethod]
    public void Bind_CancellationToken_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading; class C { void M(CancellationToken ct) {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid);
        Assert.AreEqual("ct", binding.CancellationTokenName);
    }

    [TestMethod]
    public void Bind_UnhandledParameters_AddedToUnhandledList()
    {
        var (method, compilation) = GetMethodAndCompilation("class C { void M(int i, string s) {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid);
        Assert.AreEqual(1, binding.UnhandledParameters.Count);
        Assert.AreEqual("i", binding.UnhandledParameters[0].Name);
    }

    [TestMethod]
    public void Bind_Integrated_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("""
            using System.Threading;
            using System.Threading.Tasks;
            using Microsoft.Extensions.Logging;
            class C {
                ILogger _logger;
                C(ILogger logger) { _logger = logger; }
                Task<int> M(string input, CancellationToken ct) => Task.FromResult(0);
            }
            """, "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        
        Assert.IsTrue(binding.IsValid, $"binding: {binding}");
        Assert.AreEqual(MethodReturnKind.TaskInt32, binding.ReturnKind);
        Assert.AreEqual(MethodInputKind.String, binding.InputKind);
        Assert.AreEqual("input", binding.InputExpression);
        Assert.AreEqual("ct", binding.CancellationTokenName);
        Assert.IsFalse(binding.IsLoggerFromParameter);
        Assert.AreEqual("_logger", binding.LoggerExpression);
    }

    [TestMethod]
    public void Bind_InvalidReturnKind_ReturnsInvalidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("class C { float M() => 0.0f; }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsFalse(binding.IsValid);
        Assert.AreEqual("ES0001", binding.ErrorId); // invalidReturnTypeErrorId
    }

    [TestMethod]
    public void Bind_PipeReaderInput_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO.Pipelines; class C { void M(PipeReader r) {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid);
        Assert.AreEqual(MethodInputKind.PipeReader, binding.InputKind);
        Assert.AreEqual("r", binding.InputExpression);
    }

    [TestMethod]
    public void Bind_PipeWriterOutput_ReturnsValidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO.Pipelines; class C { void M(PipeWriter w) {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsTrue(binding.IsValid);
        Assert.AreEqual(MethodOutputKind.PipeWriter, binding.OutputKind);
        Assert.AreEqual("w", binding.OutputExpression);
    }

    [TestMethod]
    public void Bind_RefParameter_ReturnsInvalidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("class C { void M(ref string s) {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsFalse(binding.IsValid);
        Assert.AreEqual("ES0002", binding.ErrorId); // invalidParameterErrorId
    }

    [TestMethod]
    public void Bind_DuplicateStringInput_ReturnsInvalidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("class C { void M(string s1, string s2) {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsFalse(binding.IsValid);
        Assert.AreEqual("ES0003", binding.ErrorId); // DuplicateParameterErrorId
    }

    [TestMethod]
    public void Bind_DuplicateTextReaderInput_ReturnsInvalidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO; class C { void M(TextReader r1, TextReader r2) {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsFalse(binding.IsValid);
        Assert.AreEqual("ES0003", binding.ErrorId); // DuplicateParameterErrorId
    }

    [TestMethod]
    public void Bind_DuplicateTextWriterOutput_ReturnsInvalidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO; class C { void M(TextWriter w1, TextWriter w2) {} }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsFalse(binding.IsValid);
        Assert.AreEqual("ES0003", binding.ErrorId); // DuplicateParameterErrorId
    }

    [TestMethod]
    public void Bind_ReturnOutputConflict_ReturnsInvalidBinding()
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO; class C { string M(TextWriter w) => \"\"; }", "M");
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        Assert.IsFalse(binding.IsValid);
        Assert.AreEqual("ES0004", binding.ErrorId); // ReturnOutputConflictErrorId
    }
}
