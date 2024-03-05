// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MudBlazor.State;

namespace MudBlazor.UnitTests.State.Mocks;

#nullable enable
public class ParameterChangedHandlerMock : IParameterChangedHandler
{
    public int FireCount { get; private set; }

    public Task HandleAsync()
    {
        FireCount++;

        return Task.CompletedTask;
    }
}
