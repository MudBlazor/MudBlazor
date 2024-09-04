using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// The color themes available in MudBlazor, allowing components to adapt their visual style based on the selected color.
/// </summary>
public enum Color
{
    /// <summary>
    /// The default color theme.
    /// </summary>
    [Description("default")]
    Default,

    /// <summary>
    /// The primary color theme, usually the main color used in the application.
    /// </summary>
    [Description("primary")]
    Primary,

    /// <summary>
    /// The secondary color theme, often used for accents and highlights.
    /// </summary>
    [Description("secondary")]
    Secondary,

    /// <summary>
    /// The tertiary color theme, typically used for additional accents or highlights.
    /// </summary>
    [Description("tertiary")]
    Tertiary,

    /// <summary>
    /// The info color theme, used to indicate informational messages.
    /// </summary>
    [Description("info")]
    Info,

    /// <summary>
    /// The success color theme, used to indicate successful operations.
    /// </summary>
    [Description("success")]
    Success,

    /// <summary>
    /// The warning color theme, used to indicate potential issues or warnings.
    /// </summary>
    [Description("warning")]
    Warning,

    /// <summary>
    /// The error color theme, used to indicate errors or critical issues.
    /// </summary>
    [Description("error")]
    Error,

    /// <summary>
    /// The dark color theme, usually used for dark mode or dark-themed elements.
    /// </summary>
    [Description("dark")]
    Dark,

    /// <summary>
    /// The transparent theme, making elements see-through.
    /// </summary>
    /// <remarks>
    /// Note: Not all components support this theme.
    /// </remarks>
    [Description("transparent")]
    Transparent,

    /// <summary>
    /// Inherits the color from the parent element.
    /// </summary>
    [Description("inherit")]
    Inherit,

    /// <summary>
    /// The surface color theme, typically used for the background or surface elements.
    /// </summary>
    /// <remarks>
    /// Note: Not all components support this theme.
    /// </remarks>
    [Description("surface")]
    Surface,
}
