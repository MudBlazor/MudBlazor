// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;
#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// Represents a base class for designing drop down components.
    /// </summary>
    /// <typeparam name="T">The type of item being input.</typeparam>
    public partial class MudDropDown<T> : MudFormComponent<T, string>
    {
        private ParameterState<HashSet<T>> _selectedItemsState;
        private ParameterState<bool> _openItemListState;
        private ParameterState<bool> _isLoadingState;
        private ParameterState<string> _textState;

        public MudDropDown() : base(new DefaultConverter<T>())
        {
            // default values, can be overridden
            Adornment = Adornment.End;
            IconSize = Size.Medium;

            using var registerScope = CreateRegisterScope();
            _selectedItemsState = registerScope.RegisterParameter<HashSet<T>>(nameof(SelectedItems))
                .WithParameter(() => SelectedItems)
                .WithEventCallback(() => SelectedItemsChanged);
            _openItemListState = registerScope.RegisterParameter<bool>(nameof(OpenItemList))
                .WithParameter(() => OpenItemList)
                .WithEventCallback(() => OpenItemListChanged);
            _isLoadingState = registerScope.RegisterParameter<bool>(nameof(IsLoading))
                .WithParameter(() => IsLoading)
                .WithEventCallback(() => IsLoadingChanged);
            _textState = registerScope.RegisterParameter<string>(nameof(Text))
                .WithParameter(() => Text)
                .WithEventCallback(() => TextChanged)
                .WithChangeHandler(OnTextChanged);
        }

        protected string Classname => new CssBuilder()
            .AddClass($"mud-theme-{Color.ToDescriptionString()}")
            .AddClass("mud-combobox--with-progress", IsLoading)
            .AddClass("mud-autocomplete--with-progress", IsLoading)
            .AddClass(Class)
            .Build();

        protected string DropDownClassname =>
            new CssBuilder("mud-dropdown")
                .AddClass("mud-width-full", FullWidth)
                .Build();

        protected bool GetDisabledState() => Disabled || ParentDisabled;

        protected bool GetReadOnlyState() => ReadOnly || ParentReadOnly;

        protected string GetDropDownIcon => _openItemListState.Value ? CloseIcon : OpenIcon;

        /// <summary>
        /// The text displayed in the input.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// This event is triggered when Text has changed.
        /// </summary>
        [Parameter]
        public EventCallback<string> TextChanged { get; set; }

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
        /// Defaults to <see cref="Adornment.None"/>.  When set to <c>Start</c> or <c>End</c>, the <see cref="AdornmentText"/> will be displayed, or <see cref="AdornmentIcon"/> if no adornment text is specified.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Adornment Adornment { get; set; } = Adornment.None;

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
        /// The size of the icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// Occurs when the adornment text or icon has been clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }

        /// <summary>
        /// Occurs when the add button is clicked
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAddButtonClick { get; set; }

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
        public bool OpenItemList { get; set; }

        [Parameter]
        public EventCallback<bool> OpenItemListChanged { get; set; }

        /// <summary>
        /// The content in the Popover, can be anything. Add items of type <typeparamref name="T"/> to the context.<see cref="SelectedItems"/>
        /// </summary>
        [Parameter, EditorRequired]
        public RenderFragment<MudDropDown<T>> DropDownContent { get; set; } = default!;

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
        /// The function used to determine if an item should be disabled.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public Func<T, bool>? ItemDisabledFunc { get; set; }

        /// <summary>
        /// Whether a user can select multiple items
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool MultiSelection { get; set; }

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



        private bool ShouldLabelShrink =>
            SelectedItemsCount == 0 &&              // no SelectedItems to Display
            string.IsNullOrEmpty(Text) &&           // no text in the input
            Adornment != Adornment.Start &&         // no adornment set to Adornment.Start
            string.IsNullOrEmpty(Placeholder) &&    // no Placeholder Text
                                                    //!_isFocused &&                          // element isn't focused
            !_openItemListState.Value &&            // popover is closed
            !ShrinkLabel;                           // is allowed to shrink into input area

        private Task ClearButtonClickHandlerAsync()
        {
            return Task.CompletedTask;
            //return SetTextAsync(default, false);
        }

        internal async Task AdornmentClickHandlerAsync()
        {
            if (OnAdornmentClick.HasDelegate)
            {

                await OnAdornmentClick.InvokeAsync();
            }
            else
            {
                await ToggleOpenAsync();
            }
        }

        internal async Task AddButtonClickHandlerAsync()
        {
            if (OnAddButtonClick.HasDelegate)
            {
                await OnAdornmentClick.InvokeAsync();
            }
        }

        // do not make public, access to two way bind activates accordingly
        private async Task ToggleOpenAsync()
        {
            if (_openItemListState.Value)
            {
                await _openItemListState.SetValueAsync(false);
            }
            else
            {
                await _openItemListState.SetValueAsync(true);
            }
        }

        private async Task OnInputClickedAsync()
        {
            // TODO: See latest AutoComplete fix by DC
            // this fires at nearly the same time as OnInputFocusedAsync, so we need to delay when both fire together
            // to prevent running the search method twice
            //await Task.Delay(5);
            //if (_activatorEvents)
            //{
            //    _activatorEvents = false;
            //    return;
            //}
            //await InputActivationAsync(true);
            await Task.CompletedTask;
        }

        private async Task OnInputFocusedAsync()
        {
            //if (OpenOnFocus)
            //{
            //    _activatorEvents = true;
            //}
            //await InputActivationAsync(OpenOnFocus);
            await Task.CompletedTask;
        }

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
                await ToggleOpenAsync();
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

        private async Task OnTextChangedHandler(ParameterChangedEventArgs<string> args)
        {
            await Task.CompletedTask;
        }
    }
}
