// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor.State;

internal class ParameterSet : IReadOnlyCollection<IParameterSynchronization>, IParameterSynchronization
{
    private readonly IReadOnlyCollection<IParameterSynchronization> _stateSynchronizations;

    public int Count => _stateSynchronizations.Count;

    public ParameterSet(params IParameterSynchronization[] stateSynchronizations)
    {
        _stateSynchronizations = stateSynchronizations;
    }

    public ParameterSet(IReadOnlyCollection<IParameterSynchronization> stateSynchronizations)
    {
        _stateSynchronizations = stateSynchronizations;
    }

    public void OnInitialized()
    {
        foreach (var stateSynchronization in _stateSynchronizations)
        {
            stateSynchronization.OnInitialized();
        }
    }

    public async Task OnParametersSetAsync()
    {
        foreach (var stateSynchronization in _stateSynchronizations)
        {
            await stateSynchronization.OnParametersSetAsync();
        }
    }

    public IEnumerator<IParameterSynchronization> GetEnumerator() => _stateSynchronizations.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
