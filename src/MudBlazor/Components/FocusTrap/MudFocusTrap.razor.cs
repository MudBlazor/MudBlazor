using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A component which prevents the keyboard focus from cycling out of its child content.
    /// </summary>
    /// <remarks>
    /// Typically used within dialogs and other overlays.
    /// </remarks>
    public partial class MudFocusTrap : IDisposable
    {
        private bool _shiftDown;
        private bool _disabled;
        private bool _initialized;
        private bool _shouldRender = true;

        protected string Classname =>
            new CssBuilder("mud-focus-trap")
                .AddClass("outline-none")
                .AddClass(Class)
                .Build();

        protected ElementReference _firstBumper;
        protected ElementReference _lastBumper;
        protected ElementReference _fallback;
        protected ElementReference _root;

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

            if (firstRender)
            {
                await SaveFocusAsync();
            }

            if (!_initialized)
            {
                await InitializeFocusAsync();
            }
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
            HandleKeyEvent(args);
        }

        private void OnRootKeyUp(KeyboardEventArgs args)
        {
            HandleKeyEvent(args);
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

        private void HandleKeyEvent(KeyboardEventArgs args)
        {
            _shouldRender = false;
            if (args.Key == "Tab")
            {
                _shiftDown = args.ShiftKey;
            }
        }

        private Task RestoreFocusAsync()
        {
            return _root.MudRestoreFocusAsync().AsTask();
        }

        private Task SaveFocusAsync()
        {
            return _root.MudSaveFocusAsync().AsTask();
        }

        protected override bool ShouldRender()
        {
            if (_shouldRender)
            {
                return true;
            }

            _shouldRender = true; // auto-reset _shouldRender to true

            return false;
        }

        /// <summary>
        /// Releases resources used by this focus trap.
        /// </summary>
        public void Dispose()
        {
            if (!_disabled)
            {
                RestoreFocusAsync().CatchAndLog(ignoreExceptions: true);
            }
        }
    }
}
