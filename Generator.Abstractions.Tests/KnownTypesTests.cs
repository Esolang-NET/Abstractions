using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Esolang.Generator.Tests;

[TestClass]
public class KnownTypesTests
{
    private static Compilation CreateCompilation(string code)
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
            new[] { CSharpSyntaxTree.ParseText(code) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
    }

    [TestMethod]
    public void Constructor_ResolvesSpecialTypes()
    {
        var compilation = CreateCompilation("class C {}");
        var knownTypes = new KnownTypes(compilation);

        Assert.IsNotNull(knownTypes.String);
        Assert.AreEqual(SpecialType.System_String, knownTypes.String.SpecialType);
        Assert.IsNotNull(knownTypes.Byte);
        Assert.AreEqual(SpecialType.System_Byte, knownTypes.Byte.SpecialType);
        Assert.IsNotNull(knownTypes.Int32);
        Assert.AreEqual(SpecialType.System_Int32, knownTypes.Int32.SpecialType);
    }

    [TestMethod]
    public void IsByte_ChecksCorrectly()
    {
        var compilation = CreateCompilation("class C { byte b; }");
        var knownTypes = new KnownTypes(compilation);
        var byteType = compilation.GetSpecialType(SpecialType.System_Byte);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        Assert.IsTrue(knownTypes.IsByte(byteType));
        Assert.IsFalse(knownTypes.IsByte(intType));
        Assert.IsFalse(knownTypes.IsByte(null));
    }

    [TestMethod]
    public void IsInt32_ChecksCorrectly()
    {
        var compilation = CreateCompilation("class C { int i; }");
        var knownTypes = new KnownTypes(compilation);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var byteType = compilation.GetSpecialType(SpecialType.System_Byte);

        Assert.IsTrue(knownTypes.IsInt32(intType));
        Assert.IsFalse(knownTypes.IsInt32(byteType));
        Assert.IsFalse(knownTypes.IsInt32(null));
    }

    [TestMethod]
    public void IsTask_ChecksCorrectly()
    {
        var compilation = CreateCompilation("using System.Threading.Tasks; class C { Task T() => Task.CompletedTask; }");
        var knownTypes = new KnownTypes(compilation);
        var taskType = knownTypes.Task;

        Assert.IsNotNull(taskType);
        Assert.IsTrue(knownTypes.IsTask(taskType));
        Assert.IsFalse(knownTypes.IsTask(compilation.GetSpecialType(SpecialType.System_String)));
        Assert.IsFalse(knownTypes.IsTask(null));
    }

    [TestMethod]
    public void IsString_NullableEnabledChecksCorrectly()
    {
        var compilation = CreateCompilation("""
            #nullable enable
            class C { string? s; }
            """);
        var knownTypes = new KnownTypes(compilation);
        var classC = compilation.GetTypeByMetadataName("C");
        var field = classC?.GetMembers("s").OfType<IFieldSymbol>().FirstOrDefault();
        
        Assert.IsNotNull(field);
        Assert.IsTrue(knownTypes.IsString(field.Type, isNullable: true));
        Assert.IsFalse(knownTypes.IsString(field.Type, isNullable: false));
    }

    [TestMethod]
    public void IsString_NullableDisabledChecksCorrectly()
    {
        var compilation = CreateCompilation("""
            #nullable disable
            class C { string s; }
            """);
        var knownTypes = new KnownTypes(compilation);
        var classC = compilation.GetTypeByMetadataName("C");
        var field = classC?.GetMembers("s").OfType<IFieldSymbol>().FirstOrDefault();

        Assert.IsNotNull(field);
        Assert.IsTrue(knownTypes.IsString(field.Type, isNullable: false));
        Assert.IsFalse(knownTypes.IsString(field.Type, isNullable: true));
    }

    [TestMethod]
    public void IsTaskT_NullableChecksCorrectly()
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

        Assert.IsNotNull(method1);
        Assert.IsNotNull(method2);
        
        Assert.IsTrue(knownTypes.IsTaskT(method1.ReturnType, isNullable: false));
        Assert.IsFalse(knownTypes.IsTaskT(method1.ReturnType, isNullable: true));
        
        Assert.IsTrue(knownTypes.IsTaskT(method2.ReturnType, isNullable: true));
        Assert.IsFalse(knownTypes.IsTaskT(method2.ReturnType, isNullable: false));
    }

    [TestMethod]
    public void IsValueTaskT_NullableChecksCorrectly()
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

        Assert.IsNotNull(method1);
        Assert.IsNotNull(method2);
        
        Assert.IsTrue(knownTypes.IsValueTaskT(method1.ReturnType, isNullable: false));
        Assert.IsFalse(knownTypes.IsValueTaskT(method1.ReturnType, isNullable: true));
        
        Assert.IsTrue(knownTypes.IsValueTaskT(method2.ReturnType, isNullable: true));
        Assert.IsFalse(knownTypes.IsValueTaskT(method2.ReturnType, isNullable: false));
    }
}
