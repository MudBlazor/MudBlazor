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
        private Breakpoint _breakpoint;
        private ElementReference _contentRef;
        private DrawerClipMode _clipMode;
        private bool? _isOpenWhenLarge = null;
        private bool _open, _rtl, _isRendered, _initial = true;

        private bool OverlayVisible => _open && !DisableOverlay &&
            (Variant == DrawerVariant.Temporary || 
            (_breakpoint < Breakpoint && Variant == DrawerVariant.Responsive));
        
        protected string Classname =>
        new CssBuilder("mud-drawer")
          .AddClass($"mud-drawer-fixed", IsFixed)
          .AddClass($"mud-drawer-anchor-{Anchor.ToDescriptionString()}")
          .AddClass($"mud-drawer--open", Open)
          .AddClass($"mud-drawer--closed", !Open)
          .AddClass($"mud-drawer--initial", _initial)
          .AddClass($"mud-drawer-{Breakpoint.ToDescriptionString()}")
          .AddClass($"mud-drawer-clipped-{_clipMode.ToDescriptionString()}")
          .AddClass($"mud-drawer-color-{Color.ToDescriptionString()}", Color != Color.Default)
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
            .AddStyle("--mud-drawer-content-height", $"{_height}px", Anchor == Anchor.Bottom || Anchor == Anchor.Top)
            .AddStyle(Style)
        .Build();

        [Inject] public IDomService DomService { get; set; }

        [Inject] public IResizeListenerService ResizeListener { get; set; }

        [CascadingParameter] MudLayout Layout { get; set; }

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
                    this.Anchor = this.Anchor == Anchor.Left ? Anchor.Right : Anchor.Left;
                }
            }
        }
        
        /// <summary>
        /// If true, drawer will be fixed.
        /// </summary>
        [Parameter] public bool IsFixed { get; set; } = true;

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
        /// Variant of the drawer. It affects how the component behaves on different screen sizes.
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
        /// Switching point for responsive drawers
        /// </summary>
        [Parameter] public Breakpoint Breakpoint { get; set; } = Breakpoint.Lg;

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
                if (_isRendered && _initial)
                {
                    _initial = false;
                }
                if (_isRendered && value && (Anchor == Anchor.Top || Anchor == Anchor.Bottom))
                {
                    _ = UpdateHeight();
                }
                if (Layout != null && IsFixed)
                {
                    Layout.FireDrawersChanged();
                }
                OpenChanged.InvokeAsync(_open);
            }
        }

        [Parameter] public EventCallback<bool> OpenChanged { get; set; }

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
                if (IsFixed)
                {
                    Layout?.FireDrawersChanged();
                }
                StateHasChanged();
            }
        }

        protected override void OnInitialized()
        {
            if (IsFixed)
            {
                Layout?.DrawerContainer?.Add(this);
            }
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isRendered = true;
                await UpdateHeight();
                ResizeListener.OnBreakpointChanged += ResizeListener_OnBreakpointChanged;

                if (await ResizeListener.GetBreakpoint() < Breakpoint)
                {
                    await OpenChanged.InvokeAsync(false);
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            Layout?.DrawerContainer?.Remove(this);
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
            _breakpoint = breakpoint;
            if (_breakpoint < Breakpoint && Variant == DrawerVariant.Responsive)
            {
                _isOpenWhenLarge = Open;

                OpenChanged.InvokeAsync(false);
                StateHasChanged();
            }
            else if (_breakpoint >= Breakpoint && _isOpenWhenLarge != null && Variant == DrawerVariant.Responsive)
            {
                OpenChanged.InvokeAsync(_isOpenWhenLarge.Value);
                StateHasChanged();
                _isOpenWhenLarge = null;
            }
        }

        private async Task UpdateHeight()
        {
            _height = (await DomService.GetBoundingClientRect(_contentRef)).Height;
        }
    }
}
