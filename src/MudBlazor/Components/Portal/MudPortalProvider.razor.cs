using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPortalProvider : IDisposable
    {
        private PortalItem _itemToRender;

        [Inject] internal IPortal Portal { get; set; }

        protected override void OnInitialized() => Portal.OnChange +=  HandleChange ;

        /// <summary>
        /// This is called when the portal adds or removes an item
        /// </summary>
        private void HandleChange(object _, PortalEventsArg e)
        {
            //this is the only item that changed, so the only that is going to rerender
            _itemToRender = e.Item;
            InvokeAsync(StateHasChanged);
        }

        protected virtual void Dispose(bool disposing)
        {
            Portal.OnChange -= HandleChange;
        }

        void IDisposable.Dispose()
        {
          
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
