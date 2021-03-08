using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTabs : MudComponentBase, IDisposable
    {
        private bool _isDisposed;
        private bool _prevButtonDisabled;
        private bool _nextButtonDisabled;
        private bool _showScrollButtons;
        public ElementReference TabsContentSize;
        private double _size;
        private double _position;
        private double _toolbarContentSize;
        private double _allTabsSize;
        private double _scrollValue;
        private double _scrollPosition;

        [Inject] public IResizeListenerService ResizeListener { get; set; }

        protected string TabsClassnames =>
            new CssBuilder("mud-tabs")
            .AddClass($"mud-tabs-rounded", ApplyEffectsToContainer && Rounded)
            .AddClass($"mud-paper-outlined", ApplyEffectsToContainer && Outlined)
            .AddClass($"mud-elevation-{Elevation}", ApplyEffectsToContainer && Elevation != 0)
            .AddClass($"mud-tabs-reverse", Position == Position.Bottom)
            .AddClass($"mud-tabs-vertical", Position == Position.Left || Position == Position.Right)
            .AddClass($"mud-tabs-vertical-reverse", Position == Position.Right)
            .AddClass(Class)
            .Build();

        protected string ToolbarClassnames =>
            new CssBuilder("mud-tabs-toolbar")
            .AddClass($"mud-tabs-rounded", !ApplyEffectsToContainer && Rounded)
            .AddClass($"mud-tabs-vertical", Position == Position.Left || Position == Position.Right)
            .AddClass($"mud-tabs-toolbar-{Color.ToDescriptionString()}", Color != Color.Default)
            .AddClass($"mud-tabs-border-{Position.ToDescriptionString()}", Border)
            .AddClass($"mud-paper-outlined", !ApplyEffectsToContainer && Outlined)
            .AddClass($"mud-elevation-{Elevation}", !ApplyEffectsToContainer && Elevation != 0)
            .Build();

        protected string WrapperClassnames =>
            new CssBuilder("mud-tabs-toolbar-wrapper")
            .AddClass($"mud-tabs-centered", Centered)
            .AddClass($"mud-tabs-vertical", Position == Position.Left || Position == Position.Right)
            .Build();

        protected string WrapperScrollStyle =>
        new StyleBuilder()
            .AddStyle("transform", $"translateX({_scrollPosition.ToString(CultureInfo.InvariantCulture)}px)", Position == Position.Top || Position == Position.Bottom)
            .AddStyle("transform", $"translateY({_scrollPosition.ToString(CultureInfo.InvariantCulture)}px)", Position == Position.Left || Position == Position.Right)
        .Build();

        protected string PanelsClassnames =>
            new CssBuilder("mud-tabs-panels")
            .AddClass($"mud-tabs-vertical", Position == Position.Left || Position == Position.Right)
            .AddClass(PanelClass)
            .Build();

        protected string SliderClass =>
            new CssBuilder("mud-tab-slider")
            .AddClass($"mud-{Color.ToDescriptionString()}", SliderColor != Color.Inherit)
            .AddClass($"mud-tab-slider-horizontal", Position == Position.Top || Position == Position.Bottom)
            .AddClass($"mud-tab-slider-vertical", Position == Position.Left || Position == Position.Right)
            .AddClass($"mud-tab-slider-horizontal-reverse", Position == Position.Bottom)
            .AddClass($"mud-tab-slider-vertical-reverse", Position == Position.Right)
            .Build();

        protected string MaxHeightStyles =>
            new StyleBuilder()
            .AddStyle("max-height", $"{MaxHeight}px", MaxHeight != null)
            .Build();

        protected string SliderStyle =>
        new StyleBuilder()
            .AddStyle("width", $"{_size.ToString(CultureInfo.InvariantCulture)}px", Position == Position.Top || Position == Position.Bottom)
            .AddStyle("left", $"{_position.ToString(CultureInfo.InvariantCulture)}px", Position == Position.Top || Position == Position.Bottom)
            .AddStyle("height", $"{_size.ToString(CultureInfo.InvariantCulture)}px", Position == Position.Left || Position == Position.Right)
            .AddStyle("top", $"{_position.ToString(CultureInfo.InvariantCulture)}px", Position == Position.Left || Position == Position.Right)
        .Build();

        /// <summary>
        /// If true, render all tabs and hide (display:none) every non-active.
        /// </summary>
        [Parameter] public bool KeepPanelsAlive { get; set; } = false;

        /// <summary>
        /// If true, sets the border-radius to theme default.
        /// </summary>
        [Parameter] public bool Rounded { get; set; }

        /// <summary>
        /// If true, sets a border betwen the content and the toolbar depending on the position.
        /// </summary>
        [Parameter] public bool Border { get; set; }

        /// <summary>
        /// If true, toolbar will be outlined.
        /// </summary>
        [Parameter] public bool Outlined { get; set; }

        /// <summary>
        /// If true, centers the tabitems.
        /// </summary>
        [Parameter] public bool Centered { get; set; }

        /// <summary>
        /// Hides the active tab slider.
        /// </summary>
        [Parameter] public bool HideSlider { get; set; }

        /// <summary>
        /// Icon to use for left pagination.
        /// </summary>
        [Parameter] public string PrevIcon { get; set; } = Icons.Filled.ChevronLeft;

        /// <summary>
        /// Icon to use for right pagination.
        /// </summary>
        [Parameter] public string NextIcon { get; set; } = Icons.Filled.ChevronRight;

        /// <summary>
        /// If true, always display the scroll buttons even if the tabs are smaller than the required with, buttons will be disabled if there is nothing to scroll.
        /// </summary>
        [Parameter] public bool AlwaysShowScrollButtons { get; set; }

        /// <summary>
        /// Sets the maxheight the component can have.
        /// </summary>
        [Parameter] public int? MaxHeight { get; set; } = null;

        /// <summary>
        /// Sets the position of the tabs itself.
        /// </summary>
        [Parameter] public Position Position { get; set; } = Position.Top;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The color of the tab slider. It supports the theme colors.
        /// </summary>
        [Parameter] public Color SliderColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter] public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The color of the next/prev icons. It supports the theme colors.
        /// </summary>
        [Parameter] public Color ScrollIconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The higher the number, the heavier the drop-shadow, applies around the whole component.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 0;

        /// <summary>
        /// If true, will apply elevation, rounded, outlined effects to the whole tab component instead of just toolbar.
        /// </summary>
        [Parameter] public bool ApplyEffectsToContainer { get; set; }


        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Custom class/classes for TabPanel
        /// </summary>
        [Parameter] public string TabPanelClass { get; set; }

        /// <summary>
        /// Custom class/classes for Selected Content Panel
        /// </summary>
        [Parameter] public string PanelClass { get; set; }

        public MudTabPanel ActivePanel { get; set; }

        private int _activePanelIndex = 0;

        /// <summary>
        /// The current active panel index. Also with Bidirectional Binding
        /// </summary>
        [Parameter]
        public int ActivePanelIndex
        {
            get => _activePanelIndex;
            set
            {
                if (_activePanelIndex != value)
                {
                    _activePanelIndex = value;
                    ActivatePanel(_activePanelIndex);
                    ActivePanelIndexChanged.InvokeAsync(value);
                }
            }
        }

        /// <summary>
        /// Fired when ActivePanelIndex changes.
        /// </summary>
        [Parameter]
        public EventCallback<int> ActivePanelIndexChanged { get; set; }

        public List<MudTabPanel> Panels = new List<MudTabPanel>();

        public void Dispose()
        {
            _isDisposed = true;
            ResizeListener.OnResized -= OnResized;
        }

        internal void AddPanel(MudTabPanel tabPanel)
        {
            Panels.Add(tabPanel);
            if (Panels.Count == 1)
                ActivePanel = tabPanel;
            StateHasChanged();
        }

        internal void RemovePanel(MudTabPanel tabPanel)
        {
            if (_isDisposed)
                return;

            var index = Panels.IndexOf(tabPanel);
            if (ActivePanelIndex == index && index == Panels.Count - 1)
            {
                ActivePanelIndex = index > 0 ? index - 1 : 0;
                if (Panels.Count == 1)
                    ActivePanel = null;
            }
            Panels.Remove(tabPanel);
            StateHasChanged();
        }

        string GetTabClass(MudTabPanel panel)
        {
            var tabClass = new CssBuilder("mud-tab")
              .AddClass($"mud-tab-active", when: () => panel == ActivePanel)
              .AddClass($"mud-disabled", panel.Disabled)
              .AddClass($"mud-ripple", !DisableRipple)
              .AddClass(TabPanelClass)
              .AddClass(panel.Class)
            .Build();

            return tabClass;
        }

        string GetTabStyle(MudTabPanel panel)
        {
            var tabStyle = new StyleBuilder()
            .AddStyle(panel.Style)
            .Build();

            return tabStyle;
        }
        void ActivatePanel(MudTabPanel panel, MouseEventArgs ev, bool scrollToActivePanel)
        {
            if (!panel.Disabled)
            {
                ActivePanel = panel;
                _activePanelIndex = Panels.IndexOf(panel);
                ActivePanelIndexChanged.InvokeAsync(_activePanelIndex);

                _ = UpdateSlider(scrollToActivePanel);
                if (ev != null)
                    ActivePanel.OnClick.InvokeAsync(ev);
            }
        }

        public void ActivatePanel(MudTabPanel panel)
        {
            ActivatePanel(panel, null, _showScrollButtons);
        }

        public void ActivatePanel(int index)
        {
            var panel = Panels[index];
            ActivatePanel(panel, null, _showScrollButtons);
        }

        public void ActivatePanel(object id)
        {
            var panel = Panels.Where((p) => p.ID == id).FirstOrDefault();
            if (panel != null)
                ActivatePanel(panel, null, _showScrollButtons);
        }

        private Placement GetTooltipPlacement()
        {
            if (Position == Position.Right)
                return Placement.Start;
            else if (Position == Position.Left)
                return Placement.End;
            else if (Position == Position.Bottom)
                return Placement.Top;
            else
                return Placement.Bottom;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CalculateTabsSize();
                ResizeListener.OnResized += OnResized;
            }
        }

        private async void OnResized(object sender, BrowserWindowSize size)
        {
            await CalculateTabsSize();
            await InvokeAsync(StateHasChanged);
        }

        private async Task CalculateTabsSize()
        {
            await UpdateSlider(true);

            await GetToolbarContentSize();
            await GetAllTabsSize();

            if (AlwaysShowScrollButtons || _allTabsSize > _toolbarContentSize)
            {
                _showScrollButtons = true;
                if (_scrollValue >= 0)
                    _prevButtonDisabled = true;
                else
                    _prevButtonDisabled = false;
                if (Math.Abs(_scrollValue) + _toolbarContentSize >= _allTabsSize)
                    _nextButtonDisabled = true;
                else
                    _nextButtonDisabled = false;
            }
            else
            {
                _scrollPosition = 0;
                _showScrollButtons = false;
            }
            StateHasChanged();
        }

        private async Task UpdateSlider(bool scrollToActivePanel)
        {
            if (HideSlider && !scrollToActivePanel)
                return;

            if (ActivePanel != null)
            {
                if (Position == Position.Top || Position == Position.Bottom)
                    _size = (await ActivePanel.PanelRef.MudGetBoundingClientRectAsync())?.Width ?? 0;
                else
                    _size = (await ActivePanel.PanelRef.MudGetBoundingClientRectAsync())?.Height ?? 0;

                _position = 0;

                if (ActivePanelIndex != 0)
                {
                    double position = 0;
                    var counter = 0;
                    foreach (var panel in Panels) if (counter < ActivePanelIndex)
                        {
                            if (Position == Position.Top || Position == Position.Bottom)
                                position += (await panel.PanelRef.MudGetBoundingClientRectAsync())?.Width ?? 0;
                            else
                                position += (await panel.PanelRef.MudGetBoundingClientRectAsync())?.Height ?? 0;
                            counter++;
                        }
                    _position = position;
                    if (scrollToActivePanel)
                    {
                        _scrollPosition = -position;
                    }
                }
                else if (scrollToActivePanel)
                {
                    _scrollPosition = 0;
                }
                StateHasChanged();
            }
        }

        private async Task GetToolbarContentSize()
        {
            if (Position == Position.Top || Position == Position.Bottom)
                _toolbarContentSize = (await TabsContentSize.MudGetBoundingClientRectAsync())?.Width ?? 0;
            else
                _toolbarContentSize = (await TabsContentSize.MudGetBoundingClientRectAsync())?.Height ?? 0;
        }

        private async Task GetAllTabsSize()
        {
            double totalTabsSize = 0;

            foreach (var panel in Panels)
            {
                if (Position == Position.Top || Position == Position.Bottom)
                    totalTabsSize += (await panel.PanelRef.MudGetBoundingClientRectAsync())?.Width ?? 0;
                else
                    totalTabsSize += (await panel.PanelRef.MudGetBoundingClientRectAsync())?.Height ?? 0;
            }

            _allTabsSize = totalTabsSize;
        }

        private void ScrollPrev()
        {
            _nextButtonDisabled = false;
            _scrollValue += _toolbarContentSize;
            if (_scrollValue >= 0)
            {
                _scrollPosition = 0;
                _prevButtonDisabled = true;
            }
            else
            {
                _scrollPosition = _scrollValue;
            }
        }

        private void ScrollNext()
        {
            _prevButtonDisabled = false;
            _scrollValue -= _toolbarContentSize;
            if (Math.Abs(_scrollValue) + _toolbarContentSize >= _allTabsSize)
            {
                _nextButtonDisabled = true;
                _scrollPosition = _toolbarContentSize - _allTabsSize - 96;
            }
            else
                _scrollPosition = _scrollValue;
        }
    }
}
