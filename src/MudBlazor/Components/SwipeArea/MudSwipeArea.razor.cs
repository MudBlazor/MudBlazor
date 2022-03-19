using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public partial class MudSwipeArea : MudComponentBase
    {
        internal double? _xDown, _yDown;

        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public Action<SwipeDirection> OnSwipe { get; set; }

        private void OnTouchStart(TouchEventArgs arg)
        {
            _xDown = arg.Touches[0].ClientX;
            _yDown = arg.Touches[0].ClientY;
        }

        internal void OnTouchEnd(TouchEventArgs arg)
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

        internal void OnTouchCancel(TouchEventArgs arg)
        {
            _xDown = _yDown = null;
        }
    }
}
