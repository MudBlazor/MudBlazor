using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudLink : MudComponentBase, IHandleEvent
    {
        protected string Classname =>
            new CssBuilder("mud-typography mud-link")
                .AddClass($"mud-{Color.ToDescriptionString()}-text")
                .AddClass($"mud-link-underline-{Underline.ToDescriptionString()}")
                .AddClass($"mud-typography-{Typo.ToDescriptionString()}")
                // When Href is empty, link's hover cursor is text "I beam" even when OnClick has a delegate.
                // To change this for more expected look change hover cursor to a pointer:
                .AddClass("cursor-pointer", Href == default && OnClick.HasDelegate && !Disabled)
                .AddClass("mud-link-disabled", Disabled)
                .AddClass(Class)
                .Build();

        private Dictionary<string, object?> Attributes
        {
            get
            {
                var attributes = new Dictionary<string, object?>();

                if (Disabled)
                {
                    attributes["aria-disabled"] = "true";
                }
                else
                {
                    attributes["href"] = Href;
                    attributes["target"] = Target;
                }

                if (OnClick.HasDelegate)
                {
                    attributes["role"] = "button";
                }

                // Apply user attributes last so they take precedence.
                foreach (var attribute in UserAttributes)
                {
                    attributes[attribute.Key] = attribute.Value;
                }

                return attributes;
            }
        }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Link.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Typography variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Link.Appearance)]
        public Typo Typo { get; set; } = Typo.body1;

        /// <summary>
        /// Controls when the link should have an underline.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Link.Appearance)]
        public Underline Underline { get; set; } = Underline.Hover;

        /// <summary>
        /// The URL, which is the actual link.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Link.Behavior)]
        public string? Href { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the link, if Href is specified.
        /// Possible values: _blank | _self | _parent | _top | <i>framename</i>
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Link.Behavior)]
        public string? Target { get; set; }

        /// <summary>
        /// If true, the navlink will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Link.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Link.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Link click event.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
            {
                return;
            }

            await OnClick.InvokeAsync(ev);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// See: https://github.com/MudBlazor/MudBlazor/issues/8365
        /// <para/>
        /// Since <see cref="MudLink"/> implements only single <see cref="EventCallback"/> <see cref="OnClick"/> this is safe to disable globally within the component.
        /// </remarks>
        Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg) => callback.InvokeAsync(arg);
    }
}
