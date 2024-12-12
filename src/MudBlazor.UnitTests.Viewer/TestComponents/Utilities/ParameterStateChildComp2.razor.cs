// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

public partial class ParameterStateChildComp2 : MudComponentBase
{
    private readonly ParameterState<int> _counterState;
    private readonly List<ParameterChangedEventArgs<int>> _parameterChangedEvents = [];

    public ParameterStateChildComp2()
    {
        using var registerScope = CreateRegisterScope();
        _counterState = registerScope.RegisterParameter<int>(nameof(Counter))
            .WithParameter(() => Counter)
            .WithEventCallback(() => CounterChanged)
            .WithChangeHandler(OnParameterChanged);
    }

    [Parameter]
    public int Counter { get; set; } = 0;

    [Parameter]
    public EventCallback<int> CounterChanged { get; set; }

    private void OnParameterChanged(ParameterChangedEventArgs<int> args)
    {
        _parameterChangedEvents.Add(args);
    }

    private Task OnClickChild2Async() => _counterState.SetValueAsync(_counterState.Value + 1);
}
