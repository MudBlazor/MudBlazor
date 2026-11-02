using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// A maximum width based on a responsive breakpoint.
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
