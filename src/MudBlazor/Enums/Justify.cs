using System.ComponentModel;

namespace MudBlazor
{
    public enum Justify
    {
        [Description("flex-start")]
        FlexStart,
        [Description("center")]
        Center,
        [Description("flex-end")]
        FlexEnd,
        [Description("space-between")]
        SpaceBetween,
        [Description("space-around")]
        SpaceAround,
        [Description("space-evenly")]
        SpaceEvenly
    }
}
