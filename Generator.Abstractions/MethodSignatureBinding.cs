using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Esolang.Generator;

/// <summary>
/// Represents the result of binding a method signature for generation.
/// </summary>
public record struct MethodSignatureBinding(
    bool IsValid,
    MethodReturnKind ReturnKind,
    MethodInputKind InputKind,
    MethodOutputKind OutputKind,
    string InputExpression,
    string OutputExpression,
    string? CancellationTokenName,
    string? LoggerExpression,
    bool IsLoggerFromParameter,
    IReadOnlyList<IParameterSymbol> UnhandledParameters,
    string? ErrorId = null,
    Location? Location = null)
{
    /// <summary>Gets a value indicating whether the method has an explicit input mechanism.</summary>
    public readonly bool HasExplicitInput => InputKind != MethodInputKind.None;

    /// <summary>Gets a value indicating whether the method has an explicit output mechanism.</summary>
    public readonly bool HasExplicitOutput => OutputKind != MethodOutputKind.None;

    /// <summary>Gets a value indicating whether the method is asynchronous.</summary>
    public readonly bool IsAsync => ReturnKind switch
    {
        MethodReturnKind.Task or MethodReturnKind.TaskInt32 or MethodReturnKind.TaskString or MethodReturnKind.TaskNullableString or
        MethodReturnKind.ValueTask or MethodReturnKind.ValueTaskInt32 or MethodReturnKind.ValueTaskString or MethodReturnKind.ValueTaskNullableString or
        MethodReturnKind.IAsyncEnumerableByte => true,
        _ => false
    };

    /// <summary>Gets a value indicating whether the method returns an enumerable.</summary>
    public readonly bool IsEnumerable => ReturnKind == MethodReturnKind.IEnumerableByte;

    /// <summary>Gets a value indicating whether the method returns an async enumerable.</summary>
    public readonly bool IsAsyncEnumerable => ReturnKind == MethodReturnKind.IAsyncEnumerableByte;
}
