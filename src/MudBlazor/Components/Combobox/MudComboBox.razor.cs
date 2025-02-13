using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;
#nullable enable
namespace MudBlazor
{
    public partial class MudComboBox<T> : MudComponentBase
    {
        private int _selectedComboBoxIndex = -1;

        private ParameterState<string?> _comboBoxValueState;
        private ParameterState<T?> _selectedItemState;
        private ParameterState<HashSet<T>> _selectedItemsState;
        private ParameterState<bool> _openItemListState;

        private MudTextField<string>? _searchField;

        public MudComboBox()
        {
            using var registerScope = CreateRegisterScope();
            _comboBoxValueState = registerScope.RegisterParameter<string?>(nameof(ComboBoxValue))
                .WithParameter(() => ComboBoxValue)
                .WithEventCallback(() => ComboBoxValueChanged);
            _selectedItemState = registerScope.RegisterParameter<T?>(nameof(SelectedItem))
                .WithParameter(() => SelectedItem)
                .WithEventCallback(() => SelectedItemChanged);
            _selectedItemsState = registerScope.RegisterParameter<HashSet<T>>(nameof(SelectedItems))
                .WithParameter(() => SelectedItems)
                .WithEventCallback(() => SelectedItemsChanged);
            _openItemListState = registerScope.RegisterParameter<bool>(nameof(OpenItemList))
                .WithParameter(() => OpenItemList)
                .WithEventCallback(() => OpenItemListChanged);
        }

        protected string Classname => new CssBuilder("mud-combobox")
            .AddClass(Class)
            .Build();

        #region Confirmed Parameters

        /// <summary>
        /// The class or classes applied to the <see cref="MudPopover" /> that contains the list of ComboBox items.
        /// </summary>
        [Parameter]
        public string? PopoverClass { get; set; }

        /// <summary>
        /// The display variant for this input.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        [Parameter]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// The location where the popover will open from.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.BottomLeft" />.
        /// </remarks>
        [Parameter]
        public Origin AnchorOrigin { get; set; } = Origin.BottomLeft;

        /// <summary>
        /// The transform origin point for the popover.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.TopLeft"/>.
        /// </remarks>
        [Parameter]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// Uses compact padding, including the search items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c> or <c>MudGlobal.InputDefaults.Margin</c>.
        /// </remarks>
        [Parameter]
        public bool Dense { get; set; }

        /// <summary>
        /// Whether the ComboBox text field can be used to filter the available items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>
        /// </remarks>
        [Parameter]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// The maximum height, in pixels, of the Combobox Popover when it is open.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>300</c>.
        /// </remarks>
        [Parameter]
        public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// When disabled interactivity of the ComboBox is disabled and appropriate effect is applied.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// Changes the <see cref="ComboBoxValue"/> as soon as input is received.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>true</c>, the <see cref="ComboBoxValue"/> property will be updated any time user input occurs.
        /// If <c>false</c>, <see cref="ComboBoxValue"/> is updated when the user presses <c>Enter</c> or the input loses focus.
        /// </remarks>
        public bool Immediate { get; set; } = true;

        /// <summary>
        /// Any template you wish to place Before the Items list.
        /// </summary>
        [Parameter]
        public RenderFragment? BeforeItemsTemplate { get; set; }

        /// <summary>
        /// What is displayed when there are no AutoCompleteItems. 
        /// </summary>
        [Parameter]
        public RenderFragment? NoRecords { get; set; }

        /// <summary>
        /// Determines the width of the ComboBox dropdown in relation to the parent container.
        /// </summary>
        /// <remarks>
        /// <para>Defaults to <see cref="DropdownWidth.Relative" />. </para>
        /// <para>When SmallScreens is set DropdownWidth is overridden to <see cref="DropdownWidth.Ignore" /></para>.
        /// <para>When <see cref="DropdownWidth.Relative" />, restricts the max-width of the component to the width of the parent container</para>
        /// <para>When <see cref="DropdownWidth.Adaptive" />, restricts the min-width of the component to the width of the parent container</para>
        /// <para>When <see cref="DropdownWidth.Ignore" />, there are no width restrictions of the component to the width of the parent container</para>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public DropdownWidth RelativeWidth { get; set; } = DropdownWidth.Relative;

