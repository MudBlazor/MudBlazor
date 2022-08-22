using System.ComponentModel;

namespace MudBlazor
{
    public enum OverflowBehavior
    {
        [Description("flip-never")]
        FlipNever,
        [Description("flip-onopen")]
        FilpOnOpen,
        [Description("flip-always")]
        FlipAlways,
    }
}
