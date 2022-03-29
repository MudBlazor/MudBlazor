using System.ComponentModel;

namespace MudBlazor
{
    public enum TooltipClickBehavior
    {
        [Description("none")]
        None,
        [Description("click to show")]
        ClickToShow,
        [Description("click to hide")]
        ClickToHide,
    }
}