        /// <summary>
        /// The behavior of the ComboBox dropdown. 
        /// <para>OverflowBehavior when it cannot display in full at the original Anchor and Transform positions.</para>
        /// <para>Fixed true displays the dropdown popover in a fixed position, even while scrolling.</para>
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DropdownSettings.Fixed" /> false
        /// Defaults to <see cref="DropdownSettings.OverflowBehavior" /> <see cref="OverflowBehavior.FlipOnOpen" />
        /// </remarks>
        [Category(CategoryTypes.Popover.Behavior)]
        [Parameter]
        public DropdownSettings DropdownSettings { get; set; } = new DropdownSettings();

        /// <summary>
        /// Whether or not the ComboBox uses an overlay when the dropdown is active.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool Overlay { get; set; } = true;

        /// <summary>
        /// The text displayed in the input if no <see cref="ComboBoxValue"/> is specified/selected.
        /// </summary>
        /// <remarks>
        /// This property is typically used to give the user a hint as to what kind of input is expected.
        /// </remarks>
        [Parameter]
        public string? PlaceHolder { get; set; }

        /// <summary>
        /// The label for this input.
        /// </summary>
        /// <remarks>
        /// If no <see cref="ComboBoxValue"/> is specified, the label will be displayed in the input. Otherwise, it will be scaled down to the top of the input.
        /// </remarks>
        [Parameter]
        public string? Label { get; set; }

        /// <summary>
        /// Shows the label inside the input if no <see cref="ComboBoxValue"/> is specified.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c> in <see cref="MudGlobal.InputDefaults.ShrinkLabel"/>.
        /// When <c>true</c>, the label will not move into the input when the input is empty.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool ShrinkLabel { get; set; } = MudGlobal.InputDefaults.ShrinkLabel;

        /// <summary>
        /// The text displayed below the text field.
        /// </summary>
        /// <remarks>
        /// This property is typically used to help the user understand what kind of input is allowed.  The <see cref="HelperTextOnFocus"/> property controls when this text is visible.
        /// </remarks>
        [Parameter]
        public string? HelperText { get; set; }

        /// <summary>
        /// Displays the <see cref="HelperText"/> only when this input has focus.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool HelperTextOnFocus { get; set; }

        #endregion

        /// <summary>
        /// The "open" Combobox icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowDropDown"/>.
        /// </remarks>
        [Parameter]
        public string OpenIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// The "close" Combobox icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowDropDown"/>.
        /// </remarks>
        [Parameter]
        public string CloseIcon { get; set; } = Icons.Material.Filled.ArrowDropUp;

        /// <summary>
        /// The "add" Combobox icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.AddCircle"/>.
        /// </remarks>
        [Parameter]
        public string AddIcon { get; set; } = Icons.Material.Filled.AddCircle;

        /// <summary>
        /// When <c>true</c> an AddIcon is displayed when custom input does not have an exact match. 
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>
        /// </remarks>
        [Parameter]
        public bool CustomInput { get; set; }

        /// <summary>
        /// Sets the point at which the list becomes a BottomSheet encompassing the entire bottom (or top) of the presumed mobile display.
        /// <para>--TODO--</para>
        /// </summary>
        [Parameter]
        public Breakpoint? SmallScreens { get; set; } = Breakpoint.SmAndDown;

        /// <summary>
        /// The function used to get the display text for each item.
        /// </summary>
        /// <remarks>
        /// Defaults to the <c>ToString()</c> method of items.
        /// </remarks>
        [Parameter]
        public Func<T?, string?>? ToStringFunc { get; set; }

        /// <summary>
        /// Shows the progress indicator during searches.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  The progress indicator uses the color specified in the <see cref="ProgressIndicatorColor"/> property.
        /// </remarks>
        [Parameter]
        public bool ShowProgressIndicator { get; set; }

        /// <summary>
        /// The color of the progress indicator.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.  This property is used when <see cref="ShowProgressIndicator"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        public Color ProgressIndicatorColor { get; set; } = Color.Default;

