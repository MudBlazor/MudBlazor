using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudAppBar :  MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-appbar")
                .AddClass($"mud-appbar-fixed", Fixed)
                .AddClass($"mud-appbar-drawer-open", Layout?.IsDrawerOpen(Anchor.Left) == true || Layout?.IsDrawerOpen(Anchor.Right) == true)
                .AddClass($"mud-appbar-drawer-open-left", Layout?.IsDrawerOpen(Anchor.Left))
                .AddClass($"mud-appbar-drawer-open-right", Layout?.IsDrawerOpen(Anchor.Right))
                .AddClass($"mud-appbar-drawer-clipped", Layout?.IsDrawerClipped(Anchor.Left) == true  && !Layout?.HasDrawer(Anchor.Right) == true || Layout?.IsDrawerClipped(Anchor.Right) == true && !Layout?.HasDrawer(Anchor.Left) == true || Layout?.IsDrawerClipped(Anchor.Left) == true && Layout?.IsDrawerClipped(Anchor.Right) == true)
                .AddClass($"mud-appbar-drawer-clipped-left", Layout?.IsDrawerClipped(Anchor.Left) == true && !Layout?.IsDrawerClipped(Anchor.Right) == true)
                .AddClass($"mud-appbar-drawer-clipped-right", Layout?.IsDrawerClipped(Anchor.Right) == true && !Layout?.IsDrawerClipped(Anchor.Left) == true)
                .AddClass($"mud-elevation-{Elevation.ToString()}")
                .AddClass($"mud-theme-{Color.ToDescriptionString()}", Color != Color.Default)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 4;

        /// <summary>
        /// If true, compact padding will be used.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// If true, appbar will be Fixed.
        /// </summary>
        [Parameter] public bool Fixed { get; set; } = true;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] MudLayout Layout { get; set; }
    }
}
