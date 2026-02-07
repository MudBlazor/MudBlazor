// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interop;
using MudBlazor.Services;
using MudBlazor.State;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Throttle;

namespace MudBlazor
{
    /// <summary>
    /// Organizes content across multiple tab pages.
    /// </summary>
    /// <seealso cref="MudTabPanel"/>
    public partial class MudTabs : MudComponentBase, IAsyncDisposable
    {
        internal List<MudTabPanel> _panels;
        private bool _isDisposed;
        private string? _prevIcon;
        private string? _nextIcon;
        private bool _isVerticalTabs;
        private bool _redraw;
        private bool _isSliderPositionDetermined;
        private bool _prevButtonDisabled;
        private bool _nextButtonDisabled;
        private bool _showScrollButtons;
        private ElementReference _tabsContentSize;
        private ElementReference _tabsInnerSize;
        private double _sliderSizePercentage;
        private double _sliderPositionPercentage;
        private double _tabBarContentSize;
        private double _allTabsSize;
        private double _scrollPosition;
        private IResizeObserver? _resizeObserver;
        private MudDropContainer<MudTabPanel>? _dropContainer;
        private readonly Lazy<ThrottleDispatcher> _throttleDispatcher;
        private readonly ParameterState<int> _activePanelIndexState;
        private readonly Dictionary<ElementReference, BoundingClientRect> _tabSizes = [];
        /// <summary>
        /// Unique identifier for this MudTabs component instance.
        /// Used to generate stable, unique IDs for tabs and panels to ensure ARIA compliance.
        /// Prevents ID conflicts when multiple tab components exist on the same page.
        /// </summary>
        private readonly string _componentId = Identifier.Create();
        private readonly string _elementId = Identifier.Create("tab");
        private string? _tabListId;

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

        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        [Inject]
        private TimeProvider TimeProvider { get; set; } = null!;

        /// <summary>
        /// Enables drag-and-drop re-ordering of tabs.
        /// </summary>
        /// <remarks>Defaults to <c>false</c>.</remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public bool EnableDragAndDrop { get; set; }

        /// <summary>
        /// When <see cref="EnableDragAndDrop" /> is set to true, this event will be raised when an item is dropped.
        /// The dropped item is provided in the <see cref="MudItemDropInfo{T}"/> and will have already been moved to its new position.
        /// </summary>
        [Parameter]
        public EventCallback<MudItemDropInfo<MudTabPanel>> OnItemDropped { get; set; }

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
        public bool KeepPanelsAlive { get; set; }

        /// <summary>
        /// Uses rounded corners on the tab's edges.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// When <c>true</c>, the <c>border-radius</c> style is set to the theme's default value.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public bool Rounded { get; set; }

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
        public bool Outlined { get; set; }

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
        public int? MaxHeight { get; set; }

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
        /// The CSS classes applied to all tab buttons.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string? TabButtonsClass { get; set; }

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
        /// The CSS classes applied to the element encasing the tab panels.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string? TabPanelsClass { get; set; }

        /// <summary>
        /// The currently selected tab panel.
        /// </summary>
        public MudTabPanel? ActivePanel => _activePanelIndexState.Value >= 0 && _activePanelIndexState.Value < _panels.Count ?
            _panels[_activePanelIndexState.Value] : null;

        /// <summary>
        /// The index of the currently selected tab panel.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c> (the first tab). When this value changes, <see cref="ActivePanelIndexChanged"/> occurs.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.Tabs.Behavior)]
        public int ActivePanelIndex { get; set; }

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
        /// Sort tab labels lexicographically by <see cref="MudTabPanel.Text"/> or <see cref="MudTabPanel.SortKey"/>. Ignored if <see cref="SortComparer" /> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SortDirection.None"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public SortDirection SortDirection { get; set; } = SortDirection.None;

        /// <summary>
        /// Specify a custom Comparer to sort tabs. When set, <see cref="SortDirection" /> is not used.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public IComparer<MudTabPanel>? SortComparer { get; set; }

        /// <summary>
        /// Can be used in derived class to add a class to the main container. If not overwritten return an empty string
        /// </summary>
        protected virtual string InternalClassName { get; } = string.Empty;

        #region Life cycle management