        /// <summary>
        /// The function used to search for items.
        /// </summary>
        /// <remarks>
        /// This function searches for items containing the specified <c>string</c> value, and returns items which match up to the <see cref="MaxItems"/> property.  You can use the provided <see cref="CancellationToken"/> which is marked as canceled when the user changes the search text or selects a value from the list.
        /// </remarks>
        [Parameter]
        public Func<string?, CancellationToken, Task<IEnumerable<T>>?>? SearchFunc { get; set; }

        /// <summary>
        /// The maximum number of items to display.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>10</c>. A value of 0 will display all items.
        /// </remarks>
        [Parameter]
        public int MaxItems { get; set; } = 10;

        /// <summary>
        /// The minimum number of characters typed to initiate a search.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.
        /// </remarks>
        [Parameter]
        public int MinCharacters { get; set; } = 0;

        /// <summary>
        /// Reset the selected value if the user deletes the text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool ResetValueOnEmptyText { get; set; }

        /// <summary>
        /// Highlights the text when the component receives focus.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// Previously known as <c>SelectOnClick</c>.
        /// </remarks>
        [Parameter]
        public bool SelectOnActivation { get; set; } = true;

        /// <summary>
        /// The debounce interval, in milliseconds.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>100</c>. A higher value can help reduce the number of calls to <see cref="SearchFunc"/>, which can improve responsiveness.
        /// </remarks>
        [Parameter]
        public int DebounceInterval { get; set; } = 100;

        /// <summary>
        /// The custom template used to display items. Has access to <c>context</c> and <c>context.Item</c>
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<T>? ItemTemplate { get; set; }

        /// <summary>
        /// Overrides the <c>Text</c> property when an item is selected.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>true</c>, selecting a value will update the Text property.  When <c>false</c>, incomplete values for Text are allowed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool CoerceText { get; set; } = true;

        /// <summary>
        /// Sets the <c>Value</c> property even if no match is found by <see cref="SearchFunc"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the user input will be applied to the Value property which allows it to be validated and show an error message.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool CoerceValue { get; set; }

        /// <summary>
        /// Whether a user can select multiple items
        /// </summary>
        [Parameter]
        public SelectionMode MultiSelection { get; set; }

        /// <summary>
        /// The currently selected ComboBox item
        /// </summary>
        [Parameter]
        public T? SelectedItem { get; set; }

        /// <summary>
        /// Event is fired when the selected item changes
        /// </summary>
        [Parameter]
        public EventCallback<T?> SelectedItemChanged { get; set; }

        /// <summary>
        /// The currently selected ComboBox items
        /// </summary>
        [Parameter]
        public HashSet<T> SelectedItems { get; set; } = [];

        /// <summary>
        /// Event is fired when the selected items change
        /// </summary>
        [Parameter]
        public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        /// <summary>
        /// Sets the filter type, Client filters based on ComboBox Items, Server expects user to update ComboBoxItems.
        /// Default is Client
        /// </summary>
        [Parameter]
        public ComboBoxFilterType FilterType { get; set; } = ComboBoxFilterType.Client;

        [Parameter]
        public bool OpenOnEnter { get; set; }

        [Parameter]
        public bool OpenItemList { get; set; }

        [Parameter]
        public EventCallback<bool> OpenItemListChanged { get; set; }

        [Parameter]
        public string? ComboBoxValue { get; set; }

        [Parameter]
        public EventCallback<string?> ComboBoxValueChanged { get; set; }

        /// <summary>
        /// The list of items filtered
        /// </summary>
        public List<T> FilteredItems { get; private set; } = [];

        /// <summary>
        /// The number of items currently filtered
        /// </summary>
        public int FilteredItemsCount { get => FilteredItems.Count; }

        /// <summary>
        /// The list of items to display in the ComboBox, the ToString method is used for display
        /// </summary>
        [Parameter]
        public List<T> Items { get; set; } = [];

        /// <summary>
        /// The number of items
        /// </summary>
        public int ItemsCount { get => Items.Count; }

