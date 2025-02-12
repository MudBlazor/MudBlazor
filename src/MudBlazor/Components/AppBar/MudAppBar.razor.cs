using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Sections;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents a bar used to display actions, branding, navigation and screen titles.
/// </summary>
/// <remarks>
/// This component is often used to keep important information persistent while browsing different pages to ease navigation and access to actions for users.
/// </remarks>
/// <seealso cref="MudContextualActionBar"/>
public partial class MudAppBar : MudComponentBase
{
    internal static SectionOutlet ContextualActionBar { get; } = new();

    protected string Classname =>
        new CssBuilder("mud-appbar")
            .AddClass($"mud-appbar-dense", Dense)
            .AddClass($"mud-appbar-fixed-top", Fixed && !Bottom)
            .AddClass($"mud-appbar-fixed-bottom", Fixed && Bottom)
            .AddClass($"mud-elevation-{Elevation}")
            .AddClass($"mud-theme-{Color.ToDescriptionString()}", Color != Color.Default)
            .AddClass(Class)
            .Build();

    protected string ToolBarClassname =>
        new CssBuilder("mud-toolbar-appbar")
            .AddClass(ToolBarClass)
            .Build();

    /// <summary>
    /// Places the appbar at the bottom of the screen instead of the top.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.AppBar.Behavior)]
    public bool Bottom { get; set; }

    /// <summary>
    /// Allows the app bar to be overridden with page specific actions
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>. When <c>true</c>, can be overridden by <see cref="MudContextualActionBar"/>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.AppBar.Behavior)]
    public bool Contextual { get; set; } = false;

    /// <summary>
    /// The size of the drop shadow.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>4</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.AppBar.Appearance)]
    public int Elevation { set; get; } = 4;

    /// <summary>
    /// Uses compact padding.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.AppBar.Appearance)]
    public bool Dense { get; set; }

    /// <summary>
    /// Adds left and right padding to this appbar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.AppBar.Appearance)]
    public bool Gutters { get; set; } = true;

    /// <summary>
    /// The color of this appbar.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Default"/>.  Theme colors are supported.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.AppBar.Appearance)]
    public Color Color { get; set; } = Color.Default;

    /// <summary>
    /// Fixes this appbar in place as the page is scrolled.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.  When <c>false</c>, the appbar will scroll with other page content.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.AppBar.Behavior)]
    public bool Fixed { get; set; } = true;

    /// <summary>
    /// Allows appbar content to wrap.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.AppBar.Behavior)]
    public bool WrapContent { get; set; } = false;

    /// <summary>
    /// The CSS classes applied to the nested toolbar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  You can use spaces to separate multiple classes.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.AppBar.Appearance)]
    public string? ToolBarClass { get; set; }

    /// <summary>
    /// The content within this component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.AppBar.Behavior)]
    public RenderFragment? ChildContent { get; set; }
}
