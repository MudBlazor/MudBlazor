using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

#nullable enable
public partial class ParameterStateTestComp : MudComponentBase
{
    private List<string> _parameterChanges = new();

    public ParameterStateTestComp()
    {
        _intParam = RegisterParameter(nameof(IntParam), () => IntParam, OnIntParamChanged);
    }

    private IParameterState<int> _intParam;

    private void OnIntParamChanged(ParameterChangedEventArgs<int> args)
    {
        _parameterChanges.Add($"IntParam: {args.LastValue}=>{args.Value}");
    }

    [Parameter]
    public int IntParam { get; set; }
}
