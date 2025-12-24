// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents a non-generic snapshot of a parameter's name, current value, and last value.
/// </summary>
/// <remarks>
/// This struct is used to pass parameter state information to shared change handlers
/// that need to coordinate changes across multiple parameters.
/// </remarks>
[DebuggerDisplay("{Name}: {LastValue} -> {Value}")]
public readonly struct ParameterStateValue
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the current value of the parameter.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Gets the last value of the parameter before the change.
    /// </summary>
    public object? LastValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterStateValue"/> struct.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="lastValue">The last value of the parameter.</param>
    /// <param name="value">The current value of the parameter.</param>
    internal ParameterStateValue(string name, object? lastValue, object? value)
    {
        Name = name;
        LastValue = lastValue;
        Value = value;
    }
}

/// <summary>
/// A collection of parameter state values that allows efficient lookup by parameter name.
/// </summary>
/// <remarks>
/// This type is similar to <see cref="Microsoft.AspNetCore.Components.ParameterView"/> but for parameter state values.
/// It provides O(1) lookup performance for accessing parameter last values by name using a frozen dictionary.
/// </remarks>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ParameterStateCollectionDebugView))]
public readonly struct ParameterStateCollection
{
    internal readonly IReadOnlyDictionary<string, ParameterStateValue>? Dictionary;

    /// <summary>
    /// Gets an empty <see cref="ParameterStateCollection"/>.
    /// </summary>
    public static ParameterStateCollection Empty { get; } = new(null);

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
    public bool TryGetValue<T>(string parameterName, [NotNullWhen(true)] out T? value, [NotNullWhen(true)] out T? lastValue)
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
}

