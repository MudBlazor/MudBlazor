using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPortalProvider : IDisposable
    {
        

        [Inject] internal IPortal Portal { get; set; }

        private string GetAnchorStyle(PortalItem item) =>
            new StyleBuilder()

                   .AddStyle("position", item.Position)
                   .AddStyle("top", item.ClientRect?.Top.ToPixels())
                   .AddStyle("left", item.ClientRect?.Left.ToPixels())
                   .AddStyle("height", item.ClientRect?.Height.ToPixels())
                   .AddStyle("width", item.ClientRect?.Width.ToPixels())
                   .AddStyle("z-index", new ZIndex().Popover.ToString(), item.Position == "fixed")
                   .Build();

        protected override void OnInitialized() => Portal.OnChange += Refresh;

        void Refresh(object e, EventArgs c) => InvokeAsync(StateHasChanged);
        
        public void Dispose()
        {
            Portal.OnChange -= Refresh;
            GC.SuppressFinalize(this);
        }
    }
}
