using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public abstract class MudBaseInput<T> : MudFormComponent<T>
    {
        /// <summary>
        /// If true, this is a top-level form component. If false, this input is a sub-component of another input (i.e. TextField, Select, etc).
        /// If it is sub-component, it will NOT do form validation!!
        /// </summary>
        [CascadingParameter(Name = "Standalone")]
        internal bool Standalone { get; set; } = true;

        /// <summary>
        /// If true, the input element will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, the input will be read only.
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
        /// Icon that will be used if Adornment is set to Start or End.
        /// </summary>
        [Parameter] public string AdornmentIcon { get; set; }

        /// <summary>
        /// Text that will be used if Adornment is set to Start or End, the Text overrides Icon.
        /// </summary>
        [Parameter] public string AdornmentText { get; set; }

        /// <summary>
        /// Sets Start or End Adornment if not set to None.
        /// </summary>
        [Parameter] public Adornment Adornment { get; set; } = Adornment.None;

        /// <summary>
        /// Sets the Icon Size.
        /// </summary>
        [Parameter] public Size IconSize { get; set; } = Size.Small;

        /// <summary>
        /// Button click event if set and Adornment used.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }
        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter] public InputType InputType { get; set; } = InputType.Text;

        /// <summary>
        /// Variant to use.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        ///  Will adjust vertical spacing. 
        /// </summary>
        [Parameter] public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        ///  A multiline input (textarea) will be shown, if set to more than one line.
        /// </summary>
        [Parameter] public int Lines { get; set; } = 1;

        protected string _text;

        [Parameter]
        public string Text
        {
            get => _text;
            set => SetText(value, true);
        }

        private void SetText(string text, bool updateValue)
        {
            if (_text != text)
            {
                _text = text;
                if (updateValue)
                    UpdateValueProperty(false);
                TextChanged.InvokeAsync(_text);
            }
        }

        /// <summary>
        /// Text change hook for descendants  
        /// </summary>
        protected virtual void UpdateTextProperty(bool updateValue)
        {
            SetText(Converter.Set(Value), updateValue);
        }

        [Parameter] public EventCallback<string> TextChanged { get; set; }

        [Parameter] public EventCallback<FocusEventArgs> OnBlur { get; set; }

        protected virtual void OnBlurred(FocusEventArgs obj)
        {
            BeginValidateAfter(OnBlur.InvokeAsync(obj));
        }

        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        protected virtual void onKeyDown(KeyboardEventArgs obj) => OnKeyDown.InvokeAsync(obj);

        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyPress { get; set; }

        protected virtual void onKeyPress(KeyboardEventArgs obj) => OnKeyPress.InvokeAsync(obj);

        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

        protected virtual void onKeyUp(KeyboardEventArgs obj) => OnKeyUp.InvokeAsync(obj);

        /// <summary>
        /// Fired when the Value property changes. 
        /// </summary>
        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        /// <summary>
        /// The value of this input element. This property is two-way bindable.
        /// </summary>
        [Parameter]
        public T Value
        {
            get => _value;
            set => SetValue(value, true);
        }

        private void SetValue(T value, bool updateText)
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                if (updateText)
                    UpdateTextProperty(false);
                BeginValidateAfter(ValueChanged.InvokeAsync(_value));
            }
        }

        /// <summary>
        /// Value change hook for descendants  
        /// </summary>
        protected virtual void UpdateValueProperty(bool updateText)
        {
            SetValue(Converter.Get(Text), updateText);
        }

        private Converter<T> _converter = new DefaultConverter<T>();

        [Parameter]
        public Converter<T> Converter
        {
            get => _converter;
            set
            {
                if (_converter == value)
                    return;
                _converter = value;
                if (_converter == null)
                    return;
                _converter.OnError = OnConversionError;
                UpdateTextProperty(false);      // refresh only Text property from current Value
            }
        }

        [Parameter]
        public CultureInfo Culture
        {
            get => _converter?.Culture;
            set
            {
                if (_converter == null)
                    _converter = new DefaultConverter<T>();
                _converter.Culture = value;
                UpdateTextProperty(false);      // refresh only Text property from current Value
            }
        }

        private string _format = null;

        /// <summary>
        /// Conversion format parameter for ToString(), can be used for formatting primitive types, DateTimes and TimeSpans
        /// </summary>
        [Parameter]
        public string Format
        {
            get => _format;
            set
            {
                _format = value;
                if (_converter==null)
                    _converter = new DefaultConverter<T>();
                _converter.Format = _format;
                UpdateTextProperty(false);      // refresh only Text property from current Value
            }
        }

        /// <summary>
        /// True if the conversion from string to T failed
        /// </summary>
        public override bool ConversionError
        {
            get
            {
                if (_converter == null)
                    return false;
                return _converter.GetError;
            }
        }

        /// <summary>
        /// The error message of the conversion error from string to T. Null otherwise
        /// </summary>
        public override string ConversionErrorMessage
        {
            get
            {
                if (_converter == null)
                    return null;
                return _converter.GetErrorMessage;
            }
        }


        public string GetErrorText()
        {
            // ErrorText is either set from outside or the first validation error
            if (!string.IsNullOrWhiteSpace(ErrorText))
                return ErrorText; 
            if (!string.IsNullOrWhiteSpace(ConversionErrorMessage))
                return ConversionErrorMessage;
            return null;
        }

        internal override async Task ValidateValue()
        {
            if (Standalone)
                await base.ValidateValue();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Because the way the Value setter is built, it won't cause an update if the incoming Value is
            // equal to the initial value. This is why we force an update to the Text property here.
            UpdateTextProperty(false); 
        }

        protected override Task OnInitializedAsync()
        {
            if (_converter != null)
                _converter.OnError = OnConversionError;
            return base.OnInitializedAsync();
        }

        protected override void RegisterAsFormComponent()
        {
            if (Standalone)
                base.RegisterAsFormComponent();
        }

        protected override void OnParametersSet()
        {
            if (!Standalone)
                return;
            base.OnParametersSet();
        }

        protected override void ResetValue()
        {
            base.ResetValue();
            if (string.IsNullOrWhiteSpace(_text))
                return;
            _text = null;
            StateHasChanged();
        }
    }
}
