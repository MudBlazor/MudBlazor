using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the clipping behavior of a <see cref="MudDrawer"/> when inside of a <see cref="MudLayout"/>.
/// </summary>
public enum DrawerClipMode
{
    /// <summary>
    /// The drawer will display over the <see cref="MudAppBar"/> and other content.
    /// </summary>
    [Description("never")]
    Never,

    /// <summary>
    /// The drawer will display underneath the <see cref="MudAppBar"/> and push content to the side when opening.
    /// </summary>
    [Description("docked")]
    Docked,

    /// <summary>
    /// The drawer will display underneath the <see cref="MudAppBar"/> and display over content when opened.
    /// </summary>
    [Description("always")]
    Always
}
