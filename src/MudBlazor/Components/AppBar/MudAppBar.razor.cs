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
                .AddClass($"mud-appbar-drawer-open", LayoutState.DrawerOpen)
                .AddClass($"mud-appbar-drawer-clipped", LayoutState.DrawerClipped)
                .AddClass($"mud-elevation-{Elevation.ToString()}")
                .AddClass($"mud-appbar-color-{Color.ToDescriptionString()}")
                .AddClass(Class)
                .Build();

        [Parameter] public int Elevation { set; get; } = 4;

        [Parameter] public Color Color { get; set; } = Color.Primary;

        [Parameter] public Position Position { get; set; } = Position.Fixed;

        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] LayoutState LayoutState { get; set; }
    }
}
