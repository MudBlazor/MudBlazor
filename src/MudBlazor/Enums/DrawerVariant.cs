using System.ComponentModel;

namespace MudBlazor
{
    public enum DrawerVariant
    {
        [Description("temporary")]
        Temporary,
        [Description("responsive")]
        Responsive,
        [Description("persistent")]
        Persistent,
        [Description("mini")]
        Mini
    }
}
