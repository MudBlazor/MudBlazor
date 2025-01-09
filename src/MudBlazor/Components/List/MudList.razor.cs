using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Services;
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
        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        private string _elementId = Identifier.Create("list");
        private string? _activeItemId;

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

        /// <summary>
        /// Occurs when a key has been pressed down.
        /// </summary>
        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        /// <summary>
        /// Occurs when a pressed key has been released.
        /// </summary>
        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (ParentList is not null)
            {
                TopLevelList = ParentList.TopLevelList;
                ParentList.Register(this);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // setup key interceptor    
            if (firstRender)
            {
                var options = new KeyInterceptorOptions(
                    "mud-list-item", true,
                    [
                        // prevent scrolling page, toggle open/close
                        new(" ", preventDown: "key+none"),
                        // prevent scrolling page, instead highlight previous item
                        new("ArrowUp", preventDown: "key+none"),
                        // prevent scrolling page, instead highlight next item
                        new("ArrowDown", preventDown: "key+none"),
                        new("Home", preventDown: "key+none"),
                        new("End", preventDown: "key+none"),
                        new("Enter", preventDown: "key+none"),
                        new("NumpadEnter", preventDown: "key+none"),
                        // select all items instead of all page text
                        new("a", preventDown: "key+ctrl"),
                        // select all items instead of all page text
                        new("A", preventDown: "key+ctrl"),
                        // for our users
                        new("/./", subscribeDown: true, subscribeUp: true)
                    ]);

                await KeyInterceptorService.SubscribeAsync(_elementId, options, keyDown: HandleKeyDownAsync, keyUp: HandleKeyUpAsync);
            }
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

        private async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (GetDisabled() || GetReadOnly())
                return;

            var key = args.Key.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(key) == false
                && key.Length == 1
                && char.IsLetterOrDigit(key[0]))
            {
                await FocusFirstItemAsync(key);
                return;
            }

            switch (args.Key)
            {
                case "ArrowDown":
                    await FocusNextItemAsync();
                    break;
                case "ArrowUp":
                    await FocusPreviousItemAsync();
                    break;
                case "Home":
                    await FocusFirstItemAsync();
                    break;
                case "End":
                    await FocusLastItemAsync();
                    break;
                case "Enter":
                case "NumpadEnter":
                case " ":
                    {
                        if (_activeItemId is not null)
                        {
                            var item = _items.FirstOrDefault(x => x.ItemId == _activeItemId);
                            item?.OnItemClickAsync();
                        }

                        break;
                    }
                default:
                    await OnKeyDown.InvokeAsync(args);
                    break;
            }
        }

        private Task HandleKeyUpAsync(KeyboardEventArgs args)
        {
            return OnKeyUp.InvokeAsync(args);
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

        internal void SetFocusedItem(T? value)
        {
            _activeItemId = _items.FirstOrDefault(x => Comparer.Equals(x.GetValue(), value))?.ItemId ?? string.Empty;
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

                if (selected)
                {
                    _activeItemId = item.ItemId;
                }
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

        /// <summary>
        /// gets the role of the MudList
        /// </summary>
        /// <returns>the role of the MudList</returns>
        /// <remarks>
        /// If <see crew="readonly"/> is true, the role is "list". Otherwise, the role is "listbox".
        ///     </remarks>
        private string GetRole()
        {
            return GetReadOnly() ? "list" : "listbox";
        }

        private string GetAriaMultiselectableValue() => SelectionMode == SelectionMode.MultiSelection ? "true" : "false";

        private async Task FocusAdjacentItemAsync(int direction)
        {
            if (_items.Count == 0)
                return;

            var itemList = _items.ToList();
            var index = itemList.FindIndex(x => x.ItemId == _activeItemId);
            if (direction < 0 && index < 0)
                index = 0;
            MudListItem<T>? item = null;
            // the loop allows us to jump over disabled items until we reach the next non-disabled one
            for (var i = 0; i < itemList.Count; i++)
            {
                index += direction;
                if (index < 0)
                    index = 0;
                if (index >= itemList.Count)
                    index = itemList.Count - 1;
                if (itemList[index].Disabled)
                    continue;
                item = itemList[index];
                await item.OnFocusAsync();
                _activeItemId = item.ItemId;
                break;
            }
        }

        private async Task FocusPreviousItemAsync() => await FocusAdjacentItemAsync(-1);

        private async Task FocusNextItemAsync() => await FocusAdjacentItemAsync(1);

        private async Task FocusFirstItemAsync(string? startChar = null)
        {
            if (_items.Count == 0)
                return;

            var items = _items.Where(x => !x.Disabled);

            if (!string.IsNullOrWhiteSpace(startChar))
            {
                // Find first item that starts with the letter
                var currentItem = items.FirstOrDefault(x => x.ItemId == _activeItemId);
                if (currentItem is not null &&
                    currentItem.Text?.ToLowerInvariant().StartsWith(startChar) == true)
                {
                    // Step through items starting with the same letter
                    items = items.SkipWhile(x => x != currentItem).Skip(1);
                }
                items = items.Where(x => x.Text?.ToLowerInvariant().StartsWith(startChar) == true);
            }

            var item = items.FirstOrDefault();
            if (item is null)
                return;

            await item.OnFocusAsync();
        }

        private async Task FocusLastItemAsync()
        {
            if (_items.Count == 0)
                return;
            var item = _items.LastOrDefault(x => !x.Disabled);
            if (item is null)
                return;

            await item.OnFocusAsync();
        }

        internal int GetItemCount() => _items.Count;

        internal int GetIndexOfItemById(string itemId) => _items.ToList().FindIndex(x => x.ItemId == itemId);

    }
}
