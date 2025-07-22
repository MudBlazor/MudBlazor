using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Components.Combobox;
using MudBlazor.State;
using MudBlazor.Utilities;
#nullable enable
namespace MudBlazor
{
    public partial class MudComboBox<T> : MudBaseInput<T>
    {
        private readonly object _itemsLock = new();
        private readonly string _elementId = Identifier.Create("select");
        private readonly List<ComboBoxItem<T>> _comboBoxItems = [];

        private ParameterState<HashSet<T>> _selectedItemsState;
        private ParameterState<bool> _isLoadingState;
        private ParameterState<bool> _openItemListState;

        private CancellationTokenSource? _cancellationTokenSrc;
        private Timer? _debounceTimer;

        private MudInput<string> _elementReference = default!;

        public MudComboBox()
        {
            using var registerScope = CreateRegisterScope();
            _selectedItemsState = registerScope.RegisterParameter<HashSet<T>>(nameof(SelectedItems))
                .WithParameter(() => SelectedItems)
                .WithEventCallback(() => SelectedItemsChanged);
            _isLoadingState = registerScope.RegisterParameter<bool>(nameof(IsLoading))
                .WithParameter(() => IsLoading)
                .WithEventCallback(() => IsLoadingChanged);
            _openItemListState = registerScope.RegisterParameter<bool>(nameof(OpenItemList))
                .WithParameter(() => OpenItemList)
                .WithEventCallback(() => OpenItemListChanged);
        }

        [Inject]
        private IScrollManager ScrollManager { get; set; } = null!;

        [Inject]
        private InternalMudLocalizer Localizer { get; set; } = null!;

        protected string OuterClassname =>
            new CssBuilder("mud-select")
                .AddClass("mud-width-full", FullWidth)
                .AddClass("mud-width-content", FitContent && !FullWidth)
                .AddClass(OuterClass)
                .Build();

        protected string Classname =>
            new CssBuilder("mud-select")
                .AddClass(Class)
                .Build();

        protected string InputClassname =>
            new CssBuilder("mud-select-input")
                .AddClass(InputClass)
                .Build();

        protected string FillerClassname =>
            new CssBuilder("mud-select-filler")
                .AddClass("d-inline-block")
                .AddClass("invisible")
                .AddClass("mx-2", Variant == Variant.Text)
                .AddClass("mx-4", Variant != Variant.Text)
                .Build();

        /// <summary>
        /// The Right to Left designated by the parent
        /// </summary>
        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        #region Confirmed Parameters

        /// <summary>
        /// The class or classes applied to the input element.
        /// </summary>
        [Parameter]
        public string? InputClass { get; set; }

        /// <summary>
        /// The class or classes applied to the <see cref="MudPopover" /> that contains the list of ComboBox items.
        /// </summary>
        [Parameter]
        public string? PopoverClass { get; set; }

        /// <summary>
        /// The location where the popover will open from.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.BottomLeft" />.
        /// </remarks>
        [Parameter]
        public Origin AnchorOrigin { get; set; } = Origin.BottomLeft;

        /// <summary>
        /// The transform origin point for the popover.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.TopLeft"/>.
        /// </remarks>
        [Parameter]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// Updates the Value and SelectedItems to the currently selected item when pressing the Tab key.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool SelectValueOnTab { get; set; }

        /// <summary>
        /// Opens the list when focus is received on the input element; otherwise only opens on click.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c> so the list opens anytime it receives focus regardless of how.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool OpenOnFocus { get; set; } = true;

        /// <summary>
        /// Whether the OpenList closes when an item is Selected via ComboBoxToggleItem
        /// </summary>
        /// <remarks>Defaults to <c>true</c>.</remarks>
        [Parameter]
        public bool AutoClose { get; set; } = true;

        /// <summary>
        /// The maximum height, in pixels, of the Combobox Popover when it is open.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>300</c>.
        /// </remarks>
        [Parameter]
        public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// Whether the dropdown becomes filterable by text input. In client mode the items will be filtered by the <see cref="ToStringFunc"/> 
        /// or default <c>ToString()</c> method.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Filterable { get; set; }

