// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

public partial class ParameterStateComparerSwapTestComp : MudComponentBase
{
    private readonly List<ParameterChangedEventArgs<double>> _parameterChanges = new();

    [Parameter]
    public double DoubleParam { get; set; }

    [Parameter]
    public IEqualityComparer<double> DoubleEqualityComparer { get; set; } = new DoubleEpsilonEqualityComparer(0.0001f);

    public ParameterStateComparerSwapTestComp()
    {
        using var registerScope = CreateRegisterScope();
        registerScope.RegisterParameter<double>(nameof(DoubleParam))
            .WithParameter(() => DoubleParam)
            .WithChangeHandler(ParameterChangedHandler)
            .WithComparer(() => DoubleEqualityComparer);
    }

    private void ParameterChangedHandler(ParameterChangedEventArgs<double> args)
    {
        _parameterChanges.Add(args);
    }
}
