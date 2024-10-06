using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.Services;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a navigation panel docked to the side of the page.
    /// </summary>
    /// <seealso cref="MudDrawerContainer"/>
    /// <seealso cref="MudDrawerHeader"/>
    public partial class MudDrawer : MudComponentBase, INavigationEventReceiver, IBrowserViewportObserver, IDisposable
    {
        private double _height;
        private int _disposeCount;
        private readonly ParameterState<bool> _rtlState;
        private readonly ParameterState<bool> _openState;
        private readonly ParameterState<Breakpoint> _breakpointState;
        private readonly ParameterState<DrawerClipMode> _clipModeState;
        private ElementReference _contentRef;
        private bool _closeOnPointerLeave = false;
        private bool _isRendered;
        private bool _initial = true;
        private bool _keepInitialState;
        private Breakpoint _lastUpdatedBreakpoint = Breakpoint.None;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public MudDrawer()
        {
            using var registerScope = CreateRegisterScope();
            _clipModeState = registerScope.RegisterParameter<DrawerClipMode>(nameof(ClipMode))
                .WithParameter(() => ClipMode)
                .WithChangeHandler(OnClipModeParameterChange);
            _breakpointState = registerScope.RegisterParameter<Breakpoint>(nameof(Breakpoint))
                .WithParameter(() => Breakpoint)
                .WithChangeHandler(OnBreakpointParameterChangedAsync);
            _openState = registerScope.RegisterParameter<bool>(nameof(Open))
                .WithParameter(() => Open)
                .WithEventCallback(() => OpenChanged)
                .WithChangeHandler(OnOpenParameterChangedAsync);
            _rtlState = registerScope.RegisterParameter<bool>(nameof(RightToLeft))
                .WithParameter(() => RightToLeft)
                .WithChangeHandler(OnRightToLeftParameterChanged);
        }

        private bool OverlayVisible => _openState.Value && Overlay && (Variant == DrawerVariant.Temporary || (IsBelowCurrentBreakpoint() && IsResponsiveOrMini()));

        protected string Classname =>
            new CssBuilder("mud-drawer")
                .AddClass($"mud-drawer-fixed", IsFixed)
                .AddClass($"mud-drawer-pos-{GetPosition()}")
                .AddClass($"mud-drawer--open", _openState.Value)
                .AddClass($"mud-drawer--closed", !_openState.Value)
                .AddClass($"mud-drawer--initial", _initial)
                .AddClass($"mud-drawer-{_breakpointState.Value.ToDescriptionString()}")
                .AddClass($"mud-drawer-clipped-{_clipModeState.Value.ToDescriptionString()}")
                .AddClass($"mud-theme-{Color.ToDescriptionString()}", Color != Color.Default)
                .AddClass($"mud-elevation-{Elevation}")
                .AddClass($"mud-drawer-{Variant.ToDescriptionString()}")
                .AddClass(Class)
                .Build();

        protected string OverlayClass =>
            new CssBuilder("mud-drawer-overlay mud-overlay-drawer")
                .AddClass($"mud-drawer-pos-{GetPosition()}")
                .AddClass($"mud-drawer-overlay--open", _openState.Value)
                .AddClass($"mud-drawer-overlay-{Variant.ToDescriptionString()}")
                .AddClass($"mud-drawer-overlay-{_breakpointState.Value.ToDescriptionString()}")
                .AddClass($"mud-drawer-overlay--initial", _initial)
                .Build();

        protected string Stylename =>
            new StyleBuilder()
                .AddStyle("--mud-drawer-width", Width, !string.IsNullOrWhiteSpace(Width) && (!IsFixed || Variant == DrawerVariant.Temporary))
                .AddStyle("height", Height, !string.IsNullOrWhiteSpace(Height))
                .AddStyle("--mud-drawer-content-height", string.IsNullOrWhiteSpace(Height) ? _height.ToPx() : Height, Anchor == Anchor.Bottom || Anchor == Anchor.Top)
                .AddStyle("visibility", "hidden", string.IsNullOrWhiteSpace(Height) && _height == 0 && Anchor is Anchor.Bottom or Anchor.Top)
                .AddStyle(Style)
                .Build();

        [Inject]
        protected IBrowserViewportService BrowserViewportService { get; set; } = null!;

        [CascadingParameter]
        private MudDrawerContainer? DrawerContainer { get; set; }

        [CascadingParameter(Name = "RightToLeft")]
        private bool RightToLeft { get; set; }

        /// <summary>
        /// Shows the drawer in the same position even if the page is scrolled.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public bool Fixed { get; set; } = true;

        /// <summary>
        /// The size of the drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Drawer.Appearance)]
        public int Elevation { set; get; } = 1;

        /// <summary>
        /// The edge of the container that the drawer will appear.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Anchor.Start" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public Anchor Anchor { get; set; } = Anchor.Start;

        /// <summary>
        /// The color of the drawer.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Drawer.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The display variant of this drawer.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DrawerVariant.Responsive"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public DrawerVariant Variant { get; set; } = DrawerVariant.Responsive;

        /// <summary>
        /// The content within this drawer.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// For responsive and temporary drawers, darkens the screen with an overlay when displaying this drawer.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  Applies when <see cref="Variant"/> is <see cref="DrawerVariant.Responsive"/> or <see cref="DrawerVariant.Temporary"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public bool Overlay { get; set; } = true;

        /// <summary>
        /// Sets a value indicating whether the overlay should automatically close when clicked.
        /// </summary>
        /// <remarks>
        /// If the <see cref="Variant"/> is set to <see cref="DrawerVariant.Temporary"/>, an overlay will be displayed. 
        /// When this property is <c>true</c>, clicking on the overlay will close it automatically. 
        /// When this property is <c>false</c>, the overlay will not close automatically.
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public bool OverlayAutoClose { get; set; } = true;

        /// <summary>
        /// For mini drawers, opens this drawer when the pointer hovers over it.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Applies when <see cref="Variant" /> is set to <see cref="DrawerVariant.Mini" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public bool OpenMiniOnHover { get; set; }

        /// <summary>
        /// The browser width at which responsive drawers are hidden.
        /// </summary>
        /// <remarks> 
        /// Defaults to <see cref="Breakpoint.Md"/>.  Supported breakpoints are: 
        /// <list type="bullet"> 
        /// <item><description><see cref="Breakpoint.Xs"/></description></item> 
        /// <item><description><see cref="Breakpoint.Sm"/></description></item> 
        /// <item><description><see cref="Breakpoint.Md"/></description></item> 
        /// <item><description><see cref="Breakpoint.Lg"/></description></item> 
        /// <item><description><see cref="Breakpoint.Xl"/></description></item> 
        /// <item><description><see cref="Breakpoint.Xxl"/></description></item> 
        /// </list> 
        /// Other breakpoint combinations are aliased as follows: 
        /// <list type="bullet"> 
        /// <item><description><see cref="Breakpoint.SmAndDown"/>: Aliases to <see cref="Breakpoint.Sm"/></description></item> 
        /// <item><description><see cref="Breakpoint.MdAndDown"/>: Aliases to <see cref="Breakpoint.Md"/></description></item> 
        /// <item><description><see cref="Breakpoint.LgAndDown"/>: Aliases to <see cref="Breakpoint.Lg"/></description></item> 
        /// <item><description><see cref="Breakpoint.XlAndDown"/>: Aliases to <see cref="Breakpoint.Xl"/></description></item> 
        /// <item><description><see cref="Breakpoint.SmAndUp"/>: Aliases to <see cref="Breakpoint.Sm"/></description></item> 
        /// <item><description><see cref="Breakpoint.MdAndUp"/>: Aliases to <see cref="Breakpoint.Md"/></description></item> 
        /// <item><description><see cref="Breakpoint.LgAndUp"/>: Aliases to <see cref="Breakpoint.Lg"/></description></item> 
        /// <item><description><see cref="Breakpoint.XlAndUp"/>: Aliases to <see cref="Breakpoint.Xl"/></description></item> 
        /// </list> 
        /// Setting the value to <see cref="Breakpoint.None"/> will always close the drawer, while <see cref="Breakpoint.Always"/> will always keep it open. 
        /// </remarks> 
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public Breakpoint Breakpoint { get; set; } = Breakpoint.Md;

        /// <summary>
        /// Displays this drawer.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Raises the <see cref="OpenChanged"/> event upon change.  When bound via <c>@bind-Open</c>, this property is updated when this drawer closes itself.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public bool Open { get; set; }

        /// <summary>
        /// Occurs when the <see cref="Open"/> value has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> OpenChanged { get; set; }

        /// <summary>
        /// For non-fixed or temporary drawers, the width of this drawer.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Values such as <c>300px</c> and <c>30%</c> are supported.  Applies to non-fixed or <see cref="DrawerVariant.Temporary"/> drawers anchored to the left or right.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Drawer.Appearance)]
        public string? Width { get; set; }

        /// <summary>
        /// For mini drawers, the width of this drawer.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Values such as <c>300px</c> and <c>30%</c> are supported. Applies to <see cref="DrawerVariant.Mini"/> drawers achored to the left or right.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Drawer.Appearance)]
        public string? MiniWidth { get; set; }

        /// <summary>
        /// The height of this drawer.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Values such as <c>300px</c> and <c>30%</c> are supported. Applies to drawers achored to the top or bottom.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Drawer.Appearance)]
        public string? Height { get; set; }

        /// <summary>
        /// The position of this drawer when opened, relative to a <see cref="MudAppBar"/> when inside a <see cref="MudLayout"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Drawer.Behavior)]
        public DrawerClipMode ClipMode { get; set; }

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
                await UpdateHeightAsync();
                await BrowserViewportService.SubscribeAsync(this, fireImmediately: true);

                _isRendered = true;
                if (string.IsNullOrWhiteSpace(Height) && Anchor is Anchor.Bottom or Anchor.Top)
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
                        BrowserViewportService.UnsubscribeAsync(this).CatchAndLog();
                    }
                }
            }
        }

        private async Task OnOpenParameterChangedAsync(ParameterChangedEventArgs<bool> arg)
        {
            if (_isRendered && _initial && !_keepInitialState)
            {
                _initial = false;
            }
            if (_keepInitialState)
            {
                _keepInitialState = false;
            }
            if (_isRendered && arg.Value && Anchor is Anchor.Top or Anchor.Bottom)
            {
                await UpdateHeightAsync();
            }

            DrawerContainerUpdate();
        }

        private async Task OnBreakpointParameterChangedAsync(ParameterChangedEventArgs<Breakpoint> arg)
        {
            if (_isRendered)
            {
                await UpdateBreakpointStateAsync(_lastUpdatedBreakpoint);
            }

            DrawerContainerUpdate();
        }

        private void OnClipModeParameterChange()
        {
            if (IsFixed)
            {
                DrawerContainerUpdate();
            }

            StateHasChanged();
        }

        private void OnRightToLeftParameterChanged() => DrawerContainerUpdate();

        private void DrawerContainerUpdate() => (DrawerContainer as IMudStateHasChanged)?.StateHasChanged();

        private Task CloseDrawerAsync()
        {
            if (_openState.Value)
            {
                return _openState.SetValueAsync(false);
            }

            return Task.CompletedTask;
        }

        private async Task UpdateHeightAsync()
        {
            _height = (await _contentRef.MudGetBoundingClientRectAsync())?.Height ?? 0;
        }

        private async Task UpdateBreakpointStateAsync(Breakpoint breakpoint)
        {
            if (breakpoint == Breakpoint.None)
            {
                breakpoint = await BrowserViewportService.GetCurrentBreakpointAsync();
            }

            var isStateChanged = false;
            if (ShouldCloseDrawer(breakpoint))
            {
                await _openState.SetValueAsync(false);
                isStateChanged = true;
            }
            else if (ShouldOpenDrawer(breakpoint))
            {
                await _openState.SetValueAsync(true);
                isStateChanged = true;
            }

            _lastUpdatedBreakpoint = breakpoint;
            if (isStateChanged)
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        private bool IsBelowCurrentBreakpoint() => IsBelowBreakpoint(_lastUpdatedBreakpoint);

        private bool IsBelowBreakpoint(Breakpoint breakpoint) => breakpoint < NormalizeBreakpoint(_breakpointState.Value);

        private bool IsResponsiveOrMini() => Variant is DrawerVariant.Responsive or DrawerVariant.Mini;

        private bool ShouldCloseDrawer(Breakpoint breakpoint) => IsResponsiveOrMini() && (_breakpointState.Value == Breakpoint.None || (IsBelowBreakpoint(breakpoint) && !IsBelowCurrentBreakpoint()));

        private bool ShouldOpenDrawer(Breakpoint breakpoint) => IsResponsiveOrMini() && (_breakpointState.Value == Breakpoint.Always || (!IsBelowBreakpoint(breakpoint) && IsBelowCurrentBreakpoint()));

        internal string GetPosition()
        {
            return Anchor switch
            {
                Anchor.Start => _rtlState.Value ? "right" : "left",
                Anchor.End => _rtlState.Value ? "left" : "right",
                _ => Anchor.ToDescriptionString()
            };
        }

        internal bool IsFixed => Fixed && DrawerContainer is MudLayout;

        private async Task OnPointerEnterAsync()
        {
            if (Variant == DrawerVariant.Mini && !_openState.Value && OpenMiniOnHover)
            {
                _closeOnPointerLeave = true;
                await _openState.SetValueAsync(true);
            }
        }

        private async Task OnPointerLeaveAsync()
        {
            if (Variant == DrawerVariant.Mini && _openState.Value && _closeOnPointerLeave)
            {
                _closeOnPointerLeave = false;
                await _openState.SetValueAsync(false);
            }
        }

        async Task INavigationEventReceiver.OnNavigation()
        {
            if (Variant == DrawerVariant.Temporary ||
                (Variant == DrawerVariant.Responsive && await BrowserViewportService.GetCurrentBreakpointAsync() < _breakpointState.Value))
            {
                await _openState.SetValueAsync(false);
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
                _lastUpdatedBreakpoint = browserViewportEventArgs.Breakpoint;
                if (HandleBreakpointNone())
                {
                    await InitialOpenState(false);
                }
                else if (HandleBreakpointAlways())
                {
                    await InitialOpenState(true);
                }
                else if (HandleBelowBreakpointAndOpenState())
                {
                    await InitialOpenState(false);
                }

                return;
            }

            if (!_isRendered)
            {
                return;
            }

            await InvokeAsync(() => UpdateBreakpointStateAsync(browserViewportEventArgs.Breakpoint));
            return;

            bool HandleBreakpointNone() => _breakpointState.Value == Breakpoint.None;
            bool HandleBreakpointAlways() => _breakpointState.Value == Breakpoint.Always;
            bool HandleBelowBreakpointAndOpenState() => IsBelowBreakpoint(browserViewportEventArgs.Breakpoint) && _openState.Value;
            Task InitialOpenState(bool open)
            {
                _keepInitialState = true;
                return _openState.SetValueAsync(open);
            }
        }

        private static Breakpoint NormalizeBreakpoint(Breakpoint breakpoint)
        {
            // Historically, MudDrawer only functioned with breakpoints like Xs, Sm, Md, Lg, Xl, and Xxl.
            // However, some users may supply additional breakpoints such as SmAndDown, MdAndDown, LgAndDown, XlAndDown, SmAndUp, MdAndUp, LgAndUp, XlAndUp, None, and Always.
            // The IBrowserViewportService provides an IsBreakpointWithinReferenceSizeAsync method that considers these additional breakpoints.
            // However, utilizing it would constitute a breaking change.
            // For instance, users who previously specified Sm would find that their drawer opens only on screens of that size but not on any larger ones, requiring them to switch to SmAndUp.
            // To maintain backward compatibility, we decided to alias SmAndUp and SmAndDown to simply Sm, and similarly for other breakpoints.

            return breakpoint switch
            {
                Breakpoint.SmAndDown or Breakpoint.SmAndUp => Breakpoint.Sm,
                Breakpoint.MdAndDown or Breakpoint.MdAndUp => Breakpoint.Md,
                Breakpoint.LgAndDown or Breakpoint.LgAndUp => Breakpoint.Lg,
                Breakpoint.XlAndUp or Breakpoint.XlAndDown => Breakpoint.Xl,
                _ => breakpoint,
            };
        }
    }
}
