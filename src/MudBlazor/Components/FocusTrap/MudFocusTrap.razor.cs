using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public partial class MudFocusTrap : IDisposable
    {
        protected ElementReference _firstBumper;
        protected ElementReference _lastBumper;
        protected ElementReference _fallback;
        protected ElementReference _root;

        private bool _shiftDown;
        private bool _disabled;
        private bool _initialized;

        [Inject] protected IJSRuntime JsRuntime { get; set; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// If true, the focus will no longer loop inside the component.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => _disabled;
            set
            {
                if (_disabled != value)
                {
                    _disabled = value;
                    _initialized = false;
                }
            }
        }

        /// <summary>
        /// Defines on which element to set the focus when the component is created or enabled.
        /// When DefaultFocus.Element is used, the focus will be set to the FocusTrap itself, so the user will have to press TAB key once to focus the first tabbable element.
        /// </summary>
        [Parameter] public DefaultFocus DefaultFocus { get; set; } = DefaultFocus.FirstChild;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
                await SaveFocusAsync();

            if (!_initialized)
                await InitializeFocusAsync();
        }

        private Task OnBottomFocusAsync(FocusEventArgs args)
        {
            return FocusLastAsync();
        }

        private Task OnBumperFocusAsync(FocusEventArgs args)
        {
            return _shiftDown ? FocusLastAsync() : FocusFirstAsync();
        }

        private Task OnRootFocusAsync(FocusEventArgs args)
        {
            return FocusFallbackAsync();
        }

        private void OnRootKeyDown(KeyboardEventArgs args)
        {
            HandleKeyEvent(args, true);
        }

        private void OnRootKeyUp(KeyboardEventArgs args)
        {
            HandleKeyEvent(args, false);
        }

        private Task OnTopFocusAsync(FocusEventArgs args)
        {
            return FocusFirstAsync();
        }

        private Task InitializeFocusAsync()
        {
            _initialized = true;

            if (!_disabled)
            {
                switch (DefaultFocus)
                {
                    case DefaultFocus.Element: return FocusFallbackAsync();
                    case DefaultFocus.FirstChild: return FocusFirstAsync();
                    case DefaultFocus.LastChild: return FocusLastAsync();
                }
            }
            return Task.CompletedTask;
        }

        private Task FocusFallbackAsync()
        {
            return JsRuntime.InvokeVoidAsync("elementReference.focus", _fallback).AsTask();
        }

        private Task FocusFirstAsync()
        {
            return JsRuntime.InvokeVoidAsync("elementReference.focusFirst", _root, 2, 4).AsTask();
        }

        private Task FocusLastAsync()
        {
            return JsRuntime.InvokeVoidAsync("elementReference.focusLast", _root, 2, 4).AsTask();
        }

        private void HandleKeyEvent(KeyboardEventArgs args, bool isDown)
        {
            if (args.Key == "Tab")
                _shiftDown = args.ShiftKey;
        }

        private Task RestoreFocusAsync()
        {
            return JsRuntime.InvokeVoidAsync("elementReference.restoreFocus", _root).AsTask();
        }

        private Task SaveFocusAsync()
        {
            return JsRuntime.InvokeVoidAsync("elementReference.saveFocus", _root).AsTask();
        }

        public void Dispose()
        {
            if (!_disabled)
                RestoreFocusAsync().AndForget(TaskOption.Safe);
        }
    }
}
