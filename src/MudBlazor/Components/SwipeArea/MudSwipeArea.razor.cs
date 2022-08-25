﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public partial class MudSwipeArea : MudComponentBase
    {
        internal double? _xDown, _yDown;
        private ElementReference _componentRef;
        private static readonly string[] preventDefaultEventNames = { "touchstart", "touchend", "touchcancel" };
        private int[] _listenerIds;

        [Inject] public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.SwipeArea.Behavior)]
        public Action<SwipeDirection> OnSwipe { get; set; }

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
                _listenerIds = await _componentRef.AddDefaultPreventingHandlers(preventDefaultEventNames);
            }
            else
            {
                if (_listenerIds != null)
                {
                    await _componentRef.RemoveDefaultPreventingHandlers(preventDefaultEventNames, _listenerIds);
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
