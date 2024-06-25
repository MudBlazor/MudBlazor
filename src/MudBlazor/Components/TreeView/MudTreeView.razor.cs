using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudTreeView<T> : MudComponentBase
    {
        public MudTreeView()
        {
            MudTreeRoot = this;
            using var registerScope = CreateRegisterScope();
            _selectedValueState = registerScope.RegisterParameter<T?>(nameof(SelectedValue))
                .WithParameter(() => SelectedValue)
                .WithEventCallback(() => SelectedValueChanged)
                .WithChangeHandler(OnSelectedValueChangedAsync)
                .WithComparer(() => Comparer);
            _selectedValuesState = registerScope.RegisterParameter<IReadOnlyCollection<T>?>(nameof(SelectedValues))
                .WithParameter(() => SelectedValues)
                .WithEventCallback(() => SelectedValuesChanged)
                .WithChangeHandler(OnSelectedValuesChangedAsync)
                .WithComparer(() => Comparer, comparer => new CollectionComparer<T>(comparer));
            registerScope.RegisterParameter<IEqualityComparer<T?>>(nameof(Comparer))
                .WithParameter(() => Comparer)
                .WithChangeHandler(OnComparerChangedAsync);
            registerScope.RegisterParameter<SelectionMode>(nameof(SelectionMode))
                .WithParameter(() => SelectionMode)
                .WithChangeHandler(OnParameterChangedAsync);
            registerScope.RegisterParameter<bool>(nameof(TriState))
                .WithParameter(() => TriState)
                .WithChangeHandler(OnParameterChangedAsync);
            registerScope.RegisterParameter<bool>(nameof(Disabled))
                .WithParameter(() => Disabled)
                .WithChangeHandler(OnParameterChangedAsync);
            registerScope.RegisterParameter<bool>(nameof(ReadOnly))
                .WithParameter(() => ReadOnly)
                .WithChangeHandler(OnParameterChangedAsync);
            _selection = new();
        }

        private readonly ParameterState<T?> _selectedValueState;
        private readonly ParameterState<IReadOnlyCollection<T>?> _selectedValuesState;

        private HashSet<T> _selection;
        private HashSet<MudTreeViewItem<T>> _childItems = new();
        private bool _isFirstRender = true;
        internal bool MultiSelection => SelectionMode == SelectionMode.MultiSelection;
        private bool ToggleSelection => SelectionMode == SelectionMode.ToggleSelection;

        protected string Classname =>
            new CssBuilder("mud-treeview")
                .AddClass("mud-treeview-dense", Dense)
                .AddClass("mud-treeview-hover", !Disabled && Hover && (!ReadOnly || ExpandOnClick))
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


        [CascadingParameter]
        private MudTreeView<T> MudTreeRoot { get; set; }

        /// <summary>
        /// The color of the selected TreeViewItem.
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
        /// The selection mode determines whether only a single item (SingleSelection) or multiple items
        /// can be selected (MultiSelection) and whether the selected item can be toggled off by clicking a
        /// second time (ToggleSelection).
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public SelectionMode SelectionMode { get; set; } = SelectionMode.SingleSelection;

        /// <summary>
        /// If true, the checkboxes will use the undetermined state in MultiSelection if any children in the subtree
        /// have a different selection value than the parent item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public bool TriState { get; set; } = true;

        /// <summary>
        /// If true, clicking anywhere on the item will expand it, if it has children.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.ClickAction)]
        public bool ExpandOnClick { get; set; }

        /// <summary>
        /// If true, double-clicking anywhere on the item will expand it, if it has children.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.ClickAction)]
        public bool ExpandOnDoubleClick { get; set; }

        /// <summary>
        /// Gets or sets whether the tree automatically expands to reveal the selected item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public bool AutoExpand { get; set; }

        /// <summary>
        /// Hover effect for item's on mouse-over.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public bool Hover { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all TreeView items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// Setting a height will allow to scroll the TreeView. If not set, it will try to grow in height.
        /// You can set this to any CSS value that the attribute 'height' accepts, i.e. 500px.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? Height { get; set; }

        /// <summary>
        /// Setting a maximum height will allow to scroll the TreeView. If not set, it will try to grow in height.
        /// You can set this to any CSS value that the attribute 'height' accepts, i.e. 500px.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? MaxHeight { get; set; }

        /// <summary>
        /// Setting a width the TreeView. You can set this to any CSS value that the attribute 'height' accepts, i.e. 500px.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? Width { get; set; }

        /// <summary>
        /// If true, TreeView will be disabled and all its children.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// Tree items that will be rendered using the Item
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public IReadOnlyCollection<TreeItemData<T>>? Items { get; set; } = Array.Empty<TreeItemData<T>>();

        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public T? SelectedValue { get; set; }

        /// <summary>
        /// Called whenever the selected value changed.
        /// </summary>
        [Parameter]
        public EventCallback<T?> SelectedValueChanged { get; set; }

        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public IReadOnlyCollection<T>? SelectedValues { get; set; }

        /// <summary>
        /// Called whenever the selection changes.
        /// </summary>
        [Parameter]
        public EventCallback<IReadOnlyCollection<T>?> SelectedValuesChanged { get; set; }

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
        public RenderFragment<TreeItemData<T>>? ItemTemplate { get; set; }

        /// <summary>
        /// Comparer is used to check if two tree items are equal
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public IEqualityComparer<T?> Comparer { get; set; } = EqualityComparer<T?>.Default;

        /// <summary>
        /// Supply a func that asynchronously loads tree view items on demand
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public Func<T?, Task<IReadOnlyCollection<TreeItemData<T?>>>>? ServerData { get; set; }

        /// <summary>
        /// If true, the selection of the tree view can not be changed by clicking its items.
        /// The currently selected value(s) are still displayed however
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Custom checked icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// Custom unchecked icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public string UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// Custom tri-state indeterminate icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && MudTreeRoot == this)
            {
                _isFirstRender = false;
                await UpdateItemsAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// Expands all items and their children recursively.
        /// </summary>
        public async Task ExpandAllAsync()
        {
            foreach (var item in _childItems)
                await item.ExpandAllAsync();
        }

        /// <summary>
        /// Collapses all items and their children recursively.
        /// </summary>
        public async Task CollapseAllAsync()
        {
            foreach (var item in _childItems)
                await item.CollapseAllAsync();
        }

        /// <summary>
        /// SingleSelection or ToggleSelection: SelectedValue was updated via binding
        /// </summary>
        private Task OnSelectedValueChangedAsync(ParameterChangedEventArgs<T?> args)
        {
            // on first render the children are not yet initialized, so ignore this update
            if (_isFirstRender)
            {
                return Task.CompletedTask;
            }
            return SetSelectedValueAsync(args.Value);
        }

        /// <summary>
        /// MultiSelection: SelectedValues was updated via binding
        /// </summary>
        private Task OnSelectedValuesChangedAsync(ParameterChangedEventArgs<IReadOnlyCollection<T>?> args)
        {
            if (_isFirstRender)
            {
                // on first render the children are not yet initialized, so just initialize the selection
                _selection = args.Value is not null ? new HashSet<T>(args.Value!, Comparer) : new HashSet<T>(Comparer);
                return Task.CompletedTask;
            }
            return SetSelectedValuesAsync(args.Value ?? Array.Empty<T>());
        }

        private Task OnComparerChangedAsync(ParameterChangedEventArgs<IEqualityComparer<T?>> args)
        {
            if (_isFirstRender)
            {
                return Task.CompletedTask;
            }
            return UpdateItemsAsync();
        }

        private Task OnParameterChangedAsync()
        {
            if (_isFirstRender)
            {
                return Task.CompletedTask;
            }
            return UpdateItemsAsync();
        }

        internal async Task OnItemClickAsync(MudTreeViewItem<T> clickedItem)
        {
            if (ReadOnly)
            {
                return;
            }
            if (MultiSelection)
            {
                var items = clickedItem.GetChildItemsRecursive();
                items.Add(clickedItem!);
                var allSelected = items.All(x => x.GetState<bool>(nameof(MudTreeViewItem<T>.Selected)));
                // toggle selection of the clickedItem and its children
                foreach (var item in items.Where(x => x.GetValue() is not null))
                {
                    if (allSelected)
                    {
                        _selection.Remove(item.GetValue()!);
                    }
                    else
                    {
                        _selection.Add(item.GetValue()!);
                    }
                }
                UpdateParentItem(clickedItem.Parent);
                await _selectedValuesState.SetValueAsync(_selection.ToList()); // note: .ToList() is essential here!
                await UpdateItemsAsync();
                return;
            }
            var selected = clickedItem.GetState<bool>(nameof(MudTreeViewItem<T>.Selected));
            if (ToggleSelection)
            {
                await SetSelectedValueAsync(selected ? default : clickedItem.GetValue()); // <-- toggle selected value
            }
            else if (!selected)
            {
                // SingleSelection
                await SetSelectedValueAsync(clickedItem.GetValue());
            }
        }

        /// <summary>
        /// This changes the parent item's state based on the selection state of its children in multi-selection mode
        /// But only if the items are clicked, not when the selection is modified via SelectedValues
        /// </summary>
        private void UpdateParentItem(MudTreeViewItem<T>? parentItem)
        {
            while (parentItem is not null)
            {
                var parentValue = parentItem.GetValue();
                if (parentValue is not null)
                {
                    var parentSelected = parentItem.ChildItems.Select(x => x.GetValue()).Where(x => x is not null).All(x => _selection.Contains(x!));
                    if (parentSelected)
                        _selection.Add(parentValue);
                    else
                        _selection.Remove(parentValue);
                }
                parentItem = parentItem.Parent;
            }
        }

        internal async Task AddChildAsync(MudTreeViewItem<T> item)
        {
            _childItems.Add(item);
            // this is to ensure that setting Selected="true" on the item will update the single/multiselection.
            // Note: Setting Selected="false" has no effect however because it would cancel the initialization of the SelectedValue or SelectedValues !
            var value = item.GetValue();
            if (value is not null && item.GetState<bool>(nameof(MudTreeViewItem<T>.Selected)))
            {
                await SelectAsync(value);
            }
            await item.UpdateSelectionStateAsync(GetSelection());
        }

        internal void RemoveChild(MudTreeViewItem<T> item)
        {
            _childItems.Remove(item);
        }

        internal async Task SelectAsync(T value)
        {
            if (MultiSelection)
            {
                _selection.Add(value);
                if (!_isFirstRender)
                {
                    await _selectedValuesState.SetValueAsync(_selection.ToList()); // note: .ToList() is essential here!
                    await UpdateItemsAsync();
                }
                return;
            }
            // single and toggle selection
            await _selectedValueState.SetValueAsync(value);
            if (!_isFirstRender)
            {
                await UpdateItemsAsync();
            }
        }

        internal async Task UnselectAsync(T value)
        {
            if (_isFirstRender || !MultiSelection)
            {
                return;
            }
            _selection.Remove(value);
            await _selectedValuesState.SetValueAsync(_selection.ToList()); // note: .ToList() is essential here!
        }

        ///  <summary>
        ///  Sets the selected value of the tree view in Single- and ToggleSelection mode.
        ///  If the value is found, the corresponding item is selected; 
        ///  otherwise, selected value is set default.
        ///  If the selected item is valid it sets the corresponding tree item to selected.
        ///  </summary>
        ///  <param name="value">The value to be set as the selected value.</param>
        internal async Task SetSelectedValueAsync(T? value)
        {
            var isValid = value != null && GetChildValuesRecursive().Contains(value);
            // note: if there is no item that corresponds to the value, the value is reset to default!
            await _selectedValueState.SetValueAsync(isValid ? value : default);
            await UpdateItemsAsync();
        }

        ///  <summary>
        ///  Sets the selected values of the tree view in MultiSelection mode.
        /// Discard any values which are not represented by child values.
        ///  </summary>
        private async Task SetSelectedValuesAsync(IReadOnlyCollection<T> newValues)
        {
            var allChildValues = GetChildValuesRecursive();
            var newSelection = new HashSet<T>(newValues.Where(x => allChildValues.Contains(x)), Comparer);
            if (_selection.SetEquals(newSelection))
            {
                return;
            }
            _selection = newSelection;
            await _selectedValuesState.SetValueAsync(newSelection);
            await UpdateItemsAsync();
        }

        /// <summary>
        /// Let the items update their selection state visualization and state according to
        /// the selection in the tree view
        /// </summary>
        private async Task UpdateItemsAsync()
        {
            var selection = GetSelection();
            foreach (var item in _childItems)
            {
                await item.UpdateSelectionStateAsync(selection);
            }
        }

        private HashSet<T> GetSelection()
        {
            HashSet<T> selection;
            if (MultiSelection)
            {
                selection = new HashSet<T>(_selection, Comparer);
            }
            else
            {
                selection = new HashSet<T>(Comparer);
                if (_selectedValueState.Value != null)
                {
                    selection.Add(_selectedValueState.Value);
                }
            }
            return selection;
        }

        // TODO: speed this up with caching
        private HashSet<T> GetChildValuesRecursive(IEnumerable<MudTreeViewItem<T>>? children = null, HashSet<T>? values = null)
        {
            values ??= new HashSet<T>(Comparer);
            children ??= _childItems;

            foreach (var item in children)
            {
                var value = item.GetValue();
                if (value is not null)
                {
                    values.Add(value);
                }
                if (item.ChildItems.Count > 0)
                {
                    GetChildValuesRecursive(item.ChildItems, values);
                }
            }

            return values;
        }

    }
}
