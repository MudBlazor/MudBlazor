// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A navigation link as part of a <see cref="MudNavMenu"/>.
    /// </summary>
    /// <seealso cref="MudNavGroup"/>
    /// <seealso cref="MudNavMenu"/>
    public partial class MudNavLink : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-nav-item")
                .AddClass(Class)
                .Build();

        protected string LinkClassname =>
            new CssBuilder("mud-nav-link")
                .AddClass($"mud-nav-link-disabled", Disabled)
                .AddClass($"mud-ripple", Ripple && !Disabled)
                .Build();

        protected string IconClassname =>
            new CssBuilder("mud-nav-link-icon")
                .AddClass($"mud-nav-link-icon-default", IconColor == Color.Default)
                .Build();

        protected Dictionary<string, object?> Attributes
        {
            get => Disabled ? new Dictionary<string, object?>() : new Dictionary<string, object?>
            {
                { "href", Href },
                { "target", Target },
                { "rel", !string.IsNullOrWhiteSpace(Target) ? "noopener noreferrer" : string.Empty }
            };
        }

        protected int TabIndex => Disabled || NavigationContext is { Disabled: true } or { Expanded: false } ? -1 : 0;

        [Inject]
        private NavigationManager UriHelper { get; set; } = null!;

        [CascadingParameter]
        private INavigationEventReceiver? NavigationEventReceiver { get; set; }

        [CascadingParameter]
        private NavigationContext? NavigationContext { get; set; }

        /// <summary>
        /// The icon displayed for this link.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The color of the icon when <see cref="Icon"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public Color IconColor { get; set; } = Color.Default;

        /// <summary>
        /// Controls when this link is highlighted.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="NavLinkMatch.Prefix"/>.  This link is compared against the current URL to determine whether it is highlighted.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;

        /// <summary>
        /// The browser frame to open this link when <see cref="Href"/> is specified.
        /// </summary>
        /// <remarks>
        /// Possible values include <c>_blank</c>, <c>_self</c>, <c>_parent</c>, <c>_top</c>, or a <i>frame name</i>. <br/>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.ClickAction)]
        public string? Target { get; set; }

        /// <summary>
        /// The CSS applied when this link is active.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>active</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ComponentBase.Common)]
        public string ActiveClass { get; set; } = "active";

        /// <summary>
        /// Prevents the user from interacting with this link.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Shows a ripple effect when the user clicks this link.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// The URL to navigate to when this link is clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.ClickAction)]
        public string? Href { get; set; }

        /// <summary>
        /// Performs a full page load during navigation.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, client-side routing is bypassed and the browser is forced to load the new page from the server.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.ClickAction)]
        public bool ForceLoad { get; set; }

        /// <summary>
        /// The content within this link.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Occurs when this link has been clicked.
        /// </summary>
        /// <remarks>
        /// This event only occurs when the <see cref="Href"/> property has not been set.
        /// </remarks>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected Task HandleNavigation()
        {
            if (!Disabled && NavigationEventReceiver != null)
            {
                return NavigationEventReceiver.OnNavigation();
            }

            return Task.CompletedTask;
        }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
            {
                return;
            }

            if (Href is not null)
            {
                UriHelper.NavigateTo(Href, ForceLoad);
            }
            else
            {
                await OnClick.InvokeAsync(ev);
            }
        }
    }
}
