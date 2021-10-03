// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor
{
    public partial class MudSelect<T> : MudBaseInput<T>, IMudSelect, IMudShadowSelect
    {
        private HashSet<T> _selectedValues = new HashSet<T>();
        private bool _dense;
        private string multiSelectionText;
        private bool? _selectAllChecked;
        private MudElement _multiSelectContainer;

        protected string Classname =>
            new CssBuilder("mud-select")
            .AddClass(Class)
            .Build();

        [Inject] private IKeyInterceptor _keyInterceptor { get; set; }

        [Inject] IScrollManager ScrollManager { get; set; }

        private ElementReference _self;

        private int _hilitedItemIndex = 0;

        private Task SelectNextItem() => SelectAdjacentItem(+1);

        private Task SelectPreviousItem() => SelectAdjacentItem(-1);

        private async Task SelectAdjacentItem(int direction)
        {
            if (_items == null || _items.Count == 0)
                return;
            // the loop allows us to jump over disabled items until we reach the next non-disabled one
            for (int i = 0; i < _items.Count; i++)
            {
                _hilitedItemIndex = (_hilitedItemIndex + direction) % _items.Count;
                // modulo of negative numbers is negative, so we add item count to get a valid positive index.
                if (_hilitedItemIndex < 0)
                    _hilitedItemIndex = _items.Count + _hilitedItemIndex;
                if (_items[_hilitedItemIndex].Disabled)
                    continue;
                if (!MultiSelection)
                {
                    _selectedValues.Clear();
                    _selectedValues.Add(_items[_hilitedItemIndex].Value);
                    await SetValueAsync(_items[_hilitedItemIndex].Value, updateText: true);
                    HilightItem(_items[_hilitedItemIndex]);
                    break;
                }
                else
                {
                    // in multiselect mode don't select anything, just hilight.
                    // selecting is done by Enter
                    HilightItem(_items[_hilitedItemIndex]);
                    break;
                }
            }
        }

        private readonly string _componentId = Guid.NewGuid().ToString();

        private string GetListItemId(in int index)
        {
            return $"{_componentId}_item{index}";
        }

        private async Task SelectFirstItem()
        {
            if (_items == null || _items.Count == 0)
                return;
            for (int i = 0; i < _items.Count; i++)
            {
                _hilitedItemIndex = i;
                if (_items[_hilitedItemIndex].Disabled == false)
                {
                    // TODO: MultiSelect!
                    _selectedValues.Clear();
                    _selectedValues.Add(_items[_hilitedItemIndex].Value);
                    await SetValueAsync(_items[_hilitedItemIndex].Value, updateText: true);
                    HilightItem(_items[_hilitedItemIndex]);
                    break;
                }
            }
        }

        private async Task SelectLastItem()
        {
            if (_items == null || _items.Count == 0)
                return;
            _hilitedItemIndex = _items.Count;
            for (int i = 0; i < _items.Count; i++)
            {
                _hilitedItemIndex -= 1;
                if (_items[_hilitedItemIndex].Disabled == false)
                {
                    // TODO: MultiSelect!
                    _selectedValues.Clear();
                    _selectedValues.Add(_items[_hilitedItemIndex].Value);
                    await SetValueAsync(_items[_hilitedItemIndex].Value, updateText: true);
                    var item = _items[_hilitedItemIndex];
                    HilightItem(item);
                    await ScrollManager.ScrollToAsync(item.ItemId, 0, 100, ScrollBehavior.Smooth);
                    break;
                }
            }
        }

        /// <summary>
        /// Add the MudSelectItems here
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// User class names for the popover, separated by space
        /// </summary>
        [Parameter] public string PopoverClass { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all Select items.
        /// </summary>
        [Parameter]
        public bool Dense
        {
            get { return _dense; }
            set { _dense = value; }
        }

        /// <summary>
        /// The Open Select Icon
        /// </summary>
        [Parameter] public string OpenIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// The Close Select Icon
        /// </summary>
        [Parameter] public string CloseIcon { get; set; } = Icons.Material.Filled.ArrowDropUp;

        /// <summary>
        /// If set to true and the MultiSelection option is set to true, a "select all" checkbox is added at the top of the list of items.
        /// </summary>
        [Parameter] public bool SelectAll { get; set; }

        /// <summary>
        /// Define the text of the Select All option.
        /// </summary>
        [Parameter] public string SelectAllText { get; set; } = "Select all";

        /// <summary>
        /// Fires when SelectedValues changes.
        /// </summary>
        [Parameter] public EventCallback<HashSet<T>> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Function to define a customized multiselection text.
        /// </summary>
        [Parameter] public Func<List<string>, string> MultiSelectionTextFunc { get; set; }

        /// <summary>
        /// Parameter to define the delimited string separator.
        /// </summary>
        [Parameter] public string Delimiter { get; set; } = ", ";

        /// <summary>
        /// Set of selected values. If MultiSelection is false it will only ever contain a single value. This property is two-way bindable.
        /// </summary>
        [Parameter]
        public HashSet<T> SelectedValues
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
                if (SelectedValues.Count == set.Count && SelectedValues.All(x => set.Contains(x)))
                    return;
                _selectedValues = new HashSet<T>(set);
                SelectionChangedFromOutside?.Invoke(_selectedValues);
                if (!MultiSelection)
                    SetValueAsync(_selectedValues.FirstOrDefault()).AndForget();
                else
                {
                    //Warning. Here the Converter was not set yet
                    if (MultiSelectionTextFunc != null)
                    {
                        SetCustomizedTextAsync(string.Join(Delimiter, SelectedValues.Select(x => Converter.Set(x))),
                            selectedConvertedValues: SelectedValues.Select(x => Converter.Set(x)).ToList(),
                            multiSelectionTextFunc: MultiSelectionTextFunc).AndForget();
                    }
                    else
                    {
                        SetTextAsync(string.Join(Delimiter, SelectedValues.Select(x => Converter.Set(x)))).AndForget();
                    }
                }
                SelectedValuesChanged.InvokeAsync(new HashSet<T>(SelectedValues));
            }
        }

        private Func<T, string> _toStringFunc = x => x?.ToString();

        private MudInput<string> _elementReference;

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
            // it is impossible to prevent double registration of RenderFragment providers, so we remove them at this stage
            // because their cascading value HideContent has been set now 
            CleanupHiddenItems();
        }

        private void CleanupHiddenItems()
        {
            //// it is impossible to prevent double registration of RenderFragment providers, so we remove them here
            //// because their cascading value HideContent has been set now 
            //_items=_items.Where(x=>x.HideContent==false).ToList();
            //var set=new HashSet<T>();
            //_valueLookup = _items.Where(x=>
            //{
            //    var rv= x.Value != null && !set.Contains(x.Value);
            //    if (x.Value!=null)
            //        set.Add(x.Value);
            //    return rv;
            //}).ToDictionary(x => x.Value);
        }

        /// <summary>
        /// Returns whether or not the Value can be found in items. If not, the Select will display it as a string.
        /// </summary>
        protected bool CanRenderValue
        {
            get
            {
                if (Value == null)
                    return false;
                if (!_shadowLookup.TryGetValue(Value, out var item))
                    return false;
                return (item.ChildContent != null);
            }
        }

        protected bool IsValueInList
        {
            get
            {
                if (Value == null)
                    return false;
                return _shadowLookup.TryGetValue(Value, out var _);
            }
        }

        protected RenderFragment GetSelectedValuePresenter()
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
                    ? SetCustomizedTextAsync(string.Join(Delimiter, SelectedValues.Select(x => Converter.Set(x))),
                        selectedConvertedValues: SelectedValues.Select(x => Converter.Set(x)).ToList(),
                        multiSelectionTextFunc: MultiSelectionTextFunc)
                    : base.UpdateTextPropertyAsync(updateValue);
            }
            else
            {
                return MultiSelection
                    ? SetTextAsync(string.Join(Delimiter, SelectedValues.Select(x => Converter.Set(x))))
                    : base.UpdateTextPropertyAsync(updateValue);
            }
        }

        internal event Action<HashSet<T>> SelectionChangedFromOutside;

        /// <summary>
        /// If true, multiple values can be selected via checkboxes which are automatically shown in the dropdown
        /// </summary>
        [Parameter] public bool MultiSelection { get; set; }

        protected List<MudSelectItem<T>> _items = new();
        protected Dictionary<T, MudSelectItem<T>> _valueLookup = new();
        protected Dictionary<T, MudSelectItem<T>> _shadowLookup = new();

        // note: this must be object to satisfy MudList
        private object _activeItemId = null;

        internal bool Add(MudSelectItem<T> item)
        {
            if (item == null)
                return false;
            bool? result = null;
            if (!_items.Select(x => x.Value).Contains(item.Value))
            {
                _items.Add(item);
                if (item.Value != null)
                {
                    _valueLookup[item.Value] = item;
                    if (item.Value.Equals(Value))
                    {
                        _activeItemId = item.ItemId;
                        result = true;
                    }
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
            if (item.Value != null)
                _valueLookup.Remove(item.Value);
        }

        /// <summary>
        /// Sets the maxheight the Select can have when open.
        /// </summary>
        [Parameter] public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// Set the anchor origin point to determen where the popover will open from.
        /// </summary>
        [Parameter] public Origin AnchorOrigin { get; set; } = Origin.TopCenter;

        /// <summary>
        /// Sets the transform origin point for the popover.
        /// </summary>
        [Parameter] public Origin TransformOrigin { get; set; } = Origin.TopCenter;

        /// <summary>
        /// Sets the direction the Select menu should open.
        /// </summary>
        [Obsolete("Direction is obsolete. Use AnchorOrigin or TransformOrigin instead!", false)]
        [Parameter] public Direction Direction { get; set; } = Direction.Bottom;

        /// <summary>
        /// If true, the Select menu will open either before or after the input (left/right).
        /// </summary>
        [Obsolete("OffsetX is obsolete. Use AnchorOrigin or TransformOrigin instead!", false)]
        [Parameter] public bool OffsetX { get; set; }

        /// <summary>
        /// If true, the Select menu will open either before or after the input (top/bottom).
        /// </summary>
        [Obsolete("OffsetY is obsolete. Use AnchorOrigin or TransformOrigin instead!", false)]
        [Parameter] public bool OffsetY { get; set; }

        /// <summary>
        /// If true, the Select's input will not show any values that are not defined in the dropdown.
        /// This can be useful if Value is bound to a variable which is initialized to a value which is not in the list
        /// and you want the Select to show the label / placeholder instead.
        /// </summary>
        [Parameter] public bool Strict { get; set; }

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter] public bool Clearable { get; set; } = false;

        /// <summary>
        /// If true, prevent scrolling while dropdown is open.
        /// </summary>
        [Parameter] public bool LockScroll { get; set; } = false;

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        internal bool _isOpen;

        public string _currentIcon { get; set; }

        internal Origin _anchorOrigin;
        internal Origin _transformOrigin;

#pragma warning disable CS0618 // This is for backwards compability until Obsolete is removed
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

        public async Task SelectOption(object obj)
        {
            var value = (T)obj;
            if (MultiSelection)
            {
                // multi-selection: menu stays open
                if (!SelectedValues.Contains(value))
                    SelectedValues.Add(value);
                else
                    SelectedValues.Remove(value);

                if (MultiSelectionTextFunc != null)
                {
                    await SetCustomizedTextAsync(string.Join(Delimiter, SelectedValues.Select(x => Converter.Set(x))),
                        selectedConvertedValues: SelectedValues.Select(x => Converter.Set(x)).ToList(),
                        multiSelectionTextFunc: MultiSelectionTextFunc);
                }
                else
                {
                    await SetTextAsync(string.Join(Delimiter, SelectedValues.Select(x => Converter.Set(x))));
                }

                UpdateSelectAllChecked();
                BeginValidate();
            }
            else
            {
                // single selection
                _isOpen = false;
                UpdateIcon();

                if (EqualityComparer<T>.Default.Equals(Value, value))
                {
                    StateHasChanged();
                    return;
                }

                await SetValueAsync(value);
                _elementReference.SetText(Text).AndForget();
                SelectedValues.Clear();
                SelectedValues.Add(value);
                HilightItemForValue(value);
            }

            StateHasChanged();
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
        }

        private MudSelectItem<T> HilightItemForValue(T value)
        {
            if (value == null)
            {
                HilightItem(null);
                return null;
            }
            _valueLookup.TryGetValue(value, out var item);
            HilightItem(item);
            return item;
        }

        private void HilightItem(MudSelectItem<T> item)
        {
            _activeItemId = item?.ItemId;
            //_itemList?.ForceRender();
        }

        private void HilightSelectedValue()
        {
            MudSelectItem<T> item;
            if (MultiSelection)
                HilightItem(item = _items.FirstOrDefault(x => !x.Disabled));
            else
                item = HilightItemForValue(Value);
            _hilitedItemIndex = _items.FindIndex(x => x == item);
        }

        private void UpdateSelectAllChecked()
        {
            if (MultiSelection && SelectAll)
            {
                var oldState = _selectAllChecked;
                if (SelectedValues.Count == 0)
                {
                    _selectAllChecked = false;
                }
                else if (_items.Count == SelectedValues.Count)
                {
                    _selectAllChecked = true;
                }
                else
                {
                    _selectAllChecked = null;
                }

                if (oldState != _selectAllChecked)
                {
                    _multiSelectContainer?.Refresh();
                }
            }
        }

        public void ToggleMenu()
        {
            if (Disabled || ReadOnly)
                return;
            if (_isOpen)
                CloseMenu(true);
            else
                OpenMenu();
        }

        public void OpenMenu()
        {
            if (Disabled || ReadOnly)
                return;
            _isOpen = true;
            HilightSelectedValue();
            UpdateIcon();

            StateHasChanged();
        }

        public async void CloseMenu(bool focusAgain = true)
        {
            _isOpen = false;
            UpdateIcon();
            if (focusAgain == true)
            {
                StateHasChanged();
                await OnBlur.InvokeAsync(new FocusEventArgs());
                _elementReference.FocusAsync().AndForget();
                StateHasChanged();
            }
        }

        private void UpdateIcon()
        {
            _currentIcon = !string.IsNullOrWhiteSpace(AdornmentIcon) ? AdornmentIcon : _isOpen ? CloseIcon : OpenIcon;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            UpdateIcon();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            GetPopoverOrigins(); // Just to keep Obsolete functional until removed.
            UpdateIcon();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _keyInterceptor.Connect(_self, new KeyInterceptorOptions()
                {
                    EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    Keys = {
                        new KeyOptions { Key=" ", PreventDown = "key+none" }, //prevent scrolling page, toggle open/close
                        new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page, instead hilight previous item
                        new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page, instead hilight next item
                        new KeyOptions { Key="Home", PreventDown = "key+none" },
                        new KeyOptions { Key="End", PreventDown = "key+none" },
                        new KeyOptions { Key="Enter", PreventDown = "key+none" },
                        new KeyOptions { Key="/./", SubscribeDown = true, SubscribeUp = true }, // for our users
                    },
                });
                _keyInterceptor.KeyDown += HandleKeyDown;
                _keyInterceptor.KeyUp += HandleKeyUp;
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void CheckGenericTypeMatch(object select_item)
        {
            var itemT = select_item.GetType().GenericTypeArguments[0];
            if (itemT != typeof(T))
                throw new GenericTypeMismatchException("MudSelect", "MudSelectItem", typeof(T), itemT);
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

        /// <summary>
        /// Extra handler for clearing selection.
        /// </summary>
        protected async ValueTask SelectClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await SetValueAsync(default, false);
            await SetTextAsync(default, false);
            SelectedValues.Clear();
            BeginValidate();
            StateHasChanged();
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
            await OnClearButtonClick.InvokeAsync(e);
        }

        protected async Task SetCustomizedTextAsync(string text, bool updateValue = true,
            List<string> selectedConvertedValues = null,
            Func<List<string>, string> multiSelectionTextFunc = null)
        {
            // The Text property of the control is updated
            Text = multiSelectionTextFunc?.Invoke(selectedConvertedValues);

            // The comparison is made on the multiSelectionText variable
            if (multiSelectionText != text)
            {
                multiSelectionText = text;
                if (!string.IsNullOrWhiteSpace(multiSelectionText))
                    Touched = true;
                if (updateValue)
                    await UpdateValuePropertyAsync(false);
                await TextChanged.InvokeAsync(multiSelectionText);
            }
        }

        /// <summary>
        /// Custom checked icon.
        /// </summary>
        [Parameter] public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// Custom unchecked icon.
        /// </summary>
        [Parameter] public string UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// Custom indeterminate icon.
        /// </summary>
        [Parameter] public string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        /// <summary>
        /// The checkbox icon reflects the select all option's state
        /// </summary>
        protected string SelectAllCheckBoxIcon
        {
            get
            {
                return _selectAllChecked.HasValue ? _selectAllChecked.Value ? CheckedIcon : UncheckedIcon : IndeterminateIcon;
            }
        }

        private async void HandleKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly)
                return;
            switch (obj.Key)
            {
                case "Tab":
                    CloseMenu(false);
                    break;
                case "ArrowUp":
                    await SelectPreviousItem();
                    await _elementReference.SetText(Text);
                    break;
                case "ArrowDown":
                    await SelectNextItem();
                    await _elementReference.SetText(Text);
                    break;
                case " ":
                    _isOpen = !_isOpen;
                    break;
                case "Escape":
                    CloseMenu(true);
                    break;
                case "Home":
                    await SelectFirstItem();
                    break;
                case "End":
                    await SelectLastItem();
                    break;
                case "Enter":
                    if (!MultiSelection)
                    {
                        _isOpen = !_isOpen;
                        break;
                    }
                    else
                    {
                        if (_isOpen == false)
                        {
                            _isOpen = true;
                            break;
                        }
                        else
                        {
                            await SelectOption(_items[_hilitedItemIndex].Value);
                            await _elementReference.SetText(Text);
                            break;
                        }
                    }
            }
            OnKeyDown.InvokeAsync(obj).AndForget();

        }

        private void HandleKeyUp(KeyboardEventArgs obj)
        {
            OnKeyUp.InvokeAsync(obj).AndForget();
        }

        /// <summary>
        /// Clear the selection
        /// </summary>
        public async Task ClearAsync()
        {
            await SetValueAsync(default, false);
            await SetTextAsync(default, false);
            SelectedValues.Clear();
            BeginValidate();
            StateHasChanged();
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
        }

        private async Task SelectAllClickAsync()
        {
            // Manage the fake tri-state of a checkbox
            if (!_selectAllChecked.HasValue)
            {
                _selectAllChecked = true;
            }
            else if (_selectAllChecked.Value)
            {
                _selectAllChecked = false;
            }
            else
            {
                _selectAllChecked = true;
            }

            // Define the items selection
            if (_selectAllChecked.HasValue)
            {
                if (_selectAllChecked.Value)
                {
                    foreach (var item in _items)
                    {
                        if (item != null && !item.IsSelected)
                        {
                            await SelectOption(item.Value);
                        }
                    }
                }
                else
                {
                    await ClearAsync();
                }
            }
        }

        public void RegisterShadowItem(MudSelectItem<T> item)
        {
            if (item==null || item.Value == null)
                return;
            _shadowLookup[item.Value] = item;
        }

        public void UnregisterShadowItem(MudSelectItem<T> item)
        {
            if (item == null || item.Value == null)
                return;
            _shadowLookup.Remove(item.Value);
        }
    }
}
