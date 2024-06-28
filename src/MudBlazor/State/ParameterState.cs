﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// The <see cref="ParameterState{T}"/> automatically manages parameter value changes for <see cref="ParameterAttribute"/> as part of
/// MudBlazor's ParameterState framework. For details and usage please read CONTRIBUTING.md
/// </summary>
/// <remarks>
/// You don't need to create this object directly.
/// Instead, use the "MudComponentBase.RegisterParameter" method from within the component's constructor.
/// </remarks>
/// <typeparam name="T">The type of the component's property value.</typeparam>
public abstract class ParameterState<T>
{
    /// <summary>
    /// Gets the current value.
    /// </summary>
    public abstract T? Value { get; }

    /// <summary>
    /// Set the parameter's value.
    /// </summary>
    /// <remarks>
    /// Note: you should never set the parameter's property directly from within the component.
    /// Instead, use SetValueAsync on the ParameterState object.
    /// </remarks>
    /// <param name="value">New parameter's value.</param>
    /// <param name="parameterStateValueChangeTiming">The type of value change logic to use. Defaults to <see cref="ParameterStateValueChangeTiming.Immediate"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public abstract Task SetValueAsync(T value, ParameterStateValueChangeTiming parameterStateValueChangeTiming = ParameterStateValueChangeTiming.Immediate);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="ParameterState{T}"/> object to its underlying value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="parameterState">The <see cref="ParameterState{T}"/> object to convert.</param>
    /// <returns>The underlying value of type <typeparamref name="T"/>.</returns>
    public static implicit operator T?(ParameterState<T> parameterState) => parameterState.Value;
}

/// <summary>
/// Defines when <see cref="ParameterState{T}.Value"/> should be changed.
/// </summary>
public enum ParameterStateValueChangeTiming
{
    /// <summary>
    /// Sets <see cref="ParameterState{T}.Value"/> immediately using the value provided to the <see cref="ParameterState{T}.SetValueAsync"/> method.
    /// </summary>
    Immediate,
    /// <summary>
    /// Sets <see cref="ParameterState{T}.Value"/> after the event callbacks have been executed.
    /// </summary>
    /// <remarks>
    /// When a parameter event callback is provided we let Blazor handle the update.
    /// </remarks>
    AfterEventCallbacks
}
