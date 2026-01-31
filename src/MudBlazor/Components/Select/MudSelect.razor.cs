// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.State;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Comparer;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A dropdown input for selecting an item from a list of options.
    /// </summary>
    /// <typeparam name="T">The kind of object being selected.</typeparam>
    /// <seealso cref="MudSelectItem{T}"/>
    /// <seealso cref="MudAutocomplete{T}"/>
    public partial class MudSelect<T> : MudBaseInput<T>, IMudSelect, IMudShadowSelect
    {
        private string? _activeItemId;
        private bool? _selectAllChecked;
        private string? _multiSelectionText;
        private MudSelectItem<T>? _longestItem;
        private bool _needsHighlightAfterRender;
        private MudInput<string> _elementReference = null!;
        private HashSet<T?> _selectedValues = new HashSet<T?>();
        private readonly string _elementId = Identifier.Create("select");
        private string _searchText = string.Empty;
        private string? _lastSelectedId = string.Empty;
        private DateTime _lastSearchTime = DateTime.MinValue;
        private readonly ParameterState<IEnumerable<T?>?> _selectedValuesState;
        private readonly MudSelectContext<T> _context;

        /// <summary>
        /// Gets the context that manages communication with child items.
        /// </summary>
        /// <remarks>
        /// This context provides a clean, explicit communication model:
        /// <list type="bullet">
        /// <item>Items register/unregister explicitly</item>
        /// <item>Selection state is centralized</item>
        /// <item>Items observe changes via subscriptions</item>
        /// </list>
        /// </remarks>
        object IMudSelect.SelectContext => _context;

        /// <summary>
        /// Gets the context that manages shadow item registration.
        /// </summary>
        object IMudShadowSelect.SelectContext => _context;

        /// <summary>
        /// Gets the ordered list of all visible items.
        /// </summary>
        /// <remarks>
        /// This property now delegates to the context instead of maintaining its own list.
        /// </remarks>
        protected internal List<MudSelectItem<T>> _items => _context.Items;

        public MudSelect()
        {
            _context = new MudSelectContext<T>(this);
            Adornment = Adornment.End;
            IconSize = Size.Medium;
            // Set default value to ensure ParameterState never holds null
            SelectedValues = new HashSet<T?>();
            using var registerScope = CreateRegisterScope();
            registerScope.RegisterParameter<bool>(nameof(MultiSelection))
                .WithParameter(() => MultiSelection)
                .WithChangeHandler(() => UpdateTextPropertyAsync(false));
            registerScope.RegisterParameter<IEqualityComparer<T?>?>(nameof(Comparer))
                .WithParameter(() => Comparer)
                .WithChangeHandler(OnComparerChangedAsync);
            _selectedValuesState = registerScope.RegisterParameter<IEnumerable<T?>?>(nameof(SelectedValues))
                .WithParameter(() => SelectedValues)
                .WithEventCallback(() => SelectedValuesChanged)
                .WithChangeHandler(OnSelectedValuesChangedAsync)
                .WithComparer(() => new SequenceComparer<T?>(Comparer));
            registerScope.RegisterParameter<bool>(nameof(FitContent))
                .WithParameter(() => FitContent)
                .WithChangeHandler(OnFitContentChanged);
        }

        protected string OuterClassname =>
            new CssBuilder("mud-select")
                .AddClass("mud-width-full", FullWidth)
                .AddClass("mud-width-content", FitContent && !FullWidth)
                .AddClass(OuterClass)
                .Build();

        protected string Classname =>
            new CssBuilder("mud-select")
                .AddClass(Class)
                .Build();

        protected string InputClassname =>
            new CssBuilder("mud-select-input")
                .AddClass(InputClass)
                .Build();

        protected string FillerClassname =>
            new CssBuilder("mud-select-filler")
                .AddClass("d-inline-block")
                .AddClass("invisible")
                .AddClass("mx-2", Variant == Variant.Text)
                .AddClass("mx-4", Variant != Variant.Text)
                .Build();

        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        [Inject]
        private IScrollManager ScrollManager { get; set; } = null!;

        [Inject]
        private IPopoverService PopoverService { get; set; } = null!;

        private Task SelectNextItem() => SelectAdjacentItem(+1);

        private Task SelectPreviousItem() => SelectAdjacentItem(-1);

        private async Task SelectAdjacentItem(int direction)
        {
            if (_items.Count == 0)
                return;
            var index = _items.FindIndex(x => x.ItemId == _activeItemId);
            if (direction < 0 && index < 0)
                index = 0;
            MudSelectItem<T>? item = null;
            // the loop allows us to jump over disabled items until we reach the next non-disabled one
            for (var i = 0; i < _items.Count; i++)
            {
                index += direction;
                if (index < 0)
                    index = 0;
                if (index >= _items.Count)
                    index = _items.Count - 1;
                if (_items[index].Disabled)
                    continue;
                item = _items[index];
                if (!MultiSelection)
                {
                    // When SelectionOnEnter is true, we only update the visual highlight during navigation.
                    // When false (default), the value is immediately updated as the user moves through the list.
                    if (!SelectionOnEnter)
                    {
                        _selectedValues.Clear();
                        _selectedValues.Add(item.Value);
                        await SetValueAndUpdateTextAsync(item.Value, updateText: true);
                    }

                    await HighlightItemAsync(item);
                    break;
                }

                // in multiselect mode don't select anything, just highlight.
                // selecting is done by Enter
                await HighlightItemAsync(item);
                break;
            }
            await _elementReference.SetText(ReadText);
            await ScrollToItemAsync(item);
        }
        private ValueTask ScrollToItemAsync(MudSelectItem<T>? item)
            => item != null ? ScrollManager.ScrollToListItemAsync(item.ItemId) : ValueTask.CompletedTask;

        private async Task SelectFirstItem(string? startChar = null)
        {
            IEnumerable<MudSelectItem<T>> selectList = _context.Items;

            if (!_open)
            {
                // When closed, use shadow lookup to include all items (visible + hidden)
                selectList = GetAllShadowItems();
            }

            if (!selectList.Any())
                return;

            var items = selectList.Where(x => !x.Disabled);

            if (!string.IsNullOrWhiteSpace(startChar))
            {
                var searchItem = SelectItemBySearch(items, startChar);

                if (searchItem != null)
                {
                    await SelectAndHighlightItemAsync(searchItem);
                    return;
                }
            }

            // If no specific search or no matching items, select the first item
            var firstItem = items.FirstOrDefault();
            if (firstItem == null)
                return;

            await SelectAndHighlightItemAsync(firstItem);
        }

        private MudSelectItem<T>? SelectItemBySearch(IEnumerable<MudSelectItem<T>> items, string inputChar)
        {
            var now = DateTime.UtcNow;

            if (now - _lastSearchTime > QuickSearchInterval)
            {
                _lastSelectedId = _activeItemId;
                _searchText = inputChar;
            }
            else
            {
                _searchText += inputChar;
            }

            _lastSearchTime = now;

            var mudSelectItems = items as MudSelectItem<T>[] ?? items.ToArray();

            var matchingItems = mudSelectItems
                .Where(x => !x.Disabled && ConvertSet(x.Value)?.StartsWith(_searchText, StringComparison.InvariantCultureIgnoreCase) == true)
                .ToList();

            if (matchingItems.Count == 0)
                return mudSelectItems.FirstOrDefault(x => x.ItemId == _activeItemId);

            var currentItem = mudSelectItems.FirstOrDefault(x => x.ItemId == _activeItemId);
            if (currentItem == null)
                return matchingItems[0];

            var previousItem = mudSelectItems.First(x => x.ItemId == _lastSelectedId);
            var currentIndex = matchingItems.IndexOf(previousItem);
            var nextIndex = (currentIndex + 1) % matchingItems.Count;

            return matchingItems[nextIndex];
        }

        private async Task SelectAndHighlightItemAsync(MudSelectItem<T> item)
        {
            if (!MultiSelection)
            {
                _selectedValues.Clear();
                _selectedValues.Add(item.Value);
                await SetValueAndUpdateTextAsync(item.Value, updateText: true);
                // Update ParameterState to keep SelectedValues in sync
                await _selectedValuesState.SetValueAsync(new HashSet<T?>(_selectedValues, Comparer));
            }

            await HighlightItemAsync(item);
            await _elementReference.SetText(ReadText);
            await ScrollToItemAsync(item);
        }

        private async Task SelectLastItem()
        {
            if (_items.Count == 0)
                return;
            var item = _items.LastOrDefault(x => !x.Disabled);
            if (item == null)
                return;
            if (!MultiSelection)
            {
                _selectedValues.Clear();
                _selectedValues.Add(item.Value);
                await SetValueAndUpdateTextAsync(item.Value, updateText: true);
                await HighlightItemAsync(item);
            }
            else
            {
                await HighlightItemAsync(item);
            }
            await _elementReference.SetText(ReadText);
            await ScrollToItemAsync(item);
        }

        /// <summary>
        /// Displays the dropdown popover in a fixed position, even while scrolling.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Category(CategoryTypes.Popover.Behavior)]
        [Parameter]
        public bool PopoverFixed { get; set; }

        /// <summary>
        /// Determines the width of this Popover dropdown in relation to the parent container.
        /// </summary>
        /// <remarks>
        /// <para>Defaults to <see cref="DropdownWidth.Relative" />. </para>
        /// <para>When <see cref="DropdownWidth.Relative" />, restricts the max-width of the component to the width of the parent container</para>
        /// <para>When <see cref="DropdownWidth.Adaptive" />, restricts the min-width of the component to the width of the parent container</para>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public DropdownWidth RelativeWidth { get; set; } = DropdownWidth.Relative;

        /// <summary>
        /// Sets the container width to match its contents.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. Requires FullWidth to be <c>false</c>
        /// </remarks>
        [Parameter, ParameterState(ParameterUsage = ParameterUsageOptions.None)]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool FitContent { get; set; }

        /// <summary>
        /// The CSS classes applied to the outer <c>div</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter]
        public string? OuterClass { get; set; }

        /// <summary>
        /// The CSS classes applied to the input.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Category(CategoryTypes.FormComponent.Appearance)]
        [Parameter]
        public string? InputClass { get; set; }

        /// <summary>
        /// Occurs when this drop-down opens.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Behavior)]
        [Parameter]
        public EventCallback OnOpen { get; set; }

        /// <summary>
        /// Occurs when this drop-down closes.
        /// </summary>
        [Category(CategoryTypes.FormComponent.Behavior)]
        [Parameter]
        public EventCallback OnClose { get; set; }

        /// <summary>
        /// Prevents interaction with background elements while this list is open.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="PopoverOptions.ModalOverlay" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool? Modal { get; set; }

        /// <summary>
        /// Gets the resolved modal overlay value, using the global default from <see cref="PopoverOptions"/> if not explicitly set.
        /// </summary>
        protected bool GetModal() => Modal ?? PopoverService.PopoverOptions.ModalOverlay;

        /// <summary>
        /// The content within this component, typically a list of <see cref="MudSelectItem{T}"/> components.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The CSS classes applied to the popover.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string? PopoverClass { get; set; }

        /// <summary>
        /// The CSS classes applied to the internal list.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string? ListClass { get; set; }

        /// <summary>
        /// Uses compact vertical padding for all items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The icon for opening the popover of items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowDropDown"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string OpenIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// The icon for closing the popover of items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowDropUp"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string CloseIcon { get; set; } = Icons.Material.Filled.ArrowDropUp;

        /// <summary>
        /// Shows a "Select all" checkbox to select all items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Only applies when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool SelectAll { get; set; }

        /// <summary>
        /// The text of the "Select all" checkbox.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>"Select all"</c>.  Only applies when <see cref="SelectAll"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string SelectAllText { get; set; } = "Select all";

        /// <summary>
        /// Occurs when <see cref="SelectedValues"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<IEnumerable<T?>?> SelectedValuesChanged { get; set; }

        /// <summary>
        /// The custom function for setting the <c>Text</c> from a list of selected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Func<List<string?>?, string>? MultiSelectionTextFunc { get; set; }

        /// <summary>
        /// The string used to separate multiple selected values.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>", "</c>.  Only applies when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string Delimiter { get; set; } = ", ";

        /// <summary>
        /// The <see cref="TimeSpan"/> interval for accepting characters for search input.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TimeSpan.Zero"/> for single-character searches. <br/>
        /// Set to a value greater than zero to enable multi-character searches within the specified interval.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public TimeSpan QuickSearchInterval { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The currently selected values.
        /// </summary>
        /// <remarks>
        /// When <see cref="MultiSelection"/> is <c>false</c>, only one value will be returned.  When this value changes, <see cref="SelectedValuesChanged"/> occurs.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.Data)]
        public IEnumerable<T?>? SelectedValues { get; set; }

        private async Task OnSelectedValuesChangedAsync(ParameterChangedEventArgs<IEnumerable<T?>?> arg)
        {
            var value = arg.Value;
            var set = value ?? new HashSet<T?>(Comparer);

            // Update internal HashSet with new values - make a defensive copy to avoid shared references
            _selectedValues = new HashSet<T?>(set, Comparer);

            // Notify all subscribed items of the selection change
            // This replaces the SelectionChangedFromOutside event
            _context.NotifySelectionChanged();

            if (!MultiSelection)
            {
                await SetValueAndUpdateTextAsync(_selectedValues.FirstOrDefault());
            }
            else
            {
                //Warning. Here the Converter was not set yet
                if (MultiSelectionTextFunc != null)
                {
                    await SetCustomizedTextAsync(string.Join(Delimiter, _selectedValues.Select(ConvertSet)),
                        selectedConvertedValues: _selectedValues.Select(ConvertSet).ToList(),
                        multiSelectionTextFunc: MultiSelectionTextFunc);
                }
                else
                {
                    await SetTextAndUpdateValueAsync(string.Join(Delimiter, _selectedValues.Select(ConvertSet)), updateValue: false);
                }
            }

            // Only fire FieldChanged after the first render to avoid triggering during initialization
            if (HasRendered)
            {
                FieldChanged(_selectedValues);
            }
            if (MultiSelection && typeof(T) == typeof(string))
                await SetValueAndUpdateTextAsync((T?)(object?)ReadText, updateText: false);
        }

        /// <summary>
        /// The comparer for testing equality of selected values.
        /// </summary>
        [Parameter, ParameterState(ParameterUsage = ParameterUsageOptions.None)]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public IEqualityComparer<T?>? Comparer { get; set; }

        private async Task OnComparerChangedAsync(ParameterChangedEventArgs<IEqualityComparer<T?>?> arg)
        {
            // Apply comparer and refresh selected values
            _selectedValues = new HashSet<T?>(_selectedValues, arg.Value);
            await _selectedValuesState.SetValueAsync(new HashSet<T?>(_selectedValues, arg.Value));
        }

        /// <summary>
        /// The function for the <c>Text</c> in drop-down items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T?, string?>? ToStringFunc { get; set; }

        /// <summary>
        /// Whether the <c>Value</c> can be found in the list of <see cref="Items"/>.
        /// </summary>
        /// <remarks>
        /// When <c>false</c>, the <c>Value</c> will be displayed as a string.
        /// </remarks>
        protected bool CanRenderValue
        {
            get
            {
                if (MultiSelection)
                    return false;
                if (!_context.TryGetShadowItemByValue(ReadValue, out var item) || item == null)
                    return false;
                return item.ChildContent != null;
            }
        }

        protected bool IsValueInList
        {
            get
            {
                return _context.TryGetShadowItemByValue(ReadValue, out _);
            }
        }

        protected RenderFragment? GetSelectedValuePresenter()
        {
            if (!_context.TryGetShadowItemByValue(ReadValue, out var item) || item == null)
                return null; //<-- for now. we'll add a custom template to present values (set from outside) which are not on the list?
            return item.ChildContent;
        }

        protected override Task UpdateValuePropertyAsync(bool updateText)
        {
            // For MultiSelection of non-string T's we don't update the Value!!!
            if (typeof(T) == typeof(string) || !MultiSelection)
                base.UpdateValuePropertyAsync(updateText);
            return Task.CompletedTask;
        }

        protected override Task UpdateTextPropertyAsync(bool updateValue)
        {
            // when multiselection is true, we return
            // a comma separated list of selected values
            if (MultiSelectionTextFunc != null)
            {
                return MultiSelection
                    ? SetCustomizedTextAsync(string.Join(Delimiter, _selectedValues.Select(ConvertSet)),
                        selectedConvertedValues: _selectedValues.Select(ConvertSet).ToList(),
                        multiSelectionTextFunc: MultiSelectionTextFunc)
                    : base.UpdateTextPropertyAsync(updateValue);
            }

            return MultiSelection
                ? SetTextAndUpdateValueAsync(string.Join(Delimiter, _selectedValues.Select(ConvertSet)))
                : base.UpdateTextPropertyAsync(updateValue);
        }

        /// <summary>
        /// Allows multiple values to be selected via checkboxes.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>false</c>, only one value can be selected at a time.
        /// </remarks>
        [Parameter, ParameterState(ParameterUsage = ParameterUsageOptions.None)]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool MultiSelection { get; set; }

        /// <summary>
        /// The list of choices the user can select.
        /// </summary>
        /// <remarks>
        /// Use <see cref="MudSelectItem{T}"/> components to provide more items.
        /// This property now delegates to the context which manages item registration.
        /// </remarks>
        public IReadOnlyList<MudSelectItem<T>> Items => _context.Items;

        /// <summary>
        /// The maximum height, in pixels, of the popover of items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>300</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// The location where the popover will open from.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.BottomLeft" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Origin AnchorOrigin { get; set; } = Origin.BottomLeft;

        /// <summary>
        /// The transform origin point for the popover.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.TopLeft"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// Restricts the selected values to the ones defined in <see cref="MudSelectItem{T}"/> items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, any values not defined will not be displayed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Strict { get; set; }

        /// <summary>
        /// Shows a button for clearing any selected values.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the <see cref="ClearIcon"/> can be used to control the icon, and <see cref="OnClearButtonClick"/> occurs when the clear button is clicked.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// The icon displayed for the clear button when <see cref="Clearable"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Clear"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string ClearIcon { get; set; } = Icons.Material.Filled.Clear;

        /// <summary>
        /// Prevents scrolling while the dropdown is open.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool LockScroll { get; set; } = false;

        /// <summary>
        /// Occurs when the clear button is clicked.
        /// </summary>
        /// <remarks>
        /// Only occurs when <see cref="Clearable"/> is <c>true</c>.   This event occurs after the <c>Text</c> and <c>Value</c> have been cleared.
        /// </remarks>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// If <c>true</c>, navigating with arrow keys will only highlight items without updating the selected value.
        /// The selection must be confirmed by pressing Enter or clicking the item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool SelectionOnEnter { get; set; }

        internal bool _open;

        /// <summary>
        /// The current adornment icon to display.
        /// </summary>
        /// <remarks>
        /// If an <c>AdornmentIcon</c> is set, it is returned.  Otherwise, either <see cref="OpenIcon"/> or <see cref="CloseIcon"/> is returned depending on whether the drop-down is open.
        /// </remarks>
        internal string? _currentIcon { get; set; }

        /// <summary>
        /// Selects the item at the specified index.
        /// </summary>
        /// <param name="index">The ordinal of the item to select (starting at <c>0</c>).  When <see cref="MultiSelection"/> is <c>true</c>, the item will be added to the selected items.</param>
        public async Task SelectOption(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                if (!MultiSelection)
                    await CloseMenu();
                return;
            }
            await SelectOption(_items[index].Value);
        }

        /// <summary>
        /// Selects the item with the specified value.
        /// </summary>
        /// <param name="obj">The value to select.  When <see cref="MultiSelection"/> is <c>true</c>, the selection is cleared if it was already selected.</param>
        public async Task SelectOption(object? obj)
        {
            var value = (T?)obj;
            if (MultiSelection)
            {
                // multi-selection: menu stays open
                if (!_selectedValues.Add(value))
                    _selectedValues.Remove(value);

                if (MultiSelectionTextFunc != null)
                {
                    await SetCustomizedTextAsync(string.Join(Delimiter, _selectedValues.Select(ConvertSet!)),
                        selectedConvertedValues: _selectedValues.Select(ConvertSet!).ToList(),
                        multiSelectionTextFunc: MultiSelectionTextFunc);
                }
                else
                {
                    await SetTextAndUpdateValueAsync(string.Join(Delimiter, _selectedValues.Select(ConvertSet!)), updateValue: false);
                }

                UpdateSelectAllChecked();
                await BeginValidateAsync();
            }
            else
            {
                // single selection
                // Highlight the item BEFORE closing so the next open shows it highlighted
                await HighlightItemForValueAsync(value);

                // CloseMenu(true) doesn't close popover in BSS
                await CloseMenu(false);

                // Update internal selected values and ParameterState
                _selectedValues.Clear();
                _selectedValues.Add(value);

                // Early return if value hasn't changed (but after updating SelectedValues)
                // Use Comparer if available, otherwise use default
                var comparer = Comparer ?? EqualityComparer<T?>.Default;
                if (comparer.Equals(ReadValue, value))
                {
                    // Still need to publish SelectedValues to ParameterState in case it wasn't initialized
                    await _selectedValuesState.SetValueAsync(new HashSet<T?>(_selectedValues, Comparer));
                    StateHasChanged();
                    return;
                }

                await SetValueAndUpdateTextAsync(value);
                _elementReference.SetText(ReadText).CatchAndLog();
            }

            // For multi-selection, highlight after value is set
            if (MultiSelection)
            {
                await HighlightItemForValueAsync(value);
            }

            // Create a new HashSet to ensure ParameterState detects the change
            await _selectedValuesState.SetValueAsync(new HashSet<T?>(_selectedValues, Comparer));
            FieldChanged(_selectedValues);
            if (MultiSelection && typeof(T) == typeof(string))
                await SetValueAndUpdateTextAsync((T?)(object?)ReadText, updateText: false);
            await InvokeAsync(StateHasChanged);
        }

        private Task HighlightItemForValueAsync(T? value)
        {
            _context.TryGetItemByValue(value, out var item);
            return HighlightItemAsync(item);
        }

        private Task HighlightItemAsync(MudSelectItem<T>? item)
        {
            _activeItemId = item?.ItemId;
            return InvokeAsync(StateHasChanged);
        }

        private void UpdateSelectAllChecked()
        {
            if (MultiSelection && SelectAll)
            {
                if (_selectedValues.Count == 0)
                {
                    _selectAllChecked = false;
                }
                else if (_items.Count(x => !x.Disabled) == _selectedValues.Count)
                {
                    _selectAllChecked = true;
                }
                else
                {
                    _selectAllChecked = null;
                }
            }
        }

        internal Task HandleMouseDown(MouseEventArgs args)
        {
            if (args.Button != 0) // if it wasn't left click drop out
                return Task.CompletedTask;
            return ToggleMenu();
        }

        /// <summary>
        /// Opens or closes the drop-down menu.
        /// </summary>
        /// <remarks>
        /// Has no effect if <c>Disabled</c> or <c>ReadOnly</c> is <c>true</c>.
        /// </remarks>
        public async Task ToggleMenu()
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            if (_open)
                await CloseMenu(true);
            else
                await OpenMenu();
        }

        /// <summary>
        /// Opens the drop-down menu.
        /// </summary>
        /// <remarks>
        /// Has no effect if <c>Disabled</c> or <c>ReadOnly</c> is <c>true</c>.
        /// </remarks>
        public async Task OpenMenu()
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;

            _open = true;
            _needsHighlightAfterRender = true;
            UpdateIcon();
            StateHasChanged();

            //Scroll the active item on each opening
            if (_activeItemId != null)
            {
                var index = _items.FindIndex(x => x.ItemId == _activeItemId);
                if (index > 0)
                {
                    var item = _items[index];
                    await ScrollToItemAsync(item);
                }
            }
            //disable escape propagation: if selectmenu is open, only the select popover should close and underlying components should not handle escape key
            await KeyInterceptorService.UpdateKeyAsync(_elementId, new("Escape", stopDown: "key+none"));

            await OnOpen.InvokeAsync();
        }

        /// <summary>
        /// Closes the drop-down menu.
        /// </summary>
        /// <remarks>
        /// Has no effect if <c>Disabled</c> or <c>ReadOnly</c> is <c>true</c>.
        /// </remarks>
        public async Task CloseMenu(bool focusAgain = true)
        {
            _open = false;
            UpdateIcon();
            if (focusAgain)
            {
                StateHasChanged();
                await OnBlur.InvokeAsync(new FocusEventArgs());
                _elementReference.FocusAsync().CatchAndLog(ignoreExceptions: true);
                StateHasChanged();
            }

            //enable escape propagation: the select popover was closed, now underlying components are allowed to handle escape key
            await KeyInterceptorService.UpdateKeyAsync(_elementId, new("Escape", stopDown: "none"));

            await OnClose.InvokeAsync();
        }

        private void OnFitContentChanged(ParameterChangedEventArgs<bool> args)
        {
            if (args.Value)
            {
                var longestItemLength = 0;
                foreach (var item in GetAllShadowItems())
                {
                    var value = item.Value;
                    var valueToString = ConvertSet(value);
                    var length = valueToString?.Length ?? 0;

                    if (length > longestItemLength)
                    {
                        _longestItem = item;
                        longestItemLength = length;
                    }
                }
                StateHasChanged();
            }
            else
            {
                _longestItem = null;
            }
        }

        private void UpdateIcon()
        {
            _currentIcon = !string.IsNullOrWhiteSpace(AdornmentIcon) ? AdornmentIcon : _open ? CloseIcon : OpenIcon;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            UpdateIcon();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            UpdateIcon();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var options = new KeyInterceptorOptions(
                    "mud-input-control",
                    [
                        // prevent scrolling page, toggle open/close
                        new(" ", preventDown: "key+none"),
                        // prevent scrolling page, instead highlight previous item
                        new("ArrowUp", preventDown: "key+none"),
                        // prevent scrolling page, instead highlight next item
                        new("ArrowDown", preventDown: "key+none"),
                        new("Home", preventDown: "key+none"),
                        new("End", preventDown: "key+none"),
                        new("Escape"),
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

            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                // we need to render the initial Value which is not possible without the items
                // which supply the RenderFragment. So in this case, a second render is necessary
                StateHasChanged();
            }

            UpdateSelectAllChecked();

            // Highlight after items are fully rendered
            if (_needsHighlightAfterRender)
            {
                _needsHighlightAfterRender = false;
                await InvokeAsync(async () =>
                {
                    if (MultiSelection)
                    {
                        var firstNonDisabled = _items.FirstOrDefault(x => !x.Disabled);
                        await HighlightItemAsync(firstNonDisabled);
                    }
                    else
                    {
                        await HighlightItemForValueAsync(ReadValue);
                    }
                });
            }
        }

        /// <remarks>
        /// If <see cref="ToStringFunc"/> is set, it is used to convert the value to a string; otherwise, the base implementation is used.
        /// </remarks>
        /// <inheritdoc />
        protected override string? ConvertSet(T? input)
        {
            return ToStringFunc is not null
                ? ToStringFunc(input)
                : base.ConvertSet(input);
        }

        /// <summary>
        /// Internal method for MudSelectItem to access the converted string value.
        /// </summary>
        internal string? ConvertValueToString(T? value) => ConvertSet(value);

        /// <summary>
        /// Internal method for the context to access the current selected values.
        /// </summary>
        internal IEnumerable<T?>? GetSelectedValues() => _selectedValuesState.Value;

        /// <summary>
        /// Internal method for the context to access the current value.
        /// </summary>
        internal T? GetCurrentValue() => ReadValue;

        /// <summary>
        /// Gets all items including shadow items (items with HideContent=true).
        /// </summary>
        /// <remarks>
        /// This is used for operations that need access to all registered items,
        /// not just the visible ones in the dropdown.
        /// </remarks>
        private IEnumerable<MudSelectItem<T>> GetAllShadowItems()
        {
            return _context.ShadowItems;
        }

        /// <summary>
        /// Sets the focus to this component.
        /// </summary>
        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        /// <summary>
        /// Releases the focus from this component.
        /// </summary>
        public override ValueTask BlurAsync()
        {
            return _elementReference.BlurAsync();
        }

        /// <summary>
        /// Selects the text within this component.
        /// </summary>
        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        /// <summary>
        /// Selects a portion of text within this component.
        /// </summary>
        /// <param name="pos1">The index of the first character to select.  (Starting at <c>0</c>.)</param>
        /// <param name="pos2">The index of the last character to select.</param>
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        /// <summary>
        /// Occurs when the <c>Clear</c> button has been clicked.
        /// </summary>
        /// <remarks>
        /// This is the first event raised when the clear button is clicked.
        /// The <see cref="SelectedValues"/> are cleared and the <see cref="OnClearButtonClick"/> event is raised.
        /// </remarks>
        protected async ValueTask SelectClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await SetValueAndUpdateTextAsync(default, false);
            await SetTextAndUpdateValueAsync(default, false);
            _selectedValues.Clear();
            await BeginValidateAsync();
            StateHasChanged();
            await _selectedValuesState.SetValueAsync(new HashSet<T?>(_selectedValues, Comparer));
            FieldChanged(_selectedValues);
            await OnClearButtonClick.InvokeAsync(e);
        }

        protected async Task SetCustomizedTextAsync(string text, bool updateValue = true,
            List<string?>? selectedConvertedValues = null,
            Func<List<string?>?, string>? multiSelectionTextFunc = null)
        {
            // The Text property of the control is updated
            var customText = multiSelectionTextFunc?.Invoke(selectedConvertedValues);
            await SetTextCoreAsync(customText);

            // The comparison is made on the multiSelectionText variable
            if (_multiSelectionText != text)
            {
                _multiSelectionText = text;
                if (!string.IsNullOrWhiteSpace(_multiSelectionText))
                    Touched = true;
                if (updateValue)
                    await UpdateValuePropertyAsync(false);
            }
        }

        /// <summary>
        /// The icon used for selected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.CheckBox"/>.  Only applies when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// The icon used for unselected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.CheckBoxOutlineBlank"/>.  Only applies when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// The icon used when at least one, but not all, items are selected.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.IndeterminateCheckBox"/>.  Only applies when <see cref="MultiSelection"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        /// <summary>
        /// The icon to display whether all, none, or some items are selected.
        /// </summary>
        /// <remarks>
        /// Only applies when <see cref="MultiSelection"/> is <c>true</c>.
        /// If all items are selected, <see cref="CheckedIcon"/> is returned.
        /// If no items are selected, <see cref="UncheckedIcon"/> is returned.
        /// Otherwise, <see cref="IndeterminateIcon"/> is returned.
        /// </remarks>
        protected string SelectAllCheckBoxIcon
        {
            get => _selectAllChecked.HasValue ? _selectAllChecked.Value ? CheckedIcon : UncheckedIcon : IndeterminateIcon;
        }

        internal async Task HandleKeyDownAsync(KeyboardEventArgs obj)
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            var key = obj.Key.ToLowerInvariant();
            if (key.Length == 1 && key != " " && !(obj.CtrlKey || obj.ShiftKey || obj.AltKey || obj.MetaKey))
            {
                await SelectFirstItem(key);
                await FocusAsync();
                return;
            }
            switch (obj.Key)
            {
                case "Tab":
                    await CloseMenu(false);
                    break;
                case "ArrowUp":
                    if (obj.AltKey)
                    {
                        await CloseMenu();
                        break;
                    }

                    if (_open == false)
                    {
                        await OpenMenu();
                        break;
                    }

                    await SelectPreviousItem();
                    break;
                case "ArrowDown":
                    if (obj.AltKey)
                    {
                        await OpenMenu();
                        break;
                    }

                    if (_open == false)
                    {
                        await OpenMenu();
                        break;
                    }

                    await SelectNextItem();
                    break;
                case " ":
                    await ToggleMenu();
                    break;
                case "Escape":
                    await CloseMenu(true);
                    break;
                case "Home":
                    await SelectFirstItem();
                    break;
                case "End":
                    await SelectLastItem();
                    break;
                case "Enter":
                case "NumpadEnter":
                    var index = _items.FindIndex(x => x.ItemId == _activeItemId);
                    if (!MultiSelection)
                    {
                        if (!_open)
                        {
                            await OpenMenu();
                            break;
                        }

                        // this also closes the menu
                        await SelectOption(index);
                        break;
                    }

                    if (!_open)
                    {
                        await OpenMenu();
                        break;
                    }

                    await SelectOption(index);
                    await _elementReference.SetText(ReadText);
                    break;
                case "a":
                case "A":
                    if (obj.CtrlKey)
                    {
                        if (MultiSelection)
                        {
                            await SelectAllClickAsync();
                            StateHasChanged();
                        }
                    }
                    break;
            }

            await OnKeyDown.InvokeAsync(obj);
        }

        internal Task HandleKeyUpAsync(KeyboardEventArgs obj)
        {
            return OnKeyUp.InvokeAsync(obj);
        }

        /// <summary>
        /// Clears all selections and resets validation
        /// </summary>
        /// <remarks>
        /// To maintain validation errors (e.g. required), use <see cref="ClearAsync"/>
        /// </remarks>
        protected override async Task ResetValueAsync()
        {
            await ClearAsync();
            await base.ResetValueAsync();
        }

        /// <summary>
        /// Clears all selections.
        /// </summary>
        /// <remarks>
        /// To reset validation errors (e.g. required), use <see cref="ResetValueAsync"/>
        /// </remarks>
        public async Task ClearAsync()
        {
            await SetValueAndUpdateTextAsync(default, false);
            await SetTextAndUpdateValueAsync(default, false);
            _selectedValues.Clear();
            await BeginValidateAsync();
            StateHasChanged();
            await _selectedValuesState.SetValueAsync(new HashSet<T?>(_selectedValues, Comparer));
            FieldChanged(_selectedValues);
        }

        private async Task SelectAllClickAsync()
        {
            // Manage the fake tri-state of a checkbox
            if (!_selectAllChecked.HasValue)
                _selectAllChecked = true;
            else if (_selectAllChecked.Value)
                _selectAllChecked = false;
            else
                _selectAllChecked = true;
            // Define the items selection
            if (_selectAllChecked.Value)
                await SelectAllItems();
            else
                await ClearAsync();
        }

        private async Task SelectAllItems()
        {
            if (!MultiSelection)
                return;
            var selectedValues = new HashSet<T?>(_items.Where(x => !x.Disabled && x.Value != null).Select(x => x.Value), Comparer);
            _selectedValues = new HashSet<T?>(selectedValues, Comparer);
            if (MultiSelectionTextFunc != null)
            {
                await SetCustomizedTextAsync(string.Join(Delimiter, _selectedValues.Select(ConvertSet)),
                    selectedConvertedValues: _selectedValues.Select(ConvertSet).ToList(),
                    multiSelectionTextFunc: MultiSelectionTextFunc);
            }
            else
            {
                await SetTextAndUpdateValueAsync(string.Join(Delimiter, _selectedValues.Select(ConvertSet)), updateValue: false);
            }
            UpdateSelectAllChecked();
            _selectedValues = selectedValues; // need to force selected values because Blazor overwrites it under certain circumstances due to changes of Text or Value
            await BeginValidateAsync();
            await _selectedValuesState.SetValueAsync(new HashSet<T?>(_selectedValues, Comparer));
            FieldChanged(_selectedValues);
            if (MultiSelection && typeof(T) == typeof(string))
                SetValueAndUpdateTextAsync((T?)(object?)ReadText, updateText: false).CatchAndLog();
        }

        /// <summary>
        /// Links a selection item to this component.
        /// </summary>
        /// <remarks>
        /// This method now delegates to the context for registration.
        /// </remarks>
        /// <param name="item">The item to add.</param>
        public void RegisterShadowItem(MudSelectItem<T>? item)
        {
            if (item == null)
                return;

            _context.RegisterShadowItem(item);
        }

        /// <summary>
        /// Unregisters a selection item to this component.
        /// </summary>
        /// <remarks>
        /// This method now delegates to the context for unregistration.
        /// </remarks>
        /// <param name="item">The item to remove.</param>
        public void UnregisterShadowItem(MudSelectItem<T>? item)
        {
            if (item == null)
                return;
            _context.UnregisterShadowItem(item);
        }

        private async Task OnFocusOutAsync(FocusEventArgs focusEventArgs)
        {
            if (_open)
            {
                // when the menu is open we immediately get back the focus if we lose it (i.e. because of checkboxes in multi-select)
                // otherwise we can't receive key strokes any longer
                await FocusAsync();
            }
        }

        internal Task OnBlurAsync(FocusEventArgs obj)
        {
            return base.OnBlur.InvokeAsync(obj);
        }

        /// <inheritdoc />
        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            if (IsJSRuntimeAvailable)
            {
                await KeyInterceptorService.UnsubscribeAsync(_elementId);
            }
        }

        /// <summary>
        /// Gets whether the value is currently selected.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>When <c>true</c>, the specified value exists in <see cref="SelectedValues"/>.</returns>
        protected override bool HasValue(T? value)
        {
            // Fixes issue #4328

            if (MultiSelection)
                return _selectedValues?.Any() ?? false;
            return base.HasValue(value);
        }
    }
}
