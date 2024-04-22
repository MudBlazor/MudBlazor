using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
#nullable enable
    public partial class MudSwipeArea : MudComponentBase
    {
        private static readonly string[] _preventDefaultEventNames = { "touchstart", "touchend", "touchcancel" };

        private double? _swipeDelta;
        internal int[]? _listenerIds;
        internal double? _xDown, _yDown;
        private bool _preventDefaultChanged;
        private ElementReference _componentRef;

        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public RenderFragment? ChildContent { get; set; }

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
        /// Usable especially when swiping up/down - this will prevent the whole page from scrolling up/down.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public bool PreventDefault { get; set; }

        /// <inheritdoc />
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

        /// <inheritdoc />
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
            if (_xDown is null || _yDown is null)
            {
                return;
            }

            var xDiff = _xDown.Value - arg.ChangedTouches[0].ClientX;
            var yDiff = _yDown.Value - arg.ChangedTouches[0].ClientY;

            if (Math.Abs(xDiff) < Sensitivity && Math.Abs(yDiff) < Sensitivity)
            {
                _xDown = _yDown = null;
                return;
            }

            var swipeDirection = Math.Abs(xDiff) > Math.Abs(yDiff) ?
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
            _xDown = _yDown = null;
        }

        internal void OnTouchCancel(TouchEventArgs arg)
        {
            _xDown = _yDown = null;
        }
    }
}
