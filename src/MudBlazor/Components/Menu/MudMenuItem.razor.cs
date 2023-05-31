﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public partial class MudMenuItem : MudComponentBase
    {
        [CascadingParameter] public MudMenu MudMenu { get; set; }

        [Parameter] [Category(CategoryTypes.Menu.Behavior)] public RenderFragment ChildContent { get; set; }
        [Parameter] [Category(CategoryTypes.Menu.Behavior)] public bool Disabled { get; set; }

        [Inject] public NavigationManager UriHelper { get; set; }
        [Inject] public IJsApiService JsApiService { get; set; }

        /// <summary>
        /// If set to a URL, clicking the button will open the referenced document. Use Target to specify where (Obsolete replaced by Href)
        /// </summary>
        [Obsolete("Use Href Instead.", false)]
        [Parameter]
        [Category(CategoryTypes.Menu.ClickAction)]
        public string Link { get => Href; set => Href = value; }

        /// <summary>
        /// If set to a URL, clicking the button will open the referenced document. Use Target to specify where
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.ClickAction)]
        public string Href { get; set; }

        /// <summary>
        /// Icon to be used for this menu entry
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string Icon { get; set; }
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

        [Parameter] [Category(CategoryTypes.Menu.ClickAction)] public string Target { get; set; }
        [Parameter] [Category(CategoryTypes.Menu.ClickAction)] public bool ForceLoad { get; set; }

        [Parameter]
        [Category(CategoryTypes.Menu.ClickAction)]
        [Obsolete($"Use {nameof(OnClick)} instead. This will be removed in v7.")]
        public ICommand Command { get; set; }

        [Parameter]
        [Category(CategoryTypes.Menu.ClickAction)]
        [Obsolete("This will be removed in v7.")]
        public object CommandParameter { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter] public EventCallback<TouchEventArgs> OnTouch { get; set; }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            if (AutoClose) MudMenu.CloseMenu();

            if (Href != null)
            {
                if (string.IsNullOrWhiteSpace(Target))
                    UriHelper.NavigateTo(Href, ForceLoad);
                else
                    await JsApiService.Open(Href, Target);
            }
            else
            {
                await OnClick.InvokeAsync(ev);
#pragma warning disable CS0618
                if (Command?.CanExecute(CommandParameter) ?? false)
                {
                    Command.Execute(CommandParameter);
                }
#pragma warning restore CS0618
            }
        }

        protected internal async Task OnTouchHandler(TouchEventArgs ev)
        {
            if (Disabled)
                return;
            if (AutoClose) MudMenu.CloseMenu();

            if (Href != null)
            {
                if (string.IsNullOrWhiteSpace(Target))
                    UriHelper.NavigateTo(Href, ForceLoad);
                else
                    await JsApiService.Open(Href, Target);
            }
            else
            {
                await OnTouch.InvokeAsync(ev);
#pragma warning disable CS0618
                if (Command?.CanExecute(CommandParameter) ?? false)
                {
                    Command.Execute(CommandParameter);
                }
#pragma warning restore CS0618
            }
        }
    }
}
