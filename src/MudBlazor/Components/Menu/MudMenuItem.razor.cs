﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
#nullable enable
    public partial class MudMenuItem : MudComponentBase
    {
        [Inject]
        protected NavigationManager UriHelper { get; set; } = null!;

        [Inject]
        protected IJsApiService JsApiService { get; set; } = null!;

        [CascadingParameter]
        public MudMenu? MudMenu { get; set; }

        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// If set to a URL, clicking the button will open the referenced document. Use Target to specify where
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.ClickAction)]
        public string? Href { get; set; }

        /// <summary>
        /// Icon to be used for this menu entry
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The Icon Size.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// If set to false, clicking the menu item will keep the menu open
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.ClickAction)]
        public bool AutoClose { get; set; } = true;

        [Parameter]
        [Category(CategoryTypes.Menu.ClickAction)]
        public string? Target { get; set; }

        [Parameter]
        [Category(CategoryTypes.Menu.ClickAction)]
        public bool ForceLoad { get; set; }

        /// <summary>
        /// Raised when the menu item is activated by either the mouse or touch.
        /// Won't be raised if Href is also set.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected async Task OnClickHandlerAsync(MouseEventArgs ev)
        {
            if (Disabled)
            {
                return;
            }

            if (AutoClose)
            {
                if (MudMenu is not null)
                {
                    await MudMenu.CloseMenuAsync();
                }
            }

            if (Href != null)
            {
                if (string.IsNullOrWhiteSpace(Target))
                {
                    UriHelper.NavigateTo(Href, ForceLoad);
                }
                else
                {
                    await JsApiService.Open(Href, Target);
                }
            }
            else
            {
                if (OnClick.HasDelegate)
                {
                    await OnClick.InvokeAsync(ev);
                }
            }
        }
    }
}
