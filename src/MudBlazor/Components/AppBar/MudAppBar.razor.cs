﻿using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
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
        /// If true, Appbar will be placed at the bottom of the screen.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AppBar.Behavior)]
        public bool Bottom { get; set; }

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AppBar.Appearance)]
        public int Elevation { set; get; } = 4;

        /// <summary>
        /// If true, compact padding will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AppBar.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// If true, left and right padding is added to the appbar. Default is true
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AppBar.Appearance)]
        public bool Gutters { get; set; } = true;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AppBar.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// If true, appbar will be Fixed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AppBar.Behavior)]
        public bool Fixed { get; set; } = true;

        /// <summary>
        /// If true, AppBar is allowed to wrap.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AppBar.Behavior)]
        public bool WrapContent { get; set; } = false;

        /// <summary>
        /// User class names, separated by spaces for the nested toolbar.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AppBar.Appearance)]
        public string? ToolBarClass { get; set; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.AppBar.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
