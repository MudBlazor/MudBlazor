using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudList : MudComponentBase, IDisposable
    {
        private T? _selectedValue;
        private IParameterState<T?> _selectedValueState;
        private IParameterState<MudListItem<T?>?> _selectedItemState;
        private HashSet<MudListItem<T?>> _items = new();
        private HashSet<MudList<T?>> _childLists = new();

        internal event Action? ParametersChanged;

        protected string Classname =>
            new CssBuilder("mud-list")
                .AddClass("mud-list-padding", !DisablePadding)
                .AddClass(Class)
                .Build();

        [CascadingParameter]
        protected MudList<T?>? ParentList { get; set; }

        /// <summary>
        /// The color of the selected List Item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Set true to make the list items clickable. This is also the precondition for list selection to work.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool Clickable { get; set; }

        /// <summary>
        /// If true, vertical padding will be removed from the list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisablePadding { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all list items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed on all list items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableGutters { get; set; }

        /// <summary>
        /// If true, will disable the list item if it has onclick.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

        internal MudListItem? SelectedItem { get; set; }

        /// <summary>
        /// The current selected value.
        /// Note: make the list Clickable for item selection to work.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public T? SelectedValue { get; set; }

        /// <summary>
        /// Called whenever the selection changed
        /// </summary>
        [Parameter]
        public EventCallback<T?> SelectedValueChanged { get; set; }

        internal bool CanSelect { get; private set; }

        public MudList()
        {
            _selectedItemState = RegisterParameter(
                nameof(SelectedItem),
                () => SelectedItem,
                () => SelectedItemChanged,
                OnSelectedItemParameterChangedAsync);
            _selectedValueState = RegisterParameter(
                nameof(SelectedValue),
                () => SelectedValue,
                () => SelectedValueChanged,
                OnSelectedValueParameterChangedAsync);
        }

        private Task OnSelectedItemParameterChangedAsync(ParameterChangedEventArgs<MudListItem?> args)
        {
            return SetSelectedValueAsync(args.Value?.Value, force: true);
        }

        private Task OnSelectedValueParameterChangedAsync(ParameterChangedEventArgs<object?> args)
        {
            return SetSelectedValueAsync(args.Value, force: true);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (ParentList is not null)
            {
                ParentList.Register(this);
                CanSelect = ParentList.CanSelect;
            }
            else
            {
                CanSelect = SelectedItemChanged.HasDelegate || SelectedValueChanged.HasDelegate || SelectedValue is not null;
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ParametersChanged?.Invoke();
        }

        internal async Task RegisterAsync(MudListItem item)
        {
            _items.Add(item);
            if (CanSelect && SelectedValue is not null && Equals(item.Value, SelectedValue))
            {
                item.SetSelected(true);
                await _selectedItemState.SetValueAsync(item);
            }
        }

        internal void Unregister(MudListItem<T> item)
        {
            _items.Remove(item);
        }

        internal void Register(MudList<T> child)
        {
            _childLists.Add(child);
        }

        internal void Unregister(MudList<T> child)
        {
            _childLists.Remove(child);
        }

        internal async Task SetSelectedValueAsync(T? value, bool force = false)
        {
            if ((!CanSelect || !Clickable) && !force)
            {
                return;
            }

            // We cannot use the _selectedValueState.Value here instead of _selectedValue
            // The problem arises when the SelectedValue is preselected
            // Then OnInitialized will set the _selectedValueState.Value = SelectedValue
            // And this will early exit, while we need it to be null for this complicated logic to work like it was before
            if (Equals(_selectedValue, value))
            {
                return;
            }

            _selectedValue = value;
            await _selectedValueState.SetValueAsync(_selectedValue);

            // Find and update selected item based on value
            var selectedItem = await UpdateSelectedItems(value);

            await _selectedItemState.SetValueAsync(selectedItem);
            if (ParentList is not null)
            {
                await ParentList.SetSelectedValueAsync(value);
            }
        }

        private async Task<MudListItem?> UpdateSelectedItems(object? value)
        {
            MudListItem? selectedItem = null;
            foreach (var listItem in _items.ToArray())
            {
                var isSelected = value is not null && Equals(value, listItem.Value);
                listItem.SetSelected(isSelected);
                if (isSelected)
                {
                    selectedItem = listItem;
                }
            }

            foreach (var childList in _childLists.ToArray())
            {
                await childList.SetSelectedValueAsync(value);
                if (childList._selectedItemState.Value is not null)
                {
                    selectedItem = childList._selectedItemState.Value;
                }
            }

            return selectedItem;
        }

        public void Dispose()
        {
            ParametersChanged = null;
            ParentList?.Unregister(this);
        }
    }
}
