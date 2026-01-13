// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.State.Comparer;
using MudBlazor.State.Invocation;
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
    private T? _initialValue;
    private bool _isInitialized;
    private bool _isChildOriginatedChange;
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

    /// <inheritdoc />
    public override bool HasCallback => _eventCallbackFunc().HasDelegate;

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(_value), nameof(_initialValue))]
    public override bool IsInitialized => _isInitialized;

    [MemberNotNullWhen(true, nameof(_value))]
    private bool TreatAsInitialized { get; set; }

    /// <inheritdoc />
    public override T InitialValue
    {
        get
        {
            if (!IsInitialized)
            {
                return _getParameterValueFunc();
            }

            return _initialValue;
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// Some (bad) components may attempt to read parameter values before OnInitialized is called
    /// (e.g., during SetParametersAsync). In that case, the state is not yet fully initialized,
    /// and accessing the Value will return null even when the parameter has a default value, such as:
    /// <code>[Parameter] string MyParameter { get; set; } = "some value";</code>
    /// <br/>
    /// To avoid this, if initialization has not yet occurred, fall back to the delegate
    /// that retrieves the current parameter value.
    /// <br/>
    /// Important: Some incorrect tests bypass the normal Blazor lifecycle, which can lead to
    /// incorrect state being read. For example:
    /// </para>
    /// <code>
    ///      var panels = Context.RenderComponent&lt;MudExpansionPanels&gt;();
    ///      var panel = new MudExpansionPanel();
    ///      panels.Instance.AddPanelAsync(panel);
    /// </code>
    /// <para>
    /// In this scenario, MudExpansionPanel is created outside Blazor's lifecycle, so
    /// AddPanelAsync receives an invalid _expandedState.Value.
    /// Such test patterns should be avoided—rewrite the tests to follow normal Blazor usage.
    /// </para>
    /// </remarks>
    public override T Value
    {
        get
        {
            if (!TreatAsInitialized && !IsInitialized)
            {
                return _getParameterValueFunc();
            }

            return _value;
        }
    }

    /// <inheritdoc/>
    public override T RenderValue => _getParameterValueFunc();

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
    }

    /// <inheritdoc/>
    public override Task SetValueAsync(T value)
    {
        // Avoid using the Value property here because its getter includes an
        // IsInitialized branch which may cause the SetValueAsync to be skipped.
        if (!_comparer.Equals(_value, value))
        {
            _value = value;
            if (!TreatAsInitialized)
            {
                // https://github.com/MudBlazor/MudBlazor/pull/12241
                // Workaround for components that read parameter values before OnInitialized is called aka Select and Autocomplete.
                // Can be removed in future major versions when such components are fixed.
                TreatAsInitialized = true;
            }
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
        _isInitialized = true;
        var currentParameterValue = _getParameterValueFunc();
        _initialValue = currentParameterValue;
        _value = currentParameterValue;
        _lastValue = currentParameterValue;
    }

    /// <inheritdoc />
    public void OnParametersSet()
    {
        var currentParameterValue = _getParameterValueFunc();
        if (!_comparer.Equals(_lastValue, currentParameterValue))
        {
            _isChildOriginatedChange = _comparer.Equals(_value, currentParameterValue);
            _value = currentParameterValue;
            _lastValue = currentParameterValue;
        }
    }

    /// <inheritdoc/>
    public IParameterStateInvocationSnapshot CreateInvocationSnapshot()
    {
        return new ParameterStateInvocationSnapshot<T>(
            Metadata,
            HasParameterChangedEventArgs ? _parameterChangedEventArgs.Clone() : null,
            _parameterChangedHandler,
            // We should not cache this value because it may be modified by OnParametersSet.
            // In theory, this could also lead to race conditions if multiple OnParametersSet calls occur with different _lastValue, currentParameterValue values.
            // For now, we'll leave it as-is since properly fixing this would be complex, it should be fixed only if it happens in practise.
            () => _isChildOriginatedChange);
    }

    /// <inheritdoc />
    public bool HasParameterChanged(ParameterView parameters)
    {
        var currentParameterValue = _getParameterValueFunc();

        var changed = false;
        _parameterChangedEventArgs = null;
        var comparer = ExtractComparer(parameters);

        // This if construction is to trigger [MaybeNullWhen(false)] for newValue, otherwise it wouldn't if we assign it directly to a variable,
        // and we'd need to suppress it's nullability.
        if (parameters.HasParameterChanged(Metadata.ParameterName, currentParameterValue, out var newValue, comparer: comparer))
        {
            changed = true;
            _parameterChangedEventArgs = new ParameterChangedEventArgs<T>(parameters, Metadata.ParameterName, currentParameterValue, newValue);
        }

        return changed;
    }

    /// <summary>
    /// Extracts the appropriate equality comparer for the parameter from the provided <see cref="ParameterView"/>.
    /// </summary>
    /// <param name="parameters">The <see cref="ParameterView"/> containing the incoming parameter values.</param>
    /// <returns>
    /// The equality comparer to use for comparing parameter values. Returns either the updated comparer from the 
    /// <paramref name="parameters"/> if available, or the current comparer stored in <see cref="_comparer"/>.
    /// </returns>
    /// <remarks>
    /// This method handles a special edge case where both a parameter and its associated comparer parameter 
    /// change simultaneously in Razor syntax. Since Blazor calls <c>SetParameterProperties</c> after this method executes,
    /// the new comparer may not yet be set in the component. This method manually extracts the updated comparer 
    /// from the <paramref name="parameters"/> to ensure <see cref="HasParameterChanged"/> uses the correct comparer 
    /// instead of a stale one.
    /// </remarks>
    public IEqualityComparer<T> ExtractComparer(ParameterView parameters)
    {
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

        return comparer;
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
