using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudAutocomplete<T> : MudBaseInput<T>, IDisposable
    {
        [Inject] IScrollManager ScrollManager { get; set; }

        private bool _dense;

        protected string Classname =>
            new CssBuilder("mud-select")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// User class names for the popover, separated by space
        /// </summary>
        [Parameter] public string PopoverClass { get; set; }

        /// <summary>
        /// Set the anchor origin point to determen where the popover will open from.
        /// </summary>
        [Parameter] public Origin AnchorOrigin { get; set; } = Origin.BottomCenter;

        /// <summary>
        /// Sets the transform origin point for the popover.
        /// </summary>
        [Parameter] public Origin TransformOrigin { get; set; } = Origin.TopCenter;

        /// <summary>
        /// Set the anchor origin point to determen where the popover will open from.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Direction is obsolete. Use AnchorOrigin or TransformOrigin instead!", false)]
        [Parameter] public Direction Direction { get; set; } = Direction.Bottom;

        /// <summary>
        /// If true, the Autocomplete menu will open either before or after the input (left/right).
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("OffsetX is obsolete. Use AnchorOrigin or TransformOrigin instead!", false)]
        [Parameter] public bool OffsetX { get; set; }

        /// <summary>
        /// If true, the Autocomplete menu will open either before or after the input (top/bottom).
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("OffsetY is obsolete. Use AnchorOrigin or TransformOrigin instead!", false)]
        [Parameter] public bool OffsetY { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all Autocomplete items.
        /// </summary>
        [Parameter]
        public bool Dense
        {
            get { return _dense; }
            set { _dense = value; }
        }

        /// <summary>
        /// The Open Autocomplete Icon
        /// </summary>
        [Parameter] public string OpenIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// The Close Autocomplete Icon
        /// </summary>
        [Parameter] public string CloseIcon { get; set; } = Icons.Material.Filled.ArrowDropUp;

        //internal event Action<HashSet<T>> SelectionChangedFromOutside;

        /// <summary>
        /// The maximum height of the Autocomplete when it is open.
        /// </summary>
        [Parameter] public int MaxHeight { get; set; } = 300;

        private Func<T, string> _toStringFunc;

        /// <summary>
        /// Defines how values are displayed in the drop-down list
        /// </summary>
        [Parameter]
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
        public Func<string, Task<IEnumerable<T>>> SearchFunc { get; set; }

        /// <summary>
        /// Maximum items to display, defaults to 10.
        /// A null value will display all items.
        /// </summary>
        [Parameter]
        public int? MaxItems { get; set; } = 10;

        /// <summary>
        /// Minimum characters to initiate a search
        /// </summary>
        [Parameter]
        public int MinCharacters { get; set; } = 0;

        /// <summary>
        /// Reset value if user deletes the text
        /// </summary>
        [Parameter]
        public bool ResetValueOnEmptyText { get; set; } = false;

        /// <summary>
        /// Debounce interval in milliseconds.
        /// </summary>
        [Parameter] public int DebounceInterval { get; set; } = 100;

        /// <summary>
        /// Optional presentation template for unselected items
        /// </summary>
        [Parameter] public RenderFragment<T> ItemTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for the selected item
        /// </summary>
        [Parameter] public RenderFragment<T> ItemSelectedTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for disabled item
        /// </summary>
        [Parameter] public RenderFragment<T> ItemDisabledTemplate { get; set; }

        /// <summary>
        /// On drop-down close override Text with selected Value. This makes it clear to the user
        /// which list value is currently selected and disallows incomplete values in Text.
        /// </summary>
        [Parameter] public bool CoerceText { get; set; } = true;

        /// <summary>
        /// If user input is not found by the search func and CoerceValue is set to true the user input
        /// will be applied to the Value which allows to validate it and display an error message.
        /// </summary>
        [Parameter] public bool CoerceValue { get; set; }

        /// <summary>
        /// Function to be invoked when checking whether an item should be disabled or not
        /// </summary>
        [Parameter] public Func<T, bool> ItemDisabledFunc { get; set; }

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
                UpdateIcon();
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
        [Parameter] public bool SelectValueOnTab { get; set; } = false;

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter] public bool Clearable { get; set; } = false;

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        private string _currentIcon;

        private MudInput<string> _elementReference;

        internal Origin _anchorOrigin;
        internal Origin _transformOrigin;