        /// <summary>
        /// The custom template used for the progress indicator when <see cref="ShowProgressIndicator"/> is <c>true</c>.
        /// <para>In Order to create a progress indicator inside your popover use the BeforeItemsTemplate.</para>
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment? ProgressIndicatorTemplate { get; set; }

        /// <summary>
        /// Any template you wish to place Before the Items list.
        /// </summary>
        [Parameter]
        public RenderFragment? BeforeItemsTemplate { get; set; }

        /// <summary>
        /// What is displayed when there are no AutoCompleteItems. 
        /// </summary>
        [Parameter]
        public RenderFragment? NoRecords { get; set; }

        /// <summary>
        /// Determines the width of the ComboBox dropdown in relation to the parent container.
        /// </summary>
        /// <remarks>
        /// <para>Defaults to <see cref="DropdownWidth.Relative" />. </para>
        /// <para>When SmallScreens is set DropdownWidth is overridden to <see cref="DropdownWidth.Ignore" /></para>.
        /// <para>When <see cref="DropdownWidth.Relative" />, restricts the max-width of the component to the width of the parent container</para>
        /// <para>When <see cref="DropdownWidth.Adaptive" />, restricts the min-width of the component to the width of the parent container</para>
        /// <para>When <see cref="DropdownWidth.Ignore" />, there are no width restrictions of the component to the width of the parent container</para>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public DropdownWidth RelativeWidth { get; set; } = DropdownWidth.Relative;

        /// <summary>
        /// Sets the container width to match its contents.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. Requires FullWidth to be <c>false</c>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool FitContent { get; set; }

        /// <summary>
        /// The CSS classes applied to the outer <c>div</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter]
        public string? OuterClass { get; set; }

        /// <summary>
        /// The behavior of the ComboBox dropdown. 
        /// <para>OverflowBehavior when it cannot display in full at the original Anchor and Transform positions.</para>
        /// <para>Fixed true displays the dropdown popover in a fixed position, even while scrolling.</para>
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DropdownSettings.Fixed" /> false
        /// Defaults to <see cref="DropdownSettings.OverflowBehavior" /> <see cref="OverflowBehavior.FlipOnOpen" />
        /// </remarks>
        [Category(CategoryTypes.Popover.Behavior)]
        [Parameter]
        public DropdownSettings DropdownSettings { get; set; } = new DropdownSettings();

        /// <summary>
        /// Uses a <see cref="MudOverlay"/> when the dropdown is open. 
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool Overlay { get; set; } = true;

        /// <summary>
        /// Displays the Clear icon button. Has no impact if Filterable is not <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, an icon is displayed which, when clicked, clears the filter Text.  Use the <c>ClearIcon</c> property to control the Clear button icon.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; }

        /// <summary>
        /// The "open" Combobox icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowDropDown"/>.
        /// </remarks>
        [Parameter]
        public string OpenIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// The "close" Combobox icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowDropDown"/>.
        /// </remarks>
        [Parameter]
        public string CloseIcon { get; set; } = Icons.Material.Filled.ArrowDropUp;

        /// <summary>
        /// The icon to display when <see cref="Clearable"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Clear"/>.
        /// </remarks>
        [Parameter]
        public string ClearIcon { get; set; } = Icons.Material.Filled.Cancel;

        /// <summary>
        /// The Add Combobox item icon. When OnAddItemClick is defined this icon is shown when the Text property exceeds MinCharacters.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.AddCircle"/>.
        /// </remarks>
        [Parameter]
        public string AddIcon { get; set; } = Icons.Material.Filled.AddCircle;

        /// <summary>
        /// When this method is defined the AddIcon is shown when the Text property exceeds MinCharacters. When clicked it executes the method.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAddItemClick { get; set; }

        /// <summary>
        /// The maximum number of items to display.
        /// </summary>
        /// <remarks>
        /// <para>Defaults to <c>0</c>. A value of 0 will display all items.</para>
        /// <para>Value cannot be less than 0</para>
        /// </remarks>
        [Parameter]
        public int MaxItems { get; set; } = 0;

