using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudList<T> : MudComponentBase, IDisposable
    {
        public MudList()
        {
            TopLevelList = this;
            using var registerScope = CreateRegisterScope();
            _selectedValueState = registerScope.RegisterParameter<T?>(nameof(SelectedValue))
                .WithParameter(() => SelectedValue)
                .WithEventCallback(() => SelectedValueChanged)
                .WithChangeHandler(OnSelectedValueParameterChangedAsync)
                .WithComparer(() => Comparer);
            _selectedValuesState = registerScope.RegisterParameter<IReadOnlyCollection<T>?>(nameof(SelectedValues))
                .WithParameter(() => SelectedValues)
                .WithEventCallback(() => SelectedValuesChanged)
                .WithChangeHandler(OnSelectedValuesChangedAsync)
                .WithComparer(() => Comparer, x => new CollectionComparer<T>(x));
            registerScope.RegisterParameter<IEqualityComparer<T?>>(nameof(Comparer))
                .WithParameter(() => Comparer)
                .WithChangeHandler(OnComparerChangedAsync);
            registerScope.RegisterParameter<SelectionMode>(nameof(SelectionMode))
                .WithParameter(() => SelectionMode)
                .WithChangeHandler(UpdateSelection);
            registerScope.RegisterParameter<bool>(nameof(Dense))
                .WithParameter(() => Dense)
                .WithChangeHandler(Update);
            registerScope.RegisterParameter<bool>(nameof(Disabled))
                .WithParameter(() => Disabled)
                .WithChangeHandler(Update);
            registerScope.RegisterParameter<bool>(nameof(ReadOnly))
                .WithParameter(() => ReadOnly)
                .WithChangeHandler(Update);
        }

        private ParameterState<T?> _selectedValueState;
        private ParameterState<IReadOnlyCollection<T>?> _selectedValuesState;

        private HashSet<MudListItem<T>> _items = new();
        private HashSet<MudList<T>> _childLists = new();
        private HashSet<T> _selection = new();
        internal MudList<T> TopLevelList { get; private set; }

        protected string Classname =>
            new CssBuilder("mud-list")
                .AddClass("mud-list-padding", Padding)
                .AddClass(Class)
                .Build();

        [CascadingParameter]
        protected MudList<T>? ParentList { get; set; }

        /// <summary>
        /// The color of the selected List Item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Check box color if multiselection is used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public Color CheckBoxColor { get; set; } = Color.Default;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// If true, the list items will not be clickable and the selected item can not be changed by the user.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// If true, vertical padding will be applied to the list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Padding { get; set; }

        /// <summary>
        /// If true, list items will take up less vertical space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// If true, left and right padding is added to all list items. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Gutters { get; set; } = true;

        /// <summary>
        /// If true, will disable the list item if it has onclick.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The selection mode determines whether only a single item (SingleSelection or ToggleSelection) or multiple items
        /// can be selected (MultiSelection). The difference between SingleSelection and ToggleSelection is whether the selected
        /// item can be toggled off by clicking a second time.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public SelectionMode SelectionMode { get; set; } = SelectionMode.SingleSelection;

        /// <summary>
        /// The current selected value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public T? SelectedValue { get; set; }

        /// <summary>
        /// Called whenever the selection changed
        /// </summary>
        [Parameter]
        public EventCallback<T?> SelectedValueChanged { get; set; }

        /// <summary>
        /// The current selected value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public IReadOnlyCollection<T>? SelectedValues { get; set; }

        /// <summary>
        /// Called whenever the selection changed
        /// </summary>
        [Parameter]
        public EventCallback<IReadOnlyCollection<T>?> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Comparer is used to check if two tree items are equal
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public IEqualityComparer<T?> Comparer { get; set; } = EqualityComparer<T?>.Default;

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

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (ParentList is not null)
            {
                TopLevelList = ParentList.TopLevelList;
                ParentList.Register(this);
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender && TopLevelList == this)
            {
                if (SelectionMode == SelectionMode.MultiSelection)
                {
                    UpdateSelectedItems(_selection);
                }
                else
                {
                    UpdateSelectedItem(_selectedValueState);
                }
            }
        }

        internal void Update()
        {
            foreach (var item in _items)
                ((IMudStateHasChanged)item).StateHasChanged();
            foreach (var list in _childLists)
                list.Update();
        }

        /// <summary>
        /// Called when the SelectedValue parameter was changed outside the component
        /// </summary>
        private Task OnSelectedValueParameterChangedAsync(ParameterChangedEventArgs<T?> args)
        {
            return SetSelectedValueAsync(args.Value);
        }

        /// <summary>
        /// Called when the SelectedValues parameter was changed outside the component
        /// </summary>
        private void OnSelectedValuesChangedAsync(ParameterChangedEventArgs<IReadOnlyCollection<T>?> arg)
        {
            SetSelectedValues(arg.Value ?? Array.Empty<T>());
        }

        private void SetSelectedValues(IReadOnlyCollection<T> values)
        {
            _selection = new HashSet<T>(values, Comparer);
            UpdateSelectedItems(_selection);
        }

        private async Task OnComparerChangedAsync(ParameterChangedEventArgs<IEqualityComparer<T?>> args)
        {
            if (SelectionMode == SelectionMode.MultiSelection)
            {
                SetSelectedValues(new HashSet<T>(_selection, args.Value));
                await _selectedValuesState.SetValueAsync(_selection.ToList()); // note: ToList is essential here!
                return;
            }
            // single and toggle-selection
            UpdateSelectedItem(_selectedValueState);
        }

        internal async Task RegisterAsync(MudListItem<T> item)
        {
            _items.Add(item);
            if (SelectedValue is not null && Equals(item.GetValue(), SelectedValue))
            {
                item.SetSelected(true);
                await _selectedValueState.SetValueAsync(item.GetValue());
            }
        }

        internal void Unregister(MudListItem<T> item)
        {
            _items.Remove(item);
        }

        internal void Register(MudList<T> child)
        {
            _childLists.Add(child);
        }

        internal void Unregister(MudList<T> child)
        {
            _childLists.Remove(child);
        }

        internal bool GetDisabled() => Disabled || (ParentList?.Disabled ?? false);

        internal bool GetReadOnly() => ReadOnly || (ParentList?.ReadOnly ?? false);

        internal async Task SetSelectedValueAsync(T? value)
        {
            await _selectedValueState.SetValueAsync(value);
            // Find and update selected item based on value
            UpdateSelectedItem(value);
        }

        internal async Task SelectValueAsync(T? value)
        {
            if (SelectionMode != SelectionMode.MultiSelection || value is null)
            {
                return;
            }
            _selection.Add(value);
            UpdateSelectedItems(_selection);
            await _selectedValuesState.SetValueAsync(_selection.ToList()); // note: ToList is essential here!
        }

        internal async Task DeselectValueAsync(T? value)
        {
            if (SelectionMode != SelectionMode.MultiSelection || value is null)
            {
                return;
            }
            _selection.Remove(value);
            UpdateSelectedItems(_selection);
            await _selectedValuesState.SetValueAsync(_selection.ToList()); // note: ToList is essential here!
        }

        internal void UpdateSelection()
        {
            if (SelectionMode == SelectionMode.MultiSelection)
            {
                UpdateSelectedItems(new HashSet<T>(TopLevelList.SelectedValues ?? Array.Empty<T>(), Comparer));
            }
            else
            {
                UpdateSelectedItem(TopLevelList.SelectedValue);
            }
            foreach (var childList in _childLists.ToArray())
                childList.UpdateSelection();
        }

        /// <summary>
        /// Updates items and child lists with the current single selection
        /// </summary>
        private void UpdateSelectedItem(T? value)
        {
            foreach (var item in _items.ToArray())
            {
                var selected = value is not null && Comparer.Equals(value, item.GetValue());
                item.SetSelected(selected);
            }
            foreach (var childList in _childLists.ToArray())
            {
                childList.UpdateSelectedItem(value);
            }
        }

        /// <summary>
        /// Updates items and child lists with the current multi selection
        /// </summary>
        internal void UpdateSelectedItems(HashSet<T> selection)
        {
            foreach (var listItem in _items.ToArray())
            {
                var itemValue = listItem.GetValue();
                var selected = itemValue is not null && selection.Contains(itemValue);
                listItem.SetSelected(selected);
            }
            foreach (var childList in _childLists.ToArray())
            {
                childList.SetSelectedValues(selection);
            }
        }

        public void Dispose()
        {
            ParentList?.Unregister(this);
        }
    }
}
