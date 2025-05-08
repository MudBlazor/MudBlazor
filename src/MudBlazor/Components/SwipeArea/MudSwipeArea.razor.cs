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
        #region Fields & Parameters

        private static readonly string[] _preventDefaultEventNames = ["onpointerdown", "onpointerup", "onpointercancel", "onpointermove", "onpointerleave"];

        private double? _swipeDelta;
        internal int[]? _listenerIds;
        internal double? _xDown, _yDown;
        internal double? _xDownway, _yDownway;
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

        #endregion

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

        internal async Task OnPointerMove(PointerEventArgs arg)
        {
            if (_isSwipeOnProgress == false)
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

            await OnSwipeMove.InvokeAsync(new MultiDimensionSwipeEventArgs(arg, new List<SwipeDirection>() { xDiff == 0 ? SwipeDirection.None : xDiff > 0 ? SwipeDirection.RightToLeft : SwipeDirection.LeftToRight, yDiff == 0 ? SwipeDirection.None : yDiff > 0 ? SwipeDirection.BottomToTop : SwipeDirection.TopToBottom }, new List<double?>() { xDiff, yDiff }, this));

            _xDownway = arg.ClientX;
            _yDownway = arg.ClientY;
        }

        internal async Task OnPointerUp(PointerEventArgs arg)
        {
            if (_xDown is null || _yDown is null)
            {
                _isSwipeOnProgress = false;
                return;
            }

            var xDiff = _xDown.Value - arg.ClientX;
            var yDiff = _yDown.Value - arg.ClientY;

            if (OnSwipeMove.HasDelegate == false && Math.Abs(xDiff) < Sensitivity && Math.Abs(yDiff) < Sensitivity)
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

        internal async Task OnPointerCancel(PointerEventArgs arg)
        {
            Cancel();
            await OnSwipeCancel.InvokeAsync(arg);
        }

        protected SwipeDirection GetSwipeDirection(double? xFirst, double? yFirst, double? xLast, double? yLast)
        {
            var xDiff = (xFirst - xLast) ?? 0;
            var yDiff = (yFirst - yLast) ?? 0;

            return Math.Abs(xDiff) > Math.Abs(yDiff) ?
                xDiff > 0 ? SwipeDirection.RightToLeft : SwipeDirection.LeftToRight :
                yDiff > 0 ? SwipeDirection.BottomToTop : SwipeDirection.TopToBottom;
        }

        public void Cancel()
        {
            _xDown = _yDown = _xDownway = _yDownway = null;
            _isSwipeOnProgress = false;
        }

    }
}