        /// <summary>
        /// The minimum number of characters typed to initiate a search. 
        /// <para>The clear and add buttons use this as <c>MinCharacters + 1</c> to display.</para>
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.
        /// </remarks>
        [Parameter]
        public int MinCharacters { get; set; }

        #endregion

        /// <summary>
        /// Sets the point at which the list becomes a BottomSheet encompassing the entire bottom (or top) of the presumed mobile display.
        /// <para>--TODO--</para>
        /// </summary>
        [Parameter]
        public Breakpoint? BottomSheet { get; set; } = Breakpoint.SmAndDown;

        /// <summary>
        /// The function used to determine if an item should be disabled.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T, bool>? ItemDisabledFunc { get; set; }

        /// <summary>
        /// The function used to get the display text for each item.
        /// </summary>
        /// <remarks>
        /// Defaults to the <c>ToString()</c> method of items.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T?, string?>? ToStringFunc { get; set; }

        /// <summary>
        /// Shows the progress indicator during searches.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. The progress indicator uses the color specified in the <see cref="ProgressIndicatorColor"/> property.
        /// </remarks>
        [Parameter]
        public bool ShowProgressIndicator { get; set; }

        /// <summary>
        /// The color of the progress indicator.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.  This property is used when <see cref="ShowProgressIndicator"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        public Color ProgressIndicatorColor { get; set; } = Color.Default;

        /// <summary>
        /// The function used to search for items.
        /// </summary>
        /// <remarks>
        /// This function searches for items containing the specified <c>string</c> value, and returns items which match up to the <see cref="MaxItems"/> property.  You can use the provided <see cref="CancellationToken"/> which is marked as canceled when the user changes the search text or selects a value from the list.
        /// </remarks>
        [Parameter]
        public Func<string?, CancellationToken?, Task<IEnumerable<T>>?>? SearchFunc { get; set; }

        /// <summary>
        /// Reset the selected value if the user deletes the text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool ResetValueOnEmptyText { get; set; }

        /// <summary>
        /// Highlights the text when the component receives focus.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool SelectOnActivation { get; set; } = true;

        /// <summary>
        /// The debounce interval, in milliseconds.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>100</c>. A higher value can help reduce the number of calls to <see cref="SearchFunc"/>, which can improve responsiveness.
        /// </remarks>
        [Parameter]
        public int DebounceInterval { get; set; } = 100;

        /// <summary>
        /// The template used to display all the items in the PopoverList, contains a context of <see cref="MudComboBoxItem{T}"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<MudComboBoxItem<T>>? ItemTemplate { get; set; }

        /// <summary>
        /// The template used to display selected items in the textbox area, contains a context of <see cref="MudComboBoxItem{T}"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<MudComboBoxItem<T>>? SelectedItemsTemplate { get; set; }

        /// <summary>
        /// Whether a user can select multiple items
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool MultiSelection { get; set; }

        /// <summary>
        /// The currently selected ComboBox items
        /// </summary>
        [Parameter]
        public HashSet<T> SelectedItems { get; set; } = [];

        /// <summary>
        /// Gets the number of items currently selected.
        /// </summary>
        public int SelectedItemsCount { get => SelectedItems.Count; }

        /// <summary>
        /// Event is fired when the selected item(s) change
        /// </summary>
        [Parameter]
        public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        /// <summary>
        /// Sets the filter type, Client filters based on <see cref="Items"/>, Server expects user to return <see cref="SearchFunc"/> results.
        /// Default is Client
        /// </summary>
        [Parameter]
        public ComboBoxFilterType FilterType { get; set; } = ComboBoxFilterType.Client;

        /// <summary>
        /// Whether the item list is currently open or not, in a MudPopover
        /// </summary>
        [Parameter]
        public bool OpenItemList { get; set; }

        /// <summary>
        /// Gets or sets the callback that is invoked when the state of the open item list changes.
        /// </summary>
        [Parameter]
        public EventCallback<bool> OpenItemListChanged { get; set; }

