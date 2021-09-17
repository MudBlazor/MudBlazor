﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor
{
    public partial class MudSelect<T> : MudBaseInput<T>, IMudSelect
    {
        private HashSet<T> _selectedValues = new HashSet<T>();
        private bool _dense;
        private string multiSelectionText;
        private bool? _selectAllChecked;

        protected string Classname =>
            new CssBuilder("mud-select")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Add the MudSelectItems here
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        private int itemIndex = 0;

        private async Task SelectNextItem()
        {
            if (MultiSelection == false)
            {
                if (_items == null || _items.Count == 0)
                    return;
                string val = Converter.Set(Value);
                itemIndex = _items.FindIndex(e => Converter.Set(e.Value) == val);
                
                for (int i = 0; i <= _items.Count; i++)
                {
                    itemIndex = itemIndex + 1;
                    if (_items.Count <= itemIndex)
                    {
                        break;
                    }
                    else if (itemIndex == -1 && _items[itemIndex].Disabled == false)
                    {
                        _selectedValues.Clear();
                        _selectedValues.Add(_items[itemIndex].Value);
                        await SetValueAsync(_items[itemIndex].Value, updateText: true);
                        break;
                    }
                    else if (itemIndex <= _items.Count && _items[itemIndex].Disabled == false)
                    {
                        _selectedValues.Clear();
                        _selectedValues.Add(_items[itemIndex].Value);
                        await SetValueAsync(_items[itemIndex].Value, updateText: true);
                        break;
                    }
                }

            }

        }

        private async Task SelectPreviousItem()
        {
            if (MultiSelection == false)
            {
                if (_items == null || _items.Count == 0)
                    return;
                string val = Converter.Set(Value);
                itemIndex = _items.FindIndex(e => Converter.Set(e.Value) == val);

                for (int i = 0; i <= _items.Count; i++)
                {
                    itemIndex = itemIndex - 1;
                    if (itemIndex < 0)
                    {
                        break;
                    }
                    else if (_items[itemIndex].Disabled == false)
                    {
                        _selectedValues.Clear();
                        _selectedValues.Add(_items[itemIndex].Value);
                        await SetValueAsync(_items[itemIndex].Value, updateText: true);
                        break;
                    }
                }

            }
        }

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
            set
            {
                // Ensure that when dense is applied we set the margin on the input controls
                _dense = value;
                Margin = _dense ? Margin.Dense : Margin.None;
            }
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

            _keyPressPreventDefault = true;
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
                if (!_value_lookup.TryGetValue(Value, out var item))
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
                return _value_lookup.TryGetValue(Value, out var _);
            }
        }

        protected RenderFragment GetSelectedValuePresenter()
        {
            if (Value == null)
                return null;
            if (!_value_lookup.TryGetValue(Value, out var selected_item))
                return null; //<-- for now. we'll add a custom template to present values (set from outside) which are not on the list?
            return selected_item.ChildContent;
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
        protected Dictionary<T, MudSelectItem<T>> _value_lookup = new();
        internal void Add(MudSelectItem<T> item)
        {
            // Check to avoid duplicate items based on their value
            // It fixes that the number of real items is correct in the items list
            if (!_items.Select(x => x.Value).Contains(item.Value))
            {
                _items.Add(item);
                if (item.Value != null)
                    _value_lookup[item.Value] = item;
            }
        }

        internal void Remove(MudSelectItem<T> item)
        {
            if (_items.Contains(item))
            {
                _items.Remove(item);
                if (item.Value != null)
                    _value_lookup.Remove(item.Value);
            }
        }

        /// <summary>
        /// Sets the maxheight the Select can have when open.
        /// </summary>
        [Parameter] public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// Sets the direction the Select menu should open.
        /// </summary>
        [Parameter] public Direction Direction { get; set; } = Direction.Bottom;

        /// <summary>
        /// If true, the Select menu will open either before or after the input (left/right).
        /// </summary>
        [Parameter] public bool OffsetX { get; set; }

        /// <summary>
        /// If true, the Select menu will open either before or after the input (top/bottom).
        /// </summary>
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
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        internal bool _isOpen;

        public string _currentIcon { get; set; }

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
                SelectedValues.Clear();
                SelectedValues.Add(value);
                await _elementReference.FocusAsync();
                StateHasChanged();
            }
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
        }

        private void UpdateSelectAllChecked()
        {
            if (MultiSelection && SelectAll)
            {
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
            }
        }

        public void ToggleMenu()
        {
            if (Disabled || ReadOnly)
                return;
            if (_isOpen)
                CloseMenu();
            else
                OpenMenu();
        }

        public void OpenMenu()
        {
            if (Disabled || ReadOnly)
                return;
            _isOpen = true;
            UpdateIcon();
            
            StateHasChanged();
        }

        public async void CloseMenu()
        {
            _isOpen = false;
            UpdateIcon();
            StateHasChanged();
            await OnBlur.InvokeAsync(new FocusEventArgs());
            await _elementReference.FocusAsync();
            StateHasChanged();
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
            UpdateIcon();
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

        private bool _keyPressPreventDefault;

        private int _key = 0;

        protected void InterceptKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly)
                return;
            if (obj.Key == " ")
            {
                _keyPressPreventDefault = true;
                _isOpen = !_isOpen;
                return;
            }
            else if (obj.Key == "Tab")
            {
                _keyPressPreventDefault = false;
                if (_isOpen == true)
                {
                    _isOpen = false;
                }
            }

            OnKeyDown.InvokeAsync(obj).AndForget();
        }

        protected async Task InterceptKeyUp(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly)
                return;
            else if (obj.AltKey == true && obj.Key == "ArrowUp")
            {
                _keyPressPreventDefault = true;
                _isOpen = false;
            }
            else if (obj.AltKey == true && obj.Key == "ArrowDown")
            {
                _keyPressPreventDefault = true;
                _isOpen = true;
            }
            else if (obj.Key == "ArrowUp")
            {
                _keyPressPreventDefault = true;
                if (_isOpen == true)
                {
                    _isOpen = false;
                }
                await SelectPreviousItem();
                _key++;
                await Task.Delay(1);
                StateHasChanged();
                await Task.Delay(1);
                await _elementReference.FocusAsync();
            }
            else if (obj.Key == "ArrowDown")
            {
                _keyPressPreventDefault = true;
                if (_isOpen == true)
                {
                    _isOpen = false;
                }
                await SelectNextItem();
                _key++;
                await Task.Delay(1);
                StateHasChanged();
                await Task.Delay(1);
                await _elementReference.FocusAsync();
            }
            else if (obj.Key == "Escape")
            {
                _keyPressPreventDefault = true;
                _isOpen = false;
            }
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
    }
}
