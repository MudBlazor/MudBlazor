// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
internal struct StateManager<T>
{
    private readonly bool _fireOnSynchronize;
    private readonly Func<T>? _parameterState;
    private readonly EventCallback<T> _eventCallback;

    private T _lastValue;

    [MemberNotNullWhen(true, nameof(_parameterState))]
    public readonly bool IsAttached => _parameterState is not null;

    public T Value { get; private set; }

    private StateManager(Func<T> parameterState, EventCallback<T> eventCallback, bool fireOnSynchronize)
    {
        var currentParameterValue = parameterState();
        _parameterState = parameterState;
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

    public Task SynchronizeParameterAsync()
    {
        if (!IsAttached)
        {
            throw new InvalidOperationException("StateManager is not attached.");
        }

        var currentParameterValue = _parameterState();
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

    public static StateManager<T> Attach(Func<T> parameterState, EventCallback<T> eventCallback = default, bool fireOnSynchronize = false) => new(parameterState, eventCallback, fireOnSynchronize);
}
