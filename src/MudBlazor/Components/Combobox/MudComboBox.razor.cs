using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;
#nullable enable
namespace MudBlazor
{
    public partial class MudComboBox<T> : MudComponentBase
    {
        private bool _searchDisabled;
        private int _selectedComboBoxIndex = -1;
        private bool _openItemList;

        private ParameterState<string?> _comboBoxValueState;
        private ParameterState<T?> _selectedItemState;
        private ParameterState<HashSet<T>> _selectedItemsState;

        private MudTextField<string>? _searchField;

        public MudComboBox()
        {
            SelectedItems = new HashSet<T>();
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
        }

        protected string Classname => new CssBuilder("mud-combobox")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The class or classes applied to the <see cref="MudPopover" />
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
        /// Uses compact padding.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Dense { get; set; }

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
        /// The maximum height, in pixels, of the Combobox Popover when it is open.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>300</c>.
        /// </remarks>
        [Parameter]
        public int MaxHeight { get; set; } = 300;

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
        /// Defaults to <c>10</c>.  A value of <c>null</c> will display all items.
        /// </remarks>
        [Parameter]
        public int? MaxItems { get; set; } = 10;

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
        /// Defaults to <c>100</c>.  A higher value can help reduce the number of calls to <see cref="SearchFunc"/>, which can improve responsiveness.
        /// </remarks>
        [Parameter]
        public int DebounceInterval { get; set; } = 100;

        /// <summary>
        /// The custom template used to display items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<T>? ItemTemplate { get; set; }

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
        /// The behavior of the dropdown popover menu
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DropdownSettings.Fixed" /> false
        /// Defaults to <see cref="DropdownSettings.OverflowBehavior" /> <see cref="OverflowBehavior.FlipOnOpen" />
        /// </remarks>
        [Category(CategoryTypes.Popover.Behavior)]
        [Parameter]
        public DropdownSettings DropdownSettings { get; set; } = new DropdownSettings();

        /// <summary>
        /// Whether or not the Popover generated uses an overlay.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool Overlay { get; set; } = true;

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
        /// Whether a user can select multiple items
        /// </summary>
        [Parameter]
        public bool MultiSelection { get; set; }

        /// <summary>
        /// The currently selected ComboBox items
        /// </summary>
        [Parameter]
        public HashSet<T> SelectedItems { get; set; }

        /// <summary>
        /// Event is fired when the selected items change
        /// </summary>
        [Parameter]
        public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        /// <summary>
        /// Sets the filter type, Client filters based on AutoCompleteItems, Server expects user to update AutoCompleteItems.
        /// Default is Client
        /// </summary>
        [Parameter]
        public ComboBoxFilterType FilterType { get; set; } = ComboBoxFilterType.Client;

        /// <summary>
        /// What is displayed when there are no AutoCompleteItems
        /// </summary>
        [Parameter]
        public RenderFragment? NoRecords { get; set; }

        [Parameter]
        public string? PlaceHolder { get; set; }

        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public bool OpenOnEnter { get; set; }

        [Parameter]
        public string? HelperText { get; set; }

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
                _openItemList = false;
                if (_searchField != null)
                {
                    await _searchField.ResetAsync();
                    await _searchField.FocusAsync();
                }
                return;
            }
            _openItemList = true;
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

        private async Task ComboBoxSelectItem(T item)
        {
            await _comboBoxValueState.SetValueAsync(item?.ToString() ?? FilteredItems[0]?.ToString());
            _openItemList = false;
            SelectedItem = item;
        }

        private void FocusOnEnter()
        {
            if (OpenOnEnter)
                _openItemList = true;
        }

        private async Task ComboBoxValueClear()
        {
            await _comboBoxValueState.SetValueAsync(default);
            _openItemList = false;
            FilteredItems = Items;
        }

        private async Task ComboBoxValueUpdated(string? value)
        {
            await _comboBoxValueState.SetValueAsync(value);
            if (_comboBoxValueState.Value?.Length > 0)
            {
                _openItemList = true;
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
