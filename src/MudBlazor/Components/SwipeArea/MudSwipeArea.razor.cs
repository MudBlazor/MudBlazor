using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interop;

namespace MudBlazor
{
    public partial class MudSwipeArea : MudComponentBase
    {
        private double? _xDown, _yDown;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<SwipeDirection> OnSwipe { get; set; }

        public void OnTouchStart(TouchEventArgs arg)
        {
            _xDown = arg.Touches[0].ClientX;
            _yDown = arg.Touches[0].ClientY;
        }

        public void OnTouchEnd(TouchEventArgs arg)
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

        public void OnTouchCancel(TouchEventArgs arg)
        {
            _xDown = _yDown = null;
        }
    }
}
