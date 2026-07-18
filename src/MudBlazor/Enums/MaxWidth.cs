using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// Specifies the maximum width of a container or dialog using responsive breakpoints from extra small to extra-extra large, or <c>False</c> to remove the limit.
    /// </summary>
    [EnumExtensions]
    public enum MaxWidth
    {
        [Description("lg")]
        Large,
        [Description("md")]
        Medium,
        [Description("sm")]
        Small,
        [Description("xl")]
        ExtraLarge,
        [Description("xxl")]
        ExtraExtraLarge,
        [Description("xs")]
        ExtraSmall,
        [Description("false")]
        False,
    }
}
