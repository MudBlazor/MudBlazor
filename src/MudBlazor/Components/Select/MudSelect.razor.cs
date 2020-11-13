using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Components.Select;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor
{
    public partial class MudSelect<T> : MudBaseInput<T>, IMudSelect
    {
        private HashSet<T> _selectedValues;

        protected string Classname =>
            new CssBuilder("mud-select")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Add the MudSelectItems here
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

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

        /// <summary>
        /// Fires when SelectedValues changes.
        /// </summary>
        [Parameter] public EventCallback<HashSet<T>> SelectedValuesChanged { get; set; }

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
                if (SelectedValues.Count==set.Count && SelectedValues.All(x => set.Contains(x)))
                    return;
                _selectedValues = new HashSet<T>(set);
                SelectionChangedFromOutside?.Invoke(_selectedValues);
                if (!MultiSelection)
                    Value = _selectedValues.FirstOrDefault();
                else
                    Text = string.Join(", ", SelectedValues.Select(x=> Converter.Set(x)));
                SelectedValuesChanged.InvokeAsync(new HashSet<T>(SelectedValues));
            }
        }

        private Func<T, string> _toStringFunc = x => x?.ToString();

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
                //RegenerateLookup();
            }
        }

        ///// <summary>
        ///// LookupValue is used to find the value for a given input text by looking it up in the string -> value lookup.
        ///// </summary>
        //protected T LookupValue(string arg)
        //{
        //    if (_lookup.TryGetValue(arg, out var value))
        //        return value;
        //    return default(T);
        //}

        //private Dictionary<string, T> _lookup = new Dictionary<string, T>();

        protected override void StringValueChanged(string text)
        {
            // Select does not support updating the value through the Text property at all!
            //base.StringValueChanged(text);
        }

        protected override void GenericValueChanged(T value)
        {
            // when multiselection is true, we don't update the text when the value changes. 
            // instead the Text will be set with a comma separated list of selected values
            if (MultiSelection)
                return;
            base.GenericValueChanged(value);
        }

        internal event Action<HashSet<T>> SelectionChangedFromOutside;

        /// <summary>
        /// If true, multiple values can be selected via checkboxes which are automatically shown in the dropdown
        /// </summary>
        [Parameter] public bool MultiSelection { get; set; }

        /// <summary>
        /// Sets the maxheight the select can have when open.
        /// </summary>
        [Parameter] public int? MaxHeight { get; set; }

        /// <summary>
        /// Sets the direction the select menu should be.
        /// </summary>
        [Parameter] public Direction Direction { get; set; } = Direction.Bottom;

        /// <summary>
        /// If true, the select menu will open either before or after the input.
        /// </summary>
        [Parameter] public bool OffsetY { get; set; }

        [Parameter] public bool OffsetX { get; set; }

        internal bool isOpen { get; set; }

        public string CurrentIcon { get; set; }

        public async Task SelectOption(object obj)
        {
            var value = (T) obj;
            if (!MultiSelection)
            {
                // single selection
                Value = value;
                isOpen = false;
                UpdateIcon();
                SelectedValues.Clear();
                SelectedValues.Add(value);
            }
            else
            {
                // multi-selection: menu stays open
                if (!SelectedValues.Contains(value))
                    SelectedValues.Add(value);
                else
                    SelectedValues.Remove(value);
                Text = string.Join(", ", SelectedValues.Select(x=>Converter.Set(x)));
            }
            StateHasChanged();
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
        }

        public void ToggleMenu()
        {
            if (Disabled)
                return;
            isOpen = !isOpen;
            UpdateIcon();
            StateHasChanged();
        }

        public void CloseMenu()
        {
            if (Disabled)
                return;
            isOpen = false;
            UpdateIcon();
            StateHasChanged();
        }

        public void UpdateIcon()
        {
                if (isOpen)
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
            base.OnInitialized();
            UpdateIcon();            
            if (MultiSelection && MaxHeight == null)
            {
                MaxHeight = 300;
            }
        }

        //private HashSet<MudSelectItem<T>> _items = new HashSet<MudSelectItem<T>>();

        public void CheckGenericTypeMatch(object select_item)
        {
            if (select_item is MudSelectItemString && typeof(T) == typeof(string))
                return;
            var itemT=select_item.GetType().GenericTypeArguments[0];
            if (itemT != typeof(T))
                throw new GenericTypeMismatchException("MudSelect", "MudSelectItem", typeof(T), itemT);
            //var item = select_item as MudSelectItem<T>;
            //_items.Add(item);
            //AddToLookup(item);
        }

        //private void AddToLookup(MudSelectItem<T> item)
        //{
        //    try
        //    {
        //        if (item.Value == null)
        //            return;
        //        var text = _toStringFunc(item.Value);
        //        _lookup[text] = item.Value;
        //    }
        //    catch (Exception) { }
        //}
        //private void RemoveFromLookup(MudSelectItem<T> item)
        //{
        //    try
        //    {
        //        if (item.Value == null)
        //            return;
        //        var text = _toStringFunc(item.Value);
        //        if (_lookup.TryGetValue(text, out var v) && object.Equals(v,  item.Value))
        //            _lookup.Remove(text);
        //    }
        //    catch (Exception) { }
        //}

        //protected void RegenerateLookup()
        //{
        //    _lookup.Clear();
        //    foreach (var item in _items)
        //        AddToLookup(item);
        //}

        //internal void Remove(MudSelectItem<T> item)
        //{
        //    _items.Remove(item);
        //    RemoveFromLookup(item);
        //}
    }

    public class MudSelectString : MudSelect<string> {}
}
