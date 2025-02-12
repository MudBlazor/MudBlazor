// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interop;
using MudBlazor.Services;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// A set of views organized into one or more <see cref="MudTabPanel" /> components.
    /// </summary>
    public partial class MudTabs : MudComponentBase, IAsyncDisposable
    {
        private bool _isDisposed;

        private int _activePanelIndex = 0;
        private int _scrollIndex = 0;

        private bool _isRendered = false;
        private bool _prevButtonDisabled;
        private bool _nextButtonDisabled;
        private bool _showScrollButtons;
        private ElementReference _tabsContentSize;
        private double _sliderSize;
        private double _sliderPosition;
        private double _tabBarContentSize;
        private double _allTabsSize;
        private double _scrollPosition;

        private IResizeObserver? _resizeObserver = null;

        /// <summary>
        /// Displays text right-to-left.
        /// </summary>
        /// <remarks>
        /// Controlled via the <see cref="MudRTLProvider"/> component.
        /// </remarks>
        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        [Inject]
        private IResizeObserverFactory _resizeObserverFactory { get; set; } = null!;

        /// <summary>
        /// Persists the content of tabs when they are not visible.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.<br />
        /// When <c>false</c>, selecting a tab will initialize its content each time the tab is visited.<br />
        /// When <c>true</c>, a tab's content is initialized only once and is hidden via <c>display:none</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public bool KeepPanelsAlive { get; set; } = false;

        /// <summary>
        /// Uses rounded corners on the tab's edges.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="MudGlobal.Rounded" />.
        /// When <c>true</c>, the <c>border-radius</c> style is set to the theme's default value.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public bool Rounded { get; set; } = MudGlobal.Rounded == true;

        /// <summary>
        /// Shows a border between the tab content and tab header.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public bool Border { get; set; }

        /// <summary>
        /// Shows an outline around the tab header.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public bool Outlined { get; set; } = false;

        /// <summary>
        /// Centers tabs horizontally in the tab header.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public bool Centered { get; set; }

        /// <summary>
        /// Hides the slider underneath the tab header.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public bool HideSlider { get; set; }

        /// <summary>
        /// The icon for scrolling to the previous page of tabs.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ChevronLeft"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string PrevIcon { get; set; } = Icons.Material.Filled.ChevronLeft;

        /// <summary>
        /// The icon for scrolling to the next page of tabs.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ChevronRight"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string NextIcon { get; set; } = Icons.Material.Filled.ChevronRight;

        /// <summary>
        /// Shows the scroll buttons even if all tabs are visible.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public bool AlwaysShowScrollButtons { get; set; }

        /// <summary>
        /// The maximum height for this component, in pixels.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public int? MaxHeight { get; set; } = null;

        /// <summary>
        /// The minimum width of each tab panel.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>160px</c>. Can be a CSS width or a percentage (e.g. <c>30%</c>).
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string MinimumTabWidth { get; set; } = "160px";

        /// <summary>
        /// The location of the tab header.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Position.Top"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public Position Position { get; set; } = Position.Top;

        /// <summary>
        /// The color of the tab header.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The color of the tab slider.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Inherit" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public Color SliderColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The color of each tab panel's icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Inherit" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The color of the scroll icon buttons.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Inherit" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public Color ScrollIconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The size of the drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>. Use a higher number for a larger drop shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public int Elevation { set; get; } = 0;

        /// <summary>
        /// Applies the <see cref="Elevation"/>, <see cref="Rounded"/> and <see cref="Outlined"/> effects to the tab panel.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.<br />
        /// When <c>false</c>, effects are only applied to the header.<br />
        /// When <c>true</c>, effects are applied to both the tab header and panel.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public bool ApplyEffectsToContainer { get; set; }

        /// <summary>
        /// Shows a ripple effect when a tab is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// Shows an animated line which slides to the selected tab.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public bool SliderAnimation { get; set; } = true;

        /// <summary>
        /// The content within this component.
        /// </summary>
        /// <remarks>
        /// Typically a set of <see cref="MudTabPanel"/> components.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// This fragment is placed between tabHeader and panels. 
        /// It can be used to display additional content like an address line in a browser.
        /// The active tab will be the content of this RenderFragement
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public RenderFragment<MudTabPanel>? PrePanelContent { get; set; }

        /// <summary>
        /// The CSS classes applied to tab panels.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string? TabPanelClass { get; set; }

        /// <summary>
        /// The CSS classes applied to the tab header.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Multiple classes must be separated by spaces.<br />
        /// The "header" is the set of tab names which users click on to change the active tab.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string? TabHeaderClass { get; set; }

        /// <summary>
        /// The CSS classes applied to the currently selected tab.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string? ActiveTabClass { get; set; }

        /// <summary>
        /// The CSS classes applied to all tab panels.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string? PanelClass { get; set; }

        /// <summary>
        /// The currently selected tab panel.
        /// </summary>
        public MudTabPanel? ActivePanel { get; private set; }

        /// <summary>
        /// The index of the currently selected tab panel.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c> (the first tab). When this value changes, <see cref="ActivePanelIndexChanged"/> occurs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public int ActivePanelIndex
        {
            get => _activePanelIndex;
            set
            {
                var validPanel = _panels.Count > 0 && value != -1 && value <= _panels.Count - 1;

                if (_activePanelIndex != value)
                {
                    _activePanelIndex = value;
                    if (_isRendered)
                    {
                        ActivePanel = validPanel ? _panels[value] : null;
                        ActivePanelIndexChanged.InvokeAsync(value);
                    }
                }
                else if (validPanel)
                    ActivePanel = _panels[value];
            }
        }

        /// <summary>
        /// Occurs when <see cref="ActivePanelIndex"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<int> ActivePanelIndexChanged { get; set; }

        /// <summary>
        /// A read-only list of the panels within this component. 
        /// </summary>
        /// <remarks>
        /// Tab panels are controlled by either adding more <see cref="MudTabPanel"/> components in the Razor page, or by using the <see cref="MudDynamicTabs"/> component instead.
        /// </remarks>
        public IReadOnlyList<MudTabPanel> Panels { get; private set; }

        private List<MudTabPanel> _panels;

        /// <summary>
        /// The custom content added before or after the list of tabs.
        /// </summary>
        /// <remarks>
        /// The location of this header is controlled via the <see cref="HeaderPosition"/> parameter.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public RenderFragment<MudTabs>? Header { get; set; }

        /// <summary>
        /// The location of custom header content provided in <see cref="Header"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TabHeaderPosition.After"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public TabHeaderPosition HeaderPosition { get; set; } = TabHeaderPosition.After;

        /// <summary>
        /// Custom content added before or after each tab panel.
        /// </summary>
        /// <remarks>
        /// The location of this header is controlled via the <see cref="TabPanelHeaderPosition"/> parameter.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public RenderFragment<MudTabPanel>? TabPanelHeader { get; set; }

        /// <summary>
        /// The location of custom tab panel content provided in <see cref="TabPanelHeader"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public TabHeaderPosition TabPanelHeaderPosition { get; set; } = TabHeaderPosition.After;

        /// <summary>
        /// Occurs before a panel is activated.
        /// </summary>
        /// <remarks>
        /// Set <see cref="TabInteractionEventArgs.Cancel"/> to <c>true</c> to prevent the tab from being activated.<br />
        /// The returned <c>Task</c> will be awaited.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public Func<TabInteractionEventArgs, Task>? OnPreviewInteraction { get; set; }

        /// <summary>
        /// Can be used in derived class to add a class to the main container. If not overwritten return an empty string
        /// </summary>
        protected virtual string InternalClassName { get; } = string.Empty;

        private string? _prevIcon;

        private string? _nextIcon;

        #region Life cycle management

        public MudTabs()
        {
            _panels = new List<MudTabPanel>();
            Panels = _panels.AsReadOnly();
        }

        protected override void OnInitialized()
        {
            _resizeObserver = _resizeObserverFactory.Create();
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _resizeObserver ??= _resizeObserverFactory.Create();

            Rerender();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var items = _panels.Select(x => x.PanelRef).ToList();
                items.Add(_tabsContentSize);

                if (_activePanelIndex != -1 && _panels.Count > 0)
                    ActivePanel = _panels[_activePanelIndex];

                await _resizeObserver!.Observe(items);

                _resizeObserver.OnResized += OnResized;

                Rerender();
                StateHasChanged();
                ActivatePanel(ActivePanelIndex);

                _isRendered = true;
            }
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
            if (_resizeObserver is not null)
            {
                _resizeObserver.OnResized -= OnResized;
                if (IsJSRuntimeAvailable)
                {
                    await _resizeObserver.DisposeAsync();
                }
            }
        }

        #endregion

        #region Children

        internal void AddPanel(MudTabPanel tabPanel)
        {
            _panels.Add(tabPanel);
            if (_panels.Count == _activePanelIndex + 1 || _activePanelIndex == -1 && _panels.Count == 1)
                ActivePanel = tabPanel;
            StateHasChanged();
        }

        internal async Task SetPanelRef(ElementReference reference)
        {
            if (_isRendered && _resizeObserver!.IsElementObserved(reference) == false)
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

            // We're at the right-most tab.
            if (_activePanelIndex == index && index == _panels.Count - 1)
            {
                if (_panels.Count == 1)
                    ActivePanelIndex = -1;
                else if (index > 0)
                    ActivePanelIndex = index - 1;
                else
                    ActivePanelIndex = 0;
            }

            // Active tab is not necessarily the tab being closed.
            else if (_activePanelIndex > index)
            {
                _activePanelIndex--;
                await ActivePanelIndexChanged.InvokeAsync(_activePanelIndex);
            }

            _panels.Remove(tabPanel);
            await _resizeObserver!.Unobserve(tabPanel.PanelRef);
            Rerender();
            StateHasChanged();
        }

        /// <summary>
        /// Sets the active panel.
        /// </summary>
        /// <param name="panel">The panel to activate.</param>
        /// <param name="ignoreDisabledState">When <c>true</c>, the panel will be activated even if it is disabled.</param>
        public void ActivatePanel(MudTabPanel? panel, bool ignoreDisabledState = false)
        {
            if (panel is not null && _panels.IndexOf(panel) > -1)
                ActivatePanel(panel, null, ignoreDisabledState);
        }

        /// <summary>
        /// Sets the active panel.
        /// </summary>
        /// <param name="index">The index of the panel to activate.</param>
        /// <param name="ignoreDisabledState">When <c>true</c>, the panel will be activated even if it is disabled.</param>
        public void ActivatePanel(int index, bool ignoreDisabledState = false)
        {
            if (index > -1 && index <= _panels.Count - 1)
                ActivatePanel(_panels[index], null, ignoreDisabledState);
        }

        /// <summary>
        /// Sets the active panel.
        /// </summary>
        /// <param name="id">The unique ID of the panel to activate.</param>
        /// <param name="ignoreDisabledState">When <c>true</c>, the panel will be activated even if it is disabled.</param>
        public void ActivatePanel(object id, bool ignoreDisabledState = false)
        {
            var panel = _panels.FirstOrDefault(p => Equals(p.ID, id));
            if (panel != null)
                ActivatePanel(panel, null, ignoreDisabledState);
        }

        private async void ActivatePanel(MudTabPanel panel, MouseEventArgs? ev, bool ignoreDisabledState = false)
        {
            if ((panel.Visible && !panel.Disabled) || ignoreDisabledState)
            {
                var index = _panels.IndexOf(panel);
                var previewArgs = new TabInteractionEventArgs
                {
                    PanelIndex = index,
                    InteractionType = TabInteractionType.Activate
                };

                if (OnPreviewInteraction != null)
                    await OnPreviewInteraction.Invoke(previewArgs);

                if (previewArgs.Cancel) return;

                ActivePanelIndex = previewArgs.PanelIndex;
                if (ActivePanel is not null)
                {
                    await ActivePanel.OnClick.InvokeAsync(ev);
                }

                CenterScrollPositionAroundSelectedItem();
                SetScrollabilityStates();
                SetSliderState();
                SetScrollButtonVisibility();
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

        protected string TabBarClassnames =>
            new CssBuilder("mud-tabs-tabbar")
                .AddClass($"mud-tabs-rounded", !ApplyEffectsToContainer && Rounded)
                .AddClass($"mud-tabs-vertical", IsVerticalTabs())
                .AddClass($"mud-tabs-tabbar-{Color.ToDescriptionString()}", Color != Color.Default)
                .AddClass($"mud-tabs-border-{ConvertPosition(Position).ToDescriptionString()}", Border)
                .AddClass($"mud-paper-outlined", !ApplyEffectsToContainer && Outlined)
                .AddClass($"mud-elevation-{Elevation}", !ApplyEffectsToContainer && Elevation != 0)
                .AddClass(TabHeaderClass)
                .Build();

        protected string WrapperClassnames =>
            new CssBuilder("mud-tabs-tabbar-wrapper")
                .AddClass($"mud-tabs-centered", Centered)
                .AddClass($"mud-tabs-vertical", IsVerticalTabs())
                .Build();

        protected string WrapperScrollStyle =>
            new StyleBuilder()
                .AddStyle("transform", $"translateX({(-1 * _scrollPosition).ToString(CultureInfo.InvariantCulture)}px)", Position is Position.Top or Position.Bottom)
                .AddStyle("transform", $"translateY({(-1 * _scrollPosition).ToString(CultureInfo.InvariantCulture)}px)", IsVerticalTabs())
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

        protected string SliderStyle => RightToLeft
            ? new StyleBuilder()
                .AddStyle("width", _sliderSize.ToPx(), Position is Position.Top or Position.Bottom)
                .AddStyle("right", _sliderPosition.ToPx(), Position is Position.Top or Position.Bottom)
                .AddStyle("transition", SliderAnimation ? "right .3s cubic-bezier(.64,.09,.08,1);" : "none", Position is Position.Top or Position.Bottom)
                .AddStyle("transition", SliderAnimation ? "top .3s cubic-bezier(.64,.09,.08,1);" : "none", IsVerticalTabs())
                .AddStyle("height", _sliderSize.ToPx(), IsVerticalTabs())
                .AddStyle("top", _sliderPosition.ToPx(), IsVerticalTabs())
                .Build()
            : new StyleBuilder()
                .AddStyle("width", _sliderSize.ToPx(), Position is Position.Top or Position.Bottom)
                .AddStyle("left", _sliderPosition.ToPx(), Position is Position.Top or Position.Bottom)
                .AddStyle("transition", SliderAnimation ? "left .3s cubic-bezier(.64,.09,.08,1);" : "none", Position is Position.Top or Position.Bottom)
                .AddStyle("transition", SliderAnimation ? "top .3s cubic-bezier(.64,.09,.08,1);" : "none", IsVerticalTabs())
                .AddStyle("height", _sliderSize.ToPx(), IsVerticalTabs())
                .AddStyle("top", _sliderPosition.ToPx(), IsVerticalTabs())
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
              .AddClass($"mud-ripple", Ripple)
              .AddClass(ActiveTabClass, when: () => panel == ActivePanel)
              .AddClass(TabPanelClass)
              .AddClass(panel.Classname)
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
            .AddStyle("min-width", MinimumTabWidth)
            .AddStyle(panel.Style)
            .Build();

            return tabStyle;
        }

        private Color GetPanelIconColor(MudTabPanel panel)
        {
            var iconColor = panel.Disabled ? Color.Inherit : panel.IconColor != default ? panel.IconColor : IconColor;

            return iconColor;
        }

        #endregion

        #region Rendering and placement

        private void Rerender()
        {
            _nextIcon = RightToLeft ? PrevIcon : NextIcon;
            _prevIcon = RightToLeft ? NextIcon : PrevIcon;

            GetTabBarContentSize();
            GetAllTabsSize();
            SetScrollButtonVisibility();
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
            if (ActivePanel is null)
            {
                return;
            }

            _sliderPosition = GetLengthOfPanelItems(ActivePanel);
            _sliderSize = GetRelevantSize(ActivePanel.PanelRef);
        }

        private bool IsSliderPositionDetermined => _activePanelIndex > 0 && _sliderPosition > 0 ||
                                                   _activePanelIndex <= 0;

        private void GetTabBarContentSize() => _tabBarContentSize = GetRelevantSize(_tabsContentSize);

        private void GetAllTabsSize()
        {
            double totalTabsSize = 0;

            foreach (var panel in _panels)
            {
                totalTabsSize += GetRelevantSize(panel.PanelRef);
            }

            _allTabsSize = totalTabsSize;
        }

        private double GetRelevantSize(ElementReference reference) =>
            Position switch
            {
                Position.Top or Position.Bottom => _resizeObserver!.GetWidth(reference),
                _ => _resizeObserver!.GetHeight(reference)
            };

        private double GetLengthOfPanelItems(MudTabPanel panel, bool inclusive = false)
        {
            var value = 0.0;
            foreach (var item in _panels)
            {
                if (item == panel)
                {
                    if (inclusive)
                    {
                        value += GetRelevantSize(item.PanelRef);
                    }

                    break;
                }

                value += GetRelevantSize(item.PanelRef);
            }

            return value;
        }

        private double GetPanelLength(MudTabPanel? panel) => panel == null ? 0.0 : GetRelevantSize(panel.PanelRef);

        #endregion

        #region scrolling

        private void SetScrollButtonVisibility()
        {
            _showScrollButtons = AlwaysShowScrollButtons || (int)_allTabsSize > (int)_tabBarContentSize || _scrollIndex != 0;
        }

        private void ScrollPrev()
        {
            var scrollAmount = Math.Max(GetVisiblePanels(), 1);
            _scrollIndex = Math.Max(_scrollIndex - scrollAmount, 0);
            ScrollToItem(_panels[_scrollIndex]);
            SetScrollButtonVisibility();
            SetScrollabilityStates();
        }

        private void ScrollNext()
        {
            var scrollAmount = Math.Max(GetVisiblePanels(), 1);
            _scrollIndex = Math.Min(_scrollIndex + scrollAmount, _panels.Count - 1);
            ScrollToItem(_panels[_scrollIndex]);
            SetScrollabilityStates();
        }

        /// <summary>
        /// Calculates the amount of panels that are completely visible inside the toolbar content area. Panels that are just partially visible are not considered here!
        /// </summary>
        /// <returns>The amount of panels visible inside the toolbar area. CAUTION: Might return 0!</returns>
        private int GetVisiblePanels()
        {
            var x = 0D;
            var count = 0;

            var toolbarContentSize = GetRelevantSize(_tabsContentSize);

            foreach (var panel in _panels)
            {
                x += GetRelevantSize(panel.PanelRef);

                if (x < toolbarContentSize)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        private bool ScrollToItem(MudTabPanel panel, bool isLast = false)
        {
            var position = GetLengthOfPanelItems(panel, isLast);
            if (isLast)
            {
                var compare = _tabBarContentSize;
                if (position - compare > 0)
                {
                    if (!AlwaysShowScrollButtons && _showScrollButtons)
                        compare -= 48 * 2;
                    position -= compare;
                }
                else
                    return false;
            }
            _scrollPosition = RightToLeft ? -position : position;
            return true;
        }

        private bool IsAfterLastPanelIndex(int index) => index >= _panels.Count;
        private bool IsBeforeFirstPanelIndex(int index) => index < 0;

        private void CenterScrollPositionAroundSelectedItem()
        {
            if (_showScrollButtons && ActivePanelIndex + 1 == _panels.Count)
            {
                var lastPanel = _panels.Last();
                var isScrolled = ScrollToItem(lastPanel, true);
                if (isScrolled)
                {
                    _scrollIndex = _panels.IndexOf(lastPanel);
                    return;
                }
            }

            if (ActivePanel is null)
            {
                return;
            }

            var panelToStart = ActivePanel;
            var length = GetPanelLength(panelToStart);
            if (length >= _tabBarContentSize)
            {
                _scrollIndex = _panels.IndexOf(panelToStart);
                ScrollToItem(panelToStart);
                return;
            }

            var indexCorrection = 1;
            while (true)
            {
                var panelAfterIndex = _activePanelIndex + indexCorrection;
                if (!IsAfterLastPanelIndex(panelAfterIndex))
                {
                    length += GetPanelLength(_panels[panelAfterIndex]);
                }

                if (length >= _tabBarContentSize)
                {
                    _scrollIndex = _panels.IndexOf(panelToStart);
                    break;
                }

                length = _tabBarContentSize - length;

                var panelBeforeindex = _activePanelIndex - indexCorrection;
                if (!IsBeforeFirstPanelIndex(panelBeforeindex))
                {
                    length -= GetPanelLength(_panels[panelBeforeindex]);
                }
                else
                {
                    break;
                }

                if (length < 0)
                {
                    _scrollIndex = _panels.IndexOf(panelToStart);
                    break;
                }

                length = _tabBarContentSize - length;
                panelToStart = _panels[_activePanelIndex - indexCorrection];

                indexCorrection++;
            }

            _scrollIndex = _panels.IndexOf(panelToStart);
            ScrollToItem(panelToStart);
        }

        private void SetScrollabilityStates()
        {
            var isEnoughSpace = _allTabsSize <= _tabBarContentSize;

            if (isEnoughSpace)
            {
                _nextButtonDisabled = true;
                _prevButtonDisabled = _scrollIndex == 0;
            }
            else
            {
                // Disable next button if the last panel is completely visible
                _nextButtonDisabled = _scrollIndex == _panels.Count - 1 || Math.Abs(_scrollPosition) >= GetLengthOfPanelItems(_panels.Last(), true) - _tabBarContentSize;
                _prevButtonDisabled = _scrollIndex == 0;
            }
        }

        #endregion
    }
}
