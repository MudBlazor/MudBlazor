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
internal class ParameterSet : IEnumerable<IParameterComponentLifeCycle>
{
    private readonly List<IParameterComponentLifeCycle> _parameters = new();

    public void Add(IParameterComponentLifeCycle parameter)
    {
        if (_parameters.Contains(parameter))
        {
            throw new InvalidOperationException($"{parameter.ParameterName} is already registered");
        }

        _parameters.Add(parameter);
    }

    public void OnInitialized()
    {
        foreach (var parameter in _parameters)
        {
            parameter.OnInitialized();
        }
    }

    public void OnParametersSet()
    {
        foreach (var parameter in _parameters)
        {
            parameter.OnParametersSet();
        }
    }

    public async Task SetParametersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters)
    {
        // We check for HasHandler first for performance since we do not need HasParameterChanged if there is nothing to execute.
        // We need to call .ToList() otherwise the IEnumerable will be lazy invoked after the baseSetParametersAsync but we need before.
        var changedParams = _parameters.Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters)).ToList();

        await baseSetParametersAsync(parameters);

        foreach (var changedParam in changedParams)
        {
            await changedParam.ParameterChangeHandleAsync();
        }
    }

    public IEnumerator<IParameterComponentLifeCycle> GetEnumerator() => _parameters.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
