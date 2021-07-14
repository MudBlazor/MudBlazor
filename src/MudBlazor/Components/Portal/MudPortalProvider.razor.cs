using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPortalProvider : IDisposable
    {
        [Inject] private IPortal Portal { get; set; }

        private string AnchorClass => new CssBuilder("portal-anchor").Build();

        /// <summary>
        /// Set the coordinates (x,y,width,height) of the point where the Portal is going to be anchored
        /// </summary>
        private string AnchorStyle(PortalItem item) =>
            new StyleBuilder()
            .AddStyle("top", item.CssPosition == "fixed" || !item.IsRendered
                ? item.AnchorRect?.Top.ToPixels()
                : item.AnchorRect?.AbsoluteTop.ToPixels())
            .AddStyle("left", item.CssPosition == "fixed" || !item.IsRendered
                ? item.AnchorRect?.Left.ToPixels()
                : item.AnchorRect?.AbsoluteLeft.ToPixels())
            .AddStyle("height", item.AnchorRect?.Height.ToPixels())
            .AddStyle("width", item.AnchorRect?.Width.ToPixels())
            .AddStyle("position", !item.IsRendered ? "fixed" : item.CssPosition)
            .AddStyle("z-index", new ZIndex().Popover.ToString(), item.CssPosition == "fixed")
            .Build();

        protected override void OnInitialized() => Portal.OnChange += HandleChange;

        /// <summary>
        /// This is called when the portal adds or removes an item
        /// </summary>
        private void HandleChange(object _, PortalEventsArg e)
        {
            InvokeAsync(StateHasChanged);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            Portal.OnChange -= HandleChange;
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
