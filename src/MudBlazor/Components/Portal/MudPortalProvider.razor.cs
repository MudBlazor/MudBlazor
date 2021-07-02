using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPortalProvider : IDisposable
    {
        [Inject] internal IPortal Portal { get; set; }

        [CascadingParameter] public bool RightToLeft { get; set; }

        private string ClassName => new CssBuilder()
            .AddClass("mud-application-layout-rtl", RightToLeft==true)
            .Build();

        protected override void OnInitialized() => Portal.OnChange += Refresh;

        //TODO Refresh only individual items
        private void Refresh(object e, EventArgs c)
        {
            InvokeAsync(StateHasChanged);
        }
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }
        public void Dispose()
        {
            Portal.OnChange -= Refresh;
            GC.SuppressFinalize(this);
        }
    }
}
