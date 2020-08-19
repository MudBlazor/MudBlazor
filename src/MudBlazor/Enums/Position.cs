using System.ComponentModel;

namespace MudBlazor
{
    public enum Position
    {
        [Description("fixed")]
        Fixed,
        [Description("absolute")]
        Absolute,
        [Description("relative")]
        Relative,
        [Description("sticky")]
        Sticky,
        [Description("static")]
        Static
    }
}
