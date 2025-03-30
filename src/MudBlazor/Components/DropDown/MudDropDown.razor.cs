// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Components.DropDown;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a base class for designing drop down components.
    /// </summary>
    /// <typeparam name="T">The type of item being input.</typeparam>
    public partial class MudDropDown<T> : MudFormComponent<T, string>
    {
        private readonly ParameterState<HashSet<T>> _selectedItemsState;
        private readonly ParameterState<bool> _openMenuState;
        private readonly ParameterState<bool> _isLoadingState;
        private readonly ParameterState<string?> _textState;
        private readonly ParameterState<string?> _inputIdState;

        private ElementReference _elementReference = default!;
        private int _elementKey = 0;
        private string? _userAttributesId = Identifier.Create("mudinput");
        private readonly string _componentId = Identifier.Create("mudinput");
        private bool _opening;

        public MudDropDown() : base(new DefaultConverter<T>())
        {
            using var registerScope = CreateRegisterScope();
            _selectedItemsState = registerScope.RegisterParameter<HashSet<T>>(nameof(SelectedItems))
                .WithParameter(() => SelectedItems)
                .WithEventCallback(() => SelectedItemsChanged);
            _openMenuState = registerScope.RegisterParameter<bool>(nameof(OpenMenu))
                .WithParameter(() => OpenMenu)
                .WithEventCallback(() => OpenMenuChanged)
                .WithChangeHandler(OnOpenMenuChanged);
            _isLoadingState = registerScope.RegisterParameter<bool>(nameof(IsLoading))
                .WithParameter(() => IsLoading)
                .WithEventCallback(() => IsLoadingChanged);
            _textState = registerScope.RegisterParameter<string?>(nameof(Text))
                .WithParameter(() => Text)
                .WithEventCallback(() => TextChanged)
                .WithChangeHandler(OnTextChangedHandler);
            _inputIdState = registerScope.RegisterParameter<string?>(nameof(InputId))
                .WithParameter(() => InputId)
                .WithChangeHandler(UpdateInputIdStateAsync);
        }

        [Inject]
        private InternalMudLocalizer Localizer { get; set; } = null!;

        protected string Classname => new CssBuilder()
            .AddClass("mud-combobox--with-progress", ShowProgressIndicator && _isLoadingState.Value)
            .AddClass("mud-autocomplete--with-progress", ShowProgressIndicator && _isLoadingState.Value)
            .AddClass(Class)
            .Build();

        protected string DropDownClassname =>
            new CssBuilder("mud-dropdown")
                .AddClass($"mud-dropdown-{Color.ToDescriptionString()}", Color != Color.Default)
                .AddClass("mud-width-full", FullWidth)
                .Build();

        protected string InputClassname => new CssBuilder(MudInputCssHelper.GetInputClassname(this))
            .AddClass(InputClass)
            .Build();

        protected string ClearButtonClassname =>
            new CssBuilder("mud-input-clear-button")
                .Build();

        protected string CircularProgressClassname =>
            new CssBuilder("progress-indicator-circular")
                .AddClass("progress-indicator-circular--with-adornment", Adornment == Adornment.End)
                .Build();

        protected string? InputElementId => _inputIdState.Value;

        protected bool GetDisabledState() => Disabled || ParentDisabled;

        protected bool GetReadOnlyState() => ReadOnly || ParentReadOnly;

        protected string GetDropDownIcon => _openMenuState.Value ? CloseIcon : OpenIcon;

        protected string? GetAriaDescribedByString()
        {
            var errorId = HasErrors ? ErrorId : null;
            var helperId = GetHelperId();

            return errorId is not null && helperId is not null
                ? $"{errorId} {helperId}"
                : errorId ?? helperId ?? null;
        }

        protected string? GetHelperId()
        {
            if (HelperId is not null)
            {
                return HelperId;
            }

            // error text replaces helper text in MudInputControl, so if the user does not provide a custom helper id, we have no valid helper element
            if (HasErrors)
            {
                return null;
            }

            return HelperText is not null
                ? $"{_inputIdState.Value}-helper-text"
                : null;
        }

        /// <summary>
        /// The regular expression used to validate the <see cref="Text"/> property.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. This property is used to validate the input against a regular expression.  Must be a valid JavaScript regular expression.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public virtual string? Pattern { get; set; }

        /// <summary>
        /// The text displayed in the input.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public string? Text { get; set; }

        /// <summary>
        /// This event is triggered when Text has changed.
        /// </summary>
        [Parameter]
        public EventCallback<string?> TextChanged { get; set; }

        /// <summary>
        /// The ID of the input element.
        /// </summary>
        /// <remarks>
        /// When set takes precedence over any internally generated IDs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? InputId { get; set; }

        /// <summary>
        /// The class or classes applied to the input element.
        /// </summary>
        [Parameter]
        public string? InputClass { get; set; }

        /// <summary>
        /// The class or classes applied to the <see cref="MudPopover" /> that contains the list of ComboBox items.
        /// </summary>
        [Parameter]
        public string? PopoverClass { get; set; }

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
        /// Uses a <see cref="MudOverlay"/> when the dropdown is open. 
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool Overlay { get; set; } = true;

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
        /// Displays the Clear icon button. Has no impact if Filterable is not <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, an icon is displayed which, when clicked, clears the filter Text.  Use the <c>ClearIcon</c> property to control the Clear button icon.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; }

        /// <summary>
        /// The icon to display when <see cref="Clearable"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Clear"/>.
        /// </remarks>
        [Parameter]
        public string ClearIcon { get; set; } = Icons.Material.Filled.Cancel;

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
        /// The Add Combobox item icon. When OnAddItemClick is defined this icon is shown when the Text property exceeds MinCharacters.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.AddCircle"/>.
        /// </remarks>
        [Parameter]
        public string AddIcon { get; set; } = Icons.Material.Filled.AddCircle;

        /// <summary>
        /// Allows the component to receive input.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }

        /// <summary>
        /// Prevents the input from being changed by the user.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the user can copy text in the control, but cannot change the <see cref="Text" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }

        [CascadingParameter(Name = "ParentReadOnly")]
        private bool ParentReadOnly { get; set; }

        /// <summary>
        /// Fills the full width of the parent container.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool FullWidth { get; set; }

        /// <summary>
        /// Displays an underline for the input.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Underline { get; set; } = true;

        /// <summary>
        /// The ID of the helper element, for use by <c>aria-describedby</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set it is appended to the <c>aria-describedby</c> attribute to improve accessibility for users. This ID takes precedence over the helper element rendered when <see cref="HelperText"/> is provided.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public string? HelperId { get; set; }

        /// <summary>
        /// The text displayed below the text field.
        /// </summary>
        /// <remarks>
        /// This property is typically used to help the user understand what kind of input is allowed.  The <see cref="HelperTextOnFocus"/> property controls when this text is visible.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
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

        /// <summary>
        /// Opens the list when focus is received on the input element; otherwise only opens on click.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c> so the list opens anytime it receives focus regardless of how.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool OpenOnFocus { get; set; } = true;

        /// <summary>
        /// The icon displayed for the adornment.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  This icon will be displayed when <see cref="Adornment"/> is <c>Start</c> or <c>End</c>, and no value for <see cref="AdornmentText"/> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? AdornmentIcon { get; set; }

        /// <summary>
        /// The text displayed for the adornment.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  This text will be displayed when <see cref="Adornment"/> is <c>Start</c> or <c>End</c>.  The <see cref="AdornmentIcon"/> property will be ignored if this property is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? AdornmentText { get; set; }

        /// <summary>
        /// The location of the adornment icon or text.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Adornment.End"/>.  When set to <c>Start</c> or <c>End</c>, the <see cref="AdornmentText"/> will be displayed, or <see cref="AdornmentIcon"/> if no adornment text is specified.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Adornment Adornment { get; set; } = Adornment.End;

        /// <summary>
        /// Limits validation to when the user changes the <see cref="Text"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, validation only occurs if the user has changed the input value at least once.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool OnlyValidateIfDirty { get; set; }

        /// <summary>
        /// The color of <see cref="AdornmentText"/> or <see cref="AdornmentIcon"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.  Theme colors are supported.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color AdornmentColor { get; set; } = Color.Default;

        /// <summary>
        /// The <c>aria-label</c> for the adornment.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string? AdornmentAriaLabel { get; set; }

        /// <summary>
        /// The label for this input.
        /// </summary>
        /// <remarks>
        /// If no <see cref="Text"/> is specified, the label will be displayed in the input.  Otherwise, it will be scaled down to the top of the input.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Label { get; set; }

        /// <summary>
        /// Controls the size of the icons for adornment, clear, and add buttons.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Small"/>. Larger Icon sizes will cause the Ripple effect to expand the size.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Size IconSize { get; set; } = Size.Small;

        /// <summary>
        /// Occurs when the adornment text or icon has been clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }

        /// <summary>
        /// Occurs when the add button is clicked
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAddItemClick { get; set; }

        /// <summary>
        /// The appearance variation to use.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/> in <see cref="MudGlobal.InputDefaults.Variant"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Variant Variant { get; set; } = MudGlobal.InputDefaults.Variant;

        /// <summary>
        /// The amount of vertical spacing for this input.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Margin.None"/> in <see cref="MudGlobal.InputDefaults.Margin"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Margin Margin { get; set; } = MudGlobal.InputDefaults.Margin;

        /// <summary>
        /// Typography for the input text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Typo Typo { get; set; } = Typo.subtitle1;

        /// <summary>
        /// The text displayed in the input if no <see cref="Text"/> is specified.
        /// </summary>
        /// <remarks>
        /// This property is typically used to give the user a hint as to what kind of input is expected.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Placeholder { get; set; }

        /// <summary>
        /// <para>When <c>false</c>, shows the label inside the input if no <see cref="Text"/> is specified.</para>
        /// <para>When <c>true</c>, the label will not move into the input when the input is empty.</para>
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c> in <see cref="MudGlobal.InputDefaults.ShrinkLabel"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool ShrinkLabel { get; set; } = MudGlobal.InputDefaults.ShrinkLabel;

        [Parameter]
        public bool OpenMenu { get; set; }

        [Parameter]
        public EventCallback<bool> OpenMenuChanged { get; set; }

        /// <summary>
        /// Whether the dropdown becomes filterable by text input. 
        /// or default <c>ToString()</c> method.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Filterable { get; set; }

        /// <summary>
        /// The template used to display selected items in the textbox area. When <c>Filterable</c> is <c>true</c> the template is shown under the input.
        /// </summary>
        [Parameter]
        public RenderFragment<DropDownItem<T>>? SelectedItemsTemplate { get; set; }

        /// <summary>
        /// The content in the Popover, can be anything. Add items of type <typeparamref name="T"/> to the context.<see cref="SelectedItems"/> and
        /// access public actions like context.<see cref="OpenMenuAsync"/> and context.<see cref="CloseMenuAsync"/> or context.<see cref="DropDownToggleItem(T?, bool)"/>
        /// </summary>
        [Parameter, EditorRequired]
        public RenderFragment<MudDropDown<T>> DropDownContent { get; set; } = default!;

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
        /// Defaults to <see cref="Color.Default"/>. 
        /// </remarks>
        [Parameter]
        public Color ProgressIndicatorColor { get; set; } = Color.Default;

        /// <summary>
        /// Set this when you want the Progress Indicator to show up.
        /// </summary>
        /// <remarks>
        /// Defautls to <c>false</c>. The progress indicator uses the color specified in the <see cref="ProgressIndicatorColor"/> property.
        /// </remarks>
        [Parameter]
        public bool IsLoading { get; set; }

        /// <summary>
        /// Event is Invoked when IsLoading is Changed
        /// </summary>
        [Parameter]
        public EventCallback<bool> IsLoadingChanged { get; set; }

        /// <summary>
        /// Whether a user can select multiple items
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool MultiSelection { get; set; }

        /// <summary>
        /// The maximum height, in pixels, of the Combobox Popover when it is open.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>300</c>.
        /// </remarks>
        [Parameter]
        public int MaxHeight { get; set; } = 300;

        /// <summary>
        /// The theming of the component
        /// </summary>
        [Parameter]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The minimum number of characters typed to initiate a search. 
        /// <para>The clear and add buttons use this as <c>MinCharacters + 1</c> to display.</para>
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.
        /// </remarks>
        [Parameter]
        public int MinCharacters { get; set; }

        /// <summary>
        /// The currently selected ComboBox items
        /// </summary>
        [Parameter]
        public HashSet<T> SelectedItems { get; set; } = [];

        public int SelectedItemsCount { get => SelectedItems.Count; }

        /// <summary>
        /// Event is fired when the selected items change
        /// </summary>
        [Parameter]
        public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnInputKeyDown { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnInputKeyUp { get; set; }

        private bool ShowClearButton => !GetDisabledState() && !GetReadOnlyState() && Clearable && Text?.Length > MinCharacters;

        private bool ShowAddButton => !GetDisabledState() && !GetReadOnlyState() && Text?.Length > MinCharacters && OnAddItemClick.HasDelegate;

        private bool ShouldLabelShrink =>
            SelectedItemsCount == 0 &&              // no SelectedItems to Display
            string.IsNullOrEmpty(Text) &&           // no text in the input
            Adornment != Adornment.Start &&         // no adornment set to Adornment.Start
            string.IsNullOrEmpty(Placeholder) &&    // no Placeholder Text
                                                    //!_isFocused &&                          // element isn't focused
            !_openMenuState.Value &&                // popover is closed
            !ShrinkLabel;                           // is allowed to shrink into input area

        /// <summary>
        /// Returns a value for the <c>autocomplete</c> html attribute, either supplied by default or the one specified in the attribute overrides.
        /// </summary>
        protected object? GetAutocomplete() => UserAttributes.GetValueOrDefault("autocomplete", "off");

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (string.IsNullOrEmpty(Label) && For != null)
            {
                Label = For.GetLabelString();
            }

            _userAttributesId = UserAttributes.FirstOrDefault(userAttribute => userAttribute.Key.Equals("id", StringComparison.InvariantCultureIgnoreCase)).Value?.ToString();

            if (InputId is null)
            {
                await UpdateInputIdStateAsync();
            }
        }

        // fires for every keystroke change
        protected Task OnInput(ChangeEventArgs? args)
        {
            return SetTextAsync(args?.Value as string);
        }

        private Task ClearButtonClickHandlerAsync()
        {
            return SetTextAsync(default);
        }

        internal async Task AddButtonClickHandlerAsync()
        {
            if (OnAddItemClick.HasDelegate)
            {
                await OnAdornmentClick.InvokeAsync();
            }
        }

        public async Task OpenMenuAsync()
        {
            if (GetReadOnlyState() || GetDisabledState())
                return;

            if (MinCharacters > 0 && (string.IsNullOrWhiteSpace(Text) || Text.Length < MinCharacters))
            {
                return;
            }

            _opening = true;
            await _elementReference.MudForceFocusAsync();
            // TODO: Perform Search Action

            // only set the value if it's not already set
            if (!_openMenuState.Value)
            {
                await _openMenuState.SetValueAsync(true);
            }

            _opening = false;
        }

        public async Task CloseMenuAsync()
        {
            if (_openMenuState.Value)
                await _openMenuState.SetValueAsync(false);
        }

        public async Task ToggleMenuAsync()
        {
            if (_openMenuState.Value)
            {
                await CloseMenuAsync();
            }
            else
            {
                await OpenMenuAsync();
            }
        }

        internal async Task AdornmentClickHandlerAsync()
        {
            if (OnAdornmentClick.HasDelegate)
            {
                await OnAdornmentClick.InvokeAsync();
            }
            else
            {
                await ToggleMenuAsync();
            }
        }

        private Task OnInputClickedAsync() => OnInputActivationAsync(true);

        private Task OnInputFocusedAsync() => OnInputActivationAsync(OpenOnFocus);

        private async Task OnInputActivationAsync(bool openMenu)
        {
            if (GetDisabledState() || GetReadOnlyState())
            {
                return;
            }

            if (openMenu && !_openMenuState.Value && !_opening)
            {
                await OpenMenuAsync();
            }
        }

        /// <summary>
        /// Toggles the item in the SelectedItems HashSet&lt;<typeparamref name="T"/>&gt;
        /// </summary>
        /// <param name="item">The item to be toggled, if exists it will be removed, otherwise it will be added</param>
        /// <param name="toggleMenu">Whether the menu should be toggled after the item is toggled</param>
        /// <returns></returns>
        public async Task DropDownToggleItem(T? item, bool toggleMenu = false)
        {
            if (item == null)
                return;

            // Toggle SelectedItems to Add if it doesn't exist, remove it if it does.
            // start by creating a new hashset list to ensure updates
            var selectedItems = new HashSet<T>(_selectedItemsState.Value ?? []);

            // if removing the item is false then add the item
            var toggled = selectedItems.Remove(item);
            if (!toggled)
            {
                // if it's single selection clear the list first
                if (!MultiSelection)
                {
                    selectedItems.Clear();
                }
                selectedItems.Add(item);
            }
            // clear text and update Selected Items
            await SetTextAsync(default);
            await _selectedItemsState.SetValueAsync(selectedItems);
            await BeginValidateAsync();
            // Toggle Menu if it's supposed to (they update StateHasChanged) if not call StateHasChanged manually
            if (toggleMenu)
            {
                await ToggleMenuAsync();
            }
            else
                StateHasChanged();
        }

        private async Task SetTextAsync(string? text)
        {
            if (_textState.Value != text)
            {
                await _textState.SetValueAsync(text);

                if (!string.IsNullOrEmpty(_textState.Value))
                {
                    Touched = true;
                }
            }
        }

        private async Task OnTextChangedHandler(ParameterChangedEventArgs<string?> args)
        {
            await Task.CompletedTask;
        }

        private async Task OnOpenMenuChanged(ParameterChangedEventArgs<bool> args)
        {
            if (!args.LastValue)
                await OpenMenuAsync();
            else
                await CloseMenuAsync();
        }

        private async Task UpdateInputIdStateAsync()
        {
            if (InputId is not null)
            {
                return;
            }

            if (_userAttributesId is not null)
            {
                await _inputIdState.SetValueAsync(_userAttributesId);
                return;
            }

            await _inputIdState.SetValueAsync(_componentId);
        }
    }
}
