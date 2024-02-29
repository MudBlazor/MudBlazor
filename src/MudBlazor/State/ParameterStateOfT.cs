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
internal class ParameterState<T> : IParameterComponentLifeCycle
{
    private readonly Func<T>? _getParameterValueFunc;
    private readonly Func<EventCallback<T>>? _eventCallbackFunc;

    private T? _lastValue;

    public string ParameterName { get; }

    [MemberNotNullWhen(true, nameof(_getParameterValueFunc), nameof(_eventCallbackFunc))]
    public bool IsAttached => _getParameterValueFunc is not null;

    [MemberNotNullWhen(true, nameof(ParameterChangedHandler))]
    public bool HasHandler => ParameterChangedHandler is not null;

    [MemberNotNullWhen(true, nameof(_lastValue), nameof(Value))]
    public bool IsInitialized { get; private set; }

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
        if (!IsAttached)
        {
            throw new InvalidOperationException("ParameterState is not attached.");
        }

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
        if (!IsAttached)
        {
            throw new InvalidOperationException("ParameterState is not attached.");
        }

        IsInitialized = true;
        var currentParameterValue = _getParameterValueFunc();
        Value = currentParameterValue;
        _lastValue = currentParameterValue;
    }

    public void OnParametersSet()
    {
        if (!IsAttached)
        {
            throw new InvalidOperationException("ParameterState is not attached.");
        }

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

    public bool HasParameterChanged(ParameterView parameters)
    {
        if (!IsAttached)
        {
            throw new InvalidOperationException("ParameterState is not attached.");
        }

        var currentParameterValue = _getParameterValueFunc();

        return parameters.HasParameterChanged(ParameterName, currentParameterValue);
    }

    public static ParameterState<T> Attach(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, IParameterChangedHandler? parameterChangedHandler = null) => new(parameterName, getParameterValueFunc, eventCallbackFunc, parameterChangedHandler);
}
