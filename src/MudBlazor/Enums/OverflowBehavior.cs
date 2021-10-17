using System.ComponentModel;

namespace MudBlazor
{
    public enum OverflowBehavior
    {
        [Description("none")]
        None,
        [Description("flip-once")]
        FlipOnce,
        [Description("flip-always")]
        FlipAlways,
    }
}
