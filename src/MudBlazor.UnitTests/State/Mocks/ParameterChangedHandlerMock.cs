// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MudBlazor.State;


namespace MudBlazor.UnitTests.State.Mocks;

#nullable enable
internal class ParameterChangedHandlerMock<TArgs> : IParameterChangedHandler<TArgs>
{
    private readonly List<ParameterChangedEventArgs<TArgs>> _changes = new();

    public IReadOnlyList<ParameterChangedEventArgs<TArgs>> Changes => _changes;

    public Task HandleAsync(ParameterChangedEventArgs<TArgs> parameterChangedEventArgs)
    {
        _changes.Add(parameterChangedEventArgs);

        return Task.CompletedTask;
    }
}
