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
        protected MudBaseInput() : base(new DefaultConverter<T>()) { }

        /// <summary>
        /// If true, the input element will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, the input will be read-only.
        /// </summary>
        [Parameter] public bool ReadOnly { get; set; }

        /// <summary>
        /// If true, the input will take up the full width of its container.
        /// </summary>
        [Parameter] public bool FullWidth { get; set; }

        /// <summary>
        /// If true, the input will update the Value immediately on typing.
        /// If false, the Value is updated only on Enter.
        /// </summary>
        [Parameter] public bool Immediate { get; set; }

        /// <summary>
        /// If true, the input will not have an underline.
        /// </summary>
        [Parameter] public bool DisableUnderLine { get; set; }

        /// <summary>
        /// The HelperText will be displayed below the text field.
        /// </summary>
        [Parameter] public string HelperText { get; set; }

        /// <summary>
        /// If true, the helper text will only be visible on focus.
        /// </summary>
        [Parameter] public bool HelperTextOnFocus { get; set; }

        /// <summary>
        /// Icon that will be used if Adornment is set to Start or End.
        /// </summary>
        [Parameter] public string AdornmentIcon { get; set; }

        /// <summary>
        /// Text that will be used if Adornment is set to Start or End, the Text overrides Icon.
        /// </summary>
        [Parameter] public string AdornmentText { get; set; }

        /// <summary>
        /// The Adornment if used. By default, it is set to None.
        /// </summary>
        [Parameter] public Adornment Adornment { get; set; } = Adornment.None;

        /// <summary>
        /// The color of the adornment if used. It supports the theme colors.
        /// </summary>
        [Parameter] public Color AdornmentColor { get; set; } = Color.Default;

        /// <summary>
        /// The Icon Size.
        /// </summary>
        [Parameter] public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// Button click event if set and Adornment used.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }

        /// <summary>
        /// Variant to use.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        ///  Will adjust vertical spacing.
        /// </summary>
        [Parameter] public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        /// <summary>
        /// If set, will display the counter, value 0 will display current count but no stop count.
        /// </summary>
        [Parameter] public int? Counter { get; set; }

        /// <summary>
        /// Maximum number of characters that the input will accept
        /// </summary>
        [Parameter] public int MaxLength { get; set; } = 524288;

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// If true the input will focus automatically.
        /// </summary>
        [Parameter] public bool AutoFocus { get; set; }

        /// <summary>
        ///  A multiline input (textarea) will be shown, if set to more than one line.
        /// </summary>
        [Parameter] public int Lines { get; set; } = 1;

        /// <summary>
        ///  The text to be displayed.
        /// </summary>
        [Parameter]
        public string Text { get; set; }

        /// <summary>
        /// When TextUpdateSuppression is true (which is default) the text can not be updated by bindings while the component is focused in BSS (not WASM).
        /// This solves issue #1012: Textfield swallowing chars when typing rapidly
        /// If you need to update the input's text while it is focused you can set this parameter to false.
        /// Note: on WASM text update suppression is not active, so this parameter has no effect.
        /// </summary>
        [Parameter] public bool TextUpdateSuppression { get; set; } = true;

        /// <summary>
        ///  Hints at the type of data that might be entered by the user while editing the input
        /// </summary>
        [Parameter] public virtual InputMode InputMode { get; set; } = InputMode.text;

        /// <summary>
        /// The pattern attribute, when specified, is a regular expression which the input's value must match in order for the value to pass constraint validation. It must be a valid JavaScript regular expression
        /// Not Supported in multline input
        /// </summary>
        [Parameter] public virtual string Pattern { get; set; }

        /// <summary>
        /// Derived classes need to override this if they can be something other than text
        /// </summary>
        internal virtual InputType GetInputType() { return InputType.Text; }

        protected async Task SetTextAsync(string text, bool updateValue = true)
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

        protected virtual void OnBlurred(FocusEventArgs obj)
        {
            _isFocused = false;
            Touched = true;
            BeginValidateAfter(OnBlur.InvokeAsync(obj));
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
        [Parameter] public bool KeyDownPreventDefault { get; set; }


        /// <summary>
        /// Fired on the KeyPress event.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyPress { get; set; }

        protected virtual void InvokeKeyPress(KeyboardEventArgs obj)
        {
            OnKeyPress.InvokeAsync(obj).AndForget();
        }

        /// <summary>
        /// Prevent the default action for the KeyPress event.
        /// </summary>
        [Parameter] public bool KeyPressPreventDefault { get; set; }

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
        [Parameter] public bool KeyUpPreventDefault { get; set; }

        /// <summary>
        /// Fired when the Value property changes.
        /// </summary>
        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        /// <summary>
        /// The value of this input element.
        /// </summary>
        [Parameter]
        public T Value
        {
            get => _value;
            set => _value = value;
        }

        protected virtual async Task SetValueAsync(T value, bool updateText = true)
        {
            if (!EqualityComparer<T>.Default.Equals(Value, value))
            {
                Value = value;
                if (updateText)
                    await UpdateTextPropertyAsync(false);
                await ValueChanged.InvokeAsync(Value);
                BeginValidate();
            }
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
            if (Standalone)
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
                        updateText=false;
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

        protected override void RegisterAsFormComponent()
        {
            if (Standalone)
                base.RegisterAsFormComponent();
        }

        protected override void OnParametersSet()
        {
            if (Standalone)
                base.OnParametersSet();
        }

        protected override void ResetValue()
        {
            SetTextAsync(null, updateValue:true).AndForget();
            base.ResetValue();
        }
    }
}
