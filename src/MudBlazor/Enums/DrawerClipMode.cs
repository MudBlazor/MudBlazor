using System.ComponentModel;

namespace MudBlazor
{
    public enum DrawerClipMode
    {
        [Description("never")]
        Never,
        [Description("docked")]
        Docked,
        [Description("always")]
        Always
    }
}
