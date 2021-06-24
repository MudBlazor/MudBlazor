using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPortal : MudComponentBase, IDisposable
    {
        private Guid _id = Guid.NewGuid();
        private PortalItem _portalItem = new();
        private ElementReference _portalRef;
        private ElementReference _fragmentRef;

        [Inject] internal IPortal Portal { get; set; }

        [Inject] public IResizeListenerService WindowResizeListener { get; set; }

        [Inject] public IScrollManager ScrollManager { get; set; }

        [Parameter] public bool Autopositioned { get; set; } = false;

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public bool IsVisible { get; set; }

        [Parameter] public bool IsFixed { get; set; }

        [Parameter] public bool LockScroll { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (IsVisible)
            {
                await ConfigureAnchorRect();
                ConfigurePosition();
                ConfigurePortalItem();
                await AddItem();
            }
            else
            {
                if (firstRender) return;
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
            _portalItem.FragmentRect = await _fragmentRef.MudGetClientRectFromFirstChildAsync();

        }

        private void ConfigurePosition()
        {
            if (_portalItem.FragmentRect.IsOutsideBottom)
            {
                _portalItem.AnchorRect.Top -=
                    _portalItem.FragmentRect.Height 
                    + _portalItem.FragmentRect.Top
                    - _portalItem.AnchorRect.Top 
                    ;

            }
            if (_portalItem.FragmentRect.IsOutsideTop)
            {
                _portalItem.AnchorRect.Top +=
                    _portalItem.AnchorRect.Top
                    - _portalItem.FragmentRect.Top
                    + _portalItem.AnchorRect.Height
                    + _portalItem.AnchorRect.Top
                    - _portalItem.FragmentRect.Bottom;

            }
            if (_portalItem.FragmentRect.IsOutsideLeft)
            {
                _portalItem.AnchorRect.Left += 
                    _portalItem.AnchorRect.Left 
                    - _portalItem.FragmentRect.Left 
                    + _portalItem.AnchorRect.Width
                    + _portalItem.AnchorRect.Left
                    - _portalItem.FragmentRect.Right;

            }
            if (_portalItem.FragmentRect.IsOutsideRight)
            {
                _portalItem.AnchorRect.Left -= _portalItem.FragmentRect.Width + _portalItem.AnchorRect.Width;

            }




        }

        private void ConfigurePortalItem()
        {
            _portalItem.Id = _id;
            _portalItem.Fragment = ChildContent;
            _portalItem.Position = IsFixed ? "fixed" : "absolute";
        }

        private void OnWindowResize(object sender, BrowserWindowSize e)
        {
            Task.Run(async () =>
           {
               await ConfigureAnchorRect();
               await Task.Delay(0);
               if (IsVisible) { 
                   Portal.AddOrUpdate(_portalItem); } else { Portal.Remove(_portalItem); }
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
