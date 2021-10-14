using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public partial class MudSwipeArea : MudComponentBase
    {
        private double? _xDown, _yDown;

        [Inject] public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public Action<SwipeDirection> OnSwipe { get; set; }

        [Parameter]
        public bool PreventDefault { get; set; }

        private ElementReference _componentRef;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && PreventDefault)
            {
                await JsRuntime.InvokeVoidAsync("addDefaultPreventingHandler", _componentRef, "touchstart");
                await JsRuntime.InvokeVoidAsync("addDefaultPreventingHandler", _componentRef, "touchend");
                await JsRuntime.InvokeVoidAsync("addDefaultPreventingHandler", _componentRef, "touchcancel");
            }
        }

        private void OnTouchStart(TouchEventArgs arg)
        {
            _xDown = arg.Touches[0].ClientX;
            _yDown = arg.Touches[0].ClientY;
        }

        private void OnTouchEnd(TouchEventArgs arg)
        {
            if (_xDown == null || _yDown == null)
                return;

            var xDiff = _xDown.Value - arg.ChangedTouches[0].ClientX;
            var yDiff = _yDown.Value - arg.ChangedTouches[0].ClientY;

            if (Math.Abs(xDiff) < 100 && Math.Abs(yDiff) < 100)
            {
                _xDown = _yDown = null;
                return;
            }

            if (Math.Abs(xDiff) > Math.Abs(yDiff))
            {
                if (xDiff > 0)
                {
                    InvokeAsync(() => OnSwipe(SwipeDirection.RightToLeft));
                }
                else
                {
                    InvokeAsync(() => OnSwipe(SwipeDirection.LeftToRight));
                }
            }
            else
            {
                if (yDiff > 0)
                {
                    InvokeAsync(() => OnSwipe(SwipeDirection.BottomToTop));
                }
                else
                {
                    InvokeAsync(() => OnSwipe(SwipeDirection.TopToBottom));
                }
            }

            _xDown = _yDown = null;
        }

        private void OnTouchCancel(TouchEventArgs arg)
        {
            _xDown = _yDown = null;
        }
    }
}
