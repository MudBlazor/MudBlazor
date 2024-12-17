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
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The <see cref="MudMenu"/> which contains this item.
        /// </summary>
        [CascadingParameter]
        public MudMenu? MudMenu { get; set; }

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
        /// The browser tab/window opened when a click occurs and <see cref="Href"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. This property allows navigation to open a new tab/window or to reuse a specific tab.  Possible values are <c>_blank</c>, <c>_self</c>, <c>_parent</c>, <c>_top</c>, <c>noopener</c>, or the name of an <c>iframe</c> element.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string? Target { get; set; }

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
        /// The icon displayed for this menu item.
        /// </summary>
        /// <remarks>
        /// When set, this menu will display a <see cref="MudIconButton" />.
        /// </remarks>
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
        /// The size of the icon when <see cref="Icon"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

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
        /// <remarks>
        /// This event only occurs if <see cref="Href"/> is not set.
        /// </remarks>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected async Task OnClickHandlerAsync(MouseEventArgs ev)
        {
            if (Disabled)
            {
                return;
            }

            if (AutoClose && MudMenu is not null)
            {
                await MudMenu.CloseAllMenusAsync();
            }

            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(ev);
            }
        }
    }
}
