using System.ComponentModel;

namespace MudBlazor
{
    public enum Color
    {
        [Description("inherit")]
        Inherit,
        [Description("default")]
        Default,
        [Description("primary")]
        Primary,
        [Description("secondary")]
        Secondary,
        [Description("info")]
        Info,
        [Description("success")]
        Success,
        [Description("warning")]
        Warning,
        [Description("danger")]
        Danger,
        [Description("dark")]
        Dark
    }
}
