// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents a collection of multiple <see cref="ParameterSet"/> instances combined into a union.
/// </summary>
/// <remarks>
/// This class allows combining multiple <see cref="ParameterSet"/> instances into a single union, enabling the management of parameters across different sets.
/// </remarks>
internal class ParameterSetUnion : IEnumerable<ParameterSet>
{
    private readonly List<ParameterSet> _parameterSets = new();

    /// <summary>
    /// Gets the number of <see cref="ParameterSet"/> instances in the union.
    /// </summary>
    public int Count => _parameterSets.Count;

    /// <summary>
    /// Adds a <see cref="ParameterSet"/> instance to the union.
    /// </summary>
    /// <param name="parameterSet">The <see cref="ParameterSet"/> instance to add to the union.</param>
    public void Add(ParameterSet parameterSet) => _parameterSets.Add(parameterSet);

    /// <summary>
    /// Executes <see cref="ParameterSet.OnInitialized"/> for all registered <see cref="ParameterSet"/>.
    /// </summary>
    public void OnInitialized()
    {
        foreach (var parameterSet in _parameterSets)
        {
            parameterSet.OnInitialized();
        }
    }

    /// <summary>
    /// Executes <see cref="ParameterSet.OnParametersSet"/> for all registered <see cref="ParameterSet"/>.
    /// </summary>
    public void OnParametersSet()
    {
        foreach (var parameterSet in _parameterSets)
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
        var parametersHandlerShouldFire = _parameterSets.SelectMany(parameter => parameter)
            .Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters))
            .ToFrozenSet(ParameterHandlerUniquenessComparer.Default);
#else
        var parametersHandlerShouldFire = _parameterSets.SelectMany(parameter => parameter)
            .Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters))
            .ToHashSet(ParameterHandlerUniquenessComparer.Default);
#endif

        await baseSetParametersAsync(parameters);

        foreach (var parameterHandlerShouldFire in parametersHandlerShouldFire)
        {
            await parameterHandlerShouldFire.ParameterChangeHandleAsync();
        }
    }

    /// <inheritdoc/>
    public IEnumerator<ParameterSet> GetEnumerator() => _parameterSets.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
