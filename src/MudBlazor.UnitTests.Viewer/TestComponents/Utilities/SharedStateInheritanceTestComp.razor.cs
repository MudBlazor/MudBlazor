// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

public abstract class AnotherComponentBase : MudComponentBase
{
    protected AnotherComponentBase()
    {
        // ac shared handler group
        using var registerScope = CreateRegisterScope();
        _a = registerScope.RegisterParameter<int>(nameof(A))
            .WithParameter(() => A)
            .WithChangeHandler(OnAbcChanged);
        _c = registerScope.RegisterParameter<int>(nameof(C))
            .WithParameter(() => C)
            .WithChangeHandler(OnAbcChanged);
        // o and p are not sharing their handler because lambdas are excluded, even if they contain the same code
        _o = registerScope.RegisterParameter<int>(nameof(O))
            .WithParameter(() => O)
            .WithChangeHandler(() => OpHandlerCallCount++);
        // other join to shared handler later
        _z = registerScope.RegisterParameter<int>(nameof(Z))
            .WithParameter(() => Z)
            .WithChangeHandler(OnXyzChanged);
    }

    private readonly ParameterState<int> _a;
    private readonly ParameterState<int> _c;
    private readonly ParameterState<int> _o;
    private readonly ParameterState<int> _z;

    protected void OnAbcChanged()
    {
        AbcHandlerCallCount++;
    }

    protected void OnXyzChanged()
    {
        XyzHandlerCallCount++;
    }

    public int AbcHandlerCallCount { get; private set; }

    public int OpHandlerCallCount { get; protected set; }

    public int XyzHandlerCallCount { get; private set; }

    [Parameter]
    public int A { get; set; }

    [Parameter]
    public int C { get; set; }

    [Parameter]
    public int O { get; set; }

    [Parameter]
    public int Z { get; set; }
}

public partial class SharedStateInheritanceTestComp : AnotherComponentBase
{
    public SharedStateInheritanceTestComp()
    {
        using var registerScope = CreateRegisterScope();

        // b should share handler with ac group
        _b = registerScope.RegisterParameter<int>(nameof(B))
            .WithParameter(() => B)
            .WithChangeHandler(OnAbcChanged);

        // o and p are not sharing their handler because lambdas are excluded, even if they contain the same code
        _p = registerScope.RegisterParameter<int>(nameof(P))
            .WithParameter(() => P)
            .WithChangeHandler(() => OpHandlerCallCount++);
        // x should share handler with yzgroup
        _x = registerScope.RegisterParameter<int>(nameof(X))
            .WithParameter(() => X)
            .WithChangeHandler(OnXyzChanged);
        _y = registerScope.RegisterParameter<int>(nameof(Y))
            .WithParameter(() => Y)
            .WithChangeHandler(OnXyzChanged);
    }

    private readonly ParameterState<int> _b;
    private readonly ParameterState<int> _p;
    private readonly ParameterState<int> _x;
    private readonly ParameterState<int> _y;


    [Parameter]
    public int B { get; set; }

    [Parameter]
    public int P { get; set; }

    [Parameter]
    public int X { get; set; }

    [Parameter]
    public int Y { get; set; }
}
