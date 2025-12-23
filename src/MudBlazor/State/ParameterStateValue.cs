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

    /// <summary>
    /// Returns a string representation of the parameter state value showing the transition from last value to current value.
    /// </summary>
    /// <returns>A string in the format "Name: LastValue -> Value".</returns>
    public override string ToString() => $"{Name}: {LastValue} -> {Value}";
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

        var parameterState1Comparer = parameterState1Internal.ExtractComparer(ParameterView);
        var parameterState2Comparer = parameterState2Internal.ExtractComparer(ParameterView);

        var hasParameter1Changed = false;
        var hasParameter2Changed = false;

        TParameter1? parameter1Value = default;
        TParameter2? parameter2Value = default;

        // Get last/current values
        if (ParameterStates.TryGetValue<TParameter1>(parameterState1Internal.Metadata.ParameterName, out _, out var parameterState1LastValue))
        {
            hasParameter1Changed = ParameterView.HasParameterChanged(
                parameterState1Internal.Metadata.ParameterName, parameterState1LastValue, out parameter1Value, parameterState1Comparer);
        }

        if (ParameterStates.TryGetValue<TParameter2>(parameterState2Internal.Metadata.ParameterName, out _, out var parameterState2LastValue))
        {
            hasParameter2Changed = ParameterView.HasParameterChanged(
                parameterState2Internal.Metadata.ParameterName, parameterState2LastValue, out parameter2Value, parameterState2Comparer);
        }

        // If neither changed
        if (!hasParameter1Changed && !hasParameter2Changed)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.None();
        }

        // If both changed, prefer non-null value
        if (hasParameter1Changed && hasParameter2Changed)
        {
            var parameter1IsNonNull = parameter1Value is not null;
            var parameter2IsNonNull = parameter2Value is not null;

            if (parameter1IsNonNull && !parameter2IsNonNull)
            {
                return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter1(parameterState1Internal.Metadata.ParameterName, parameter1Value!);
            }

            if (!parameter1IsNonNull && parameter2IsNonNull)
            {
                return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter2(parameterState2Internal.Metadata.ParameterName, parameter2Value!);
            }

            // If both non-null or both null, fallback to dominant parameter
            if (dominantParameterName == parameterState1Internal.Metadata.ParameterName)
            {
                return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter1(parameterState1Internal.Metadata.ParameterName, parameter1Value!);
            }

            if (dominantParameterName == parameterState2Internal.Metadata.ParameterName)
            {
                return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter2(parameterState2Internal.Metadata.ParameterName, parameter2Value!);
            }

            throw new ArgumentException($"Unknown dominant parameter '{dominantParameterName}'.");
        }

        // If only one changed, pick the one that is non-null
        if (hasParameter1Changed && parameter1Value is not null)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter1(parameterState1Internal.Metadata.ParameterName, parameter1Value);
        }

        if (hasParameter2Changed && parameter2Value is not null)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter2(parameterState2Internal.Metadata.ParameterName, parameter2Value);
        }

        // Fallback
        return EffectiveParameterResult<TParameter1, TParameter2>.None();
    }

    ///// <summary>
    ///// Returns a string representation of the parameter changed context showing the count of parameter states.
    ///// </summary>
    ///// <returns>A string indicating the number of parameter states in the context.</returns>
    //public override string ToString() => $"ParameterChangedContext (ParameterStates.Count = {ParameterStates.Count})";

    /// <summary>
    /// Gets an empty <see cref="ParameterChangedContext"/>.
    /// </summary>
    public static ParameterChangedContext Empty { get; } = new(ParameterView.Empty, ParameterStateCollection.Empty);
}

/// <summary>
/// Debugger type proxy for <see cref="ParameterStateCollection"/> that provides a better view of the collection in the debugger.
/// </summary>
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
