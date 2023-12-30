// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudToggleGroup<T> : MudComponentBase
    {
        private T? _value;
        private Color _color;
        private IEnumerable<T?>? _values;
        private string? _selectedClass;
        private bool _outline = true;
        private bool _delimiters = true;
        private bool _rtl;
        private List<MudToggleItem<T>> _items = new();
        private bool _dense;
        private bool _rounded;
        private bool _checkMark = true;
        private bool _fixedContent = false;

        protected string Classes => new CssBuilder("mud-toggle-group")
            .AddClass("mud-toggle-group-horizontal", !Vertical)
            .AddClass("mud-toggle-group-vertical", Vertical)
            .AddClass("rounded", !Rounded)
            .AddClass("rounded-xl", Rounded)
            .AddClass("mud-toggle-group-rtl", RightToLeft)
            .AddClass($"border mud-border-{Color.ToDescriptionString()} border-solid", Outline)
            .AddClass(Class)
            .Build();

        protected string Styles => new StyleBuilder()
            .AddStyle("grid-template-columns", $"repeat({_items.Count}, minmax(0, 1fr))", !Vertical)
            .AddStyle("grid-template-rows", $"repeat({_items.Count}, minmax(0, 1fr))", Vertical)
            .AddStyle(Style)
            .Build();

        /// <summary>
        /// The generic value for the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public T? Value { get; set; }

        /// <summary>
        /// Fires when value changed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public EventCallback<T?> ValueChanged { get; set; }

        /// <summary>
        /// Selected values that stored for multiselection mode.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public IEnumerable<T?>? SelectedValues { get; set; }

        /// <summary>
        /// Fires when SelectedValues changed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public EventCallback<IEnumerable<T?>> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Classnames only applied selected item, sepereated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? SelectedClass { get; set; }

        /// <summary>
        /// Class for toggle item text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? TextClass { get; set; }
        
        /// <summary>
        /// Class for toggle item icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? CheckMarkClass { get; set; }
        
        /// <summary>
        /// If true, items ordered vertically.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Vertical { get; set; }

        /// <summary>
        /// If true, first and last item will be rounded.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Rounded { get; set; }

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// If true, outline border will show. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Outline { get; set; } = true;

        /// <summary>
        /// If true, the line delimiter between items will show. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Delimiters { get; set; } = true;

        /// <summary>
        /// If true, disables the ripple effect.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, component's margin and padding will reduce.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The selection behavior of the group. SingleSelection (the default) is a radio-button like exclusive collection. 
        /// MultiSelection behaves like a group of check boxes. ToggleSelection is an exclusive single selection where
        /// you can also select nothing by toggling off the current choice.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public SelectionMode SelectionMode { get; set; }

        /// <summary>
        /// The color of the component. Affect borders and selection color. Default is primary.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// If true, the items show a check mark next to the text or render fragment. Customize the check mark by setting
        /// SelectedIcon and UnselectedIcon 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool CheckMark { get; set; }

        /// <summary>
        /// If true, the check mark is counter balanced with padding on the right side which makes the content stay always
        /// centered no matter if the check mark is shown or not. 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool FixedContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        protected internal void Register(MudToggleItem<T> item)
        {
            if (_items.Select(x => x.Value).Contains(item.Value))
            {
                return;
            }

            _items.Add(item);
        }
        
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            var multiSelection = SelectionMode == SelectionMode.MultiSelection;
            // Handle single selection mode
            if (((_value is null && Value is not null) || (_value is not null && Value is null) || (_value is not null && !_value.Equals(Value))) && !multiSelection)
            {
                DeselectAllItems();

                if (Value is not null)
                {
                    var selectedItem = _items.FirstOrDefault(x => Value.Equals(x.Value));
                    selectedItem?.SetSelected(true);
                }

                _value = Value;
            }

            // Handle multi-selection mode
            if (((_values is null && SelectedValues is not null) || (_values is not null && !_values.Equals(SelectedValues))) && multiSelection)
            {
                DeselectAllItems();

                if (SelectedValues is not null)
                {
                    var selectedItems = _items.Where(x => SelectedValues.Contains(x.Value)).ToList();
                    selectedItems.ForEach(x => x.SetSelected(true));
                }

                _values = SelectedValues;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                var multiSelection = SelectionMode == SelectionMode.MultiSelection;
                // Handle single selection mode
                if (Value is not null && !multiSelection)
                {
                    var selectedItem = _items.FirstOrDefault(x => Value.Equals(x.Value));
                    selectedItem?.SetSelected(true);
                }

                // Handle multi-selection mode
                if (SelectedValues is not null && multiSelection)
                {
                    var selectedItems = _items.Where(x => SelectedValues.Contains(x.Value)).ToList();
                    selectedItems.ForEach(x => x.SetSelected(true));
                }

                StateHasChanged();
            }

            if (Color != _color ||
                SelectedClass != _selectedClass ||
                Outline != _outline ||
                Delimiters != _delimiters ||
                RightToLeft != _rtl || 
                Dense != _dense ||
                Rounded != _rounded || 
                CheckMark != _checkMark ||
                FixedContent != _fixedContent
                )
            {
                _color = Color;
                _selectedClass = SelectedClass;
                _outline = Outline;
                _delimiters = Delimiters;
                _rtl = RightToLeft;
                _dense = Dense;
                _rounded = Rounded;
                _checkMark = CheckMark;
                _fixedContent = FixedContent;
                foreach (IMudStateHasChanged mudComponent in _items)
                {
                    mudComponent.StateHasChanged();
                }

                StateHasChanged();
            }
        }

        protected internal async Task ToggleItemAsync(MudToggleItem<T> item)
        {
            if (SelectionMode == SelectionMode.MultiSelection)
            {
                if (item.IsSelected)
                {
                    SelectedValues = SelectedValues?.Where(x => !Equals(x, item.Value));
                    await SelectedValuesChanged.InvokeAsync(SelectedValues);
                }
                else
                {
                    SelectedValues ??= new HashSet<T>();
                    SelectedValues = SelectedValues.Append(item.Value);
                    await SelectedValuesChanged.InvokeAsync(SelectedValues);
                }
                item.SetSelected(!item.IsSelected);
            }
            else if (SelectionMode == SelectionMode.ToggleSelection)
            {
                var selected = item.IsSelected;
                if (!selected)
                {
                    DeselectAllItems();
                    item.SetSelected(true);
                    Value = item.Value;
                    await ValueChanged.InvokeAsync(Value);
                }
                else
                {
                    item.SetSelected(false);
                    Value = default(T);
                    await ValueChanged.InvokeAsync(Value);
                }
            }
            else
            {
                DeselectAllItems();
                item.SetSelected(true);
                Value = item.Value;
                await ValueChanged.InvokeAsync(Value);
            }
        }

        protected void DeselectAllItems()
        {
            foreach (var item in _items)
            {
                item.SetSelected(false);
            }
        }

        protected internal IEnumerable<MudToggleItem<T>> GetItems() => _items;

        protected internal bool IsFirstItem(MudToggleItem<T> item) => item.Equals(_items.FirstOrDefault());

        protected internal bool IsLastItem(MudToggleItem<T> item) => item.Equals(_items.LastOrDefault());
        
    }
}
