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
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudToggleGroup<T> : MudComponentBase
    {
        public MudToggleGroup()
        {
            using var registerScope = CreateRegisterScope();
            _value = registerScope.RegisterParameter<T?>(nameof(Value))
                .WithParameter(() => Value)
                .WithEventCallback(() => ValueChanged)
                .WithChangeHandler(OnValueChanged);
            _values = registerScope.RegisterParameter<IEnumerable<T?>?>(nameof(Values))
                .WithParameter(() => Values)
                .WithEventCallback(() => ValuesChanged)
                .WithChangeHandler(OnValuesChanged);
            _color = registerScope.RegisterParameter<Color>(nameof(Color))
                .WithParameter(() => Color)
                .WithChangeHandler(OnParameterChanged);
            _selectedClass = registerScope.RegisterParameter<string?>(nameof(SelectedClass))
                .WithParameter(() => SelectedClass)
                .WithChangeHandler(OnParameterChanged);
            _outline = registerScope.RegisterParameter<bool>(nameof(Outlined))
                .WithParameter(() => Outlined)
                .WithChangeHandler(OnParameterChanged);
            _delimiters = registerScope.RegisterParameter<bool>(nameof(Delimiters))
                .WithParameter(() => Delimiters)
                .WithChangeHandler(OnParameterChanged);
            _rtl = registerScope.RegisterParameter<bool>(nameof(RightToLeft))
                .WithParameter(() => RightToLeft)
                .WithChangeHandler(OnParameterChanged);
            _size = registerScope.RegisterParameter<Size>(nameof(Size))
                .WithParameter(() => Size)
                .WithChangeHandler(OnParameterChanged);
            _rounded = registerScope.RegisterParameter<bool>(nameof(Rounded))
                .WithParameter(() => Rounded).
                WithChangeHandler(OnParameterChanged);
            _checkMark = registerScope.RegisterParameter<bool>(nameof(CheckMark))
                .WithParameter(() => CheckMark)
                .WithChangeHandler(OnParameterChanged);
            _fixedContent = registerScope.RegisterParameter<bool>(nameof(FixedContent))
                .WithParameter(() => FixedContent)
                .WithChangeHandler(OnParameterChanged);
            _disabled = registerScope.RegisterParameter<bool>(nameof(Disabled))
                .WithParameter(() => Disabled)
                .WithChangeHandler(OnParameterChanged);
        }

        private readonly ParameterState<T?> _value;
        private readonly ParameterState<IEnumerable<T?>?> _values;
        private readonly ParameterState<Color> _color;
        private readonly ParameterState<string?> _selectedClass;
        private readonly ParameterState<bool> _outline;
        private readonly ParameterState<bool> _delimiters;
        private readonly ParameterState<bool> _rtl;
        private readonly ParameterState<Size> _size;
        private readonly ParameterState<bool> _rounded;
        private readonly ParameterState<bool> _checkMark;
        private readonly ParameterState<bool> _fixedContent;
        private readonly ParameterState<bool> _disabled;
        private readonly List<MudToggleItem<T>> _items = new();

        protected string Classname => new CssBuilder("mud-toggle-group")
            .AddClass("mud-toggle-group-horizontal", !Vertical)
            .AddClass("mud-toggle-group-vertical", Vertical)
            .AddClass($"mud-toggle-group-size-{Size.ToDescriptionString()}")
            .AddClass("rounded", !Rounded)
            .AddClass("rounded-xl", Rounded)
            .AddClass("mud-toggle-group-rtl", RightToLeft)
            .AddClass($"border mud-border-{Color.ToDescriptionString()} border-solid", Outlined)
            .AddClass("mud-disabled", Disabled)
            .AddClass(Class)
            .Build();

        protected string Stylename => new StyleBuilder()
            .AddStyle("grid-template-columns", $"repeat({_items.Count}, minmax(0, 1fr))", !Vertical)
            .AddStyle("grid-template-rows", $"repeat({_items.Count}, minmax(0, 1fr))", Vertical)
            .AddStyle(Style)
            .Build();

        /// <summary>
        /// If true, the group will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

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
        public EventCallback<IEnumerable<T?>?> ValuesChanged { get; set; }

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
        public bool Outlined { get; set; } = true;

        /// <summary>
        /// If true, show a line delimiter between items. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Delimiters { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// The size of the items in the toggle group.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Size Size { get; set; } = Size.Medium;

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
        /// SelectedIcon and UnselectedIcon.
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
                    {
                        Logger.LogWarning($"For SelectionMode {SelectionMode} you should bind {nameof(Value)} instead of {nameof(Values)}");
                    }
                    break;
                case SelectionMode.MultiSelection:
                    if (isValueBound && !isSelectedValuesBound)
                    {
                        Logger.LogWarning($"For SelectionMode {SelectionMode} you should bind {nameof(Values)} instead of {nameof(Value)}");
                    }
                    break;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                var multiSelection = SelectionMode == SelectionMode.MultiSelection;
                var value = _value.Value;
                var values = _values.Value;

                // Handle single and toggle selection mode
                if (value is not null && !multiSelection)
                {
                    var selectedItem = _items.Find(x => value.Equals(x.Value));
                    selectedItem?.SetSelected(true);
                }

                // Handle multi-selection mode
                if (values is not null && multiSelection)
                {
                    foreach (var item in _items.Where(x => values.Contains(x.Value)).ToList())
                    {
                        item.SetSelected(true);
                    }
                }

                StateHasChanged();
            }
        }

        private void OnValueChanged()
        {
            if (SelectionMode == SelectionMode.MultiSelection)
            {
                return;
            }

            // Handle single and toggle selection mode 
            DeselectAllItems();

            var value = _value.Value;
            if (value is not null)
            {
                var selectedItem = _items.Find(x => value.Equals(x.Value));
                selectedItem?.SetSelected(true);
            }
        }

        private void OnValuesChanged()
        {
            if (SelectionMode != SelectionMode.MultiSelection)
            {
                return;
            }

            // Handle multi-selection mode
            DeselectAllItems();

            if (Values is not null)
            {
                foreach (var item in _items.Where(x => Values.Contains(x.Value)).ToList())
                {
                    item.SetSelected(true);
                }
            }
        }

        private void OnParameterChanged()
        {
            foreach (IMudStateHasChanged mudComponent in _items)
            {
                mudComponent.StateHasChanged();
            }

            StateHasChanged();
        }

        protected internal async Task ToggleItemAsync(MudToggleItem<T> item)
        {
            var itemValue = item.Value;
            if (SelectionMode == SelectionMode.MultiSelection)
            {
                var selectedValues = new HashSet<T?>(_values.Value ?? Array.Empty<T?>());
                item.SetSelected(!item.Selected);

                if (item.Selected)
                {
                    selectedValues.Add(itemValue);
                }
                else
                {
                    selectedValues.Remove(itemValue);
                }

                await _values.SetValueAsync(selectedValues);
            }
            else if (SelectionMode == SelectionMode.ToggleSelection)
            {
                if (item.Selected)
                {
                    item.SetSelected(false);
                    await _value.SetValueAsync(default);
                }
                else
                {
                    DeselectAllItems();
                    item.SetSelected(true);
                    await _value.SetValueAsync(itemValue);
                }
            }
            else // SingleSelection
            {
                DeselectAllItems();
                item.SetSelected(true);
                await _value.SetValueAsync(itemValue);
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
