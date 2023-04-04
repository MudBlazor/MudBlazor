using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public partial class MudSwipeArea : MudComponentBase
    {
        internal double? _xDown, _yDown;
        private double? _swipeDelta;
        internal ElementReference _componentRef;
        private static readonly string[] _preventDefaultEventNames = { "touchstart", "touchend", "touchcancel" };
        internal int[] _listenerIds;

        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public RenderFragment ChildContent { get; set; }

        [Obsolete("Use OnSwipeEnd instead.")]
        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public Action<SwipeDirection> OnSwipe { get; set; }

        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public EventCallback<SwipeEventArgs> OnSwipeEnd { get; set; }

        /// <summary>
        /// Swipe threshold in pixels. If SwipeDelta is below Sensitivity then OnSwipe is not called.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public int Sensitivity { get; set; } = 100;

        /// <summary>
        /// Prevents default behavior of the browser when swiping.
        /// Usable espacially when swiping up/down - this will prevent the whole page from scrolling up/down.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public bool PreventDefault { get; set; }

        private bool _preventDefaultChanged;

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var preventDefault = parameters.GetValueOrDefault<bool>(nameof(PreventDefault));
            if (preventDefault != PreventDefault)
            {
                _preventDefaultChanged = true;
            }

            await base.SetParametersAsync(parameters);
        }

        private async Task SetPreventDefaultInternal(bool value)
        {
            if (value)
            {
                _listenerIds = await _componentRef.AddDefaultPreventingHandlers(_preventDefaultEventNames);
            }
            else
            {
                if (_listenerIds != null)
                {
                    await _componentRef.RemoveDefaultPreventingHandlers(_preventDefaultEventNames, _listenerIds);
                    _listenerIds = null;
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_preventDefaultChanged)
            {
                _preventDefaultChanged = false;
                await SetPreventDefaultInternal(PreventDefault);
            }
        }

        internal void OnTouchStart(TouchEventArgs arg)
        {
            _xDown = arg.Touches[0].ClientX;
            _yDown = arg.Touches[0].ClientY;
        }

        internal async Task OnTouchEnd(TouchEventArgs arg)
        {
            if (_xDown == null || _yDown == null)
                return;

            var xDiff = _xDown.Value - arg.ChangedTouches[0].ClientX;
            var yDiff = _yDown.Value - arg.ChangedTouches[0].ClientY;

            if (Math.Abs(xDiff) < Sensitivity && Math.Abs(yDiff) < Sensitivity)
            {
                _xDown = _yDown = null;
                return;
            }

            SwipeDirection swipeDirection = Math.Abs(xDiff) > Math.Abs(yDiff) ?
                xDiff > 0 ? SwipeDirection.RightToLeft : SwipeDirection.LeftToRight :
                yDiff > 0 ? SwipeDirection.BottomToTop : SwipeDirection.TopToBottom;

            if (Math.Abs(xDiff) > Math.Abs(yDiff))
            {
                _swipeDelta = xDiff;
            }
            else
            {
                _swipeDelta = yDiff;
            }

            await OnSwipeEnd.InvokeAsync(new SwipeEventArgs(arg, swipeDirection, _swipeDelta, this));
            if (OnSwipe != null)
            {
                await InvokeAsync(() => OnSwipe(swipeDirection));
            }
            _xDown = _yDown = null;
        }

        /// <summary>
        /// The last successful swipe difference in pixels since the last OnSwipe invocation
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use OnSwipeEnd to get SwipeDelta")]
        public double? GetSwipeDelta() => _swipeDelta;

        internal void OnTouchCancel(TouchEventArgs arg)
        {
            _xDown = _yDown = null;
        }
    }
}
