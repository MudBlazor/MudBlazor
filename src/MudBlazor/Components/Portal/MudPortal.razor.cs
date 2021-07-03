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

        /// <summary>
        /// The content to be teleported
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// True when the Portal is going to be rendered
        /// </summary>
        [Parameter] public bool IsRendered { get; set; }

        /// <summary>
        /// The type of the component to be teleported
        /// </summary>
        [Parameter] public Type Type { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
           
            if (IsRendered)
            {
                //if is rendered, set the properties of the PortalItem and add it to the PortalService
                await ConfigurePortalItem();
                if (_portalItem.CssPosition != "fixed") WindowResizeListener.OnResized += OnWindowResize;
                Portal.AddOrUpdate(_portalItem);
            }
            else
            {
                //if it's not rendered, detach the listener and remove the element from the PortalService
                WindowResizeListener.OnResized -= OnWindowResize;
                Portal.Remove(_portalItem);
            }
        }

        private async Task ConfigurePortalItem()
        {
            //get the rects of the anchor and the fragment
            _portalItem.AnchorRect = await _portalRef.MudGetClientRectFromParentAsync();
            _portalItem.FragmentRect = await _fragmentRef.MudGetClientRectFromFirstChildAsync();

            //correct the position if it's out of the viewport
            CorrectAnchorBoundaries();

            //if has an ancestor with position==fixed, then set the position of the element to fixed
            if (Type != typeof(MudTooltip))
            {
                var isFixed = await _portalRef.MudHasFixedAncestorsAsync();
                if (isFixed) _portalItem.CssPosition = "fixed";
            }

            //rest
            _portalItem.Id = _id;
            _portalItem.Fragment = ChildContent;
            _portalItem.Type = Type;

        }

        /// <summary>
        /// If the PortalItem is outside of the viewport, correct the position to be visible
        /// </summary>
        private void CorrectAnchorBoundaries()
        {
            if (_portalItem.FragmentRect is null || _portalItem.AnchorRect is null) return;

            // comes out at the bottom 
            if (_portalItem.FragmentRect.IsOutsideBottom)
            {
                _portalItem.AnchorRect.Top -=
                   2 * (_portalItem.FragmentRect.Top - _portalItem.AnchorRect.Bottom)
                    + _portalItem.AnchorRect.Height
                    + _portalItem.FragmentRect.Height;
            }

            // comes out at the top
            if (_portalItem.FragmentRect.IsOutsideTop)
            {
                _portalItem.AnchorRect.Top +=
                    2 * (Math.Abs(_portalItem.AnchorRect.Top - _portalItem.FragmentRect.Bottom))
                  + _portalItem.AnchorRect.Height
                  + _portalItem.FragmentRect.Height;
            }

            // comes out at the left
            if (_portalItem.FragmentRect.IsOutsideLeft)
            {
                _portalItem.AnchorRect.Left +=
                     FragmentIsAboveorBelowAnchor
                        ? _portalItem.AnchorRect.Left - _portalItem.FragmentRect.Left
                        : 2 * (Math.Abs(_portalItem.AnchorRect.Left - _portalItem.FragmentRect.Right))
                            + _portalItem.FragmentRect.Width
                            + _portalItem.AnchorRect.Width;
            }

            // comes out at the right
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

        private bool FragmentIsAboveorBelowAnchor
            => _portalItem.FragmentRect.Top > _portalItem.AnchorRect.Bottom
            || _portalItem.FragmentRect.Bottom < _portalItem.AnchorRect.Top;

        /// <summary>
        /// If the window is resized, calculate the new coordinates of the PortalItem
        /// </summary>
        private void OnWindowResize(object sender, BrowserWindowSize e)
        {
            if (!IsRendered) return;
            Task.Run(async () =>
            {
                await ConfigurePortalItem();
                Portal.AddOrUpdate(_portalItem);
            });
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
