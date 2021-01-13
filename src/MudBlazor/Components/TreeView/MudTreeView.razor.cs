using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTreeView : MudComponentBase
    {
        private MudTreeViewItem _activatedItem;
        private HashSet<MudTreeViewItem> _selectedItems;
        private List<MudTreeViewItem> childItems = new List<MudTreeViewItem>();

        protected string Classname =>
        new CssBuilder("mud-treeview")
          .AddClass("mud-treeview-canhover", CanHover)
          .AddClass("mud-treeview-canactivate", CanActivate)
          .AddClass("mud-treeview-expand-on-click", ExpandOnClick)
          .AddClass(Class)
        .Build();

        [Parameter]
        public bool CanSelect { get; set; }

        [Parameter]
        public bool CanActivate { get; set; }

        [Parameter]
        public bool ExpandOnClick { get; set; }

        [Parameter]
        public bool CanHover { get; set; }

        [Parameter]
        public MudTreeViewItem ActivatedItem
        {
            get => _activatedItem;
            set
            {
                if (_activatedItem == value)
                    return;

                InvokeAsync(() => UpdateActivatedItem(_activatedItem, true));
            }
        }

        [Parameter]
        public HashSet<MudTreeViewItem> SelectedItems
        {
            get => _selectedItems;
            set
            {
                if (_selectedItems == value)
                    return;

                if (_selectedItems != null)
                {
                    foreach (var item in _selectedItems)
                    {
                        _ = item.Select(false);
                    }
                }

                if (value != null)
                {
                    foreach (var item in value)
                    {
                        _ = item.Select(true);
                    }
                }

                _selectedItems = value;
                SelectedItemsChanged.InvokeAsync(_selectedItems);
            }
        }

        [Parameter]
        public EventCallback<MudTreeViewItem> ActivatedItemChanged { get; set; }

        [Parameter]
        public EventCallback<HashSet<MudTreeViewItem>> SelectedItemsChanged { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] MudTreeView MudTreeRoot { get; set; }

        public MudTreeView()
        {
            MudTreeRoot = this;
        }

        internal async Task UpdateActivatedItem(MudTreeViewItem item, bool requestedValue)
        {
            if ((_activatedItem == item && requestedValue) ||
                (_activatedItem != item && !requestedValue))
                return;

            if (_activatedItem == item && !requestedValue)
            {
                _activatedItem = null;
                await item.Activate(requestedValue);
                await ActivatedItemChanged.InvokeAsync(_activatedItem);
                return;
            }

            if (_activatedItem != null)
            {
                await _activatedItem.Activate(false);
            }

            _activatedItem = item;
            await item?.Activate(requestedValue);
            await ActivatedItemChanged.InvokeAsync(item);
        }

        internal async Task UpdateSelectedItems()
        {
            if (_selectedItems == null)
                _selectedItems = new HashSet<MudTreeViewItem>();

            //collect selected items
            _selectedItems.Clear();
            foreach (var item in childItems)
            {
                foreach (var selectedItem in item.GetSelectedItems())
                {
                    _selectedItems.Add(selectedItem);
                }
            }

            await SelectedItemsChanged.InvokeAsync(_selectedItems);
        }

        internal void AddChild(MudTreeViewItem item) => childItems.Add(item);
    }
}
