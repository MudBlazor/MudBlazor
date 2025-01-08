using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// Represents a component with simple and flexible type-ahead functionality.
    /// </summary>
    /// <typeparam name="T">The type of item to search.</typeparam>
    public partial class MudAutocomplete<T> : MudBaseInput<T>
    {
        /// <summary>
        /// We need a random id for the year items in the year list so we can scroll to the item safely in every DatePicker.
        /// </summary>
        private readonly string _componentId = Identifier.Create();

        private bool _isCleared;
        private bool _isClearing;
        private bool _isProcessingValue;
        private int _selectedListItemIndex;
        private int _elementKey = 0;
        private int _returnedItemsCount;
        private bool _open;
        private MudInput<string> _elementReference = null!;
        private CancellationTokenSource? _cancellationTokenSrc;
        private Task? _currentSearchTask;
        private Timer? _debounceTimer;
        private T[]? _items;
        private List<int> _enabledItemIndices = [];
        private Func<T?, string?>? _toStringFunc;

        [Inject]
        private IScrollManager ScrollManager { get; set; } = null!;

        protected string Classname =>
            new CssBuilder("mud-select")
                .AddClass(Class)
                .Build();

        protected string InputClassname =>
            new CssBuilder("mud-select-input")
                .AddClass(InputClass)
                .Build();

        protected string AutocompleteClassname =>
            new CssBuilder("mud-select")
                .AddClass("mud-autocomplete")
                .AddClass("mud-width-full", FullWidth)
                .AddClass("mud-autocomplete--with-progress", ShowProgressIndicator && IsLoading)
                .Build();

        protected string CircularProgressClassname =>
            new CssBuilder("progress-indicator-circular")
                .AddClass("progress-indicator-circular--with-adornment", Adornment == Adornment.End)
                .Build();

        protected string GetListItemClassname(bool isSelected) =>
            new CssBuilder()
                .AddClass("mud-selected-item mud-primary-text mud-primary-hover", isSelected)
                .AddClass(ListItemClass)
                .Build();

        /// <summary>
        /// Input's classnames, separated by space.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter]
        public string? InputClass { get; set; }

        /// <summary>
        /// The CSS classes applied to the popover.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  You can use spaces to separate multiple classes.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string? PopoverClass { get; set; }

        /// <summary>
        /// The CSS classes applied to the internal list.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  You can use spaces to separate multiple classes.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string? ListClass { get; set; }

        /// <summary>
        /// The CSS classes applied to internal list items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  You can use spaces to separate multiple classes.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string? ListItemClass { get; set; }

        /// <summary>
        /// The location where the popover will open from.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.BottomLeft" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Origin AnchorOrigin { get; set; } = Origin.BottomLeft;

        /// <summary>
        /// The transform origin point for the popover.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.TopLeft"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// Uses compact padding.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The "open" Autocomplete icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowDropDown"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string OpenIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// The "close" Autocomplete icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowDropDown"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string CloseIcon { get; set; } = Icons.Material.Filled.ArrowDropUp;

        /// <summary>
        /// The maximum height, in pixels, of the Autocomplete when it is open.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>300</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// The function used to get the display text for each item.
        /// </summary>
        /// <remarks>
        /// Defaults to the <c>ToString()</c> method of items.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T?, string?>? ToStringFunc
        {
            get => _toStringFunc;
            set
            {
                if (_toStringFunc == value)
                    return;

                _toStringFunc = value;
                Converter = new Converter<T>
                {
                    SetFunc = _toStringFunc ?? (x => x?.ToString()),
                };
            }
        }

        /// <summary>
        /// Shows the progress indicator during searches.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  The progress indicator uses the color specified in the <see cref="ProgressIndicatorColor"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ShowProgressIndicator { get; set; }

        /// <summary>
        /// The color of the progress indicator.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.  This property is used when <see cref="ShowProgressIndicator"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color ProgressIndicatorColor { get; set; } = Color.Default;

        /// <summary>
        /// The function used to search for items.
        /// </summary>
        /// <remarks>
        /// This function searches for items containing the specified <c>string</c> value, and returns items which match up to the <see cref="MaxItems"/> property.  You can use the provided <see cref="CancellationToken"/> which is marked as canceled when the user changes the search text or selects a value from the list.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<string?, CancellationToken, Task<IEnumerable<T>>?>? SearchFunc { get; set; }

        /// <summary>
        /// The maximum number of items to display.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>10</c>.  A value of <c>null</c> will display all items.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public int? MaxItems { get; set; } = 10;

        /// <summary>
        /// The minimum number of characters typed to initiate a search.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int MinCharacters { get; set; } = 0;

        /// <summary>
        /// Reset the selected value if the user deletes the text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ResetValueOnEmptyText { get; set; }

        /// <summary>
        /// Highlights the text when the component receives focus.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// Previously known as <c>SelectOnClick</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool SelectOnActivation { get; set; } = true;

        /// <summary>
        /// Selects items without resetting the <c>Value</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>true</c>, selecting an option will trigger a <see cref="SearchFunc"/> with the current Text.  Otherwise, an empty string is passed which can make it easier to view and select other options without resetting the Value. When <c>false</c>, <c>T</c> must either be a <c>record</c> or override the <c>GetHashCode</c> and <c>Equals</c> methods.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Strict { get; set; } = true;

        /// <summary>
        /// The debounce interval, in milliseconds.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>100</c>.  A higher value can help reduce the number of calls to <see cref="SearchFunc"/>, which can improve responsiveness.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int DebounceInterval { get; set; } = 100;

        /// <summary>
        /// The custom template used to display unselected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Use the <see cref="ItemSelectedTemplate"/> property to control the display of selected items.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<T>? ItemTemplate { get; set; }

        /// <summary>
        /// The custom template used to display selected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Use the <see cref="ItemTemplate"/> property to control the display of unselected items.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<T>? ItemSelectedTemplate { get; set; }

        /// <summary>
        /// The custom template used to display disabled items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<T>? ItemDisabledTemplate { get; set; }

        /// <summary>
        /// The custom template used when the number of items returned by <see cref="SearchFunc"/> is more than the value of the <see cref="MaxItems"/> property.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment? MoreItemsTemplate { get; set; }

        /// <summary>
        /// The custom template used when no items are returned by <see cref="SearchFunc"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment? NoItemsTemplate { get; set; }

        /// <summary>
        /// The custom template shown above the list of items, if <see cref="SearchFunc"/> returns items to display.  Otherwise, the fragment is hidden.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Use the <see cref="AfterItemsTemplate"/> property to control content displayed below items.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment? BeforeItemsTemplate { get; set; }

        /// <summary>
        /// The custom template shown below the list of items, if <see cref="SearchFunc"/> returns items to display.  Otherwise, the fragment is hidden.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Use the <see cref="BeforeItemsTemplate"/> property to control content displayed above items.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment? AfterItemsTemplate { get; set; }

        /// <summary>
        /// The custom template used for the progress indicator when <see cref="ShowProgressIndicator"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Use the <see cref="ProgressIndicatorInPopoverTemplate"/> property to control content displayed for the progress indicator inside the popover.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment? ProgressIndicatorTemplate { get; set; }

        /// <summary>
        /// The custom template used for the progress indicator inside the popover when <see cref="ShowProgressIndicator"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Use the <see cref="ProgressIndicatorTemplate"/> property to control content displayed for the progress indicator.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment? ProgressIndicatorInPopoverTemplate { get; set; }

        /// <summary>
        /// Overrides the <c>Text</c> property when an item is selected.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>true</c>, selecting a value will update the Text property.  When <c>false</c>, incomplete values for Text are allowed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool CoerceText { get; set; } = true;

        /// <summary>
        /// Sets the <c>Value</c> property even if no match is found by <see cref="SearchFunc"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the user input will be applied to the Value property which allows it to be validated and show an error message.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool CoerceValue { get; set; }

        /// <summary>
        /// The behavior of the dropdown popover menu
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DropdownSettings.Fixed" /> false
        /// Defaults to <see cref="DropdownSettings.OverflowBehavior" /> <see cref="OverflowBehavior.FlipOnOpen" />
        /// </remarks>
        [Category(CategoryTypes.Popover.Behavior)]
        [Parameter]
        public DropdownSettings DropdownSettings { get; set; }

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
        /// Occurs when the <see cref="Open"/> property has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> OpenChanged { get; set; }

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
        /// Additionally, opens the list when focus is received on the input element; otherwise only opens on click.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool OpenOnFocus { get; set; } = true;

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
        /// Custom clear icon when <see cref="Clearable"/> is enabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string ClearIcon { get; set; } = Icons.Material.Filled.Clear;

        /// <summary>
        /// Occurs when the Clear button has been clicked.
        /// </summary>
        /// <remarks>
        /// The Text and Value properties will be blank when this callback occurs.
        /// </remarks>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// Occurs when the number of items returned by <see cref="SearchFunc"/> has changed.
        /// </summary>
        /// <remarks>
        /// The number of items returned determines when custom templates are shown.  If the number is <c>0</c>, <see cref="NoItemsTemplate"/> will be shown. If the number is beyond <see cref="MaxItems"/>, <see cref="MoreItemsTemplate"/> will be shown.
        /// </remarks>
        [Parameter]
        public EventCallback<int> ReturnedItemsCountChanged { get; set; }

        /// <summary>
        /// Displays the search result drop-down.
        /// </summary>
        /// <remarks>
        /// When this property changes, the <see cref="OpenChanged"/> event will occur.
        /// </remarks>
        public bool Open
        {
            get => _open;
            // Note: the setter is protected because it was needed by a user who derived his own autocomplete from this class.
            // Note: setting Open will not open or close it. Use ToggleMenuAsync() for that.
            protected set
            {
                if (_open == value)
                    return;
                _open = value;

                OpenChanged.InvokeAsync(_open).CatchAndLog();
            }
        }

        private bool IsLoading => _currentSearchTask is { IsCompleted: false };

        private string CurrentIcon => !string.IsNullOrWhiteSpace(AdornmentIcon) ? AdornmentIcon : _open ? CloseIcon : OpenIcon;

        /// <summary>
        /// Returns a value for the <c>autocomplete</c> attribute, either supplied by default or the one specified in the attribute overrides.
        /// </summary>
        protected object? GetAutocomplete() => UserAttributes.GetValueOrDefault("autocomplete", "off");

        public MudAutocomplete()
        {
            Adornment = Adornment.End;
            IconSize = Size.Medium;
            Immediate = true;
        }

        /// <summary>
        /// Changes the currently selected item to the specified value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public async Task SelectOptionAsync(T value)
        {
            _isProcessingValue = true;
            try
            {
                await SetValueAsync(value);

                if (_items != null)
                    _selectedListItemIndex = Array.IndexOf(_items, value);

                var optionText = GetItemString(value);

                if (!_isCleared)
                    await SetTextAsync(optionText, false);

                _debounceTimer?.Dispose();

                await BeginValidateAsync();

                if (!_isCleared)
                {
                    await _elementReference.SetText(optionText);
                }

                await FocusAsync();

                Open = false;

                StateHasChanged();
            }
            finally
            {
                _isProcessingValue = false;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var text = GetItemString(Value);
            if (!string.IsNullOrWhiteSpace(text))
                Text = text;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (_isClearing || _isProcessingValue)
            {
                //When you select a value in the popover, SelectOptionAsync will be called.
                //When it reaches SetValueAsync, it will be awaited.
                //Meanwhile, in parallel, the ClearAsync method will be called, which sets isCleared to true.
                //However, by the time SetValueAsync is released and SelectOptionAsync continues its execution, an OnAfterRender event might fire, setting isCleared back to false.
                //This can result in a race condition.
                //https://github.com/MudBlazor/MudBlazor/pull/6701
                base.OnAfterRender(firstRender);
                return;
            }

            _isCleared = false;

            base.OnAfterRender(firstRender);
        }

        protected override Task UpdateTextPropertyAsync(bool updateValue)
        {
            // This keeps the text from being set when ClearAsync() was called
            if (_isCleared)
                return Task.CompletedTask;

            return base.UpdateTextPropertyAsync(updateValue);
        }

        protected override async Task UpdateValuePropertyAsync(bool updateText)
        {
            _debounceTimer?.Dispose();

            if (ResetValueOnEmptyText && string.IsNullOrWhiteSpace(Text))
                await SetValueAsync(default(T), updateText);
            else if (Immediate)
                await CoerceValueToTextAsync();

            if (DebounceInterval <= 0)
                await OpenMenuAsync();
            else
                _debounceTimer = new Timer(OnDebounceComplete, null, DebounceInterval, Timeout.Infinite);
        }

        private void OnDebounceComplete(object? stateInfo) => InvokeAsync(OpenMenuAsync);

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

        private Task SetReturnedItemsCountAsync(int value)
        {
            _returnedItemsCount = value;
            return ReturnedItemsCountChanged.InvokeAsync(value);
        }

        /// <summary>
        /// Opens or closes the drop-down of items depending on whether it was closed or open.
        /// </summary>
        /// <remarks>
        /// Will have no effect if the autocomplete is disabled or read-only.
        /// </remarks>
        public Task ToggleMenuAsync()
        {
            if (!Open && (GetDisabledState() || GetReadOnlyState()))
            {
                return Task.CompletedTask;
            }

            return Open ? CloseMenuAsync() : OpenMenuAsync();
        }

        /// <summary>
        /// Closes the drop-down of items.
        /// </summary>
        public async Task CloseMenuAsync()
        {
            CancelToken();
            _debounceTimer?.Dispose();
            await RestoreScrollPositionAsync();
            await CoerceTextToValueAsync();
            Open = false;
            StateHasChanged();
        }

        /// <summary>
        /// Opens the drop-down of items.
        /// </summary>
        /// <remarks>
        /// Will have no effect if the autocomplete is disabled or read-only.
        /// </remarks>
        public async Task OpenMenuAsync()
        {
            if (MinCharacters > 0 && (string.IsNullOrWhiteSpace(Text) || Text.Length < MinCharacters))
            {
                Open = false;
                StateHasChanged();
                return;
            }

            var searchedItems = Array.Empty<T>();
            CancelToken();

            var wasFocused = _isFocused;
            var searchingWhileSelected = false;
            try
            {
                if (ProgressIndicatorInPopoverTemplate != null)
                {
                    // Open before searching if a progress indicator is defined.
                    Open = true;
                }

                // Search while selected if enabled and the Text is equivalent to the Value.
                searchingWhileSelected = !Strict && Value != null && (Value.ToString() == Text || (ToStringFunc != null && ToStringFunc(Value) == Text));
                _cancellationTokenSrc ??= new CancellationTokenSource();
                var searchText = searchingWhileSelected ? string.Empty : Text;
                var searchTask = SearchFunc?.Invoke(searchText, _cancellationTokenSrc.Token);

                _currentSearchTask = searchTask;

                StateHasChanged();
                searchedItems = searchTask switch
                {
                    null => [],
                    _ => (await searchTask).ToArray()
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
                Logger.LogWarning("The search function failed to return results: " + e.Message);
            }

            await SetReturnedItemsCountAsync(searchedItems.Length);

            if (MaxItems.HasValue)
            {
                searchedItems = searchedItems.Take(MaxItems.Value).ToArray();
            }

            _items = searchedItems;

            var enabledItems = _items.Select((item, idx) => (item, idx)).Where(tuple => ItemDisabledFunc?.Invoke(tuple.item) != true).ToList();
            _enabledItemIndices = enabledItems.Select(tuple => tuple.idx).ToList();
            if (searchingWhileSelected) //compute the index of the currently select value, if it exists
            {
                _selectedListItemIndex = Array.IndexOf(_items, Value);
            }
            else
            {
                _selectedListItemIndex = _enabledItemIndices.Any() ? _enabledItemIndices[0] : -1;
            }

            if (_isFocused || !wasFocused)
            {
                // Open after the search has finished if we're still focused (UI), or were never focused in the first place (programmatically).
                Open = true;
            }

            StateHasChanged();
        }

        /// <summary>
        /// Resets the Text and Value, and closes the drop-down if it is open.
        /// </summary>
        public async Task ClearAsync()
        {
            _isClearing = true;
            try
            {
                _isCleared = true;
                Open = false;

                await SetTextAsync("", updateValue: false);
                await SetValueAsync(default(T), updateText: false);

                await _elementReference.ResetAsync();

                _debounceTimer?.Dispose();
                StateHasChanged();
            }
            finally
            {
                _isClearing = false;
            }
        }

        protected override Task ResetValueAsync() => ClearAsync();

        private string? GetItemString(T? item)
        {
            if (item is null)
            {
                return string.Empty;
            }

            try
            {
                return Converter.Set(item);
            }
            catch (NullReferenceException)
            {
                // ignore
            }

            return "null";
        }

        private async Task OnInputKeyDownAsync(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                // We need to catch Tab here because a tab will move focus to the next element and thus we'd never get the tab key in OnInputKeyUpAsync.
                case "Tab":
                    if (Open)
                    {
                        if (SelectValueOnTab)
                            await OnEnterKeyAsync();
                    }
                    await CloseMenuAsync();
                    break;
                case "ArrowDown":
                    if (Open)
                    {
                        await SelectAdjacentItemAsync(+1);
                    }
                    else
                    {
                        await OpenMenuAsync();
                    }
                    break;
                case "ArrowUp":
                    if (args.AltKey)
                    {
                        await CloseMenuAsync();
                    }
                    else if (!Open)
                    {
                        await OpenMenuAsync();
                    }
                    else
                    {
                        await SelectAdjacentItemAsync(-1);
                    }
                    break;
            }

            await base.InvokeKeyDownAsync(args);
        }

        private async Task OnInputKeyUpAsync(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Enter":
                case "NumpadEnter":
                    if (Open)
                    {
                        await OnEnterKeyAsync();
                    }
                    else
                    {
                        await OpenMenuAsync();
                    }
                    break;
                case "Escape":
                    await CloseMenuAsync();
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
        private ValueTask SelectAdjacentItemAsync(int direction)
        {
            if (_items == null || _items.Length == 0 || !_enabledItemIndices.Any())
                return ValueTask.CompletedTask;

            // Get the current index among enabled items
            var currentEnabledIndex = _enabledItemIndices.IndexOf(_selectedListItemIndex);

            // Determine the new index based on the direction
            var newEnabledIndex = currentEnabledIndex + direction;

            // Ensure new index is within bounds
            if (newEnabledIndex >= 0 && newEnabledIndex < _enabledItemIndices.Count)
            {
                _selectedListItemIndex = _enabledItemIndices[newEnabledIndex];
                return SelectItemAsync(_selectedListItemIndex);
            }

            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Selects the item in the list at the specified index and scrolls to it.
        /// </summary>
        /// <param name="index">The index of the item to scroll to. If it's out of range then nothing will happen.</param>
        private ValueTask SelectItemAsync(int index)
        {
            if (_items == null || _items.Length == 0 || !_enabledItemIndices.Any() || index < 0 || index > _enabledItemIndices.Count - 1)
                return ValueTask.CompletedTask;

            _selectedListItemIndex = index;

            var id = GetListItemId(index);

            return ScrollManager.ScrollToListItemAsync(id);
        }

        /// <summary>
        /// This restores the scroll position after closing the menu and element being 0
        /// </summary>
        private ValueTask RestoreScrollPositionAsync()
        {
            if (_selectedListItemIndex != 0)
                return ValueTask.CompletedTask;

            return ScrollManager.ScrollToListItemAsync(GetListItemId(0));
        }

        private string GetListItemId(in int index)
        {
            return $"{_componentId}_item{index}";
        }

        internal async Task OnEnterKeyAsync()
        {
            if (!Open || _items == null || _items.Length == 0)
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
                if (_selectedListItemIndex >= 0 && _selectedListItemIndex < _items.Length)
                    await SelectOptionAsync(_items[_selectedListItemIndex]);
            }
            finally
            {
                if (Open)
                    Open = false;
            }
        }

        private Task OnInputClickedAsync() => OnInputActivationAsync(true);

        private Task OnInputFocusedAsync() => OnInputActivationAsync(OpenOnFocus);

        private async Task OnInputActivationAsync(bool openMenu)
        {
            _isFocused = true;

            if (Open || GetDisabledState() || GetReadOnlyState())
            {
                return;
            }

            if (SelectOnActivation)
            {
                await SelectAsync();
            }

            if (openMenu)
            {
                await OpenMenuAsync();
            }
        }

        internal async Task AdornmentClickHandlerAsync()
        {
            if (OnAdornmentClick.HasDelegate)
            {
                await FocusAsync();
                await OnAdornmentClick.InvokeAsync();
            }
            else
            {
                await ToggleMenuAsync();
            }
        }

        private Task OnInputBlurredAsync(FocusEventArgs args)
        {
            _isFocused = false;

            // When Immediate is enabled, then the CoerceValue is set by TextChanged
            // So only coerce the value on blur when Immediate is disabled
            if (!Immediate)
            {
                return CoerceValueToTextAsync();
            }

            return OnBlur.InvokeAsync(args);
            // we should not validate on blur in autocomplete, because the user needs to click out of the input to select a value,
            // resulting in a premature validation. thus, don't call base
            //base.OnBlurred(args);
        }

        private Task OnOverlayClosedAsync()
        {
            if (Open)
            {
                return CloseMenuAsync();
            }

            return Task.CompletedTask;
        }

        private Task CoerceTextToValueAsync()
        {
            if (!CoerceText)
                return Task.CompletedTask;

            if (ResetValueOnEmptyText && string.IsNullOrEmpty(Text))
                return Task.CompletedTask;

            _debounceTimer?.Dispose();

            var text = Value == null ? null : GetItemString(Value);

            // Don't update the value to prevent the popover from opening again after coercion
            if (text != Text)
                return SetTextAsync(text, updateValue: false);

            return Task.CompletedTask;
        }

        private Task CoerceValueToTextAsync()
        {
            if (!CoerceValue)
                return Task.CompletedTask;

            _debounceTimer?.Dispose();

            var value = Converter.Get(Text);
            return SetValueAsync(value, updateText: false);
        }

        /// <inheritdoc />
        protected override async ValueTask DisposeAsyncCore()
        {
            if (_debounceTimer is not null)
            {
                await _debounceTimer.DisposeAsync();
            }

            if (_cancellationTokenSrc is not null)
            {
                try
                {
                    await _cancellationTokenSrc.CancelAsync();
                }
                catch { /*ignored*/ }
                try
                {
                    _cancellationTokenSrc.Dispose();
                }
                catch { /*ignored*/ }
            }

            await base.DisposeAsyncCore();
        }

        /// <summary>
        /// Sets focus to this Autocomplete.
        /// </summary>
        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        /// <summary>
        /// Releases focus from this Autocomplete.
        /// </summary>
        public override ValueTask BlurAsync()
        {
            return _elementReference.BlurAsync();
        }

        /// <summary>
        /// Selects all the current text within the Autocomplete text box.
        /// </summary>
        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        /// <summary>
        /// Selects a portion of the text within the Autocomplete text box.
        /// </summary>
        /// <param name="pos1">The index of the first character to select.</param>
        /// <param name="pos2">The index of the last character to select.</param>
        /// <returns>A <see cref="ValueTask"/> object.</returns>
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        private async Task OnTextChangedAsync(string? text)
        {
            await base.TextChanged.InvokeAsync(text);

            if (text == null)
                return;

            await SetTextAsync(text, true);
        }

        private Task ListItemOnClickAsync(T item) => SelectOptionAsync(item);
    }
}
