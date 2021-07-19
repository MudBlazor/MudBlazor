using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;

namespace MudBlazor
{
    public partial class MudPortalProvider : IDisposable
    {
        [Inject] private IPortal Portal { get; set; }

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
