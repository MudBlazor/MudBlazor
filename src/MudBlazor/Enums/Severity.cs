using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// Indicates the severity level of an alert, snackbar, or message box, such as normal, info, success, warning, or error.
    /// </summary>
    [EnumExtensions]
    public enum Severity
    {
        [Description("normal")]
        Normal,
        [Description("info")]
        Info,
        [Description("success")]
        Success,
        [Description("warning")]
        Warning,
        [Description("error")]
        Error
    }
}
