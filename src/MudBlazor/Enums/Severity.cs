using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// The severity level of a status or message.
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
