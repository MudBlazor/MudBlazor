using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// An expandable branch of a <see cref="MudTreeView{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the selectable value held by the item.</typeparam>
    /// <remarks>
    /// Used as the data model of the tree.
    /// </remarks>
    /// <seealso cref="MudTreeView{T}"/>
    /// <seealso cref="MudTreeViewItemToggleButton"/>
    public partial class MudTreeViewItem<T> : MudComponentBase, IDisposable
    {
        private bool _isServerLoaded;
        private readonly ParameterState<bool> _selectedState;
        private readonly ParameterState<bool> _expandedState;
        private readonly ParameterState<IReadOnlyCollection<TreeItemData<T?>>?> _itemsState;
        private Converter<T> _converter = new DefaultConverter<T>();
        private readonly HashSet<MudTreeViewItem<T>> _childItems = new();

        public MudTreeViewItem()
        {
            using var registerScope = CreateRegisterScope();
            _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
                .WithParameter(() => Expanded)
                .WithEventCallback(() => ExpandedChanged);
            _selectedState = registerScope.RegisterParameter<bool>(nameof(Selected))
                .WithParameter(() => Selected)
                .WithEventCallback(() => SelectedChanged)
                .WithChangeHandler(OnSelectedParameterChangedAsync);
            _itemsState = registerScope.RegisterParameter<IReadOnlyCollection<TreeItemData<T?>>?>(nameof(Items))
                .WithParameter(() => Items)
                .WithEventCallback(() => ItemsChanged);
        }

        protected string Classname =>
            new CssBuilder("mud-treeview-item")
                .AddClass("mud-treeview-select-none", GetExpandOnDoubleClick)
                .AddClass("mud-treeview-item-disabled", GetDisabled())
                .AddClass(Class)
                .Build();

        protected string ContentClassname =>
            new CssBuilder("mud-treeview-item-content")
                .AddClass("cursor-pointer", !GetDisabled() && (!GetReadOnly() || GetExpandOnClick() && HasChildren()))
                .AddClass("mud-ripple", GetRipple() && !GetDisabled() && !GetExpandOnDoubleClick() && (!GetReadOnly() || GetExpandOnClick() && HasChildren()))
                .AddClass("mud-treeview-item-selected", !GetDisabled() && !MultiSelection && _selectedState)
                .Build();

        public string TextClassname =>
            new CssBuilder("mud-treeview-item-label")
                .AddClass(TextClass)
                .Build();

        private bool MultiSelection => MudTreeRoot?.MultiSelection == true;

        [CascadingParameter]
        private MudTreeView<T>? MudTreeRoot { get; set; }

        [CascadingParameter]
        internal MudTreeViewItem<T>? Parent { get; set; }

        /// <summary>
        /// The value associated with this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Acts as the displayed text if no text is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public T? Value { get; set; }

        /// <summary>
        /// The text to display.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. When no value is set, the <see cref="Value"/> is used if it is a basic value such as <c>string</c> or <c>int</c>, etc.<br />
        /// Ignored if <see cref="BodyContent"/> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string? Text { get; set; }

        /// <summary>
        /// The size of the text.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Typo.body1"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Typo TextTypo { get; set; } = Typo.body1;

        /// <summary>
        /// The CSS classes applied to the <see cref="Text"/> parameter.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Multiple values must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? TextClass { get; set; }

        /// <summary>
        /// The text at the end of the item.
        /// </summary>
        /// <remarks>
        /// Ignored if <see cref="BodyContent"/> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string? EndText { get; set; }

        /// <summary>
        /// The size of the end text.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Typo.body1"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Typo EndTextTypo { get; set; } = Typo.body1;

        /// <summary>
        /// The CSS classes applied to the <see cref="EndText"/> parameter.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Multiple values must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? EndTextClass { get; set; }

        /// <summary>
        /// Whether this item and its children are displayed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Prevents the user from interacting with this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Prevents this item from being selected.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Allows this item to expand to display children.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>. A value of <c>false</c> is typically used for lazy-loaded items via <see cref="MudTreeView{T}.ServerData" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public bool CanExpand { get; set; } = true;

        /// <summary>
        /// The child items within this item.
        /// </summary>
        /// <remarks>
        /// Must be one or more <see cref="MudTreeViewItem{T}"/> components. Only applies when <see cref="Content"/> is not set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The custom content within this item.
        /// </summary>
        /// <remarks>
        /// When set, completely controls the rendering of child items. For <see cref="MudTreeViewItem{T}"/> children, use <see cref="Items"/> or <see cref="ChildContent"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public RenderFragment? Content { get; set; }

        /// <summary>
        /// The custom content for the text, end text, and end icon.
        /// </summary>
        /// <remarks>
        /// When set, the <see cref="Text"/>, <see cref="EndText"/>, and <see cref="EndIcon"/> properties are ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public RenderFragment<MudTreeViewItem<T?>>? BodyContent { get; set; }

        /// <summary>
        /// The child items underneath this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public IReadOnlyCollection<TreeItemData<T?>>? Items { get; set; }

        /// <summary>
        /// Occurs when <see cref="Items"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<IReadOnlyCollection<TreeItemData<T?>>?> ItemsChanged { get; set; }

        /// <summary>
        /// Shows the children items underneath this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Expanding)]
        public bool Expanded { get; set; }

        /// <summary>
        /// Occurs when <see cref="Expanded"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        /// <summary>
        /// Selects this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. Can be set alongside other items if <see cref="MudTreeView{T}.SelectionMode"/> is <see cref="SelectionMode.MultiSelection"/> or <see cref="SelectionMode.ToggleSelection"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public bool Selected { get; set; }

        /// <summary>
        /// The item shown before the text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The icon shown when this item is expanded.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string? IconExpanded { get; set; }

        /// <summary>
        /// The color of the icon when <see cref="Icon"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Color IconColor { get; set; } = Color.Default;

        /// <summary>
        /// Icon placed after the text if set.
        /// </summary>
        /// <remarks>
        /// Ignored if <see cref="BodyContent"/> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string? EndIcon { get; set; }

        /// <summary>
        /// The color of the end icon when <see cref="EndIcon"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Color EndIconColor { get; set; } = Color.Default;

        /// <summary>
        /// The icon shown for the expand/collapse button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ChevronRight"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Expanding)]
        public string ExpandButtonIcon { get; set; } = Icons.Material.Filled.ChevronRight;

        /// <summary>
        /// The color of the expand/collapse button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Expanding)]
        public Color ExpandButtonIconColor { get; set; } = Color.Default;

        /// <summary>
        /// The icon shown while this item is loading.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Loop"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string LoadingIcon { get; set; } = Icons.Material.Filled.Loop;

        /// <summary>
        /// The color of the loading icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Color LoadingIconColor { get; set; } = Color.Default;

        /// <summary>
        /// Occurs when <see cref="Selected"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> SelectedChanged { get; set; }

        /// <summary>
        /// Occurs when this item has been clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// Occurs when this item has been double-clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnDoubleClick { get; set; }

        private string CheckedIcon => MudTreeRoot?.CheckedIcon ?? Icons.Material.Filled.CheckBox;

        private string UncheckedIcon => MudTreeRoot?.UncheckedIcon ?? Icons.Material.Filled.CheckBoxOutlineBlank;

        private string IndeterminateIcon => MudTreeRoot?.IndeterminateIcon ?? Icons.Material.Filled.IndeterminateCheckBox;

        private bool _loading;

        private bool HasChildren()
        {
            return ChildContent != null
                || (MudTreeRoot != null && GetItems().Count != 0)
                || (MudTreeRoot?.ServerData != null && CanExpand && !_isServerLoaded && GetItems().Count == 0);
        }

        private bool AreChildrenVisible() => _itemsState.Value is null || _itemsState.Value.Any(i => i.Visible);

        private IReadOnlyCollection<TreeItemData<T>> GetItems()
        {
            if (_itemsState.Value == null)
                return Array.Empty<TreeItemData<T>>();
            return _itemsState.Value!;
        }

        internal T? GetValue()
        {
            if (typeof(T) == typeof(string) && Value is null && Text is not null)
            {
                return (T)(object)Text;
            }
            return Value;
        }

        private string? GetText() => string.IsNullOrEmpty(Text) ? _converter.Set(Value) : Text;

        private bool GetDisabled() => Disabled || MudTreeRoot?.Disabled == true;

        private bool? GetCheckBoxStateTriState()
        {
            var allChildrenChecked = GetChildItemsRecursive().All(x => x.GetState<bool>(nameof(Selected)));
            var noChildrenChecked = GetChildItemsRecursive().All(x => !x.GetState<bool>(nameof(Selected)));
            if (allChildrenChecked && _selectedState)
            {
                return true;
            }
            if (noChildrenChecked && !_selectedState)
            {
                return false;
            }
            return null;
        }

        /// <summary>
        /// Expands this item and all children recursively.
        /// </summary>
        public async Task ExpandAllAsync()
        {
            if (!CanExpand || _childItems.Count == 0)
            {
                return;
            }
            if (!_expandedState)
            {
                await _expandedState.SetValueAsync(true);
                StateHasChanged();
            }
            foreach (var item in _childItems)
                await item.ExpandAllAsync();
        }

        /// <summary>
        /// Collapse this item and all children recursively.
        /// </summary>
        public async Task CollapseAllAsync()
        {
            if (_expandedState)
            {
                await _expandedState.SetValueAsync(false);
                StateHasChanged();
            }
            foreach (var item in _childItems)
                await item.CollapseAllAsync();
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Text == null && Value == null && MudTreeRoot?.ServerData != null)
                throw new InvalidOperationException(
                    $"'{nameof(MudTreeView<T>)}.{nameof(MudTreeRoot.ServerData)}' requires '{nameof(MudTreeRoot.ItemTemplate)}.{nameof(MudTreeViewItem<T>)}.{nameof(Value)}' to be supplied.");
        }

        private async Task OnCheckboxChangedAsync()
        {
            if (MudTreeRoot == null)
            {
                return;
            }
            await MudTreeRoot.OnItemClickAsync(this);
        }

        protected override async Task OnInitializedAsync()
        {
            if (Parent != null)
            {
                Parent.AddChild(this);
            }
            else
            {
                if (MudTreeRoot is not null)
                {
                    await MudTreeRoot.AddChildAsync(this);
                }
            }
            await base.OnInitializedAsync();
        }

        private Task OnSelectedParameterChangedAsync(ParameterChangedEventArgs<bool> arg)
        {
            if (MudTreeRoot is null)
            {
                return Task.CompletedTask;
            }
            var value = GetValue();
            if (value is null)
            {
                return Task.CompletedTask;
            }
            var selected = arg.Value;
            if (selected)
            {
                return MudTreeRoot.SelectAsync(value);
            }
            return MudTreeRoot.UnselectAsync(value);
        }

        private bool GetReadOnly() => ReadOnly || MudTreeRoot?.ReadOnly == true;

        private bool GetExpandOnClick() => MudTreeRoot?.ExpandOnClick == true;

        private bool GetExpandOnDoubleClick() => MudTreeRoot?.ExpandOnDoubleClick == true;

        private bool GetRipple() => MudTreeRoot?.Ripple == true;

        private bool GetAutoExpand() => MudTreeRoot?.AutoExpand == true;

        private async Task OnItemClickedAsync(MouseEventArgs ev)
        {
            // note: when both click and doubleClick are enabled, doubleClick wins
            if (HasChildren() && GetExpandOnClick() && !GetExpandOnDoubleClick())
            {
                await _expandedState.SetValueAsync(!_expandedState);
                await TryInvokeServerLoadFunc();
            }
            if (GetDisabled())
            {
                return;
            }
            if (!GetReadOnly())
            {
                if (MudTreeRoot is not null)
                {
                    await MudTreeRoot.OnItemClickAsync(this);
                }
            }
            await OnClick.InvokeAsync(ev);
        }

        private async Task OnItemDoubleClickedAsync(MouseEventArgs ev)
        {
            if (HasChildren() && GetExpandOnDoubleClick())
            {
                await _expandedState.SetValueAsync(!_expandedState);
                await TryInvokeServerLoadFunc();
            }
            if (GetDisabled())
            {
                return;
            }
            if (!GetReadOnly())
            {
                if (MudTreeRoot is not null)
                {
                    await MudTreeRoot.OnItemClickAsync(this);
                }
            }
            await OnDoubleClick.InvokeAsync(ev);
        }

        private async Task OnItemExpanded(bool expanded)
        {
            if (_expandedState != expanded)
            {
                await _expandedState.SetValueAsync(expanded);
                await TryInvokeServerLoadFunc();
            }
        }

        /// <summary>
        /// Clears the children under this item.
        /// </summary>
        public async Task ReloadAsync()
        {
            if (_itemsState.Value is not null)
            {
                await _itemsState.SetValueAsync(Array.Empty<TreeItemData<T?>>());
            }
            await TryInvokeServerLoadFunc();

            if (Parent != null)
            {
                Parent.StateHasChanged();
            }
            else if (MudTreeRoot is not null)
            {
                ((IMudStateHasChanged)MudTreeRoot).StateHasChanged();
            }
        }

        private void AddChild(MudTreeViewItem<T> item) => _childItems.Add(item);

        private void RemoveChild(MudTreeViewItem<T> item) => _childItems.Remove(item);

        internal List<MudTreeViewItem<T>> ChildItems => _childItems.ToList();

        private bool HasIcon => _expandedState && (!string.IsNullOrWhiteSpace(IconExpanded) || !string.IsNullOrWhiteSpace(Icon)) || !_expandedState && !string.IsNullOrWhiteSpace(Icon);

        private string? GetIcon() => _expandedState && !string.IsNullOrWhiteSpace(IconExpanded) ? IconExpanded : Icon;

        internal IEnumerable<MudTreeViewItem<T>> GetSelectedItems()
        {
            if (_selectedState)
            {
                yield return this;
            }

            foreach (var treeItem in _childItems)
            {
                foreach (var selected in treeItem.GetSelectedItems())
                {
                    yield return selected;
                }
            }
        }

        internal async Task TryInvokeServerLoadFunc()
        {
            if (GetItems().Count != 0 || !CanExpand || MudTreeRoot?.ServerData == null)
                return;
            _loading = true;
            StateHasChanged();
            try
            {
                var items = await MudTreeRoot.ServerData(GetValue());
                await _itemsState.SetValueAsync(items);
            }
            finally
            {
                _loading = false;
                _isServerLoaded = true;

                StateHasChanged();
            }
        }

        /// <summary>
        /// Updates the selection state of all items and sub-items.
        /// </summary>
        /// <param name="selectedValues"></param>
        /// <returns>True if the item or any sub-item changed from non-selected to selected.</returns>
        internal async Task<bool> UpdateSelectionStateAsync(HashSet<T> selectedValues)
        {
            if (MudTreeRoot == null)
            {
                return false;
            }
            var value = GetValue();
            var selected = value is not null && selectedValues.Contains(value);
            var selectedBecameTrue = selected && !_selectedState;
            await _selectedState.SetValueAsync(selected);
            // since the tree view doesn't know our children we need to take care of updating them
            bool childSelectedBecameTrue = false;
            foreach (var child in _childItems)
            {
                var becameTrue = await child.UpdateSelectionStateAsync(selectedValues);
                childSelectedBecameTrue = childSelectedBecameTrue || becameTrue;
            }
            if (GetAutoExpand() && CanExpand && childSelectedBecameTrue && !_expandedState)
            {
                await _expandedState.SetValueAsync(true);
            }
            StateHasChanged();
            return selectedBecameTrue || childSelectedBecameTrue;
        }

        /// <summary>
        /// Disposes the resources used by this component.
        /// </summary>
        public void Dispose()
        {
            MudTreeRoot?.RemoveChild(this);
            Parent?.RemoveChild(this);
        }

        internal List<MudTreeViewItem<T?>> GetChildItemsRecursive(List<MudTreeViewItem<T?>>? list = null)
        {
            list ??= new List<MudTreeViewItem<T?>>();
            foreach (var child in _childItems)
            {
                list.Add(child!);
                child.GetChildItemsRecursive(list);
            }
            return list;
        }

        private string GetIndeterminateIcon()
        {
            if (MudTreeRoot?.TriState == true)
            {
                return IndeterminateIcon;
            }
            // in non-tri-state mode we need to fake the checked status. the actual status of the checkbox is irrelevant,
            // only _selectedState.Value matters!
            return _selectedState ? CheckedIcon : UncheckedIcon;
        }
    }
}
