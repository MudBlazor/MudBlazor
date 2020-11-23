using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

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
        /// If true, compact vertical padding will be applied to all select items.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// The Open Select Icon
        /// </summary>
        [Parameter] public string OpenIcon { get; set; } = Icons.Material.ArrowDropUp;

        /// <summary>
        /// The Open Select Icon
        /// </summary>
        [Parameter] public string CloseIcon { get; set; } = Icons.Material.ArrowDropDown;

        internal event Action<HashSet<T>> SelectionChangedFromOutside;

        /// <summary>
        /// Sets the maxheight the select can have when open.
        /// </summary>
        [Parameter] public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// Defines how values are displayed in the drop-down list
        /// </summary>
        [Parameter]
        public Func<T, string> ToStringFunc { get; set; } = null;

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


        internal bool IsOpen { get; set; }

        public string CurrentIcon { get; set; }

        public void SelectOption(T value)
        {
            Value = value;
            if (Items != null)
                SelectedListItemIndex = Array.IndexOf(Items, value);
            _text = GetItemString(value);
            Timer?.Dispose();
            IsOpen = false;
            UpdateIcon();
            ValidateValue(Value);
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
                InvokeAsync(() => ScrollToListItem(SelectedListItemIndex));
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

        private Timer Timer;
        private T[] Items;
        private int SelectedListItemIndex = 0;

        protected override void GenericValueChanged(T value)
        {
            base.GenericValueChanged(value);
            Timer?.Dispose();
        }

        protected override void StringValueChanged(string text)
        {
            if (ResetValueOnEmptyText && string.IsNullOrWhiteSpace(text))
                Value = default(T);
            Timer?.Dispose();
            var autoReset = new AutoResetEvent(false);
            Timer = new Timer(OnTimerComplete, autoReset, DebounceInterval, Timeout.Infinite);
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
            SelectedListItemIndex = 0;

            var searched_items = await SearchFunc(Text);
            if (MaxItems.HasValue)
                searched_items = searched_items.Take(MaxItems.Value);
            Items = searched_items.ToArray();

            if (Items?.Count() == 0)
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

        private Func<T, object> toStringFunc;

        private string GetItemString(T item)
        {
            if (item == null) 
                return string.Empty;
            if (ToStringFunc != null)
            {
                try
                {
                    return ToStringFunc(item);
                }
                catch (NullReferenceException) { }
                return "null";
            }
            try
            {
                return Converter.Set(item);
            }
            catch (NullReferenceException) { }
            return "null";
        }

        protected virtual void OnInputKeyDown(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Enter":
                    OnEnterKey();
                    break;
                case "ArrowDown":
                    SelectNextItem(+1);
                    break;
                case "ArrowUp":
                    SelectNextItem(-1);
                    break;
            }
            base.onKeyDown(args);
        }

        private void SelectNextItem(int increment)
        {
            if (Items == null || Items.Length == 0)
                return;
            SelectedListItemIndex = Math.Max(0, Math.Min(Items.Length - 1, SelectedListItemIndex + increment));
            ScrollToListItem(SelectedListItemIndex);
            StateHasChanged();
        }

        /// <summary>
        /// We need a random id for the year items in the year list so we can scroll to the item safely in every DatePicker.
        /// </summary>
        private string _componentId = Guid.NewGuid().ToString();

        public async void ScrollToListItem(int index)
        {
            string id = GetListItemId(index);
            await JsRuntime.InvokeVoidAsync("blazorHelpers.scrollToFragment", id);
            StateHasChanged();
        }

        private string GetListItemId(in int index)
        {
            return $"{_componentId}_item{index}";
        }

        private void OnEnterKey()
        {
            if (IsOpen == false)
                return;
            if (Items == null || Items.Length == 0)
                return;
            if (SelectedListItemIndex >= 0 && SelectedListItemIndex < Items.Length)
                SelectOption(Items[SelectedListItemIndex]);
        }

        private void OnInputBlurred(FocusEventArgs args)
        {
            if (!IsOpen)
                CoerceTextToValue();
            base.OnBlurred(args);
        }

        private void CoerceTextToValue()
        {
            if (Value == null)
            {
                Text = null;
                Timer?.Dispose();
                return;
            }
            string actualvalueStr = GetItemString(Value);
            if (!object.Equals(actualvalueStr, Text))
            {
                Text = actualvalueStr;
                Timer?.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            Timer?.Dispose();
            base.Dispose(disposing);
        }
    }
}
