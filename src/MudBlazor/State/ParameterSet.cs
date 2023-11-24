// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
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
internal class ParameterSet : IEnumerable<IParameterComponentLifeCycle>
{
    private readonly List<IParameterComponentLifeCycle> _parameters = new();

    /// <summary>
    /// Adds a parameter to the parameter set.
    /// </summary>
    /// <param name="parameter">The parameter to add.</param>
    /// <exception cref="InvalidOperationException">Thrown when the parameter is already registered.</exception>
    public void Add(IParameterComponentLifeCycle parameter)
    {
        if (_parameters.Contains(parameter))
        {
            throw new InvalidOperationException($"{parameter.ParameterName} is already registered.");
        }

        _parameters.Add(parameter);
    }

    /// <summary>
    /// Executes <see cref="IParameterComponentLifeCycle.OnInitialized"/> for all registered parameters.
    /// </summary>
    public void OnInitialized()
    {
        foreach (var parameter in _parameters)
        {
            parameter.OnInitialized();
        }
    }

    /// <summary>
    /// Executes <see cref="IParameterComponentLifeCycle.OnParametersSet"/> for all registered parameters.
    /// </summary>
    public void OnParametersSet()
    {
        foreach (var parameter in _parameters)
        {
            parameter.OnParametersSet();
        }
    }

    /// <summary>
    /// Determines which <see cref="ParameterState"/> have been changed and calls their respective change handler.
    /// </summary>
    /// <param name="baseSetParametersAsync">A func to call the base class' <see cref="ComponentBase.SetParametersAsync"/>.</param>
    /// <param name="parameters">The ParameterView coming from Blazor's  <see cref="ComponentBase.SetParametersAsync"/>.</param>
    public async Task SetParametersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters)
    {
        // We check for HasHandler first for performance since we do not need HasParameterChanged if there is nothing to execute.
        // We need to call .ToList() otherwise the IEnumerable will be lazy invoked after the baseSetParametersAsync, but we need before.
        var changedParams = _parameters.Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters)).ToList();

        await baseSetParametersAsync(parameters);

        foreach (var changedParam in changedParams)
        {
            await changedParam.ParameterChangeHandleAsync();
        }
    }

    /// <inheritdoc/>
    public IEnumerator<IParameterComponentLifeCycle> GetEnumerator() => _parameters.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