        /// <summary>
        /// Whether or not to the built in progress indicator is being currently shown.
        /// </summary>
        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public EventCallback<bool> IsLoadingChanged { get; set; }

        /// <summary>
        /// Controls whether text input overrides selected item values. Only applies to current/last selected item when Multiselection is true.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When true (default): Selecting an item from the dropdown will replace any user-typed text
        /// in the input field with the selected item's text.
        /// </para>
        /// <para>
        /// When false: User-typed text remains unchanged even after selecting items, allowing for 
        /// partial/incomplete text that doesn't match any selection.
        /// </para>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool CoerceText { get; set; } = true;

        /// <summary>
        /// The list of items filtered 
        /// TODO: Add SortFunc
        /// </summary>
        public IReadOnlyList<T> FilteredItems { get; private set; } = [];

        /// <summary>
        /// The number of items currently filtered
        /// </summary>
        public int FilteredItemsCount { get => FilteredItems.Count; }

        /// <summary>
        /// The list of items in the ComboBox when using <see cref="ComboBoxFilterType.Client"/>, the <see cref="ToStringFunc"/> is used for display with <c>ToString()</c> used as fallback.
        /// </summary>
        [Parameter]
        public IEnumerable<T> Items
        {
            get { return value; }
            set
            {
                foreach (var item in value)
                {
                    if (item is null)
                        continue;
                    var comboBoxItem = new MudComboBoxItem<T>()
                    {
                        Value = item,
                        ComboBox = this,
                        ItemId = GetListItemId(_comboBoxItems.Count)
                    };
                }
            }
        }

        /// <summary>
        /// The number of items
        /// </summary>
        public int ItemsCount { get => Items.Count(); }

        private string? GetItemString(T? item)
        {
            if (item is null)
            {
                return string.Empty;
            }

            try
            {
                return ToStringFunc?.Invoke(item) ?? item.ToString();
            }
            catch (NullReferenceException)
            {
                return "null";
            }
        }

        private string GetListItemId(in int index)
        {
            return $"{_componentId}_item{index}";
        }

        /// <summary>
        /// Toggles the ComboBox item, if it's not selected it will be, if it is selected it will be unselected.
        /// </summary>
        /// <param name="item">The item of type <c>T</c> to Toggle.</param>
        /// <param name="toggleMenu">Whether to toggle the menu open/closed after.</param>
        public async Task ComboBoxToggleItem(T? item, bool toggleMenu = false)
        {
            if (item == null)
                return;

            // Toggle SelectedItems to Add if it doesn't exist, remove it if it does.
            // start by creating a new hashset list to ensure updates
            var selectedItems = new HashSet<T>(_selectedItemsState.Value ?? []);

            // if removing the item is false then add the item
            var toggled = selectedItems.Remove(item);
            if (!toggled)
            {
                // if it's single selection clear the list first
                if (!MultiSelection)
                {
                    selectedItems.Clear();
                }
                selectedItems.Add(item);
                // set value to item
                Value = item;
            }
            else
            {
                // set Value to default
                Value = default;
            }
            // clear text and update Selected Items
            await SetTextAsync(default, false);
            await _selectedItemsState.SetValueAsync(selectedItems);
            await DebounceTimerDispose();
            await BeginValidateAsync();
            // Toggle Menu if it's supposed to (they update StateHasChanged) if not call StateHasChanged manually
            if (toggleMenu)
            {
                await ComboBoxToggleListAsync();
            }
            else
                StateHasChanged();
        }

        private async Task ComboBoxToggleListAsync()
        {
            // do not make public, access to two way bind activates accordingly
            if (_openItemListState.Value)
            {
                await CloseListAsync();
            }
            else
            {
                await _elementReference.FocusAsync();
                await OpenListAsync();
            }
        }

