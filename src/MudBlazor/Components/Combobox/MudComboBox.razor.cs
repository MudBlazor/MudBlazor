using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using MudBlazor.Components.Combobox;
using MudBlazor.State;
using MudBlazor.Utilities;
#nullable enable
namespace MudBlazor
{
    public partial class MudComboBox<T> : MudBaseInput<T>
    {
        private int _selectedComboBoxIndex = -1;
        private int _elementKey = 0;
        private int _filteredTake = 0;
        private bool _activatorEvents;
        private readonly string _componentId = Identifier.Create();
        private int _maxItems = 10;

        private ParameterState<HashSet<T>> _selectedItemsState;
        private ParameterState<bool> _openItemListState;
        private ParameterState<bool> _isLoadingState;

        private CancellationTokenSource? _cancellationTokenSrc;
        private Timer? _debounceTimer;

        private MudInput<string> _elementReference = null!;

        public MudComboBox()
        {
            // default values, can be overridden
            Adornment = Adornment.End;
            IconSize = Size.Medium;
            Immediate = true;

            using var registerScope = CreateRegisterScope();
            _selectedItemsState = registerScope.RegisterParameter<HashSet<T>>(nameof(SelectedItems))
                .WithParameter(() => SelectedItems)
                .WithEventCallback(() => SelectedItemsChanged);
            _openItemListState = registerScope.RegisterParameter<bool>(nameof(OpenItemList))
                .WithParameter(() => OpenItemList)
                .WithEventCallback(() => OpenItemListChanged);
            _isLoadingState = registerScope.RegisterParameter<bool>(nameof(IsLoading))
                .WithParameter(() => IsLoading)
                .WithEventCallback(() => IsLoadingChanged);
        }

        [Inject]
        private InternalMudLocalizer Localizer { get; set; } = null!;

        protected string Classname => new CssBuilder("mud-select")
            .AddClass("mud-combobox")
            .AddClass(Class)
            .Build();

        protected string ComboBoxClassname =>
            new CssBuilder("mud-select")
                .AddClass("mud-combobox")
                .AddClass("mud-width-full", FullWidth)
                .AddClass("mud-autocomplete--with-progress", ShowProgressIndicator && IsLoading)
                .Build();

        protected string InputClassname => new CssBuilder("mud-select-input")
            .AddClass("mud-combobox-input")
            .AddClass(InputClass)
            .Build();
        protected string CircularProgressClassname =>
            new CssBuilder("progress-indicator-circular")
                .AddClass("progress-indicator-circular--with-adornment", Adornment == Adornment.End)
                .Build();

        /// <summary>
        /// Wether Right to Left is designated by the parent
        /// </summary>
        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; } = false;

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

        // hidden public Variant Variant { get; set; } = Variant.Text;

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
        /// Uses compact padding, including the search items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c> or <c>MudGlobal.InputDefaults.Margin</c>.
        /// </remarks>
        [Parameter]
        public bool Dense { get; set; }

        /// <summary>
        /// Margin to be applied to the component, make internal.
        /// </summary>
        internal new Margin Margin => Dense ? Margin.Dense : Margin.None;

        /// <summary>
        /// Updates the Value to the currently selected item when pressing the Tab key.
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
        /// Whether or not the ComboBox uses an overlay when the dropdown is active.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool Overlay { get; set; } = true;

        #endregion

        /// <summary>
        /// Displays the Clear icon button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, an icon is displayed which, when clicked, clears the Text and Value.  Use the <c>ClearIcon</c> property to control the Clear button icon.
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
        public string ClearIcon { get; set; } = Icons.Material.Filled.Clear;

        /// <summary>
        /// The "add" Combobox icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.AddCircle"/>.
        /// </remarks>
        [Parameter]
        public string AddIcon { get; set; } = Icons.Material.Filled.AddCircle;

        /// <summary>
        /// When <c>true</c> an AddIcon is displayed when custom input does not have an exact match. 
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>
        /// </remarks>
        [Parameter]
        public bool CustomInput { get; set; }

