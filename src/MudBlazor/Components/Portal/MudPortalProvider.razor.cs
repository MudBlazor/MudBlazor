using System;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPortalProvider : IDisposable
    {

        ElementReference _menuRef;

        [Inject] public IPortal Portal { get; set; }


        private string GetItemStyle(PortalItem item)
        {
            static string ToString(double? measure) =>
                measure?.ToString(CultureInfo.InvariantCulture) + "px";

            var result = new StyleBuilder()
                .AddStyle("position", "absolute")
                .AddStyle("z-index", new ZIndex().Popover.ToString())
                .AddStyle("width", ToString(item.ClientRect?.Width))
                .AddStyle("top", ToString(item.ClientRect.Top))
                .AddStyle("height", ToString(item.ClientRect.Height))
                .AddStyle("left", ToString(item.ClientRect?.Left))               
                .Build();
            return result;
        }

        protected override void OnInitialized()
        {
            Portal.OnChange += Refresh;
        }

        void Refresh(object e, EventArgs c)
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Portal.OnChange -= Refresh;
            GC.SuppressFinalize(this);
        }
    }
}
