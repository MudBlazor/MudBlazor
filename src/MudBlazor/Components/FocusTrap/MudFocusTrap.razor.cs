using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{

    /// <summary>
    /// Prevents the keyboard focus from cycling out of its child content. Typically used within dropdown selectors and modal dialogs.
    /// </summary>
    /// <remarks>
    /// Typically used within dialogs and other overlays.
    /// </remarks>
    public partial class MudFocusTrap : IDisposable
    {
        private bool _disabled;
        private bool _disposed;
        private bool _initialized;

        protected string Classname =>
            new CssBuilder("mud-focus-trap")
                .AddClass("outline-none")
                .AddClass(Class)
                .Build();

        protected ElementReference _firstBumper;
        protected ElementReference _lastBumper;
        protected ElementReference _fallback;
        protected ElementReference _root;
        private ElementReference _top;
        private ElementReference _bottom;

        /// <summary>
        /// The content within this focus trap.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FocusTrap.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Prevents the user from interacting with this focus trap.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FocusTrap.Behavior)]
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
        /// The element which receives focus when this focus trap is created or enabled.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DefaultFocus.FirstChild"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FocusTrap.Behavior)]
        public DefaultFocus DefaultFocus { get; set; } = DefaultFocus.FirstChild;

        private string TrapTabIndex => Disabled ? "-1" : "0";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            // need to check _disposed because we do not want to Save Focus if disposed - will cause an exception
            if (_disposed) return;
            if (firstRender)
            {
                await SaveFocusAsync();
                if (_disposed) return;
                await RegisterFocusTrapAsync();
            }

            // need to check _disposed again because it could have changed during the above await
            // we do not want to initialize focus if disposed - will cause an exception
            if (_disposed) return;
            if (!_initialized)
            {
                await InitializeFocusAsync();
            }
        }

        private ValueTask RegisterFocusTrapAsync() =>
            _root.GetJSRuntime()?.InvokeVoidAsyncIgnoreErrors(
                "mudElementRef.registerFocusTrap", _root, _top, _firstBumper, _fallback, _lastBumper, _bottom) ?? ValueTask.CompletedTask;

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
            return _fallback.FocusAsync().AsTask();
        }

        private Task FocusFirstAsync()
        {
            return _root.MudFocusFirstAsync(2, 4).AsTask();
        }

        private Task FocusLastAsync()
        {
            return _root.MudFocusLastAsync(2, 4).AsTask();
        }

        private Task RestoreFocusAsync()
        {
            return _root.MudRestoreFocusAsync().AsTask();
        }

        private Task SaveFocusAsync()
        {
            return _root.MudSaveFocusAsync().AsTask();
        }

        /// <summary>
        /// Releases resources used by this focus trap.
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            if (!_disabled)
            {
                RestoreFocusAsync().CatchAndLog(ignoreExceptions: true);
            }
        }
    }
}
