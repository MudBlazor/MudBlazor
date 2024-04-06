﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudAutocomplete<T> : MudBaseInput<T>
    {
        /// <summary>
        /// We need a random id for the year items in the year list so we can scroll to the item safely in every DatePicker.
        /// </summary>
        private readonly string _componentId = Guid.NewGuid().ToString();

        /// <summary>
        /// This boolean will keep track if the clear function is called too keep the set text function to be called.
        /// </summary>
        private bool _isCleared;
        private bool _isClearing;
        private bool _isProcessingValue;
        private int _selectedListItemIndex = 0;
        private int _elementKey = 0;
        private int _returnedItemsCount;
        private bool _isOpen;
        private MudInput<string> _elementReference;
        private CancellationTokenSource _cancellationTokenSrc;
        private Task _currentSearchTask;
        private Timer _timer;
        private T[] _items;
        private IList<int> _enabledItemIndices = new List<int>();
        private Func<T, string> _toStringFunc;

        [Inject]
        private IScrollManager ScrollManager { get; set; }

        protected string Classname =>
            new CssBuilder("mud-select")
            .AddClass(Class)
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
        /// User class names for the popover, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string PopoverClass { get; set; }

        /// <summary>
        /// User class names for the internal list, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string ListClass { get; set; }

        /// <summary>
        /// User class names for the internal list item, separated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string ListItemClass { get; set; }

        /// <summary>
        /// Set the anchor origin point to determen where the popover will open from.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Origin AnchorOrigin { get; set; } = Origin.BottomCenter;

        /// <summary>
        /// Sets the transform origin point for the popover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopCenter;

        /// <summary>
        /// If true, compact vertical padding will be applied to all Autocomplete items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The Open Autocomplete Icon
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string OpenIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// The Close Autocomplete Icon
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string CloseIcon { get; set; } = Icons.Material.Filled.ArrowDropUp;

        /// <summary>
        /// The maximum height of the Autocomplete when it is open.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// Defines how values are displayed in the drop-down list
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T, string> ToStringFunc
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
        /// Whether to show the progress indicator. 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ShowProgressIndicator { get; set; } = false;

        /// <summary>
        /// The color of the progress indicator. 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color ProgressIndicatorColor { get; set; } = Color.Default;

        /// <summary>
        /// Func that returns a list of items matching the typed text. Provides a cancellation token that
        /// is marked as cancelled when the user changes the search text or selects a value from the list. 
        /// This can be used to cancel expensive asynchronous work occuring within the SearchFunc itself.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<string, CancellationToken, Task<IEnumerable<T>>> SearchFunc { get; set; }

        /// <summary>
        /// Maximum items to display, defaults to 10.
        /// A null value will display all items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public int? MaxItems { get; set; } = 10;

        /// <summary>
        /// Minimum characters to initiate a search
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int MinCharacters { get; set; } = 0;

        /// <summary>
        /// Reset value if user deletes the text
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ResetValueOnEmptyText { get; set; } = false;

        /// <summary>
        /// If true, clicking the text field will select (highlight) its contents.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool SelectOnClick { get; set; } = true;

        /// <summary>
        /// If false, clicking on the Autocomplete after selecting an option will query the Search method again with an empty string. This makes it easier to view and select other options without resetting the Value.
        /// T must either be a record or override GetHashCode and Equals.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Strict { get; set; } = true;

        /// <summary>
        /// Debounce interval in milliseconds.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int DebounceInterval { get; set; } = 100;

        /// <summary>
        /// Optional presentation template for unselected items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<T> ItemTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for the selected item
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<T> ItemSelectedTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for disabled item
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<T> ItemDisabledTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for when more items were returned from the Search function than the MaxItems limit
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment MoreItemsTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for when no items were returned from the Search function
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment NoItemsTemplate { get; set; }


        /// <summary>
        /// Optional presentation template that is shown at the top of the list. If no items are present, the fragment is hidden.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment BeforeItemsTemplate { get; set; }

        /// <summary>
        /// Optional presentation template that is shown at the bottom of the list. If no items are present, the fragment is hidden.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment AfterItemsTemplate { get; set; }

        /// <summary>
        /// Optional template for progress indicator
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment ProgressIndicatorTemplate { get; set; }

        /// <summary>
        /// Optional template for showing progress indicator inside the popover
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment ProgressIndicatorInPopoverTemplate { get; set; }

        /// <summary>
        /// On drop-down close override Text with selected Value. This makes it clear to the user
        /// which list value is currently selected and disallows incomplete values in Text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool CoerceText { get; set; } = true;

        /// <summary>
        /// If user input is not found by the search func and CoerceValue is set to true the user input
        /// will be applied to the Value which allows to validate it and display an error message.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool CoerceValue { get; set; }

        /// <summary>
        /// Function to be invoked when checking whether an item should be disabled or not
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T, bool> ItemDisabledFunc { get; set; }

        /// <summary>
        /// An event triggered when the state of IsOpen has changed
        /// </summary>
        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        /// <summary>
        /// If true, the currently selected item from the drop-down (if it is open) is selected.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool SelectValueOnTab { get; set; } = false;

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// <para>An event triggered when the number of items returned by the search query has changed.</para>
        /// <para>
        /// If the number is <c>0</c>, <see cref="NoItemsTemplate"/> will be shown.<br />
        /// If the number is beyond <see cref="MaxItems"/>, <see cref="MoreItemsTemplate"/> will be shown.
        /// </para>
        /// </summary>
        [Parameter]
        public EventCallback<int> ReturnedItemsCountChanged { get; set; }

        /// <summary>
        /// Returns the open state of the drop-down.
        /// </summary>
        public bool IsOpen
        {
            get => _isOpen;
            // Note: the setter is protected because it was needed by a user who derived his own autocomplete from this class.
            // Note: setting IsOpen will not open or close it. Use ToggleMenu() for that. 
            protected set
            {
                if (_isOpen == value)
                    return;
                _isOpen = value;

                IsOpenChanged.InvokeAsync(_isOpen).AndForget();
            }
        }

        private bool IsLoading => _currentSearchTask is { IsCompleted: false };

        private string CurrentIcon => !string.IsNullOrWhiteSpace(AdornmentIcon) ? AdornmentIcon : _isOpen ? CloseIcon : OpenIcon;

        public MudAutocomplete()
        {
            Adornment = Adornment.End;
            IconSize = Size.Medium;
        }

        public async Task SelectOption(T value)
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
                _timer?.Dispose();
                IsOpen = false;
                await BeginValidateAsync();
                if (!_isCleared)
                    _elementReference?.SetText(optionText);
                _elementReference?.FocusAsync().AndForget();
                StateHasChanged();
            }
            finally
            {
                _isProcessingValue = false;
            }
        }

        /// <summary>
        /// Toggle the menu (if not disabled or not readonly, and is opened).
        /// </summary>
        public async Task ToggleMenu()
        {
            if ((GetDisabledState() || GetReadOnlyState()) && !IsOpen)
                return;
            await ChangeMenu(!IsOpen);
        }

        private async Task ChangeMenu(bool open)
        {
            if (open)
            {
                if (SelectOnClick)
                    await _elementReference.SelectAsync();
                await OnSearchAsync();
            }
            else
            {
                _timer?.Dispose();
                await RestoreScrollPositionAsync();
                await CoerceTextToValue();
                IsOpen = false;
                StateHasChanged();
            }
        }

        protected override void OnInitialized()
        {
            var text = GetItemString(Value);
            if (!string.IsNullOrWhiteSpace(text))
                Text = text;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (_isClearing || _isProcessingValue)
            {
                //When you select a value in the popover, SelectOption will be called.
                //When it reaches SetValueAsync, it will be awaited.
                //Meanwhile, in parallel, the Clear method will be called, which sets isCleared to true.
                //However, by the time SetValueAsync is released and SelectOption continues its execution, an OnAfterRender event might fire, setting isCleared back to false.
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
            _timer?.Dispose();
            // This keeps the text from being set when clear() was called
            if (_isCleared)
                return Task.CompletedTask;
            return base.UpdateTextPropertyAsync(updateValue);
        }

        protected override async Task UpdateValuePropertyAsync(bool updateText)
        {
            _timer?.Dispose();
            if (ResetValueOnEmptyText && string.IsNullOrWhiteSpace(Text))
                await SetValueAsync(default(T), updateText);
            if (DebounceInterval <= 0)
                await OnSearchAsync();
            else
                _timer = new Timer(OnTimerComplete, null, DebounceInterval, Timeout.Infinite);
        }

        private void OnTimerComplete(object stateInfo) => InvokeAsync(OnSearchAsync);

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

        /// <remarks>
        /// This async method needs to return a task and be awaited in order for
        /// unit tests that trigger this method to work correctly.
        /// </remarks>
        private async Task OnSearchAsync()
        {
            if (MinCharacters > 0 && (string.IsNullOrWhiteSpace(Text) || Text.Length < MinCharacters))
            {
                IsOpen = false;
                StateHasChanged();
                return;
            }

            var searchedItems = Array.Empty<T>();
            CancelToken();

            var searchingWhileSelected = false;
            try
            {
                if (ProgressIndicatorInPopoverTemplate != null)
                {
                    IsOpen = true;
                }

                searchingWhileSelected = !Strict && Value != null && (Value.ToString() == Text || (ToStringFunc != null && ToStringFunc(Value) == Text)); //search while selected if enabled and the Text is equivalent to the Value
                var searchText = searchingWhileSelected ? string.Empty : Text;

                var searchTask = SearchFunc(searchText, _cancellationTokenSrc.Token);

                _currentSearchTask = searchTask;

                StateHasChanged();
                var searchItems = await searchTask ?? Enumerable.Empty<T>();
                searchedItems = searchItems.ToArray();
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
                _selectedListItemIndex = _enabledItemIndices.Any() ? _enabledItemIndices.First() : -1;
            }

            IsOpen = true;

            if (_items?.Length == 0)
            {
                await CoerceValueToText();
                StateHasChanged();
                return;
            }

            if (!CoerceText && CoerceValue)
            {
                await CoerceValueToText();
            }

            StateHasChanged();
        }

        /// <summary>
        /// Clears the autocomplete's text
        /// </summary>
        public async Task Clear()
        {
            _isClearing = true;
            try
            {
                _isCleared = true;
                IsOpen = false;
                await SetTextAsync(null, updateValue: false);
                await CoerceValueToText();
                if (_elementReference != null)
                    await _elementReference.SetText("");
                _timer?.Dispose();
                StateHasChanged();
            }
            finally
            {
                _isClearing = false;
            }
        }

        protected override Task ResetValueAsync() => Clear();

        private string GetItemString(T item)
        {
            if (item == null)
                return string.Empty;
            try
            {
                return Converter.Set(item);
            }
            catch (NullReferenceException) { }
            return "null";
        }

        internal virtual async Task OnInputKeyDown(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Tab":
                    // NOTE: We need to catch Tab in Keydown because a tab will move focus to the next element and thus
                    // in OnInputKeyUp we'd never get the tab key
                    if (!IsOpen)
                        return;
                    if (SelectValueOnTab)
                        await OnEnterKey();
                    else
                        IsOpen = false;
                    break;
            }
            await base.InvokeKeyDownAsync(args);
        }

        internal virtual async Task OnInputKeyUp(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Enter":
                case "NumpadEnter":
                    if (!IsOpen)
                    {
                        await ToggleMenu();
                    }
                    else
                    {
                        await OnEnterKey();
                    }
                    break;
                case "ArrowDown":
                    if (!IsOpen)
                    {
                        await ToggleMenu();
                    }
                    else
                    {
                        var increment = _enabledItemIndices.ElementAtOrDefault(_enabledItemIndices.IndexOf(_selectedListItemIndex) + 1) - _selectedListItemIndex;
                        await SelectNextItem(increment < 0 ? 1 : increment);
                    }
                    break;
                case "ArrowUp":
                    if (args.AltKey)
                    {
                        await ChangeMenu(open: false);
                    }
                    else if (!IsOpen)
                    {
                        await ToggleMenu();
                    }
                    else
                    {
                        var decrement = _selectedListItemIndex - _enabledItemIndices.ElementAtOrDefault(_enabledItemIndices.IndexOf(_selectedListItemIndex) - 1);
                        await SelectNextItem(-(decrement < 0 ? 1 : decrement));
                    }
                    break;
                case "Escape":
                    await ChangeMenu(open: false);
                    break;
                case "Tab":
                    await Task.Delay(1);
                    if (!IsOpen)
                        return;
                    if (SelectValueOnTab)
                        await OnEnterKey();
                    else
                        await ToggleMenu();
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

        private ValueTask SelectNextItem(int increment)
        {
            if (increment == 0 || _items == null || _items.Length == 0 || !_enabledItemIndices.Any())
                return ValueTask.CompletedTask;
            // if we are at the end, or the beginning we just do an rollover
            _selectedListItemIndex = Math.Clamp(value: ((10 * _items.Length) + _selectedListItemIndex + increment) % _items.Length, min: 0, max: _items.Length - 1);
            return ScrollToListItem(_selectedListItemIndex);
        }

        /// <summary>
        /// Scroll to a specific item index in the Autocomplete list of items.
        /// </summary>
        /// <param name="index">the index to scroll to</param>
        public ValueTask ScrollToListItem(int index)
        {
            var id = GetListItemId(index);
            //id of the scrolled element
            return ScrollManager.ScrollToListItemAsync(id);
        }

        //This restores the scroll position after closing the menu and element being 0
        private ValueTask RestoreScrollPositionAsync()
        {
            if (_selectedListItemIndex != 0) return ValueTask.CompletedTask;
            return ScrollManager.ScrollToListItemAsync(GetListItemId(0));
        }

        private string GetListItemId(in int index)
        {
            return $"{_componentId}_item{index}";
        }

        internal async Task OnEnterKey()
        {
            if (IsOpen == false)
                return;
            try
            {
                if (_items == null || _items.Length == 0)
                    return;
                if (_selectedListItemIndex >= 0 && _selectedListItemIndex < _items.Length)
                    await SelectOption(_items[_selectedListItemIndex]);
            }
            finally
            {
                if (IsOpen)
                    IsOpen = false;
            }
        }

        private Task OnInputBlurred(FocusEventArgs args)
        {
            return OnBlur.InvokeAsync(args);
            // we should not validate on blur in autocomplete, because the user needs to click out of the input to select a value,
            // resulting in a premature validation. thus, don't call base
            //base.OnBlurred(args);
        }

        private Task CoerceTextToValue()
        {
            if (CoerceText == false)
                return Task.CompletedTask;

            _timer?.Dispose();

            var text = Value == null ? null : GetItemString(Value);

            // Don't update the value to prevent the popover from opening again after coercion
            if (text != Text)
                return SetTextAsync(text, updateValue: false);

            return Task.CompletedTask;
        }

        private Task CoerceValueToText()
        {
            if (CoerceValue == false)
                return Task.CompletedTask;
            _timer?.Dispose();
            var value = Converter.Get(Text);
            return SetValueAsync(value, updateText: false);
        }

        protected override void Dispose(bool disposing)
        {
            _timer?.Dispose();

            if (_cancellationTokenSrc != null)
            {
                try
                {
                    _cancellationTokenSrc.Dispose();
                }
                catch { /*ignored*/ }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Focus the input in the Autocomplete component.
        /// </summary>
        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        /// <summary>
        /// Blur from the input in the Autocomplete component.
        /// </summary>
        public override ValueTask BlurAsync()
        {
            return _elementReference.BlurAsync();
        }

        /// <summary>
        /// Select all text within the Autocomplete input.
        /// </summary>
        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        /// <summary>
        /// Select all text within the Autocomplete input and aligns its start and end points to the text content of the current input.
        /// </summary>
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        private async Task OnTextChanged(string text)
        {
            await base.TextChanged.InvokeAsync(text);

            if (text == null)
                return;
            await SetTextAsync(text, true);
        }

        private Task ListItemOnClick(T item) => SelectOption(item);
    }
}
