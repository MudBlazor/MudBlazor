﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudDrawer : MudComponentBase, IDisposable, INavigationEventReceiver
    {
        private double _height;
        private ElementReference _contentRef, _drawerRef;
        private DrawerClipMode _clipMode;
        private bool? _isOpenWhenLarge = null;
        private int _mouseEnterListenerId, _mouseLeaveListenerId;
        private bool _open, _rtl, _isRendered, _initial = true, _keepInitialState, _fixed = true;
        private Breakpoint _breakpoint = Breakpoint.Md, _screenBreakpoint = Breakpoint.None;
        private DotNetObjectReference<MudDrawer> _dotNetRef;

        private Guid _breakpointListenerSubscriptionId;

        private bool OverlayVisible => _open && !DisableOverlay &&
            (Variant == DrawerVariant.Temporary ||
             (_screenBreakpoint < Breakpoint && Variant == DrawerVariant.Mini) ||
             (_screenBreakpoint < Breakpoint && Variant == DrawerVariant.Responsive));

        protected string Classname =>
        new CssBuilder("mud-drawer")
          .AddClass($"mud-drawer-fixed", Fixed)
          .AddClass($"mud-drawer-pos-{GetPosition()}")
          .AddClass($"mud-drawer--open", Open)
          .AddClass($"mud-drawer--closed", !Open)
          .AddClass($"mud-drawer--initial", _initial)
          .AddClass($"mud-drawer-{Breakpoint.ToDescriptionString()}")
          .AddClass($"mud-drawer-clipped-{_clipMode.ToDescriptionString()}")
          .AddClass($"mud-theme-{Color.ToDescriptionString()}", Color != Color.Default)
          .AddClass($"mud-elevation-{Elevation}")
          .AddClass($"mud-drawer-{Variant.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        protected string OverlayClass =>
        new CssBuilder("mud-drawer-overlay mud-overlay-drawer")
          .AddClass($"mud-drawer-pos-{GetPosition()}")
          .AddClass($"mud-drawer-overlay--open", Open)
          .AddClass($"mud-drawer-overlay-{Variant.ToDescriptionString()}")
          .AddClass($"mud-drawer-overlay-{Breakpoint.ToDescriptionString()}")
          .AddClass($"mud-drawer-overlay--initial", _initial)
        .Build();

        protected string Stylename =>
        new StyleBuilder()
            //.AddStyle("width", Width, !string.IsNullOrWhiteSpace(Width) && !Fixed)
            .AddStyle("--mud-drawer-width", Width, !string.IsNullOrWhiteSpace(Width) && (!Fixed || Variant == DrawerVariant.Temporary))
            .AddStyle("height", Height, !string.IsNullOrWhiteSpace(Height))
            .AddStyle("--mud-drawer-content-height",
                string.IsNullOrWhiteSpace(Height) ? _height.ToPx() : Height,
                Anchor == Anchor.Bottom || Anchor == Anchor.Top)
            .AddStyle("visibility", "hidden", string.IsNullOrWhiteSpace(Height) && _height == 0 && (Anchor == Anchor.Bottom || Anchor == Anchor.Top))
            .AddStyle(Style)
        .Build();

        [Inject] public IBreakpointService Breakpointistener { get; set; }

        [CascadingParameter] MudDrawerContainer DrawerContainer { get; set; }

        [CascadingParameter]
        bool RightToLeft
        {
            get => _rtl;
            set
            {
                if (_rtl != value)
                {
                    _rtl = value;
                    DrawerContainer?.FireDrawersChanged();
                }
            }
        }

        /// <summary>
        /// If true, drawer position will be fixed. (CSS position: fixed;)
        /// </summary>
        [Parameter]
        public bool Fixed
        {
            get => _fixed && DrawerContainer is MudLayout;
            set
            {
                if (_fixed == value)
                    return;
                _fixed = value;
            }
        }

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 1;

        /// <summary>
        /// Side from which the drawer will appear.
        /// </summary>
        [Parameter] public Anchor Anchor { get; set; } = Anchor.Start;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Variant of the drawer. It specifies how the drawer will be displayed.
        /// </summary>
        [Parameter] public DrawerVariant Variant { get; set; } = DrawerVariant.Responsive;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Show overlay for responsive and temporary drawers.
        /// </summary>
        [Parameter] public bool DisableOverlay { get; set; } = false;

        /// <summary>
        /// Preserve open state for responsive drawer when window resized above <see cref="Breakpoint" />.
        /// </summary>
        [Parameter] public bool PreserveOpenState { get; set; } = false;

        /// <summary>
        /// Open drawer automatically on mouse enter when <see cref="Variant" /> parameter is set to <see cref="DrawerVariant.Mini" />.
        /// </summary>
        [Parameter] public bool OpenMiniOnHover { get; set; }

        /// <summary>
        /// Switching point for responsive drawers
        /// </summary>
        [Parameter]
        public Breakpoint Breakpoint
        {
            get => _breakpoint;
            set
            {
                if (value == _breakpoint)
                    return;

                _breakpoint = value;
                if (_isRendered)
                {
                    UpdateBreakpointState(_screenBreakpoint);
                }

                DrawerContainer?.FireDrawersChanged();
            }
        }

        /// <summary>
        /// Sets the opened state on the drawer. Can be used with two-way binding to close itself on navigation.
        /// </summary>
        [Parameter]
        public bool Open
        {
            get => _open;
            set
            {
                if (_open == value)
                {
                    return;
                }
                _open = value;
                if (_isRendered && _initial && !_keepInitialState)
                {
                    _initial = false;
                }
                if (_keepInitialState)
                {
                    _keepInitialState = false;
                }
                if (_isRendered && value && (Anchor == Anchor.Top || Anchor == Anchor.Bottom))
                {
                    _ = UpdateHeight();
                }

                DrawerContainer?.FireDrawersChanged();
                OpenChanged.InvokeAsync(_open);
            }
        }

        [Parameter] public EventCallback<bool> OpenChanged { get; set; }

        /// <summary>
        /// Width of left/right drawer. Only for non-fixed drawers.
        /// </summary>
        [Parameter] public string Width { get; set; }

        /// <summary>
        /// Width of left/right drawer. Only for non-fixed drawers.
        /// </summary>
        [Parameter] public string MiniWidth { get; set; }

        /// <summary>
        /// Height of top/bottom temporary drawer
        /// </summary>
        [Parameter] public string Height { get; set; }

        /// <summary>
        /// Specify how the drawer should behave inside a MudLayout. It affects the position relative to <b>MudAppbar</b>
        /// </summary>
        [Parameter]
        public DrawerClipMode ClipMode
        {
            get => _clipMode;
            set
            {
                if (_clipMode == value)
                {
                    return;
                }
                _clipMode = value;
                if (Fixed)
                {
                    DrawerContainer?.FireDrawersChanged();
                }
                StateHasChanged();
            }
        }

        protected override void OnInitialized()
        {
            if (Variant != DrawerVariant.Temporary)
            {
                DrawerContainer?.Add(this);
            }
            _dotNetRef = DotNetObjectReference.Create(this);
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await UpdateHeight();
                var result = await Breakpointistener.Subscribe(UpdateBreakpointState);
                var currentBreakpoint = result.Breakpoint;

                _breakpointListenerSubscriptionId = result.SubscriptionId;

                _screenBreakpoint = result.Breakpoint;
                if (_screenBreakpoint < Breakpoint && _open)
                {
                    _keepInitialState = true;
                    await OpenChanged.InvokeAsync(false);
                }

                _isRendered = true;
                if (string.IsNullOrWhiteSpace(Height) && (Anchor == Anchor.Bottom || Anchor == Anchor.Top))
                {
                    StateHasChanged();
                }

                _mouseEnterListenerId = await _drawerRef.MudAddEventListenerAsync(_dotNetRef, "mouseenter", nameof(OnMouseEnter), true);
                _mouseLeaveListenerId = await _drawerRef.MudAddEventListenerAsync(_dotNetRef, "mouseleave", nameof(OnMouseLeave), true);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        int _disposeCount;
        public virtual void Dispose(bool disposing)
        {
            if (Interlocked.Increment(ref _disposeCount) == 1)
            {
                if (disposing)
                {
                    DrawerContainer?.Remove(this);

                    if (_mouseEnterListenerId != 0)
                        _ = _drawerRef.MudRemoveEventListenerAsync("mouseenter", _mouseEnterListenerId);
                    if (_mouseLeaveListenerId != 0)
                        _ = _drawerRef.MudRemoveEventListenerAsync("mouseleave", _mouseLeaveListenerId);

                    var toDispose = _dotNetRef;
                    _dotNetRef = null;
                    toDispose?.Dispose();

                    if (_breakpointListenerSubscriptionId != default)
                    {
                        Breakpointistener.Unsubscribe(_breakpointListenerSubscriptionId).AndForget();
                    }
                }
            }
        }

        private void CloseDrawer()
        {
            if (Open)
            {
                OpenChanged.InvokeAsync(false);
            }
        }

        public async Task OnNavigation()
        {
            if (Variant == DrawerVariant.Temporary ||
                (Variant == DrawerVariant.Responsive && await Breakpointistener.GetBreakpoint() < Breakpoint))
            {
                await OpenChanged.InvokeAsync(false);
            }
        }

        private void ResizeListener_OnBreakpointChanged(object sender, Breakpoint breakpoint)
        {
            if (!_isRendered)
                return;

            InvokeAsync(() => UpdateBreakpointState(breakpoint));
        }

        private async Task UpdateHeight()
        {
            _height = (await _contentRef.MudGetBoundingClientRectAsync())?.Height ?? 0;
        }

        private async void UpdateBreakpointState(Breakpoint breakpoint)
        {
            var isStateChanged = false;
            if (breakpoint == Breakpoint.None)
            {
                breakpoint = await Breakpointistener.GetBreakpoint();
            }

            if (breakpoint < Breakpoint && _screenBreakpoint >= Breakpoint && (Variant == DrawerVariant.Responsive || Variant == DrawerVariant.Mini))
            {
                _isOpenWhenLarge = Open;

                await OpenChanged.InvokeAsync(false);
                isStateChanged = true;
            }
            else if (breakpoint >= Breakpoint && _screenBreakpoint < Breakpoint && (Variant == DrawerVariant.Responsive || Variant == DrawerVariant.Mini))
            {
                if (Open && PreserveOpenState)
                {
                    DrawerContainer?.FireDrawersChanged();
                    isStateChanged = true;
                }
                else if (_isOpenWhenLarge != null)
                {
                    await OpenChanged.InvokeAsync(_isOpenWhenLarge.Value);
                    _isOpenWhenLarge = null;
                    isStateChanged = true;
                }
            }

            _screenBreakpoint = breakpoint;
            if (isStateChanged)
            {
                StateHasChanged();
            }
        }

        internal string GetPosition()
        {
            switch (Anchor)
            {
                case Anchor.Start:
                    return RightToLeft ? "right" : "left";
                case Anchor.End:
                    return RightToLeft ? "left" : "right";
                default: break;
            }

            return Anchor.ToDescriptionString();
        }


        private bool closeOnMouseLeave = false;
        [JSInvokable]
        public async void OnMouseEnter()
        {
            if (Variant == DrawerVariant.Mini && !Open && OpenMiniOnHover)
            {
                closeOnMouseLeave = true;
                await OpenChanged.InvokeAsync(true);
            }
        }

        [JSInvokable]
        public async void OnMouseLeave()
        {
            if (Variant == DrawerVariant.Mini && Open && closeOnMouseLeave)
            {
                closeOnMouseLeave = false;
                await OpenChanged.InvokeAsync(false);
            }
        }
    }
}
