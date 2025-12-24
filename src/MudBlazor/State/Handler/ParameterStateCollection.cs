// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// A collection of parameter state values that allows efficient lookup by parameter name.
/// </summary>
/// <remarks>
/// This type is similar to <see cref="Microsoft.AspNetCore.Components.ParameterView"/> but for parameter state values.
/// </remarks>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ParameterStateCollectionDebugView))]
public readonly struct ParameterStateCollection
{
    internal readonly IReadOnlyDictionary<string, ParameterStateValue>? Dictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterStateCollection"/> struct.
    /// </summary>
    /// <param name="dictionary">The dictionary of parameter state values keyed by parameter name.</param>
    internal ParameterStateCollection(IReadOnlyDictionary<string, ParameterStateValue>? dictionary)
    {
        Dictionary = dictionary;
    }

    /// <summary>
    /// Gets the number of parameter state values in the collection.
    /// </summary>
    public int Count => Dictionary?.Count ?? 0;

    /// <summary>
    /// Attempts to get a parameter state value by its name.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">When this method returns, contains the parameter state value if found; otherwise, the default value.</param>
    /// <returns><c>true</c> if the parameter was found; otherwise, <c>false</c>.</returns>
    public bool TryGetValue(string parameterName, out ParameterStateValue value)
    {
        if (Dictionary is not null && Dictionary.TryGetValue(parameterName, out value))
        {
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to get a strongly-typed value for a parameter.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter value.</typeparam>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">When this method returns, contains the current value if found and successfully cast; otherwise, the default value.</param>
    /// <param name="lastValue">When this method returns, contains the last value if found and successfully cast; otherwise, the default value.</param>
    /// <returns><c>true</c> if the parameter was found; otherwise, <c>false</c>.</returns>
    public bool TryGetValue<T>(string parameterName, [MaybeNullWhen(false)] out T value, [MaybeNullWhen(false)] out T lastValue)
    {
        if (TryGetValue(parameterName, out var parameterState))
        {
            value = (T)parameterState.Value!;
            lastValue = (T)parameterState.LastValue!;
            return true;
        }

        value = default;
        lastValue = default;
        return false;
    }

    /// <summary>
    /// Gets the parameter state value by its name.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The parameter state value.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the parameter is not found.</exception>
    public ParameterStateValue this[string parameterName]
    {
        get
        {
            if (Dictionary is not null && Dictionary.TryGetValue(parameterName, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException($"The parameter '{parameterName}' was not found.");
        }
    }

    /// <summary>
    /// Gets an empty <see cref="ParameterStateCollection"/>.
    /// </summary>
    public static ParameterStateCollection Empty { get; } = new(null);
}
