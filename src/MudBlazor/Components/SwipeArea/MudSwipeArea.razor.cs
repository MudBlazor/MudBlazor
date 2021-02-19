using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.Services;

namespace MudBlazor
{
    public partial class MudSwipeArea : MudComponentBase, IDisposable
    {
        private double? _xDown, _yDown;
        private ElementReference _swipeArea;
        private DotNetObjectReference<MudSwipeArea> _dotnet;
        private int _touchStartId, _touchEndId, _touchCancelId;

        [Inject]
        private IDomService DomService { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<SwipeDirection> OnSwipe { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _dotnet = DotNetObjectReference.Create(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _touchStartId = await DomService.AddEventListener(_swipeArea, _dotnet, "touchstart", nameof(OnTouchStart), true);
                _touchEndId = await DomService.AddEventListener(_swipeArea, _dotnet, "touchend", nameof(OnTouchEnd), true);
                _touchCancelId = await DomService.AddEventListener(_swipeArea, _dotnet, "touchcancel", nameof(OnTouchCancel), true);
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        
        public void Dispose()
        {
            DomService.RemoveEventListener(_swipeArea, "touchstart", _touchStartId);
            DomService.RemoveEventListener(_swipeArea, "touchend", _touchEndId);
            DomService.RemoveEventListener(_swipeArea, "touchcancel", _touchCancelId);
        }

        [JSInvokable]
        public void OnTouchStart(TouchEvent arg)
        {
            _xDown = arg.Touches[0].ClientX;
            _yDown = arg.Touches[0].ClientY;
        }

        [JSInvokable]
        public void OnTouchEnd(TouchEvent arg)
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
                    OnSwipe.InvokeAsync(SwipeDirection.RightToLeft);
                }
                else
                {
                    OnSwipe.InvokeAsync(SwipeDirection.LeftToRight);
                }
            }
            else
            {
                if (yDiff > 0)
                {
                    OnSwipe.InvokeAsync(SwipeDirection.BottomToTop);
                }
                else
                {
                    OnSwipe.InvokeAsync(SwipeDirection.TopToBottom);
                }
            }

            _xDown = _yDown = null;
        }

        [JSInvokable]
        public void OnTouchCancel(TouchEvent arg)
        {
            _xDown = _yDown = null;
        }
    }
}
