using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// A width based on a responsive breakpoint.
    /// </summary>
    [EnumExtensions]
    public enum Width
    {
        [Description("xs")]
        xs,
        [Description("sm")]
        sm,
        [Description("md")]
        md,
        [Description("lg")]
        lg,
        [Description("xl")]
        xl,
        [Description("xxl")]
        xxl,
        [Description("false")]
        False,
    }
}
