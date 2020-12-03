using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using System.Globalization;

namespace MudBlazor
{
    public abstract class MudBaseInput<T> : MudComponentBase, IFormComponent, IDisposable
    {

        /// <summary>
        /// If true, this is a top-level form component. If false, this input is a sub-component of another input (i.e. TextField, Select, etc).
        /// If it is sub-component, it will NOT do form validation!!
        /// </summary>
        [CascadingParameter(Name = "Standalone")]
        internal bool Standalone { get; set; } = true;

        [CascadingParameter] internal MudForm Form { get; set; }

        /// <summary>
        /// If true, this form input is required to be filled out.
        /// </summary>
        [Parameter] public bool Required { get; set; }

        /// <summary>
        /// Set an error text that will be displayed if the input is not filled out but required!
        /// </summary>
        [Parameter] public string RequiredError { get; set; } = "Required";

        /// <summary>
        /// The ErrorText that will be displayed if Error true
        /// </summary>
        [Parameter] public string ErrorText { get; set; }

        /// <summary>
        /// If true, the label will be displayed in an error state.
        /// </summary>
        [Parameter] public bool Error { get; set; }

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
        /// Text change hook for descendants  
        /// </summary>
        /// <param name="value"></param>
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


        private T _value;

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
                    ValidateValue(value);
                    ValueChanged.InvokeAsync(value);
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

        protected virtual void OnConversionError(string error)
        {
            /* to be overridden */
        }

        /// <summary>
        /// True if the conversion from string to T failed
        /// </summary>
        public bool ConversionError
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
        public string ConversionErrorMessage
        {
            get
            {
                if (_converter == null)
                    return null;
                return _converter.GetErrorMessage;
            }
        }

        /// <summary>
        /// True if the input has any of the following errors: An error set from outside, a conversion error or
        /// one or more validation errors
        /// </summary>
        public bool HasErrors => Error || ConversionError || ValidationErrors.Count > 0;

        public string GetErrorText()
        {
            // ErrorText is either set from outside or the first validation error
            if (!string.IsNullOrWhiteSpace(ErrorText))
                return ErrorText; 
            if (!string.IsNullOrWhiteSpace(ConversionErrorMessage))
                return ConversionErrorMessage;
            return null;
        }

        #region Validation

        public List<string> ValidationErrors { get; set; } = new List<string>();

        /// <summary>
        /// A validation func or a validation attribute. Supported types are:
        /// Func<T, bool> ... will output the standard error message "Invalid" if false
        /// Func<T, string> ... outputs the result as error message, no error if null
        /// Func<T, IEnumerable<string>> ... outputs all the returned error messages, no error if empty
        /// Func<T, Task<bool>> ... will output the standard error message "Invalid" if false
        /// Func<T, Task<string>> ... outputs the result as error message, no error if null
        /// Func<T, Task<IEnumerable<string>>> ... outputs all the returned error messages, no error if empty
        /// System.ComponentModel.DataAnnotations.ValidationAttribute instances
        /// </summary>
        [Parameter]
        public object Validation { get; set; }


        /// <summary>
        /// Causes this component to validate its value
        /// </summary>
        public async Task Validate() => await ValidateValue(Value);

        internal async virtual Task ValidateValue(T value)
        {
            if (Form == null || !Standalone)
                return;
            ValidationErrors = new List<string>();
            try
            {
                if (Required)
                {
                    // a value is required, so if nothing has been entered, we'll return ERROR
                    var is_valid = true;
                    if (typeof(T)==typeof(string))
                        is_valid = !string.IsNullOrWhiteSpace((string)(object)value);
                    else if (value == null)
                        is_valid = false;
                    if (!is_valid)
                    {
                        ValidationErrors.Add(RequiredError);
                        return;
                    }
                }
                else
                {
                    // a value is not required, so if nothing has been entered, we'll return OK without calling validation funcs
                    var is_empty = false;
                    if (typeof(T) == typeof(string))
                        is_empty = string.IsNullOrWhiteSpace((string)(object)value);
                    else if (value == null)
                        is_empty = false;
                    if (is_empty)
                        return;
                }
                if (Validation is ValidationAttribute)
                    ValidateWithAttribute(Validation as ValidationAttribute, value);
                else if (Validation is Func<T, bool>)
                    ValidateWithFunc(Validation as Func<T, bool>, value);
                else if (Validation is Func<T, string>)
                    ValidateWithFunc(Validation as Func<T, string>, value);
                else if (Validation is Func<T, IEnumerable<string>>)
                    ValidateWithFunc(Validation as Func<T, IEnumerable<string>>, value);
                else if (Validation is Func<T, Task<bool>>)
                    await ValidateWithFunc(Validation as Func<T, Task<bool>>, value);
                else if (Validation is Func<T, Task<string>>)
                    await ValidateWithFunc(Validation as Func<T, Task<string>>, value);
                else if (Validation is Func<T, Task<IEnumerable<string>>>)
                    await ValidateWithFunc(Validation as Func<T, Task<IEnumerable<string>>>, value);
            }
            finally
            {
                // this must be called in any case, because even if Validation is null the user might have set Error and ErrorText manually
                // if Error and ErrorText are set by the user, setting them here will have no effect. 
                Error = ValidationErrors.Count > 0;
                ErrorText = ValidationErrors.FirstOrDefault();
                Form.Update(this);
                StateHasChanged();
            }
        }

