// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <typeparam name="T">Parameter's type.</typeparam>
#nullable enable
internal class ParameterState<T> : IParameterComponentLifeCycle, IEquatable<ParameterState<T>>
{
    private T? _lastValue;
    private readonly Func<T> _getParameterValueFunc;
    private readonly Func<EventCallback<T>> _eventCallbackFunc;

    /// <summary>
    /// Gets the associated parameter name of the component's <see cref="ParameterAttribute"/>.
    /// </summary>
    public string ParameterName { get; }

    [MemberNotNullWhen(true, nameof(ParameterChangedHandler))]
    public bool HasHandler => ParameterChangedHandler is not null;

    [MemberNotNullWhen(true, nameof(_lastValue), nameof(Value))]
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets the current value.
    /// </summary>
    public T? Value { get; private set; }

    public IParameterChangedHandler? ParameterChangedHandler { get; }

    private ParameterState(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, IParameterChangedHandler? parameterChangedHandler = null)
    {
        ParameterName = parameterName;
        _getParameterValueFunc = getParameterValueFunc;
        _eventCallbackFunc = eventCallbackFunc;
        ParameterChangedHandler = parameterChangedHandler;
        _lastValue = default;
        Value = default;
    }

    public Task SetValueAsync(T value)
    {
        if (!EqualityComparer<T>.Default.Equals(Value, value))
        {
            Value = value;
            _lastValue = value;

            return _eventCallbackFunc().InvokeAsync(value);
        }

        return Task.CompletedTask;
    }

    public void OnInitialized()
    {
        IsInitialized = true;
        var currentParameterValue = _getParameterValueFunc();
        Value = currentParameterValue;
        _lastValue = currentParameterValue;
    }

    public void OnParametersSet()
    {
        var currentParameterValue = _getParameterValueFunc();
        if (!EqualityComparer<T>.Default.Equals(_lastValue, currentParameterValue))
        {
            Value = currentParameterValue;
            _lastValue = currentParameterValue;
        }
    }

    public Task ParameterChangeHandleAsync()
    {
        return HasHandler ? ParameterChangedHandler.HandleAsync() : Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool HasParameterChanged(ParameterView parameters)
    {
        var currentParameterValue = _getParameterValueFunc();

        return parameters.HasParameterChanged(ParameterName, currentParameterValue);
    }

    public static ParameterState<T> Attach(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, IParameterChangedHandler? parameterChangedHandler = null) => new(parameterName, getParameterValueFunc, eventCallbackFunc, parameterChangedHandler);

    /// <inheritdoc />
    public bool Equals(ParameterState<T>? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        // We expect parameter name to be unique within the component (considering inheritance).
        // To ensure uniqueness, the equals method is utilized to prevent registering the same parameter multiple times.
        // Each [Parameter] should have a one-to-one relationship with its corresponding ParameterState.
        return ParameterName == other.ParameterName;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ParameterState<T> parameterState && Equals(parameterState);

    /// <inheritdoc />
    public override int GetHashCode() => ParameterName.GetHashCode();
}
