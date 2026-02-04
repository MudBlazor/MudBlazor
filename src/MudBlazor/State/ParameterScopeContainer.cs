// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.State.Invocation;

namespace MudBlazor.State;

/// <summary>
/// Represents a collection of registered parameters.
/// This class is part of MudBlazor's ParameterState framework.
/// </summary>
/// <remarks>
/// For details and usage please read CONTRIBUTING.md
/// </remarks>
internal class ParameterScopeContainer : IParameterScopeContainer
{
    private readonly IParameterStatesReader _parameterStatesReader;
    private readonly Lazy<FrozenDictionary<string, IParameterComponentLifeCycle>> _parameters;

    // Cache handler count for fast path optimization
    private int _handlerCount = -1;  // -1 means not computed yet

    /// <inheritdoc/>
    public bool IsLocked { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the parameter set has been initialized.
    /// </summary>
    /// <remarks>
    /// The parameter set is considered initialized once the inner dictionary of parameters has been created.
    /// </remarks>
    public bool IsInitialized => _parameters.IsValueCreated;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterScopeContainer"/> class with the specified parameters.
    /// </summary>
    /// <param name="parameters">An optional array of parameters to initialize the set.</param>
    public ParameterScopeContainer(params IParameterComponentLifeCycle[] parameters)
        : this(new ParameterScopeContainerReadonlyEnumerable(parameters))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterScopeContainer"/> class with the specified parameters.
    /// </summary>
    /// <param name="parameters">An enumerable collection of parameters to initialize the set.</param>
    public ParameterScopeContainer(IEnumerable<IParameterComponentLifeCycle> parameters)
        : this(new ParameterScopeContainerReadonlyEnumerable(parameters))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterScopeContainer"/> class with the specified parameter states factory.
    /// </summary>
    /// <param name="parameterStatesReader">The factory used to read an enumerable collection of parameters to initialize the set.</param>
    public ParameterScopeContainer(IParameterStatesReader parameterStatesReader)
    {
        _parameterStatesReader = parameterStatesReader;
        _parameters = new Lazy<FrozenDictionary<string, IParameterComponentLifeCycle>>(ParametersFactory);
    }

    private FrozenDictionary<string, IParameterComponentLifeCycle> ParametersFactory()
    {
        IsLocked = true;
        var parameters = _parameterStatesReader.ReadParameters();
        var dictionary = parameters.ToFrozenDictionary(
            parameter => parameter.Metadata.ParameterName,
            parameter => parameter,
            StringComparer.Ordinal);  // Parameter names are case-sensitive; use Ordinal for best performance
        _parameterStatesReader.Complete();

        return dictionary;
    }

    /// <summary>
    /// Forces the attachment of the collection of <seealso cref="IParameterComponentLifeCycle"/> immediately and initializes the inner dictionary.
    /// </summary>
    /// <remarks>
    /// This method is designed for performance optimization. By calling this method, the dictionary initialization is done immediately instead of waiting for the Blazor lifecycle to access the values. 
    /// This helps avoid potential slowdowns in rendering speed that could occur if the dictionary were initialized during the Blazor lifecycle.
    /// </remarks>
    public void ForceParametersAttachment() => _ = _parameters.Value;

    /// <summary>
    /// Executes <see cref="IParameterComponentLifeCycle.OnInitialized"/> for all registered parameters.
    /// </summary>
    public void OnInitialized()
    {
        foreach (var parameter in _parameters.Value.Values)
        {
            parameter.OnInitialized();
        }
    }

    /// <summary>
    /// Executes <see cref="IParameterComponentLifeCycle.OnParametersSet"/> for all registered parameters.
    /// </summary>
    public void OnParametersSet()
    {
        foreach (var parameter in _parameters.Value.Values)
        {
            parameter.OnParametersSet();
        }
    }

    /// <summary>
    /// Determines which <see cref="ParameterState{T}"/> have been changed and calls their respective change handler.
    /// </summary>
    /// <param name="baseSetParametersAsync">A func to call the base class' <see cref="ComponentBase.SetParametersAsync"/>.</param>
    /// <param name="parameters">The ParameterView coming from Blazor's <see cref="ComponentBase.SetParametersAsync"/>.</param>
    public Task SetParametersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters)
    {
        // Fast path: if no parameters have change handlers, skip handler detection entirely
        if (GetHandlerCount() == 0)
        {
            return baseSetParametersAsync(parameters);
        }

        // IMPORTANT: Do not inline the async implementation here.
        // Avoid async state machine allocation on the common path by returning the Task directly.
        // The async state machine is only used when parameter change handlers must be invoked.
        return SetParametersWithHandlersAsync(baseSetParametersAsync, parameters);
    }

    /// <inheritdoc/>
    public bool TryGetValue(string parameterName, [MaybeNullWhen(false)] out IParameterComponentLifeCycle parameterComponentLifeCycle)
    {
        return _parameters.Value.TryGetValue(parameterName, out parameterComponentLifeCycle);
    }

    private async Task SetParametersWithHandlersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters)
    {
        var handlerCollection = CollectChangedHandlers(parameters);

        await baseSetParametersAsync(parameters).ConfigureAwait(false);
        await ParameterChangeHandlerUtility.InvokeHandlersAsync(handlerCollection).ConfigureAwait(false);
    }

    private ParameterChangeHandlerUtility.HandlerCollection? CollectChangedHandlers(ParameterView parameters)
    {
        List<IParameterStateInvocationSnapshot>? parametersHandlerShouldFire = null;
        List<ParameterStateValue>? parameterStateValues = null;

        foreach (var parameter in _parameters.Value.Values)
        {
            if (parameter.HasHandler && parameter.HasParameterChanged(parameters))
            {
                parametersHandlerShouldFire ??= new List<IParameterStateInvocationSnapshot>();
                parameterStateValues ??= new List<ParameterStateValue>();
                ParameterChangeHandlerUtility.AddSnapshotIfUnique(parametersHandlerShouldFire, parameter.CreateInvocationSnapshot(), parameterStateValues);
            }
        }

        return ParameterChangeHandlerUtility.CreateHandlerCollection(parametersHandlerShouldFire, parameterStateValues, parameters);
    }

    /// <summary>
    /// Gets the total count of parameters with change handlers.
    /// This is computed once and cached for the fast path optimization.
    /// </summary>
    private int GetHandlerCount()
    {
        if (_handlerCount == -1)
        {
            _handlerCount = 0;
            foreach (var parameter in this)
            {
                if (parameter.HasHandler)
                {
                    _handlerCount++;
                }
            }
        }

        return _handlerCount;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!IsLocked)
        {
            ForceParametersAttachment();
        }
    }

    /// <inheritdoc/>
    public IEnumerator<IParameterComponentLifeCycle> GetEnumerator() => ((IReadOnlyDictionary<string, IParameterComponentLifeCycle>)_parameters.Value).Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Represents an enumerable reader for parameter states.
    /// </summary>
    private class ParameterScopeContainerReadonlyEnumerable : IParameterStatesReader
    {
        private readonly IEnumerable<IParameterComponentLifeCycle> _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterScopeContainerReadonlyEnumerable"/> class with the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters to be read.</param>
        public ParameterScopeContainerReadonlyEnumerable(IEnumerable<IParameterComponentLifeCycle> parameters) => _parameters = parameters;

        /// <inheritdoc />
        public IEnumerable<IParameterComponentLifeCycle> ReadParameters() => _parameters;

        /// <inheritdoc />
        public void Complete() { /*Noop*/ }
    }
}
