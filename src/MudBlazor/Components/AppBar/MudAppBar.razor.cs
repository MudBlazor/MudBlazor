using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a bar used to display actions, branding, navigation and screen titles.
    /// </summary>
    /// <remarks>
    /// This component is often used to keep important information persistent while browsing different pages to ease navigation and access to actions for users.
    /// </remarks>
    public partial class MudAppBar : MudComponentBase
    {
#nullable enable
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
        /// Gets or sets a value indicating whether the appbar will be placed at the bottom of the screen instead of the top.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AppBar.Behavior)]
        public bool Bottom { get; set; }

        /// <summary>
        /// Gets or sets the size of the drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>4</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AppBar.Appearance)]
        public int Elevation { set; get; } = 4;

        /// <summary>
        /// Gets or sets a value indicating whether compact padding will be used.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AppBar.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether left and right padding is added to the appbar.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AppBar.Appearance)]
        public bool Gutters { get; set; } = true;

        /// <summary>
        /// Gets or sets the color of the component. It supports the theme colors.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AppBar.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Gets or sets a value indicating whether the appbar remains in the same place when the page is scrolled.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AppBar.Behavior)]
        public bool Fixed { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the appbar content is allowed to wrap.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AppBar.Behavior)]
        public bool WrapContent { get; set; } = false;

        /// <summary>
        /// Gets or sets CSS classes applied to the nested toolbar.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  You can use spaces to separate multiple classes.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.AppBar.Appearance)]
        public string? ToolBarClass { get; set; }

        /// <summary>
        /// Gets or sets any child content for the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AppBar.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
