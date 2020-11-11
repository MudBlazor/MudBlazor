using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudIcon : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("")
           .AddClass($"mud-icon-default", Color == Color.Default)
          .AddClass($"mud-svg-icon-root", !String.IsNullOrEmpty(Icon))
          .AddClass($"mud-icon-root", !String.IsNullOrEmpty(FontIcon))
          .AddClass($"mud-{Color.ToDescriptionString()}-text", Color != Color.Default)
          .AddClass($"mud-icon-size-{Size.ToDescriptionString()}")
          .AddClass(FontClass, !String.IsNullOrEmpty(FontClass))
          .AddClass(Class)
        .Build();

        /// <summary>
        /// If set will display an SVG Icon.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// If set will display Font Icon.
        /// </summary>
        [Parameter] public string FontIcon { get; set; }

        /// <summary>
        /// Font Icon Class, only applies if Font Icon is used.
        /// </summary>
        [Parameter] public string FontClass { get; set; }

        /// <summary>
        /// The Size of the icon.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Inherit;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
