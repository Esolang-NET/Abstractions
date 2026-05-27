using Microsoft.CodeAnalysis;

namespace Esolang.Generator;

/// <summary>
/// Defines metadata names for commonly used types in esolang source generators.
/// </summary>
public static class KnownTypeNames
{
    public const string Task = "System.Threading.Tasks.Task";
    public const string TaskT = "System.Threading.Tasks.Task`1";
    public const string ValueTask = "System.Threading.Tasks.ValueTask";
    public const string ValueTaskT = "System.Threading.Tasks.ValueTask`1";
    public const string IEnumerableT = "System.Collections.Generic.IEnumerable`1";
    public const string IAsyncEnumerableT = "System.Collections.Generic.IAsyncEnumerable`1";
    public const string PipeReader = "System.IO.Pipelines.PipeReader";
    public const string PipeWriter = "System.IO.Pipelines.PipeWriter";
    public const string TextReader = "System.IO.TextReader";
    public const string TextWriter = "System.IO.TextWriter";
    public const string CancellationToken = "System.Threading.CancellationToken";
    public const string ILogger = "Microsoft.Extensions.Logging.ILogger";
    public const string ILoggerT = "Microsoft.Extensions.Logging.ILogger`1";
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
