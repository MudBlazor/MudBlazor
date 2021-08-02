using System;
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
        private List<MudTreeViewItem<T>> _childItems = new();

        protected string Classname =>
        new CssBuilder("mud-treeview")
          .AddClass("mud-treeview-dense", Dense)
          .AddClass("mud-treeview-canhover", CanHover)
          .AddClass("mud-treeview-canactivate", CanActivate)
          .AddClass("mud-treeview-expand-on-click", ExpandOnClick)
          .AddClass(Class)
        .Build();
        protected string Stylename =>
        new StyleBuilder()
            .AddStyle($"width", Width, !string.IsNullOrWhiteSpace(Width))
            .AddStyle($"height", Height, !string.IsNullOrWhiteSpace(Height))
            .AddStyle($"max-height", MaxHeight, !string.IsNullOrWhiteSpace(MaxHeight))
            .AddStyle(Style)
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
        public bool Dense { get; set; }

        /// <summary>
        /// Setting a height will allow to scroll the treeview. If not set, it will try to grow in height. 
        /// You can set this to any CSS value that the attribute 'height' accepts, i.e. 500px. 
        /// </summary>
        [Parameter]
        public string Height { get; set; }

        /// <summary>
        /// Setting a maximum height will allow to scroll the treeview. If not set, it will try to grow in height. 
        /// You can set this to any CSS value that the attribute 'height' accepts, i.e. 500px. 
        /// </summary>
        [Parameter]
        public string MaxHeight { get; set; }

        /// <summary>
        /// Setting a width the treeview. You can set this to any CSS value that the attribute 'height' accepts, i.e. 500px. 
        /// </summary>
        [Parameter]
        public string Width { get; set; }

        [Parameter] public HashSet<T> Items { get; set; }

        [Parameter] public EventCallback<T> ActivatedValueChanged { get; set; }

        [Parameter] public EventCallback<HashSet<T>> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// ItemTemplate for rendering children.
        /// </summary>
        [Parameter] public RenderFragment<T> ItemTemplate { get; set; }

        [CascadingParameter] MudTreeView<T> MudTreeRoot { get; set; }

        [Parameter]
        public Func<T, Task<HashSet<T>>> ServerData { get; set; }

        public MudTreeView()
        {
            MudTreeRoot = this;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && MudTreeRoot == this)
            {
                await UpdateSelectedItems();
            }
            await base.OnAfterRenderAsync(firstRender);
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
                await ActivatedValueChanged.InvokeAsync(default);
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
            _selectedValues ??= new HashSet<MudTreeViewItem<T>>();

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
