using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Services;

namespace MudBlazor
{
    public partial class MudPortal : MudComponentBase, IAsyncDisposable
    {
        private readonly Guid _id = Guid.NewGuid();
        //    private ElementReference _portalRef;
        //    private bool _isDisposed;

        //    [CascadingParameter] private bool Fixed { get; set; }
        //    [Inject] private IPortal Portal { get; set; }
        //    [Inject] private IResizeListenerService WindowResizeListener { get; set; }
        [Inject] private IJSRuntime Js { get; set; }

        /// <summary>
        /// The content to be teleported
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        private bool _isConnected;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender == true)
            {
                await Js.InvokeVoidAsync("mudportal2.connect", _id);
                _isConnected = true;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public async ValueTask DisposeAsync()
        {
            if (_isConnected == false) { return; }

            await Js.InvokeVoidAsync("mudportal2.disconnect", _id);
        }

        //    /// <summary>
        //    /// True when the Portal is going to be rendered
        //    /// </summary>
        //    [Parameter] public bool IsVisible { get; set; }


        //    protected override void OnInitialized()
        //    {
        //        var item = new PortalItem
        //        {
        //            Id = _id,
        //            Fragment = ChildContent,
        //            CssPosition = Fixed ? "fixed" : "absolute"
        //        };
        //        Portal.Add(item);
        //    }

        //    protected override async Task OnAfterRenderAsync(bool firstRender)
        //    {
        //        if (firstRender)
        //        {
        //            WindowResizeListener.OnResized += OnWindowResize;
        //        }

        //        var item = Portal.GetItem(_id).Clone();

        //        item.IsVisible = IsVisible;
        //        Portal.Update(item);
        //        await Js.InvokeVoidAsync("mudHandlePortal", item.JavaScriptModel, _portalRef);
        //    }

        //    /// <summary>
        //    /// If the window is resized, calculate the new coordinates of the PortalItem
        //    /// </summary>
        //    private async void OnWindowResize(object sender, BrowserWindowSize e)
        //    {
        //        if (!IsVisible) return;
        //        var item = Portal.GetItem(_id).Clone();
        //        await Js.InvokeVoidAsync("mudHandlePortal", item.JavaScriptModel, _portalRef);
        //    }

        //    protected virtual void Dispose(bool disposing)
        //    {
        //        if (_isDisposed) return;
        //        if (disposing)
        //        {
        //            WindowResizeListener.OnResized -= OnWindowResize;
        //            Portal.Remove(_id);
        //        }
        //        _isDisposed = true;
        //    }

        //    public void Dispose()
        //    {
        //        Dispose(disposing: true);
        //        GC.SuppressFinalize(this);
        //    }
    }
}

