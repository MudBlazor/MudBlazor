using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;
using System.Globalization;
using Microsoft.AspNetCore.Components;

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
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

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
        /// If true the input will focus automatically
        /// </summary>
        [Parameter] public bool AutoFocus { get; set; }

        /// <summary>
        ///  A multiline input (textarea) will be shown, if set to more than one line.
        /// </summary>
        [Parameter] public int Lines { get; set; } = 1;

        protected bool _settingText;
        protected string _text;
        [Parameter]
        public string Text
        {
            get => _text;
            set
            {
                if (_text == value)
                    return;
                // update loop protection!
                if (_settingText)
                    return;
                _settingText = true;
                try
                {
                    _text = value;
                    StringValueChanged(value);
                    TextChanged.InvokeAsync(value);
                }
                finally
                {
                    _settingText = false;
                }
            }
        }

        [Parameter] public EventCallback<string> TextChanged { get; set; }

        /// <summary>
        /// Focuses the element
        /// </summary>
        /// <returns>The ValueTask</returns>
        public abstract ValueTask FocusAsync();

        /// <summary>
        /// Text change hook for descendants  
        /// </summary>
        /// <param name="text"></param>
        protected virtual void StringValueChanged(string text)
        {
            Value = Converter.Get(text);
        }

        [Parameter] public EventCallback<FocusEventArgs> OnBlur { get; set; }

        protected virtual void OnBlurred(FocusEventArgs obj)
        {
            ValidateValue(Value);
            EditFormValidate();
            OnBlur.InvokeAsync(obj);
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
            set
            {
                if (object.Equals(value, _value))
                    return;
                if (_settingValue)
                    return;
                _settingValue = true;
                try
                {
                    _value = value;
                    GenericValueChanged(value);
                    ValueChanged.InvokeAsync(value);
                    ValidateValue(value);
                    EditFormValidate();
                }
                finally
                {
                    _settingValue = false;
                }
            }
        }

        private bool _settingValue;

        /// <summary>
        /// Value change hook for descendants
        /// </summary>
        /// <param name="value"></param>
        protected virtual void GenericValueChanged(T value)
        {
            Text = Converter.Set(value);
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
                Text = Converter.Set(Value);
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
                Text = Converter.Set(Value);
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
                Text = Converter.Set(Value);
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

        internal override async Task ValidateValue(T value)
        {
            if (!Standalone)
                return;
            await base.ValidateValue(value);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            // this is important for value type T's where the initial Value is equal to the default(T) because the way the Value setter is built,
            // it won't cause an update if the incoming value is equal to the internal value. That's why we trigger that update here
            GenericValueChanged(Value); 
        }

        protected override Task OnInitializedAsync()
        {
            if (_converter != null)
                _converter.OnError = OnConversionError;

            return base.OnInitializedAsync();
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