        private async Task OpenListAsync()
        {
            // do not make public, access to two way bind activates accordingly
            // make sure it can be opened
            if (GetReadOnlyState() || GetDisabledState())
                return;

            // only set the value if it's not already set
            if (!_openItemListState.Value)
            {
                await _openItemListState.SetValueAsync(true);
                if (FilteredItemsCount > 0)
                    await ScrollManager.ScrollToListItemAsync(GetListItemId(0));
            }

            // start searching
            if (DebounceInterval <= 0)
                await PerformSearchAsync();
            else
                _debounceTimer = new Timer(OnDebounceComplete, null, DebounceInterval, Timeout.Infinite);
        }

        private async Task CloseListAsync()
        {
            // do not make public, access to two way bind activates accordingly
            CancelToken();
            await DebounceTimerDispose();
            //await RestoreScrollPositionAsync();
            await _openItemListState.SetValueAsync(false);
            StateHasChanged();
        }

        private Task OnOpenChanged(ParameterChangedEventArgs<bool> args)
        {
            // triggers when OpenListItems is toggled by two way bind
            if (!args.LastValue)
            {
                return OpenListAsync();
            }
            return CloseListAsync();
        }

        private ValueTask DebounceTimerDispose()
        {
            if (_debounceTimer != null)
            {
                return _debounceTimer.DisposeAsync();
            }
            return ValueTask.CompletedTask;
        }

        private void OnDebounceComplete(object? stateInfo) => InvokeAsync(PerformSearchAsync);

        /// <summary>
        /// Selects all the current text within the Autocomplete text box.
        /// </summary>
        public override ValueTask SelectAsync()
        {
            return _elementReference.MudSelectAsync();
        }

        /// <summary>
        /// Selects a portion of the text within the Autocomplete text box.
        /// </summary>
        /// <param name="pos1">The index of the first character to select.</param>
        /// <param name="pos2">The index of the last character to select.</param>
        /// <returns>A <see cref="ValueTask"/> object.</returns>
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.MudSelectRangeAsync(pos1, pos2);
        }

        private async Task OnInputClickedAsync()
        {
            // this fires at nearly the same time as OnInputFocusedAsync, so we need to delay when both fire together
            // to prevent running the search method twice
            await Task.Delay(5);
            if (_activatorEvents)
            {
                _activatorEvents = false;
                return;
            }
            await InputActivationAsync(true);
        }

        private async Task OnInputFocusedAsync()
        {
            if (OpenOnFocus)
            {
                _activatorEvents = true;
            }
            await InputActivationAsync(OpenOnFocus);
        }

        private async Task InputActivationAsync(bool openMenu)
        {
            if (SelectOnActivation)
            {
                await SelectAsync();
            }

            if (openMenu)
                await OpenListAsync();
        }

        public async Task PerformSearchAsync()
        {
            // We use this to allow pagination of the items and a More Button
            _filteredTake = MaxItems;

            // Perform filtering based on FilterType
            if (FilterType == ComboBoxFilterType.Client)
            {
                if (Filterable)
                {
                    // Filter the items based on the text
                    FilteredItems = Items
                        .Where(x => GetItemString(x)?.Contains(Text ?? string.Empty, StringComparison.CurrentCultureIgnoreCase) ?? false).ToList();
                }
                else
                {
                    // No filtering, just show all items
                    FilteredItems = Items.ToList();
                }
            }
            else if (FilterType == ComboBoxFilterType.Server)
            {
                CancelToken();
                _cancellationTokenSrc ??= new CancellationTokenSource();
                var searchText = Text ?? string.Empty;
                var searchTask = SearchFunc?.Invoke(searchText, _cancellationTokenSrc.Token);
                // User does the filtering himself via SearchFunc
                try
                {
                    FilteredItems = searchTask switch
                    {
                        null => [],
                        _ => (await searchTask).ToList()
                    };
                }
                catch (TaskCanceledException)
                {
                }
                catch (OperationCanceledException)
                {
                }
            }

            // Make sure FilteredItems updates the list
            StateHasChanged();
        }

        private void NextFilteredPage()
        {
            // if we have more items to show, increase the take to the maximum number of items
            if (_filteredTake < FilteredItems.Count)
            {
                _filteredTake += MaxItems;
            }
            StateHasChanged();
        }

