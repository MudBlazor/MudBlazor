using System.ComponentModel;

namespace MudBlazor
{
    public enum Height
    {
        [Description("custom")]
        Custom,
        [Description("full")]
        Full,
        [Description("full-without-appbar")]
        FullWithoutAppbar
    }
}
