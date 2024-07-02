using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

#nullable enable
public partial class ParameterStateImmediateComp : MudComponentBase
{
    private readonly List<string> _parameterChanges = new();
    private readonly ParameterState<bool> _valueState;

    [Parameter]
    public bool Value { get; set; }

    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    public ParameterStateImmediateComp()
    {
        using var registerScope = CreateRegisterScope();
        _valueState = registerScope.RegisterParameter<bool>(nameof(Value))
            .WithParameter(() => Value)
            .WithEventCallback(() => ValueChanged)
            .WithChangeHandler(OnValueChanged);
    }

    // this component's job is to invert the value
    public Task ToggleValueAsync()
        => _valueState.SetValueAsync(!_valueState.Value);

    private void OnValueChanged(ParameterChangedEventArgs<bool> args)
    {
        _parameterChanges.Add($"Value: {args.LastValue}=>{args.Value}");
    }
}
