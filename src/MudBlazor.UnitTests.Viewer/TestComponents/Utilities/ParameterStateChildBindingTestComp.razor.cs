// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

#nullable enable
public partial class ParameterStateChildBindingTestComp : MudComponentBase
{
    private readonly List<(bool lastValue, bool value)> _parameterChangedEvents = new();

    private readonly ParameterState<bool> _expandedState;

    [Parameter]
    public string? Id { get; set; }

    [Parameter]
    public bool Expanded { get; set; }

    [Parameter]
    public EventCallback<bool> ExpandedChanged { get; set; }

    public bool ExpandedStateValue => _expandedState.Value;

    public IReadOnlyList<(bool lastValue, bool value)> ParameterChangedEvents => _parameterChangedEvents;

    public ParameterStateChildBindingTestComp()
    {
        using var registerScope = CreateRegisterScope();
        _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
            .WithParameter(() => Expanded)
            .WithEventCallback(() => ExpandedChanged)
            .WithChangeHandler(OnParameterChanged);
    }

    private void OnParameterChanged(ParameterChangedEventArgs<bool> args)
    {
        _parameterChangedEvents.Add(new ValueTuple<bool, bool>(args.LastValue, args.Value));
    }

    public Task ToggleAsync()
    {
        return _expandedState.SetValueAsync(!_expandedState.Value);
    }
}
