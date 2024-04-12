using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

#nullable enable
public partial class SharedStateHandlerTestComp : MudComponentBase
{
    public SharedStateHandlerTestComp()
    {
        // abc shared handler group
        _a = RegisterParameterBuilder<int>(nameof(A))
            .WithParameter(() => A)
            .WithChangeHandler(OnAbcChanged);
        _b = RegisterParameterBuilder<int>(nameof(B))
            .WithParameter(() => B)
            .WithChangeHandler(OnAbcChanged);
        _c = RegisterParameterBuilder<int>(nameof(C))
            .WithParameter(() => C)
            .WithChangeHandler(OnAbcChanged);
        // o and p are not sharing their handler because lambdas are excluded, even if they contain the same code
        _o = RegisterParameterBuilder<int>(nameof(O))
            .WithParameter(() => O)
            .WithChangeHandler(() => OpHandlerCallCount++);
        _p = RegisterParameterBuilder<int>(nameof(P))
            .WithParameter(() => P)
            .WithChangeHandler(() => OpHandlerCallCount++);
        // xyz shared handler group
        _x = RegisterParameterBuilder<int>(nameof(X))
            .WithParameter(() => X)
            .WithChangeHandler(OnXyzChanged);
        _y = RegisterParameterBuilder<int>(nameof(Y))
            .WithParameter(() => Y)
            .WithChangeHandler(OnXyzChanged);
        _z = RegisterParameterBuilder<int>(nameof(Z))
            .WithParameter(() => Z)
            .WithChangeHandler(OnXyzChanged);
    }

    private readonly ParameterState<int> _a;
    private readonly ParameterState<int> _b;
    private readonly ParameterState<int> _c;
    private readonly ParameterState<int> _o;
    private readonly ParameterState<int> _p;
    private readonly ParameterState<int> _x;
    private readonly ParameterState<int> _y;
    private readonly ParameterState<int> _z;

    private void OnAbcChanged()
    {
        AbcHandlerCallCount++;
    }

    private void OnXyzChanged()
    {
        XyzHandlerCallCount++;
    }

    public int AbcHandlerCallCount { get; private set; }
    public int OpHandlerCallCount { get; private set; }
    public int XyzHandlerCallCount { get; private set; }

    [Parameter]
    public int A { get; set; }

    [Parameter]
    public int B { get; set; }

    [Parameter]
    public int C { get; set; }

    [Parameter]
    public int O { get; set; }

    [Parameter]
    public int P { get; set; }

    [Parameter]
    public int X { get; set; }

    [Parameter]
    public int Y { get; set; }

    [Parameter]
    public int Z { get; set; }
}
