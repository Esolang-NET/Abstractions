using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Esolang.Generator.Tests;

public class MethodSignatureBinderTests
{
    readonly Compilation baseCompilation = default!;

    public MethodSignatureBinderTests()
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

    (IMethodSymbol, Compilation) GetMethodAndCompilation(string code, string methodName, CancellationToken CancellationToken)
    {
        var tree = CSharpSyntaxTree.ParseText("#nullable enable\n" + code, cancellationToken: CancellationToken);
        var compilation = baseCompilation.AddSyntaxTrees(tree);
        var classC = compilation.GetTypeByMetadataName("C");
        Assert.NotNull(classC, "Failed to get symbol for class C");
        var methodSymbol = classC.GetMembers(methodName).OfType<IMethodSymbol>().FirstOrDefault();
        Assert.NotNull(methodSymbol, $"Failed to get symbol for method {methodName}");
        return (methodSymbol, compilation);
    }

    [Test]
    public async Task Bind_VoidMethod_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("class C { void M() {} }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.ReturnKind).IsEqualTo(MethodReturnKind.Void).Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_TaskMethod_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading.Tasks; class C { Task M() => Task.CompletedTask; }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        await Assert.That(knownTypes.Task).IsNotNull().Because($"KnownTypes.Task is null");

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.ReturnKind).IsEqualTo(MethodReturnKind.Task).Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_ValueTaskMethod_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading.Tasks; class C { ValueTask M() => default; }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.ReturnKind).IsEqualTo(MethodReturnKind.ValueTask).Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_TaskTMethod_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading.Tasks; class C { Task<int> M() => Task.FromResult(0); }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.ReturnKind).IsEqualTo(MethodReturnKind.TaskInt32).Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_ValueTaskTMethod_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading.Tasks; class C { ValueTask<int> M() => default; }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.ReturnKind).IsEqualTo(MethodReturnKind.ValueTaskInt32).Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_StringInput_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("class C { void M(string s) {} }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.InputKind).IsEqualTo(MethodInputKind.String).Because($"binding: {binding}");
        await Assert.That(binding.InputExpression).IsEqualTo("s").Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_TextReaderInput_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO; class C { void M(TextReader r) {} }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.InputKind).IsEqualTo(MethodInputKind.TextReader).Because($"binding: {binding}");
        await Assert.That(binding.InputExpression).IsEqualTo("r").Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_TextWriterOutput_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO; class C { void M(TextWriter w) {} }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.OutputKind).IsEqualTo(MethodOutputKind.TextWriter).Because($"binding: {binding}");
        await Assert.That(binding.OutputExpression).IsEqualTo("w").Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_LoggerInField_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("""
            using Microsoft.Extensions.Logging;
            class C {
                ILogger _logger;
                C(ILogger logger) => _logger = logger;
                void M() {}
            }
            """, "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.IsLoggerFromParameter).IsFalse().Because($"binding: {binding}");
        await Assert.That(binding.LoggerExpression).IsEqualTo("_logger").Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_LoggerInBaseClass_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("""
            using Microsoft.Extensions.Logging;
            class B { protected ILogger _logger; }
            class C : B { void M() {} }
            """, "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.IsLoggerFromParameter).IsFalse().Because($"binding: {binding}");
        await Assert.That(binding.LoggerExpression).IsEqualTo("_logger").Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_LoggerInConstructorParameter_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("""
            using Microsoft.Extensions.Logging;
            class C {
                C(ILogger logger) { }
                void M() {}
            }
            """, "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.IsLoggerFromParameter).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.LoggerExpression).IsEqualTo("logger").Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_CancellationToken_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("using System.Threading; class C { void M(CancellationToken ct) {} }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.CancellationTokenName).IsEqualTo("ct").Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_UnhandledParameters_AddedToUnhandledList(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("class C { void M(int i, string s) {} }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.UnhandledParameters).Count().IsEqualTo(1).Because($"binding: {binding}");
        await Assert.That(binding.UnhandledParameters[0].Name).IsEqualTo("i").Because($"binding: {binding}");
    }

    [Test]
    public async Task Bind_Integrated_ReturnsValidBinding(CancellationToken CancellationToken)
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
            """, "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);

        await Assert.That(binding.IsValid).IsTrue().Because($"binding: {binding}");
        await Assert.That(binding.ReturnKind).IsEqualTo(MethodReturnKind.TaskInt32);
        await Assert.That(binding.InputKind).IsEqualTo(MethodInputKind.String);
        await Assert.That(binding.InputExpression).IsEqualTo("input");
        await Assert.That(binding.CancellationTokenName).IsEqualTo("ct");
        await Assert.That(binding.IsLoggerFromParameter).IsFalse();
        await Assert.That(binding.LoggerExpression).IsEqualTo("_logger");
    }

    [Test]
    public async Task Bind_InvalidReturnKind_ReturnsInvalidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("class C { float M() => 0.0f; }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsFalse();
        await Assert.That(binding.Error!.Kind).IsEqualTo(BindingErrorKind.UnsupportedReturnType); // invalidReturnTypeErrorId
    }

    [Test]
    public async Task Bind_PipeReaderInput_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO.Pipelines; class C { void M(PipeReader r) {} }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue();
        await Assert.That(binding.InputKind).IsEqualTo(MethodInputKind.PipeReader);
        await Assert.That(binding.InputExpression).IsEqualTo("r");
    }

    [Test]
    public async Task Bind_PipeWriterOutput_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO.Pipelines; class C { void M(PipeWriter w) {} }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue();
        await Assert.That(binding.OutputKind).IsEqualTo(MethodOutputKind.PipeWriter);
        await Assert.That(binding.OutputExpression).IsEqualTo("w");
    }

    [Test]
    public async Task Bind_RefParameter_ReturnsInvalidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("class C { void M(ref string s) {} }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsFalse();
        await Assert.That(binding.Error!.Kind).IsEqualTo(BindingErrorKind.InvalidParameterModifier); // invalidParameterErrorId
    }

    [Test]
    public async Task Bind_DuplicateStringInput_ReturnsInvalidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("class C { void M(string s1, string s2) {} }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsFalse();
        await Assert.That(binding.Error!.Kind).IsEqualTo(BindingErrorKind.DuplicateInput); // DuplicateParameterErrorId
    }

    [Test]
    public async Task Bind_DuplicateTextReaderInput_ReturnsInvalidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO; class C { void M(TextReader r1, TextReader r2) {} }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsFalse();
        await Assert.That(binding.Error!.Kind).IsEqualTo(BindingErrorKind.DuplicateInput); // DuplicateParameterErrorId
    }

    [Test]
    public async Task Bind_DuplicateTextWriterOutput_ReturnsInvalidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO; class C { void M(TextWriter w1, TextWriter w2) {} }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsFalse();
        await Assert.That(binding.Error!.Kind).IsEqualTo(BindingErrorKind.DuplicateOutput); // DuplicateParameterErrorId
    }

    [Test]
    public async Task Bind_ReturnOutputConflict_ReturnsInvalidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("using System.IO; class C { string M(TextWriter w) => \"\"; }", "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsFalse();
        await Assert.That(binding.Error!.Kind).IsEqualTo(BindingErrorKind.ReturnOutputConflict); // ReturnOutputConflictErrorId
    }

    [Test]
    public async Task Bind_DuplicateLoggerParameter_ReturnsInvalidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("""
            using Microsoft.Extensions.Logging;
            class C {
                void M(ILogger l1, ILogger l2) {}
            }
            """, "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsFalse();
        await Assert.That(binding.Error!.Kind).IsEqualTo(BindingErrorKind.DuplicateLogger); // DuplicateParameterErrorId
    }

    [Test]
    public async Task Bind_LoggerInField_CanBeReferencedByName_ReturnsValidBinding(CancellationToken CancellationToken)
    {
        var (method, compilation) = GetMethodAndCompilation("""
            using Microsoft.Extensions.Logging;
            class C {
                public ILogger loggerField;
                void M() {}
            }
            """, "M", CancellationToken);
        var knownTypes = new KnownTypes(compilation);

        var binding = MethodSignatureBinder.Bind(method, knownTypes);
        await Assert.That(binding.IsValid).IsTrue();
        await Assert.That(binding.IsLoggerFromParameter).IsFalse();
        await Assert.That(binding.LoggerExpression).IsEqualTo("loggerField");
    }
}
