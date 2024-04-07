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
/// Represents a collection of registered parameters.
/// This class is part of MudBlazor's ParameterState framework.
/// </summary>
/// <remarks>
/// For details and usage please read CONTRIBUTING.md
/// </remarks>
internal class ParameterSet : IReadOnlyCollection<IParameterComponentLifeCycle>
{
    private readonly Dictionary<string, IParameterComponentLifeCycle> _parameters = new();

    /// <inheritdoc/>
    public int Count => _parameters.Count;

    /// <summary>
    /// Adds a parameter to the parameter set.
    /// </summary>
    /// <param name="parameter">The parameter to add.</param>
    /// <exception cref="InvalidOperationException">Thrown when the parameter is already registered.</exception>
    public void Add(IParameterComponentLifeCycle parameter)
    {
        if (!_parameters.TryAdd(parameter.Metadata.ParameterName, parameter))
        {
            throw new InvalidOperationException($"{parameter.Metadata.ParameterName} is already registered.");
        }
    }

    /// <summary>
    /// Executes <see cref="IParameterComponentLifeCycle.OnInitialized"/> for all registered parameters.
    /// </summary>
    public void OnInitialized()
    {
        foreach (var parameter in _parameters.Values)
        {
            parameter.OnInitialized();
        }
    }

    /// <summary>
    /// Executes <see cref="IParameterComponentLifeCycle.OnParametersSet"/> for all registered parameters.
    /// </summary>
    public void OnParametersSet()
    {
        foreach (var parameter in _parameters.Values)
        {
            parameter.OnParametersSet();
        }
    }

    /// <summary>
    /// Determines which <see cref="ParameterState{T}"/> have been changed and calls their respective change handler.
    /// </summary>
    /// <param name="baseSetParametersAsync">A func to call the base class' <see cref="ComponentBase.SetParametersAsync"/>.</param>
    /// <param name="parameters">The ParameterView coming from Blazor's  <see cref="ComponentBase.SetParametersAsync"/>.</param>
    public async Task SetParametersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters)
    {
#if NET8_0_OR_GREATER
        var parametersHandlerShouldFire = _parameters.Values
            .Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters))
            .ToFrozenSet(ParameterHandlerUniquenessComparer.Default);
#else
        var parametersHandlerShouldFire = _parameters.Values
            .Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters))
            .ToHashSet(ParameterHandlerUniquenessComparer.Default);
#endif

        await baseSetParametersAsync(parameters);

        foreach (var parameterHandlerShouldFire in parametersHandlerShouldFire)
        {
            await parameterHandlerShouldFire.ParameterChangeHandleAsync();
        }
    }

    /// <summary>Searches the set for a given value and returns the equal value it finds, if any.</summary>
    /// <param name="parameterName">The value to search for.</param>
    /// <param name="parameterComponentLifeCycle">The value from the set that the search found, or the default value when the search yielded no match.</param>
    /// <returns>A value indicating whether the search was successful.</returns>
    public bool TryGetValue(string parameterName, [MaybeNullWhen(false)] out IParameterComponentLifeCycle parameterComponentLifeCycle)
    {
        return _parameters.TryGetValue(parameterName, out parameterComponentLifeCycle);
    }

    /// <inheritdoc/>
    public IEnumerator<IParameterComponentLifeCycle> GetEnumerator() => _parameters.Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
