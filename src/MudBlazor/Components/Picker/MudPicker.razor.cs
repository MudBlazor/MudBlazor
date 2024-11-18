// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// A component for selecting date, time, and color values.
    /// </summary>
    /// <typeparam name="T">The type of value being chosen.</typeparam>
    /// <seealso cref="MudPickerContent" />
    /// <seealso cref="MudPickerToolbar" />
    public partial class MudPicker<T> : MudFormComponent<T, string>
    {
        private string? _text;
        private bool _pickerSquare;
        private ElementReference _pickerInlineRef;
        private bool _keyInterceptorObserving = false;
        private string _elementId = Identifier.Create("picker");

        public MudPicker() : base(new Converter<T, string>()) { }

        protected MudPicker(Converter<T, string> converter) : base(converter) { }

        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        protected string PickerClassname =>
            new CssBuilder("mud-picker")
                .AddClass("mud-picker-inline", PickerVariant != PickerVariant.Static)
                .AddClass("mud-picker-static", PickerVariant == PickerVariant.Static)
                .AddClass("mud-rounded", PickerVariant == PickerVariant.Static && !_pickerSquare)
                .AddClass($"mud-elevation-{Elevation ?? 0}", PickerVariant != PickerVariant.Inline)
                .AddClass("mud-picker-input-button", !Editable && PickerVariant != PickerVariant.Static)
                .AddClass("mud-picker-input-text", Editable && PickerVariant != PickerVariant.Static)
                .AddClass("mud-disabled", GetDisabledState() && PickerVariant != PickerVariant.Static)
                .AddClass(Class)
                .Build();

        protected string PickerPaperClassname =>
            new CssBuilder("mud-picker")
                .AddClass("mud-picker-paper")
                .AddClass("mud-picker-view", PickerVariant == PickerVariant.Inline)
                .AddClass("mud-picker-open", Open && PickerVariant == PickerVariant.Inline)
                .AddClass("mud-picker-popover-paper", PickerVariant == PickerVariant.Inline)
                .AddClass("mud-dialog", PickerVariant == PickerVariant.Dialog)
                .Build();

        protected string PickerPaperStylename =>
            new StyleBuilder()
                .AddStyle("transition-duration", $"{Math.Round(MudGlobal.TransitionDefaults.Duration.TotalMilliseconds)}ms")
                .AddStyle("transition-delay", $"{Math.Round(MudGlobal.TransitionDefaults.Delay.TotalMilliseconds)}ms")
                .AddStyle(Style)
                .Build();

        protected string PickerInlineClassname =>
            new CssBuilder("mud-picker-inline-paper")
                .Build();

        protected string PickerContainerClassname =>
            new CssBuilder("mud-picker-container")
                .AddClass("mud-paper-square", _pickerSquare)
                .AddClass("mud-picker-container-landscape",
                    Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
                .Build();

        protected string PickerInputClassname =>
            new CssBuilder("mud-input-input-control")
                .AddClass(Class)
                .Build();

        protected string PopoverClassname =>
            new CssBuilder("mud-picker-popover")
                // We can't use the Elevation parameter because it requires Paper=true; Instead we define the class explicitly.
                .AddClass($"mud-elevation-{Elevation ?? MudGlobal.PopoverDefaults.Elevation}")
                .Build();

        protected string ActionsClassname =>
            new CssBuilder("mud-picker-actions")
                .AddClass(ActionsClass)
                .Build();

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }

        [CascadingParameter(Name = "ParentReadOnly")]
        private bool ParentReadOnly { get; set; }

        /// <summary>
        /// The color of the <see cref="AdornmentIcon"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color AdornmentColor { get; set; } = Color.Default;

        /// <summary>
        /// The icon shown next to the text input.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Event"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string AdornmentIcon { get; set; } = Icons.Material.Filled.Event;

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
        /// The text displayed in the input if no value is specified.
        /// </summary>
        /// <remarks>
        /// This property is typically used to give the user a hint as to what kind of input is expected.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Placeholder { get; set; }

        /// <summary>
        /// Occurs when this picker has opened.
        /// </summary>
        [Parameter]
        public EventCallback PickerOpened { get; set; }

        /// <summary>
        /// Occurs when this picker has closed.
        /// </summary>
        [Parameter]
        public EventCallback PickerClosed { get; set; }

        /// <summary>
        /// The size of the drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>8</c> for inline pickers; otherwise <c>0</c>.<br />
        /// A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public int? Elevation { set; get; }

        /// <summary>
        /// Disables rounded corners.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public bool Square { get; set; }

        /// <summary>
        /// Shows rounded corners.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.<br />
        /// When <c>true</c>, the <c>border-radius</c> style is set to the theme's default value.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public bool Rounded { get; set; }

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
        /// The label for this input.
        /// </summary>
        /// <remarks>
        /// If no value is specified, the label will be displayed in the input.  Otherwise, it will be scaled down to the top of the input.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Label { get; set; }

        /// <summary>
        /// Displays the Clear icon button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.<br />
        /// When <c>true</c>, an icon is displayed which, when clicked, clears the Text and Value.  Use the <c>ClearIcon</c> property to control the Clear button icon.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// Prevents the user from interacting with this button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Shows an underline under the input text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Underline { get; set; } = true;

        /// <summary>
        /// Prevents the input from being changed by the user.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.<br />
        /// When <c>true</c>, the user can copy text in the control, but cannot change the value.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Allows the value to be edited.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Editable { get; set; } = false;

        /// <summary>
        /// Shows the toolbar.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public bool ShowToolbar { get; set; } = true;

        /// <summary>
        /// The CSS classes for the toolbar when <see cref="ShowToolbar"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string? ToolbarClass { get; set; }

        /// <summary>
        /// The display variant for this picker.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="PickerVariant.Inline"/>.<br />
        /// Other values are <see cref="PickerVariant.Dialog"/> and <see cref="PickerVariant.Static"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public PickerVariant PickerVariant { get; set; } = PickerVariant.Inline;

        /// <summary>
        /// The display variant of the text input.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// The location of the <see cref="AdornmentIcon"/> for the input.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Adornment.End"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Adornment Adornment { get; set; } = Adornment.End;

        /// <summary>
        /// The orientation of the picker when <see cref="PickerVariant"/> is <see cref="PickerVariant.Static"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Orientation.Portrait"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public Orientation Orientation { get; set; } = Orientation.Portrait;

        /// <summary>
        /// The size of the icon in the input field.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the toolbar, selected, and active values.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Primary"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Occurs when <see cref="Text"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<string> TextChanged { get; set; }

        /// <summary>
        /// Updates <see cref="Text"/> immediately upon typing when <see cref="Editable"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.<br />
        /// When <c>false</c>, <see cref="Text"/> is only updated when pressing <c>Enter</c> or upon loss of focus.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ImmediateText { get; set; }

        /// <summary>
        /// Occurs when the text input has been clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// The currently selected value, as a string.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public string? Text
        {
            get => _text;
            set => SetTextAsync(value, true).CatchAndLog();
        }

        /// <summary>
        /// The CSS classes applied to the action buttons container.
        /// </summary>
        /// <remarks>Multiple classes must be separated by a space.</remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string? ActionsClass { get; set; }

        /// <summary>
        /// The custom action buttons to display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public RenderFragment<MudPicker<T>>? PickerActions { get; set; }

        /// <summary>
        /// Applies vertical spacing.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// The mask to apply to input values when <see cref="Editable"/> is <c>true</c>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public IMask? Mask
        {
            get => _mask;
            set => _mask = value;
        }

        /// <summary>
        /// The location the popover opens, relative to its container.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.BottomLeft"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public Origin AnchorOrigin { get; set; } = Origin.BottomLeft;

        /// <summary>
        /// The direction the popover opens, relative to its container.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.TopLeft"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// The behavior of the popover when it overflows its container.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="OverflowBehavior.FlipOnOpen"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public OverflowBehavior OverflowBehavior { get; set; } = OverflowBehavior.FlipOnOpen;

        protected IMask? _mask = null;

        protected bool GetDisabledState() => Disabled || ParentDisabled;

        protected bool GetReadOnlyState() => ReadOnly || ParentReadOnly;

        protected async Task SetTextAsync(string? value, bool callback)
        {
            if (_text != value)
            {
                _text = value;
                if (callback)
                    await StringValueChangedAsync(_text);
                await TextChanged.InvokeAsync(_text);
            }
        }

        /// <summary>
        /// Occurs when the string value has changed.
        /// </summary>
        protected virtual Task StringValueChangedAsync(string? value) => Task.CompletedTask;

        protected bool Open { get; set; }

        /// <summary>
        /// Opens or closes this picker.
        /// </summary>
        public Task ToggleOpenAsync()
        {
            if (Open)
            {
                return CloseAsync();
            }

            return OpenAsync();
        }

        /// <summary>
        /// Closes this picker.
        /// </summary>
        /// <param name="submit">When <c>true</c>, the value is committed.</param>
        public async Task CloseAsync(bool submit = true)
        {
            Open = false;

            if (submit)
            {
                await SubmitAsync();
            }

            await OnClosedAsync();
            StateHasChanged();
        }

        /// <summary>
        /// Displays this picker.
        /// </summary>
        public Task OpenAsync()
        {
            Open = true;
            StateHasChanged();

            return OnOpenedAsync();
        }

        private Task CloseOverlayAsync() => CloseAsync(PickerActions == null);

        protected internal virtual Task SubmitAsync() => Task.CompletedTask;

        /// <summary>
        /// Hides this picker.
        /// </summary>
        /// <param name="close">When <c>true</c>, the picker will be closed if <see cref="PickerVariant"/> is not <see cref="PickerVariant.Static"/>.</param>
        public virtual async Task ClearAsync(bool close = true)
        {
            if (close && PickerVariant != PickerVariant.Static)
            {
                await CloseAsync(false);
            }
        }

        protected override async Task ResetValueAsync()
        {
            if (_inputReference is not null)
            {
                await _inputReference.ResetAsync();
            }
            await base.ResetValueAsync();
        }

        protected internal MudTextField<string>? _inputReference;

        /// <summary>
        /// Focuses the input.
        /// </summary>
        public virtual ValueTask FocusAsync() => _inputReference?.FocusAsync() ?? ValueTask.CompletedTask;

        /// <summary>
        /// Releases focus for the input.
        /// </summary>
        public virtual ValueTask BlurAsync() => _inputReference?.BlurAsync() ?? ValueTask.CompletedTask;

        /// <summary>
        /// Selects the input content.
        /// </summary>
        public virtual ValueTask SelectAsync() => _inputReference?.SelectAsync() ?? ValueTask.CompletedTask;

        /// <summary>
        /// Selects a portion of the input content.
        /// </summary>
        /// <param name="pos1">The index of the first character to select.</param>
        /// <param name="pos2">The index of the last character to select.</param>
        public virtual ValueTask SelectRangeAsync(int pos1, int pos2) =>
            _inputReference?.SelectRangeAsync(pos1, pos2) ?? ValueTask.CompletedTask;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (PickerVariant == PickerVariant.Static)
            {
                Open = true;

                if (!Rounded)
                {
                    _pickerSquare = true;
                }
            }
            else
            {
                _pickerSquare = Square;
            }

            if (Label == null && For != null)
                Label = For.GetLabelString();
        }

        private async Task EnsureKeyInterceptorAsync()
        {
            if (_keyInterceptorObserving)
            {
                return;
            }

            _keyInterceptorObserving = true;
            var options = new KeyInterceptorOptions(
                "mud-input-slot",
                [
                    new(" ", preventDown: "key+none"),
                    new("ArrowUp", preventDown: "key+none"),
                    new("ArrowDown", preventDown: "key+none"),
                    new("Enter", preventDown: "key+none"),
                    new("NumpadEnter", preventDown: "key+none"),
                    new("/./", subscribeDown: true, subscribeUp: true)
                ]);

            await KeyInterceptorService.SubscribeAsync(_elementId, options, keyDown: OnHandleKeyDownAsync);
        }

        private async Task OnClickAsync(MouseEventArgs args)
        {
            if (!Editable)
            {
                await ToggleStateAsync();
            }

            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(args);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await EnsureKeyInterceptorAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected internal async Task ToggleStateAsync()
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            if (Open)
            {
                Open = false;
                await OnClosedAsync();
            }
            else
            {
                Open = true;
                await OnOpenedAsync();
                await FocusAsync();
            }
        }

        protected virtual async Task OnOpenedAsync()
        {
            await OnPickerOpenedAsync();

            if (PickerVariant == PickerVariant.Inline)
            {
                await _pickerInlineRef.MudChangeCssAsync(PickerInlineClassname);
            }

            await EnsureKeyInterceptorAsync();
            await KeyInterceptorService.UpdateKeyAsync(_elementId, new("Escape", stopDown: "key+none"));
        }

        protected virtual async Task OnClosedAsync()
        {
            await OnPickerClosedAsync();

            await EnsureKeyInterceptorAsync();
            await KeyInterceptorService.UpdateKeyAsync(_elementId, new("Escape", stopDown: "none"));
        }

        protected virtual Task OnPickerOpenedAsync() => PickerOpened.InvokeAsync(this);

        protected virtual Task OnPickerClosedAsync() => PickerClosed.InvokeAsync(this);

        protected internal virtual async Task OnHandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            switch (args.Key)
            {
                case "Backspace":
                    if (args.CtrlKey && args.ShiftKey)
                    {
                        await ClearAsync();
                        _value = default;
                        await ResetAsync();
                    }

                    break;
                case "Escape":
                case "Tab":
                    await CloseAsync(false);
                    break;
            }
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
    }
}
