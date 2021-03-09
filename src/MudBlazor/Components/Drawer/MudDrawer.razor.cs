using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudDrawer : MudComponentBase, IDisposable, INavigationEventReceiver
    {
        private double _height;
        private ElementReference _contentRef;
        private DrawerClipMode _clipMode;
        private bool? _isOpenWhenLarge = null;
        private bool _open, _rtl, _isRendered, _initial = true, _keepInitialState, _fixed = true;
        private Breakpoint _breakpoint = Breakpoint.Md, _screenBreakpoint = Breakpoint.None;

        private bool OverlayVisible => _open && !DisableOverlay &&
            (Variant == DrawerVariant.Temporary ||
             (_screenBreakpoint < Breakpoint && Variant == DrawerVariant.Responsive));

        protected string Classname =>
        new CssBuilder("mud-drawer")
          .AddClass($"mud-drawer-fixed", Fixed || Variant == DrawerVariant.Temporary)
          .AddClass($"mud-drawer-anchor-{Anchor.ToDescriptionString()}")
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
          .AddClass($"mud-drawer-anchor-{Anchor.ToDescriptionString()}")
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
                string.IsNullOrWhiteSpace(Height) ? $"{_height}px" : Height,
                Anchor == Anchor.Bottom || Anchor == Anchor.Top)
            .AddStyle(Style)
        .Build();

        [Inject] public IResizeListenerService ResizeListener { get; set; }

        [CascadingParameter] MudDrawerContainer DrawerContainer { get; set; }

        [CascadingParameter]
        bool RightToLeft
        {
            get
            {
                return _rtl;
            }
            set
            {
                if (_rtl != value)
                {
                    _rtl = value;
                    Anchor = Anchor == Anchor.Left ? Anchor.Right : Anchor.Left;
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
        [Parameter] public Anchor Anchor { get; set; }

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
                    UpdateBreakpointState();
                }
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
                if (DrawerContainer != null)
                {
                    DrawerContainer.FireDrawersChanged();
                }
                OpenChanged.InvokeAsync(_open);
            }
        }

        [Parameter] public EventCallback<bool> OpenChanged { get; set; }

        /// <summary>
        /// Width of left/right drawer. Only for non-fixed drawers.
        /// </summary>
        [Parameter] public string Width { get; set; }

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
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await UpdateHeight();
                ResizeListener.OnBreakpointChanged += ResizeListener_OnBreakpointChanged;

                _screenBreakpoint = await ResizeListener.GetBreakpoint();
                if (_screenBreakpoint < Breakpoint && _open)
                {
                    _keepInitialState = true;
                    await OpenChanged.InvokeAsync(false);
                }

                _isRendered = true;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            DrawerContainer?.Remove(this);
            ResizeListener.OnBreakpointChanged -= ResizeListener_OnBreakpointChanged;
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
                (Variant == DrawerVariant.Responsive && await ResizeListener.GetBreakpoint() < Breakpoint))
            {
                await OpenChanged.InvokeAsync(false);
            }
        }

        private void ResizeListener_OnBreakpointChanged(object sender, Breakpoint breakpoint)
        {
            if (!_isRendered)
                return;

            _screenBreakpoint = breakpoint;
            InvokeAsync(() => UpdateBreakpointState());
        }

        private async Task UpdateHeight()
        {
            _height = (await _contentRef.MudGetBoundingClientRectAsync())?.Height ?? 0;
        }

        private async void UpdateBreakpointState()
        {
            if (_screenBreakpoint == Breakpoint.None)
            {
                _screenBreakpoint = await ResizeListener.GetBreakpoint();
            }

            if (_screenBreakpoint < Breakpoint && Variant == DrawerVariant.Responsive)
            {
                _isOpenWhenLarge = Open;

                await OpenChanged.InvokeAsync(false);
                StateHasChanged();
            }
            else if (_screenBreakpoint >= Breakpoint && Variant == DrawerVariant.Responsive)
            {
                if (Open && PreserveOpenState)
                {
                    DrawerContainer?.FireDrawersChanged();
                    StateHasChanged();
                }
                else if (_isOpenWhenLarge != null)
                {
                    await OpenChanged.InvokeAsync(_isOpenWhenLarge.Value);
                    _isOpenWhenLarge = null;
                    StateHasChanged();
                }
            }
        }
    }
}
