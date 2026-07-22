// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

#nullable enable
public partial class ParameterStateMultipleScopeTestComp : MudComponentBase
{
    public ParameterStateMultipleScopeTestComp()
    {
        using (var registerScope1 = CreateRegisterScope())
        {
            _a = registerScope1.RegisterParameter<int>(nameof(A))
                .WithParameter(() => A);
            _b = registerScope1.RegisterParameter<int>(nameof(B))
                .WithParameter(() => B);
        }

        using (var registerScope2 = CreateRegisterScope())
        {
            _c = registerScope2.RegisterParameter<int>(nameof(C))
                .WithParameter(() => C);
        }
    }

    private readonly ParameterState<int> _a;
    private readonly ParameterState<int> _b;
    private readonly ParameterState<int> _c;

    [Parameter]
    public int A { get; set; }

    [Parameter]
    public int B { get; set; }

    [Parameter]
    public int C { get; set; }
}
