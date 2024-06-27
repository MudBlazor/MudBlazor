// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State.Comparer;
using MudBlazor.State.Rule;

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
[DebuggerDisplay("ParameterName = {Metadata.ParameterName}, Value = {_value}")]
internal class ParameterStateInternal<T> : ParameterState<T>, IParameterComponentLifeCycle, IEquatable<ParameterStateInternal<T>>
{
    private T? _value;
    private T? _lastValue;
    private ParameterChangedEventArgs<T>? _parameterChangedEventArgs;

    private readonly Func<T> _getParameterValueFunc;
    private readonly IParameterEqualityComparerSwappable<T> _comparer;
    private readonly Func<EventCallback<T>> _eventCallbackFunc;
    private readonly IParameterChangedHandler<T>? _parameterChangedHandler;

    [MemberNotNullWhen(true, nameof(_parameterChangedEventArgs))]
    private bool HasParameterChangedEventArgs => _parameterChangedEventArgs is not null;

    /// <inheritdoc />
    public ParameterMetadata Metadata { get; }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(_parameterChangedHandler))]
    public bool HasHandler => _parameterChangedHandler is not null;

    /// <summary>
    /// Gets a value indicating whether the object is initialized.
    /// </summary>
    /// <remarks>
    /// This property is <c>true</c> once the <see cref="OnInitialized"/> method is called; otherwise, <c>false</c>.
    /// </remarks>
    [MemberNotNullWhen(true, nameof(_lastValue), nameof(_value), nameof(Value))]
    public bool IsInitialized { get; private set; }

    /// <inheritdoc/>
    public override T? Value => _value;

    /// <summary>
    /// Gets the function to provide the comparer for the parameter.
    /// </summary>
    public IParameterEqualityComparerSwappable<T> Comparer => _comparer;

    private ParameterStateInternal(ParameterMetadata metadata, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, IParameterChangedHandler<T>? parameterChangedHandler = null, IParameterEqualityComparerSwappable<T>? comparer = null)
    {
        Metadata = metadata;
        _getParameterValueFunc = getParameterValueFunc;
        _eventCallbackFunc = eventCallbackFunc;
        _parameterChangedHandler = parameterChangedHandler;
        _comparer = comparer ?? new ParameterEqualityComparerSwappable<T>(() => EqualityComparer<T>.Default);
        _lastValue = default;
        _value = default;
    }

    /// <inheritdoc/>
    public override Task SetValueAsync(T value)
    {
        if (!_comparer.Equals(Value, value))
        {
            _value = value;
            var eventCallback = _eventCallbackFunc();
            if (eventCallback.HasDelegate)
            {
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
        _value = currentParameterValue;
        _lastValue = currentParameterValue;
    }

    /// <inheritdoc />
    public void OnParametersSet()
    {
        var currentParameterValue = _getParameterValueFunc();
        if (!_comparer.Equals(_lastValue, currentParameterValue))
        {
            _value = currentParameterValue;
            _lastValue = currentParameterValue;
        }
    }

    /// <inheritdoc />
    public Task ParameterChangeHandleAsync()
    {
        if (HasHandler)
        {
            if (HasParameterChangedEventArgs)
            {
                // Since the ParameterSet lifecycles control all operations, it is acceptable to trigger the handler only when
                // HasParameterChanged has been invoked and stored the ParameterChangedEventArgs.
                // Direct invocation of this method by external callers is discouraged, so we shouldn't worry about it.
                return _parameterChangedHandler.HandleAsync(_parameterChangedEventArgs);
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool HasParameterChanged(ParameterView parameters)
    {
        var currentParameterValue = _getParameterValueFunc();

        var changed = false;
        _parameterChangedEventArgs = null;
        IEqualityComparer<T> comparer = _comparer;

        // This handles a very special case when the Parameter and the associated Comparer change in razor syntax at same time.
        // Then we need to extract it manually if it exists, otherwise the HasParameterChanged will use a stale comparer.
        // The problem happens because blazor will call the parameters.SetParameterProperties(this) only after this method, this means the new comparer is not set yet and comparerFunc returns an old one.
        if (!string.IsNullOrEmpty(Metadata.ComparerParameterName))
        {
            if (_comparer.TryGetFromParameterView(parameters, Metadata.ComparerParameterName, out var newComparer))
            {
                comparer = newComparer;
            }
        }

        // This if construction is to trigger [MaybeNullWhen(false)] for newValue, otherwise it wouldn't if we assign it directly to a variable,
        // and we'd need to suppress it's nullability.
        if (parameters.HasParameterChanged(Metadata.ParameterName, currentParameterValue, out var newValue, comparer: comparer))
        {
            changed = true;
            _parameterChangedEventArgs = new ParameterChangedEventArgs<T>(Metadata.ParameterName, currentParameterValue, newValue);
        }

        return changed;
    }

    ///  <summary>
    ///  Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// <para />
    ///  <b>NB!</b> Usually you don't need to call this directly. Instead, use the RegisterParameter method (<see cref="MudComponentBase"/>) from within the
    ///  component's constructor.  
    ///  </summary>
    ///  <param name="metadata">The parameter's metadata.</param>
    ///  <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    ///  <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    ///  <param name="parameterChangedHandler">A change handler containing code that needs to be executed when the parameter value changes/</param>
    ///  <param name="comparer">An optional comparer used to determine equality of parameter values.</param>
    ///  <remarks>
    ///  For details and usage please read CONTRIBUTING.md
    ///  </remarks>
    ///  <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterStateInternal<T> Attach(ParameterMetadata metadata, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, IParameterChangedHandler<T>? parameterChangedHandler = null, IParameterEqualityComparerSwappable<T>? comparer = null)
    {
        metadata = ParameterMetadataRules.Morph(metadata);

        return new ParameterStateInternal<T>(metadata, getParameterValueFunc, eventCallbackFunc, parameterChangedHandler, comparer);
    }

    /// <inheritdoc />
    public bool Equals(ParameterStateInternal<T>? other)
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
        return Metadata.ParameterName == other.Metadata.ParameterName;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ParameterStateInternal<T> parameterState && Equals(parameterState);

    /// <inheritdoc />
    public override int GetHashCode() => Metadata.ParameterName.GetHashCode();
}
