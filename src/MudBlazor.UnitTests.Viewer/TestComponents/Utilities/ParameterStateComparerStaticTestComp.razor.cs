using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

public partial class ParameterStateComparerStaticTestComp : MudComponentBase
{
    private readonly List<ParameterChangedEventArgs<double>> _parameterChanges = new();

    [Parameter]
    public double DoubleParam { get; set; }

    public ParameterStateComparerStaticTestComp()
    {
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);
        using var registerScope = CreateRegisterScope();
        registerScope.RegisterParameter<double>(nameof(DoubleParam))
            .WithParameter(() => DoubleParam)
            .WithChangeHandler(OnParameterChanged)
            .WithComparer(comparer);
    }

    private void OnParameterChanged(ParameterChangedEventArgs<double> args)
    {
        _parameterChanges.Add(args);
    }
}
