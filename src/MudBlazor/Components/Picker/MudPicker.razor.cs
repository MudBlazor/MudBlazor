using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a common form component for selecting date, time, and color values.
    /// </summary>
    /// <typeparam name="T">The type of value being chosen.</typeparam>
    /// <seealso cref="MudPickerContent" />
    /// <seealso cref="MudPickerToolbar" />
    public partial class MudPicker<T> : MudFormComponent<T, string>
    {
        protected IKeyInterceptor _keyInterceptor;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public MudPicker() : base(new Converter<T, string>()) { }

        protected MudPicker(Converter<T, string> converter) : base(converter) { }

        [Inject]
        private IKeyInterceptorFactory KeyInterceptorFactory { get; set; }

        private string _elementId = "picker" + Guid.NewGuid().ToString().Substring(0, 8);

        protected string PickerClassname =>
            new CssBuilder("mud-picker")
                .AddClass("mud-picker-inline", PickerVariant != PickerVariant.Static)
                .AddClass("mud-picker-static", PickerVariant == PickerVariant.Static)
                .AddClass("mud-rounded", PickerVariant == PickerVariant.Static && !_pickerSquare)
                .AddClass($"mud-elevation-{_pickerElevation}", PickerVariant == PickerVariant.Static)
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

        protected string ActionsClassname =>
            new CssBuilder("mud-picker-actions")
                .AddClass(ActionsClass)
                .Build();

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
        public string AdornmentAriaLabel { get; set; }

        /// <summary>
        /// The text displayed in the input if no value is specified.
        /// </summary>
        /// <remarks>
        /// This property is typically used to give the user a hint as to what kind of input is expected.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string Placeholder { get; set; }

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
        /// Defaults to <c>8</c>.<br />
        /// A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public int Elevation { set; get; } = 8;

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
        public string HelperText { get; set; }

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
        public string Label { get; set; }

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

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }
        protected bool GetDisabledState() => Disabled || ParentDisabled;

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

        [CascadingParameter(Name = "ParentReadOnly")]
        private bool ParentReadOnly { get; set; }
        protected bool GetReadOnlyState() => ReadOnly || ParentReadOnly;

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
        public string ToolbarClass { get; set; }

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
        public string Text
        {
            get => _text;
            set => SetTextAsync(value, true).CatchAndLog();
        }

        private string _text;

        /// <summary>
        /// The CSS classes applied to the action buttons container.
        /// </summary>
        /// <remarks>Multiple classes must be separated by a space.</remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string ActionsClass { get; set; }

        /// <summary>
        /// The custom action buttons to display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public RenderFragment<MudPicker<T>> PickerActions { get; set; }

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
        public IMask Mask
        {
            get => _mask;
            set => _mask = value;
        }

        /// <summary>
        /// The location the popover opens, relative to its container.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.TopLeft"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public Origin AnchorOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// The direction the popover opens, relative to its container.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.TopLeft"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        protected IMask _mask = null;

        protected async Task SetTextAsync(string value, bool callback)
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
        protected virtual Task StringValueChangedAsync(string value)
        {
            return Task.CompletedTask;
        }

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
            else
            {
                return OpenAsync();
            }
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

        protected internal MudTextField<string> _inputReference;

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

        private bool _pickerSquare;
        private int _pickerElevation;
        private ElementReference _pickerInlineRef;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (PickerVariant == PickerVariant.Static)
            {
                Open = true;
                if (Elevation == 8)
                {
                    _pickerElevation = 0;
                }
                else
                {
                    _pickerElevation = Elevation;
                }

                if (!Rounded)
                {
                    _pickerSquare = true;
                }
            }
            else
            {
                _pickerSquare = Square;
                _pickerElevation = Elevation;
            }

            if (Label == null && For != null)
                Label = For.GetLabelString();
        }

        private async Task EnsureKeyInterceptorAsync()
        {
            if (_keyInterceptor == null)
            {
                _keyInterceptor = KeyInterceptorFactory.Create();

                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    Keys =
                    {
                        new KeyOptions { Key = " ", PreventDown = "key+none" },
                        new KeyOptions { Key = "ArrowUp", PreventDown = "key+none" },
                        new KeyOptions { Key = "ArrowDown", PreventDown = "key+none" },
                        new KeyOptions { Key = "Enter", PreventDown = "key+none" },
                        new KeyOptions { Key = "NumpadEnter", PreventDown = "key+none" },
                        new KeyOptions { Key = "/./", SubscribeDown = true, SubscribeUp = true }, // for our users
                    },
                });
                _keyInterceptor.KeyDown += HandleKeyDown;
            }
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

        /// <summary>
        /// 'HandleKeyDown' needed to be async in order to call other async methods. Because
        /// the HandleKeyDown is virtual and the base needs to be called from overriden methods
        /// we can't use 'async void'. This would break the synchronous behavior of those
        /// overriden methods. The KeyInterceptor does not support async behavior, so we have to
        /// add this hook method for handling the KeyDown event.
        /// This method can be removed when the KeyInterceptor supports async behavior.
        /// </summary>
        /// <param name="args"></param>
        private async void HandleKeyDown(KeyboardEventArgs args)
        {
            await OnHandleKeyDownAsync(args);
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
            await _keyInterceptor.UpdateKey(new() { Key = "Escape", StopDown = "key+none" });
        }

        protected virtual async Task OnClosedAsync()
        {
            await OnPickerClosedAsync();

            await EnsureKeyInterceptorAsync();
            await _keyInterceptor.UpdateKey(new() { Key = "Escape", StopDown = "none" });
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_keyInterceptor != null)
                {
                    _keyInterceptor.KeyDown -= HandleKeyDown;
                    if (IsJSRuntimeAvailable)
                    {
                        _keyInterceptor.Dispose();
                    }
                }
            }
        }
    }
}
