using System.ComponentModel;

namespace MudBlazor
{
    public enum Variant
    {
        [Description("regular")]
        Regular,
        [Description("dense")]
        Dense,
        [Description("text")]
        Text,
        [Description("outlined")]
        Outlined,
        [Description("contained")]
        Contained
    }
}