#pragma warning disable CS0618 // This is for backwards compability until Obsolete is removed
        [ExcludeFromCodeCoverage]
        private void GetPopoverOrigins()
        {
            if (Direction != Direction.Bottom || OffsetY || OffsetX)
            {
                switch (Direction)
                {
                    case Direction.Bottom when OffsetY:
                    case Direction.Top when OffsetY:
                        _anchorOrigin = Origin.BottomCenter;
                        _transformOrigin = Origin.TopCenter;
                        break;
                    case Direction.Top when !OffsetY:
                        _anchorOrigin = Origin.BottomCenter;
                        _transformOrigin = Origin.BottomCenter;
                        break;
                    case Direction.Start when OffsetX:
                    case Direction.Left when OffsetX:
                        _anchorOrigin = Origin.TopLeft;
                        _transformOrigin = Origin.TopRight;
                        break;
                    case Direction.End when OffsetX:
                    case Direction.Right when OffsetX:
                        _anchorOrigin = Origin.TopRight;
                        _transformOrigin = Origin.TopLeft;
                        break;
                }
            }
            else
            {
                _anchorOrigin = AnchorOrigin;
                _transformOrigin = TransformOrigin;
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete


        public MudAutocomplete()
        {
            Adornment = Adornment.End;
            IconSize = Size.Medium;
        }

        public async Task SelectOption(T value)
        {
            await SetValueAsync(value);
            if (_items != null)
                _selectedListItemIndex = Array.IndexOf(_items, value);
            var optionText = GetItemString(value);
            await SetTextAsync(optionText, false);
            _timer?.Dispose();
            IsOpen = false;
            BeginValidate();
            _elementReference?.SetText(optionText);
            _elementReference?.FocusAsync().AndForget();
            StateHasChanged();
        }

        /// <summary>
        /// Toggle the menu (if not disabled or not readonly, and is opened).
        /// </summary>
        public async Task ToggleMenu()
        {
            if ((Disabled || ReadOnly) && !IsOpen)
                return;
            await ChangeMenu(!IsOpen);
        }

        private async Task ChangeMenu(bool open)
        {
            IsOpen = open;
            if (open)
            {
                await _elementReference.SelectAsync();
                await OnSearchAsync();
            }
            else
            {
                _timer?.Dispose();
                RestoreScrollPosition();
                await CoerceTextToValue();
            }
            StateHasChanged();
        }

        private void UpdateIcon()
        {
            _currentIcon = !string.IsNullOrWhiteSpace(AdornmentIcon) ? AdornmentIcon : _isOpen ? CloseIcon : OpenIcon;
        }

        protected override void OnInitialized()
        {
            UpdateIcon();
            GetPopoverOrigins(); // Just to keep Obsolete functional until removed.
            var text = GetItemString(Value);
            if (!string.IsNullOrWhiteSpace(text))
                Text = text;
        }

        private Timer _timer;
        private T[] _items;
        private int _selectedListItemIndex = 0;
        private IList<int> _enabledItemIndices = new List<int>();

        protected override Task UpdateTextPropertyAsync(bool updateValue)
        {
            _timer?.Dispose();
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

            IEnumerable<T> searched_items = Array.Empty<T>();
            try
            {
                searched_items = (await SearchFunc(Text)) ?? Array.Empty<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine("The search function failed to return results: " + e.Message);
            }
            if (MaxItems.HasValue)
                searched_items = searched_items.Take(MaxItems.Value);
            _items = searched_items.ToArray();

            _enabledItemIndices = _items.Select((item, idx) => (item, idx)).Where(tuple => ItemDisabledFunc?.Invoke(tuple.item) != true).Select(tuple => tuple.idx).ToList();
            _selectedListItemIndex = _enabledItemIndices.Any() ? _enabledItemIndices.First() : -1;

            if (_items?.Length == 0)
            {
                await CoerceValueToText();
                StateHasChanged();
                return;
            }

            IsOpen = true;
            StateHasChanged();
        }

        int _elementKey = 0;

        /// <summary>
        /// Clears the autocomplete's text
        /// </summary>
        public async Task Clear()
        {
            IsOpen = false;
            await SetTextAsync(string.Empty, updateValue: false);
            await CoerceValueToText();
            await _elementReference.SetText("");
            _timer?.Dispose();
            StateHasChanged();
        }

        protected override async void ResetValue()
        {
            await Clear();
            base.ResetValue();
        }


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

        protected virtual async Task OnInputKeyDown(KeyboardEventArgs args)
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
        }

        protected virtual async Task OnInputKeyUp(KeyboardEventArgs args)
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
                    if (args.AltKey == true)
                    {
                        await ChangeMenu(open:false);
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
            }
            base.InvokeKeyUp(args);
        }

        private async Task SelectNextItem(int increment)
        {
            if (_items == null || _items.Length == 0 || !_enabledItemIndices.Any())
                return;
            _selectedListItemIndex = Math.Max(0, Math.Min(_items.Length - 1, _selectedListItemIndex + increment));
            await ScrollToListItem(_selectedListItemIndex, increment);
            StateHasChanged();
        }

        /// <summary>
        /// We need a random id for the year items in the year list so we can scroll to the item safely in every DatePicker.
        /// </summary>
        private readonly string _componentId = Guid.NewGuid().ToString();

        /// <summary>
        /// Scroll to a specific item in the Autocomplete list of items.
        /// </summary>
        public async Task ScrollToListItem(int index, int increment)
        {
            var id = GetListItemId(index);
            //id of the scrolled element
            //increment 1 down; -1 up
            //onEdges, last param, boolean. If true, only scrolls when elements reaches top or bottom of container.
            //If false, scrolls always
            await ScrollManager.ScrollToListItemAsync(id, increment, true);
            StateHasChanged();
        }

        //This restores the scroll position after closing the menu and element being 0
        private void RestoreScrollPosition()
        {
            if (_selectedListItemIndex != 0) return;
            ScrollManager.ScrollToListItemAsync(GetListItemId(0), 0, false);
        }

        private string GetListItemId(in int index)
        {
            return $"{_componentId}_item{index}";
        }

        private Task OnEnterKey()
        {
            if (IsOpen == false)
                return Task.CompletedTask;
            if (_items == null || _items.Length == 0)
                return Task.CompletedTask;
            if (_selectedListItemIndex >= 0 && _selectedListItemIndex < _items.Length)
                return SelectOption(_items[_selectedListItemIndex]);
            return Task.CompletedTask;
        }

        private Task OnInputBlurred(FocusEventArgs args)
        {
            OnBlur.InvokeAsync(args);
            return Task.CompletedTask;
            // we should not validate on blur in autocomplete, because the user needs to click out of the input to select a value,
            // resulting in a premature validation. thus, don't call base
            //base.OnBlurred(args);
        }

        private Task CoerceTextToValue()
        {
            if (CoerceText == false)
                return Task.CompletedTask;
            if (Value == null)
            {
                _timer?.Dispose();
                return SetTextAsync(null);
            }
            var actualvalueStr = GetItemString(Value);
            if (!Equals(actualvalueStr, Text))
            {
                _timer?.Dispose();
                return SetTextAsync(actualvalueStr);
            }
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
            if (text == null)
                return;
            await SetTextAsync(text, true);
        }

    }
}
