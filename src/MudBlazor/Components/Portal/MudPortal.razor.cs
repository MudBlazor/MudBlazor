using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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

        [Inject] public IJSRuntime JS { get; set; }

        [CascadingParameter] public MouseEvent ActivationEvent { get; set; }

        [Parameter] public bool Autopositioned { get; set; } = true;

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public bool IsVisible { get; set; }

        [Parameter] public bool IsFixed { get; set; }

        [Parameter] public bool OpenOnHover { get; set; }

        [Parameter] public bool LockScroll { get; set; }

        [Parameter] public Type Type { get; set; }

        [Parameter] public ElementReference AnchorRef{ get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsVisible)
            {
                await AddOrUpdateItem();
              
                if (Autopositioned) WindowResizeListener.OnResized += OnWindowResize;               
            }
            else
            {
                if (firstRender) return;
                await RemoveItem();
            }
        }

        private async Task AddOrUpdateItem()
        {
            await ConfigureRects();
            await ConfigureCssPosition();
            CorrectAnchorBoundaries();
            ConfigurePortalItem();

            Portal.AddOrUpdate(_portalItem);

            if (LockScroll) await ScrollManager.LockScrollAsync();

        }

        private async Task RemoveItem()
        {
            Portal.Remove(_portalItem);
            if (LockScroll) await ScrollManager.UnlockScrollAsync();
            if (Autopositioned) WindowResizeListener.OnResized -= OnWindowResize;
        }

        private async Task ConfigureRects()
        {
            await JS.InvokeVoidAsync("window.setStylePositionInParent", _portalRef, "relative");
            
            var apagarAnchorRect = await AnchorRef.MudGetBoundingClientRectAsync();
            if (apagarAnchorRect.Width > 1000000) return;
            _portalItem.AnchorRect = await _portalRef.MudGetBoundingClientRectAsync();
            _portalItem.FragmentRect = await _fragmentRef.MudGetClientRectFromFirstChildAsync();

        }

        private async Task ConfigureCssPosition()
        {
            if (Type == typeof(MudTooltip)) return;

            var isFixed = await _portalRef.MudHasFixedAncestorsAsync();
            if (isFixed) _portalItem.CssPosition = "fixed";

        }

        private void CorrectAnchorBoundaries()
        {
            if (_portalItem.FragmentRect is null || _portalItem.AnchorRect is null) return;

            if (_portalItem.FragmentRect.IsOutsideBottom)
            {
                _portalItem.AnchorRect.Top -=
                   2 * (_portalItem.FragmentRect.Top - _portalItem.AnchorRect.Bottom)
                    + _portalItem.AnchorRect.Height
                    + _portalItem.FragmentRect.Height;

            }
            if (_portalItem.FragmentRect.IsOutsideTop)
            {
                _portalItem.AnchorRect.Top +=
                    2 * (Math.Abs(_portalItem.AnchorRect.Top - _portalItem.FragmentRect.Bottom))
                  + _portalItem.AnchorRect.Height
                  + _portalItem.FragmentRect.Height;

            }
            if (_portalItem.FragmentRect.IsOutsideLeft)
            {
                _portalItem.AnchorRect.Left +=
                     FragmentIsAboveorBelowAnchor
                        ? _portalItem.AnchorRect.Left - _portalItem.FragmentRect.Left
                        : 2 * (Math.Abs(_portalItem.AnchorRect.Left - _portalItem.FragmentRect.Right))
                            + _portalItem.FragmentRect.Width
                            + _portalItem.AnchorRect.Width;

            }
            if (_portalItem.FragmentRect.IsOutsideRight)
            {
                _portalItem.AnchorRect.Left -=
                    FragmentIsAboveorBelowAnchor
                    ? _portalItem.FragmentRect.Right - _portalItem.AnchorRect.Right
                    : 2 * (Math.Abs(_portalItem.FragmentRect.Left - _portalItem.AnchorRect.Right))
                        + _portalItem.FragmentRect.Width
                        + _portalItem.AnchorRect.Width;
            }
        }

        private bool FragmentIsBelowAnchor
            => _portalItem.FragmentRect.Top > _portalItem.AnchorRect.Bottom;

        private bool FragmentIsAboveAnchor
            => _portalItem.FragmentRect.Bottom < _portalItem.AnchorRect.Top;

        private bool FragmentIsAboveorBelowAnchor
            => FragmentIsAboveAnchor || FragmentIsBelowAnchor;

        private void ConfigurePortalItem()
        {
            _portalItem.Id = _id;
            _portalItem.Fragment = ChildContent;
            _portalItem.Type = Type;
            _portalItem.OpenOnHover = ActivationEvent == MouseEvent.MouseOver;
        }

       
        private void OnWindowResize(object sender, BrowserWindowSize e)
        {
            Task.Run(async () =>
           {
               if (IsVisible || OpenOnHover)
               {
                   await AddOrUpdateItem();
               }
               else
               {
                   await RemoveItem();
               }
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
