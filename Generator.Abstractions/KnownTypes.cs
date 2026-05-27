using Microsoft.CodeAnalysis;

namespace Esolang.Generator;

/// <summary>
/// Holds resolved type symbols for a compilation.
/// </summary>
public readonly struct KnownTypes
{
    /// <summary>The <see cref="string"/> type symbol.</summary>
    public readonly INamedTypeSymbol? String;
    /// <summary>The <see cref="byte"/> type symbol.</summary>
    public readonly INamedTypeSymbol? Byte;
    /// <summary>The <see cref="int"/> type symbol.</summary>
    public readonly INamedTypeSymbol? Int32;
    /// <summary>The <see cref="System.Threading.Tasks.Task"/> type symbol.</summary>
    public readonly INamedTypeSymbol? Task;
    /// <summary>The <see cref="System.Threading.Tasks.Task{TResult}"/> type symbol.</summary>
    public readonly INamedTypeSymbol? TaskT;
    /// <summary>The <see cref="System.Threading.Tasks.ValueTask"/> type symbol.</summary>
    public readonly INamedTypeSymbol? ValueTask;
    /// <summary>The <see cref="System.Threading.Tasks.ValueTask{TResult}"/> type symbol.</summary>
    public readonly INamedTypeSymbol? ValueTaskT;
    /// <summary>The <see cref="System.Collections.Generic.IEnumerable{T}"/> type symbol.</summary>
    public readonly INamedTypeSymbol? IEnumerableT;
    /// <summary>The <c>System.Collections.Generic.IAsyncEnumerable{T}</c> type symbol.</summary>
    public readonly INamedTypeSymbol? IAsyncEnumerableT;
    /// <summary>The <c>System.IO.Pipelines.PipeReader</c> type symbol.</summary>
    public readonly INamedTypeSymbol? PipeReader;
    /// <summary>The <c>System.IO.Pipelines.PipeWriter</c> type symbol.</summary>
    public readonly INamedTypeSymbol? PipeWriter;
    /// <summary>The <see cref="System.IO.TextReader"/> type symbol.</summary>
    public readonly INamedTypeSymbol? TextReader;
    /// <summary>The <see cref="System.IO.TextWriter"/> type symbol.</summary>
    public readonly INamedTypeSymbol? TextWriter;
    /// <summary>The <c>System.Threading.CancellationToken</c> type symbol.</summary>
    public readonly INamedTypeSymbol? CancellationToken;
    /// <summary>The <c>Microsoft.Extensions.Logging.ILogger</c> type symbol.</summary>
    public readonly INamedTypeSymbol? ILogger;
    /// <summary>The <c>Microsoft.Extensions.Logging.ILogger{T}</c> type symbol.</summary>
    public readonly INamedTypeSymbol? ILoggerT;

    /// <summary>
    /// Initializes a new instance of the <see cref="KnownTypes"/> struct.
    /// </summary>
    /// <param name="compilation">The compilation to resolve types from.</param>
    public KnownTypes(Compilation compilation)
    {
        String = compilation.GetSpecialType(SpecialType.System_String);
        Byte = compilation.GetSpecialType(SpecialType.System_Byte);
        Int32 = compilation.GetSpecialType(SpecialType.System_Int32);

        Task = compilation.GetBestTypeByMetadataName("System.Threading.Tasks.Task");
        TaskT = compilation.GetBestTypeByMetadataName("System.Threading.Tasks.Task`1");
        ValueTask = compilation.GetBestTypeByMetadataName("System.Threading.Tasks.ValueTask");
        ValueTaskT = compilation.GetBestTypeByMetadataName("System.Threading.Tasks.ValueTask`1");
        IEnumerableT = compilation.GetBestTypeByMetadataName("System.Collections.Generic.IEnumerable`1");
        IAsyncEnumerableT = compilation.GetBestTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1");
        PipeReader = compilation.GetBestTypeByMetadataName("System.IO.Pipelines.PipeReader");
        PipeWriter = compilation.GetBestTypeByMetadataName("System.IO.Pipelines.PipeWriter");
        TextReader = compilation.GetBestTypeByMetadataName("System.IO.TextReader");
        TextWriter = compilation.GetBestTypeByMetadataName("System.IO.TextWriter");
        CancellationToken = compilation.GetBestTypeByMetadataName("System.Threading.CancellationToken");
        ILogger = compilation.GetBestTypeByMetadataName("Microsoft.Extensions.Logging.ILogger");
        ILoggerT = compilation.GetBestTypeByMetadataName("Microsoft.Extensions.Logging.ILogger`1");
    }

    private static bool EqualsDefinition(ITypeSymbol? type, ISymbol? symbol) =>
        type != null && symbol != null && SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, symbol);

    private static bool EqualsType(ITypeSymbol? type, ISymbol? symbol) =>
        type != null && symbol != null && SymbolEqualityComparer.Default.Equals(type, symbol);

    /// <summary>Gets a value indicating whether the type is <see cref="string"/>.</summary>
    /// <param name="type">The type to check.</param>
    /// <param name="isNullable">Optional: Whether to check for nullability.</param>
    public bool IsString(ITypeSymbol? type, bool? isNullable = null) =>
        type != null && SymbolEqualityComparer.Default.Equals(type, String) && (isNullable == null || type.NullableAnnotation == (isNullable.Value ? NullableAnnotation.Annotated : NullableAnnotation.NotAnnotated));

    /// <summary>Gets a value indicating whether the type is <see cref="byte"/>.</summary>
    public bool IsByte(ITypeSymbol? type) => EqualsType(type, Byte);
    /// <summary>Gets a value indicating whether the type is <see cref="int"/>.</summary>
    public bool IsInt32(ITypeSymbol? type) => EqualsType(type, Int32);

    /// <summary>Gets a value indicating whether the type is <see cref="System.Threading.Tasks.Task"/>.</summary>
    public bool IsTask(ITypeSymbol? type) => EqualsType(type, Task);
    /// <summary>Gets a value indicating whether the type is <see cref="System.Threading.Tasks.Task{TResult}"/>.</summary>
    public bool IsTaskT(ITypeSymbol? type, bool? isNullable = null)
    {
        if (type is not INamedTypeSymbol named || !EqualsDefinition(named, TaskT)) return false;
        return isNullable == null || named.TypeArguments[0].NullableAnnotation == (isNullable.Value ? NullableAnnotation.Annotated : NullableAnnotation.NotAnnotated);
    }
    /// <summary>Gets a value indicating whether the type is <see cref="System.Threading.Tasks.ValueTask"/>.</summary>
    public bool IsValueTask(ITypeSymbol? type) => EqualsType(type, ValueTask);
    /// <summary>Gets a value indicating whether the type is <see cref="System.Threading.Tasks.ValueTask{TResult}"/>.</summary>
    public bool IsValueTaskT(ITypeSymbol? type, bool? isNullable = null)
    {
        if (type is not INamedTypeSymbol named || !EqualsDefinition(named, ValueTaskT)) return false;
        return isNullable == null || named.TypeArguments[0].NullableAnnotation == (isNullable.Value ? NullableAnnotation.Annotated : NullableAnnotation.NotAnnotated);
    }
    /// <summary>Gets a value indicating whether the type is <see cref="System.Collections.Generic.IEnumerable{T}"/>.</summary>
    public bool IsIEnumerableT(ITypeSymbol? type) => EqualsDefinition(type, IEnumerableT);
    /// <summary>Gets a value indicating whether the type is <c>System.Collections.Generic.IAsyncEnumerable{T}</c>.</summary>
    public bool IsIAsyncEnumerableT(ITypeSymbol? type) => EqualsDefinition(type, IAsyncEnumerableT);

    /// <summary>Gets a value indicating whether the type is <c>System.IO.Pipelines.PipeReader</c>.</summary>
    public bool IsPipeReader(ITypeSymbol? type) => EqualsType(type, PipeReader);
    /// <summary>Gets a value indicating whether the type is <c>System.IO.Pipelines.PipeWriter</c>.</summary>
    public bool IsPipeWriter(ITypeSymbol? type) => EqualsType(type, PipeWriter);
    /// <summary>Gets a value indicating whether the type is <see cref="System.IO.TextReader"/>.</summary>
    public bool IsTextReader(ITypeSymbol? type) => EqualsType(type, TextReader);
    /// <summary>Gets a value indicating whether the type is <see cref="System.IO.TextWriter"/>.</summary>
    public bool IsTextWriter(ITypeSymbol? type) => EqualsType(type, TextWriter);
    /// <summary>Gets a value indicating whether the type is <c>System.Threading.CancellationToken</c>.</summary>
    public bool IsCancellationToken(ITypeSymbol? type) => EqualsType(type, CancellationToken);

    /// <summary>Gets a value indicating whether the type is <see cref="System.Threading.Tasks.Task{String}"/>.</summary>
    public bool IsTaskString(ITypeSymbol? type, bool? isNullable = null) => IsTaskT(type, isNullable) && ((INamedTypeSymbol)type!).TypeArguments[0].SpecialType == SpecialType.System_String;
    /// <summary>Gets a value indicating whether the type is <see cref="System.Threading.Tasks.ValueTask{String}"/>.</summary>
    public bool IsValueTaskString(ITypeSymbol? type, bool? isNullable = null) => IsValueTaskT(type, isNullable) && ((INamedTypeSymbol)type!).TypeArguments[0].SpecialType == SpecialType.System_String;

    /// <summary>Gets a value indicating whether the type is <see cref="System.Threading.Tasks.Task{Int32}"/>.</summary>
    public bool IsTaskInt32(ITypeSymbol? type) => IsTaskT(type) && ((INamedTypeSymbol)type!).TypeArguments[0].SpecialType == SpecialType.System_Int32;
    /// <summary>Gets a value indicating whether the type is <see cref="System.Threading.Tasks.ValueTask{Int32}"/>.</summary>
    public bool IsValueTaskInt32(ITypeSymbol? type) => IsValueTaskT(type) && ((INamedTypeSymbol)type!).TypeArguments[0].SpecialType == SpecialType.System_Int32;

    /// <summary>Gets a value indicating whether the type is <see cref="System.Collections.Generic.IEnumerable{Byte}"/>.</summary>
    public bool IsIEnumerableByte(ITypeSymbol? type) => IsIEnumerableT(type) && ((INamedTypeSymbol)type!).TypeArguments[0].SpecialType == SpecialType.System_Byte;
    /// <summary>Gets a value indicating whether the type is <c>System.Collections.Generic.IAsyncEnumerable{Byte}</c>.</summary>
    public bool IsIAsyncEnumerableByte(ITypeSymbol? type) => IsIAsyncEnumerableT(type) && ((INamedTypeSymbol)type!).TypeArguments[0].SpecialType == SpecialType.System_Byte;

    /// <summary>Gets a value indicating whether the type is a logger type (<c>ILogger</c> or <c>ILogger{T}</c>).</summary>
    public bool IsLogger(ITypeSymbol? type)
    {
        if (type == null) return false;
        if (EqualsType(type, ILogger) || EqualsDefinition(type, ILoggerT)) return true;
        foreach (var iface in type.AllInterfaces)
        {
            if (EqualsType(iface, ILogger) || EqualsDefinition(iface, ILoggerT)) return true;
        }
        return false;
    }
}

/// <summary>
/// Provides utility methods for resolving types from a <see cref="Compilation"/>.
/// </summary>
public static class TypeResolutionExtensions
{
    /// <summary>
    /// Resolves the best <see cref="INamedTypeSymbol"/> for the specified metadata name.
    /// </summary>
    public static INamedTypeSymbol? GetBestTypeByMetadataName(this Compilation compilation, string metadataName)
    {
        var type = compilation.GetTypeByMetadataName(metadataName);
        if (type != null) return type;

        foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            var found = assembly.GetTypeByMetadataName(metadataName);
            if (found != null) return found;
        }
        return null;
    }
}
