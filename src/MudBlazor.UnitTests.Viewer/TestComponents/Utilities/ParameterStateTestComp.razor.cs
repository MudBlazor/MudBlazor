using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

#nullable enable
public partial class ParameterStateTestComp : MudComponentBase
{
    private readonly List<string> _parameterChanges = new();

    public ParameterStateTestComp()
    {
        using var registerScope = CreateRegisterScope();
        _intParam = registerScope.RegisterParameter<int>(nameof(IntParam))
            .WithParameter(() => IntParam)
            .WithChangeHandler(OnIntParamChanged);
    }

    private readonly ParameterState<int> _intParam;

    private void OnIntParamChanged(ParameterChangedEventArgs<int> args)
    {
        _parameterChanges.Add($"IntParam: {args.LastValue}=>{args.Value}");
    }

    [Parameter]
    public int IntParam { get; set; }

    [Parameter]
    public int NonStateDummyIntParam { get; set; }
}
