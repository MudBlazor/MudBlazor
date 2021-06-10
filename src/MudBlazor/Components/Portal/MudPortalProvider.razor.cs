using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPortalProvider : IDisposable
    {
        [Inject] public IPortal Portal { get; set; }

        private string GetAnchorStyle(PortalItem item) =>
            new StyleBuilder()
                   .AddStyle("position", "absolute")
                   .AddStyle("top", item.ClientRect?.Top.ToPixels())
                   .AddStyle("left", item.ClientRect?.Left.ToPixels())
                   .AddStyle("height", item.ClientRect?.Height.ToPixels())
                   .AddStyle("width", item.ClientRect?.Width.ToPixels())
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
