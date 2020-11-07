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
                .AddClass($"mud-left", Layout?.HasDrawer(Anchor.Left))
                .AddClass($"mud-right", Layout?.HasDrawer(Anchor.Right))
                .AddClass($"mud-appbar-drawer-open", Layout?.IsDrawerOpen(Anchor.Left))
                .AddClass($"mud-appbar-drawer-clipped", Layout?.IsDrawerClipped(Anchor.Left))
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

        [CascadingParameter] MudLayout Layout { get; set; }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            if(Layout!=null)
                Layout.DrawersChanged+=OnDrawersChanged;
        }

        private void OnDrawersChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            try
            {
                if (Layout!=null)
                    Layout.DrawersChanged -= OnDrawersChanged;
            }
            catch (Exception) { }
        }
    }
}
