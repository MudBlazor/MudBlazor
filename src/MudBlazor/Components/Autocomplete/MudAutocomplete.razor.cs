using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudAutocomplete<T> : MudBaseInput<T>, IDisposable
    {
        [Inject] IJSRuntime JsRuntime { get; set; }

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

        public void SelectOption(T value)
        {
            Value = value;
            if (_items != null)
                _selectedListItemIndex = Array.IndexOf(_items, value);
            _text = GetItemString(value);
            _timer?.Dispose();
            IsOpen = false;
            UpdateIcon();
            BeginValidate();
            StateHasChanged();
        }

        public void ToggleMenu()
        {
            if (Disabled || MinCharacters > 0 && (string.IsNullOrEmpty(Text) || Text.Length < MinCharacters))
                return;
            IsOpen = !IsOpen;
            //if (IsOpen && string.IsNullOrEmpty(Text))
            //    IsOpen = false;
            if (IsOpen)
            {
                OnSearch();
            }
            else
            {
                CoerceTextToValue();
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

            var searched_items = await SearchFunc(Text);
            if (MaxItems.HasValue)
                searched_items = searched_items.Take(MaxItems.Value);
            _items = searched_items.ToArray();

            if (_items?.Count() == 0)
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
                    OnEnterKey();
                    break;
                case "ArrowDown":
                    await SelectNextItem(+1);
                    break;
                case "ArrowUp":
                    await SelectNextItem(-1);
                    break;
            }
            base.onKeyDown(args);
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
            await JsRuntime
                .InvokeVoidAsync("scrollHelpers.scrollToListItem", id, increment, true);
            StateHasChanged();
        }


        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender) return;
            RestoreScrollPosition();
        }

        //This restores the scroll position after closing the menu and element being 0
        private void RestoreScrollPosition()
        {
            if (_selectedListItemIndex != 0) return;
            JsRuntime
             .InvokeVoidAsync("scrollHelpers.scrollToListItem", GetListItemId(0), 0);
        }

        private string GetListItemId(in int index)
        {
            return $"{_componentId}_item{index}";
        }

        private void OnEnterKey()
        {
            if (IsOpen == false)
                return;
            if (_items == null || _items.Length == 0)
                return;
            if (_selectedListItemIndex >= 0 && _selectedListItemIndex < _items.Length)
                SelectOption(_items[_selectedListItemIndex]);
        }

        private void OnInputBlurred(FocusEventArgs args)
        {
            if (!IsOpen)
                CoerceTextToValue();
            // we should not validate on blur in autocomplete, because the user needs to click out of the input to select a value, 
            // resulting in a premature validation. thus, don't call base
            //base.OnBlurred(args);
        }

        private void CoerceTextToValue()
        {
            if (CoerceText == false)
                return;
            if (Value == null)
            {
                Text = null;
                _timer?.Dispose();
                return;
            }
            var actualvalueStr = GetItemString(Value);
            if (!object.Equals(actualvalueStr, Text))
            {
                Text = actualvalueStr;
                _timer?.Dispose();
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

        public override ValueTask SelectAsnyc()
        {
            return _elementReference.SelectAsnyc();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

    }
}