        /// <inheritdoc />
        public MudTabs()
        {
            _panels = new List<MudTabPanel>();
            Panels = _panels.AsReadOnly();
            _throttleDispatcher = new Lazy<ThrottleDispatcher>(() => new ThrottleDispatcher(500, TimeProvider));
            using var registerScope = CreateRegisterScope();
            _activePanelIndexState = registerScope.RegisterParameter<int>(nameof(ActivePanelIndex))
                .WithParameter(() => ActivePanelIndex)
                .WithEventCallback(() => ActivePanelIndexChanged)
                .WithChangeHandler(HandleActivePanelIndexChanged);
        }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            _resizeObserver = _resizeObserverFactory.Create();
            _tabListId = $"tablist-{_componentId}";
            base.OnInitialized();
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _resizeObserver ??= _resizeObserverFactory.Create();

            _nextIcon = RightToLeft ? PrevIcon : NextIcon;
            _prevIcon = RightToLeft ? NextIcon : PrevIcon;
            _isVerticalTabs = Position is Position.Left or Position.Right or Position.Start or Position.End;
        }

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var dragAndDropChanged = parameters.TryGetValue<bool>(nameof(EnableDragAndDrop), out var _);
            var positionChanged = parameters.TryGetValue<Position>(nameof(Position), out var _);
            if (dragAndDropChanged || positionChanged)
            {
                // need to recalculate the layout since drag and drop and position
                // changes the tab structure more broadly
                _redraw = true;
                _isVerticalTabs = Position is Position.Left or Position.Right or Position.Start or Position.End;
            }
            await base.SetParametersAsync(parameters);
        }

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                // add observer to inner and outer tab container to detect size changes
                // specifically for when the browser is resized, we need the inner 
                // observed since the content is width: 100% it'll bubble size changes to parent
                var items = new HashSet<ElementReference>(ResizeObserver.ElementReferenceComparer.Default)
                {
                    _tabsInnerSize,
                    _tabsContentSize
                };

                await _resizeObserver!.Observe(items);

                _resizeObserver.OnResized += OnResized;

                // fix ActivePanelIndex on initial render
                // https://github.com/MudBlazor/MudBlazor/issues/11519
                // must have an active panel to set scroll states                

                var startingIndex = _activePanelIndexState.Value;
                var index = FindNearestValidPanelIndex(startingIndex);

                if (index.HasValue && index.Value != startingIndex)
                {
                    // update the ActivePanelIndex to be valid
                    await _activePanelIndexState.SetValueAsync(index.Value);
                }

                var options = new KeyInterceptorOptions(
                    "mud-tab",
                    [
                        // prevent scrolling page
                        new(" ", preventDown: "key+none", preventUp: "key+none"),
                        new("Enter", preventDown: "key+none"),
                        new("NumpadEnter", preventDown: "key+none"),
                        new("Backspace", preventDown: "key+none")
                    ]);

                await KeyInterceptorService.SubscribeAsync(_elementId, options, keyDown: HandleKeyInterceptorAsync);
                _redraw = true;
                await InvokeAsync(StateHasChanged);
            }
            else if (_redraw)
            {
                _redraw = false;
                await CalculateLayoutAsync();
                await UpdateVisualStateAsync();
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
            if (_throttleDispatcher.IsValueCreated)
            {
                _throttleDispatcher.Value.Dispose();
            }
            if (_resizeObserver is not null)
            {
                _resizeObserver.OnResized -= OnResized;
                if (IsJSRuntimeAvailable)
                {
                    await _resizeObserver.DisposeAsync();
                }
            }
            if (IsJSRuntimeAvailable)
            {
                await KeyInterceptorService.UnsubscribeAsync(_elementId);
            }
        }

        #endregion

        #region Children

        internal async Task AddPanelAsync(MudTabPanel tabPanel)
        {
            _panels.Add(tabPanel);
            SortPanels();

            var activeIndex = _activePanelIndexState.Value;
            var activePanel = ActivePanel;

            if (activeIndex == -1)
            {
                // if no index set
                await _activePanelIndexState.SetValueAsync(_panels.IndexOf(tabPanel));
            }
            else if (activePanel != null && activeIndex != _panels.IndexOf(activePanel))
            {
                // if sortpanels changed the index readjust
                await _activePanelIndexState.SetValueAsync(_panels.IndexOf(activePanel));
            }
            _redraw = true;
            await InvokeAsync(StateHasChanged);
        }

        internal async Task SetPanelRefAsync(ElementReference reference)
        {
            if (HasRendered && _resizeObserver!.IsElementObserved(reference) == false)
                await _resizeObserver!.Observe(reference);

            _redraw = true;
            await InvokeAsync(StateHasChanged);
        }

        internal async Task RemovePanel(MudTabPanel tabPanel)
        {
            if (_isDisposed)
                return;

            await _resizeObserver!.Unobserve(tabPanel.PanelRef);

            var index = _panels.IndexOf(tabPanel);
            var wasActive = _activePanelIndexState.Value == index;
            var activePanel = ActivePanel;

            _panels.RemoveAt(index);

            // no panels left that are visible and not disabled
            if (!_panels.Any(x => x.Visible && !x.Disabled))
            {
                await _activePanelIndexState.SetValueAsync(-1);
            }
            else if (wasActive) // if removed tab was active, choose a new one
            {
                // either the last panel or the panel in the existing index whichever is lower
                var newIndex = FindNearestValidPanelIndex(index);
                if (!newIndex.HasValue)
                    await _activePanelIndexState.SetValueAsync(-1);
                else
                    await _activePanelIndexState.SetValueAsync(newIndex.Value);
            }
            else if (activePanel != null) // update index
            {
                await _activePanelIndexState.SetValueAsync(_panels.IndexOf(activePanel));
            }
            // queue a redraw if needed
            if (!_redraw)
            {
                _redraw = true;
                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Finds the nearest valid panel index that is Visible and not Disabled.
        /// Preference is given to panels to the left (previous tabs).
        /// </summary>
        private int? FindNearestValidPanelIndex(int startIndex)
        {
            if (!_panels.Any())
            {
                // nothing to find if no panels
                return null;
            }
            // Clamp starting point
            startIndex = Math.Clamp(startIndex, 0, _panels.Count - 1);
            // If the provided index is good stop here.
            if (_panels[startIndex] is { Visible: true, Disabled: false })
                return startIndex;

            // Search to the left
            for (int i = startIndex; i >= 0; i--)
            {
                if (_panels[i].Visible && !_panels[i].Disabled)
                    return i;
            }
            // Search to the right
            for (int i = startIndex + 1; i < _panels.Count; i++)
            {
                if (_panels[i].Visible && !_panels[i].Disabled)
                    return i;
            }
            return null;
        }

        /// <summary>
        /// Handles when ActivePanelIndex is changed outside of the component
        /// </summary>
        private Task HandleActivePanelIndexChanged(ParameterChangedEventArgs<int> args)
        {
            return ActivatePanelAsync(args.Value);
        }

        /// <summary>
        /// Sets the active panel and <see cref="ActivePanelIndex"/> property to match the provided index. 
        /// An invalid index is discarded and no changes are made.
        /// </summary>
        /// <param name="index">The index of the panel to activate.</param>
        /// <param name="ignoreDisabledState">When <c>true</c>, the panel will be activated even if it is disabled.</param>
        public Task ActivatePanelAsync(int index, bool ignoreDisabledState = false)
        {
            if (index < 0 || index >= _panels.Count)
                return Task.CompletedTask;

            return ActivatePanelAsync(_panels[index], ignoreDisabledState);
        }

        /// <summary>
        /// Sets the active panel and <see cref="ActivePanelIndex"/> property to match the provided unique id. 
        /// An invalid id is discarded and no changes are made.
        /// </summary>
        /// <param name="id">The unique ID of the panel to activate.</param>
        /// <param name="ignoreDisabledState">When <c>true</c>, the panel will be activated even if it is disabled.</param>
        public Task ActivatePanelAsync(object id, bool ignoreDisabledState = false)
        {
            var panel = _panels.FirstOrDefault(p => Equals(p.ID, id));
            return panel is null
                ? Task.CompletedTask
                : ActivatePanelAsync(panel, ignoreDisabledState);
        }

        /// <summary>
        /// Sets the active panel and <see cref="ActivePanelIndex"/> property to match the provided panel. 
        /// A <c>null</c> panel deactivates all panels.
        /// </summary>
        /// <param name="panel">The panel to activate. <c>null</c> to deactivate.</param>
        /// <param name="ignoreDisabledState">When <c>true</c>, the panel will be activated even if it is disabled.</param>
        public async Task ActivatePanelAsync(MudTabPanel? panel, bool ignoreDisabledState = false)
        {
            if (panel == null)
            {
                await _activePanelIndexState.SetValueAsync(-1);
            }
            else if (panel.Visible && (!panel.Disabled || ignoreDisabledState))
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

                await _activePanelIndexState.SetValueAsync(previewArgs.PanelIndex);
            }
            _redraw = true;
            await InvokeAsync(StateHasChanged);
        }

        private async Task ActivatePanelClickAsync(MudTabPanel panel, MouseEventArgs ev, bool ignoreDisabledState = false)
        {
            await ActivatePanelAsync(panel, ignoreDisabledState);
            await panel.OnClick.InvokeAsync(ev);
        }

        private void SortPanels()
        {
            if (_panels.Count == 0 || (SortDirection == SortDirection.None && SortComparer is null))
                return;

            _panels.Sort(GetTabSortExpression);
        }

        private int GetTabSortExpression(MudTabPanel a, MudTabPanel b)
        {
            if (SortComparer is not null)
            {
                return SortComparer.Compare(a, b);
            }

            var dir = SortDirection is SortDirection.Ascending ? 1 : -1;
            return Comparer.Default.Compare(GetTabSortKey(a), GetTabSortKey(b)) * dir;
        }

        private static string? GetTabSortKey(MudTabPanel panel) => panel.SortKey ?? panel.Text;
        #endregion

        #region Style and classes

        protected string TabsClassnames =>
            new CssBuilder("mud-tabs")
                .AddClass($"mud-tabs-rounded", ApplyEffectsToContainer && Rounded)
                .AddClass($"mud-paper-outlined", ApplyEffectsToContainer && Outlined)
                .AddClass($"mud-elevation-{Elevation}", ApplyEffectsToContainer && Elevation != 0)
                .AddClass($"mud-tabs-reverse", Position == Position.Bottom)
                .AddClass($"mud-tabs-vertical", _isVerticalTabs)
                .AddClass($"mud-tabs-vertical-reverse", Position == Position.Right && !RightToLeft || (Position == Position.Left) && RightToLeft || Position == Position.End)
                .AddClass(InternalClassName)
                .AddClass(Class)
                .Build();

        protected string TabBarClassnames =>
            new CssBuilder("mud-tabs-tabbar")
                .AddClass($"mud-tabs-rounded", !ApplyEffectsToContainer && Rounded)
                .AddClass($"mud-tabs-vertical", _isVerticalTabs)
                .AddClass($"mud-tabs-tabbar-{Color.ToStringFast(true)}", Color != Color.Default)
                .AddClass($"mud-tabs-border-{ConvertPosition(Position).ToStringFast(true)}", Border)
                .AddClass($"mud-paper-outlined", !ApplyEffectsToContainer && Outlined)
                .AddClass($"mud-elevation-{Elevation}", !ApplyEffectsToContainer && Elevation != 0)
                .AddClass(TabHeaderClass)
                .Build();

        protected string WrapperClassnames =>
            new CssBuilder("mud-tabs-tabbar-wrapper")
                .AddClass($"mud-tabs-centered", Centered)
                .AddClass($"mud-tabs-vertical", _isVerticalTabs)
                .Build();

        /// <summary>
        /// The pixel-based scroll offset for the tab bar, used as the value for the CSS transform property.
        /// For horizontal tabs, _scrollPosition is applied to translateX; for vertical tabs, to translateY.
        /// </summary>
        protected string WrapperScrollStyle =>
            new StyleBuilder()
                .AddStyle("transform", $"translateX({(-1 * _scrollPosition).ToString(CultureInfo.InvariantCulture)}px)", !_isVerticalTabs)
                .AddStyle("transform", $"translateY({(-1 * _scrollPosition).ToString(CultureInfo.InvariantCulture)}px)", _isVerticalTabs)
                .Build();

        protected string PanelsClassnames =>
            new CssBuilder("mud-tabs-panels")
                .AddClass($"mud-tabs-vertical", _isVerticalTabs)
                .AddClass(TabPanelsClass)
                .Build();

        protected string SliderClass =>
            new CssBuilder("mud-tab-slider")
                .AddClass($"mud-{SliderColor.ToStringFast(true)}", SliderColor != Color.Inherit)
                .AddClass($"mud-tab-slider-horizontal", Position is Position.Top or Position.Bottom)
                .AddClass($"mud-tab-slider-vertical", _isVerticalTabs)
                .AddClass($"mud-tab-slider-horizontal-reverse", Position == Position.Bottom)
                .AddClass($"mud-tab-slider-vertical-reverse", Position == Position.Right || Position == Position.Start && RightToLeft || Position == Position.End && !RightToLeft)
                .Build();

        protected string DropZoneClassnames =>
            new CssBuilder("mud-tabs-dropzone")
                .AddClass("d-flex", !_isVerticalTabs)
                .AddClass($"mud-tabs-vertical", _isVerticalTabs)
                .AddClass("flex-grow-1")
                .Build();

        protected string MaxHeightStyles =>
            new StyleBuilder()
                .AddStyle("max-height", MaxHeight.ToPx(), MaxHeight != null)
                .Build();

        protected string SliderStyle => RightToLeft
            ? new StyleBuilder()
                .AddStyle("width", _sliderSizePercentage.ToPercent(), Position is Position.Top or Position.Bottom)
                .AddStyle("right", _sliderPositionPercentage.ToPercent(), Position is Position.Top or Position.Bottom)
                .AddStyle("transition", SliderAnimation ? "right .3s cubic-bezier(.64,.09,.08,1);" : "none", Position is Position.Top or Position.Bottom)
                .AddStyle("transition", SliderAnimation ? "top .3s cubic-bezier(.64,.09,.08,1);" : "none", _isVerticalTabs)
                .AddStyle("height", _sliderSizePercentage.ToPercent(), _isVerticalTabs)
                .AddStyle("top", _sliderPositionPercentage.ToPercent(), _isVerticalTabs)
                .Build()
            : new StyleBuilder()
                .AddStyle("width", _sliderSizePercentage.ToPercent(), Position is Position.Top or Position.Bottom)
                .AddStyle("left", _sliderPositionPercentage.ToPercent(), Position is Position.Top or Position.Bottom)
                .AddStyle("transition", SliderAnimation ? "left .3s cubic-bezier(.64,.09,.08,1);" : "none", Position is Position.Top or Position.Bottom)
                .AddStyle("transition", SliderAnimation ? "top .3s cubic-bezier(.64,.09,.08,1);" : "none", _isVerticalTabs)
                .AddStyle("height", _sliderSizePercentage.ToPercent(), _isVerticalTabs)
                .AddStyle("top", _sliderPositionPercentage.ToPercent(), _isVerticalTabs)
                .Build();

        private Position ConvertPosition(Position position)
        {
            return position switch
            {
                Position.Start => RightToLeft ? Position.Right : Position.Left,
                Position.End => RightToLeft ? Position.Left : Position.Right,
                _ => position
            };
        }

        private string GetTabClass(MudTabPanel panel)
        {
            var tabClass = new CssBuilder("mud-tab")
              .AddClass($"mud-tab-active", when: () => panel == ActivePanel)
              .AddClass($"mud-disabled", panel.Disabled)
              .AddClass($"mud-ripple", Ripple)
              .AddClass(ActiveTabClass, when: () => panel == ActivePanel)
              .AddClass(TabButtonsClass)
              .AddClass(panel.Classname)
              .Build();

            return tabClass;
        }

        private Placement GetTooltipPlacement()
        {
            return Position switch
            {
                Position.Right => Placement.Left,
                Position.Left => Placement.Right,
                Position.Bottom => Placement.Top,
                _ => Placement.Bottom
            };
        }

        private string GetTabStyle(MudTabPanel panel)
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

        /// <summary>
        /// Calculates the layout sizing of the containers and scroll buttons.
        /// </summary>
        private async Task CalculateLayoutAsync()
        {
            _dropContainer?.Refresh();
            await GetAllReferenceSizes();
            GetAllTabsSize();
            _tabBarContentSize = GetTabBarContentSize();
            SetScrollButtonVisibility();
        }

        /// <summary>
        /// Determines the visual state of the component after rendering.
        /// </summary>
        /// <remarks>
        /// Used to update scroll position, scrollability states (scrollprevious and scrollnext buttons), 
        /// and slider state.
        /// </remarks>
        private Task UpdateVisualStateAsync()
        {
            CenterScrollPositionAroundSelectedItem();
            SetScrollabilityStates();
            SetSliderState();
            return InvokeAsync(StateHasChanged);
        }

        private async void OnResized(IDictionary<ElementReference, BoundingClientRect> changes)
        {
            if (!_redraw)
            {
                _redraw = true;
                await InvokeAsync(StateHasChanged);
            }
        }

        private void SetSliderState()
        {
            if (ActivePanel is null)
            {
                return;
            }
            _sliderPositionPercentage = (GetLengthOfPanelItems(ActivePanel) / _allTabsSize) * 100;
            _sliderSizePercentage = (GetPanelLength(ActivePanel) / _allTabsSize) * 100;
            _isSliderPositionDetermined =
                (_activePanelIndexState.Value > 0 && _sliderPositionPercentage > 0)
                || IsFirstVisiblePanel(ActivePanel);
        }

        /// <summary>
        /// this sets _tabBarContentSize to the total calculated width of the mud-tabs-tabbar-content
        /// </summary>
        private double GetTabBarContentSize()
        {
            return GetRelevantSize(_tabsContentSize);
        }

        private async Task GetAllReferenceSizes()
        {
            _tabSizes.Clear();
            var panelRefs = _panels.Select(x => x.PanelRef).ToList();
            panelRefs.Add(_tabsContentSize);
            var tasks = panelRefs.Select(async panelRef => (panelRef, rect: await panelRef.MudGetBoundingClientRectAsync())).ToList();
            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                _tabSizes.TryAdd(result.panelRef, result.rect);
            }
        }

        private void GetAllTabsSize()
        {
            double totalTabsSize = 0;

            foreach (var panel in _panels)
            {
                totalTabsSize += GetRelevantSize(panel.PanelRef);
            }

            _allTabsSize = totalTabsSize;
        }

        private double GetRelevantSize(ElementReference reference)
        {
            // _tabSizes get current values using MudGetBoundingClientRectAsync
            var success = _tabSizes.TryGetValue(reference, out var rect);

            var height = rect?.Height ?? 0.0;
            var width = rect?.Width ?? 0.0;
            const double epsilon = 0.01; // handles any odd rounding errors

            if (Math.Abs(height) < epsilon || Math.Abs(width) < epsilon)
                success = false;

            // fallback to resizeobserver values if null or 0
            if (!success)
            {
                rect = _resizeObserver!.GetSizeInfo(reference);
                // ensure we don't return a null value
                height = rect?.Height ?? 0.0;
                width = rect?.Width ?? 0.0;
            }

            return _isVerticalTabs ? height : width;
        }

        /// <summary>
        /// Returns the width or height of a subset of panel items up to the panel item selected, depending on tab orientation.
        /// </summary>
        /// <remarks>
        /// If inclusive is true, it returns the width or height of the panel item selected as well.
        /// For horizontal tabs, this is the width; for vertical tabs, this is the height.
        /// </remarks>
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

        private bool IsFirstVisiblePanel(MudTabPanel? activePanel)
        {
            foreach (var panel in _panels)
            {
                if (activePanel == panel)
                {
                    return true;
                }

                if (panel.Visible)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region scrolling

        /// <summary>
        /// Sets the visibility of scroll buttons based on the total size of all tabs and the tab bar content size.
        /// Scroll buttons are shown if AlwaysShowScrollButtons is true or if the tabs exceed the available space.
        /// </summary>
        private void SetScrollButtonVisibility()
        {
            _showScrollButtons = AlwaysShowScrollButtons || (int)_allTabsSize > (int)_tabBarContentSize;
        }

        private void ScrollPrev()
        {
            if (_panels.Count == 0)
                return;

            ScrollBy(isNext: false);
            SetScrollButtonVisibility();
            SetScrollabilityStates();
        }

        private void ScrollNext()
        {
            ScrollBy(isNext: true);
            SetScrollabilityStates();
        }

        /// <summary>
        /// Scrolls a <see cref="MudTabPanel" /> to the center of the tab content viewport. Will not scroll beyond the bounds of tabs.
        /// </summary>
        private void ScrollToItem(MudTabPanel panel, bool isLast = false)
        {
            // set start and max scroll
            double position;
            // all panels before selected panel
            var preSize = GetLengthOfPanelItems(panel, false);
            var panelSize = GetPanelLength(panel);
            var maxScroll = _allTabsSize - _tabBarContentSize;
            if (isLast)
            {
                // scroll so the right edge of the last tab is flush to the right edge of the mud-tabs-content (visible tab area)
                position = maxScroll;
            }
            else
            {
                // normal scroll to center the active tab
                var viewportCenter = _tabBarContentSize / 2;
                var panelCenter = panelSize / 2;
                position = preSize - viewportCenter + panelCenter;
            }
            // ensure no extra space past the start or end
            position = ScrollEdgeAdjust(position, panelSize);
            // right to left uses negative positioning but only when it's horizontal tabs
            position = RightToLeft && !_isVerticalTabs ? -position : position;
            _scrollPosition = position;
        }

        /// <summary>
        /// Sets the minimum and maximum scroll position values and clamps scrollposition to it.
        /// </summary>
        private double ScrollEdgeAdjust(double position, double panelSize)
        {
            var minScroll = 0.0;
            var maxScroll = Math.Max(0, _allTabsSize - _tabBarContentSize);
            if (_tabBarContentSize < panelSize)
            {
                var tooSmallSize = _tabBarContentSize;
                maxScroll = Math.Max(0, maxScroll - tooSmallSize);
                minScroll = Math.Min(maxScroll, tooSmallSize / 2);
            }
            return Math.Clamp(position, minScroll, maxScroll);
        }

        /// <summary>
        /// Scroll by page; isNext is true to scroll forward (right or down), false to scroll backward (left or up), depending on tab orientation.
        /// </summary>
        private void ScrollBy(bool isNext)
        {
            if (_panels.Count == 0)
                return;

            var panel = isNext ? _panels[^1] : _panels[0];
            var panelSize = GetPanelLength(panel);
            var scrollAmount = Math.Max(_tabBarContentSize, panelSize); // minimum 1 tab scroll
            if (!isNext)
            {
                scrollAmount = -scrollAmount;
            }
            var position = ScrollEdgeAdjust(_scrollPosition + scrollAmount, panelSize);
            _scrollPosition = position;
        }

        private void CenterScrollPositionAroundSelectedItem()
        {
            var activeIndex = _activePanelIndexState.Value;
            if (activeIndex < 0)
                return;
            if (activeIndex + 1 == _panels.Count)
            {
                var lastPanel = _panels.Last();
                ScrollToItem(lastPanel, true);
                return;
            }

            // scroll to the panel
            ScrollToItem(ActivePanel!);
        }

        private void SetScrollabilityStates()
        {
            var isEnoughSpace = _allTabsSize <= _tabBarContentSize;

            if (isEnoughSpace)
            {
                _nextButtonDisabled = true;
                _prevButtonDisabled = Math.Abs(_scrollPosition) < 0.01;
            }
            else
            {
                // Disable next button if the last panel is completely visible
                _nextButtonDisabled = Math.Abs(_scrollPosition) >= GetLengthOfPanelItems(_panels[^1], true) - _tabBarContentSize;
                _prevButtonDisabled = Math.Abs(_scrollPosition) < 0.01;
            }
        }

        #endregion

        internal async Task ItemUpdated(MudItemDropInfo<MudTabPanel> dropItem)
        {
            if (dropItem.Item is null)
            {
                return;
            }

            // get the old index where this item was at
            var oldIndex = _panels.IndexOf(dropItem.Item);
            // get the new index in _panels using IndexInZone
            var newIndex = dropItem.IndexInZone;

            // remove the item from the old index
            _panels.RemoveAt(oldIndex);

            // insert the item at the new index
            if (newIndex < _panels.Count)
            {
                _panels.Insert(newIndex, dropItem.Item);
            }
            else
            {
                _panels.Add(dropItem.Item);
            }

            // Set the dragged tab as active
            await ActivatePanelAsync(dropItem.Item);

            if (OnItemDropped.HasDelegate)
            {
                await OnItemDropped.InvokeAsync(dropItem);
            }
        }

        /// <summary>
        /// Handles keyboard navigation for tabs according to W3C accessibility guidelines
        /// Supports Enter/Space for activation and arrow keys for navigation
        /// </summary>
        protected virtual async Task HandleTabKeyDownAsync(KeyboardEventArgs e, MudTabPanel panel)
        {
            switch (e.Key)
            {
                case "Enter":
                case " ":
                    // considered a click
                    var args = new MouseEventArgs() { Type = e.Key, Detail = 1, ClientX = 0, ClientY = 0 };
                    await ActivatePanelClickAsync(panel, args);
                    break;

                case "ArrowLeft":
                    if (!_isVerticalTabs)
                    {
                        await MoveFocusToPreviousTab(panel);
                    }
                    break;

                case "ArrowRight":
                    if (!_isVerticalTabs)
                    {
                        await MoveFocusToNextTab(panel);
                    }
                    break;

                case "ArrowUp":
                    if (_isVerticalTabs)
                    {
                        await MoveFocusToPreviousTab(panel);
                    }
                    break;

                case "ArrowDown":
                    if (_isVerticalTabs)
                    {
                        await MoveFocusToNextTab(panel);
                    }
                    break;
            }
        }

        private async Task HandleKeyInterceptorAsync(KeyboardEventArgs e)
        {
            var focusedPanel = ActivePanel;
            if (focusedPanel != null)
            {
                await HandleTabKeyDownAsync(e, focusedPanel);
            }
        }

        /// <summary>
        /// Allows the user to move to the previous tab using key arrow
        /// </summary>
        private async Task MoveFocusToPreviousTab(MudTabPanel currentPanel)
        {
            var enabledPanels = _panels.Where(p => !p.Disabled).ToList();
            if (enabledPanels.Count <= 1) return;

            var currentIndex = enabledPanels.IndexOf(currentPanel);
            var previousIndex = currentIndex <= 0 ? enabledPanels.Count - 1 : currentIndex - 1;
            var previousPanel = enabledPanels[previousIndex];

            await FocusPanel(previousPanel);
        }

        /// <summary>
        /// Allows the user to move to the next tab using KeyArrow
        /// </summary>
        private async Task MoveFocusToNextTab(MudTabPanel currentPanel)
        {
            var enabledPanels = _panels.Where(p => !p.Disabled).ToList();
            if (enabledPanels.Count <= 1) return;

            var currentIndex = enabledPanels.IndexOf(currentPanel);
            var nextIndex = currentIndex >= enabledPanels.Count - 1 ? 0 : currentIndex + 1;
            var nextPanel = enabledPanels[nextIndex];

            await FocusPanel(nextPanel);
        }

        /// <summary>
        /// Focuses user onto selected panel
        /// </summary>
        private static async Task FocusPanel(MudTabPanel panel)
        {
            if (panel.PanelRef.Context != null)
            {
                await panel.PanelRef.FocusAsync();
            }
        }

        /// <summary>
        /// Generates a unique ID for a tab element using the tab panels field id.
        /// Required for aria-controls attribute to link tab to its panel.
        /// </summary>
        internal string GetTabId(MudTabPanel panel)
        {
            return $"tablist-{_componentId}-tab-{panel.FieldId}";
        }

        /// <summary>
        /// Generates a unique ID for a tab panel element using the tab panels field id.
        /// Required for aria-controls attribute to link tab panel to its tab.
        /// </summary>
        internal string GetTabPanelId(MudTabPanel panel)
        {
            return $"tablist-{_componentId}-tabpanel-{panel.FieldId}";
        }

        /// <summary>
        /// Generates a unique ID for a tab list. 
        /// Required for aria-controls attribute to identify each tablist.
        /// </summary>
        internal string GetTabListId()
        {
            return _tabListId!;
        }
    }
}
