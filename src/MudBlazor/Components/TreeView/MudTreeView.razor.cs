using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTreeView<T> : MudComponentBase
    {
        private MudTreeViewItem<T> _activatedValue;
        private HashSet<MudTreeViewItem<T>> _selectedValues;
        private List<MudTreeViewItem<T>> _childItems = new List<MudTreeViewItem<T>>();

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

        [Parameter] public HashSet<T> Items { get; set; }

        [Parameter] public EventCallback<T> ActivatedValueChanged { get; set; }

        [Parameter] public EventCallback<HashSet<T>> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// ItemTemplate for rendering childre.
        /// </summary>
        [Parameter] public RenderFragment<T> ItemTemplate { get; set; }

        [CascadingParameter] MudTreeView<T> MudTreeRoot { get; set; }

        public MudTreeView()
        {
            MudTreeRoot = this;
        }

        internal async Task UpdateActivatedItem(MudTreeViewItem<T> item, bool requestedValue)
        {
            if ((_activatedValue == item && requestedValue) ||
                (_activatedValue != item && !requestedValue))
                return;

            if (_activatedValue == item && !requestedValue)
            {
                _activatedValue = default;
                await item.Activate(requestedValue);
                await ActivatedValueChanged.InvokeAsync(_activatedValue.Value);
                return;
            }

            if (_activatedValue != null)
            {
                await _activatedValue.Activate(false);
            }

            _activatedValue = item;
            await item?.Activate(requestedValue);
            await ActivatedValueChanged.InvokeAsync(item.Value);
        }

        internal async Task UpdateSelectedItems()
        {
            if (_selectedValues == null)
                _selectedValues = new HashSet<MudTreeViewItem<T>>();

            //collect selected items
            _selectedValues.Clear();
            foreach (var item in _childItems)
            {
                foreach (var selectedItem in item.GetSelectedItems())
                {
                    _selectedValues.Add(selectedItem);
                }
            }

            await SelectedValuesChanged.InvokeAsync(new HashSet<T>(_selectedValues.Select(i => i.Value)));
        }

        internal void AddChild(MudTreeViewItem<T> item) => _childItems.Add(item);
    }
}
