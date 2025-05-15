using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// An area which receives swipe events for devices where touch events are supported.
    /// </summary>
    public partial class MudSwipeArea : MudComponentBase
    {
        private static readonly string[] _preventDefaultEventNames = ["onpointerdown", "onpointerup", "onpointercancel", "onpointermove", "onpointerleave"];

        private double? _swipeDelta;
        internal int[]? _listenerIds;
        internal double? _xDown, _yDown;
        private double? _xDownway, _yDownway;
        private bool _isSwipeOnProgress;
        private bool _preventDefaultChanged;
        private ElementReference _componentRef;

        /// <summary>
        /// The content within this swipe area.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Occurs when a swipe has on progress. Ignores sensitivity.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public EventCallback<MultiDimensionSwipeEventArgs> OnSwipeMove { get; set; }

        /// <summary>
        /// Occurs when a swipe has ended.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public EventCallback<SwipeEventArgs> OnSwipeEnd { get; set; }

        /// <summary>
        /// Occurs when a swipe leaves the area.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public EventCallback<PointerEventArgs> OnSwipeLeave { get; set; }

        /// <summary>
        /// Occurs when a swipe cancelled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public EventCallback<PointerEventArgs> OnSwipeCancel { get; set; }

        /// <summary>
        /// The amount of pixels which must be swiped to raise the <see cref="OnSwipeEnd"/> event.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>100</c> (100 pixels).
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public int Sensitivity { get; set; } = 100;

        /// <summary>
        /// Prevents the default behavior of the browser when swiping.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. Typically <c>true</c> when swiping up or down, which will prevent the whole page from scrolling.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public bool PreventDefault { get; set; }

        protected string Classname =>
            new CssBuilder("mud-swipearea")
                .AddClass(Class)
                .Build();

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

        internal void OnPointerDown(PointerEventArgs arg)
        {
            _isSwipeOnProgress = true;
            _xDown = arg.ClientX;
            _yDown = arg.ClientY;
            _xDownway = arg.ClientX;
            _yDownway = arg.ClientY;
        }

        private async Task OnPointerMoveAsync(PointerEventArgs arg)
        {
            if (!_isSwipeOnProgress)
            {
                return;
            }

            var xDiff = (_xDownway - arg.ClientX) ?? 0;
            var yDiff = (_yDownway - arg.ClientY) ?? 0;

            if (Math.Abs(xDiff) > Math.Abs(yDiff))
            {
                _swipeDelta = xDiff;
            }
            else
            {
                _swipeDelta = yDiff;
            }

            var swipeDirection = GetSwipeDirections(xDiff, yDiff);
            await OnSwipeMove.InvokeAsync(new MultiDimensionSwipeEventArgs(arg, swipeDirection, [xDiff, yDiff], this));

            _xDownway = arg.ClientX;
            _yDownway = arg.ClientY;
        }

        internal async Task OnPointerUpAsync(PointerEventArgs arg)
        {
            if (_xDown is null || _yDown is null)
            {
                _isSwipeOnProgress = false;
                return;
            }

            var xDiff = _xDown.Value - arg.ClientX;
            var yDiff = _yDown.Value - arg.ClientY;

            if (!OnSwipeMove.HasDelegate && Math.Abs(xDiff) < Sensitivity && Math.Abs(yDiff) < Sensitivity)
            {
                Cancel();
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
            _xDown = _yDown = _xDownway = _yDownway = null;
            _isSwipeOnProgress = false;
        }

        internal Task OnPointerCancelAsync(PointerEventArgs arg)
        {
            Cancel();
            return OnSwipeCancel.InvokeAsync(arg);
        }

        public void Cancel()
        {
            _xDown = _yDown = _xDownway = _yDownway = null;
            _isSwipeOnProgress = false;
        }

        private static IReadOnlyList<SwipeDirection> GetSwipeDirections(double xDiff, double yDiff)
        {
            var horizontalDirection = GetDirection(xDiff, SwipeDirection.RightToLeft, SwipeDirection.LeftToRight);
            var verticalDirection = GetDirection(yDiff, SwipeDirection.BottomToTop, SwipeDirection.TopToBottom);

            return [horizontalDirection, verticalDirection];

            SwipeDirection GetDirection(double diff, SwipeDirection positiveDirection, SwipeDirection negativeDirection)
            {
                const double Epsilon = 1e-6;

                if (Math.Abs(diff) < Epsilon)
                {
                    return SwipeDirection.None;
                }

                return diff > Epsilon
                    ? positiveDirection
                    : negativeDirection;
            }
        }
    }
}
