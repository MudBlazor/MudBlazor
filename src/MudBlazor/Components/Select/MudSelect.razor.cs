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

        private MudInput<string> _elementReference;

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

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender && Value != null)
            {
                // we need to render the initial Value which is not possible without the items
                // which supply the RenderFragment. So in this case, a second render is necessary
                StateHasChanged();
            }
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
                return _value_lookup.TryGetValue(Value, out var item);
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

        protected List<MudSelectItem<T>> _items = new List<MudSelectItem<T>>();
        protected Dictionary<T, MudSelectItem<T>> _value_lookup = new Dictionary<T, MudSelectItem<T>>();
        internal void Add(MudSelectItem<T> item)
        {
            _items.Add(item);
            if (item.Value!=null)
                _value_lookup[item.Value] = item;
        }

        internal void Remove(MudSelectItem<T> item)
        {
            _items.Remove(item);
            if (item.Value != null)
                _value_lookup.Remove(item.Value);
        }

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

        /// <summary>
        /// If true, the select's input will not show any values that are not defined in the dropdown.
        /// This can be useful if Value is bound to a variable which is initialized to a value which is not in the list
        /// and you want the select to show the label / placeholder instead.
        /// </summary>
        [Parameter] public bool Strict { get; set; }

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
            if (Disabled || ReadOnly)
                return;
            isOpen = !isOpen;
            UpdateIcon();
            StateHasChanged();
        }

        public void CloseMenu()
        {
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

        public void CheckGenericTypeMatch(object select_item)
        {
            var itemT=select_item.GetType().GenericTypeArguments[0];
            if (itemT != typeof(T))
                throw new GenericTypeMismatchException("MudSelect", "MudSelectItem", typeof(T), itemT);
        }

        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }
    }

}
