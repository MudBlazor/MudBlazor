using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A scrollable list for displaying text, avatars, and icons.
    /// </summary>
    /// <remarks>
    /// This component contains an optional <see cref="MudListSubheader"/> and one or more <see cref="MudListItem{T}"/>.
    /// </remarks>
    /// <typeparam name="T">The type of item being listed.</typeparam>
    /// <seealso cref="MudListItem{T}"/>
    /// <seealso cref="MudListSubheader"/>
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
            registerScope.RegisterParameter<bool>(nameof(Gutters))
                .WithParameter(() => Gutters)
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
        /// The color of the selected list item.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Primary"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The color of checkboxes when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public Color CheckBoxColor { get; set; } = Color.Default;

        /// <summary>
        /// The content within this list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Prevents list items from being selected.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Applies vertical padding to this list.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Padding { get; set; }

        /// <summary>
        /// Uses less vertical space for list items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// Applies left and right padding to all list items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Gutters { get; set; } = true;

        /// <summary>
        /// Prevents any list item from being clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Controls how list items are selected.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SelectionMode.SingleSelection"/>.<br />
        /// Use <see cref="SelectionMode.SingleSelection"/> to select one list item at a time.<br />
        /// Use <see cref="SelectionMode.MultiSelection"/> to allow selecting multiple list items.<br />
        /// Use <see cref="SelectionMode.ToggleSelection"/> to toggle selections on and off when clicked.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public SelectionMode SelectionMode { get; set; } = SelectionMode.SingleSelection;

        /// <summary>
        /// The currently selected value.
        /// </summary>
        /// <remarks>
        /// This value is updated when <see cref="SelectionMode"/> is <see cref="SelectionMode.SingleSelection"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public T? SelectedValue { get; set; }

        /// <summary>
        /// Occurs when <see cref="SelectedValue"/> has changed.
        /// </summary>
        /// <remarks>
        /// This event occurs when <see cref="SelectionMode"/> is <see cref="SelectionMode.SingleSelection"/>.
        /// </remarks>
        [Parameter]
        public EventCallback<T?> SelectedValueChanged { get; set; }

        /// <summary>
        /// The currently selected values.
        /// </summary>
        /// <remarks>
        /// This value is updated when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public IReadOnlyCollection<T>? SelectedValues { get; set; }

        /// <summary>
        /// Occurs when <see cref="SelectedValues"/> has changed.
        /// </summary>
        /// <remarks>
        /// This event occurs when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection"/>.
        /// </remarks>
        [Parameter]
        public EventCallback<IReadOnlyCollection<T>?> SelectedValuesChanged { get; set; }

        /// <summary>
        /// The comparer used to see if two list items are equal.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="EqualityComparer{T}.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public IEqualityComparer<T?> Comparer { get; set; } = EqualityComparer<T?>.Default;

        /// <summary>
        /// The icon to use for checked checkboxes when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.CheckBox"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// The icon to use for unchecked checkboxes when <see cref="SelectionMode"/> is <see cref="SelectionMode.MultiSelection"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.CheckBoxOutlineBlank"/>.
        /// </remarks>
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

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        public void Dispose()
        {
            ParentList?.Unregister(this);
        }
    }
}
