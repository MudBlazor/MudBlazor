// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
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

    // A scope holds a handful of parameters, so an array scanned linearly beats any hashed lookup.
    // The scope is materialized once per component instance and read far less often than it is built.
    private IParameterComponentLifeCycle[]? _parameters;

    // Cache handler count for fast path optimization
    private int _handlerCount = -1;  // -1 means not computed yet

    /// <inheritdoc/>
    public bool IsLocked { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the parameter set has been initialized.
    /// </summary>
    /// <remarks>
    /// The parameter set is considered initialized once the registered parameters have been materialized.
    /// </remarks>
    public bool IsInitialized => _parameters is not null;

    /// <inheritdoc/>
    public IReadOnlyList<IParameterComponentLifeCycle> Parameters => _parameters ?? Materialize();

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
    }

    private IParameterComponentLifeCycle[] Materialize()
    {
        IsLocked = true;
        var parameters = _parameterStatesReader.ReadParameters();
        var materialized = parameters as IParameterComponentLifeCycle[] ?? parameters.ToArray();
        ThrowOnDuplicates(materialized);
        _parameters = materialized;
        _parameterStatesReader.Complete();

        return materialized;
    }

    /// <summary>
    /// Registering the same parameter twice is a component bug, so it must not be silently accepted.
    /// </summary>
    /// <remarks>
    /// A scope holds a handful of parameters whose names are compile-time literals, so pairwise comparison is cheaper than building a hashed set for the check.
    /// </remarks>
    private static void ThrowOnDuplicates(IParameterComponentLifeCycle[] parameters)
    {
        for (var i = 1; i < parameters.Length; i++)
        {
            var name = parameters[i].Metadata.ParameterName;
            for (var j = 0; j < i; j++)
            {
                if (string.Equals(parameters[j].Metadata.ParameterName, name, StringComparison.Ordinal))
                {
                    throw new ArgumentException($"Parameter {name} is already registered!", nameof(parameters));
                }
            }
        }
    }

    /// <summary>
    /// Forces the attachment of the collection of <seealso cref="IParameterComponentLifeCycle"/> immediately and materializes the parameters.
    /// </summary>
    /// <remarks>
    /// This method is designed for performance optimization.
    /// By calling this method, the parameters are materialized immediately instead of waiting for the Blazor lifecycle to access the values.
    /// This helps avoid potential slowdowns in rendering speed that could occur if the parameters were materialized during the Blazor lifecycle.
    /// </remarks>
    public void ForceParametersAttachment() => _ = Parameters;

    /// <summary>
    /// Executes <see cref="IParameterComponentLifeCycle.OnInitialized"/> for all registered parameters.
    /// </summary>
    public void OnInitialized()
    {
        var parameters = _parameters ?? Materialize();
        for (var i = 0; i < parameters.Length; i++)
        {
            parameters[i].OnInitialized();
        }
    }

    /// <summary>
    /// Executes <see cref="IParameterComponentLifeCycle.OnParametersSet"/> for all registered parameters.
    /// </summary>
    public void OnParametersSet()
    {
        var parameters = _parameters ?? Materialize();
        for (var i = 0; i < parameters.Length; i++)
        {
            parameters[i].OnParametersSet();
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
        var parameters = _parameters ?? Materialize();
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (string.Equals(parameter.Metadata.ParameterName, parameterName, StringComparison.Ordinal))
            {
                parameterComponentLifeCycle = parameter;

                return true;
            }
        }

        parameterComponentLifeCycle = null;

        return false;
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

        var registered = _parameters ?? Materialize();
        for (var i = 0; i < registered.Length; i++)
        {
            var parameter = registered[i];
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
            var parameters = _parameters ?? Materialize();
            for (var i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].HasHandler)
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
    public IEnumerator<IParameterComponentLifeCycle> GetEnumerator() => ((IEnumerable<IParameterComponentLifeCycle>)(_parameters ?? Materialize())).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Represents an enumerable reader for parameter states.
    /// </summary>
    private sealed class ParameterScopeContainerReadonlyEnumerable : IParameterStatesReader
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
