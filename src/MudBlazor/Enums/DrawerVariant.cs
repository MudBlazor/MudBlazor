using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the behavior of a <see cref="MudDrawer"/>.
/// </summary>
public enum DrawerVariant
{
    /// <summary>
    /// The drawer will open above all other content until a section is selected.
    /// </summary>
    [Description("temporary")]
    Temporary,

    /// <summary>
    /// The drawer behaves like <see cref="DrawerVariant.Persistent"/> in wider screens, but <see cref="DrawerVariant.Temporary"/> on smaller screens.
    /// </summary>
    [Description("responsive")]
    Responsive,

    /// <summary>
    /// The drawer will open outside of its container, shifting other contents when opened.
    /// </summary>
    [Description("persistent")]
    Persistent,

    /// <summary>
    /// The drawer has a small width but will expand when hovering over it.
    /// </summary>
    [Description("mini")]
    Mini
}
