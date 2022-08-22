using System.ComponentModel;

namespace MudBlazor
{
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
}
