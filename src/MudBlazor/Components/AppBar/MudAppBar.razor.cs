using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudAppBar : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-appbar")
                .AddClass($"mud-appbar-dense", Dense)
                .AddClass($"mud-appbar-fixed", Fixed)
                .AddClass($"mud-elevation-{Elevation}")
                .AddClass($"mud-theme-{Color.ToDescriptionString()}", Color != Color.Default)
                .AddClass(Class)
                .Build();

        protected string ToolBarClassname =>
            new CssBuilder("mud-toolbar-appbar")
                .AddClass(ToolBarClass)
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
        /// User class names, separated by spaces for the nested toolbar.
        /// </summary>
        [Parameter] public string ToolBarClass { get; set; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