        private async Task OnInputBlurredAsync() => await CloseListAsync();

        private async Task OnInputKeyDownAsync(KeyboardEventArgs args)
        {
            var open = _openItemListState.Value;
            switch (args.Key)
            {
                // We need to catch Tab here because a tab will move focus to the next element thus we'd never get the tab key in OnInputKeyUpAsync.
                case "Tab":
                    if (open)
                    {
                        if (SelectValueOnTab)
                            await OnEnterKeyAsync();
                    }
                    await CloseListAsync();
                    break;
                case "ArrowDown":
                    if (open)
                    {
                        await SelectAdjacentItemAsync(+1);
                        await ScrollToListItemAsync(_selectedComboBoxIndex);
                    }
                    else
                    {
                        await OpenListAsync();
                    }
                    break;
                case "ArrowUp":
                    if (args.AltKey)
                    {
                        await CloseListAsync();
                    }
                    else if (!open)
                    {
                        await OpenListAsync();
                    }
                    else
                    {
                        await SelectAdjacentItemAsync(-1);
                        await ScrollToListItemAsync(_selectedComboBoxIndex);
                    }
                    break;
            }

            await base.InvokeKeyDownAsync(args);
        }

        private async Task OnInputKeyUpAsync(KeyboardEventArgs args)
        {
            var open = _openItemListState.Value;
            switch (args.Key)
            {
                case "Enter":
                case "NumpadEnter":
                    if (open)
                    {
                        await OnEnterKeyAsync();
                    }
                    else
                    {
                        await OpenListAsync();
                    }
                    break;
                case "Space":
                    await ComboBoxToggleListAsync();
                    break;
                case "Escape":
                    await CloseListAsync();
                    break;
                case "Backspace":
                    if (args is { CtrlKey: true, ShiftKey: true })
                    {
                        await ResetAsync();
                    }
                    break;
            }

            await base.InvokeKeyUpAsync(args);
        }

        /// <summary>Selects the next or previous enabled item in the list and scrolls to it.</summary>
        /// <param name="direction">The direction to move, positive for down, negative for up.</param>
        private async ValueTask SelectAdjacentItemAsync(int direction)
        {
            var items = FilteredItems;
            // list of valid indices that are not disabled and less than the _filteredTake
            // _filteredTake is set by MaxItems initially and updated during performsearch
            var enabledItemIndices = items.Select((item, index) => (item, index))
                .Where(x => !ItemDisabledFunc?.Invoke(x.item) ?? true &&
                            x.index < _filteredTake)
                .Select(x => x.index)
                .ToList();

            if (items.Count == 0 || enabledItemIndices.Count == 0)
                return;

            // Get the current index among enabled items
            var currentEnabledIndex = enabledItemIndices.IndexOf(_selectedComboBoxIndex);

            // Determine the new index based on the direction
            var newEnabledIndex = currentEnabledIndex + direction;

            // open up additional items and try again if more items exist
            if (newEnabledIndex == _filteredTake && _filteredTake < items.Count)
            {
                NextFilteredPage();
                await SelectAdjacentItemAsync(direction);
            }
            // Ensure new index is in the range in the list.
            else if (newEnabledIndex >= 0 && newEnabledIndex <= enabledItemIndices.Max(x => x))
            {
                if (enabledItemIndices.Contains(newEnabledIndex))
                    _selectedComboBoxIndex = enabledItemIndices[newEnabledIndex];
                else
                    await SelectAdjacentItemAsync(direction); // if in range but not in list it's disabled go to next
            }
            else if (newEnabledIndex < 0)
            {
                _selectedComboBoxIndex = enabledItemIndices.Max(x => x);
            }
            else // start at top
            {
                _selectedComboBoxIndex = 0;
            }
        }

