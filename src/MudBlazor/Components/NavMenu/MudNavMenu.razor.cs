// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A list of navigation links with support for groups.
    /// </summary>
    /// <seealso cref="MudNavGroup"/>
    /// <seealso cref="MudNavLink"/>
    public partial class MudNavMenu : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-navmenu")
                .AddClass($"mud-navmenu-{Color.ToDescriptionString()}")
                .AddClass($"mud-navmenu-margin-{Margin.ToDescriptionString()}")
                .AddClass("mud-navmenu-dense", Dense)
                .AddClass("mud-navmenu-rounded", Rounded)
                .AddClass($"mud-navmenu-bordered mud-border-{Color.ToDescriptionString()}", Bordered)
                .AddClass(Class)
                .Build();

        [CascadingParameter]
        private NavigationContext? NavigationContext { get; set; }

        /// <summary>
        /// The color of the active <see cref="MudNavLink" />.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Shows a border on the active <see cref="MudNavLink"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public bool Bordered { get; set; }

        /// <summary>
        /// Shows a rounded border for all <see cref="MudNavLink" /> items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  
        /// When <c>true</c>, the theme <c>border-radius</c> value will be used. 
        /// Only takes affect if <see cref="Bordered"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public bool Rounded { get; set; }

        /// <summary>
        /// The vertical spacing between <see cref="MudNavLink" /> items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Margin.None"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// Uses compact vertical padding to all <see cref="MudNavLink"/> items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  
        /// Will be overridden if <see cref="Margin"/> is not <see cref="Margin.None"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The content within this menu.
        /// </summary>
        /// <remarks>
        /// Typically contains <see cref="MudNavLink" />, <see cref="MudNavGroup"/>, <see cref="MudText"/>, and <see cref="MudDivider"/> components.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
