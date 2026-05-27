using Microsoft.CodeAnalysis;

namespace Esolang.Generator;

/// <summary>
/// Provides a base structure for resolving types from a <see cref="Compilation"/>.
/// </summary>
public readonly struct KnownTypes
{
    /// <summary>
    /// Resolves the best <see cref="INamedTypeSymbol"/> for the specified metadata name.
    /// </summary>
    protected static INamedTypeSymbol? GetBestTypeByMetadataName(Compilation compilation, string metadataName)
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
