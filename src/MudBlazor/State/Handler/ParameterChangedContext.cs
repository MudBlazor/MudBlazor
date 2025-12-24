// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
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

    /// <summary>
    /// Resolves which parameter is effective when coordinating changes between two related parameters.
    /// </summary>
    /// <typeparam name="TParameter1">The type of the first parameter.</typeparam>
    /// <typeparam name="TParameter2">The type of the second parameter.</typeparam>
    /// <param name="parameterState1">The state object for the first parameter.</param>
    /// <param name="parameterState2">The state object for the second parameter.</param>
    /// <param name="dominantParameterName">
    /// The name of the parameter that should take precedence if both parameters have changed.
    /// </param>
    /// <returns>
    /// An <see cref="EffectiveParameterResult{TParameter1, TParameter2}"/> indicating which parameter is effective,
    /// including its name and value.
    /// </returns>
    /// <remarks>
    /// This method compares the current and previous values of both parameters, determines which (if any) has changed,
    /// and applies precedence rules to select the effective parameter. If both parameters have changed and are non-null,
    /// the parameter specified by <paramref name="dominantParameterName"/> is selected.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="dominantParameterName"/> does not match either parameter name when both have changed.
    /// </exception>
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

    private ParameterChange<TParameter> CheckParameterChange<TParameter>(ParameterStateInternal<TParameter> parameterStateInternal)
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

            return new ParameterChange<TParameter>(hasChanged, currentValue);
        }

        return new ParameterChange<TParameter>(false, default);
    }

    private static EffectiveParameterResult<TParameter1, TParameter2> ResolveWhenOneChanged<TParameter1, TParameter2>(
        bool hasParameter1Changed,
        TParameter1 parameter1Value,
        TParameter2 parameter2Value,
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
        TParameter1 parameter1Value,
        TParameter2 parameter2Value,
        string dominantParameterName,
        string parameter1Name,
        string parameter2Name)
    {
        var parameter1IsNonNull = parameter1Value is not null;
        var parameter2IsNonNull = parameter2Value is not null;

        // Prefer non-null value when only one is non-null
        if (parameter1IsNonNull && !parameter2IsNonNull)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter1(parameter1Name, parameter1Value);
        }

        if (!parameter1IsNonNull && parameter2IsNonNull)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter2(parameter2Name, parameter2Value);
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
        TParameter1 parameter1Value,
        TParameter2 parameter2Value,
        string dominantParameterName,
        string parameter1Name,
        string parameter2Name)
    {
        if (dominantParameterName == parameter1Name)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter1(parameter1Name, parameter1Value);
        }

        if (dominantParameterName == parameter2Name)
        {
            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter2(parameter2Name, parameter2Value);
        }

        throw new ArgumentException($"Unknown dominant parameter '{dominantParameterName}'.");
    }

    /// <summary>
    /// Gets an empty <see cref="ParameterChangedContext"/>.
    /// </summary>
    public static ParameterChangedContext Empty { get; } = new(ParameterView.Empty, ParameterStateCollection.Empty);

    internal readonly struct ParameterChange<T>(bool hasChanged, T? value)
    {
        public bool HasChanged { get; } = hasChanged;

        [AllowNull]
        public T Value { get; } = value;

        public void Deconstruct(out bool hasChanged, out T value)
        {
            hasChanged = HasChanged;
            value = Value;
        }
    }
}
