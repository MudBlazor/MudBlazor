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
internal struct ParameterState<T> : IParameterSynchronization
{
    private readonly bool _fireOnSynchronize;
    private readonly Func<T>? _getParameterValueFunc;
    private readonly EventCallback<T> _eventCallback;

    private T _lastValue;

    [MemberNotNullWhen(true, nameof(_getParameterValueFunc))]
    public readonly bool IsAttached => _getParameterValueFunc is not null;

    public T Value { get; private set; }

    private ParameterState(Func<T> getParameterValueFunc, EventCallback<T> eventCallback, bool fireOnSynchronize)
    {
        var currentParameterValue = getParameterValueFunc();
        _getParameterValueFunc = getParameterValueFunc;
        _lastValue = currentParameterValue;
        Value = currentParameterValue;
        _eventCallback = eventCallback;
        _fireOnSynchronize = fireOnSynchronize;
    }

    public Task SetValueAsync(T value)
    {
        if (!IsAttached)
        {
            throw new InvalidOperationException("StateManager is not attached.");
        }

        if (!EqualityComparer<T>.Default.Equals(Value, value))
        {
            Value = value;
            _lastValue = value;

            return _eventCallback.InvokeAsync(value);
        }

        return Task.CompletedTask;
    }

    public void OnInitialized()
    {
        if (!IsAttached)
        {
            throw new InvalidOperationException("StateManager is not attached.");
        }

        var currentParameterValue = _getParameterValueFunc();
        Value = currentParameterValue;
        _lastValue = currentParameterValue;
    }

    public Task OnParametersSetAsync()
    {
        if (!IsAttached)
        {
            throw new InvalidOperationException("StateManager is not attached.");
        }

        var currentParameterValue = _getParameterValueFunc();
        if (!EqualityComparer<T>.Default.Equals(_lastValue, currentParameterValue))
        {
            Value = currentParameterValue;
            _lastValue = currentParameterValue;

            if (_fireOnSynchronize)
            {
                return _eventCallback.InvokeAsync(currentParameterValue);
            }
        }

        return Task.CompletedTask;
    }

    public static ParameterState<T> Attach(Func<T> getParameterValueFunc, EventCallback<T> eventCallback = default, bool fireOnSynchronize = false) => new(getParameterValueFunc, eventCallback, fireOnSynchronize);
}
