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
        private Boolean _isRendered = false;
        private bool _prevButtonDisabled;
        private bool _nextButtonDisabled;
        private bool _showScrollButtons;
        public ElementReference TabsContentSize;
        private double _size;
        private double _position;
        private double _toolbarContentSize;
        private double _allTabsSize;
        private double _scrollPosition;

        [Inject] public IResizeObserver ResizeObserver { get; set; }

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
                    ActivePanel = Panels[_activePanelIndex];
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

        #region Life cycle management

        protected override void OnParametersSet()
        {
            Rerender();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var items = Panels.Select(x => x.PanelRef).ToList();
                items.Add(TabsContentSize);

                await ResizeObserver.Observe(items);

                ResizeObserver.OnResized += OnResized;

                Rerender();
                await InvokeAsync(StateHasChanged);

                _isRendered = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            _isDisposed = true;
            ResizeObserver.OnResized -= OnResized;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            _isDisposed = true;
            await ResizeObserver.DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Children

        internal async Task AddPanel(MudTabPanel tabPanel)
        {
            Panels.Add(tabPanel);
            if (Panels.Count == 1)
                ActivePanel = tabPanel;

            await InvokeAsync(StateHasChanged);
        }

        internal async Task SetPanelRef(ElementReference reference)
        {
            if (_isRendered == true && ResizeObserver.IsElementObserved(reference) == false)
            {
                await ResizeObserver.Observe(reference);
                Rerender();
                await InvokeAsync(StateHasChanged);
            }
        }

        internal async Task RemovePanel(MudTabPanel tabPanel)
        {
            if (_isDisposed)
                return;

            var index = Panels.IndexOf(tabPanel);
            var newIndex = index;
            if (ActivePanelIndex == index && index == Panels.Count - 1)
            {
                newIndex = index > 0 ? index - 1 : 0;
                if (Panels.Count == 1)
                {
                    ActivePanel = null;
                }
            }
            else if (_activePanelIndex > index)
            {
                _activePanelIndex--;
                await ActivePanelIndexChanged.InvokeAsync(_activePanelIndex);
            }

            if(index != newIndex)
            {
                _activePanelIndex = newIndex;
                ActivePanel = Panels[newIndex];
               await ActivePanelIndexChanged.InvokeAsync(newIndex);
            }

            //Double size = GetRelevantSize(tabPanel.PanelRef);
            //Double positon = GetLengthOfPanelItems(tabPanel);
            //if(_scrollPosition > positon)
            //{
            //    _scrollPosition -= size;
            //}

            Panels.Remove(tabPanel);
            await ResizeObserver.Unobserve(tabPanel.PanelRef);
            Rerender();
            await InvokeAsync(StateHasChanged);
        }

        public async Task ActivatePanel(MudTabPanel panel)
        {
            await ActivatePanel(panel, null, _showScrollButtons);
        }

        public async Task ActivatePanel(int index)
        {
            var panel = Panels[index];
            await ActivatePanel(panel, null, _showScrollButtons);
        }

        public async Task ActivatePanel(object id)
        {
            var panel = Panels.Where((p) => p.ID == id).FirstOrDefault();
            if (panel != null)
                await ActivatePanel(panel, null, _showScrollButtons);
        }

        private async Task ActivatePanel(MudTabPanel panel, MouseEventArgs ev, bool scrollToActivePanel)
        {
            if (!panel.Disabled)
            {
                ActivePanel = panel;
                _activePanelIndex = Panels.IndexOf(panel);
                await ActivePanelIndexChanged.InvokeAsync(_activePanelIndex);

                if (ev != null)
                    await ActivePanel.OnClick.InvokeAsync(ev);

                CenterScrollPositionAroundSelectedItem();
                SetSliderState();
                SetScrollabilityStates();
                await InvokeAsync(StateHasChanged);
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

        string GetTabStyle(MudTabPanel panel)
        {
            var tabStyle = new StyleBuilder()
            .AddStyle(panel.Style)
            .Build();

            return tabStyle;
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

        private void GetToolbarContentSize() => _toolbarContentSize = GetRelevantSize(TabsContentSize);

        private void GetAllTabsSize()
        {
            double totalTabsSize = 0;

            foreach (var panel in Panels)
            {
                totalTabsSize += GetRelevantSize(panel.PanelRef);
            }

            _allTabsSize = totalTabsSize;
        }


        private Double GetRelevantSize(ElementReference reference) => Position switch
        {
            Position.Top or Position.Bottom => ResizeObserver.GetWidth(reference),
            _ => ResizeObserver.GetHeight(reference)
        };

        private Double GetLengthOfPanelItems(MudTabPanel panel)
        {
            Double value = 0.0;
            foreach (var item in Panels)
            {
                if (item == panel)
                {
                    break;
                }

                value += GetRelevantSize(item.PanelRef);
            }

            return value;
        }

        private Double GetPanelLength(MudTabPanel panel) => panel == null ? 0.0 : GetRelevantSize(panel.PanelRef);

        #endregion


        #region scrolling 

        private void SetScrollButtonVisibility()
        {
            _showScrollButtons = AlwaysShowScrollButtons || _allTabsSize > _toolbarContentSize;
        }

        private void ScrollPrev()
        {
            Double scrollValue = _scrollPosition - _toolbarContentSize;
            if (scrollValue < 0)
            {
                scrollValue = 0;
            }

            _scrollPosition = scrollValue;

            SetScrollabilityStates();
        }

        private void ScrollNext()
        {
            Double scrollValue = _scrollPosition + _toolbarContentSize;

            if (scrollValue > _allTabsSize)
            {
                scrollValue = _allTabsSize - _toolbarContentSize - 96;
            }

            _scrollPosition = scrollValue;

            SetScrollabilityStates();
        }

        private void ScrollToItem(MudTabPanel panel)
        {
            Double position = GetLengthOfPanelItems(panel);
            _scrollPosition = position;
        }

        private Boolean IsAfterLastPanelIndex(Int32 index) => index >= Panels.Count;
        private Boolean IsBeforeFirstPanelIndex(Int32 index) => index < 0;

        private void CenterScrollPositionAroundSelectedItem()
        {
            MudTabPanel panelToStart = ActivePanel;
            Double length = GetPanelLength(panelToStart);
            if (length >= _toolbarContentSize)
            {
                ScrollToItem(panelToStart);
                return;
            }

            Int32 indexCorrection = 1;
            while (true)
            {
                Int32 panelAfterIndex = _activePanelIndex + indexCorrection;
                if (IsAfterLastPanelIndex(panelAfterIndex) == false)
                {
                    length += GetPanelLength(Panels[panelAfterIndex]);
                }

                if (length >= _toolbarContentSize)
                {
                    ScrollToItem(panelToStart);
                    break;
                }

                length = _toolbarContentSize - length;

                Int32 panelBeforeindex = _activePanelIndex - indexCorrection;
                if (IsBeforeFirstPanelIndex(panelBeforeindex) == false)
                {
                    length -= GetPanelLength(Panels[panelBeforeindex]);
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
                panelToStart = Panels[_activePanelIndex - indexCorrection];

                indexCorrection++;
            }

            ScrollToItem(panelToStart);

            SetScrollabilityStates();
        }

        private void SetScrollabilityStates()
        {
            Boolean isEnoughSpace = _allTabsSize <= _toolbarContentSize;

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
