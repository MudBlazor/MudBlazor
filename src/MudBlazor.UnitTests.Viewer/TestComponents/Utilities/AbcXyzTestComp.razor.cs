using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor.UnitTests;

public partial class AbcXyzTestComp : MudComponentBase
{
    public AbcXyzTestComp() : base() {
        // abc shared handler group
        _a=RegisterParameter(nameof(A), ()=>A, OnAbcChanged);
        _b=RegisterParameter(nameof(B), ()=>B, OnAbcChanged);
        _c=RegisterParameter(nameof(C), ()=>C, OnAbcChanged);
        // xyz shared handler group
        _x=RegisterParameter(nameof(X), ()=>X, OnXyzChanged);
        _y=RegisterParameter(nameof(Y), ()=>Y, OnXyzChanged);
        _z=RegisterParameter(nameof(Z), ()=>Z, OnXyzChanged);
    }

    private ParameterState<int> _a;
    private ParameterState<int> _b;
    private ParameterState<int> _c;
    private ParameterState<int> _x;
    private ParameterState<int> _y;
    private ParameterState<int> _z;

    private void OnAbcChanged()
    {
        AbcHandlerCallCount++;
    }

    private void OnXyzChanged()
    {
        XyzHandlerCallCount++;
    }

    public int AbcHandlerCallCount { get; private set; }
    public int XyzHandlerCallCount { get; private set; }
    
    [Parameter]
    public int A { get; set; }

    [Parameter]
    public int B { get; set; }

    [Parameter]
    public int C { get; set; }
    
    [Parameter]
    public int X { get; set; }

    [Parameter]
    public int Y { get; set; }

    [Parameter]
    public int Z { get; set; }

}
