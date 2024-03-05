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
/// The <see cref="ParameterState{T}"/> automatically manages parameter value changes for <see cref="ParameterAttribute"/> as part of
/// MudBlazor's ParameterState framework. For details and usage please read CONTRIBUTING.md
/// </summary>
/// <remarks>
/// You don't need to create this object directly.
/// Instead, use the "MudComponentBase.RegisterParameter" method from within the component's constructor.
/// </remarks>
/// <typeparam name="T">The type of the component's property value.</typeparam>
#nullable enable
internal class ParameterState<T> : IParameterComponentLifeCycle, IEquatable<ParameterState<T>>
{
    private T? _lastValue;
    private readonly Func<T> _getParameterValueFunc;
    private readonly Func<EventCallback<T>> _eventCallbackFunc;
    private readonly IParameterChangedHandler? _parameterChangedHandler;

    /// <inheritdoc />
    public string ParameterName { get; }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(_parameterChangedHandler))]
    public bool HasHandler => _parameterChangedHandler is not null;

    /// <summary>
    /// Gets a value indicating whether the object is initialized.
    /// </summary>
    /// <remarks>
    /// This property is <c>true</c> once the <see cref="OnInitialized"/> method is called; otherwise, <c>false</c>.
    /// </remarks>
    [MemberNotNullWhen(true, nameof(_lastValue), nameof(Value))]
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets the current value.
    /// </summary>
    public T? Value { get; private set; }

    private ParameterState(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, IParameterChangedHandler? parameterChangedHandler = null)
    {
        ParameterName = parameterName;
        _getParameterValueFunc = getParameterValueFunc;
        _eventCallbackFunc = eventCallbackFunc;
        _parameterChangedHandler = parameterChangedHandler;
        _lastValue = default;
        Value = default;
    }

    /// <summary>
    /// Set the parameter's value. 
    /// </summary>
    /// <remarks>
    /// Note: you should never set the parameter's property directly from within the component. Instead, use SetValueAsync
    /// on the ParameterState object.
    /// </remarks>
    /// <param name="value">New parameter's value.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task SetValueAsync(T value)
    {
        if (!EqualityComparer<T>.Default.Equals(Value, value))
        {
            Value = value;
            var eventCallback = _eventCallbackFunc();
            if (eventCallback.HasDelegate)
            {
                _lastValue = value;

                return eventCallback.InvokeAsync(value);
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void OnInitialized()
    {
        IsInitialized = true;
        var currentParameterValue = _getParameterValueFunc();
        Value = currentParameterValue;
        _lastValue = currentParameterValue;
    }

    /// <inheritdoc />
    public void OnParametersSet()
    {
        var currentParameterValue = _getParameterValueFunc();
        if (!EqualityComparer<T>.Default.Equals(_lastValue, currentParameterValue))
        {
            Value = currentParameterValue;
            _lastValue = currentParameterValue;
        }
    }

    /// <inheritdoc />
    public Task ParameterChangeHandleAsync()
    {
        return HasHandler ? _parameterChangedHandler.HandleAsync() : Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool HasParameterChanged(ParameterView parameters)
    {
        var currentParameterValue = _getParameterValueFunc();

        return parameters.HasParameterChanged(ParameterName, currentParameterValue);
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    ///<para />
    /// <b>NB!</b> Usually you don't need to call this directly. Instead, use the RegisterParameter method (<see cref="MudComponentBase"/>) from within the
    /// component's constructor.  
    /// </summary>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">A change handler containing code that needs to be executed when the parameter value changes/</param>
    /// <remarks>
    /// For details and usage please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
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
