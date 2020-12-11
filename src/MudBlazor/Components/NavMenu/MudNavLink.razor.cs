using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNavLink : MudBaseSelectItem
    {
        [Inject] IJSRuntime JsRuntime { get; set; }

        protected string Classname =>
        new CssBuilder("mud-nav-item")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass(Class)
        .Build();


        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter] public string Icon { get; set; }
        [Parameter] public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;

        [CascadingParameter] public MudDrawer Drawer { get; set; }

        private async void OnNavigation(MouseEventArgs args)
        {
            var browserSize = await JsRuntime.InvokeAsync<BrowserWindowSize>("resizeListener.getBrowserWindowSize");
            if (browserSize.Width < 1280)
            {
                OnClickHandler(args);
                if (Drawer != null)
                {
                    if (Drawer.Open)
                    {
                        await Drawer.OpenChanged.InvokeAsync(false);
                    }
                    else
                    {
                        await Drawer.OpenChanged.InvokeAsync(true);
                    }
                }
            }
        }

        private class BrowserWindowSize
        {
            public int Height { get; set; }
            public int Width { get; set; }
        }
    }
}
