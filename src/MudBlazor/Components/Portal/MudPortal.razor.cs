using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;

namespace MudBlazor
{
    public partial class MudPortal : MudComponentBase, IDisposable
    {
        private readonly Guid _id = Guid.NewGuid();
        private ElementReference _portalRef;
        private bool _isDisposed;

        [CascadingParameter] private bool Fixed { get; set; }
        [Inject] private IPortal Portal { get; set; }

        [Inject] public IResizeListenerService WindowResizeListener { get; set; }

        /// <summary>
        /// The content to be teleported
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// True when the Portal is going to be rendered
        /// </summary>
        [Parameter] public bool HasToRender { get; set; }

        /// <summary>
        /// The type of the component to be teleported
        /// </summary>
        [Parameter] public Type Type { get; set; }

        protected override void OnInitialized()
        {
            var item = new PortalItem
            {
                Id = _id,
                Fragment = ChildContent,
                Type = Type,
                CssPosition = Fixed ? "fixed" : "absolute"
            };
            Portal.Add(item);
            WindowResizeListener.OnResized += OnWindowResize;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var item = Portal.GetItem(_id).Clone();

            if (HasToRender)
            {
                item.IsRendered = true;
                //re-render the portal provider
                Portal.Update(item);
            }
            else
            {
                item.IsRendered = false;
            }

            await ConfigureRects(item);
            //re-render the portal provider.
            //If the item has the same properties as before, is not re-rendered
            Portal.Update(item);
        }

        private async Task ConfigureRects(PortalItem item)
        {
            //get the rects of the anchor and the fragment
            item.AnchorRect = await _portalRef.MudGetClientRectFromParentAsync();
            item.FragmentRect = await _portalRef.MudGetClientRectFromFirstChildAsync();

            //correct the position if it's out of the viewport
            Repositioning.CorrectAnchorBoundaries(item);

            //rest
        }

        /// <summary>
        /// If the window is resized, calculate the new coordinates of the PortalItem
        /// </summary>
        private async void OnWindowResize(object sender, BrowserWindowSize e)
        {
            if (!HasToRender) return;
            var item = Portal.GetItem(_id).Clone();
            await ConfigureRects(item);
            Portal.Update(item);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            if (disposing)
            {
                WindowResizeListener.OnResized -= OnWindowResize;
                Portal.Remove(_id);
            }
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
