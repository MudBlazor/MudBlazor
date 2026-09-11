using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
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
        // Server-load state for items which are not bound to a backing data item (ChildContent trees).
        private readonly TreeViewServerLoadEntry _localServerLoad = new();
        private readonly ParameterState<bool> _selectedState;
        private readonly ParameterState<bool> _expandedState;
        private readonly ParameterState<IReadOnlyCollection<ITreeItemData<T>>?> _itemsState;
        private readonly DefaultConverter<T?> _converter = new();
        private readonly HashSet<MudTreeViewItem<T>> _childItems = new();
        private bool? _renderedCheckBoxState;
        private bool _hasRenderedCheckBoxState;
        private bool _isDisposed;

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
            _itemsState = registerScope.RegisterParameter<IReadOnlyCollection<ITreeItemData<T>>?>(nameof(Items))
                .WithParameter(() => Items)
                .WithEventCallback(() => ItemsChanged);
        }

        protected string Classname =>
            new CssBuilder("mud-treeview-item")
                .AddClass("mud-treeview-select-none", GetExpandOnDoubleClick)
                .AddClass("mud-treeview-item-disabled", GetDisabled())
                .AddClass("mud-treeview-item-virtualized", IsVirtualizedItem)
                .AddClass(Class)
                .Build();

        protected string Stylename =>
            new StyleBuilder()
                .AddStyle("--mud-treeview-item-depth", (CurrentItemContext?.Depth ?? 0).ToString(CultureInfo.InvariantCulture), IsVirtualizedItem)
                .AddStyle(Style)
                .Build();

        protected string ContentClassname =>
            new CssBuilder("mud-treeview-item-content")
                .AddClass("cursor-pointer", !GetDisabled() && (!GetReadOnly() || (GetExpandOnClick() && HasChildren())))
                .AddClass("mud-ripple", GetRipple() && !GetDisabled() && !GetExpandOnDoubleClick() && (!GetReadOnly() || (GetExpandOnClick() && HasChildren())))
                .AddClass("mud-treeview-item-selected", !GetDisabled() && !MultiSelection && IsSelected())
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

        // When the item comes from ItemTemplate, this links the component instance to its backing node object.
        [CascadingParameter(Name = MudTreeViewCascadingValues.ItemData)]
        internal ITreeItemData<T>? CurrentItemData { get; set; }

        // Set only when the item is rendered as a row of a virtualized tree.
        [CascadingParameter(Name = MudTreeViewCascadingValues.ItemContext)]
        internal TreeViewItemContext<T>? CurrentItemContext { get; set; }

        private bool IsVirtualizedItem => CurrentItemContext is not null;

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
        /// Ignored for a row of a virtualized tree, which reads <see cref="TreeItemData{T}.Visible"/> from the backing data instead.
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
        public RenderFragment<MudTreeViewItem<T>>? BodyContent { get; set; }

        /// <summary>
        /// The child items underneath this item.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.TreeView.Data)]
        public IReadOnlyCollection<ITreeItemData<T>>? Items { get; set; }

        /// <summary>
        /// Occurs when <see cref="Items"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<IReadOnlyCollection<ITreeItemData<T>>?> ItemsChanged { get; set; }

        /// <summary>
        /// Shows the children items underneath this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter, ParameterState]
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
        [Parameter, ParameterState]
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
        /// <remarks>
        /// In a virtualized tree only rendered rows raise this event, so use <see cref="MudTreeView{T}.SelectedValuesChanged"/> for the complete selection.
        /// </remarks>
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

        private bool HasChildren()
        {
            if (IsVirtualizedItem)
            {
                // Loading is refused for backing items which are not expandable, so no expand button is offered for them either.
                return CurrentItemContext!.HasVisibleChildren
                    || (MudTreeRoot?.ServerData != null && CanExpand && CurrentItemData?.Expandable != false && !GetServerDataLoaded() && GetItems().Count == 0);
            }

            return ChildContent != null
                || (MudTreeRoot != null && GetItems().Count != 0)
                || (MudTreeRoot?.ServerData != null && CanExpand && !GetServerDataLoaded() && GetItems().Count == 0);
        }

        private bool AreChildrenVisible() => _itemsState.Value is null || _itemsState.Value.Any(i => i.Visible);

        private IReadOnlyCollection<ITreeItemData<T>> GetItems()
        {
            if (IsVirtualizedItem)
            {
                return CurrentItemData?.Children ?? Array.Empty<ITreeItemData<T>>();
            }

            if (_itemsState.Value == null)
                return Array.Empty<ITreeItemData<T>>();
            return _itemsState.Value!;
        }

        private bool GetExpanded() => IsVirtualizedItem && CurrentItemData is not null
            ? CurrentItemData.Expanded
            : _expandedState.Value;

        internal T? GetValue()
        {
            if (typeof(T) == typeof(string) && Value is null && Text is not null)
            {
                return (T)(object)Text;
            }
            return Value;
        }

        private string? GetText() => string.IsNullOrEmpty(Text) ? _converter.Convert(Value) : Text;

        private bool GetDisabled() => Disabled || MudTreeRoot?.Disabled == true;

        /// <summary>
        /// Returns the server-load state of this item.
        /// </summary>
        /// <remarks>
        /// Items bound to a backing data item share state through the tree so that it survives re-rendering and mode switches.
        /// </remarks>
        private TreeViewServerLoadEntry GetServerLoadEntry()
        {
            return CurrentItemData is not null && MudTreeRoot is not null
                ? MudTreeRoot.GetServerLoadEntry(CurrentItemData)
                : _localServerLoad;
        }

        private bool GetServerDataLoaded() => GetServerLoadEntry().IsLoaded;

        private bool GetServerDataLoading() => GetServerLoadEntry().IsLoading;

        /// <summary>
        /// Gets the tri-state checkbox value, remembering what was rendered.
        /// </summary>
        /// <remarks>
        /// The value is derived from the sub-items, which can change without this item being told.
        /// Remembering what was last rendered lets <see cref="UpdateSelectionStateCoreAsync"/> tell whether a new render would actually produce anything different.
        /// </remarks>
        private bool? GetCheckBoxStateTriState()
        {
            var state = ComputeCheckBoxState();
            _renderedCheckBoxState = state;
            _hasRenderedCheckBoxState = true;

            return state;
        }

        private bool? ComputeCheckBoxState()
        {
            return MudTreeRoot?.TriState != true
                ? IsSelected()
                : ComputeCheckBoxStateTriState();
        }

        private bool? ComputeCheckBoxStateTriState()
        {
            if (IsVirtualizedItem)
            {
                return CurrentItemContext!.HasSelectableValues ? CurrentItemContext.CheckState : false;
            }

            var hasSelectedDescendant = _selectedState.Value;
            // An item without a value cannot be selected, so it does not count as an unselected descendant and its box follows its children, as in the virtualized projection.
            var hasUnselectedDescendant = GetValue() is not null && !_selectedState.Value;

            foreach (var child in _childItems)
            {
                Traverse(child);
                if (hasSelectedDescendant && hasUnselectedDescendant)
                {
                    break;
                }
            }

            if (hasSelectedDescendant && hasUnselectedDescendant)
            {
                return null;
            }

            return hasSelectedDescendant;

            void Traverse(MudTreeViewItem<T> item)
            {
                if (item.GetState<bool>(nameof(Selected)))
                {
                    hasSelectedDescendant = true;
                }
                else if (item.GetValue() is not null)
                {
                    // A descendant without a value cannot be selected, so it does not count as an unselected
                    // descendant, matching how the virtualized projection derives the same state.
                    hasUnselectedDescendant = true;
                }

                if (!hasSelectedDescendant || !hasUnselectedDescendant)
                {
                    foreach (var child in item._childItems)
                    {
                        Traverse(child);
                        if (hasSelectedDescendant && hasUnselectedDescendant)
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Expands this item and all children recursively.
        /// </summary>
        public async Task ExpandAllAsync()
        {
            if (!CanExpand)
            {
                return;
            }
            if (IsVirtualizedItem && CurrentItemData is not null)
            {
                // A virtualized row has no child components, so its backing data is expanded and re-projected instead.
                if (TreeViewHierarchy<T>.ExpandAll([CurrentItemData]))
                {
                    MudTreeRoot?.RefreshProjection();
                }
                return;
            }
            if (_childItems.Count == 0)
            {
                return;
            }
            if (!_expandedState)
            {
                await _expandedState.SetValueAsync(true);
                StateHasChanged();
            }
            foreach (var item in _childItems)
            {
                await item.ExpandAllAsync();
            }
        }

        /// <summary>
        /// Collapse this item and all children recursively.
        /// </summary>
        public async Task CollapseAllAsync()
        {
            if (IsVirtualizedItem && CurrentItemData is not null)
            {
                if (TreeViewHierarchy<T>.CollapseAll([CurrentItemData]))
                {
                    MudTreeRoot?.RefreshProjection();
                }
                return;
            }
            if (_expandedState)
            {
                await _expandedState.SetValueAsync(false);
                StateHasChanged();
            }
            foreach (var item in _childItems)
            {
                await item.CollapseAllAsync();
            }
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

            // Virtualized rows mirror the backing item; selection changes on the data are reconciled by the tree.
            if (IsVirtualizedItem && CurrentItemData?.Selected == arg.Value)
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
                await SetExpandedAsync(!GetExpanded());
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

        private void OnArrowDoubleClick()
        {
            /* Don't do anything on purpose. Fixes #9419 */
        }

        private async Task OnItemDoubleClickedAsync(MouseEventArgs ev)
        {
            if (HasChildren() && GetExpandOnDoubleClick())
            {
                await SetExpandedAsync(!GetExpanded());
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
            if (GetExpanded() != expanded)
            {
                await SetExpandedAsync(expanded);
            }
        }

        private async Task SetExpandedAsync(bool expanded)
        {
            if (IsVirtualizedItem && CurrentItemData is not null)
            {
                CurrentItemData.Expanded = expanded;
            }

            await _expandedState.SetValueAsync(expanded);
            await TryInvokeServerLoadFunc();
            MudTreeRoot?.RefreshProjection();
        }

        internal bool IsSelected() => IsVirtualizedItem ? CurrentItemContext?.IsSelected == true : _selectedState.Value;

        /// <summary>
        /// Clears the children under this item.
        /// </summary>
        public async Task ReloadAsync()
        {
            var version = GetServerLoadEntry().Reset();
            if (_itemsState.Value is not null)
            {
                await _itemsState.SetValueAsync(Array.Empty<ITreeItemData<T>>());
                // A newer reload may have started while the items callback ran, and the children it loaded must not be wiped by this one.
                if (GetServerLoadEntry().Version != version)
                {
                    return;
                }
            }

            // Only clear the backing children when a load will actually run to replace them.
            // The children count is deliberately not part of this check, because clearing them is
            // what lets TryInvokeServerLoadFunc past its own "already has children" guard.
            var willLoad = MudTreeRoot?.ServerData is not null
                && CanExpand
                && !(IsVirtualizedItem && CurrentItemData is { Expandable: false });

            if (willLoad && CurrentItemData is not null)
            {
                CurrentItemData.Children = Array.Empty<ITreeItemData<T>>();
                MudTreeRoot?.RefreshProjection();
            }

            await TryInvokeServerLoadFunc(version);

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

        internal IReadOnlyCollection<MudTreeViewItem<T>> ChildItems => _childItems;

        private bool HasIcon => (GetExpanded() && (!string.IsNullOrWhiteSpace(IconExpanded) || !string.IsNullOrWhiteSpace(Icon))) || (!GetExpanded() && !string.IsNullOrWhiteSpace(Icon));

        private string? GetIcon() => GetExpanded() && !string.IsNullOrWhiteSpace(IconExpanded) ? IconExpanded : Icon;

        internal IEnumerable<MudTreeViewItem<T>> GetSelectedItems()
        {
            if (IsSelected())
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

        internal Task TryInvokeServerLoadFunc() => TryInvokeServerLoadFunc(reservedVersion: null);

        /// <summary>
        /// Loads children through <see cref="MudTreeView{T}.ServerData"/> unless the item already has children or a load is in progress.
        /// </summary>
        /// <param name="reservedVersion">A generation reserved by <see cref="ReloadAsync"/>; the load is skipped when a newer request superseded it.</param>
        /// <remarks>
        /// The result is written to the backing data item when there is one, so it is visible to both renderers.
        /// A completion whose generation was superseded by a reload is discarded, including its failure.
        /// </remarks>
        private async Task TryInvokeServerLoadFunc(long? reservedVersion)
        {
            var treeRoot = MudTreeRoot;
            var dataItem = CurrentItemData;
            if (treeRoot?.ServerData is null
                || !CanExpand
                || (IsVirtualizedItem && dataItem is { Expandable: false })
                || GetItems().Count != 0)
            {
                return;
            }

            var entry = GetServerLoadEntry();
            if (entry.IsLoading || (reservedVersion is { } reserved && reserved != entry.Version))
            {
                return;
            }

            var version = reservedVersion ?? ++entry.Version;
            entry.IsLoading = true;
            entry.IsLoaded = false;
            StateHasChanged();
            treeRoot.RefreshProjection();

            var loaded = false;
            try
            {
                var items = await treeRoot.ServerData(GetValue());
                if (version != entry.Version || treeRoot.IsDisposed)
                {
                    return;
                }

                if (dataItem is not null)
                {
                    dataItem.Children = items;
                }

                await _itemsState.SetValueAsync(items);
                loaded = true;
            }
            catch (Exception) when (version != entry.Version || treeRoot.IsDisposed)
            {
                // A reload or disposal superseded this request.
                // Its result and failure no longer belong to the item.
            }
            finally
            {
                if (version == entry.Version)
                {
                    entry.IsLoading = false;
                    entry.IsLoaded = loaded;
                    StateHasChanged();
                    // A standard tree only needs a render from the root when this item was disposed while the load was pending, because its replacement has to pick up the loaded children.
                    treeRoot.RefreshProjection(alwaysRender: _isDisposed);
                }
            }
        }

        /// <summary>
        /// Updates the selection state of all items and sub-items.
        /// </summary>
        /// <param name="selectedValues"></param>
        /// <returns>True if the item or any sub-item changed from non-selected to selected.</returns>
        /// <param name="forceRender">Renders every item regardless of whether its state changed.</param>
        internal async Task<bool> UpdateSelectionStateAsync(HashSet<T> selectedValues, bool forceRender = false)
        {
            var (selectedBecameTrue, _) = await UpdateSelectionStateCoreAsync(selectedValues, forceRender);

            return selectedBecameTrue;
        }

        /// <summary>
        /// Updates the selection state of this item and its sub-items, reporting whether anything actually changed.
        /// </summary>
        /// <remarks>
        /// The tree walks every item whenever the selection changes, and it does so once per item while the tree mounts.
        /// Rendering unconditionally therefore rebuilt every item twice just to mount, and rebuilt the whole tree when a single item was clicked.
        /// Multi-selection also renders when the tri-state checkbox no longer matches what was rendered, because that value is derived from the sub-items and can go stale without this item's own state changing.
        /// A virtualized row always renders, because its selection and checkbox state come from the projection which was rebuilt for the same change.
        /// </remarks>
        /// <param name="selectedValues">The values that are currently selected.</param>
        /// <param name="forceRender">
        /// Renders every item regardless of whether its state changed.
        /// The tree root is cascaded with a fixed value, so items are never told when a root parameter such as <see cref="MudTreeView{T}.Disabled"/> changes, and an item whose own parameters are unchanged is not re-rendered by the framework either.
        /// Walking the tree is what pushes those values down, so a walk caused by a root parameter change has to render even where nothing about the selection moved.
        /// </param>
        /// <returns>Whether the item or any sub-item became selected, and whether anything rendered by this item changed.</returns>
        private async Task<(bool SelectedBecameTrue, bool Changed)> UpdateSelectionStateCoreAsync(HashSet<T> selectedValues, bool forceRender)
        {
            if (MudTreeRoot == null)
            {
                return (false, false);
            }
            var value = GetValue();
            var selected = value is not null && selectedValues.Contains(value);
            var wasSelected = IsSelected();
            var selectedBecameTrue = selected && !wasSelected;
            if (IsVirtualizedItem && CurrentItemData is not null)
            {
                CurrentItemData.Selected = selected;
            }
            await _selectedState.SetValueAsync(selected);
            var changed = selected != wasSelected || IsVirtualizedItem;
            // since the tree view doesn't know our children we need to take care of updating them
            bool childSelectedBecameTrue = false;
            foreach (var child in _childItems)
            {
                var (becameTrue, childChanged) = await child.UpdateSelectionStateCoreAsync(selectedValues, forceRender);
                childSelectedBecameTrue = childSelectedBecameTrue || becameTrue;
                changed = changed || childChanged;
            }
            if (GetAutoExpand() && CanExpand && childSelectedBecameTrue && !_expandedState)
            {
                await _expandedState.SetValueAsync(true);
                changed = true;
            }
            if (forceRender || changed || (MultiSelection && _hasRenderedCheckBoxState && ComputeCheckBoxState() != _renderedCheckBoxState))
            {
                StateHasChanged();
            }
            return (selectedBecameTrue || childSelectedBecameTrue, changed);
        }

        /// <summary>
        /// Disposes the resources used by this component.
        /// </summary>
        public void Dispose()
        {
            _isDisposed = true;
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
            return IsSelected() ? CheckedIcon : UncheckedIcon;
        }
    }
}
