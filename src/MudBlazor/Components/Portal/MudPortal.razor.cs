using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;

namespace MudBlazor
{
    public partial class MudPortal : IDisposable
    {
        private Guid _id = Guid.NewGuid();
        private PortalItem _portalItem = new();


        private ElementReference _portalRef;



        [Inject] internal IPortal Portal { get; set; }

        [Inject] public IResizeListenerService WindowResizeListener { get; set; }

        [Inject] public IScrollManager ScrollManager { get; set; }

        [Parameter] public Type PortalType { get; set; }

        [Parameter] public Placement Placement { get; set; }
        
        [Parameter] public bool Autopositioned { get; set; } = true;

        [Parameter] public bool AutoDirection { get; set; } = true;

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public bool IsVisible { get; set; }

        [Parameter] public bool IsFixed { get; set; }

        [Parameter] public bool IsEnabled { get; set; } = true;

        [Parameter] public bool LockScroll { get; set; }
        [Inject] public IBrowserWindowSizeProvider ViewPort { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            ConfigurePortalItem();
            if (IsVisible)
            {
                await ConfigureAnchorRect();
                await AddItem();
            }
            else
            {
                await RemoveItem();
            }
        }

        private async Task AddItem()
        {
            Portal.AddOrUpdate(_portalItem);
            if (LockScroll) await ScrollManager.LockScrollAsync();
            if (Autopositioned) WindowResizeListener.OnResized += OnWindowResize;
        }

        private async Task RemoveItem()
        {
            Portal.Remove(_portalItem);
            if (LockScroll) await ScrollManager.UnlockScrollAsync();
            if (Autopositioned) WindowResizeListener.OnResized -= OnWindowResize;
        }

        private async Task ConfigureAnchorRect()
        {
            _portalItem.AnchorRect = await _portalRef.MudGetBoundingClientRectAsync();
            
        }

        private void ConfigurePortalItem()
        {
            _portalItem.Id = _id;
            _portalItem.PortalType = PortalType;
            _portalItem.Placement = Placement;
            _portalItem.Fragment = ChildContent;
            _portalItem.AutoDirection = AutoDirection;
            _portalItem.Position = IsFixed ? "fixed" : "absolute";
        }

        private void OnWindowResize(object sender, BrowserWindowSize e)
        {
            Task.Run(async () =>
           {
               _portalItem.AnchorRect = await _portalRef.MudGetBoundingClientRectAsync();
               if (IsVisible) { Portal.AddOrUpdate(_portalItem); } else { Portal.Remove(_portalItem); }
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
