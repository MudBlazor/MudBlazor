// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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
        /// The selected value in single- and toggle-selection mode.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public T? Value { get; set; }

        /// <summary>
        /// Fires when Value changes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public EventCallback<T?> ValueChanged { get; set; }

        /// <summary>
        /// The selected values for multi-selection mode.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public IEnumerable<T?>? Values { get; set; }

        /// <summary>
        /// Fires when Values change.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public EventCallback<IEnumerable<T?>> ValuesChanged { get; set; }

        /// <summary>
        /// Classes (separated by space) to be applied to the selected items only.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? SelectedClass { get; set; }

        /// <summary>
        /// Classes (separated by space) to be applied to the text of all toggle items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? TextClass { get; set; }

        /// <summary>
        /// Classes (separated by space) to be applied to SelectedIcon/UnselectedIcon of the items (if CheckMark is true).
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? CheckMarkClass { get; set; }

        /// <summary>
        /// If true, use vertical layout.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Vertical { get; set; }

        /// <summary>
        /// If true, the first and last item will be rounded.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Rounded { get; set; }

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// If true, show an outline border. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Outline { get; set; } = true;

        /// <summary>
        /// If true, show a line delimiter between items. Default is true.
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
        /// If true, the component's padding is reduced so it takes up less space.
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
        /// The color of the component. Affects borders and selection color. Default is Colors.Primary.
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

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var isValueBound = ValueChanged.HasDelegate;
            var isSelectedValuesBound = ValuesChanged.HasDelegate;
            switch (SelectionMode)
            {
                default:
                case SelectionMode.SingleSelection:
                case SelectionMode.ToggleSelection:
                    if (!isValueBound && isSelectedValuesBound)
                        Logger.LogWarning($"For SelectionMode {SelectionMode} you should bind {nameof(Value)} instead of {nameof(Values)}");
                    break;
                case SelectionMode.MultiSelection:
                    if (isValueBound && !isSelectedValuesBound)
                        Logger.LogWarning($"For SelectionMode {SelectionMode} you should bind {nameof(Values)} instead of {nameof(Value)}");
                    break;
            }
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
            if (((_values is null && Values is not null) || (_values is not null && !_values.Equals(Values))) && multiSelection)
            {
                DeselectAllItems();

                if (Values is not null)
                {
                    var selectedItems = _items.Where(x => Values.Contains(x.Value)).ToList();
                    selectedItems.ForEach(x => x.SetSelected(true));
                }

                _values = Values;
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
                if (Values is not null && multiSelection)
                {
                    var selectedItems = _items.Where(x => Values.Contains(x.Value)).ToList();
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
                    Values = Values?.Where(x => !Equals(x, item.Value));
                    await ValuesChanged.InvokeAsync(Values);
                }
                else
                {
                    Values ??= new HashSet<T>();
                    Values = Values.Append(item.Value);
                    await ValuesChanged.InvokeAsync(Values);
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
