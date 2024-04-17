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
    public partial class MudList<T> : MudComponentBase, IDisposable
    {
        private T? _selectedValue;
        private ParameterState<T?> _selectedValueState;
        private HashSet<MudListItem<T>> _items = new();
        private HashSet<MudList<T>> _childLists = new();

        internal event Action? ParametersChanged;
        internal MudListItem<T>? SelectedItem { get; private set; }

        public MudList()
        {
            using var registerScope = CreateRegisterScope();
            _selectedValueState = registerScope.RegisterParameter<T?>(nameof(SelectedValue))
                .WithParameter(() => SelectedValue)
                .WithEventCallback(() => SelectedValueChanged)
                .WithChangeHandler(OnSelectedValueParameterChangedAsync);
        }

        protected string Classname =>
            new CssBuilder("mud-list")
                .AddClass("mud-list-padding", Padding)
                .AddClass(Class)
                .Build();

        [CascadingParameter]
        protected MudList<T>? ParentList { get; set; }

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
        /// If true, the list items will not be clickable and the selected item can not be changed by the user.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// If true, vertical padding will be applied to the list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Padding { get; set; }

        /// <summary>
        /// If true, list items will take up less vertical space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// If true, left and right padding is added to all list items. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Gutters { get; set; } = true;

        /// <summary>
        /// If true, will disable the list item if it has onclick.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The current selected value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public T? SelectedValue { get; set; }

        /// <summary>
        /// Called whenever the selection changed
        /// </summary>
        [Parameter]
        public EventCallback<T?> SelectedValueChanged { get; set; }

        private Task OnSelectedValueParameterChangedAsync(ParameterChangedEventArgs<T?> args)
        {
            return SetSelectedValueAsync(args.Value, force: true);
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (ParentList is not null)
            {
                ParentList.Register(this);
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ParametersChanged?.Invoke();
        }

        internal async Task RegisterAsync(MudListItem<T> item)
        {
            _items.Add(item);
            if (SelectedValue is not null && Equals(item.GetValue(), SelectedValue))
            {
                item.SetSelected(true);
                SelectedItem = item;
                await _selectedValueState.SetValueAsync(item.GetValue());
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

        internal bool GetDisabled() => Disabled || (ParentList?.Disabled ?? false);

        internal bool GetReadOnly() => ReadOnly || (ParentList?.ReadOnly ?? false);

        internal async Task SetSelectedValueAsync(T? value, bool force = false)
        {
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

            SelectedItem = selectedItem;
            if (ParentList is not null)
            {
                await ParentList.SetSelectedValueAsync(value);
            }
        }

        private async Task<MudListItem<T>?> UpdateSelectedItems(T? value)
        {
            MudListItem<T>? selectedItem = null;
            foreach (var listItem in _items.ToArray())
            {
                var isSelected = value is not null && Equals(value, listItem.GetValue());
                listItem.SetSelected(isSelected);
                if (isSelected)
                {
                    selectedItem = listItem;
                }
            }

            foreach (var childList in _childLists.ToArray())
            {
                await childList.SetSelectedValueAsync(value);
                if (childList.SelectedItem is not null)
                {
                    selectedItem = childList.SelectedItem;
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
