using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{

    /// <summary>
    /// A scrollable list for displaying text, avatars, and icons. Use lists to help users find a specific item and act on it.
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

        private readonly ParameterState<T?> _selectedValueState;
        private readonly ParameterState<IReadOnlyCollection<T>?> _selectedValuesState;

        private readonly List<MudListItem<T>> _items = [];
        private readonly HashSet<MudList<T>> _childLists = new();
        private HashSet<T> _selection = new();
        private MudListItem<T>? _activeItem;
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
        [Parameter, ParameterState]
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
        [Parameter, ParameterState]
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

                if (EnsureActiveItem() is not null)
                {
                    StateHasChanged();
                }
            }
        }

        internal void Update()
        {
            StateHasChanged();
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
            if (_items.Contains(item))
            {
                return;
            }

            _items.Add(item);
            if (_selectedValueState.Value is not null && Equals(item.GetValue(), _selectedValueState.Value))
            {
                item.SetSelected(true);
                _activeItem = item;
                await _selectedValueState.SetValueAsync(item.GetValue());
                return;
            }

            if (_activeItem is null && item.IsEnabled())
            {
                _activeItem = item;
            }
        }

        internal void Unregister(MudListItem<T> item)
        {
            if (!_items.Remove(item))
            {
                return;
            }

            if (ReferenceEquals(_activeItem, item))
            {
                _activeItem = null;
                EnsureActiveItem();
            }
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
            StateHasChanged();
            if (SelectionMode == SelectionMode.MultiSelection)
            {
                UpdateSelectedItems(new HashSet<T>(TopLevelList.GetState<IReadOnlyCollection<T>?>(nameof(TopLevelList.SelectedValues)) ?? Array.Empty<T>(), Comparer));
            }
            else
            {
                UpdateSelectedItem(TopLevelList.GetState<T?>(nameof(TopLevelList.SelectedValue)));
            }
            foreach (var childList in _childLists.ToArray())
                childList.UpdateSelection();
        }

        /// <summary>
        /// Updates items and child lists with the current single selection
        /// </summary>
        private void UpdateSelectedItem(T? value)
        {
            MudListItem<T>? selectedItem = null;
            foreach (var item in _items.ToArray())
            {
                var selected = value is not null && Comparer.Equals(value, item.GetValue());
                item.SetSelected(selected);
                if (selected)
                {
                    selectedItem = item;
                }
            }
            foreach (var childList in _childLists.ToArray())
            {
                childList.UpdateSelectedItem(value);
            }

            if (selectedItem is not null)
            {
                SetActiveItem(selectedItem);
                return;
            }

            EnsureActiveItem();
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

            EnsureActiveItem();
        }

        internal bool IsInteractive() => !GetReadOnly();

        internal bool IsTabbable(MudListItem<T> item)
        {
            if (TopLevelList != this)
            {
                return TopLevelList.IsTabbable(item);
            }

            if (!IsInteractive() || !item.IsEnabled())
            {
                return false;
            }

            return ReferenceEquals(EnsureActiveItem(), item);
        }

        internal void SetActiveItem(MudListItem<T> item)
        {
            if (TopLevelList != this)
            {
                TopLevelList.SetActiveItem(item);
                return;
            }

            if (ReferenceEquals(_activeItem, item))
            {
                return;
            }

            var previous = _activeItem;
            _activeItem = item;
            ((IMudStateHasChanged?)previous)?.StateHasChanged();
            ((IMudStateHasChanged)item).StateHasChanged();
        }

        internal async Task FocusAdjacentItemAsync(MudListItem<T> currentItem, int direction)
        {
            var items = _items.Where(x => x.IsEnabled()).ToList();
            if (items.Count == 0)
            {
                return;
            }

            var currentIndex = items.FindIndex(x => ReferenceEquals(x, currentItem));
            if (currentIndex < 0)
            {
                currentIndex = direction > 0 ? -1 : items.Count;
            }

            var nextIndex = Math.Clamp(currentIndex + direction, 0, items.Count - 1);
            await items[nextIndex].FocusAsync();
        }

        internal async Task FocusBoundaryItemAsync(bool first)
        {
            var items = _items.Where(x => x.IsEnabled()).ToList();
            if (items.Count == 0)
            {
                return;
            }

            await (first ? items[0] : items[^1]).FocusAsync();
        }

        private MudListItem<T>? EnsureActiveItem()
        {
            if (TopLevelList != this)
            {
                return TopLevelList.EnsureActiveItem();
            }

            if (_activeItem?.IsEnabled() == true && _items.Contains(_activeItem))
            {
                return _activeItem;
            }

            _activeItem = _items.FirstOrDefault(x => x.IsEnabled());
            return _activeItem;
        }

        /// <summary>
        /// Builds fallback accessibility attributes for the list container.
        /// </summary>
        /// <remarks>
        /// <see cref="MudList{T}"/> derives its container semantics from its selection behavior.
        /// </remarks>
        private Dictionary<string, object?> GetUserAttributes()
        {
            var attributes = new Dictionary<string, object?>(UserAttributes, StringComparer.OrdinalIgnoreCase);
            attributes.TryAdd("role", GetReadOnly() ? "list" : "listbox");

            if (!GetReadOnly() && SelectionMode == SelectionMode.MultiSelection)
            {
                attributes.TryAdd("aria-multiselectable", "true");
            }

            return attributes;
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
