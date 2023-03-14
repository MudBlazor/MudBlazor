using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract class MudBaseInput<T> : MudFormComponent<T, string>
    {
        private bool _isDirty;
        
        protected MudBaseInput() : base(new DefaultConverter<T>()) { }

        /// <summary>
        /// If true, the input element will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// If true, the input will be read-only.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// If true, the input will take up the full width of its container.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool FullWidth { get; set; }

        /// <summary>
        /// If true, the input will update the Value immediately on typing.
        /// If false, the Value is updated only on Enter.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Immediate { get; set; }

        /// <summary>
        /// If true, the input will not have an underline.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool DisableUnderLine { get; set; }

        /// <summary>
        /// The HelperText will be displayed below the text field.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string HelperText { get; set; }

        /// <summary>
        /// If true, the helper text will only be visible on focus.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool HelperTextOnFocus { get; set; }

        /// <summary>
        /// Icon that will be used if Adornment is set to Start or End.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string AdornmentIcon { get; set; }

        /// <summary>
        /// Text that will be used if Adornment is set to Start or End, the Text overrides Icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string AdornmentText { get; set; }

        /// <summary>
        /// The Adornment if used. By default, it is set to None.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Adornment Adornment { get; set; } = Adornment.None;

        /// <summary>
        /// The validation is only triggered if the user has changed the input value at least once. By default, it is false
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool OnlyValidateIfDirty { get; set; } = false;

        /// <summary>
        /// The color of the adornment if used. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color AdornmentColor { get; set; } = Color.Default;

        /// <summary>
        /// The aria-label of the adornment.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string AdornmentAriaLabel { get; set; } = string.Empty;

        /// <summary>
        /// The Icon Size.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// Button click event if set and Adornment used.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }

        /// <summary>
        /// Variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        ///  Will adjust vertical spacing.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string Placeholder { get; set; }

        /// <summary>
        /// If set, will display the counter, value 0 will display current count but no stop count.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public int? Counter { get; set; }

        /// <summary>
        /// Maximum number of characters that the input will accept
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public int MaxLength { get; set; } = 524288;

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string Label { get; set; }

        /// <summary>
        /// If true the input will focus automatically.
        /// </summary>
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
        ///  The text to be displayed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public string Text { get; set; }

        /// <summary>
        /// When TextUpdateSuppression is true (which is default) the text can not be updated by bindings while the component is focused in BSS (not WASM).
        /// This solves issue #1012: Textfield swallowing chars when typing rapidly
        /// If you need to update the input's text while it is focused you can set this parameter to false.
        /// Note: on WASM text update suppression is not active, so this parameter has no effect.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool TextUpdateSuppression { get; set; } = true;

        /// <summary>
        ///  Hints at the type of data that might be entered by the user while editing the input
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public virtual InputMode InputMode { get; set; } = InputMode.text;

        /// <summary>
        /// The pattern attribute, when specified, is a regular expression which the input's value must match in order for the value to pass constraint validation. It must be a valid JavaScript regular expression
        /// Not Supported in multline input
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public virtual string Pattern { get; set; }

        /// <summary>
        /// Derived classes need to override this if they can be something other than text
        /// </summary>
        internal virtual InputType GetInputType() { return InputType.Text; }

        protected virtual async Task SetTextAsync(string text, bool updateValue = true)
        {
            if (Text != text)
            {
                Text = text;
                if (!string.IsNullOrWhiteSpace(Text))
                    Touched = true;
                if (updateValue)
                    await UpdateValuePropertyAsync(false);
                await TextChanged.InvokeAsync(Text);
            }
        }

        /// <summary>
        /// Text change hook for descendants. Called when Text needs to be refreshed from current Value property.
        /// </summary>
        protected virtual Task UpdateTextPropertyAsync(bool updateValue)
        {
            return SetTextAsync(Converter.Set(Value), updateValue);
        }

        /// <summary>
        /// Focuses the element
        /// </summary>
        /// <returns>The ValueTask</returns>
        public virtual ValueTask FocusAsync() { return new ValueTask(); }

        public virtual ValueTask BlurAsync() { return new ValueTask(); }

        public virtual ValueTask SelectAsync() { return new ValueTask(); }

        public virtual ValueTask SelectRangeAsync(int pos1, int pos2) { return new ValueTask(); }

        /// <summary>
        /// Fired when the text value changes.
        /// </summary>
        [Parameter] public EventCallback<string> TextChanged { get; set; }

        /// <summary>
        /// Fired when the element loses focus.
        /// </summary>
        [Parameter] public EventCallback<FocusEventArgs> OnBlur { get; set; }

        /// <summary>
        /// Fired when the element changes internally its text value.
        /// </summary>
        [Parameter]
        public EventCallback<ChangeEventArgs> OnInternalInputChanged { get; set; }

        protected bool _isFocused;

        protected internal virtual void OnBlurred(FocusEventArgs obj)
        {
            _isFocused = false;

            if (!OnlyValidateIfDirty || _isDirty)
            {
                Touched = true;
                BeginValidateAfter(OnBlur.InvokeAsync(obj));
            }
        }

        /// <summary>
        /// Fired on the KeyDown event.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        protected virtual void InvokeKeyDown(KeyboardEventArgs obj)
        {
            _isFocused = true;
            OnKeyDown.InvokeAsync(obj).AndForget();
        }

        /// <summary>
        /// Prevent the default action for the KeyDown event.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool KeyDownPreventDefault { get; set; }


        /// <summary>
        /// Fired on the KeyPress event.
        /// </summary>
        [Obsolete("This will be removed in v7")]
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyPress { get; set; }

        [Obsolete("This will be removed in v7")]
        protected virtual void InvokeKeyPress(KeyboardEventArgs obj)
        {
            OnKeyPress.InvokeAsync(obj).AndForget();
        }

        /// <summary>
        /// Prevent the default action for the KeyPress event.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        [Obsolete("This will be removed in v7")]
        public bool KeyPressPreventDefault { get; set; }

        /// <summary>
        /// Fired on the KeyUp event.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

        protected virtual void InvokeKeyUp(KeyboardEventArgs obj)
        {
            _isFocused = true;
            OnKeyUp.InvokeAsync(obj).AndForget();
        }

        /// <summary>
        /// Prevent the default action for the KeyUp event.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool KeyUpPreventDefault { get; set; }

        /// <summary>
        /// Fired when the Value property changes.
        /// </summary>
        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        /// <summary>
        /// The value of this input element.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public T Value
        {
            get => _value;
            set => _value = value;
        }

        protected virtual async Task SetValueAsync(T value, bool updateText = true, bool force = false)
        {
            if (!EqualityComparer<T>.Default.Equals(Value, value) || force == true)
            {
                _isDirty = true;
                Value = value;
                if (updateText)
                    await UpdateTextPropertyAsync(false);
                await ValueChanged.InvokeAsync(Value);
                BeginValidate();
                FieldChanged(Value);
            }
        }

        /// <summary>
        /// Sync the value, values and text, calls validation manually. Useful to call after user changes value or text programmatically.
        /// </summary>
        /// <returns></returns>
        public virtual async Task ForceUpdate()
        {
            await SetValueAsync(Value, force: true);
        }

        /// <summary>
        /// Value change hook for descendants. Called when Value needs to be refreshed from current Text property.
        /// </summary>
        protected virtual Task UpdateValuePropertyAsync(bool updateText)
        {
            return SetValueAsync(Converter.Get(Text), updateText);
        }

        protected override bool SetConverter(Converter<T, string> value)
        {
            var changed = base.SetConverter(value);
            if (changed)
                UpdateTextPropertyAsync(false).AndForget();      // refresh only Text property from current Value

            return changed;
        }

        protected override bool SetCulture(CultureInfo value)
        {
            var changed = base.SetCulture(value);
            if (changed)
                UpdateTextPropertyAsync(false).AndForget();      // refresh only Text property from current Value

            return changed;
        }

        /// <summary>
        /// Conversion format parameter for ToString(), can be used for formatting primitive types, DateTimes and TimeSpans
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string Format
        {
            get => ((Converter<T>)Converter).Format;
            set => SetFormat(value);
        }

        protected virtual bool SetFormat(string value)
        {
            var changed = (Format != value);
            if (changed)
            {
                ((Converter<T>)Converter).Format = value;
                UpdateTextPropertyAsync(false).AndForget();      // refresh only Text property from current Value
            }
            return changed;
        }

        protected override Task ValidateValue()
        {
            if (SubscribeToParentForm)
                return base.ValidateValue();

            return Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Because the way the Value setter is built, it won't cause an update if the incoming Value is
            // equal to the initial value. This is why we force an update to the Text property here.
            if (typeof(T) != typeof(string)) 
                await UpdateTextPropertyAsync(false);

            if (Label == null && For != null)
                Label = For.GetLabelString();
        }

        public virtual void ForceRender(bool forceTextUpdate)
        {
            _forceTextUpdate = true;
            UpdateTextPropertyAsync(false).AndForget();
            StateHasChanged();
        }

        protected bool _forceTextUpdate;

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var hasText = parameters.Contains<string>(nameof(Text));
            var hasValue = parameters.Contains<T>(nameof(Value));

            // Refresh Value from Text
            if (hasText && !hasValue)
                await UpdateValuePropertyAsync(false);

            // Refresh Text from Value
            if (hasValue && !hasText)
            {
                var updateText = true;
                if (_isFocused && !_forceTextUpdate)
                {
                    // Text update suppression, only in BSS (not in WASM).
                    // This is a fix for #1012
                    if (RuntimeLocation.IsServerSide && TextUpdateSuppression)
                        updateText = false;
                }
                if (updateText)
                {
                    _forceTextUpdate = false;
                    await UpdateTextPropertyAsync(false);
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //Only focus automatically after the first render cycle!
            if (firstRender && AutoFocus)
            {
                await FocusAsync();
            }
        }

        protected override void OnParametersSet()
        {
            if (SubscribeToParentForm)
                base.OnParametersSet();
        }

        protected override void ResetValue()
        {
            SetTextAsync(null, updateValue: true).AndForget();
            this._isDirty = false;
            base.ResetValue();
        }
    }
}
