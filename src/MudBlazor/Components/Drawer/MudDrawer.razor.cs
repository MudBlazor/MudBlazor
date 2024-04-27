using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudDrawer : MudComponentBase, INavigationEventReceiver, IBrowserViewportObserver, IDisposable
    {
        private double _height;
        private int _disposeCount;
        private DrawerClipMode _clipMode;
        private ElementReference _contentRef;
        private bool? _isOpenWhenLarge = null;
        private bool _closeOnMouseLeave = false;
        private bool _open, _rtl, _isRendered, _initial = true, _keepInitialState, _fixed = true;
        private Breakpoint _breakpoint = Breakpoint.Md, _screenBreakpoint = Breakpoint.None;

        private bool OverlayVisible => _open && Overlay &&
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
                .AddStyle("--mud-drawer-content-height", string.IsNullOrWhiteSpace(Height) ? _height.ToPx() : Height, Anchor == Anchor.Bottom || Anchor == Anchor.Top)
                .AddStyle("visibility", "hidden", string.IsNullOrWhiteSpace(Height) && _height == 0 && (Anchor == Anchor.Bottom || Anchor == Anchor.Top))
                .AddStyle(Style)
                .Build();

        [Inject]
        protected IBrowserViewportService BrowserViewportService { get; set; } = null!;

        [CascadingParameter]
        private MudDrawerContainer? DrawerContainer { get; set; }

        [CascadingParameter(Name = "RightToLeft")]
        private bool RightToLeft
        {
            get => _rtl;
            set
            {
                if (_rtl == value)
                {
                    return;
                }

                _rtl = value;
                (DrawerContainer as IMudStateHasChanged)?.StateHasChanged();
            }
        }

        /// <summary>
        /// If true, drawer position will be fixed. (CSS position: fixed;)
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public bool Fixed
        {
            get => _fixed && DrawerContainer is MudLayout;
            set
            {
                _fixed = value;
            }
        }

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Appearance)]
        public int Elevation { set; get; } = 1;

        /// <summary>
        /// Side from which the drawer will appear.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public Anchor Anchor { get; set; } = Anchor.Start;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Variant of the drawer. It specifies how the drawer will be displayed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public DrawerVariant Variant { get; set; } = DrawerVariant.Responsive;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Show overlay for responsive and temporary drawers.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public bool Overlay { get; set; } = true;

        /// <summary>
        /// Preserve open state for responsive drawer when window resized above <see cref="Breakpoint" />.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public bool PreserveOpenState { get; set; } = false;

        /// <summary>
        /// Open drawer automatically on mouse enter when <see cref="Variant" /> parameter is set to <see cref="DrawerVariant.Mini" />.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public bool OpenMiniOnHover { get; set; }

        /// <summary>
        /// Switching point for responsive drawers
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
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
                    _ = UpdateBreakpointStateAsync(_screenBreakpoint);
                }

                (DrawerContainer as IMudStateHasChanged)?.StateHasChanged();
            }
        }

        /// <summary>
        /// Sets the opened state on the drawer. Can be used with two-way binding to close itself on navigation.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
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

                (DrawerContainer as IMudStateHasChanged)?.StateHasChanged();
                OpenChanged.InvokeAsync(_open);
            }
        }

        [Parameter]
        public EventCallback<bool> OpenChanged { get; set; }

        /// <summary>
        /// Width of left/right drawer. Only for non-fixed drawers.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Appearance)]
        public string? Width { get; set; }

        /// <summary>
        /// Width of left/right drawer. Only for non-fixed drawers.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Appearance)]
        public string? MiniWidth { get; set; }

        /// <summary>
        /// Height of top/bottom temporary drawer
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Appearance)]
        public string? Height { get; set; }

        /// <summary>
        /// Specify how the drawer should behave inside a MudLayout. It affects the position relative to <see cref="MudAppBar"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
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
                    (DrawerContainer as IMudStateHasChanged)?.StateHasChanged();
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
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await UpdateHeight();
                await BrowserViewportService.SubscribeAsync(this, fireImmediately: true);

                _isRendered = true;
                if (string.IsNullOrWhiteSpace(Height) && (Anchor == Anchor.Bottom || Anchor == Anchor.Top))
                {
                    StateHasChanged();
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (Interlocked.Increment(ref _disposeCount) == 1)
            {
                if (disposing)
                {
                    DrawerContainer?.Remove(this);

                    if (IsJSRuntimeAvailable)
                    {
                        BrowserViewportService.UnsubscribeAsync(this).AndForget();
                    }
                }
            }
        }

        private Task CloseDrawerAsync() => Open ? OpenChanged.InvokeAsync(false) : Task.CompletedTask;

        async Task INavigationEventReceiver.OnNavigation()
        {
            if (Variant == DrawerVariant.Temporary ||
                (Variant == DrawerVariant.Responsive && await BrowserViewportService.GetCurrentBreakpointAsync() < Breakpoint))
            {
                await OpenChanged.InvokeAsync(false);
            }
        }

        private async Task UpdateHeight()
        {
            _height = (await _contentRef.MudGetBoundingClientRectAsync())?.Height ?? 0;
        }

        private async Task UpdateBreakpointStateAsync(Breakpoint breakpoint)
        {
            var isStateChanged = false;
            if (breakpoint == Breakpoint.None)
            {
                breakpoint = await BrowserViewportService.GetCurrentBreakpointAsync();
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
                    (DrawerContainer as IMudStateHasChanged)?.StateHasChanged();
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
            return Anchor switch
            {
                Anchor.Start => RightToLeft ? "right" : "left",
                Anchor.End => RightToLeft ? "left" : "right",
                _ => Anchor.ToDescriptionString()
            };
        }

        private async Task OnMouseEnterAsync()
        {
            if (Variant == DrawerVariant.Mini && !Open && OpenMiniOnHover)
            {
                _closeOnMouseLeave = true;
                await OpenChanged.InvokeAsync(true);
            }
        }

        private async Task OnMouseLeaveAsync()
        {
            if (Variant == DrawerVariant.Mini && Open && _closeOnMouseLeave)
            {
                _closeOnMouseLeave = false;
                await OpenChanged.InvokeAsync(false);
            }
        }

        Guid IBrowserViewportObserver.Id { get; } = Guid.NewGuid();

        ResizeOptions IBrowserViewportObserver.ResizeOptions { get; } = new()
        {
            ReportRate = 50,
            NotifyOnBreakpointOnly = false
        };

        async Task IBrowserViewportObserver.NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs browserViewportEventArgs)
        {
            if (browserViewportEventArgs.IsImmediate)
            {
                _screenBreakpoint = browserViewportEventArgs.Breakpoint;
                if (_screenBreakpoint < Breakpoint && _open)
                {
                    _keepInitialState = true;
                    await OpenChanged.InvokeAsync(false);
                }

                return;
            }

            if (!_isRendered)
            {
                return;
            }

            await InvokeAsync(() => UpdateBreakpointStateAsync(browserViewportEventArgs.Breakpoint));
        }
    }
}