/// <summary>
/// Combines <see cref="ParameterView"/> and <see cref="ParameterStateCollection"/> to provide
/// complete information about parameter changes including current values and last values.
/// </summary>
/// <remarks>
/// This type is passed to shared change handlers that need to coordinate changes across multiple parameters.
/// It provides access to both the current parameter values via <see cref="ParameterView"/> and 
/// the last values via <see cref="ParameterStateCollection"/>.
/// </remarks>
[DebuggerDisplay("ParameterStates.Count = {ParameterStates.Count}")]
public readonly struct ParameterChangedContext
{
    /// <summary>
    /// Gets a snapshot of the component's <see cref="ParameterView"/> at the time the parameter change was detected.
    /// </summary>
    /// <remarks>
    /// Use this <see cref="ParameterView"/> to read current parameter values that were supplied together with the changed parameter.
    /// This snapshot reflects the raw parameter set Blazor provided during parameter assignment.
    /// </remarks>
    public ParameterView ParameterView { get; }

    /// <summary>
    /// Gets the collection of parameter state values containing last and current values.
    /// </summary>
    /// <remarks>
    /// Use this collection to access the last values of parameters before they changed.
    /// This is particularly useful for shared handlers that need to coordinate changes across multiple parameters.
    /// </remarks>
    public ParameterStateCollection ParameterStates { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterChangedContext"/> struct.
    /// </summary>
    /// <param name="parameterView">The parameter view snapshot.</param>
    /// <param name="parameterStates">The collection of parameter state values.</param>
    internal ParameterChangedContext(ParameterView parameterView, ParameterStateCollection parameterStates)
    {
        ParameterView = parameterView;
        ParameterStates = parameterStates;
    }

    public EffectiveParameterResult<TParameter1, TParameter2> ResolveEffectiveParameter<TParameter1, TParameter2>(
        ParameterState<TParameter1> parameterState1,
        ParameterState<TParameter2> parameterState2,
        string dominantParameterName)
    {
        var parameterState1Internal = (ParameterStateInternal<TParameter1>)parameterState1;
        var parameterState2Internal = (ParameterStateInternal<TParameter2>)parameterState2;

        var (hasParameter1Changed, parameter1Value) = CheckParameterChange(parameterState1Internal);
        var (hasParameter2Changed, parameter2Value) = CheckParameterChange(parameterState2Internal);

        // If neither changed
        if (!hasParameter1Changed && !hasParameter2Changed)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.None();
        }

        // If only one changed
        if (hasParameter1Changed != hasParameter2Changed)
        {
            return ResolveWhenOneChanged(
                hasParameter1Changed,
                parameter1Value,
                parameter2Value,
                parameterState1Internal.Metadata.ParameterName,
                parameterState2Internal.Metadata.ParameterName);
        }

        // Both changed
        return ResolveWhenBothChanged(
            parameter1Value,
            parameter2Value,
            dominantParameterName,
            parameterState1Internal.Metadata.ParameterName,
            parameterState2Internal.Metadata.ParameterName);
    }

    private (bool HasChanged, TParameter? Value) CheckParameterChange<TParameter>(ParameterStateInternal<TParameter> parameterStateInternal)
    {
        if (ParameterStates.TryGetValue<TParameter>(
            parameterStateInternal.Metadata.ParameterName,
            out _,
            out var lastValue))
        {
            var comparer = parameterStateInternal.ExtractComparer(ParameterView);
            var hasChanged = ParameterView.HasParameterChanged(
                parameterStateInternal.Metadata.ParameterName,
                lastValue,
                out var currentValue,
                comparer);

            return (hasChanged, currentValue);
        }

        return (false, default);
    }

    private static EffectiveParameterResult<TParameter1, TParameter2> ResolveWhenOneChanged<TParameter1, TParameter2>(
        bool hasParameter1Changed,
        TParameter1? parameter1Value,
        TParameter2? parameter2Value,
        string parameter1Name,
        string parameter2Name)
    {
        if (hasParameter1Changed)
        {
            // If parameter1 changed to null and parameter2 is non-null, prefer parameter2
            if (parameter1Value is null && parameter2Value is not null)
            {
                return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter2(parameter2Name, parameter2Value);
            }

            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter1(parameter1Name, parameter1Value);
        }

        // parameter2 changed
        // If parameter2 changed to null and parameter1 is non-null, prefer parameter1
        if (parameter2Value is null && parameter1Value is not null)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter1(parameter1Name, parameter1Value);
        }

        return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter2(parameter2Name, parameter2Value);
    }

    private static EffectiveParameterResult<TParameter1, TParameter2> ResolveWhenBothChanged<TParameter1, TParameter2>(
        TParameter1? parameter1Value,
        TParameter2? parameter2Value,
        string dominantParameterName,
        string parameter1Name,
        string parameter2Name)
    {
        var parameter1IsNonNull = parameter1Value is not null;
        var parameter2IsNonNull = parameter2Value is not null;

        // Prefer non-null value when only one is non-null
        if (parameter1IsNonNull && !parameter2IsNonNull)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter1(parameter1Name, parameter1Value!);
        }

        if (!parameter1IsNonNull && parameter2IsNonNull)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter2(parameter2Name, parameter2Value!);
        }

        // Both non-null or both null, use dominant parameter
        return ResolveDominantParameter(
            parameter1Value,
            parameter2Value,
            dominantParameterName,
            parameter1Name,
            parameter2Name);
    }

    private static EffectiveParameterResult<TParameter1, TParameter2> ResolveDominantParameter<TParameter1, TParameter2>(
        TParameter1? parameter1Value,
        TParameter2? parameter2Value,
        string dominantParameterName,
        string parameter1Name,
        string parameter2Name)
    {
        if (dominantParameterName == parameter1Name)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter1(parameter1Name, parameter1Value!);
        }

        if (dominantParameterName == parameter2Name)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter2(parameter2Name, parameter2Value!);
        }

        throw new ArgumentException($"Unknown dominant parameter '{dominantParameterName}'.");
    }

    /// <summary>
    /// Gets an empty <see cref="ParameterChangedContext"/>.
    /// </summary>
    public static ParameterChangedContext Empty { get; } = new(ParameterView.Empty, ParameterStateCollection.Empty);
}

/// <summary>
/// Debugger type proxy for <see cref="ParameterStateCollection"/> that provides a better view of the collection in the debugger.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class ParameterStateCollectionDebugView
{
    private readonly ParameterStateCollection _collection;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterStateCollectionDebugView"/> class.
    /// </summary>
    /// <param name="collection">The collection to provide a debug view for.</param>
    public ParameterStateCollectionDebugView(ParameterStateCollection collection)
    {
        _collection = collection;
    }

    /// <summary>
    /// Gets an array of parameter state values for display in the debugger.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public ParameterStateValue[] Items
    {
        get
        {
            if (_collection.Count == 0)
            {
                return [];
            }

            var items = new ParameterStateValue[_collection.Count];
            var index = 0;
            var dictionary = _collection.Dictionary;
            if (dictionary is not null)
            {
                foreach (var kvp in dictionary)
                {
                    items[index++] = kvp.Value;
                }
            }

            return items;
        }
    }
}
