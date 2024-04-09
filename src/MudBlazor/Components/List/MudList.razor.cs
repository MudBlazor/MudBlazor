using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudList<T> : MudComponentBase, IDisposable
    {
        protected string Classname =>
        new CssBuilder("mud-list")
           .AddClass("mud-list-padding", !DisablePadding)
          .AddClass(Class)
        .Build();

        [CascadingParameter] protected MudList<T> ParentList { get; set; }

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
        public RenderFragment ChildContent { get; set; }

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

        /// <summary>
        /// The current selected list item.
        /// Note: make the list Clickable for item selection to work.
        /// </summary>
        internal MudListItem<T> SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;

                var selectedValue = _selectedItem == null ? default : _selectedItem.Value;
                SetSelectedValueAsync(selectedValue, force: true).AndForget();
            }
        }

        ///// <summary>
        ///// Called whenever the selection changed
        ///// </summary>
        //[Parameter] 
        //public EventCallback<MudListItem> SelectedItemChanged { get; set; }

        /// <summary>
        /// The current selected value.
        /// Note: make the list Clickable for item selection to work.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public T SelectedValue
        {
            get => _selectedValue;
            set
            {
                SetSelectedValueAsync(value, force: true).AndForget();
            }
        }

        /// <summary>
        /// Called whenever the selection changed
        /// </summary>
        [Parameter]
        public EventCallback<T> SelectedValueChanged { get; set; }

        protected override void OnInitialized()
        {
            if (ParentList != null)
            {
                ParentList.Register(this);
                CanSelect = ParentList.CanSelect;
            }
            else
            {
                CanSelect = SelectedValueChanged.HasDelegate || SelectedValue != null;
            }
        }

        internal event Action ParametersChanged;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ParametersChanged?.Invoke();
        }

        private HashSet<MudListItem<T>> _items = new();
        private HashSet<MudList<T>> _childLists = new();
        private MudListItem<T> _selectedItem;
        private T _selectedValue;

        internal async Task RegisterAsync(MudListItem<T> item)
        {
            _items.Add(item);
            if (CanSelect && SelectedValue != null && object.Equals(item.Value, SelectedValue))
            {
                item.SetSelected(true);
                _selectedItem = item;
                await SelectedValueChanged.InvokeAsync(item.Value);
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

        internal async Task SetSelectedValueAsync(T value, bool force = false)
        {
            if ((!CanSelect || !Clickable) && !force)
                return;
            if (object.Equals(_selectedValue, value))
                return;
            _selectedValue = value;
            await SelectedValueChanged.InvokeAsync(value);
            _selectedItem = null; // <-- for now, we'll see which item matches the value below
            foreach (var listItem in _items.ToArray())
            {
                var isSelected = value != null && object.Equals(value, listItem.Value);
                listItem.SetSelected(isSelected);
                if (isSelected)
                    _selectedItem = listItem;
            }
            foreach (var childList in _childLists.ToArray())
            {
                await childList.SetSelectedValueAsync(value);
                if (childList.SelectedItem != null)
                    _selectedItem = childList.SelectedItem;
            }

            await SelectedValueChanged.InvokeAsync(_selectedItem.Value);
            if (ParentList is not null)
            {
                await ParentList.SetSelectedValueAsync(value);
            }
        }

        internal bool CanSelect { get; private set; }

        public void Dispose()
        {
            ParametersChanged = null;
            ParentList?.Unregister(this);
        }
    }
}
