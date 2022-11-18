using System.ComponentModel;

namespace MudBlazor
{
    public enum FullScreen
    {
        [Description("none")]
        None,
        [Description("full")]
        Full,
        [Description("full-without-appbar")]
        FullWithoutAppbar
    }
}
