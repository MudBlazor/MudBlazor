using System.ComponentModel;

namespace MudBlazor
{
    public enum TooltipBehavior
    {
        [Description("all")]
        All,
        [Description("hover")]
        Hover,
        [Description("focus")]
        Focus,
        [Description("none")]
        None,
    }
}
