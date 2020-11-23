using System.ComponentModel;

namespace MudBlazor
{
    public enum Color
    {
        [Description("default")]
        Default,
        [Description("primary")]
        Primary,
        [Description("secondary")]
        Secondary,
        [Description("tertiary")]
        Tertiary,
        [Description("info")]
        Info,
        [Description("success")]
        Success,
        [Description("warning")]
        Warning,
        [Description("error")]
        Error,
        [Description("dark")]
        Dark,
        [Description("transparent")]
        Transparent,
        [Description("inherit")]
        Inherit,
        [Description("surface")]
        Surface
    }
}
