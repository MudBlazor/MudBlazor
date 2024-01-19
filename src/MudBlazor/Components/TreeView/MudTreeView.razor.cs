#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTreeView<T> : MudComponentBase
    {
        private object _selectedUpdateLock = new();
        private HashSet<MudTreeViewItem<T>>? _selectedValues;
        private List<MudTreeViewItem<T>> _childItems = new();

        protected string Classname =>
        new CssBuilder("mud-treeview")
          .AddClass("mud-treeview-dense", Dense)
          .AddClass("mud-treeview-hover", Hover)
          .AddClass($"mud-treeview-selected-{Color.ToDescriptionString()}")
           .AddClass($"mud-treeview-checked-{CheckBoxColor.ToDescriptionString()}")
          .AddClass(Class)
        .Build();
        protected string Stylename =>
        new StyleBuilder()
            .AddStyle($"width", Width, !string.IsNullOrWhiteSpace(Width))
            .AddStyle($"height", Height, !string.IsNullOrWhiteSpace(Height))
            .AddStyle($"max-height", MaxHeight, !string.IsNullOrWhiteSpace(MaxHeight))
            .AddStyle(Style)
        .Build();

        /// <summary>
        /// The color of the selected treeviewitem.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Check box color if multiselection is used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public Color CheckBoxColor { get; set; }

        /// <summary>
        /// if true, multiple values can be selected via checkboxes which are automatically shown in the tree view.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public bool MultiSelection { get; set; }

        /// <summary>
        /// if true, multiple values can be selected via checkboxes which are automatically shown in the tree view.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use MultiSelection instead.", true)]
        [Parameter]
        public bool CanSelect
        {
            get => MultiSelection;
            set => MultiSelection = value;
        }

        [ExcludeFromCodeCoverage]
        [Obsolete("MudTreeView now automaticly activates when using SelectedValue.", true)]
        [Parameter] public bool CanActivate { get; set; }

        /// <summary>
        /// If true, clicking anywhere on the item will expand it, if it has childs.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.ClickAction)]
        public bool ExpandOnClick { get; set; }

        /// <summary>
        /// If true, double clicking anywhere on the item will expand it, if it has childs.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.ClickAction)]
        public bool ExpandOnDoubleClick { get; set; }

        /// <summary>
        /// Hover effect for item's on mouse-over.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public bool Hover { get; set; }

        /// <summary>
        /// Hover effect for item's on mouse-over.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use Hover instead.", true)]
        [Parameter]
        public bool CanHover
        {
            get => Hover;
            set => Hover = value;
        }

        /// <summary>
        /// If true, compact vertical padding will be applied to all treeview items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// Setting a height will allow to scroll the treeview. If not set, it will try to grow in height.
        /// You can set this to any CSS value that the attribute 'height' accepts, i.e. 500px.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? Height { get; set; }

        /// <summary>
        /// Setting a maximum height will allow to scroll the treeview. If not set, it will try to grow in height.
        /// You can set this to any CSS value that the attribute 'height' accepts, i.e. 500px.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? MaxHeight { get; set; }

        /// <summary>
        /// Setting a width the treeview. You can set this to any CSS value that the attribute 'height' accepts, i.e. 500px.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? Width { get; set; }

        /// <summary>
        /// If true, treeview will be disabled and all its childitems.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public bool Disabled { get; set; }

        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public HashSet<T> Items { get; set; } = new();

        [ExcludeFromCodeCoverage]
        [Obsolete("Use SelectedValueChanged instead.", true)]
        [Parameter] public EventCallback<T?> ActivatedValueChanged
        {
            get => SelectedValueChanged;
            set => SelectedValueChanged = value;
        }

        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public T? SelectedValue { get; set; }

        /// <summary>
        /// Called whenever the selected value changed.
        /// </summary>
        [Parameter] public EventCallback<T?> SelectedValueChanged { get; set; }
        
        /// <summary>
        /// Called whenever the selectedvalues changed.
        /// </summary>
        [Parameter] public EventCallback<HashSet<T>> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// ItemTemplate for rendering children.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public RenderFragment<T>? ItemTemplate { get; set; }
        
        /// <summary>
        /// Comparer is used to check if two tree items are equal
        /// </summary>
        [Parameter] 
        [Category(CategoryTypes.TreeView.Selecting)]
        public IEqualityComparer<T?> Comparer { get; set; } = EqualityComparer<T?>.Default;

        [CascadingParameter] MudTreeView<T> MudTreeRoot { get; set; }

        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public Func<T, Task<HashSet<T>>>? ServerData { get; set; }

        public MudTreeView()
        {
            MudTreeRoot = this;
        }

        internal bool IsSelectable { get; private set; }

        protected override void OnInitialized()
        {
            IsSelectable = SelectedValueChanged.HasDelegate;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && MudTreeRoot == this)
            {
                await SetSelectedItemsCompare();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        internal Task SetSelectedItemsCompare()
        {
            lock (_selectedUpdateLock)
            {
                _selectedValues ??= new(new MudTreeViewItemComparer<T>(Comparer));

                //collect selected items
                _selectedValues.Clear();
                foreach (var item in _childItems)
                {
                    foreach (var selectedItem in item.GetSelectedItems())
                    {
                        _selectedValues.Add(selectedItem);
                    }
                }
            }

            return SelectedValuesChanged.InvokeAsync(SelectedValues);
        }

        private HashSet<T> SelectedValues => _selectedValues is not null
            ? new(_selectedValues.Select(i => i.Value))
            : new();
        
        public async Task Select(MudTreeViewItem<T> item, bool isSelected = true)
        {
            if (MultiSelection)
            {
                await item.Select(isSelected);
                await SelectedValuesChanged.InvokeAsync(SelectedValues);
                return;
            }

            if (isSelected)
            {
                await SetSelectedValue(item.Value);
                return;
            }

            await SetSelectedValue(default);
        }

        internal void AddChild(MudTreeViewItem<T> item) => _childItems.Add(item);

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue(nameof(SelectedValue), out T? selected) && 
                !Comparer.Equals(selected, SelectedValue))
            {
                await SetSelectedValue(selected);
            }
            
            if (parameters.TryGetValue(nameof(Comparer), out IEqualityComparer<T?>? comparer) && 
                !Equals(comparer, Comparer))
            {
                await UpdateSelectedValueCompare(comparer!);
            }
            
            await base.SetParametersAsync(parameters);
        }

        ///  <summary>
        ///  Sets the selected value of the tree view.
        ///  If the value is found, the corresponding item is selected; 
        ///  otherwise, selected value is set null.
        ///  If the selected item is valid it sets the corresponding tree item to selected.
        ///  </summary>
        ///  <param name="value">The value to be set as the selected value.</param>
        internal async Task SetSelectedValue(T? value)
        {
            if(Comparer.Equals(value, SelectedValue)) return;
            
            await UnSelectAllChildren();

            if (value == null ||
                FindItemByValue(value) is not { } item)
            {
                await SelectedValueChanged.InvokeAsync(default);
                return;
            }

            await SelectedValueChanged.InvokeAsync(item.Value);
            await item.Select(true);
        }

        private async ValueTask UpdateSelectedValueCompare(IEqualityComparer<T?> comparer)
        {
            List<MudTreeViewItem<T>> unselectedItem;
            
            lock (_selectedUpdateLock)
            {
                if (_selectedValues is null)
                {
                    _selectedValues = new(new MudTreeViewItemComparer<T>(comparer));
                    return;
                }
                
                var newSelected = new HashSet<MudTreeViewItem<T>>(new MudTreeViewItemComparer<T>(comparer));

                foreach (var item in _selectedValues)
                {
                    newSelected.Add(item);
                }

                unselectedItem = _selectedValues.Except(newSelected).ToList();

                _selectedValues = newSelected;
            }
            
            
            foreach (var unselectedItems in unselectedItem)
            {
                await unselectedItems.SelectedChanged.InvokeAsync(false);
            }
        }

        internal async Task UnSelectAllChildren(List<MudTreeViewItem<T>>? children = null)
        {
            children ??= _childItems;
            
            foreach (var item in children)
            {
                await UnSelectAllChildren(item.ChildItems);

                if (item.Activated)
                {
                    await item.Select(false);
                }
            }
        }
        
        internal MudTreeViewItem<T>? FindItemByValue(T value, List<MudTreeViewItem<T>>? children = null)
        {
            children ??= _childItems;

            foreach (var item in children)
            {
                if (Comparer.Equals(item.Value, value))
                {
                    return item;
                }

                var foundChild = FindItemByValue(value, item.ChildItems);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            return null;
        }
        
    }
}
