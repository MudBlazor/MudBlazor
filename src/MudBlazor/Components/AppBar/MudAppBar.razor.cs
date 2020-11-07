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
                .AddClass($"mud-appbar-position-{Position.ToDescriptionString()}")
                .AddClass($"mud-left", LayoutState.DrawerAnchor == Anchor.Left)
                .AddClass($"mud-right", LayoutState.DrawerAnchor == Anchor.Right)
                .AddClass($"mud-appbar-drawer-open", LayoutState.DrawerOpen)
                .AddClass($"mud-appbar-drawer-clipped", LayoutState.DrawerClipped)
                .AddClass($"mud-elevation-{Elevation.ToString()}")
                .AddClass($"mud-appbar-color-{Color.ToDescriptionString()}", Color != Color.Default)
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
        /// The positioning type. The behavior of the different options is described in the MDN web docs. Note: sticky is not universally supported and will fall back to static when unavailable.
        /// </summary>
        [Parameter] public Position Position { get; set; } = Position.Fixed;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] LayoutState LayoutState { get; set; }
    }
}
