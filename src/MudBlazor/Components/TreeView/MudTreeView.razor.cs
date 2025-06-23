using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// An extensively customizable tree view component for displaying hierarchical data, featuring item selection, lazy-loading, and templating support.
    /// </summary>
    /// <typeparam name="T">The type of item to display.</typeparam>
    /// <seealso cref="MudTreeViewItem{T}"/>
    /// <seealso cref="MudTreeViewItemToggleButton"/>
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
        /// The color of the selected item.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Primary"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The color of checkboxes.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>. Only applies when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public Color CheckBoxColor { get; set; }

        /// <summary>
        /// Controls how many items can be selected at one time.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SelectionMode.SingleSelection"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public SelectionMode SelectionMode { get; set; } = SelectionMode.SingleSelection;

        /// <summary>
        /// Uses checkboxes which support an undetermined state.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>. Only applies when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection"/>. When set, 
        /// an item's checkbox will be in the "undetermined" state if child items have a mix of checked and unchecked states.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public bool TriState { get; set; } = true;

        /// <summary>
        /// Automatically checks an item if all children are selected.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>. Only applies when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection"/>. 
        /// Items will also be deselected if any children are deselected.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public bool AutoSelectParent { get; set; } = true;

        /// <summary>
        /// Expands an item with children if it is clicked anywhere (not just the expand/collapse buttons).
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.ClickAction)]
        public bool ExpandOnClick { get; set; }

        /// <summary>
        /// Expands an item with children if it is double-clicked anywhere (not just the expand/collapse buttons).
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.ClickAction)]
        public bool ExpandOnDoubleClick { get; set; }

        /// <summary>
        /// Automatically expands items to show selected children.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public bool AutoExpand { get; set; }

        /// <summary>
        /// Shows an effect when items are hovered over.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public bool Hover { get; set; }

        /// <summary>
        /// Uses compact vertical padding.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// Sets a fixed height.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Can be a CSS value such as <c>500px</c> or <c>30%</c>. When set, items can be scrolled vertically. Otherwise, the height will grow automatically.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? Height { get; set; }

        /// <summary>
        /// Sets a maximum height.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Can be a CSS value such as <c>500px</c> or <c>30%</c>. When set, items can be scrolled vertically. Otherwise, the height will grow automatically.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? MaxHeight { get; set; }

        /// <summary>
        /// Sets a fixed width.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Can be a CSS value such as <c>500px</c> or <c>30%</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? Width { get; set; }

        /// <summary>
        /// Prevents the user from interacting with any items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Determines whether items are displayed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. The function provides an item and should return <c>true</c> to display the item, or <c>false</c> to hide it.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public Func<TreeItemData<T>, Task<bool>>? FilterFunc { get; set; }

        /// <summary>
        /// Shows a ripple effect when an item is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// The items being displayed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public IReadOnlyCollection<TreeItemData<T>>? Items { get; set; } = Array.Empty<TreeItemData<T>>();

        /// <summary>
        /// The currently selected value.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="SelectionMode"/> is <see cref="SelectionMode.SingleSelection"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public T? SelectedValue { get; set; }

        /// <summary>
        /// Occurs when <see cref="SelectedValue"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<T?> SelectedValueChanged { get; set; }

        /// <summary>
        /// The currently selected values.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection"/> or <see cref="SelectionMode.ToggleSelection"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public IReadOnlyCollection<T>? SelectedValues { get; set; }

        /// <summary>
        /// Occurs when <see cref="SelectedValues"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<IReadOnlyCollection<T>?> SelectedValuesChanged { get; set; }

        /// <summary>
        /// The content within this component.
        /// </summary>
        /// <remarks>
        /// Applies when <see cref="ItemTemplate"/> and <see cref="Items"/> are both not set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The template for rendering each item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public RenderFragment<TreeItemData<T>>? ItemTemplate { get; set; }

        /// <summary>
        /// The comparer used to check if two items are equal.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public IEqualityComparer<T?> Comparer { get; set; } = EqualityComparer<T?>.Default;

        /// <summary>
        /// The function for asynchronously loading items.
        /// </summary>
        /// <remarks>
        /// When set, the function will be called to load the children of a parent item. 
        /// When the parent node is <c>null</c>, top-level items should be returned.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public Func<T?, Task<IReadOnlyCollection<TreeItemData<T?>>>>? ServerData { get; set; }

        /// <summary>
        /// Prevents selections from being changed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, selections cannot be changed, but the current
        /// selections will continue to be displayed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// The icon displayed for checked items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.CheckBox"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// The icon displayed for unchecked items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.CheckBoxOutlineBlank"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public string UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// The icon displayed for indeterminate items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.IndeterminateCheckBox"/>. Only applies when <see cref="TriState"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        /// <inheritdoc />
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
        /// Filters all items based on the <see cref="FilterFunc"/> function.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task FilterAsync()
        {
            if (Items is null)
            {
                return;
            }

            if (FilterFunc is null)
            {
                ResetFilter(Items);
                return;
            }

            await TraverseFilterAsync(Items);
        }

        /// <summary>
        /// The internal filter logic that traverses the tree recursively and applies the <see cref="FilterFunc"/> to every item to set the <see cref="MudTreeViewItem{T}.Visible"/> property
        /// </summary>
        /// <param name="items">The hierarchical tree structure to traverse</param>
        /// <returns>A task to represent the asynchronous operation.</returns>
        private async Task TraverseFilterAsync(IEnumerable<TreeItemData<T>> items)
        {
            foreach (var item in items)
            {
                if (item.HasChildren)
                {
                    /* Recursively traverse the tree. Since HasChildren performs the null check on the children we can use the null-forgiving operator here.
                     * Same goes for the FilterFunc which is checked for null in the public Filter function that invokes this function.
                     */
                    await TraverseFilterAsync(item.Children);
                    item.Expanded = item.Visible = await FilterFunc!(item) || item.Children.Any(c => c.Visible);
                }
                else
                {
                    item.Expanded = item.Visible = await FilterFunc!(item);
                }
            }
        }

        /// <summary>
        /// Resets the filter, so that all <see cref="MudTreeViewItem{T}.Visible"/> are set to true and the entire tree is visible.
        /// </summary>
        /// <param name="items">The items to reset</param>
        private static void ResetFilter(IEnumerable<TreeItemData<T>> items)
        {
            foreach (var item in items)
            {
                if (item.HasChildren)
                {
                    ResetFilter(item.Children);
                }

                item.Visible = true;
            }
        }

        /// <summary>
        /// Expands all items and their children.
        /// </summary>
        public async Task ExpandAllAsync()
        {
            foreach (var item in _childItems)
                await item.ExpandAllAsync();
        }

        /// <summary>
        /// Collapses all items and their children.
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
                if (AutoSelectParent)
                {
                    UpdateParentItem(clickedItem.Parent);
                }
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
