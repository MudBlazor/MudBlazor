// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents the result of resolving which parameter is effective when coordinating between two related parameters.
/// </summary>
/// <typeparam name="T1">The type of the first parameter.</typeparam>
/// <typeparam name="T2">The type of the second parameter.</typeparam>
/// <remarks>
/// This type is used to determine which of two parameters should take precedence when both are available,
/// such as when a component has both a typed parameter and a string parameter that represent the same value.
/// </remarks> second param
public readonly record struct EffectiveParameterResult<T1, T2>
{
    /// <summary>
    /// Gets a value indicating whether an effective parameter was resolved.
    /// </summary>
    /// <value>
    /// <c>true</c> if either parameter was selected as the effective parameter; otherwise, <c>false</c>.
    /// </value>
    public bool HasEffectiveParameter { get; }

    /// <summary>
    /// Gets a value indicating whether the first parameter is the effective parameter.
    /// </summary>
    /// <value>
    /// <c>true</c> if the first parameter was selected as the effective parameter; otherwise, <c>false</c>.
    /// </value>
    [MemberNotNullWhen(true, nameof(Parameter1Value))]
    public bool IsParameter1 { get; }

    /// <summary>
    /// Gets a value indicating whether the second parameter is the effective parameter.
    /// </summary>
    /// <value>
    /// <c>true</c> if the second parameter was selected as the effective parameter; otherwise, <c>false</c>.
    /// </value>
    [MemberNotNullWhen(true, nameof(Parameter2Value))]
    public bool IsParameter2 => HasEffectiveParameter && !IsParameter1;

    /// <summary>
    /// Gets the value of the first parameter.
    /// </summary>
    /// <value>
    /// The value of the first parameter if it was selected; otherwise, <c>default</c>.
    /// </value>
    public T1? Parameter1Value { get; }

    /// <summary>
    /// Gets the value of the second parameter.
    /// </summary>
    /// <value>
    /// The value of the second parameter if it was selected; otherwise, <c>default</c>.
    /// </value>
    public T2? Parameter2Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EffectiveParameterResult{T1, T2}"/> struct.
    /// </summary>
    /// <param name="hasEffectiveParameter">Indicates whether an effective parameter was resolved.</param>
    /// <param name="isParameter1">Indicates whether the first parameter is the effective parameter.</param>
    /// <param name="value1">The value of the first parameter.</param>
    /// <param name="value2">The value of the second parameter.</param>
    private EffectiveParameterResult(
        bool hasEffectiveParameter,
        bool isParameter1,
        T1? value1,
        T2? value2)
    {
        HasEffectiveParameter = hasEffectiveParameter;
        IsParameter1 = isParameter1;
        Parameter1Value = value1;
        Parameter2Value = value2;
    }

    /// <summary>
    /// Creates a result indicating that no effective parameter was resolved.
    /// </summary>
    /// <returns>
    /// An <see cref="EffectiveParameterResult{T1, T2}"/> with <see cref="HasEffectiveParameter"/> set to <c>false</c>.
    /// </returns>
    internal static EffectiveParameterResult<T1, T2> None()
        => new(false, false, default, default);

    /// <summary>
    /// Creates a result indicating that the first parameter is the effective parameter.
    /// </summary>
    /// <param name="name">The name of the first parameter (used for debugging and logging purposes).</param>
    /// <param name="value">The value of the first parameter.</param>
    /// <returns>
    /// An <see cref="EffectiveParameterResult{T1, T2}"/> with <see cref="IsParameter1"/> set to <c>true</c>
    /// and <see cref="Parameter1Value"/> set to the specified value.
    /// </returns>
    internal static EffectiveParameterResult<T1, T2> FromParameter1(
        string name,
        T1? value)
        => new(true, true, value, default);

    /// <summary>
    /// Creates a result indicating that the second parameter is the effective parameter.
    /// </summary>
    /// <param name="name">The name of the second parameter (used for debugging and logging purposes).</param>
    /// <param name="value">The value of the second parameter.</param>
    /// <returns>
    /// An <see cref="EffectiveParameterResult{T1, T2}"/> with <see cref="IsParameter2"/> set to <c>true</c>
    /// and <see cref="Parameter2Value"/> set to the specified value.
    /// </returns>
    internal static EffectiveParameterResult<T1, T2> FromParameter2(
        string name,
        T2? value)
        => new(true, false, default, value);
}
