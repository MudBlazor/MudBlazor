using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudAppBar : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-appbar")
                .AddClass($"mud-appbar-fixed", Fixed)
                .AddClass(GetDrawerClass(Anchor.Left))
                .AddClass(GetDrawerClass(Anchor.Right))
                .AddClass($"mud-elevation-{Elevation}")
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

        private string GetDrawerClass(Anchor anchor)
        {
            var drawer = Layout?.DrawerContainer.GetDrawerOrDefault(anchor);
            if (drawer == null)
                return string.Empty;


            var className = $"mud-appbar-drawer-{(drawer.Open && drawer.ClipMode == DrawerClipMode.Never ? "open" : "close")}-{drawer.Variant.ToDescriptionString()}";
            if (drawer.Variant == DrawerVariant.Responsive)
            {
                className += $"-{drawer.Breakpoint.ToDescriptionString()}";
            }
            className += $"-{anchor.ToDescriptionString()}";

            return className;
        }
    }
}
