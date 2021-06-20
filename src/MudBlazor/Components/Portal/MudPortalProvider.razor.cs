using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;

namespace MudBlazor
{
    public partial class MudPortalProvider : IDisposable
    {
        [Inject] internal IPortal Portal { get; set; }

        protected override void OnInitialized() => Portal.OnChange += Refresh;

        //TODO Refresh only individual items
        private void Refresh(object e, EventArgs c) => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            Portal.OnChange -= Refresh;
            GC.SuppressFinalize(this);
        }
    }
}
