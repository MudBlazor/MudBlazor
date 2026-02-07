// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State.Builder;

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
public abstract class ParameterState<T>
{
    /// <summary>
    /// Gets the current logical value tracked by the <see cref="ParameterState{T}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When you call <see cref="SetValueAsync"/> the <see cref="Value"/> is updated immediately in the local
    /// <see cref="ParameterState{T}"/> instance so subsequent reads within the same render/logic will see the
    /// new value.
    /// </para>
    /// <para>
    /// The <see cref="Value"/> represents the child component's current notion of the parameter. It may differ from
    /// <see cref="RenderValue"/> until the parent re-renders and supplies the updated value (see <see cref="HasCallback"/>).
    /// </para>
    /// </remarks>
    public abstract T Value { get; }

    /// <summary>
    /// Gets the value most recently received from the parent component's render.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="RenderValue"/> represents the parameter value as provided by the parent component during the
    /// last parameter set/render. It is the value that should be shown to the user when you want to reflect
    /// what the parent actually rendered.
    /// </para>
    /// <para>
    /// Example: when you call <see cref="SetValueAsync"/> the local <see cref="Value"/> will change immediately,
    /// but <see cref="RenderValue"/> will only update if the parent component participates in two-way binding
    /// (see <see cref="HasCallback"/>). In two-way binding the child typically invokes the <see cref="EventCallback{T}"/>
    /// and the parent responds by updating the parameter and re-rendering; when that happens the child will receive
    /// the new value from the parent and <see cref="RenderValue"/> will be updated accordingly.
    /// </para>
    /// </remarks>
    public abstract T RenderValue { get; }

    /// <summary>
    /// Gets a value indicating whether this parameter has an associated <see cref="EventCallback{T}"/> on the component.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>true</c> when the component defines an <see cref="EventCallback{T}"/> parameter that corresponds to the parameter
    /// (conventionally named <c>{ParameterName}Changed</c>). For example:
    /// <code>
    /// [Parameter]
    /// public int Counter { get; set; }
    /// 
    /// [Parameter]
    /// public EventCallback&lt;int&gt; CounterChanged { get; set; }
    /// </code>
    /// If you supply the <see cref="EventCallback{T}"/> to <see cref="RegisterParameterBuilder{T}.WithEventCallback"/> then
    /// in that case <see cref="HasCallback"/> will be <c>true</c>.
    /// </para>
    /// <para>
    /// <b>NB!</b> Having a callback doesn't always mean the usage of Blazor's <c>@bind-</c> syntax.
    /// </para>
    /// <para>
    /// When <see cref="HasCallback"/> is <c>true</c>, calling <see cref="SetValueAsync"/> triggers
    /// the associated <see cref="EventCallback{T}"/> so the parent can update its value and re-render — which in turn updates
    /// <see cref="RenderValue"/> when the parent supplies the new value.
    /// </para>
    /// </remarks>
    public abstract bool HasCallback { get; }

    /// <summary>
    /// Gets a value indicating whether the object is initialized.
    /// </summary>
    /// <remarks>
    /// This property is <c>true</c> once the <see cref="ComponentBase.OnInitialized"/> method is called; otherwise, <c>false</c>.
    /// </remarks>
    public abstract bool IsInitialized { get; }

    /// <summary>
    /// Gets the initial value of the parameter at the time <see cref="ComponentBase.OnInitialized"/> is called.
    /// </summary>
    /// <remarks>
    /// This value is captured once when the component is initialized and never changes thereafter,
    /// even if the <see cref="Value"/> property changes later.
    /// </remarks>
    public abstract T InitialValue { get; }

    /// <summary>
    /// Set the parameter's value. 
    /// </summary>
    /// <remarks>
    /// Note: you should never set the parameter's property directly from within the component.
    /// Instead, use <see cref="SetValueAsync"/> on the <see cref="ParameterState{T}"/> object.
    /// <para>
    /// When the parameter has an associated callback (<see cref="HasCallback"/> is <c>true</c>), calling this method will
    /// invoke the associated <see cref="EventCallback{T}"/> so the parent can update its state and
    /// re-render. The child's <see cref="RenderValue"/> will then be updated when the parent supplies the new
    /// value on the next render.
    /// </para>
    /// <para>
    /// When <see cref="HasCallback"/> is <c>false</c>, invoking <see cref="SetValueAsync"/> updates the child's local
    /// <see cref="Value"/> but does not notify the parent; therefore <see cref="RenderValue"/> will only change when
    /// the parent explicitly supplies a different value.
    /// </para>
    /// </remarks>
    /// <param name="value">New parameter's value.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public abstract Task SetValueAsync(T value);

    /// <summary>
    /// Defines an implicit conversion of a <see cref="ParameterState{T}"/> object to its underlying value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="parameterState">The <see cref="ParameterState{T}"/> object to convert.</param>
    /// <returns>The underlying value of type <typeparamref name="T"/>.</returns>
    public static implicit operator T(ParameterState<T> parameterState) => parameterState.Value;
}
