﻿using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudIcon : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("")
                .AddClass($"mud-icon-default", Color == Color.Default)
                .AddClass($"mud-svg-icon-root", !string.IsNullOrEmpty(Icon))
                .AddClass($"mud-icon-root", !string.IsNullOrEmpty(FontIcon))
                .AddClass($"mud-{Color.ToDescriptionString()}-text", Color != Color.Default)
                .AddClass($"mud-icon-size-{Size.ToDescriptionString()}")
                .AddClass(FontClass, !string.IsNullOrEmpty(FontClass))
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
        /// The viewbox size of an svg element. Default: '0 0 24 24'
        /// </summary>
        [Parameter] public string ViewBox { get; set; } = "0 0 24 24";

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
