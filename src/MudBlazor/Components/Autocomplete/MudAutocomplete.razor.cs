using System;
using System.Collections.Generic;
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
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }


        /// <summary>
        /// Sets the direction the Autocomplete menu should open.
        /// </summary>
        [Parameter] public Direction Direction { get; set; } = Direction.Bottom;

        /// <summary>
        /// If true, the Autocomplete menu will open either before or after the input (left/right).
        /// </summary>
        [Parameter] public bool OffsetX { get; set; }

        /// <summary>
        /// If true, the Autocomplete menu will open either before or after the input (top/bottom).
        /// </summary>
        [Parameter] public bool OffsetY { get; set; } = true;

        /// <summary>
        /// If true, compact vertical padding will be applied to all Autocomplete items.
        /// </summary>
        [Parameter]
        public bool Dense
        {
            get { return _dense; }
            set
            {
                // Ensure that when dense is applied we set the margin on the input controls
                _dense = value;
                Margin = _dense ? Margin.Dense : Margin.None;
            }
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
        /// Sets the maxheight the Autocomplete can have when open.
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
        /// Set null to display all
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
        /// On drop-down close override Text with selected Value. This makes it clear to the user
        /// which list value is currently selected and disallows incomplete values in Text.
        /// </summary>
        [Parameter] public bool CoerceText { get; set; } = true;

        /// <summary>
        /// If user input is not found by the search func and CoerceValue is set to true the user input
        /// will be applied to the Value which allows to validate it and display an error message.
        /// </summary>
        [Parameter] public bool CoerceValue { get; set; }

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
        /// Set to true to select the currently selected item from the drop-down (if it is open) 
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
            StateHasChanged();
        }

        public async Task ToggleMenu()
        {
            if ((Disabled || ReadOnly) && !IsOpen)
                return;
            IsOpen = !IsOpen;
            if (IsOpen)
            {
                await _elementReference.SelectAsync();
                OnSearch();
            }
            else
            {
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
            var text = GetItemString(Value);
            if (!string.IsNullOrWhiteSpace(text))
                Text = text;
        }

        private Timer _timer;
        private T[] _items;
        private int _selectedListItemIndex = 0;

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
                OnSearch();
            else
                _timer = new Timer(OnTimerComplete, null, DebounceInterval, Timeout.Infinite);
        }

        private void OnTimerComplete(object stateInfo) => InvokeAsync(OnSearch);

        private async void OnSearch()
        {
            if (MinCharacters > 0 && (string.IsNullOrWhiteSpace(Text) || Text.Length < MinCharacters))
            {
                IsOpen = false;
                StateHasChanged();
                return;
            }
            _selectedListItemIndex = 0;
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

            if (_items?.Length == 0)
            {
                await CoerceValueToText();
                IsOpen = false;
                StateHasChanged();
                return;
            }

            IsOpen = true;
            StateHasChanged();
        }

        /// <summary>
        /// Clears the autocomplete's text
        /// </summary>
        public async Task Clear()
        {
            await SetTextAsync(string.Empty, updateValue: false);
            await CoerceValueToText();
            IsOpen = false;
            _timer?.Dispose();
            StateHasChanged();
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
                    await OnEnterKey();
                    break;
                case "ArrowDown":
                    await SelectNextItem(+1);
                    break;
                case "ArrowUp":
                    await SelectNextItem(-1);
                    break;
                case "Escape":
                    IsOpen = false;
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
            if (_items == null || _items.Length == 0)
                return;
            _selectedListItemIndex = Math.Max(0, Math.Min(_items.Length - 1, _selectedListItemIndex + increment));
            await ScrollToListItem(_selectedListItemIndex, increment);
            StateHasChanged();
        }

        /// <summary>
        /// We need a random id for the year items in the year list so we can scroll to the item safely in every DatePicker.
        /// </summary>
        private readonly string _componentId = Guid.NewGuid().ToString();

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

        private async Task CoerceTextToValue()
        {
            if (CoerceText == false)
                return;
            if (Value == null)
            {
                _timer?.Dispose();
                await SetTextAsync(null);
                return;
            }
            var actualvalueStr = GetItemString(Value);
            if (!Equals(actualvalueStr, Text))
            {
                _timer?.Dispose();
                await SetTextAsync(actualvalueStr);
            }
        }

        private async Task CoerceValueToText()
        {
            if (CoerceValue == false)
                return;
            _timer?.Dispose();
            var value = Converter.Get(Text);
            await SetValueAsync(value, updateText: false);
        }

        protected override void Dispose(bool disposing)
        {
            _timer?.Dispose();
            base.Dispose(disposing);
        }

        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        private void OnTextChanged(string text)
        {
            if (text == null)
                return;
            _ = SetTextAsync(text, true);
        }

    }
}
