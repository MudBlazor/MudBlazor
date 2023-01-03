using System.ComponentModel;
using System;

namespace MudBlazor
{
    public enum OverflowBehavior
    {
        [Description("flip-never")]
        FlipNever,
        [Obsolete("This value has a typo and will be removed. Please use FlipOnOpen")]
        [Description("flip-onopen")]
        FilpOnOpen,
        [Description("flip-onopen")]
        FlipOnOpen,
        [Description("flip-always")]
        FlipAlways,
    }
}
