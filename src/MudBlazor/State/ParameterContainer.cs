// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.State.Comparer;
using MudBlazor.State.Invocation;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents a collection of multiple <see cref="ParameterScopeContainer"/> instances combined into a union.
/// </summary>
/// <remarks>
/// This class allows combining multiple <see cref="ParameterScopeContainer"/> instances into a single union, enabling the management of parameters across different scope containers.
/// </remarks>
internal class ParameterContainer : IParameterContainer
{
    private readonly Lazy<bool> _lazyVerify;
    private readonly List<IParameterScopeContainer> _parameterScopeContainers = new();

    // Flattened dictionary for O(1) parameter lookups (created lazily on first TryGetValue call)
    private readonly Lazy<FrozenDictionary<string, IParameterComponentLifeCycle>> _flattenedParameters;

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
    /// Initializes a new instance of the <see cref="ParameterContainer"/> class.
    /// </summary>
    public ParameterContainer()
    {
        _lazyVerify = new Lazy<bool>(VerifyInternal);
        _flattenedParameters = new Lazy<FrozenDictionary<string, IParameterComponentLifeCycle>>(CreateFlattenedDictionary);
    }

    /// <summary>
    /// Executes <see cref="ParameterScopeContainer.OnInitialized"/> for all registered <see cref="ParameterScopeContainer"/>.
    /// </summary>
    public void OnInitialized()
    {
        VerifyOnAuto();

        foreach (var parameterSet in _parameterScopeContainers)
        {
            parameterSet.OnInitialized();
        }
    }

    /// <summary>
    /// Executes <see cref="ParameterScopeContainer.OnParametersSet"/> for all registered <see cref="ParameterScopeContainer"/>.
    /// </summary>
    public void OnParametersSet()
    {
        VerifyOnAuto();

        foreach (var parameterSet in _parameterScopeContainers)
        {
            parameterSet.OnParametersSet();
        }
    }

    /// <summary>
    /// Determines which <see cref="ParameterState{T}"/> have been changed and calls their respective change handler.
    /// </summary>
    /// <param name="baseSetParametersAsync">A func to call the base class' <see cref="ComponentBase.SetParametersAsync"/>.</param>
    /// <param name="parameters">The ParameterView coming from Blazor's  <see cref="ComponentBase.SetParametersAsync"/>.</param>
    public async Task SetParametersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters)
    {
        if (Count == 0)
        {
            await baseSetParametersAsync(parameters);
            return;
        }

        VerifyOnAuto();

        // Fast path: if no parameters have change handlers, skip handler detection entirely
        if (GetHandlerCount() == 0)
        {
            await baseSetParametersAsync(parameters);
            return;
        }

        var handlerCollection = CollectChangedHandlers(parameters);

        await baseSetParametersAsync(parameters);

        await ParameterChangeHandlerUtility.InvokeHandlersAsync(handlerCollection);
    }

    private ParameterChangeHandlerUtility.HandlerCollection? CollectChangedHandlers(ParameterView parameters)
    {
        List<IParameterStateInvocationSnapshot>? parametersHandlerShouldFire = null;
        List<ParameterStateValue>? parameterStateValues = null;

        foreach (var scopeContainer in _parameterScopeContainers)
        {
            foreach (var parameter in scopeContainer)
            {
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

        // Optimized: Use flattened dictionary for O(1) lookup instead of O(scopes) iteration
        return _flattenedParameters.Value.TryGetValue(parameterName, out parameterComponentLifeCycle);
    }

    /// <summary>
    /// Verifies the container for any duplicate parameters.
    /// </summary>
    public void Verify() => _ = _lazyVerify.Value;

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

    private bool VerifyInternal()
    {
        ThrowOnDuplicates();

        return true;
    }

    /// <summary>
    /// Throws an exception if duplicates are found among the parameter scope containers.
    /// </summary>
    private void ThrowOnDuplicates()
    {
        var hashSet = new HashSet<IParameterComponentLifeCycle>(ParameterNameUniquenessComparer.Default);
        var parameters = _parameterScopeContainers.SelectMany(scopeContainers => scopeContainers);

        foreach (var parameter in parameters)
        {
            if (!hashSet.Add(parameter))
            {
                throw new InvalidOperationException($"Parameter {parameter.Metadata.ParameterName} is already registered!");
            }
        }
    }

    /// <summary>
    /// Creates a flattened dictionary from all parameter scope containers for O(1) lookups.
    /// This is called lazily on first TryGetValue call.
    /// </summary>
    private FrozenDictionary<string, IParameterComponentLifeCycle> CreateFlattenedDictionary()
    {
        return _parameterScopeContainers
            .SelectMany(scope => scope)
            .ToFrozenDictionary(
                parameter => parameter.Metadata.ParameterName,
                parameter => parameter,
                StringComparer.Ordinal);  // Parameter names are case-sensitive
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
            foreach (var scopeContainer in _parameterScopeContainers)
            {
                foreach (var parameter in scopeContainer)
                {
                    if (parameter.HasHandler)
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
        // If flattened dictionary is already created, use it for better performance
        if (_flattenedParameters.IsValueCreated)
        {
            return ((IEnumerable<IParameterComponentLifeCycle>)_flattenedParameters.Value.Values).GetEnumerator();
        }

        // Otherwise, iterate through scope containers (avoid forcing dictionary creation)
        return _parameterScopeContainers.SelectMany(scopeContainer => scopeContainer).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
