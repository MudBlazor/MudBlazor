// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.State.Comparer;
using MudBlazor.State.Invocation;

namespace MudBlazor.State;

/// <summary>
/// Represents a collection of multiple <see cref="ParameterScopeContainer"/> instances combined into a union.
/// </summary>
/// <remarks>
/// This class allows combining multiple <see cref="ParameterScopeContainer"/> instances into a single union, enabling the management of parameters across different scope containers.
/// </remarks>
internal class ParameterContainer : IParameterContainer
{
    private readonly List<IParameterScopeContainer> _parameterScopeContainers = new();

    private bool _verified;

    // Flattened lookup for parameter access by name, built on first TryGetValue call.
    private Dictionary<string, IParameterComponentLifeCycle>? _flattenedParameters;

    // Cache handler count for fast path optimization
    private int _handlerCount = -1;  // -1 means not computed yet

    /// <summary>
    /// Gets or sets a value indicating whether the container should automatically verify for duplicates.
    /// </summary>
    public bool AutoVerify { get; init; } = true;

    /// <summary>
    /// Gets the number of <see cref="ParameterScopeContainer"/> instances in the union.
    /// </summary>
    public int Count => _parameterScopeContainers.Count;

    /// <summary>
    /// Adds a <see cref="ParameterScopeContainer"/> instance to the union container.
    /// </summary>
    /// <param name="parameterScopeContainer">The <see cref="ParameterScopeContainer"/> instance to add to the union.</param>
    public void Add(IParameterScopeContainer parameterScopeContainer) => _parameterScopeContainers.Add(parameterScopeContainer);

    /// <summary>
    /// Executes <see cref="ParameterScopeContainer.OnInitialized"/> for all registered <see cref="ParameterScopeContainer"/>.
    /// </summary>
    public void OnInitialized()
    {
        VerifyOnAuto();

        for (var i = 0; i < _parameterScopeContainers.Count; i++)
        {
            _parameterScopeContainers[i].OnInitialized();
        }
    }

    /// <summary>
    /// Executes <see cref="ParameterScopeContainer.OnParametersSet"/> for all registered <see cref="ParameterScopeContainer"/>.
    /// </summary>
    public void OnParametersSet()
    {
        VerifyOnAuto();

        for (var i = 0; i < _parameterScopeContainers.Count; i++)
        {
            _parameterScopeContainers[i].OnParametersSet();
        }
    }

    /// <summary>
    /// Determines which <see cref="ParameterState{T}"/> have been changed and calls their respective change handler.
    /// </summary>
    /// <param name="baseSetParametersAsync">A func to call the base class' <see cref="ComponentBase.SetParametersAsync"/>.</param>
    /// <param name="parameters">The ParameterView coming from Blazor's  <see cref="ComponentBase.SetParametersAsync"/>.</param>
    public Task SetParametersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters)
    {
        if (Count == 0)
        {
            return baseSetParametersAsync(parameters);
        }

        VerifyOnAuto();

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

        for (var scopeIndex = 0; scopeIndex < _parameterScopeContainers.Count; scopeIndex++)
        {
            var registered = _parameterScopeContainers[scopeIndex].Parameters;
            for (var i = 0; i < registered.Count; i++)
            {
                var parameter = registered[i];
                if (parameter.HasHandler && parameter.HasParameterChanged(parameters))
                {
                    parametersHandlerShouldFire ??= new List<IParameterStateInvocationSnapshot>();
                    parameterStateValues ??= new List<ParameterStateValue>();
                    ParameterChangeHandlerUtility.AddSnapshotIfUnique(parametersHandlerShouldFire, parameter.CreateInvocationSnapshot(), parameterStateValues);
                }
            }
        }

        return ParameterChangeHandlerUtility.CreateHandlerCollection(parametersHandlerShouldFire, parameterStateValues, parameters);
    }

    /// <inheritdoc/>
    public bool TryGetValue(string parameterName, [MaybeNullWhen(false)] out IParameterComponentLifeCycle parameterComponentLifeCycle)
    {
        VerifyOnAuto();

        return FlattenedParameters.TryGetValue(parameterName, out parameterComponentLifeCycle);
    }

    /// <summary>
    /// Verifies the container for any duplicate parameters.
    /// </summary>
    public void Verify()
    {
        if (_verified)
        {
            return;
        }

        ThrowOnDuplicates();
        _verified = true;
    }

    /// <summary>
    /// Throws an exception if <see cref="AutoVerify"/> is enabled and duplicates are found.
    /// </summary>
    private void VerifyOnAuto()
    {
        if (AutoVerify)
        {
            Verify();
        }
    }

    /// <summary>
    /// Throws an exception if duplicates are found among the parameter scope containers.
    /// </summary>
    private void ThrowOnDuplicates()
    {
        var hashSet = new HashSet<IParameterComponentLifeCycle>(ParameterNameUniquenessComparer.Default);

        for (var scopeIndex = 0; scopeIndex < _parameterScopeContainers.Count; scopeIndex++)
        {
            var registered = _parameterScopeContainers[scopeIndex].Parameters;
            for (var i = 0; i < registered.Count; i++)
            {
                if (!hashSet.Add(registered[i]))
                {
                    throw new InvalidOperationException($"Parameter {registered[i].Metadata.ParameterName} is already registered!");
                }
            }
        }
    }

    /// <summary>
    /// Gets a flattened lookup across all parameter scope containers, built on first use.
    /// </summary>
    private Dictionary<string, IParameterComponentLifeCycle> FlattenedParameters
    {
        get
        {
            if (_flattenedParameters is null)
            {
                var flattened = new Dictionary<string, IParameterComponentLifeCycle>(StringComparer.Ordinal); // Parameter names are case-sensitive
                for (var scopeIndex = 0; scopeIndex < _parameterScopeContainers.Count; scopeIndex++)
                {
                    var registered = _parameterScopeContainers[scopeIndex].Parameters;
                    for (var i = 0; i < registered.Count; i++)
                    {
                        flattened.Add(registered[i].Metadata.ParameterName, registered[i]);
                    }
                }

                _flattenedParameters = flattened;
            }

            return _flattenedParameters;
        }
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
            for (var scopeIndex = 0; scopeIndex < _parameterScopeContainers.Count; scopeIndex++)
            {
                var registered = _parameterScopeContainers[scopeIndex].Parameters;
                for (var i = 0; i < registered.Count; i++)
                {
                    if (registered[i].HasHandler)
                    {
                        _handlerCount++;
                    }
                }
            }
        }

        return _handlerCount;
    }

    /// <inheritdoc/>
    public IEnumerator<IParameterComponentLifeCycle> GetEnumerator()
    {
        for (var scopeIndex = 0; scopeIndex < _parameterScopeContainers.Count; scopeIndex++)
        {
            var registered = _parameterScopeContainers[scopeIndex].Parameters;
            for (var i = 0; i < registered.Count; i++)
            {
                yield return registered[i];
            }
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
