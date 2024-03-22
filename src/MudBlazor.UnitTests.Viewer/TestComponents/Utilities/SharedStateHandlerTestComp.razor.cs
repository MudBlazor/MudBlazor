using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

#nullable enable
public partial class SharedStateHandlerTestComp : MudComponentBase
{
    public SharedStateHandlerTestComp()
    {
        // abc shared handler group
        _a = RegisterParameter(nameof(A), () => A, OnAbcChanged);
        _b = RegisterParameter(nameof(B), () => B, OnAbcChanged);
        _c = RegisterParameter(nameof(C), () => C, OnAbcChanged);
        // o and p are not sharing their handler because lambdas are excluded, even if they contain the same code
        _o = RegisterParameter(nameof(O), () => O, () => OpHandlerCallCount++);
        _p = RegisterParameter(nameof(P), () => P, () => OpHandlerCallCount++);
        // xyz shared handler group
        _x = RegisterParameter(nameof(X), () => X, OnXyzChanged);
        _y = RegisterParameter(nameof(Y), () => Y, OnXyzChanged);
        _z = RegisterParameter(nameof(Z), () => Z, OnXyzChanged);
    }

    private IParameterState<int> _a;
    private IParameterState<int> _b;
    private IParameterState<int> _c;
    private IParameterState<int> _o;
    private IParameterState<int> _p;
    private IParameterState<int> _x;
    private IParameterState<int> _y;
    private IParameterState<int> _z;

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