        protected virtual void ValidateWithAttribute(ValidationAttribute attr, T value)
        {
            if (attr.IsValid(value))
                return;
            ValidationErrors.Add(attr.ErrorMessage);
        }

        protected virtual void ValidateWithFunc(Func<T, bool> func, T value)
        {
            try
            {
                if (func(value))
                    return;
                ValidationErrors.Add("Invalid");
            }
            catch (Exception e)
            {
                ValidationErrors.Add("Error in validation func: " + e.Message);
            }
        }

        protected virtual void ValidateWithFunc(Func<T, string> func, T value)
        {
            try
            {
                var error = func(value);
                if (error == null)
                    return;
                ValidationErrors.Add(error);
            }
            catch (Exception e)
            {
                ValidationErrors.Add("Error in validation func: " + e.Message);
            }
        }

        protected virtual void ValidateWithFunc(Func<T, IEnumerable<string>> func, T value)
        {
            try
            {
                foreach (var error in func(value))
                    ValidationErrors.Add(error);
            }
            catch (Exception e)
            {
                ValidationErrors.Add("Error in validation func: " + e.Message);
            }
        }

        protected async virtual Task ValidateWithFunc(Func<T, Task<bool>> func, T value)
        {
            try
            {
                if (await func(value))
                    return;
                ValidationErrors.Add("Invalid");
            }
            catch (Exception e)
            {
                ValidationErrors.Add("Error in validation func: " + e.Message);
            }
        }

        protected async virtual Task ValidateWithFunc(Func<T, Task<string>> func, T value)
        {
            try
            {
                var error = await func(value);
                if (error == null)
                    return;
                ValidationErrors.Add(error);
            }
            catch (Exception e)
            {
                ValidationErrors.Add("Error in validation func: " + e.Message);
            }
        }

        protected async virtual Task ValidateWithFunc(Func<T, Task<IEnumerable<string>>> func, T value)
        {
            try
            {
                foreach (var error in await func(value))
                    ValidationErrors.Add(error);
            }
            catch (Exception e)
            {
                ValidationErrors.Add("Error in validation func: " + e.Message);
            }
        }

        public void Reset()
        {
            _value = default;
            ResetValidation();
        }

        public void ResetValidation()
        {
            Error = false;
            ValidationErrors.Clear();
            StateHasChanged();
        }

        #endregion

        protected override void OnInitialized()
        {
            base.OnInitialized();
            // this is important for value type T's where the initial Value is equal to the default(T) because the way the Value setter is built,
            // it won't cause an update if the incoming value is equal to the internal value. That's why we trigger that update here
            GenericValueChanged(Value); 
        }

        protected override Task OnInitializedAsync()
        {
            if (Standalone)
            {
                Form?.Add(this);
            }
            if (_converter != null)
                _converter.OnError = OnConversionError;
            return base.OnInitializedAsync();
        }

        #region --> Blazor EditForm validation support

        /// <summary>
        /// This is the form validation context for Blazor's <EditForm></EditForm> component
        /// </summary>
        [CascadingParameter]
        EditContext EditContext { get; set; } = default!;

        /// <summary>
        /// Triggers field to be validated.
        /// </summary>
        internal void EditFormValidate()
        {
            if (_fieldIdentifier.FieldName == null)
            {
                return;
            }
            EditContext?.NotifyFieldChanged(_fieldIdentifier);
        }

        /// <summary>
        /// Specify an expression which returns the model's field for which validation messages should be displayed.
        /// Currently only string fields are supported.
        /// </summary>
        [Parameter]
        public Expression<Func<T>>? For { get; set; }


        private void OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
        {
            if (EditContext == null)
                return;
            var error_msgs = EditContext.GetValidationMessages(_fieldIdentifier).ToArray();
            Error = error_msgs.Length > 0;
            ErrorText = (Error ? error_msgs[0] : null);
            StateHasChanged();
        }

        /// <summary>
        /// Points to a field of the model for which validation messages should be displayed.
        /// </summary>
        private FieldIdentifier _fieldIdentifier;

        /// <summary>
        /// To find out whether or not For parameter has changed we keep a separate reference
        /// </summary>
        private Expression<Func<T>>? _currentFor;

        /// <summary>
        /// To find out whether or not EditContext parameter has changed we keep a separate reference
        /// </summary>
        private EditContext? _currentEditContext;

        protected override void OnParametersSet()
        {
            if (EditContext == null)
                return;
            if (For == null)
                return;
            if (!Standalone)
                return;
            if (For != _currentFor)
            {
                _fieldIdentifier = FieldIdentifier.Create(For);
                _currentFor = For;
            }

            if (EditContext != _currentEditContext)
            {
                DetachValidationStateChangedListener();
                EditContext.OnValidationStateChanged += OnValidationStateChanged;
                _currentEditContext = EditContext;
            }
        }

        private void DetachValidationStateChangedListener()
        {
            if (_currentEditContext != null)
            {
                _currentEditContext.OnValidationStateChanged -= OnValidationStateChanged;
            }
        }

        #endregion

        /// <summary>
        /// Called to dispose this instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if called within <see cref="IDisposable.Dispose"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        void IDisposable.Dispose()
        {
            //ParentForm?.Remove(this);
            DetachValidationStateChangedListener();
            Dispose(disposing: true);
        }
    }
}