        protected override async void OnParametersSet()
        {
            if (FilterType == ComboBoxFilterType.Client)
            {
                FilteredItems = Items
                    .Where(x => x?.ToString()?.Contains(_comboBoxValueState.Value ?? string.Empty, StringComparison.CurrentCultureIgnoreCase) ?? false).ToList();
            }
            else if (FilterType == ComboBoxFilterType.Server)
            {
                FilteredItems = Items;
            }
            await InvokeAsync(StateHasChanged);
        }

        // Allow "enter" to search
        private async Task KeyDown(KeyboardEventArgs eventArgs)
        {
            if (eventArgs.Key.Equals("Esc"))
            {
                await _openItemListState.SetValueAsync(false);
                if (_searchField != null)
                {
                    await _searchField.ResetAsync();
                    await _searchField.FocusAsync();
                }
                return;
            }
            await _openItemListState.SetValueAsync(false);
            if (eventArgs.Key.Equals("Enter"))
            {
                if (FilteredItems.Count > 0)
                {
                    if (_selectedComboBoxIndex > -1 &&
                        FilteredItems.Count > _selectedComboBoxIndex)
                    {
                        var item = FilteredItems[_selectedComboBoxIndex];
                        await _comboBoxValueState.SetValueAsync(item?.ToString() ?? FilteredItems[0]?.ToString());
                    }
                }
                if (_searchField != null)
                {
                    await _searchField.ResetAsync();
                    await _searchField.FocusAsync();
                }
                return;
            }
            // switch on key up, down arrows
            if (FilteredItems.Count == 0)
            {
                return;
            }
            if (eventArgs.Key.Equals("ArrowDown"))
            {
                _selectedComboBoxIndex = Math.Min(_selectedComboBoxIndex + 1, FilteredItems.Count - 1);
                SelectedItem = FilteredItems[_selectedComboBoxIndex];
            }
            else if (eventArgs.Key.Equals("ArrowUp") && FilteredItems.Count > 0)
            {
                _selectedComboBoxIndex = Math.Max(_selectedComboBoxIndex - 1, 0);
                SelectedItem = FilteredItems[_selectedComboBoxIndex];
            }
        }

        public async Task ComboBoxToggleItem(T item)
        {
            if (item == null)
                return;

            // Toggle SelectedItem 
            var selectedItem = _selectedItemState.Value;
            if (item.Equals(selectedItem))
            {
                await _selectedItemState.SetValueAsync(default);
            }
            else
            {
                await _selectedItemState.SetValueAsync(item);
            }

            // Toggle SelectedItems to Add if it doesn't exist, remove it if it does.
            var selectedItems = _selectedItemsState.Value ?? [];
            if (!selectedItems.Remove(item))
            {
                selectedItems.Add(item);
            }
        }

        private async Task FocusOnEnterAsync()
        {
            if (OpenOnEnter)
                await _openItemListState.SetValueAsync(true);
        }

        private async Task BlurredAsync()
        {
            await _openItemListState.SetValueAsync(false);
        }

        private async Task ComboBoxValueClear()
        {
            await _comboBoxValueState.SetValueAsync(default);
            await _openItemListState.SetValueAsync(false);
            FilteredItems = Items;
        }

        private async Task ComboBoxValueUpdated(string? value)
        {
            await _comboBoxValueState.SetValueAsync(value);
            if (_comboBoxValueState.Value?.Length > 0)
            {
                await _openItemListState.SetValueAsync(true);
            }
            _selectedComboBoxIndex = -1;
            if (FilterType == ComboBoxFilterType.Client)
            {
                FilteredItems = Items.Where(x => x?.ToString()?.StartsWith(_comboBoxValueState.Value!.ToLower(), StringComparison.CurrentCultureIgnoreCase) ?? false).ToList();
            }
            else if (FilterType == ComboBoxFilterType.Server)
            {
                FilteredItems = Items;
            }
            SelectedItem = FilteredItems.FirstOrDefault();
            _selectedComboBoxIndex = SelectedItem != null ? FilteredItems.IndexOf(SelectedItem) : -1;
            await InvokeAsync(StateHasChanged);
        }

    }
}
