using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudFocusTrap : IDisposable
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
                        FocusFirstAsync().FireAndForget();
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && !_disabled)
            {
                await SaveFocusAsync();
                await FocusFirstAsync();
            }
        }

        private Task OnBumperFocusAsync(FocusEventArgs args)
        {
            if (!_tabDown)
                return Task.CompletedTask;

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

        private Task FocusFallbackAsync()
        {
            return JsRuntime.InvokeVoidAsync("elementReference.focus", _fallback).AsTask();
        }

        private Task FocusFirstAsync()
        {
            return JsRuntime.InvokeVoidAsync("elementReference.focusFirst", _root, 1, 2).AsTask();
        }

        private Task FocusLastAsync()
        {
            return JsRuntime.InvokeVoidAsync("elementReference.focusLast", _root, 1, 2).AsTask();
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
                RestoreFocusAsync().FireAndForget();
        }
    }
}
