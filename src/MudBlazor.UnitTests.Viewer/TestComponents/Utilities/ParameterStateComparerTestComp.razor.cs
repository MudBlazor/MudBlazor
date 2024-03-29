using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

public partial class ParameterStateComparerTestComp : MudComponentBase
{
    private readonly List<ParameterChangedEventArgs<double>> _parameterChanges = new();

    [Parameter]
    public double DoubleParam { get; set; }

    public ParameterStateComparerTestComp()
    {
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);
        RegisterParameter(nameof(DoubleParam), () => DoubleParam, ParameterChangedHandler, comparer);
    }

    private void ParameterChangedHandler(ParameterChangedEventArgs<double> args)
    {
        _parameterChanges.Add(args);
    }
}
