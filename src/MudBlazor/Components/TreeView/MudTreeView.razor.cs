using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.State;
using MudBlazor.State.Builder;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudTreeView<T> : MudComponentBase
    {
        public MudTreeView()
        {
            MudTreeRoot = this;
            _comparerState = RegisterParameterBuilder<IEqualityComparer<T?>>(nameof(Comparer))
                .WithParameter(() => Comparer)
                .WithChangeHandler(OnComparerChangedAsync);
            _selectedValueState = RegisterParameterBuilder<T?>(nameof(SelectedValue))
                .WithParameter(() => SelectedValue)
                .WithEventCallback(() => SelectedValueChanged)
                .WithChangeHandler(OnSelectedValueChangedAsync)
                .WithComparer(() => Comparer);
            _selectedValuesState = RegisterParameterBuilder<IReadOnlyCollection<T?>?>(nameof(SelectedValues))
                .WithParameter(() => SelectedValues)
                .WithEventCallback(() => SelectedValuesChanged)
                .WithChangeHandler(OnSelectedValuesChangedAsync);
            RegisterParameterBuilder<SelectionMode>(nameof(SelectionMode))
                .WithParameter(() => SelectionMode)
                .WithChangeHandler(OnParameterChanged)
                .Attach();
            _selectedValues = new();
        }

        private readonly ParameterState<T?> _selectedValueState;
        private readonly ParameterState<IEqualityComparer<T?>> _comparerState;
        private readonly ParameterState<IReadOnlyCollection<T?>?> _selectedValuesState;

        private object _selectedUpdateLock = new();
        private HashSet<T> _selectedValues;
        private HashSet<MudTreeViewItem<T>> _childItems = new();
        private bool _isFirstRender = true;
        internal bool MultiSelection => SelectionMode == SelectionMode.MultiSelection;
        private bool ToggleSelection => SelectionMode == SelectionMode.ToggleSelection;

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


        [CascadingParameter]
        private MudTreeView<T> MudTreeRoot { get; set; }

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
        /// The selection mode determines whether only a single item (SingleSelection) or multiple items
        /// can be selected (MultiSelection) and whether the selected item can be toggled off by clicking a
        /// second time (ToggleSelection).
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public SelectionMode SelectionMode { get; set; } = SelectionMode.SingleSelection;

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
        /// Hover effect for item's on mouse-over.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public bool Hover { get; set; }

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
        public IReadOnlyCollection<T?>? SelectedValues { get; set; }

        /// <summary>
        /// Called whenever the selection changes.
        /// </summary>
        [Parameter]
        public EventCallback<IReadOnlyCollection<T?>?> SelectedValuesChanged { get; set; }

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

        /// <summary>
        /// Supply a func that asynchronously loads tree view items on demand
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public Func<T?, Task<HashSet<T>>>? ServerData { get; set; }

        /// <summary>
        /// If true, the selection of the tree view can not be changed by clicking its items.
        /// The currently selected value(s) are still displayed however
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool ReadOnly { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && MudTreeRoot == this)
            {
                _isFirstRender = false;
                UpdateItems();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private Task OnSelectedValuesChangedAsync(ParameterChangedEventArgs<IReadOnlyCollection<T?>?> args)
        {
            if (_isFirstRender)
                return Task.CompletedTask;
            return SetSelectedValuesAsync(args.Value);
        }

        private Task OnSelectedValueChangedAsync(ParameterChangedEventArgs<T?> args)
        {
            Console.WriteLine($"SelectedValue {args.LastValue} => {args.Value}");
            if (_isFirstRender)
                return Task.CompletedTask;
            return SetSelectedValueAsync(args.Value);
        }

        private Task OnComparerChangedAsync(ParameterChangedEventArgs<IEqualityComparer<T?>> args)
        {
            if (_isFirstRender)
                return Task.CompletedTask;
            UpdateItems();
            return Task.CompletedTask;
        }
        private void OnParameterChanged()
        {
            if (_isFirstRender)
                return;
            UpdateItems();
        }

        internal async Task OnItemClickAsync(MudTreeViewItem<T> clickedItem)
        {
            if (ReadOnly)
                return;
            if (MultiSelection)
            {
                var items = clickedItem.GetChildItemsRecursive();
                items.Add(clickedItem!);
                var allSelected = items.All(x => x.GetState<bool>(nameof(MudTreeViewItem<T>.Selected)));
                // toggle selection of the clickedItem and its children
                foreach (var item in items.Where(x => x.Value is not null))
                {
                    if (allSelected)
                        _selectedValues.Remove(item.Value!);
                    else
                        _selectedValues.Add(item.Value!);
                }
                await _selectedValuesState.SetValueAsync(_selectedValues);
                UpdateItems();
                return;
            }
            var selected = clickedItem.GetState<bool>(nameof(MudTreeViewItem<T>.Selected));
            if (ToggleSelection)
                await SetSelectedValueAsync(selected ? default : clickedItem.Value); // <-- toggle selected value
            else if (!selected)
            {
                // SingleSelection
                await SetSelectedValueAsync(clickedItem.Value);
            }
        }

        internal void AddChild(MudTreeViewItem<T> item)
        {
            Console.WriteLine($"add child {item.Value}");
            _childItems.Add(item);
            if (MultiSelection)
                item.UpdateMultiSelectionState(_selectedValues);
            else
                item.UpdateSingleSelectionState(_selectedValueState);
        }

        internal void RemoveChild(MudTreeViewItem<T> item) => _childItems.Remove(item);

        ///  <summary>
        ///  Sets the selected value of the tree view in SingleSelection mode.
        ///  If the value is found, the corresponding item is selected; 
        ///  otherwise, selected value is set default.
        ///  If the selected item is valid it sets the corresponding tree item to selected.
        ///  </summary>
        ///  <param name="value">The value to be set as the selected value.</param>
        internal async Task SetSelectedValueAsync(T? value)
        {
            if (MultiSelection)
                return;
            var isValid = value != null && FindItemByValue(value) is not null;
            // note: if there is no item that corresponds to the value, the value is reset to default!
            await _selectedValueState.SetValueAsync(isValid ? value : default);
            UpdateItems();
        }

        /// <summary>
        /// Let the items update their selection state visualization and state according to
        /// the selection in the tree view
        /// </summary>
        private void UpdateItems()
        {
            foreach (var item in _childItems)
            {
                if (MultiSelection)
                    item.UpdateMultiSelectionState(_selectedValues);
                else
                    item.UpdateSingleSelectionState(_selectedValueState.Value);
            }
        }

        private async Task SetSelectedValuesAsync(IReadOnlyCollection<T?>? newValues)
        {
            // TODO
            await Task.Delay(0);
        }

        internal MudTreeViewItem<T>? FindItemByValue(T value, IEnumerable<MudTreeViewItem<T>>? children = null)
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
