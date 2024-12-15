// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A component for choosing an item from a list of options.
    /// </summary>
    /// <typeparam name="T">The kind of object being selected.</typeparam>
    /// <seealso cref="MudSelectItem{T}"/>
    public partial class MudSelect<T> : MudBaseInput<T>, IMudSelect, IMudShadowSelect
    {
        private string? _activeItemId;
        private bool? _selectAllChecked;
        private string? _multiSelectionText;
        private IEqualityComparer<T?>? _comparer;
        private TaskCompletionSource? _renderComplete;
        private MudInput<string> _elementReference = null!;
        private HashSet<T?> _selectedValues = new HashSet<T?>();
        protected internal List<MudSelectItem<T>> _items = new();
        private string _elementId = Identifier.Create("select");

        protected string OuterClassname =>
            new CssBuilder("mud-select")
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

        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        [Inject]
        private IScrollManager ScrollManager { get; set; } = null!;

        private Task SelectNextItem() => SelectAdjacentItem(+1);

        private Task SelectPreviousItem() => SelectAdjacentItem(-1);

        private async Task SelectAdjacentItem(int direction)
        {
            if (_items.Count == 0)
                return;
            var index = _items.FindIndex(x => x.ItemId == _activeItemId);
            if (direction < 0 && index < 0)
                index = 0;
            MudSelectItem<T>? item = null;
            // the loop allows us to jump over disabled items until we reach the next non-disabled one
            for (var i = 0; i < _items.Count; i++)
            {
                index += direction;
                if (index < 0)
                    index = 0;
                if (index >= _items.Count)
                    index = _items.Count - 1;
                if (_items[index].Disabled)
                    continue;
                item = _items[index];
                if (!MultiSelection)
                {
                    _selectedValues.Clear();
                    _selectedValues.Add(item.Value);
                    await SetValueAsync(item.Value, updateText: true);
                    HighlightItem(item);
                    break;
                }

                // in multiselect mode don't select anything, just highlight.
                // selecting is done by Enter
                HighlightItem(item);
                break;
            }
            await _elementReference.SetText(Text);
            await ScrollToItemAsync(item);
        }
        private ValueTask ScrollToItemAsync(MudSelectItem<T>? item)
            => item != null ? ScrollManager.ScrollToListItemAsync(item.ItemId) : ValueTask.CompletedTask;

        private async Task SelectFirstItem(string? startChar = null)
        {
            if (_items.Count == 0)
                return;
            var items = _items.Where(x => !x.Disabled);
            if (!string.IsNullOrWhiteSpace(startChar))
            {
                // find first item that starts with the letter
                var currentItem = items.FirstOrDefault(x => x.ItemId == _activeItemId);
                if (currentItem != null &&
                    Converter.Set(currentItem.Value)?.ToLowerInvariant().StartsWith(startChar) == true)
                {
                    // this will step through all items that start with the same letter if pressed multiple times
                    items = items.SkipWhile(x => x != currentItem).Skip(1);
                }
                items = items.Where(x => Converter.Set(x.Value)?.ToLowerInvariant().StartsWith(startChar) == true);
            }
            var item = items.FirstOrDefault();
            if (item == null)
                return;
            if (!MultiSelection)
            {
                _selectedValues.Clear();
                _selectedValues.Add(item.Value);
                await SetValueAsync(item.Value, updateText: true);
                HighlightItem(item);
            }
            else
            {
                HighlightItem(item);
            }
            await _elementReference.SetText(Text);
            await ScrollToItemAsync(item);
        }

        private async Task SelectLastItem()
        {
            if (_items.Count == 0)
                return;
            var item = _items.LastOrDefault(x => !x.Disabled);
            if (item == null)
                return;
            if (!MultiSelection)
            {
                _selectedValues.Clear();
                _selectedValues.Add(item.Value);
                await SetValueAsync(item.Value, updateText: true);
                HighlightItem(item);
            }
            else
            {
                HighlightItem(item);
            }
            await _elementReference.SetText(Text);
            await ScrollToItemAsync(item);
        }

        /// <summary>
        /// The behavior of the drop-down menu.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DropdownSettings.Fixed" /> = <c>false</c> and <see cref="DropdownSettings.OverflowBehavior" /> = <see cref="OverflowBehavior.FlipOnOpen" />.
        /// </remarks>
        [Category(CategoryTypes.Popover.Behavior)]
        [Parameter]
        public DropdownSettings DropdownSettings { get; set; }

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
        /// The CSS classes applied to the input.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter]
        public string? InputClass { get; set; }

        /// <summary>
        /// Occurs when this drop-down opens.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Behavior)]
        [Parameter]
        public EventCallback OnOpen { get; set; }

        /// <summary>
        /// Occurs when this drop-down closes.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Behavior)]
        [Parameter]
        public EventCallback OnClose { get; set; }

        /// <summary>
        /// The content within this component, typically a list of <see cref="MudSelectItem{T}"/> components.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The CSS classes applied to the popover.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string? PopoverClass { get; set; }

        /// <summary>
        /// The CSS classes applied to the internal list.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string? ListClass { get; set; }

        /// <summary>
        /// Uses compact vertical padding for all items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The icon for opening the popover of items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowDropDown"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string OpenIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// The icon for closing the popover of items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowDropUp"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string CloseIcon { get; set; } = Icons.Material.Filled.ArrowDropUp;

        /// <summary>
        /// Shows a "Select all" checkbox to select all items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Only applies when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool SelectAll { get; set; }

        /// <summary>
        /// The text of the "Select all" checkbox.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>"Select all"</c>.  Only applies when <see cref="SelectAll"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string SelectAllText { get; set; } = "Select all";

        /// <summary>
        /// Occurs when <see cref="SelectedValues"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<IEnumerable<T?>?> SelectedValuesChanged { get; set; }

        /// <summary>
        /// The custom function for setting the <c>Text</c> from a list of selected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Func<List<string?>?, string>? MultiSelectionTextFunc { get; set; }

        /// <summary>
        /// The string used to separate multiple selected values.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>", "</c>.  Only applies when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string Delimiter { get; set; } = ", ";

        /// <summary>
        /// The currently selected values.
        /// </summary>
        /// <remarks>
        /// When <see cref="MultiSelection"/> is <c>false</c>, only one value will be returned.  When this value changes, <see cref="SelectedValuesChanged"/> occurs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public IEnumerable<T?>? SelectedValues
        {
            get
            {
                return _selectedValues;
            }
            set
            {
                var set = value ?? new HashSet<T?>(_comparer);
                var selectedValues = SelectedValues ?? new HashSet<T>(_comparer);
                if (selectedValues.Count() == set.Count() && _selectedValues.All(x => set.Contains(x)))
                    return;
                _selectedValues = new HashSet<T?>(set, _comparer);
                SelectionChangedFromOutside?.Invoke(_selectedValues);
                if (!MultiSelection)
                    SetValueAsync(_selectedValues.FirstOrDefault()).CatchAndLog();
                else
                {
                    //Warning. Here the Converter was not set yet
                    if (MultiSelectionTextFunc != null)
                    {
                        SetCustomizedTextAsync(string.Join(Delimiter, _selectedValues.Select(Converter.Set)),
                            selectedConvertedValues: _selectedValues.Select(Converter.Set).ToList(),
                            multiSelectionTextFunc: MultiSelectionTextFunc).CatchAndLog();
                    }
                    else
                    {
                        SetTextAsync(string.Join(Delimiter, _selectedValues.Select(Converter.Set)), updateValue: false).CatchAndLog();
                    }
                }
                SelectedValuesChanged.InvokeAsync(new HashSet<T?>(_selectedValues, _comparer));
                if (MultiSelection && typeof(T) == typeof(string))
                    SetValueAsync((T?)(object?)Text, updateText: false).CatchAndLog();
            }
        }

        /// <summary>
        /// The comparer for testing equality of selected values.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public IEqualityComparer<T?>? Comparer
        {
            get => _comparer;
            set
            {
                _comparer = value;
                // Apply comparer and refresh selected values
                _selectedValues = new HashSet<T?>(_selectedValues, _comparer);
                SelectedValues = _selectedValues;
            }
        }

        private Func<T?, string?>? _toStringFunc = x => x?.ToString();

        /// <summary>
        /// The function for the <c>Text</c> in drop-down items.
        /// </summary>
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
                    //GetFunc = LookupValue,
                };
            }
        }

        public MudSelect()
        {
            Adornment = Adornment.End;
            IconSize = Size.Medium;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender && Value != null)
            {
                // we need to render the initial Value which is not possible without the items
                // which supply the RenderFragment. So in this case, a second render is necessary
                StateHasChanged();
            }
            UpdateSelectAllChecked();
            lock (this)
            {
                if (_renderComplete != null)
                {
                    _renderComplete.TrySetResult();
                    _renderComplete = null;
                }
            }
        }

        private Task WaitForRender()
        {
            Task? t;
            lock (this)
            {
                if (_renderComplete != null)
                    return _renderComplete.Task;
                _renderComplete = new TaskCompletionSource();
                t = _renderComplete.Task;
            }
            StateHasChanged();
            return t;
        }

        /// <summary>
        /// Whether the <c>Value</c> can be found in the list of <see cref="Items"/>.
        /// </summary>
        /// <remarks>
        /// When <c>false</c>, the <c>Value</c> will be displayed as a string.
        /// </remarks>
        protected bool CanRenderValue
        {
            get
            {
                if (MultiSelection || Value == null)
                    return false;
                if (!_shadowLookup.TryGetValue(Value, out var item))
                    return false;
                return item.ChildContent != null;
            }
        }

        protected bool IsValueInList
        {
            get
            {
                if (Value == null)
                    return false;
                return _shadowLookup.TryGetValue(Value, out _);
            }
        }

        protected RenderFragment? GetSelectedValuePresenter()
        {
            if (Value == null)
                return null;
            if (!_shadowLookup.TryGetValue(Value, out var item))
                return null; //<-- for now. we'll add a custom template to present values (set from outside) which are not on the list?
            return item.ChildContent;
        }

        protected override Task UpdateValuePropertyAsync(bool updateText)
        {
            // For MultiSelection of non-string T's we don't update the Value!!!
            if (typeof(T) == typeof(string) || !MultiSelection)
                base.UpdateValuePropertyAsync(updateText);
            return Task.CompletedTask;
        }

        protected override Task UpdateTextPropertyAsync(bool updateValue)
        {
            // when multiselection is true, we return
            // a comma separated list of selected values
            if (MultiSelectionTextFunc != null)
            {
                return MultiSelection
                    ? SetCustomizedTextAsync(string.Join(Delimiter, SelectedValues!.Select(Converter.Set)),
                        selectedConvertedValues: SelectedValues!.Select(Converter.Set).ToList(),
                        multiSelectionTextFunc: MultiSelectionTextFunc)
                    : base.UpdateTextPropertyAsync(updateValue);
            }

            return MultiSelection
                ? SetTextAsync(string.Join(Delimiter, SelectedValues!.Select(Converter.Set)))
                : base.UpdateTextPropertyAsync(updateValue);
        }

        internal event Action<ICollection<T?>>? SelectionChangedFromOutside;

        private bool _multiSelection;

        /// <summary>
        /// Allows multiple values to be selected via checkboxes.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>false</c>, only one value can be selected at a time.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool MultiSelection
        {
            get => _multiSelection;
            set
            {
                if (_multiSelection != value)
                {
                    _multiSelection = value;
                    UpdateTextPropertyAsync(false).CatchAndLog();
                }
            }
        }

        /// <summary>
        /// The list of choices the user can select.
        /// </summary>
        /// <remarks>
        /// Use <see cref="MudSelectItem{T}"/> components to provide more items.
        /// </remarks>
        public IReadOnlyList<MudSelectItem<T>> Items => _items;

        protected Dictionary<NullableObject<T?>, MudSelectItem<T>> _valueLookup = new();
        protected Dictionary<NullableObject<T?>, MudSelectItem<T>> _shadowLookup = new();

        internal bool Add(MudSelectItem<T>? item)
        {
            if (item == null)
                return false;
            bool? result = null;
            if (!_items.Select(x => x.Value).Contains(item.Value))
            {
                _items.Add(item);

                if (item.Value is not null)
                {
                    _valueLookup[item.Value] = item;
                    if (item.Value.Equals(Value) && !MultiSelection)
                        result = true;
                }
            }
            UpdateSelectAllChecked();
            if (result.HasValue == false)
            {
                result = item.Value?.Equals(Value);
            }
            return result == true;
        }

        internal void Remove(MudSelectItem<T> item)
        {
            _items.Remove(item);
            if (item.Value is not null)
            {
                _valueLookup.Remove(item.Value);
            }
        }

        /// <summary>
        /// The maximum height, in pixels, of the popover of items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>300</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public int MaxHeight { get; set; } = 300;

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
        /// Restricts the selected values to the ones defined in <see cref="MudSelectItem{T}"/> items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, any values not defined will not be displayed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Strict { get; set; }

        /// <summary>
        /// Shows a button for clearing any selected values.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the <see cref="ClearIcon"/> can be used to control the icon, and <see cref="OnClearButtonClick"/> occurs when the clear button is clicked.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// The icon displayed for the clear button when <see cref="Clearable"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Clear"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string ClearIcon { get; set; } = Icons.Material.Filled.Clear;

        /// <summary>
        /// Prevents scrolling while the dropdown is open.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool LockScroll { get; set; } = false;

        /// <summary>
        /// Occurs when the clear button is clicked.
        /// </summary>
        /// <remarks>
        /// Only occurs when <see cref="Clearable"/> is <c>true</c>.   This event occurs after the <c>Text</c> and <c>Value</c> have been cleared.
        /// </remarks>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        internal bool _open;

        /// <summary>
        /// The current adornment icon to display.
        /// </summary>
        /// <remarks>
        /// If an <c>AdornmentIcon</c> is set, it is returned.  Otherwise, either <see cref="OpenIcon"/> or <see cref="CloseIcon"/> is returned depending on whether the drop-down is open.
        /// </remarks>
        internal string? _currentIcon { get; set; }

        /// <summary>
        /// Selects the item at the specified index.
        /// </summary>
        /// <param name="index">The ordinal of the item to select (starting at <c>0</c>).  When <see cref="MultiSelection"/> is <c>true</c>, the item will be added to the selected items.</param>
        public async Task SelectOption(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                if (!MultiSelection)
                    await CloseMenu();
                return;
            }
            await SelectOption(_items[index].Value);
        }

        /// <summary>
        /// Selects the item with the specified value.
        /// </summary>
        /// <param name="obj">The value to select.  When <see cref="MultiSelection"/> is <c>true</c>, the selection is cleared if it was already selected.</param>
        public async Task SelectOption(object? obj)
        {
            var value = (T?)obj;
            if (MultiSelection)
            {
                // multi-selection: menu stays open
                if (!_selectedValues.Add(value))
                    _selectedValues.Remove(value);

                if (MultiSelectionTextFunc != null)
                {
                    await SetCustomizedTextAsync(string.Join(Delimiter, SelectedValues!.Select(Converter.Set)),
                        selectedConvertedValues: SelectedValues!.Select(Converter.Set).ToList(),
                        multiSelectionTextFunc: MultiSelectionTextFunc);
                }
                else
                {
                    await SetTextAsync(string.Join(Delimiter, SelectedValues!.Select(Converter.Set)), updateValue: false);
                }

                UpdateSelectAllChecked();
                await BeginValidateAsync();
            }
            else
            {
                // single selection
                // CloseMenu(true) doesn't close popover in BSS
                await CloseMenu(false);

                if (EqualityComparer<T>.Default.Equals(Value, value))
                {
                    StateHasChanged();
                    return;
                }

                await SetValueAsync(value);
                _elementReference.SetText(Text).CatchAndLog();
                _selectedValues.Clear();
                _selectedValues.Add(value);
            }

            HighlightItemForValueAsync(value);
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
            if (MultiSelection && typeof(T) == typeof(string))
                await SetValueAsync((T?)(object?)Text, updateText: false);
            await InvokeAsync(StateHasChanged);
        }

        private async void HighlightItemForValueAsync(T? value)
        {
            if (value == null)
            {
                HighlightItem(null);
                return;
            }
            await WaitForRender();
            _valueLookup.TryGetValue(value, out var item);
            HighlightItem(item);
        }

        private async void HighlightItem(MudSelectItem<T>? item)
        {
            _activeItemId = item?.ItemId;
            // we need to make sure we are just after a render here or else there will be race conditions
            await WaitForRender();
            // Note: this is a hack, but I found no other way to make the list highlight the currently highlighted item
            // without the delay it always shows the previously highlighted item because the popup items don't exist yet
            // they are only registered after they are rendered, so we need to render again!
            await Task.Delay(1);
            StateHasChanged();
        }

        private async Task HighlightSelectedValue()
        {
            await WaitForRender();
            if (MultiSelection)
                HighlightItem(_items.FirstOrDefault(x => !x.Disabled));
            else
                HighlightItemForValueAsync(Value);
        }

        private void UpdateSelectAllChecked()
        {
            if (MultiSelection && SelectAll)
            {
                if (_selectedValues.Count == 0)
                {
                    _selectAllChecked = false;
                }
                else if (_items.Count(x => !x.Disabled) == _selectedValues.Count)
                {
                    _selectAllChecked = true;
                }
                else
                {
                    _selectAllChecked = null;
                }
            }
        }

        /// <summary>
        /// Opens or closes the drop-down menu.
        /// </summary>
        /// <remarks>
        /// Has no effect if <c>Disabled</c> or <c>ReadOnly</c> is <c>true</c>.
        /// </remarks>
        public async Task ToggleMenu()
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            if (_open)
                await CloseMenu(true);
            else
                await OpenMenu();
        }

        /// <summary>
        /// Opens the drop-down menu.
        /// </summary>
        /// <remarks>
        /// Has no effect if <c>Disabled</c> or <c>ReadOnly</c> is <c>true</c>.
        /// </remarks>
        public async Task OpenMenu()
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            _open = true;
            UpdateIcon();
            StateHasChanged();
            await HighlightSelectedValue();
            //Scroll the active item on each opening
            if (_activeItemId != null)
            {
                var index = _items.FindIndex(x => x.ItemId == _activeItemId);
                if (index > 0)
                {
                    var item = _items[index];
                    await ScrollToItemAsync(item);
                }
            }
            //disable escape propagation: if selectmenu is open, only the select popover should close and underlying components should not handle escape key
            await KeyInterceptorService.UpdateKeyAsync(_elementId, new("Escape", stopDown: "key+none"));

            await OnOpen.InvokeAsync();
        }

        /// <summary>
        /// Closes the drop-down menu.
        /// </summary>
        /// <remarks>
        /// Has no effect if <c>Disabled</c> or <c>ReadOnly</c> is <c>true</c>.
        /// </remarks>
        public async Task CloseMenu(bool focusAgain = true)
        {
            _open = false;
            UpdateIcon();
            if (focusAgain)
            {
                StateHasChanged();
                await OnBlur.InvokeAsync(new FocusEventArgs());
                _elementReference.FocusAsync().CatchAndLog(ignoreExceptions: true);
                StateHasChanged();
            }

            //enable escape propagation: the select popover was closed, now underlying components are allowed to handle escape key
            await KeyInterceptorService.UpdateKeyAsync(_elementId, new("Escape", stopDown: "none"));

            await OnClose.InvokeAsync();
        }

        private void UpdateIcon()
        {
            _currentIcon = !string.IsNullOrWhiteSpace(AdornmentIcon) ? AdornmentIcon : _open ? CloseIcon : OpenIcon;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            UpdateIcon();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            UpdateIcon();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var options = new KeyInterceptorOptions(
                    "mud-input-control",
                    [
                        // prevent scrolling page, toggle open/close
                        new(" ", preventDown: "key+none"),
                        // prevent scrolling page, instead highlight previous item
                        new("ArrowUp", preventDown: "key+none"),
                        // prevent scrolling page, instead highlight next item
                        new("ArrowDown", preventDown: "key+none"),
                        new("Home", preventDown: "key+none"),
                        new("End", preventDown: "key+none"),
                        new("Escape"),
                        new("Enter", preventDown: "key+none"),
                        new("NumpadEnter", preventDown: "key+none"),
                        // select all items instead of all page text
                        new("a", preventDown: "key+ctrl"),
                        // select all items instead of all page text
                        new("A", preventDown: "key+ctrl"),
                        // for our users
                        new("/./", subscribeDown: true, subscribeUp: true)
                    ]);

                await KeyInterceptorService.SubscribeAsync(_elementId, options, keyDown: HandleKeyDownAsync, keyUp: HandleKeyUpAsync);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Throws an exception if the specified item is not compatible with this component.
        /// </summary>
        /// <param name="selectItem">The item to compare.  Should be of type <c>T</c> for this component.</param>
        public void CheckGenericTypeMatch(object selectItem)
        {
            var itemT = selectItem.GetType().GenericTypeArguments[0];
            if (itemT != typeof(T))
                throw new GenericTypeMismatchException("MudSelect", "MudSelectItem", typeof(T), itemT);
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

        /// <summary>
        /// Selects the text within this component.
        /// </summary>
        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        /// <summary>
        /// Selects a portion of text within this component.
        /// </summary>
        /// <param name="pos1">The index of the first character to select.  (Starting at <c>0</c>.)</param>
        /// <param name="pos2">The index of the last character to select.</param>
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        /// <summary>
        /// Occurs when the <c>Clear</c> button has been clicked.
        /// </summary>
        /// <remarks>
        /// This is the first event raised when the clear button is clicked.  
        /// The <see cref="SelectedValues"/> are cleared and the <see cref="OnClearButtonClick"/> event is raised.
        /// </remarks>
        protected async ValueTask SelectClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await SetValueAsync(default, false);
            await SetTextAsync(default, false);
            _selectedValues.Clear();
            await BeginValidateAsync();
            StateHasChanged();
            await SelectedValuesChanged.InvokeAsync(_selectedValues);
            await OnClearButtonClick.InvokeAsync(e);
        }

        protected async Task SetCustomizedTextAsync(string text, bool updateValue = true,
            List<string?>? selectedConvertedValues = null,
            Func<List<string?>?, string>? multiSelectionTextFunc = null)
        {
            // The Text property of the control is updated
            Text = multiSelectionTextFunc?.Invoke(selectedConvertedValues);

            // The comparison is made on the multiSelectionText variable
            if (_multiSelectionText != text)
            {
                _multiSelectionText = text;
                if (!string.IsNullOrWhiteSpace(_multiSelectionText))
                    Touched = true;
                if (updateValue)
                    await UpdateValuePropertyAsync(false);
                await TextChanged.InvokeAsync(_multiSelectionText);
            }
        }

        /// <summary>
        /// The icon used for selected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.CheckBox"/>.  Only applies when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// The icon used for unselected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.CheckBoxOutlineBlank"/>.  Only applies when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// The icon used when at least one, but not all, items are selected.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.IndeterminateCheckBox"/>.  Only applies when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        /// <summary>
        /// The icon to display whether all, none, or some items are selected.
        /// </summary>
        /// <remarks>
        /// Only applies when <see cref="MultiSelection"/> is <c>true</c>.  
        /// If all items are selected, <see cref="CheckedIcon"/> is returned.
        /// If no items are selected, <see cref="UncheckedIcon"/> is returned.  
        /// Otherwise, <see cref="IndeterminateIcon"/> is returned.
        /// </remarks>
        protected string SelectAllCheckBoxIcon
        {
            get => _selectAllChecked.HasValue ? _selectAllChecked.Value ? CheckedIcon : UncheckedIcon : IndeterminateIcon;
        }

        internal async Task HandleKeyDownAsync(KeyboardEventArgs obj)
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            var key = obj.Key.ToLowerInvariant();
            if (_open && key.Length == 1 && key != " " && !(obj.CtrlKey || obj.ShiftKey || obj.AltKey || obj.MetaKey))
            {
                await SelectFirstItem(key);
                return;
            }
            switch (obj.Key)
            {
                case "Tab":
                    await CloseMenu(false);
                    break;
                case "ArrowUp":
                    if (obj.AltKey)
                    {
                        await CloseMenu();
                        break;
                    }

                    if (_open == false)
                    {
                        await OpenMenu();
                        break;
                    }

                    await SelectPreviousItem();
                    break;
                case "ArrowDown":
                    if (obj.AltKey)
                    {
                        await OpenMenu();
                        break;
                    }

                    if (_open == false)
                    {
                        await OpenMenu();
                        break;
                    }

                    await SelectNextItem();
                    break;
                case " ":
                    await ToggleMenu();
                    break;
                case "Escape":
                    await CloseMenu(true);
                    break;
                case "Home":
                    await SelectFirstItem();
                    break;
                case "End":
                    await SelectLastItem();
                    break;
                case "Enter":
                case "NumpadEnter":
                    var index = _items.FindIndex(x => x.ItemId == _activeItemId);
                    if (!MultiSelection)
                    {
                        if (!_open)
                        {
                            await OpenMenu();
                            break;
                        }

                        // this also closes the menu
                        await SelectOption(index);
                        break;
                    }

                    if (!_open)
                    {
                        await OpenMenu();
                        break;
                    }

                    await SelectOption(index);
                    await _elementReference.SetText(Text);
                    break;
                case "a":
                case "A":
                    if (obj.CtrlKey)
                    {
                        if (MultiSelection)
                        {
                            await SelectAllClickAsync();
                            //If we didn't add delay, it won't work.
                            await WaitForRender();
                            await Task.Delay(1);
                            StateHasChanged();
                            //It only works when selecting all, not render unselect all.
                            //UpdateSelectAllChecked();
                        }
                    }
                    break;
            }

            await OnKeyDown.InvokeAsync(obj);
        }

        internal Task HandleKeyUpAsync(KeyboardEventArgs obj)
        {
            return OnKeyUp.InvokeAsync(obj);
        }

        /// <summary>
        /// Clears all selections.
        /// </summary>
        public async Task Clear()
        {
            await SetValueAsync(default, false);
            await SetTextAsync(default, false);
            _selectedValues.Clear();
            await BeginValidateAsync();
            StateHasChanged();
            await SelectedValuesChanged.InvokeAsync(_selectedValues);
        }

        private async Task SelectAllClickAsync()
        {
            // Manage the fake tri-state of a checkbox
            if (!_selectAllChecked.HasValue)
                _selectAllChecked = true;
            else if (_selectAllChecked.Value)
                _selectAllChecked = false;
            else
                _selectAllChecked = true;
            // Define the items selection
            if (_selectAllChecked.Value)
                await SelectAllItems();
            else
                await Clear();
        }

        private async Task SelectAllItems()
        {
            if (!MultiSelection)
                return;
            var selectedValues = new HashSet<T?>(_items.Where(x => !x.Disabled && x.Value != null).Select(x => x.Value), _comparer);
            _selectedValues = new HashSet<T?>(selectedValues, _comparer);
            if (MultiSelectionTextFunc != null)
            {
                await SetCustomizedTextAsync(string.Join(Delimiter, SelectedValues!.Select(Converter.Set)),
                    selectedConvertedValues: SelectedValues!.Select(Converter.Set).ToList(),
                    multiSelectionTextFunc: MultiSelectionTextFunc);
            }
            else
            {
                await SetTextAsync(string.Join(Delimiter, SelectedValues!.Select(Converter.Set)), updateValue: false);
            }
            UpdateSelectAllChecked();
            _selectedValues = selectedValues; // need to force selected values because Blazor overwrites it under certain circumstances due to changes of Text or Value
            await BeginValidateAsync();
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
            if (MultiSelection && typeof(T) == typeof(string))
                SetValueAsync((T?)(object?)Text, updateText: false).CatchAndLog();
        }

        /// <summary>
        /// Links a selection item to this component.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void RegisterShadowItem(MudSelectItem<T>? item)
        {
            if (item == null || item.Value == null)
                return;
            _shadowLookup[item.Value] = item;
        }

        /// <summary>
        /// Unregisters a selection item to this component.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void UnregisterShadowItem(MudSelectItem<T>? item)
        {
            if (item == null || item.Value == null)
                return;
            _shadowLookup.Remove(item.Value);
        }

        private async Task OnFocusOutAsync(FocusEventArgs focusEventArgs)
        {
            if (_open)
            {
                // when the menu is open we immediately get back the focus if we lose it (i.e. because of checkboxes in multi-select)
                // otherwise we can't receive key strokes any longer
                await FocusAsync();
            }
        }

        internal Task OnBlurAsync(FocusEventArgs obj)
        {
            return base.OnBlur.InvokeAsync(obj);
        }

        /// <inheritdoc />
        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            if (IsJSRuntimeAvailable)
            {
                await KeyInterceptorService.UnsubscribeAsync(_elementId);
            }
        }

        /// <summary>
        /// Gets whether the value is currently selected.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>When <c>true</c>, the specified value exists in <see cref="SelectedValues"/>.</returns>
        protected override bool HasValue(T? value)
        {
            // Fixes issue #4328

            if (MultiSelection)
                return SelectedValues?.Any() ?? false;
            return base.HasValue(value);
        }

        /// <summary>
        /// Forces the <see cref="SelectedValuesChanged"/> event to occur.
        /// </summary>
        public override async Task ForceUpdate()
        {
            await base.ForceUpdate();
            if (MultiSelection == false)
            {
                SelectedValues = new HashSet<T?>(_comparer) { Value };
            }
            else
            {
                await SelectedValuesChanged.InvokeAsync(new HashSet<T?>(SelectedValues!, _comparer));
            }
        }
    }
}
