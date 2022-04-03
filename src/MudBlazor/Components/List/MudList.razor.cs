using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment ListActions { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool MultiSelection { get; set; } = false;

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
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public MudListItem<T> SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;
                _selectedItem = value;
                SelectedItemChanged.InvokeAsync(_selectedItem).AndForget();
            }
        }

        /// <summary>
        /// Called whenever the selection changed
        /// </summary>
        [Parameter] public EventCallback<MudListItem<T>> SelectedItemChanged { get; set; }

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
                _selectedValue = value;
                SetSelectedValue(value);
                SelectedValueChanged.InvokeAsync(value).AndForget();
            }
        }

        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public IEnumerable<T> SelectedValues
        {
            get
            {
                if (_selectedValues == null)
                    _selectedValues = new();
                return _selectedValues;
            }
            set
            {
                _selectedValues = (HashSet<T>)value;
                SelectedValuesChanged.InvokeAsync().AndForget();
            }
        }

        /// <summary>
        /// Called whenever the selection changed
        /// </summary>
        [Parameter] public EventCallback<T> SelectedValueChanged { get; set; }

        /// <summary>
        /// Fires when SelectedValues changes.
        /// </summary>
        [Parameter] public EventCallback<IEnumerable<T>> SelectedValuesChanged { get; set; }

        protected override void OnInitialized()
        {
            if (ParentList != null)
            {
                ParentList.Register(this);
                CanSelect = ParentList.CanSelect;
            }
            else
            {
                CanSelect = SelectedItemChanged.HasDelegate || SelectedValueChanged.HasDelegate || SelectedValue != null;
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
        private MudListItem<T> _selectedItem = new();
        //private List<MudListItem> _selectedItems;
        private T _selectedValue;
        private HashSet<T> _selectedValues = new();

        internal void Register(MudListItem<T> item)
        {
            _items.Add(item);
            if (CanSelect && SelectedValue != null && object.Equals(item.Value, SelectedValue))
            {
                item.SetSelected(true);
                //SelectedItem = item;
                //SelectedItemChanged.InvokeAsync(item);
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

        internal void SetSelectedValue(T value, bool force = false)
        {
            if ((!CanSelect || !Clickable) && !force)
                return;
            //if (object.Equals(_selectedValue, value))
            //    return;

            if (!MultiSelection)
            {

                foreach (var listItem in _items)
                {
                    if (listItem.Value.ToString() != value.ToString())
                    {
                        listItem.SetSelected(false);
                    }
                    else
                    {
                        listItem.SetSelected(true);
                    }
                }
                foreach (var childList in _childLists)
                {
                    foreach (var listItem in childList._items)
                    {
                        if (listItem.Value.ToString() != value.ToString())
                        {
                            listItem.SetSelected(false);
                        }
                        else
                        {
                            listItem.SetSelected(true);
                        }
                    }
                }

                ParentList?.SetSelectedValue(value);
            }
        }

        internal void SetSelectedValue(MudListItem<T> item, bool force = false)
        {
            if ((!CanSelect || !Clickable) && !force)
                return;
            if (Equals(_selectedItem, item))
                return;

            SelectedItem = item;
            SelectedValue = item.Value;

            if (!MultiSelection)
            {

                foreach (var listItem in _items)
                {
                    if (listItem.Value.ToString() !=  item.Value.ToString())
                    {
                        listItem.SetSelected(false);
                    }
                }
                foreach (var childList in _childLists)
                {
                    foreach (var listItem in childList._items)
                    {
                        if (listItem.Value.ToString() != item.Value.ToString())
                        {
                            listItem.SetSelected(false);
                        }
                    }
                }

                ParentList?.SetSelectedValue(item.Value);
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
