using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a base class for designing form input components.
    /// </summary>
    /// <typeparam name="T">The type of item being input.</typeparam>
    public abstract class MudBaseInput<T> : MudFormComponent<T, string>
    {
        private bool _isDirty;
        /// <summary>
        /// Prevents validation from occurring more than once during a validation cycle.
        /// </summary>
        /// <remarks>
        /// This field is set to <c>true</c> to prevent validation from occurring more than once during a validation cycle.  Each change in the <see cref="Value"/> will reset this field to <c>false</c>.
        /// </remarks>
        private bool _validated;
        protected bool _isFocused;
        protected bool _forceTextUpdate;

        /// <summary>
        /// The resolved input element ID.
        /// </summary>
        protected string? InputElementId => _inputIdState.Value;
        private string? _userAttributesId = Identifier.Create("mudinput");
        private readonly string _componentId = Identifier.Create("mudinput");
        private readonly ParameterState<string?> _textState;
        private readonly ParameterState<T?> _valueState;
        private readonly ParameterState<string?> _formatState;
        private readonly ParameterState<string?> _inputIdState;

        protected MudBaseInput()
        {
            Converter = new DefaultConverter<T>
            {
                Culture = GetCulture,
                Format = GetFormat
            };

            using var registerScope = CreateRegisterScope();
            _textState = registerScope.RegisterParameter<string?>(nameof(Text))
                .WithParameter(() => Text)
                .WithEventCallback(() => TextChanged)
                .WithChangeHandler(OnTextParameterChangedAsync);
            _valueState = registerScope.RegisterParameter<T?>(nameof(Value))
                .WithParameter(() => Value)
                .WithEventCallback(() => ValueChanged)
                .WithChangeHandler(OnValueParameterChangedAsync);
            _formatState = registerScope.RegisterParameter<string?>(nameof(Format))
                .WithParameter(() => Format)
                .WithChangeHandler(OnCultureAndFormatChangedAsync);
            _inputIdState = registerScope.RegisterParameter<string?>(nameof(InputId))
                .WithParameter(() => InputId)
                .WithChangeHandler(UpdateInputIdStateAsync);
        }

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

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
        /// Defaults to <c>false</c>.  When <c>true</c>, the user can copy text in the control, but cannot change the <see cref="Value" />.
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
        /// Changes the <see cref="Value"/> as soon as input is received.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the <see cref="Value"/> property will be updated any time user input occurs.  Otherwise, <see cref="Value"/> is updated when the user presses <c>Enter</c> or the input loses focus.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Immediate { get; set; }

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
        /// Limits validation to when the user changes the <see cref="Value"/>.
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
        /// The size of the icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// Occurs when the adornment icon (but not the text) has been clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }

        /// <summary>
        /// The appearance variation to use.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// The amount of vertical spacing for this input.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Margin.None"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// Typography for the input text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Typo Typo { get; set; } = Typo.subtitle1;

        /// <summary>
        /// The text displayed in the input if no <see cref="Value"/> is specified.
        /// </summary>
        /// <remarks>
        /// This property is typically used to give the user a hint as to what kind of input is expected.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Placeholder { get; set; }

        /// <summary>
        /// The optional character count and stop count.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When <c>0</c>, the current character count is displayed.  When <c>1</c> or greater, the character count and this count are displayed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public int? Counter { get; set; }

        /// <summary>
        /// The maximum number of characters allowed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>524288</c>.  This value is typically set to a maximum length such as the size of a database column the value will be persisted to.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public int MaxLength { get; set; } = 524288;

        /// <summary>
        /// The label for this input.
        /// </summary>
        /// <remarks>
        /// If no <see cref="Value"/> is specified, the label will be displayed in the input.  Otherwise, it will be scaled down to the top of the input.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Label { get; set; }

        /// <summary>
        /// Automatically receives focus.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the input will receive focus automatically.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool AutoFocus { get; set; }

        /// <summary>
        ///  A multiline input (textarea) will be shown, if set to more than one line.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public int Lines { get; set; } = 1;

        /// <summary>
        /// The text displayed in the input.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.Data)]
        public string? Text { get; set; }

        /// <summary>
        /// The type of input expected.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="InputMode.text"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public virtual InputMode InputMode { get; set; } = InputMode.text;

        /// <summary>
        /// The regular expression used to validate the <see cref="Value"/> property.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  This property is used to validate the input against a regular expression.  Not supported if <see cref="Lines"/> is <c>2</c> or greater.  Must be a valid JavaScript regular expression.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public virtual string? Pattern { get; set; }

        /// <summary>
        /// Shows the label inside the input if no <see cref="Value"/> is specified.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// When <c>true</c>, the label will not move into the input when the input is empty.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool ShrinkLabel { get; set; }

        /// <summary>
        /// Occurs when the <see cref="Text"/> property has changed.
        /// </summary>
        [Parameter]
        public EventCallback<string?> TextChanged { get; set; }

        /// <summary>
        /// Occurs when the input loses focus.
        /// </summary>
        [Parameter]
        public EventCallback<FocusEventArgs> OnBlur { get; set; }

        /// <summary>
        /// Occurs when the internal text value has changed.
        /// </summary>
        [Parameter]
        public EventCallback<string?> OnInternalInputChanged { get; set; }

        /// <summary>
        /// Occurs when a key has been pressed down.
        /// </summary>
        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        /// <summary>
        /// Allows the default key-down action to occur.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the browser will not perform its default behavior when a key-down occurs.  This is typically used when a key-down needs to override a browser's default behavior.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool KeyDownPreventDefault { get; set; }

        /// <summary>
        /// Occurs when a pressed key has been released.
        /// </summary>
        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

        /// <summary>
        /// Prevents the default key-up action.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the browser will not perform its default behavior when a key-up occurs.  This is typically used when a key-up needs to override the browser's default behavior.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool KeyUpPreventDefault { get; set; }

        /// <summary>
        /// Occurs when the <see cref="Value"/> property has changed.
        /// </summary>
        [Parameter]
        public EventCallback<T?> ValueChanged { get; set; }

        /// <summary>
        /// The value for this input.
        /// </summary>
        /// <remarks>
        /// This property represents the strongly typed value for the input.  It is typically the result of parsing raw input via the <see cref="Text"/> property.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.Data)]
        public T? Value { get; set; }

        /// <summary>
        /// The format applied to values.
        /// </summary>
        /// <remarks>
        /// This property is passed into the <c>ToString()</c> method of the <see cref="Value"/> property, such as formatting <c>int</c>, <c>float</c>, <c>DateTime</c> and <c>TimeSpan</c> values.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Format { get; set; }

        /// <summary>
        /// The ID of the input element.
        /// </summary>
        /// <remarks>
        /// When set takes precedence over any internally generated IDs.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? InputId { get; set; }

        protected bool GetDisabledState() => Disabled || ParentDisabled;

        protected bool GetReadOnlyState() => ReadOnly || ParentReadOnly;

        /// <summary>
        /// Occurs when the value has changed internally.
        /// </summary>
        /// <remarks>
        /// This method is called when the <see cref="Text"/> property needs to be refreshed from current <see cref="Value" />.
        /// </remarks>
        protected virtual Task UpdateTextPropertyAsync(bool updateValue)
        {
            return SetTextAndUpdateValueAsync(ConvertSet(ReadValue), updateValue);
        }

        /// <summary>
        /// When overridden, obtains focus for this input.
        /// </summary>
        /// <returns>A <see cref="ValueTask" /> object.</returns>
        public virtual ValueTask FocusAsync() => ValueTask.CompletedTask;

        /// <summary>
        /// When overridden, releases focus from this input.
        /// </summary>
        /// <returns>A <see cref="ValueTask" /> object.</returns>
        public virtual ValueTask BlurAsync() => ValueTask.CompletedTask;

        /// <summary>
        /// When overridden, selects this input.
        /// </summary>
        /// <returns>A <see cref="ValueTask" /> object.</returns>
        public virtual ValueTask SelectAsync() => ValueTask.CompletedTask;

        /// <summary>
        /// When overridden, selects a portion of the input.
        /// </summary>
        /// <param name="pos1">The index of the first character to select.</param>
        /// <param name="pos2">The index of the last character to select.</param>
        /// <returns>A <see cref="ValueTask" /> object.</returns>
        public virtual ValueTask SelectRangeAsync(int pos1, int pos2) => ValueTask.CompletedTask;

        protected internal virtual async Task OnBlurredAsync(FocusEventArgs obj)
        {
            _isFocused = false;

            if (ReadOnly)
            {
                return;
            }

            // all the OnBlur parents (TextField, MudMask, NumericField, DateRange, etc) currently point to this method
            // which causes this method to be fired repeatedly, we can use the obj.Type of FocusedEventArgs to track it

            if (!OnlyValidateIfDirty || _isDirty)
            {
                Touched = true;
                if (_validated)
                {
                    if (OnBlur.HasDelegate)
                    {
                        obj.Type += ".additional";
                        await OnBlur.InvokeAsync(obj);
                    }
                }
                else
                {
                    if (OnBlur.HasDelegate)
                    {
                        obj.Type += ".additional";
                        await BeginValidationAfterAsync(OnBlur.InvokeAsync(obj));
                    }
                    else
                    {
                        await BeginValidateAsync();
                    }
                }
            }
        }

        protected virtual Task InvokeKeyDownAsync(KeyboardEventArgs obj)
        {
            _isFocused = true;

            return OnKeyDown.InvokeAsync(obj);
        }

        protected virtual Task InvokeKeyUpAsync(KeyboardEventArgs obj)
        {
            _isFocused = true;

            return OnKeyUp.InvokeAsync(obj);
        }

        protected virtual async Task SetValueAsync(T? value, bool updateText = true, bool force = false)
        {
            var valueChanged = !EqualityComparer<T?>.Default.Equals(ReadValue, value);

            if (!valueChanged && !force)
            {
                return;
            }

            _isDirty = true;
            _validated = false;

            // Use ParameterState to set Value instead of direct assignment
            // This ensures proper parameter lifecycle management
            await _valueState.SetValueAsync(value);

            // If force is true but value hasn't changed, ParameterState won't fire the callback
            // so we need to manually invoke it to maintain backward compatibility
            if (force && !valueChanged)
            {
                await ValueChanged.InvokeAsync(value);
            }

            if (updateText)
            {
                await UpdateTextPropertyAsync(false);
            }

            FieldChanged(value);
            await BeginValidateAsync();
        }

        private async Task OnValueParameterChangedAsync(ParameterChangedEventArgs<T?> arg)
        {
            _isDirty = true;
            _validated = false;

            // When Value changes from parent, update Text from Value
            // But only if Text is not also being set in the same parameter update
            // Check ParameterView to see if Text is also present
            if (!arg.ParameterView.Contains<string?>(nameof(Text)))
            {
                // Always update text when Value changes (TextUpdateSuppression removed)
                _forceTextUpdate = false;
                await UpdateTextPropertyAsync(false);
            }
        }

        /// <summary>
        /// Override to read Value from ParameterState instead of backing field.
        /// </summary>
        protected internal override T? ReadValue => _valueState.Value;

        /// <summary>
        /// Override to write Value to ParameterState instead of backing field.
        /// </summary>
        protected override Task WriteValueAsync(T? value) => _valueState.SetValueAsync(value);

        /// <summary>
        /// Sets the value, values, and text, and calls validation.
        /// </summary>
        /// <remarks>
        /// This method is typically called when the user has changed the <see cref="Value"/> or <see cref="Text"/> programmatically.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> object.
        /// </returns>
        public virtual Task ForceUpdate()
        {
            return SetValueAsync(ReadValue, force: true);
        }

        /// <summary>
        /// Occurs when the value has changed internally.
        /// </summary>
        /// <remarks>
        /// This method is called when the <see cref="Value"/> property needs to be refreshed from current <see cref="Text" />.
        /// </remarks>
        protected virtual Task UpdateValuePropertyAsync(bool updateText)
        {
            return SetValueAsync(ConvertGet(ReadText), updateText);
        }

        protected override string? GetFormat() => _formatState.Value;

        protected override async Task OnCultureAndFormatChangedAsync()
        {
            await base.OnCultureAndFormatChangedAsync();
            await UpdateTextPropertyAsync(false);
        }

        protected override async Task OnConverterChangedAsync()
        {
            await base.OnConverterChangedAsync();
            await UpdateTextPropertyAsync(false);
        }

        protected override async Task ValidateValue()
        {
            if (SubscribeToParentForm)
            {
                _validated = true;
                await base.ValidateValue();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Because the way the Value setter is built, it won't cause an update if the incoming Value is
            // equal to the initial value. This is why we force an update to the Text property here.
            if (typeof(T) != typeof(string))
            {
                await UpdateTextPropertyAsync(false);
            }

            if (Label == null && For != null)
            {
                Label = For.GetLabelString();
            }

            _userAttributesId = UserAttributes.FirstOrDefault(userAttribute => userAttribute.Key.Equals("id", StringComparison.InvariantCultureIgnoreCase)).Value?.ToString();

            if (_inputIdState.Value is null)
            {
                await UpdateInputIdStateAsync();
            }
        }

        /// <summary>
        /// Causes this input to be rerendered.
        /// </summary>
        /// <param name="forceTextUpdate">When <c>true</c>, the <see cref="Text"/> property will be updated before rendering.</param>
        public virtual void ForceRender(bool forceTextUpdate)
        {
            _forceTextUpdate = true;
            UpdateTextPropertyAsync(false).CatchAndLog();
            StateHasChanged();
        }

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var hasText = parameters.Contains<string>(nameof(Text));
            var hasValue = parameters.Contains<T>(nameof(Value));

            await base.SetParametersAsync(parameters);

            // Refresh Text from Value if Value is present but Text is not
            // This maintains backward compatibility with the old `if (hasValue && !hasText)` logic
            // ParameterState only fires OnValueParameterChangedAsync when value CHANGES,
            // but we need to update Text even when Value is passed unchanged (for formatting)
            if (hasValue && !hasText)
            {
                // Always update text when Value changes (TextUpdateSuppression removed)
                _forceTextUpdate = false;
                await UpdateTextPropertyAsync(false);
            }
        }

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //Only focus automatically after the first render cycle!
            if (firstRender && AutoFocus)
            {
                await FocusAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            if (SubscribeToParentForm)
            {
                base.OnParametersSet();
            }
            else
            {
                // MudBlazor uses an unconventional SubscribeToParentForm mechanism whose behavior is not fully understandable.
                // Because of this, we must manually call OnParametersSet on the ParameterContainer to ensure ParameterState fields update correctly.
                //
                // Without this manual call, scenarios involving inherited components can fall out of sync. For example:
                // - Component1 inherits a base component that defines a state parameter.
                // - Component2 also inherits that same base component, wraps Component1, and forwards its base parameters to Component1.
                // In this case, ParameterState will not remain properly synchronized since base.OnParametersSet is called conditionally.
                ParameterContainer.OnParametersSet();
            }
        }

        /// <inheritdoc />
        protected override async Task ResetValueAsync()
        {
            await SetTextAndUpdateValueAsync(null, updateValue: true);
            _isDirty = false;
            _validated = false;
            await base.ResetValueAsync();
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

        protected string? GetAriaDescribedByString()
        {
            var errorId = HasErrors ? ErrorIdState.Value : null;
            var helperId = GetHelperId();

            return errorId is not null && helperId is not null
                ? $"{errorId} {helperId}"
                : errorId ?? helperId ?? null;
        }

        /// <summary>
        /// The type of input received by this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="InputType.Text"/>.
        /// </remarks>
        internal virtual InputType GetInputType() => InputType.Text;

        protected internal string? ReadText => _textState.Value;

        protected Task SetTextAsync(string? text) => _textState.SetValueAsync(text);

        protected virtual async Task SetTextAndUpdateValueAsync(string? text, bool updateValue = true)
        {
            if (ReadText == text)
            {
                return;
            }

            _validated = false;

            if (!string.IsNullOrEmpty(text))
            {
                Touched = true;
            }

            await _textState.SetValueAsync(text);
            if (updateValue)
            {
                await UpdateValuePropertyAsync(false);
            }
        }

        private async Task OnTextParameterChangedAsync(ParameterChangedEventArgs<string?> arg)
        {
            _validated = false;

            if (!string.IsNullOrEmpty(arg.Value))
            {
                Touched = true;
            }

            // When Text changes from parent, update Value from Text using UpdateValuePropertyAsync
            // But only if Value is not also being set in the same parameter update
            // Check ParameterView to see if Value is also present
            if (!arg.ParameterView.Contains<T?>(nameof(Value)))
            {
                await UpdateValuePropertyAsync(updateText: false);
            }
        }

        private async Task UpdateInputIdStateAsync()
        {
            if (_inputIdState.Value is not null)
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
