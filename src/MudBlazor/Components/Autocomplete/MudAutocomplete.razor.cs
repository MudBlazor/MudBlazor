using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudAutocomplete<T> : MudBaseInput<T>, IDisposable
    {

        #region Injected Services, Constructor, Fields & Parameters

        public MudAutocomplete()
        {
            Adornment = Adornment.End;
            IconSize = Size.Medium;
        }

        [Inject] private IKeyInterceptorFactory KeyInterceptorFactory { get; set; }
        [Inject] IScrollManager ScrollManager { get; set; }

        /// <summary>
        /// This boolean will keep track if the clear function is called too keep the set text function to be called.
        /// </summary>
        private bool _isCleared;

        private MudInput<string> _elementReference;
        MudList<T> _list;
        private IKeyInterceptor _keyInterceptor;
        private bool _dense;
        private Timer _timer;
        private T[] _items;
        private int _selectedListItemIndex = 0;
        private IList<int> _enabledItemIndices = new List<int>();
        private int _itemsReturned; //the number of items returned by the search function
        //int _elementKey = 0;

        private string CurrentIcon => !string.IsNullOrWhiteSpace(AdornmentIcon) ? AdornmentIcon : _isOpen ? CloseIcon : OpenIcon;

        /// <summary>
        /// We need a random id for the year items in the year list so we can scroll to the item safely in every DatePicker.
        /// </summary>
        private readonly string _componentId = Guid.NewGuid().ToString();

        protected string Classname =>
            new CssBuilder("mud-select")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// User class names for the popover, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string PopoverClass { get; set; }

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
        /// Set the anchor origin point to determen where the popover will open from.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use AnchorOrigin or TransformOrigin instead.", true)]
        [Parameter] public Direction Direction { get; set; } = Direction.Bottom;

        /// <summary>
        /// If true, the Autocomplete menu will open either before or after the input (left/right).
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use AnchorOrigin or TransformOrigin instead.", true)]
        [Parameter] public bool OffsetX { get; set; }

        /// <summary>
        /// If true, the Autocomplete menu will open either before or after the input (top/bottom).
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use AnchorOrigin or TransformOrigin instead.", true)]
        [Parameter] public bool OffsetY { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all Autocomplete items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public bool Dense
        {
            get { return _dense; }
            set { _dense = value; }
        }

        /// <summary>
        /// The Open Autocomplete Icon
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string OpenIcon { get; set; } = Icons.Filled.ArrowDropDown;

        /// <summary>
        /// The Close Autocomplete Icon
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string CloseIcon { get; set; } = Icons.Filled.ArrowDropUp;

        //internal event Action<HashSet<T>> SelectionChangedFromOutside;

        /// <summary>
        /// The maximum height of the Autocomplete when it is open.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public int MaxHeight { get; set; } = 300;

        private bool _multiSelection = false;
        /// <summary>
        /// If true, multiple values can be selected via checkboxes which are automatically shown in the dropdown
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool MultiSelection
        {
            get => _multiSelection;
            set
            {
                if (value != _multiSelection)
                {
                    _multiSelection = value;
                    UpdateTextPropertyAsync(false).AndForget();
                }
            }
        }

        private Func<T, string> _toStringFunc;

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
        /// The SearchFunc returns a list of items matching the typed text
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<string, Task<IEnumerable<T>>> SearchFunc { get; set; }

        /// <summary>
        /// Maximum items to display, defaults to 10.
        /// A null value will display all items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public int? MaxItems { get; set; } = null;

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

        private bool _isOpen;

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
                if (value == _isOpen)
                    return;
                _isOpen = value;

                IsOpenChanged.InvokeAsync(_isOpen).AndForget();
            }
        }

        /// <summary>
        /// An event triggered when the state of IsOpen has changed
        /// </summary>
        [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

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
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        #endregion


        #region Lifecycle Methods (& Dispose)

        protected override void OnInitialized()
        {
            var text = GetItemString(Value);
            if (!string.IsNullOrWhiteSpace(text))
                Text = text;
        }

        private string _elementId = "autocomplete_" + Guid.NewGuid().ToString().Substring(0, 8);
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _keyInterceptor = KeyInterceptorFactory.Create();

                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-input-control",
                    Keys = {
                        new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page, instead hilight previous item
                        new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page, instead hilight next item
                        new KeyOptions { Key="Home", PreventDown = "key+none" },
                        new KeyOptions { Key="End", PreventDown = "key+none" },
                        new KeyOptions { Key="Escape" },
                        new KeyOptions { Key="Enter", PreventDown = "key+none" },
                        new KeyOptions { Key="NumpadEnter", PreventDown = "key+none" },
                        new KeyOptions { Key="a", PreventDown = "key+ctrl" }, // select all items instead of all page text
                        new KeyOptions { Key="A", PreventDown = "key+ctrl" }, // select all items instead of all page text
                        new KeyOptions { Key="/./", SubscribeDown = true, SubscribeUp = true }, // for our users
                    },
                });
                _keyInterceptor.KeyDown += HandleKeyDown;
                _keyInterceptor.KeyUp += HandleKeyUp;
            }
            _isCleared = false;
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void Dispose(bool disposing)
        {
            _timer?.Dispose();
            base.Dispose(disposing);
        }

        #endregion


        #region Values, Text & Coerce

        //private T _selectedValue;
        //protected T SelectedValue
        //{
        //    get => _selectedValue;

        //    set
        //    {
        //        if (Converter.Set(_selectedValue) == Converter.Set(value))
        //        {
        //            return;
        //        }
        //        _selectedValue = value;
        //        SetValueAsync(value, false).AndForget();
        //    }
        //}

        private HashSet<T> _selectedValues = new HashSet<T>();
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public IEnumerable<T> SelectedValues
        {
            get
            {
                if (_selectedValues == null)
                    _selectedValues = new HashSet<T>();
                return _selectedValues;
            }
            set
            {
                var set = value ?? new HashSet<T>();
                if (_selectedValues == value)
                {
                    return;
                }
                if (_selectedValues != null && value != null && _selectedValues.SetEquals(value))
                {
                    return;
                }
                //if (SelectedValues.Count() == set.Count() && _selectedValues.All(x => set.Contains(x)))
                //    return;
                _selectedValues = value == null ? null : value.ToHashSet();
                if (MultiSelection == false && _selectedValues != null)
                    SetValueAsync(_selectedValues.FirstOrDefault()).AndForget();
                //else
                //{
                //    //Warning. Here the Converter was not set yet
                //    if (MultiSelectionTextFunc != null)
                //    {
                //        SetCustomizedTextAsync(string.Join(Delimiter, SelectedValues.Select(x => Converter.Set(x))),
                //            selectedConvertedValues: SelectedValues.Select(x => Converter.Set(x)).ToList(),
                //            multiSelectionTextFunc: MultiSelectionTextFunc).AndForget();
                //    }
                //    else
                //    {
                //        SetTextAsync(string.Join(Delimiter, SelectedValues.Select(x => Converter.Set(x))), updateValue: false).AndForget();
                //    }
                //}
                SelectedValuesChanged.InvokeAsync(new HashSet<T>(SelectedValues)).AndForget();
                if (MultiSelection && typeof(T) == typeof(string))
                    SetValueAsync((T)(object)Text, updateText: false).AndForget();
            }
        }

        private MudListItem<T> _selectedListItem;
        internal MudListItem<T> SelectedListItem
        {
            get => _selectedListItem;

            set
            {
                if (_selectedListItem == value)
                {
                    return;
                }
                _selectedListItem = value;
            }
        }

        /// <summary>
        /// Fires when SelectedValues changes.
        /// </summary>
        [Parameter] public EventCallback<IEnumerable<T>> SelectedValuesChanged { get; set; }

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

        private async Task OnTextChanged(string text)
        {
            await base.TextChanged.InvokeAsync();

            if (text == null)
                return;
            await SetTextAsync(text, true);
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
            Value = Converter.Get(Text);
            return SetValueAsync(value, updateText: false);
        }

        #endregion


        #region Selection & Search

        public async Task SelectOption(T value)
        {
            await SetValueAsync(value);
            //if (_items != null)
            //    _selectedListItemIndex = Array.IndexOf(_items, value);
            var optionText = GetItemString(value);
            if (!_isCleared)
                await SetTextAsync(optionText, false);
            _timer?.Dispose();
            if (MultiSelection == false)
            {
                IsOpen = false;
            }
            BeginValidate();
            if (!_isCleared)
                await _elementReference?.SetText(optionText);
            _elementReference?.FocusAsync().AndForget();
            StateHasChanged();
        }

        /// <remarks>
        /// This async method needs to return a task and be awaited in order for
        /// unit tests that trigger this method to work correctly.
        /// </remarks>
        internal async Task OnSearchAsync()
        {
            if (MinCharacters > 0 && (string.IsNullOrWhiteSpace(Text) || Text.Length < MinCharacters))
            {
                IsOpen = false;
                StateHasChanged();
                return;
            }

            IEnumerable<T> searchedItems = Array.Empty<T>();
            try
            {
                searchedItems = (await SearchFunc(Text)) ?? Array.Empty<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine("The search function failed to return results: " + e.ToString());
            }
            _itemsReturned = searchedItems.Count();
            if (MaxItems.HasValue)
            {
                searchedItems = searchedItems.Take(MaxItems.Value);
            }
            _items = searchedItems.ToArray();

            _enabledItemIndices = _items.Select((item, idx) => (item, idx)).Where(tuple => ItemDisabledFunc?.Invoke(tuple.item) != true).Select(tuple => tuple.idx).ToList();
            _selectedListItemIndex = _enabledItemIndices.Any() ? _enabledItemIndices.First() : -1;

            IsOpen = true;

            if (_items?.Length == 0)
            {
                await CoerceValueToText();
                StateHasChanged();
                return;
            }

            //if (_list != null)
            //{
            //    _list.UpdateLastActivatedItem(Value);
            //    _list.UpdateSelectedStyles();
            //}

            StateHasChanged();
        }

        #endregion


        #region Popover State

        /// <summary>
        /// Toggle the menu (if not disabled or not readonly, and is opened).
        /// </summary>
        public async Task ToggleMenu()
        {
            if ((Disabled || ReadOnly) && !IsOpen)
                return;
            if (IsOpen)
                await CloseMenu();
            else
                await OpenMenu();
        }

        public async Task OpenMenu()
        {
            if (SelectOnClick)
                await _elementReference.SelectAsync();
            await OnSearchAsync();
            await Task.Delay(1);

            //disable escape propagation: if selectmenu is open, only the select popover should close and underlying components should not handle escape key
            await _keyInterceptor.UpdateKey(new() { Key = "Escape", StopDown = "Key+none" });

            //await OnOpen.InvokeAsync();
        }

        public async Task CloseMenu()
        {
            if (ResetValueOnEmptyText && string.IsNullOrEmpty(Text))
            {
                await SetValueAsync(default(T));
            }
            _timer?.Dispose();
            await CoerceTextToValue();
            IsOpen = false;
            StateHasChanged();

            //enable escape propagation: the select popover was closed, now underlying components are allowed to handle escape key
            await _keyInterceptor.UpdateKey(new() { Key = "Escape", StopDown = "none" });

            //await OnClose.InvokeAsync();
        }

        [Obsolete]
        private async Task ChangeMenu(bool open)
        {
            if (open)
            {
                if (SelectOnClick)
                    await _elementReference.SelectAsync();
                await OnSearchAsync();
                //await Task.Delay(1);
                //if (Value != null)
                //{
                //    _list.UpdateLastActivatedItem(Value);
                //}
                //if (_list != null && _list._lastActivatedItem != null && !(MultiSelection && _list._allSelected == true))
                //{
                //    await _list.ScrollToMiddleAsync(_list._lastActivatedItem);
                //}
            }
            else
            {
                if (ResetValueOnEmptyText && string.IsNullOrEmpty(Text))
                {
                    await SetValueAsync(default(T));
                }
                _timer?.Dispose();
                //RestoreScrollPosition();
                await CoerceTextToValue();
                IsOpen = false;
                StateHasChanged();
            }
        }

        #endregion


        #region Reset & Clear

        /// <summary>
        /// Clears the autocomplete's text
        /// </summary>
        public async Task Clear()
        {
            _isCleared = true;
            IsOpen = false;
            await SetTextAsync(string.Empty, updateValue: false);
            await CoerceValueToText();
            if (_elementReference != null)
                await _elementReference.SetText("");
            _timer?.Dispose();
            StateHasChanged();
        }

        protected override async void ResetValue()
        {
            await Clear();
            base.ResetValue();
        }

        #endregion


        #region Events (Key, Focus, Blur, Select, Select Range)

        internal async void HandleKeyDown(KeyboardEventArgs args)
        {
            if (Disabled || ReadOnly)
            {
                return;
            }

            if (_list != null && _isOpen == true && args.Key.Length != 1)
            {
                await _list.HandleKeyDown(args);
                //_list.UpdateSelectedStyles();
            }

            switch (args.Key)
            {
                case "Tab":
                    if (!IsOpen)
                        return;
                    if (SelectValueOnTab)
                        await OnEnterKey();
                    else
                        IsOpen = false;
                    break;
                case "Enter":
                case "NumpadEnter":
                    if (!IsOpen)
                    {
                        await ToggleMenu();
                    }
                    else
                    {
                        await ToggleMenu();
                        await _elementReference.SetText(Value?.ToString());
                    }
                    break;

                case "Escape":
                    await CloseMenu();
                    break;
                case "Backspace":
                    if (args.CtrlKey == true && args.ShiftKey == true)
                    {
                        Reset();
                    }
                    break;
                case "ArrowDown":
                    if (!IsOpen)
                    {
                        await ToggleMenu();
                    }
                    else
                    {
                        //var increment = _enabledItemIndices.ElementAtOrDefault(_enabledItemIndices.IndexOf(_selectedListItemIndex) + 1) - _selectedListItemIndex;
                        //await SelectNextItem(increment < 0 ? 1 : increment);
                    }
                    break;
                case "ArrowUp":
                    if (args.AltKey == true)
                    {
                        await OpenMenu();
                    }
                    else if (!IsOpen)
                    {
                        await ToggleMenu();
                    }
                    else
                    {
                        //var decrement = _selectedListItemIndex - _enabledItemIndices.ElementAtOrDefault(_enabledItemIndices.IndexOf(_selectedListItemIndex) - 1);
                        //await SelectNextItem(-(decrement < 0 ? 1 : decrement));
                    }
                    break;
            }
            await OnKeyDown.InvokeAsync();
        }

        internal async void HandleKeyUp(KeyboardEventArgs args)
        {

            await OnKeyUp.InvokeAsync();
        }

        internal Task OnEnterKey()
        {
            if (IsOpen == false)
                return Task.CompletedTask;
            if (_items == null || _items.Length == 0)
                return Task.CompletedTask;
            if (_selectedListItemIndex >= 0 && _selectedListItemIndex < _items.Length)
                return SelectOption(Value);
            return Task.CompletedTask;
        }

        private async Task OnInputBlurred(FocusEventArgs args)
        {
            await OnBlur.InvokeAsync(args);
            //return Task.CompletedTask;
            // we should not validate on blur in autocomplete, because the user needs to click out of the input to select a value,
            // resulting in a premature validation. thus, don't call base
            //base.OnBlurred(args);
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

        #endregion

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

        private void OnTimerComplete(object stateInfo) => InvokeAsync(OnSearchAsync).AndForget();

        //private ValueTask SelectNextItem(int increment)
        //{
        //    if (increment == 0 || _items == null || _items.Count == 0 || !_enabledItemIndices.Any())
        //        return ValueTask.CompletedTask;
        //    // if we are at the end, or the beginning we just do an rollover
        //    _selectedListItemIndex = Math.Clamp(value: (10 * _items.Count + _selectedListItemIndex + increment) % _items.Count, min: 0, max: _items.Count - 1);
        //    return ScrollToListItem(_selectedListItemIndex);
        //}




        ///// <summary>
        ///// Scroll to a specific item index in the Autocomplete list of items.
        ///// </summary>
        ///// <param name="index">the index to scroll to</param>
        ///// <param name="increment">not used</param>
        ///// <returns>ValueTask</returns>
        //[Obsolete("Use ScrollToListItem without increment parameter instead")]
        //public Task ScrollToListItem(int index, int increment)
        //    => ScrollToListItem(index).AsTask();

        ///// <summary>
        ///// Scroll to a specific item index in the Autocomplete list of items.
        ///// </summary>
        ///// <param name="index">the index to scroll to</param>
        //public ValueTask ScrollToListItem(int index)
        //{
        //    var id = GetListItemId(index);
        //    //id of the scrolled element
        //    return ScrollManager.ScrollToListItemAsync(id);
        //}

        ////This restores the scroll position after closing the menu and element being 0
        //private void RestoreScrollPosition()
        //{
        //    if (_selectedListItemIndex != 0) return;
        //    ScrollManager.ScrollToListItemAsync(GetListItemId(0));
        //}

        //private string GetListItemId(in int index)
        //{
        //    return $"{_componentId}_item{index}";
        //}

        protected internal ValueTask ScrollToMiddleAsync(MudListItem<T> item)
            => ScrollManager.ScrollToMiddleAsync(_elementId, item.ItemId);

        protected void ChipClose(MudChip chip)
        {
            SelectedValues = SelectedValues.Where(x => !x.Equals(chip.Value));
        }

        //private async Task ListItemOnClick(T item)
        //{
        //    await SelectOption(item);
        //}

    }
}
