using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Interop;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTabs : MudComponentBase, IDisposable, IAsyncDisposable
    {
        private bool _isDisposed;
        private int _activePanelIndex = 0;
        private bool _isRendered = false;
        private bool _prevButtonDisabled;
        private bool _nextButtonDisabled;
        private bool _showScrollButtons;
        private ElementReference _tabsContentSize;
        private double _size;
        private double _position;
        private double _toolbarContentSize;
        private double _allTabsSize;
        private double _scrollPosition;

        [Inject] private IResizeObserver _resizeObserver { get; set; }

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

        public MudTabPanel ActivePanel { get; private set; }

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
                    ActivePanel = _panels[_activePanelIndex];
                    ActivePanelIndexChanged.InvokeAsync(value);
                }
            }
        }

        /// <summary>
        /// Fired when ActivePanelIndex changes.
        /// </summary>
        [Parameter]
        public EventCallback<int> ActivePanelIndexChanged { get; set; }

        private List<MudTabPanel> _panels = new List<MudTabPanel>();

        #region Life cycle management

        protected override void OnParametersSet()
        {
            Rerender();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var items = _panels.Select(x => x.PanelRef).ToList();
                items.Add(_tabsContentSize);

                await _resizeObserver.Observe(items);

                _resizeObserver.OnResized += OnResized;

                Rerender();
                StateHasChanged();

                _isRendered = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            _isDisposed = true;
            _resizeObserver.OnResized -= OnResized;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            _isDisposed = true;
            await _resizeObserver.DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Children

        internal void AddPanel(MudTabPanel tabPanel)
        {
            _panels.Add(tabPanel);
            if (_panels.Count == 1)
                ActivePanel = tabPanel;

            StateHasChanged();
        }

        internal async Task SetPanelRef(ElementReference reference)
        {
            if (_isRendered == true && _resizeObserver.IsElementObserved(reference) == false)
            {
                await _resizeObserver.Observe(reference);
                Rerender();
                StateHasChanged();
            }
        }

        internal async Task RemovePanel(MudTabPanel tabPanel)
        {
            if (_isDisposed)
                return;

            var index = _panels.IndexOf(tabPanel);
            var newIndex = index;
            if (ActivePanelIndex == index && index == _panels.Count - 1)
            {
                newIndex = index > 0 ? index - 1 : 0;
                if (_panels.Count == 1)
                {
                    ActivePanel = null;
                }
            }
            else if (_activePanelIndex > index)
            {
                _activePanelIndex--;
                await ActivePanelIndexChanged.InvokeAsync(_activePanelIndex);
            }

            if (index != newIndex)
            {
                ActivePanelIndex = newIndex;
            }

            _panels.Remove(tabPanel);
            await _resizeObserver.Unobserve(tabPanel.PanelRef);
            Rerender();
            StateHasChanged();
        }

        public void ActivatePanel(MudTabPanel panel)
        {
            ActivatePanel(panel, null, _showScrollButtons);
        }

        public void ActivatePanel(int index)
        {
            var panel = _panels[index];
            ActivatePanel(panel, null, _showScrollButtons);
        }

        public void ActivatePanel(object id)
        {
            var panel = _panels.Where((p) => p.ID == id).FirstOrDefault();
            if (panel != null)
                ActivatePanel(panel, null, _showScrollButtons);
        }

        private void ActivatePanel(MudTabPanel panel, MouseEventArgs ev, bool scrollToActivePanel)
        {
            if (!panel.Disabled)
            {
                ActivePanelIndex = _panels.IndexOf(panel);

                if (ev != null)
                    ActivePanel.OnClick.InvokeAsync(ev);

                CenterScrollPositionAroundSelectedItem();
                SetSliderState();
                SetScrollabilityStates();
                StateHasChanged();
            }
        }

        #endregion

        #region Style and classes

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
            .AddStyle("transform", $"translateX({ (-1 * _scrollPosition).ToString(CultureInfo.InvariantCulture)}px)", Position == Position.Top || Position == Position.Bottom)
            .AddStyle("transform", $"translateY({ (-1 * _scrollPosition).ToString(CultureInfo.InvariantCulture)}px)", Position == Position.Left || Position == Position.Right)
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

        string GetTabStyle(MudTabPanel panel)
        {
            var tabStyle = new StyleBuilder()
            .AddStyle(panel.Style)
            .Build();

            return tabStyle;
        }

        #endregion

        #region Rendering and placement

        private void Rerender()
        {
            GetToolbarContentSize();
            GetAllTabsSize();
            SetScrollButtonVisibility();
            CenterScrollPositionAroundSelectedItem();
            SetSliderState();
            SetScrollabilityStates();
        }

        private async void OnResized(IDictionary<ElementReference, BoundingClientRect> changes)
        {
            Rerender();
            await InvokeAsync(StateHasChanged);
        }

        private void SetSliderState()
        {
            if (ActivePanel == null) { return; }

            _position = GetLengthOfPanelItems(ActivePanel);
            _size = GetRelevantSize(ActivePanel.PanelRef);
        }

        private void GetToolbarContentSize() => _toolbarContentSize = GetRelevantSize(_tabsContentSize);

        private void GetAllTabsSize()
        {
            double totalTabsSize = 0;

            foreach (var panel in _panels)
            {
                totalTabsSize += GetRelevantSize(panel.PanelRef);
            }

            _allTabsSize = totalTabsSize;
        }


        private double GetRelevantSize(ElementReference reference) => Position switch
        {
            Position.Top or Position.Bottom => _resizeObserver.GetWidth(reference),
            _ => _resizeObserver.GetHeight(reference)
        };

        private double GetLengthOfPanelItems(MudTabPanel panel)
        {
            var value = 0.0;
            foreach (var item in _panels)
            {
                if (item == panel)
                {
                    break;
                }

                value += GetRelevantSize(item.PanelRef);
            }

            return value;
        }

        private double GetPanelLength(MudTabPanel panel) => panel == null ? 0.0 : GetRelevantSize(panel.PanelRef);

        #endregion

        #region scrolling 

        private void SetScrollButtonVisibility()
        {
            _showScrollButtons = AlwaysShowScrollButtons || _allTabsSize > _toolbarContentSize;
        }

        private void ScrollPrev()
        {
            var scrollValue = _scrollPosition - _toolbarContentSize;
            if (scrollValue < 0)
            {
                scrollValue = 0;
            }

            _scrollPosition = scrollValue;

            SetScrollabilityStates();
        }

        private void ScrollNext()
        {
            var scrollValue = _scrollPosition + _toolbarContentSize;

            if (scrollValue > _allTabsSize)
            {
                scrollValue = _allTabsSize - _toolbarContentSize - 96;
            }

            _scrollPosition = scrollValue;

            SetScrollabilityStates();
        }

        private void ScrollToItem(MudTabPanel panel)
        {
            var position = GetLengthOfPanelItems(panel);
            _scrollPosition = position;
        }

        private bool IsAfterLastPanelIndex(int index) => index >= _panels.Count;
        private bool IsBeforeFirstPanelIndex(int index) => index < 0;

        private void CenterScrollPositionAroundSelectedItem()
        {
            MudTabPanel panelToStart = ActivePanel;
            var length = GetPanelLength(panelToStart);
            if (length >= _toolbarContentSize)
            {
                ScrollToItem(panelToStart);
                return;
            }

            int indexCorrection = 1;
            while (true)
            {
                int panelAfterIndex = _activePanelIndex + indexCorrection;
                if (IsAfterLastPanelIndex(panelAfterIndex) == false)
                {
                    length += GetPanelLength(_panels[panelAfterIndex]);
                }

                if (length >= _toolbarContentSize)
                {
                    ScrollToItem(panelToStart);
                    break;
                }

                length = _toolbarContentSize - length;

                var panelBeforeindex = _activePanelIndex - indexCorrection;
                if (IsBeforeFirstPanelIndex(panelBeforeindex) == false)
                {
                    length -= GetPanelLength(_panels[panelBeforeindex]);
                }
                else
                {
                    break;
                }

                if (length < 0)
                {
                    ScrollToItem(panelToStart);
                    break;
                }

                length = _toolbarContentSize - length;
                panelToStart = _panels[_activePanelIndex - indexCorrection];

                indexCorrection++;
            }

            ScrollToItem(panelToStart);

            SetScrollabilityStates();
        }

        private void SetScrollabilityStates()
        {
            var isEnoughSpace = _allTabsSize <= _toolbarContentSize;

            if (isEnoughSpace == true)
            {
                _nextButtonDisabled = true;
                _prevButtonDisabled = true;
            }
            else
            {
                _nextButtonDisabled = (_scrollPosition + _toolbarContentSize) >= _allTabsSize;
                _prevButtonDisabled = _scrollPosition <= 0;
            }
        }

        #endregion
    }
}
