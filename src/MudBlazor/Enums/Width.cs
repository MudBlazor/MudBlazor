using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// Specifies the width of an element using responsive breakpoints such as <c>xs</c>, <c>sm</c>, <c>md</c>, <c>lg</c>, <c>xl</c>, or <c>xxl</c>, or <c>False</c> for no set width.
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
