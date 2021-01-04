using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public partial class MudFocusTrap
    {
        protected ElementReference _firstBumper;
        protected ElementReference _lastBumper;
        protected ElementReference _fallback;
        protected ElementReference _root;

        private bool _tabDown;
        private bool _shiftDown;
        private bool _disabled;

        [Inject] protected IJSRuntime JsRuntime { get; set; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// If true, the focus will no longer loop inside the element.
        /// </summary>
        [Parameter] public bool Disabled
        {
            get => _disabled;
            set
            {
                if (_disabled != value)
                {
                    _disabled = value;

                    if (!_disabled)
                        FocusFirst();
                }
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender && !_disabled)
                FocusFirst();
        }

        private void OnBumperFocus(FocusEventArgs args)
        {
            if (_tabDown)
            {
                if (_shiftDown)
                    FocusLast();
                else
                    FocusFirst();
            }
        }

        private void OnRootFocus(FocusEventArgs args)
        {
            JsRuntime.InvokeVoidAsync("elementReference.focus", _fallback);
        }

        private void FocusFirst()
        {
            JsRuntime.InvokeVoidAsync("elementReference.focusFirst", _root, 1, 2);
        }

        private void FocusLast()
        {
            JsRuntime.InvokeVoidAsync("elementReference.focusLast", _root, 1, 2);
        }

        private void OnRootKeyDown(KeyboardEventArgs args)
        {
            HandleKeyEvent(args, true);
        }

        private void OnRootKeyUp(KeyboardEventArgs args)
        {
            HandleKeyEvent(args, false);
        }

        private void HandleKeyEvent(KeyboardEventArgs args, bool isDown)
        {
            switch (args.Key)
            {
                case "Tab":
                    _tabDown = isDown;
                    break;

                case "Shift":
                    _shiftDown = isDown;
                    break;
            }
        }
    }
}
