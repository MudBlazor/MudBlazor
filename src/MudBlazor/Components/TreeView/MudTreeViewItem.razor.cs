using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudTreeViewItem<T> : MudComponentBase, IDisposable
    {
        private bool _isServerLoaded;
        private readonly ParameterState<bool> _selectedState;
        private readonly ParameterState<bool> _expandedState;
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
        /// Value of the TreeViewItem. Acts as the displayed text if no text is set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public T? Value { get; set; }

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string? Text { get; set; }

        /// <summary>
        /// Typography for the text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Typo TextTypo { get; set; } = Typo.body1;

        /// <summary>
        /// User class names for the text, separated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? TextClass { get; set; }

        /// <summary>
        /// The text at the end of the item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string? EndText { get; set; }

        /// <summary>
        /// Typography for the endtext.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Typo EndTextTypo { get; set; } = Typo.body1;

        /// <summary>
        /// User class names for the endtext, separated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string? EndTextClass { get; set; }

        /// <summary>
        /// If true, TreeViewItem will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// If true, the MudTreeViewItem's selection can not be changed.  
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// If false, TreeViewItem will not be able to expand.
        /// </summary>
        /// <remarks>
        /// This is especially useful for lazy-loaded items via ServerData. If you know that an item has no children
        /// you can pre-emptively prevent expansion which would only lead to a server request that would
        /// not return children anyway.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public bool CanExpand { get; set; } = true;

        /// <summary>
        /// Child content of component used to create sub levels.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Content of the item, if used completely replaced the default rendering.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public RenderFragment? Content { get; set; }

        /// <summary>
        /// Content of the item body, if used replaced the text, end text and end icon rendering.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public RenderFragment<MudTreeViewItem<T?>>? BodyContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public IReadOnlyCollection<TreeItemData<T?>>? Items { get; set; }

        /// <summary>
        /// Expand or collapse TreeView item when it has children. Two-way bindable. Note: if you directly set this to
        /// true or false (instead of using two-way binding) it will force the item's expansion state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Expanding)]
        public bool Expanded { get; set; }

        /// <summary>
        /// Called whenever expanded changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        /// <summary>
        /// Set this to true to mark the item initially selected in single selection mode or checked in multi selection mode.
        /// You can two-way bind this to get selection updates from this item
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public bool Selected { get; set; }

        /// <summary>
        /// Icon placed before the text if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// Alternative icon to show instead of Icon if expanded.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string? IconExpanded { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Color IconColor { get; set; } = Color.Default;

        /// <summary>
        /// Icon placed after the text if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string? EndIcon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Color EndIconColor { get; set; } = Color.Default;

        /// <summary>
        /// The expand/collapse icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Expanding)]
        public string ExpandButtonIcon { get; set; } = Icons.Material.Filled.ChevronRight;

        /// <summary>
        /// The color of the expand/collapse button. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Expanding)]
        public Color ExpandButtonIconColor { get; set; } = Color.Default;

        /// <summary>
        /// The loading icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string LoadingIcon { get; set; } = Icons.Material.Filled.Loop;

        /// <summary>
        /// The color of the loading. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Color LoadingIconColor { get; set; } = Color.Default;

        /// <summary>
        /// Called whenever the selected value changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> SelectedChanged { get; set; }

        /// <summary>
        /// Tree item click event.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// Tree item double click event.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnDoubleClick { get; set; }

        private string CheckedIcon => MudTreeRoot?.CheckedIcon ?? Icons.Material.Filled.CheckBox;

        private string UncheckedIcon => MudTreeRoot?.UncheckedIcon ?? Icons.Material.Filled.CheckBoxOutlineBlank;

        private string IndeterminateIcon => MudTreeRoot?.IndeterminateIcon ?? Icons.Material.Filled.IndeterminateCheckBox;

        private bool _loading;

        private bool HasChildren()
        {
            return ChildContent != null ||
                   (MudTreeRoot != null && Items != null && Items.Count != 0) ||
                   (MudTreeRoot?.ServerData != null && CanExpand && !_isServerLoaded && (Items == null || Items.Count == 0));
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
        /// Expand this item and all its children recursively
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
        /// Collapse this item and all its children recursively
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
                Debug.Assert(MudTreeRoot != null);
                await MudTreeRoot.OnItemClickAsync(this);
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
                Debug.Assert(MudTreeRoot != null);
                await MudTreeRoot.OnItemClickAsync(this);
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
        /// Clear the tree items, and try to reload from server.
        /// </summary>
        public async Task ReloadAsync()
        {
            if (Items is not null)
            {
                Items = Array.Empty<TreeItemData<T?>>();
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
            if ((Items != null && Items.Count != 0) || !CanExpand || MudTreeRoot?.ServerData == null)
                return;
            _loading = true;
            StateHasChanged();
            try
            {
                Items = await MudTreeRoot.ServerData(GetValue());
            }
            finally
            {
                _loading = false;
                _isServerLoaded = true;

                StateHasChanged();
            }
        }

        /// <summary>
        /// Update the Selected state of all items and sub-items.
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
