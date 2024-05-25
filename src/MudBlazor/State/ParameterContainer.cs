// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

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
    private readonly List<ParameterScopeContainer> _parameterScopeContainers = new();

    /// <summary>
    /// Gets the number of <see cref="ParameterScopeContainer"/> instances in the union.
    /// </summary>
    public int Count => _parameterScopeContainers.Count;

    /// <summary>
    /// Adds a <see cref="ParameterScopeContainer"/> instance to the union container.
    /// </summary>
    /// <param name="parameterScopeContainer">The <see cref="ParameterScopeContainer"/> instance to add to the union.</param>
    public void Add(ParameterScopeContainer parameterScopeContainer) => _parameterScopeContainers.Add(parameterScopeContainer);

    /// <summary>
    /// Executes <see cref="ParameterScopeContainer.OnInitialized"/> for all registered <see cref="ParameterScopeContainer"/>.
    /// </summary>
    public void OnInitialized()
    {
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

#if NET8_0_OR_GREATER
        var parametersHandlerShouldFire = _parameterScopeContainers.SelectMany(parameter => parameter)
            .Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters))
            .ToFrozenSet(ParameterHandlerUniquenessComparer.Default);
#else
        var parametersHandlerShouldFire = _parameterScopeContainers.SelectMany(parameter => parameter)
            .Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters))
            .ToHashSet(ParameterHandlerUniquenessComparer.Default);
#endif

        await baseSetParametersAsync(parameters);

        foreach (var parameterHandlerShouldFire in parametersHandlerShouldFire)
        {
            await parameterHandlerShouldFire.ParameterChangeHandleAsync();
        }
    }

    public bool TryGetValue(string parameterName, [MaybeNullWhen(false)] out IParameterComponentLifeCycle parameterComponentLifeCycle)
    {
        foreach (var parameterSet in _parameterScopeContainers)
        {
            if (parameterSet.TryGetValue(parameterName, out parameterComponentLifeCycle))
            {
                return true;
            }
        }

        parameterComponentLifeCycle = null;

        return false;
    }

    private void ThrowOnDuplicates()
    {
        var hashSet = new HashSet<IParameterComponentLifeCycle>();
        var parameters = _parameterScopeContainers.SelectMany(scopeContainers => scopeContainers);

        foreach (var parameter in parameters)
        {
            if (!hashSet.Add(parameter))
            {
                throw new InvalidOperationException("");
            }
        }
    }

    //public static readonly IParameterContainer Empty = new ParameterContainerEmpty();

    /// <inheritdoc/>
    public IEnumerator<IParameterComponentLifeCycle> GetEnumerator() => _parameterScopeContainers.SelectMany(scopeContainer => scopeContainer).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //private class ParameterContainerEmpty : IParameterContainer
    //{
    //    /// <inheritdoc/>
    //    public void OnInitialized() { /*Noop*/ }

    //    /// <inheritdoc/>
    //    public void OnParametersSet() { /*Noop*/ }

    //    /// <inheritdoc/>
    //    public Task SetParametersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters) => baseSetParametersAsync(parameters);

    //    /// <inheritdoc/>
    //    public bool TryGetValue(string parameterName, [MaybeNullWhen(false)] out IParameterComponentLifeCycle parameterComponentLifeCycle)
    //    {
    //        parameterComponentLifeCycle = null;

    //        return false;
    //    }

    //    /// <inheritdoc/>
    //    public IEnumerator<IParameterComponentLifeCycle> GetEnumerator() => Enumerable.Empty<IParameterComponentLifeCycle>().GetEnumerator();

    //    /// <inheritdoc/>
    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    //}
}
