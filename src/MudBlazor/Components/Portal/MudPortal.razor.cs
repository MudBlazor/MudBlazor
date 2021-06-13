using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;

namespace MudBlazor
{
    public partial class MudPortal : MudComponentBase, IDisposable
    {
        private Guid _id = Guid.NewGuid();
        private ElementReference _portalRef;
        private PortalItem _portalItem = new();
        private bool? _hasFixedAncestor = null;

        [Inject] public IPortal Portal { get; set; }

        [Inject] public IResizeListenerService WindowResizeListener { get; set; }

        [Inject] public IScrollManager ScrollManager { get; set; }

        [Parameter] public bool Autopositioned { get; set; } = true;

        [Parameter] public bool AutoDirection { get; set; } = true;

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public bool IsVisible { get; set; }

        [Parameter] public bool IsEnabled { get; set; } = true;

        [Parameter] public bool LockScroll { get; set; }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!IsEnabled) return;
            if (_portalRef.Id != null)
                _hasFixedAncestor = _hasFixedAncestor ?? await _portalRef.MudHasFixedAncestorsAsync();
            await ConfigurePortalItem();
            ConfigureResizeListener();
            await AddOrRemovePortalItem();
        }

        private async Task AddOrRemovePortalItem()
        {
            if (IsVisible)
            {
                Portal.AddOrUpdate(_portalItem);
                if (LockScroll) await ScrollManager.LockScrollAsync();
            }
            else
            {
                if (LockScroll) await ScrollManager.UnlockScrollAsync();
                Portal.Remove(_portalItem);
            }
        }

        private void ConfigureResizeListener()
        {
            if (IsVisible && Autopositioned)
            {
                WindowResizeListener.OnResized += OnWindowResize;
            }

            if (!IsVisible && Autopositioned)
            {
                WindowResizeListener.OnResized -= OnWindowResize;
            }
        }

        private async Task ConfigurePortalItem()
        {
            _portalItem.ClientRect =
                _hasFixedAncestor != null && _hasFixedAncestor == true
                    ? await _portalRef.MudGetBoundingClientRectAsync()
                    : await _portalRef.MudGetRelativeClientRectAsync();

            _portalItem.Id = _id;
            _portalItem.Fragment = ChildContent;
            _portalItem.AutoDirection = AutoDirection;
            _portalItem.Position =
                _hasFixedAncestor != null && _hasFixedAncestor == true
                    ? "fixed"
                    : "absolute";
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