        private async Task OnEnterKeyAsync()
        {
            // Action that happens when the keyboard events onenter or tab with SelectValueOnTab is true
            if (!_openItemListState.Value)
            {
                await OpenListAsync();
            }
            else if (_selectedComboBoxIndex >= 0 && _selectedComboBoxIndex < FilteredItemsCount)
            {
                // toggle the item we know it's a valid index and ComboBox doesn't care if it's invalid.
                await ComboBoxToggleItem(FilteredItems[_selectedComboBoxIndex], AutoClose);
            }
        }

        /// <summary>
        /// Scrolls to the index of FilteredItems when dropdown is open
        /// </summary>
        /// <param name="index">The index of the FilteredItems to scroll to</param>
        /// <returns></returns>
        public ValueTask ScrollToListItemAsync(int index)
        {
            if (!_openItemListState.Value)
            {
                return ValueTask.CompletedTask;
            }
            var id = GetListItemId(index);

            //id of the scrolled element and scroll if not in view
            return ScrollManager.ScrollToListItemAsync(id, false);
        }

        private Task ClearButtonClickHandlerAsync()
        {
            return SetTextAsync(default, false);
        }

        internal async Task AdornmentClickHandlerAsync()
        {
            if (OnAdornmentClick.HasDelegate)
            {

                await OnAdornmentClick.InvokeAsync();
            }
            else
            {
                await ComboBoxToggleListAsync();
            }
        }

        /// <summary>
        /// Sets the focus to this component.
        /// </summary>
        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        /// <summary>
        /// Releases the focus from this component.
        /// </summary>
        public override ValueTask BlurAsync()
        {
            return _elementReference.BlurAsync();
        }

        private async Task OnFocusOutAsync(FocusEventArgs focusEventArgs)
        {
            if (_openItemListState.Value)
            {
                // when the menu is open we immediately get back the focus if we lose it (i.e. because of checkboxes in multi-select)
                // otherwise we can't receive key strokes any longer
                await FocusAsync();
            }
        }

        internal async Task AddButtonClickHandlerAsync()
        {
            await OnAddItemClick.InvokeAsync();
            await CloseListAsync();
            await SetTextAsync(default, false);
        }

        internal Task HandleMouseDown(MouseEventArgs args)
        {
            if (args.Button != 0) // if it wasn't left click drop out
                return Task.CompletedTask;
            return OpenListAsync();
        }

        private void CancelToken()
        {
            try
            {
                _cancellationTokenSrc?.Cancel();
            }
            catch { /*ignored*/ }
            finally
            {
                _cancellationTokenSrc = new CancellationTokenSource();
            }
        }

        // fires for every keystroke change
        protected Task OnInput(ChangeEventArgs? args)
        {
            return SetTextAsync(args?.Value as string);
        }

        private bool ShowClearButton => !GetDisabledState() && !GetReadOnlyState() && Clearable && Text?.Length > MinCharacters;

        private bool ShowAddButton => !GetDisabledState() && !GetReadOnlyState() && Text?.Length > MinCharacters && OnAddItemClick.HasDelegate;

        /// <summary>
        /// Returns a value for the <c>autocomplete</c> html attribute, either supplied by default or the one specified in the attribute overrides.
        /// </summary>
        protected object? GetAutocomplete() => UserAttributes.GetValueOrDefault("autocomplete", "off");

        private string GetDropDownIcon => _openItemListState.Value ? CloseIcon : OpenIcon;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (typeof(T).IsEnum)
            {
                Items = Enum.GetValues(typeof(T))
                            .Cast<T>()
                            .ToList()
                            .AsReadOnly();

                ToStringFunc = value => value?.ToString();
            }
        }

        internal void RegisterItem(MudComboBoxItem<T> item)
        {
            if (item is null || item.ComboBox != null)
                return;
            // add the item to the ComboBox
            lock (_itemsLock)
            {
                _comboBoxItems.Add(item);
                StateHasChanged();
            }
        }

        internal void UnRegisterItem(MudComboBoxItem<T> item)
        {
            if (item is null || item.ComboBox != this)
                return;
            // remove the item from the ComboBox
            lock (_itemsLock)
            {
                item.ComboBox = null;
                _comboBoxItems.Remove(item);
                StateHasChanged();
            }
        }
    }
}
