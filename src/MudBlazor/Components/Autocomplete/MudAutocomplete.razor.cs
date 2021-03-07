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
        /// If true, compact vertical padding will be applied to all select items.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// The Open Select Icon
        /// </summary>
        [Parameter] public string OpenIcon { get; set; } = Icons.Material.Filled.ArrowDropUp;

        /// <summary>
        /// The Open Select Icon
        /// </summary>
        [Parameter] public string CloseIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        //internal event Action<HashSet<T>> SelectionChangedFromOutside;

        /// <summary>
        /// Sets the maxheight the select can have when open.
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
                    //GetFunc = LookupValue,
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
        /// Minimum characters to initiate a search, defaults to 2
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

        internal bool IsOpen { get; set; }

        public string CurrentIcon { get; set; }

        private MudInput<string> _elementReference;

        public async Task SelectOption(T value)
        {
            await SetValueAsync(value);
            if (_items != null)
                _selectedListItemIndex = Array.IndexOf(_items, value);
            await SetTextAsync(GetItemString(value), false);
            _timer?.Dispose();
            IsOpen = false;
            UpdateIcon();
            BeginValidate();
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
            UpdateIcon();
            StateHasChanged();
        }

        public void UpdateIcon()
        {
            if (IsOpen)
            {
                CurrentIcon = OpenIcon;
            }
            else
            {
                CurrentIcon = CloseIcon;
            }
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
            //_timer?.Dispose();
            //return base.UpdateTextPropertyAsync(updateValue);
            return Task.CompletedTask;
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
                IsOpen = false;
                UpdateIcon();
                StateHasChanged();
                return;
            }

            IsOpen = true;
            UpdateIcon();
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
                case "Enter":
                    await OnEnterKey();
                    break;
                case "ArrowDown":
                    await SelectNextItem(+1);
                    break;
                case "ArrowUp":
                    await SelectNextItem(-1);
                    break;
            }
            base.InvokeKeyDown(args);
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
            //return !IsOpen ? CoerceTextToValue() : Task.CompletedTask;
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
            if (!object.Equals(actualvalueStr, Text))
            {
                _timer?.Dispose();
                await SetTextAsync(actualvalueStr);
            }
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
