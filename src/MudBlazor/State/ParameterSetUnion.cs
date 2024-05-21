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

internal class ParameterSetUnion : IEnumerable<ParameterSet>
{
    private readonly List<ParameterSet> _parameterSets = new();

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

    public async Task SetParametersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters)
    {
        if (_parameterSets.Count == 0)
        {
            await baseSetParametersAsync(parameters);
            return;
        }

        var parametersUnion = _parameterSets.Select(x => x.GetParameters());

#if NET8_0_OR_GREATER
        var parametersHandlerShouldFire = parametersUnion.SelectMany(parameter => parameter.Values)
            .Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters))
            .ToFrozenSet(ParameterHandlerUniquenessComparer.Default);
#else
        var parametersHandlerShouldFire = parametersUnion.SelectMany(parameter => parameter.Values)
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
