using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudNavLink : MudBaseSelectItem, IHandleEvent
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

        [CascadingParameter]
        private INavigationEventReceiver? NavigationEventReceiver { get; set; }

        [CascadingParameter]
        private NavigationContext? NavigationContext { get; set; }

        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors, default value uses the themes drawer icon color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public Color IconColor { get; set; } = Color.Default;

        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;

        [Parameter]
        [Category(CategoryTypes.NavMenu.ClickAction)]
        public string? Target { get; set; }

        /// <summary>
        /// User class names when active, separated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ComponentBase.Common)]
        public string ActiveClass { get; set; } = "active";

        protected Task HandleNavigation()
        {
            if (!Disabled && NavigationEventReceiver != null)
            {
                return NavigationEventReceiver.OnNavigation();
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// See: https://github.com/MudBlazor/MudBlazor/issues/8365
        /// <para/>
        /// Since <see cref="MudLink"/> implements only single <see cref="EventCallback"/> <see cref="MudBaseSelectItem.OnClick"/> this is safe to disable globally within the component.
        /// </remarks>
        Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg) => callback.InvokeAsync(arg);
    }
}
