// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// Maintains the selection of a group of <see cref="MudToggleItem{T}"/> components.
    /// </summary>
    /// <typeparam name="T">The type of item being toggled.</typeparam>
    /// <seealso cref="MudToggleItem{T}"/>
    /// <seealso cref="MudRadioGroup{T}"/>
    /// <seealso cref="MudRadio{T}"/>
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
        private readonly ParameterState<bool> _checkMark;
        private readonly ParameterState<bool> _fixedContent;
        private readonly ParameterState<bool> _disabled;
        private readonly List<MudToggleItem<T>> _items = new();

        protected string Classname => new CssBuilder("mud-toggle-group")
            .AddClass("mud-toggle-group-horizontal", !Vertical)
            .AddClass("mud-toggle-group-vertical", Vertical)
            .AddClass($"mud-toggle-group-size-{Size.ToDescriptionString()}")
            .AddClass("mud-toggle-group-rtl", RightToLeft)
            .AddClass($"mud-toggle-group-{Color.ToDescriptionString()}")
            .AddClass("mud-toggle-group-outlined", Outlined)
            .AddClass("mud-disabled", Disabled)
            .AddClass(Class)
            .Build();

        protected string Stylename => new StyleBuilder()
            .AddStyle("grid-template-columns", $"repeat({_items.Count}, minmax(0, 1fr))", !Vertical)
            .AddStyle("grid-template-rows", $"repeat({_items.Count}, minmax(0, 1fr))", Vertical)
            .AddStyle(Style)
            .Build();

        /// <summary>
        /// Prevents the user from interacting with this toggle group.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The currently selected value.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="SelectionMode"/> is <see cref="SelectionMode.SingleSelection"/> or <see cref="SelectionMode.ToggleSelection"/>.<br />
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public T? Value { get; set; }

        /// <summary>
        /// Occurs when <see cref="Value"/> has changed.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="SelectionMode"/> is <see cref="SelectionMode.SingleSelection"/> or <see cref="SelectionMode.ToggleSelection"/>.<br />
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public EventCallback<T?> ValueChanged { get; set; }

        /// <summary>
        /// The currently selected values.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection"/>.<br />
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public IEnumerable<T?>? Values { get; set; }

        /// <summary>
        /// Occurs when <see cref="Values"/> has changed.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection"/>.<br />
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public EventCallback<IEnumerable<T?>?> ValuesChanged { get; set; }

        /// <summary>
        /// The CSS classes applied to selected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? SelectedClass { get; set; }

        /// <summary>
        /// The CSS classes applied to checkmark icons.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.<br />
        /// Applies when <see cref="CheckMark"/> is <c>true</c>. Classes are applied to the <c>SelectedIcon</c> and <c>UnselectedIcon</c> icons.<br />
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? CheckMarkClass { get; set; }

        /// <summary>
        /// Uses a vertical layout for items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Vertical { get; set; }

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Shows an outline border.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Outlined { get; set; } = true;

        /// <summary>
        /// Shows a line delimiter between each item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Delimiters { get; set; } = true;

        /// <summary>
        /// Show a ripple effect when the user clicks an item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// The size of each toggle item.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The selection behavior for this group.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SelectionMode.SingleSelection"/>.<br />
        /// * <c>SingleSelection</c>: Allows only one item to be selected at a time.<br />
        /// * <c>MultiSelection</c>: Allows multiple items to be selected.<br />
        /// * <c>ToggleSelection</c>: Allows only one item to be selected, but the current selection can be deselected.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public SelectionMode SelectionMode { get; set; }

        /// <summary>
        /// The color of borders and the current selections.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Primary"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Shows a checkmark next to each item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, the checkmark icons can be customized via <c>SelectedIcon</c> and <c>UnselectedIcon</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool CheckMark { get; set; }

        /// <summary>
        /// Reserves space for checkmarks even when <see cref="CheckMark"/> is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool FixedContent { get; set; }

        /// <summary>
        /// Contains the <see cref="MudToggleItem{T}"/> components of this group.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Adds the specified item to this group.
        /// </summary>
        /// <param name="item">The item to add.</param>
        protected internal void Register(MudToggleItem<T> item)
        {
            if (_items.Select(x => x.Value).Contains(item.Value))
            {
                return;
            }

            _items.Add(item);
            ApplySelectionState(item);
            StateHasChanged();
        }

        /// <summary>
        /// Removes the specified item from this group.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        protected internal void Unregister(MudToggleItem<T> item)
        {
            if (_items.Remove(item))
            {
                StateHasChanged();
            }
        }

        /// <inheritdoc />
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

        private void OnValueChanged(ParameterChangedEventArgs<T?> args)
        {
            // perform Selection after user consumes Value Changed logic
            var selectedItem = _items.FirstOrDefault(x => EqualityComparer<T>.Default.Equals(x.Value, args.Value));
            var previousItem = _items.FirstOrDefault(x => EqualityComparer<T>.Default.Equals(x.Value, args.LastValue));
            if (selectedItem != null) ApplySelectionState(selectedItem);
            if (previousItem != null) ApplySelectionState(previousItem);
        }

        private void OnValuesChanged(ParameterChangedEventArgs<IEnumerable<T?>?> args)
        {
            // perform Selection after user consumes Values Changed logic
            ApplySelectionState();
        }

        private void OnParameterChanged()
        {
            foreach (IMudStateHasChanged mudComponent in _items)
            {
                mudComponent.StateHasChanged();
            }

            StateHasChanged();
        }

        private void ApplySelectionState()
        {
            foreach (var item in _items)
            {
                ApplySelectionState(item);
            }
        }

        private void ApplySelectionState(MudToggleItem<T> item)
        {
            var selected = SelectionMode == SelectionMode.MultiSelection
                ? _values.Value?.Contains(item.Value) ?? false
                : EqualityComparer<T>.Default.Equals(_value.Value, item.Value);
            if (item.Selected != selected)
            {
                item.SetSelected(selected);
            }
        }

        protected internal async Task ToggleItemAsync(MudToggleItem<T> item)
        {
            var itemValue = item.Value;
            var previousItem = SelectionMode != SelectionMode.MultiSelection
                ? _items.FirstOrDefault(x => EqualityComparer<T>.Default.Equals(_value.Value, x.Value))
                : null;
            if (SelectionMode == SelectionMode.MultiSelection)
            {
                var selectedValues = new HashSet<T?>(_values.Value ?? []);

                if (!selectedValues.Remove(itemValue))
                {
                    selectedValues.Add(itemValue);
                }

                await _values.SetValueAsync(selectedValues);
                if (!ValuesChanged.HasDelegate)
                {
                    ApplySelectionState();
                }
                return;
            }
            else if (SelectionMode == SelectionMode.ToggleSelection)
            {
                if (EqualityComparer<T>.Default.Equals(_value.Value, itemValue))
                {
                    await _value.SetValueAsync(default);
                }
                else
                {
                    await _value.SetValueAsync(itemValue);
                }
            }
            else // SingleSelection
            {
                await _value.SetValueAsync(itemValue);
            }

            // WithChangeHandler will update the selection state if the value/values changed
            // but only if the user subscribes to it via bind or directly
            // change handler is needed so the user can change the value programmatically and
            // the selection state still be updated
            if (!ValueChanged.HasDelegate)
            {
                // unselect previous item if not null and not the same as current item
                if (previousItem != null && previousItem != item)
                {
                    ApplySelectionState(previousItem);
                }
                ApplySelectionState(item);
            }
        }

        protected internal IEnumerable<MudToggleItem<T>> GetItems() => _items;

        protected internal bool IsFirstItem(MudToggleItem<T> item) => item.Equals(_items.FirstOrDefault());

        protected internal bool IsLastItem(MudToggleItem<T> item) => item.Equals(_items.LastOrDefault());
    }
}
