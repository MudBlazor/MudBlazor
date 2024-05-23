using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Specifies the distribution of children within a flex container along the main axis. 
/// </summary>
public enum Justify
{
    [Description("start")]
    FlexStart,
    [Description("center")]
    Center,
    [Description("end")]
    FlexEnd,
    [Description("space-between")]
    SpaceBetween,
    [Description("space-around")]
    SpaceAround,
    [Description("space-evenly")]
    SpaceEvenly
}
