using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;
using MudBlazor.Services;

namespace MudBlazor
{
    public partial class MudPortal : ComponentBase, IDisposable
    {
        private Guid _id = Guid.NewGuid();

        private ElementReference _portalRef;


        private PortalItem _portalItem = new();

        [Inject] public IPortal Portal { get; set; }

        [Inject] public IResizeListenerService WindowResizeListener { get; set; }

        [Inject] public IScrollManager ScrollManager { get; set; }

        [Parameter] public bool Autopositioned { get; set; } = true;

        [Parameter] public bool AutoDirection { get; set; } = true;

        [Parameter] public BoundingClientRect ClientRect { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public bool IsVisible { get; set; }

        [Parameter] public bool IsEnabled { get; set; } = true;

        [Parameter] public bool LockScroll { get; set; } = false;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (IsVisible && Autopositioned)
            {
                WindowResizeListener.OnResized += OnWindowResize;
            }
            else
            {
                WindowResizeListener.OnResized -= OnWindowResize;
            }

            if (IsVisible)
            {
                await ConfigurePortalItem();
                Portal.AddOrUpdate(_portalItem);
                if (LockScroll) await ScrollManager.LockScrollAsync();
            }
            else
            {
                if (LockScroll) await ScrollManager.UnlockScrollAsync();
                Portal.Remove(_portalItem);
            }
        }


        private async Task ConfigurePortalItem()
        {
            _portalItem.Id = _id;
            _portalItem.Fragment = ChildContent;
            _portalItem.ClientRect = await _portalRef.MudGetRelativeClientRectAsync();
            _portalItem.AutoDirection = AutoDirection;
        }

        private void OnWindowResize(object sender, BrowserWindowSize e)
        {
            Task.Run(async () =>
           {
               _portalItem.ClientRect = await _portalRef.MudGetRelativeClientRectAsync();
               Portal.AddOrUpdate(_portalItem);
           }).AndForget();
        }

        protected virtual void Dispose(bool disposing)
        {
            Portal.Remove(_portalItem);

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