        /// <summary>
        /// Sets the point at which the list becomes a BottomSheet encompassing the entire bottom (or top) of the presumed mobile display.
        /// <para>--TODO--</para>
        /// </summary>
        [Parameter]
        public Breakpoint? SmallScreens { get; set; } = Breakpoint.SmAndDown;

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
        /// Defaults to <c>false</c>.  The progress indicator uses the color specified in the <see cref="ProgressIndicatorColor"/> property.
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
        public Func<string?, CancellationToken, Task<IEnumerable<T>>?>? SearchFunc { get; set; }

        /// <summary>
        /// The maximum number of items to display.
        /// </summary>
        /// <remarks>
        /// <para>Defaults to <c>10</c>. A value of 0 will display all items.</para>
        /// <para>Value cannot be less than 0</para>
        /// </remarks>
        [Parameter]
        public int MaxItems
        {
            get => _maxItems;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(MaxItems), "Value cannot be less than 0.");
                _maxItems = value;
            }
        }

        /// <summary>
        /// The minimum number of characters typed to initiate a search.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.
        /// </remarks>
        [Parameter]
        public int MinCharacters { get; set; } = 0;

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
        /// The template used to display all the items in the PopoverList, contains a context of <c>ComboBoxItem<typeparamref name="T"/></c>
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<ComboBoxItem<T>>? ItemTemplate { get; set; }

        /// <summary>
        /// The template used to display selected items in the textbox area. When <c>Filterable</c> is <c>true</c> the template is shown under the input.
        /// </summary>
        [Parameter]
        public RenderFragment<ComboBoxItem<T>>? SelectedItemsTemplate { get; set; }

        /// <summary>
        /// Determines whether the <c>Text</c>> property should be automatically adjusted to match a valid selection from the available options.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>. When <c>true</c>, selecting an item from the dropdown updates the <c>Text</c> property to match.
        /// If the user types an input that does not match any option and presses Enter, the text remains unchanged.
        /// When set to <c>false</c>, the <c>Text</c> property can hold any user input, even if it does not correspond to a valid selection.
        /// </remarks>
        /// <para>e.g. If the available options are "Apple", "Banana", and "Cherry", and the user types "xyz" and presses Enter, the input will remain "xyz" if <c>false</c>, but will be cleared or corrected if <c>true</c>.</para>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool CoerceText { get; set; } = true;

        /// <summary>
        /// Controls whether the <c>Value</c> property is updated based on user input, even if the input does not match any option in the list.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When set to <c>true</c>, the <c>Value</c> property is updated whenever the <c>Text</c> changes,
        /// even if the text does not match an available option.
        /// This allows the component to validate and display errors for user-entered values that are not part of the predefined options.
        /// </remarks>
        /// <para>e.g. If the user types "xyz" and presses Enter, <c>Value</c> will be set to "xyz" if <c>true</c>, but will remain unchanged if <c>false</c>.</para>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool CoerceValue { get; set; }


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
        /// Event is fired when the selected items change
        /// </summary>
        [Parameter]
        public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        /// <summary>
        /// Sets the filter type, Client filters based on ComboBox Items, Server expects user to update ComboBoxItems via the Search function.
        /// Default is Client
        /// </summary>
        [Parameter]
        public ComboBoxFilterType FilterType { get; set; } = ComboBoxFilterType.Client;

        [Parameter]
        public bool OpenOnEnter { get; set; }

        [Parameter]
        public bool OpenItemList { get; set; }

        [Parameter]
        public EventCallback<bool> OpenItemListChanged { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }
        [Parameter]
        public EventCallback<bool> IsLoadingChanged { get; set; }

        /// <summary>
        /// The list of items filtered
        /// </summary>
        public List<T> FilteredItems { get; private set; } = [];

        /// <summary>
        /// The number of items currently filtered
        /// </summary>
        public int FilteredItemsCount { get => FilteredItems.Count; }

        /// <summary>
        /// The list of items to display in the ComboBox, the ToString method is used for display
        /// </summary>
        [Parameter]
        public IEnumerable<T> Items { get; set; } = [];

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
                // ignore
            }

            return "null";
        }

        private string GetListItemId(in int index)
        {
            return $"{_componentId}_item{index}";
        }

        public Task ComboBoxToggleItem(T item)
        {
            if (item == null)
                return Task.CompletedTask;

            // Toggle SelectedItems to Add if it doesn't exist, remove it if it does.
            var selectedItems = _selectedItemsState.Value ?? [];
            if (!MultiSelection)
            {
                selectedItems.Clear();
            }
            if (!selectedItems.Remove(item))
            {
                selectedItems.Add(item);
            }
            return _selectedItemsState.SetValueAsync(selectedItems);
        }

        public async Task OpenListAsync()
        {
            if (_openItemListState.Value || GetReadOnlyState() || GetDisabledState())
                return;

            await _openItemListState.SetValueAsync(true);
            await PerformSearchAsync();
            StateHasChanged();
        }

        public async Task CloseListAsync()
        {
            CancelToken();
            _debounceTimer?.Dispose();
            //await RestoreScrollPositionAsync();
            //await CoerceTextToValueAsync();
            await _openItemListState.SetValueAsync(false);
            StateHasChanged();
        }

        public async Task OnEnterKeyAsync()
        {
            if (!_openItemListState.Value || FilteredItems.Count == 0)
            {
                // When Immediate is enabled, then the CoerceValue is set by TextChanged
                // So only coerce the value on enter when Immediate is disabled
                if (!Immediate)
                {
                    await CoerceValueToTextAsync();
                }
                return;
            }

            try
            {
                if (_selectedComboBoxIndex >= 0 && _selectedComboBoxIndex < FilteredItems.Count)
                    await SelectOptionAsync(FilteredItems[_selectedComboBoxIndex]);
            }
            finally
            {
                await CloseListAsync();
            }
        }

        private Task CoerceValueToTextAsync()
        {
            if (!CoerceValue)
                return Task.CompletedTask;

            _debounceTimer?.Dispose();

            var value = Converter.Get(Text);
            return SetValueAsync(value, updateText: false);
        }

        public async Task SelectOptionAsync(T value)
        {
            try
            {
                await SetValueAsync(value);

                _selectedComboBoxIndex = FilteredItems.IndexOf(value);

                var optionText = GetItemString(value);

                await SetTextAsync(optionText, false);

                _debounceTimer?.Dispose();

                await BeginValidateAsync();

                await _elementReference.SetText(optionText);

                await FocusAsync();
                // We want focus with a closed popover
                await CloseListAsync();

            }
            finally
            {
                // And update
                StateHasChanged();
            }
        }

        /// <summary>
        /// Returns a value for the <c>autocomplete</c> html attribute, either supplied by default or the one specified in the attribute overrides.
        /// </summary>
        protected object? GetAutocomplete() => UserAttributes.GetValueOrDefault("autocomplete", "off");

        private string GetDropDownIcon => _openItemListState.Value ? CloseIcon : OpenIcon;

        protected override async Task UpdateValuePropertyAsync(bool updateText)
        {
            _debounceTimer?.Dispose();

            if (ResetValueOnEmptyText && string.IsNullOrWhiteSpace(Text))
                await SetValueAsync(default(T), updateText);
            else if (Immediate)
                await CoerceValueToTextAsync();

            if (DebounceInterval <= 0)
                await PerformSearchAsync();
            else
                _debounceTimer = new Timer(OnDebounceComplete, null, DebounceInterval, Timeout.Infinite);
        }

        private void OnDebounceComplete(object? stateInfo) => InvokeAsync(PerformSearchAsync);

        private async Task OnTextChangedAsync(string? text)
        {
            await base.TextChanged.InvokeAsync(text);
            
            if (text is null) return;

            await SetTextAsync(text);
        }

        private async Task OnInputClickedAsync()
        {
            // this fires at nearly the same time as oninputfocused when both fire together
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

            await PerformSearchAsync();
        }

        private async Task PerformSearchAsync()
        {
            // We use this to allow pagination of the items and a More Button
            _filteredTake = MaxItems;

            // Perform filtering based on FilterType
            if (FilterType == ComboBoxFilterType.Client)
            {
                if (Filterable)
                {
                    FilteredItems = Items.ToList();
                    StateHasChanged();
                    return;
                }
                // We expect user's ToStringFunc or .ToString method to work with Contains combined in GetItemString method
                FilteredItems = Items
                    .Where(x => GetItemString(x)?.Contains(Text ?? string.Empty, StringComparison.CurrentCultureIgnoreCase) ?? false).ToList();
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
                catch (Exception e)
                {
                    Console.WriteLine($"The search function failed to return results: {e.Message}");
                }
            }

            // Make sure FilterdItems updates the list
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
            var _open = _openItemListState.Value;
            switch (args.Key)
            {
                // We need to catch Tab here because a tab will move focus to the next element and thus we'd never get the tab key in OnInputKeyUpAsync.
                case "Tab":
                    if (_open)
                    {
                        if (SelectValueOnTab)
                            await OnEnterKeyAsync();
                    }
                    await CloseListAsync();
                    break;
                case "ArrowDown":
                    if (_open)
                    {
                        await SelectAdjacentItemAsync(+1);
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
                    else if (!_open)
                    {
                        await OpenListAsync();
                    }
                    else
                    {
                        await SelectAdjacentItemAsync(-1);
                    }
                    break;
            }

            await base.InvokeKeyDownAsync(args);
        }

        private async Task AdornmentClickHandlerAsync(MouseEventArgs args)
        {
            if (_openItemListState.Value)
            {
                await CloseListAsync();
            }
            else
            {
                await OpenListAsync();
            }
        }

        private async Task OnInputKeyUpAsync(KeyboardEventArgs args)
        {
            var _open = _openItemListState.Value;
            switch (args.Key)
            {
                case "Enter":
                case "NumpadEnter":
                    if (_open)
                    {
                        await OnEnterKeyAsync();
                    }
                    else
                    {
                        await OpenListAsync();
                    }
                    break;
                case "Escape":
                    await CloseListAsync();
                    break;
                case "Backspace":
                    if (args.CtrlKey && args.ShiftKey)
                    {
                        await ResetAsync();
                    }
                    break;
            }

            await base.InvokeKeyUpAsync(args);
        }

        /// <summary>
        /// Selects the next or previous enabled item in the list and scrolls to it.
        /// </summary>
        /// <param name="direction">The direction to move, positive for down, negative for up.</param>
        private async ValueTask SelectAdjacentItemAsync(int direction)
        {
            var _items = FilteredItems;
            var _enabledItemIndices = _items.Select((item, index) => (item, index))
                .Where(x => !ItemDisabledFunc?.Invoke(x.item) ?? true)
                .Select(x => x.index)
                .ToList();

            if (_items == null || _items.Count == 0 || _enabledItemIndices.Count == 0)
                await ValueTask.CompletedTask;

            // Get the current index among enabled items
            var currentEnabledIndex = _enabledItemIndices.IndexOf(_selectedComboBoxIndex);

            // Determine the new index based on the direction
            var newEnabledIndex = currentEnabledIndex + direction;

            // Ensure new index is within bounds
            if (newEnabledIndex >= 0 && newEnabledIndex < _enabledItemIndices.Count)
            {
                _selectedComboBoxIndex = _enabledItemIndices[newEnabledIndex];
                await ComboBoxToggleItem(FilteredItems[_selectedComboBoxIndex]);
            }
        }

        private bool ShowClearButton()
        {
            if (GetDisabledState())
            {
                return false;
            }

            if (!Clearable)
            {
                return false;
            }

            // If this is a standalone input it will not be clearable when read-only
            if (SubscribeToParentForm && GetReadOnlyState())
            {
                return false;
            }

            if (Value is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue);
            }

            return Value is not string and not null;
        }

        private async Task HandleClearButtonAsync()
        {
            await Task.CompletedTask;
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
    }
}
