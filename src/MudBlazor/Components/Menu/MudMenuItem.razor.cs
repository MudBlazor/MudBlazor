using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// A choice displayed as part of a list within a <see cref="MudMenu"/> component.
    /// </summary>
    /// <seealso cref="MudMenu" />
    public partial class MudMenuItem : MudComponentBase
    {
        [Inject]
        protected NavigationManager UriHelper { get; set; } = null!;

        [Inject]
        protected IJsApiService JsApiService { get; set; } = null!;

        protected string Classname =>
            new CssBuilder("mud-menu-item")
                .AddClass("mud-disabled", GetDisabled())
                .AddClass("mud-ripple", !GetDisabled())
                .AddClass("mud-list-item-clickable", !GetDisabled())
                .AddClass("mud-menu-item-dense", GetDense())
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The <see cref="MudMenu"/> which contains this item.
        /// </summary>
        [CascadingParameter]
        public MudMenu? ParentMenu { get; set; }

        /// <summary>
        /// The text shown on this menu item if <see cref="ChildContent"/> is not set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public string? Label { get; set; }

        /// <summary>
        /// The content within this menu item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Prevents the user from interacting with this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The URL to navigate to when this menu item is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. When clicked, the browser will navigate to this URL.  Use the <see cref="Target"/> property to target a specific tab.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.ClickAction)]
        public string? Href { get; set; }

        /// <summary>
        /// The browser frame to open this link when <see cref="Href"/> is specified.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="LinkTarget.Self"/> in <see cref="MudGlobal.LinkDefaults.LinkTarget"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Link.Behavior)]
        public LinkTarget Target { get; set; } = MudGlobal.LinkDefaults.LinkTarget;

        /// <summary>
        /// The iframe to which to target when <see cref="Target"/> is set to <see cref="LinkTarget.Iframe"/>
        /// </summary>
        /// <remarks>
        /// Value should be a <i>frame name</i>. <br/>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.ClickAction)]
        public string? FrameTarget { get; set; }

        /// <summary>
        /// Performs a full page load during navigation.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, client-side routing is bypassed and the browser is forced to load the new page from the server.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.ClickAction)]
        public bool ForceLoad { get; set; }

        /// <summary>
        /// The icon displayed at the start of this menu item.  The size depends on whether or not the menu is using the dense variant.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The color of the icon when <see cref="Icon"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Inherit"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// Closes the menu when this item is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>false</c>, the menu will remain open after this item is clicked.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.ClickAction)]
        public bool AutoClose { get; set; } = true;

        /// <summary>
        /// Occurs when this menu item is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected string GetHtmlTag() => string.IsNullOrEmpty(Href) ? "div" : "a";

        protected bool GetDisabled() => Disabled || ParentMenu?.Disabled == true;

        protected bool GetDense() => ParentMenu?.GetDense() == true;

        protected Typo GetTypo() => GetDense() ? Typo.body2 : Typo.body1;

        /// <summary>
        /// The menu item is acting as the activator for a sub menu.
        /// </summary>
        protected bool ActivatesSubMenu => Class?.Contains("mud-menu-sub-menu-activator") == true;

        protected async Task OnClickHandlerAsync(MouseEventArgs ev)
        {
            if (GetDisabled())
            {
                return;
            }

            if (AutoClose && ParentMenu is not null)
            {
                await ParentMenu.CloseAllMenusAsync();
            }

            // Manual navigation is only required when the target is empty and a
            // forced reload is necessary; all other scenarios are managed by the HTML anchor.
            if (ForceLoad && !string.IsNullOrEmpty(Href) && string.IsNullOrEmpty(Target.GetDescriptionStringOrFrameTarget(FrameTarget)))
            {
                UriHelper.NavigateTo(Href, forceLoad: ForceLoad);
            }

            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(ev);
            }
        }
    }
}
