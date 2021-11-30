﻿using System;
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
    public partial class MudTabs : MudComponentBase, IAsyncDisposable
    {
        private bool _isDisposed;
        private int _activePanelIndex = 0;
        private bool _isRendered = false;
        private bool _prevButtonDisabled;
        private bool _nextButtonDisabled;
        private bool _showScrollButtons;
        private bool _disableSliderAnimation;
        private ElementReference _tabsContentSize;
        private double _size;
        private double _position;
        private double _toolbarContentSize;
        private double _allTabsSize;
        private double _scrollPosition;


        [CascadingParameter] public bool RightToLeft { get; set; }

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
        /// If true, sets a border between the content and the toolbar depending on the position.
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
        /// If true, disables slider animation
        /// </summary>
        [Parameter] public bool DisableSliderAnimation { get => _disableSliderAnimation; set => _disableSliderAnimation = value; }

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
                    if (_isRendered)
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

        /// <summary>
        /// A readonly list of the current panels. Panels should be added or removed through the RenderTree use this collection to get informations about the current panels
        /// </summary>
        public IReadOnlyList<MudTabPanel> Panels { get; private set; }

        private List<MudTabPanel> _panels;

        /// <summary>
        /// A render fragment that is added before or after (based on the value of HeaderPosition) the tabs inside the header panel of the tab control
        /// </summary>
        [Parameter]
        public RenderFragment<MudTabs> Header { get; set; }

        /// <summary>
        /// Additional content specified by Header is placed either before the tabs, after or not at all
        /// </summary>
        [Parameter]
        public TabHeaderPosition HeaderPosition { get; set; } = TabHeaderPosition.After;

        /// <summary>
        /// A render fragment that is added before or after (based on the value of HeaderPosition) inside each tab panel
        /// </summary>
        [Parameter]
        public RenderFragment<MudTabPanel> TabPanelHeader { get; set; }

        /// <summary>
        /// Additional content specified by Header is placed either before the tabs, after or not at all
        /// </summary>
        [Parameter]
        public TabHeaderPosition TabPanelHeaderPosition { get; set; } = TabHeaderPosition.After;

        /// <summary>
        /// Can be used in derived class to add a class to the main container. If not overwritten return an empty string
        /// </summary>
        protected virtual string InternalClassName { get; } = string.Empty;

        private string _prevIcon;

        private string _nextIcon;

        #region Life cycle management

        public MudTabs()
        {
            _panels = new List<MudTabPanel>();
            Panels = _panels.AsReadOnly();
        }

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

                if (_panels.Count > 0)
                    ActivePanel = _panels[_activePanelIndex];

                await _resizeObserver.Observe(items);

                _resizeObserver.OnResized += OnResized;

                Rerender();
                StateHasChanged();

                _isRendered = true;
            }
        }


        public async ValueTask DisposeAsync()
        {
            if (_isDisposed == true)
                return;
            _isDisposed = true;
            _resizeObserver.OnResized -= OnResized;
            await _resizeObserver.DisposeAsync();
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

        public void ActivatePanel(MudTabPanel panel, bool ignoreDisabledState = false)
        {
            ActivatePanel(panel, null, ignoreDisabledState);
        }

        public void ActivatePanel(int index, bool ignoreDisabledState = false)
        {
            var panel = _panels[index];
            ActivatePanel(panel, null, ignoreDisabledState);
        }

        public void ActivatePanel(object id, bool ignoreDisabledState = false)
        {
            var panel = _panels.Where((p) => Equals(p.ID, id)).FirstOrDefault();
            if (panel != null)
                ActivatePanel(panel, null, ignoreDisabledState);
        }

        private void ActivatePanel(MudTabPanel panel, MouseEventArgs ev, bool ignoreDisabledState = false)
        {
            if (!panel.Disabled || ignoreDisabledState)
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
            .AddClass($"mud-tabs-vertical", IsVerticalTabs())
            .AddClass($"mud-tabs-vertical-reverse", Position == Position.Right && !RightToLeft || (Position == Position.Left) && RightToLeft || Position == Position.End)
            .AddClass(InternalClassName)
            .AddClass(Class)
            .Build();

        protected string ToolbarClassnames =>
            new CssBuilder("mud-tabs-toolbar")
            .AddClass($"mud-tabs-rounded", !ApplyEffectsToContainer && Rounded)
            .AddClass($"mud-tabs-vertical", IsVerticalTabs())
            .AddClass($"mud-tabs-toolbar-{Color.ToDescriptionString()}", Color != Color.Default)
            .AddClass($"mud-tabs-border-{ConvertPosition(Position).ToDescriptionString()}", Border)
            .AddClass($"mud-paper-outlined", !ApplyEffectsToContainer && Outlined)
            .AddClass($"mud-elevation-{Elevation}", !ApplyEffectsToContainer && Elevation != 0)
            .Build();

        protected string WrapperClassnames =>
            new CssBuilder("mud-tabs-toolbar-wrapper")
            .AddClass($"mud-tabs-centered", Centered)
            .AddClass($"mud-tabs-vertical", IsVerticalTabs())
            .Build();

        protected string WrapperScrollStyle =>
        new StyleBuilder()
            .AddStyle("transform", $"translateX({ (-1 * _scrollPosition).ToString(CultureInfo.InvariantCulture)}px)", Position is Position.Top or Position.Bottom)
            .AddStyle("transform", $"translateY({ (-1 * _scrollPosition).ToString(CultureInfo.InvariantCulture)}px)", IsVerticalTabs())
            .Build();

        protected string PanelsClassnames =>
            new CssBuilder("mud-tabs-panels")
            .AddClass($"mud-tabs-vertical", IsVerticalTabs())
            .AddClass(PanelClass)
            .Build();

        protected string SliderClass =>
            new CssBuilder("mud-tab-slider")
            .AddClass($"mud-{SliderColor.ToDescriptionString()}", SliderColor != Color.Inherit)
            .AddClass($"mud-tab-slider-horizontal", Position is Position.Top or Position.Bottom)
            .AddClass($"mud-tab-slider-vertical", IsVerticalTabs())
            .AddClass($"mud-tab-slider-horizontal-reverse", Position == Position.Bottom)
            .AddClass($"mud-tab-slider-vertical-reverse", Position == Position.Right || Position == Position.Start && RightToLeft || Position == Position.End && !RightToLeft)
            .Build();

        protected string MaxHeightStyles =>
            new StyleBuilder()
            .AddStyle("max-height", MaxHeight.ToPx(), MaxHeight != null)
            .Build();

        protected string SliderStyle => RightToLeft ?
            new StyleBuilder()
            .AddStyle("width", _size.ToPx(), Position is Position.Top or Position.Bottom)
            .AddStyle("right", _position.ToPx(), Position is Position.Top or Position.Bottom)
            .AddStyle("transition", _disableSliderAnimation ? "none" : "right .3s cubic-bezier(.64,.09,.08,1);", Position is Position.Top or Position.Bottom)
            .AddStyle("transition", _disableSliderAnimation ? "none" : "top .3s cubic-bezier(.64,.09,.08,1);", IsVerticalTabs())
            .AddStyle("height", _size.ToPx(), IsVerticalTabs())
            .AddStyle("top", _position.ToPx(), IsVerticalTabs())
            .Build() : new StyleBuilder()
            .AddStyle("width", _size.ToPx(), Position is Position.Top or Position.Bottom)
            .AddStyle("left", _position.ToPx(), Position is Position.Top or Position.Bottom)
            .AddStyle("transition", _disableSliderAnimation ? "none" : "left .3s cubic-bezier(.64,.09,.08,1);", Position is Position.Top or Position.Bottom)
            .AddStyle("transition", _disableSliderAnimation ? "none" : "top .3s cubic-bezier(.64,.09,.08,1);", IsVerticalTabs())
            .AddStyle("height", _size.ToPx(), IsVerticalTabs())
            .AddStyle("top", _position.ToPx(), IsVerticalTabs())
            .Build();

        private bool IsVerticalTabs()
        {
            return Position is Position.Left or Position.Right or Position.Start or Position.End;
        }

        private Position ConvertPosition(Position position)
        {
            return position switch
            {
                Position.Start => RightToLeft ? Position.Right : Position.Left,
                Position.End => RightToLeft ? Position.Left : Position.Right,
                _ => position
            };
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

        private Placement GetTooltipPlacement()
        {
            if (Position == Position.Right)
                return Placement.Left;
            else if (Position == Position.Left)
                return Placement.Right;
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
            _nextIcon = RightToLeft ? PrevIcon : NextIcon;
            _prevIcon = RightToLeft ? NextIcon : PrevIcon;

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
            var scrollValue = RightToLeft ? _scrollPosition + _toolbarContentSize : _scrollPosition - _toolbarContentSize;

            if (RightToLeft && scrollValue > 0) scrollValue = 0;

            if (!RightToLeft && scrollValue < 0) scrollValue = 0;

            _scrollPosition = scrollValue;

            SetScrollabilityStates();
        }

        private void ScrollNext()
        {
            var scrollValue = RightToLeft ? _scrollPosition - _toolbarContentSize : _scrollPosition + _toolbarContentSize;

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
            _scrollPosition = RightToLeft ? -position : position;
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

            var indexCorrection = 1;
            while (true)
            {
                var panelAfterIndex = _activePanelIndex + indexCorrection;
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
                _nextButtonDisabled = RightToLeft ? (_scrollPosition - _toolbarContentSize) <= -_allTabsSize : (_scrollPosition + _toolbarContentSize) >= _allTabsSize;
                _prevButtonDisabled = RightToLeft ? _scrollPosition >= 0 : _scrollPosition <= 0;
            }
        }

        #endregion
    }
}
