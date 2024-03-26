using System.ComponentModel;
using System;

namespace MudBlazor
{
    public enum OverflowBehavior
    {
        [Description("flip-never")]
        FlipNever,
        [Description("flip-onopen")]
        FlipOnOpen,
        [Description("flip-always")]
        FlipAlways,
    }
}
