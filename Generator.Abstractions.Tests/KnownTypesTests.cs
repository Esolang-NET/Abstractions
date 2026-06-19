using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Esolang.Generator.Tests;

public class KnownTypesTests
{
    static Compilation CreateCompilation(string code)
    {
        var assemblies = new[]
        {
            typeof(object).Assembly,
            typeof(System.Threading.Tasks.Task).Assembly,
            typeof(System.Linq.Enumerable).Assembly
        }.ToList();
#if NET472_OR_GREATER
        assemblies.Add(typeof(System.Threading.Tasks.ValueTask).Assembly);
#endif
        var references = assemblies
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();

        return CSharpCompilation.Create("TestCompilation",
            [CSharpSyntaxTree.ParseText(code)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
    }

    [Test]
    public async Task Constructor_ResolvesSpecialTypes()
    {
        var compilation = CreateCompilation("class C {}");
        var knownTypes = new KnownTypes(compilation);

        Assert.NotNull(knownTypes.String);
        await Assert.That(knownTypes.String.SpecialType).IsEqualTo(SpecialType.System_String);
        Assert.NotNull(knownTypes.Byte);
        await Assert.That(knownTypes.Byte.SpecialType).IsEqualTo(SpecialType.System_Byte);
        Assert.NotNull(knownTypes.Int32);
        await Assert.That(knownTypes.Int32.SpecialType).IsEqualTo(SpecialType.System_Int32);
    }

    [Test]
    public async Task IsByte_ChecksCorrectly()
    {
        var compilation = CreateCompilation("class C { byte b; }");
        var knownTypes = new KnownTypes(compilation);
        var byteType = compilation.GetSpecialType(SpecialType.System_Byte);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        await Assert.That(knownTypes.IsByte(byteType)).IsTrue();
        await Assert.That(knownTypes.IsByte(intType)).IsFalse();
        await Assert.That(knownTypes.IsByte(null)).IsFalse();
    }

    [Test]
    public async Task IsInt32_ChecksCorrectly()
    {
        var compilation = CreateCompilation("class C { int i; }");
        var knownTypes = new KnownTypes(compilation);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var byteType = compilation.GetSpecialType(SpecialType.System_Byte);

        await Assert.That(knownTypes.IsInt32(intType)).IsTrue();
        await Assert.That(knownTypes.IsInt32(byteType)).IsFalse();
        await Assert.That(knownTypes.IsInt32(null)).IsFalse();
    }

    [Test]
    public async Task IsTask_ChecksCorrectly()
    {
        var compilation = CreateCompilation("using System.Threading.Tasks; class C { Task T() => Task.CompletedTask; }");
        var knownTypes = new KnownTypes(compilation);
        var taskType = knownTypes.Task;

        Assert.NotNull(taskType);
        await Assert.That(knownTypes.IsTask(taskType)).IsTrue();
        await Assert.That(knownTypes.IsTask(compilation.GetSpecialType(SpecialType.System_String))).IsFalse();
        await Assert.That(knownTypes.IsTask(null)).IsFalse();
    }

    [Test]
    public async Task IsString_NullableEnabledChecksCorrectly()
    {
        var compilation = CreateCompilation("""
            #nullable enable
            class C { string? s; }
            """);
        var knownTypes = new KnownTypes(compilation);
        var classC = compilation.GetTypeByMetadataName("C");
        var field = classC?.GetMembers("s").OfType<IFieldSymbol>().FirstOrDefault();

        Assert.NotNull(field);
        await Assert.That(knownTypes.IsString(field.Type, isNullable: true)).IsTrue();
        await Assert.That(knownTypes.IsString(field.Type, isNullable: false)).IsFalse();
    }

    [Test]
    public async Task IsString_NullableDisabledChecksCorrectly()
    {
        var compilation = CreateCompilation("""
            #nullable disable
            class C { string s; }
            """);
        var knownTypes = new KnownTypes(compilation);
        var classC = compilation.GetTypeByMetadataName("C");
        var field = classC?.GetMembers("s").OfType<IFieldSymbol>().FirstOrDefault();

        Assert.NotNull(field);
        await Assert.That(knownTypes.IsString(field.Type, isNullable: false)).IsTrue();
        await Assert.That(knownTypes.IsString(field.Type, isNullable: true)).IsFalse();
    }

    [Test]
    public async Task IsTaskT_NullableChecksCorrectly()
    {
        var compilation = CreateCompilation("""
            #nullable enable
            using System.Threading.Tasks;
            class C {
                Task<string> T1() => Task.FromResult("");
                Task<string?> T2() => Task.FromResult<string?>(null);
            }
            """);
        var knownTypes = new KnownTypes(compilation);
        var classC = compilation.GetTypeByMetadataName("C");
        var method1 = classC?.GetMembers("T1").OfType<IMethodSymbol>().FirstOrDefault();
        var method2 = classC?.GetMembers("T2").OfType<IMethodSymbol>().FirstOrDefault();

        Assert.NotNull(method1);
        Assert.NotNull(method2);

        await Assert.That(knownTypes.IsTaskT(method1.ReturnType, isNullable: false)).IsTrue();
        await Assert.That(knownTypes.IsTaskT(method1.ReturnType, isNullable: true)).IsFalse();

        await Assert.That(knownTypes.IsTaskT(method2.ReturnType, isNullable: true)).IsTrue();
        await Assert.That(knownTypes.IsTaskT(method2.ReturnType, isNullable: false)).IsFalse();
    }

    [Test]
    public async Task IsValueTaskT_NullableChecksCorrectly()
    {
        var compilation = CreateCompilation("""
            #nullable enable
            using System.Threading.Tasks;
            class C {
                ValueTask<string> T1() => new ValueTask<string>("");
                ValueTask<string?> T2() => new ValueTask<string?>(null);
            }
            """);
        var knownTypes = new KnownTypes(compilation);
        var classC = compilation.GetTypeByMetadataName("C");
        var method1 = classC?.GetMembers("T1").OfType<IMethodSymbol>().FirstOrDefault();
        var method2 = classC?.GetMembers("T2").OfType<IMethodSymbol>().FirstOrDefault();

        Assert.NotNull(method1);
        Assert.NotNull(method2);

        await Assert.That(knownTypes.IsValueTaskT(method1.ReturnType, isNullable: false)).IsTrue();
        await Assert.That(knownTypes.IsValueTaskT(method1.ReturnType, isNullable: true)).IsFalse();

        await Assert.That(knownTypes.IsValueTaskT(method2.ReturnType, isNullable: true)).IsTrue();
        await Assert.That(knownTypes.IsValueTaskT(method2.ReturnType, isNullable: false)).IsFalse();
    }
}
