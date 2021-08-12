using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudList : MudComponentBase, IDisposable
    {
        protected string Classname =>
        new CssBuilder("mud-list")
           .AddClass("mud-list-padding", !DisablePadding)
          .AddClass(Class)
        .Build();

        [CascadingParameter] protected MudList ParentList { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Set true to make the list items clickable. This is also the precondition for list selection to work.
        /// </summary>
        [Parameter] public bool Clickable { get; set; }

        /// <summary>
        /// If true, vertical padding will be removed from the list.
        /// </summary>
        [Parameter] public bool DisablePadding { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all list items.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed on all list items.
        /// </summary>
        [Parameter] public bool DisableGutters { get; set; }

        /// <summary>
        /// If true, will disable the list item if it has onclick.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// The current selected list item. Bind this with a two-way binding to activate the lists exclusive selection behavior.
        /// Note: make the list Clickable for item selection to work.
        /// </summary>
        [Parameter]
        public MudListItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;
                SetSelectedItem(value, force: true);
            }
        }

        /// <summary>
        /// Called whenever the selection changed
        /// </summary>
        [Parameter] public EventCallback<MudListItem> SelectedItemChanged { get; set; }

        protected override void OnInitialized()
        {
            if (ParentList != null)
            {
                ParentList.Register(this);
                CanSelect = ParentList.CanSelect;
                //OnListParametersChanged();
                //ParentList.ParametersChanged += OnListParametersChanged;
            }
            else
            {
                CanSelect = SelectedItemChanged.HasDelegate;
            }
        }

        internal event Action ParametersChanged;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ParametersChanged?.Invoke();
        }

        private HashSet<MudListItem> _items = new();
        private HashSet<MudList> _childLists = new();
        private MudListItem _selectedItem;

        internal void Register(MudListItem item)
        {
            _items.Add(item);
        }

        internal void Unregister(MudListItem item)
        {
            _items.Remove(item);
        }

        internal void Register(MudList child)
        {
            _childLists.Add(child);
        }

        internal void Unregister(MudList child)
        {
            _childLists.Remove(child);
        }

        internal void SetSelectedItem(MudListItem item, bool force = false)
        {
            if ((!CanSelect || !Clickable) && !force)
                return;
            if (_selectedItem == item)
                return;
            _selectedItem = item;
            _ = SelectedItemChanged.InvokeAsync(item);
            foreach (var listItem in _items.ToArray())
            {
                listItem.SetSelected(item == listItem);
            }
            foreach (var childList in _childLists.ToArray())
                childList.SetSelectedItem(item);
            ParentList?.SetSelectedItem(item);
        }

        internal bool CanSelect { get; private set; }

        public void Dispose()
        {
            ParametersChanged = null;
            ParentList?.Unregister(this);
        }

    }
}
