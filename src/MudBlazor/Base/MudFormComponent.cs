using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor.Interfaces;
using static System.String;

namespace MudBlazor
{
    public abstract class MudFormComponent<T, U> : MudComponentBase, IFormComponent, IDisposable
    {
        private Converter<T, U> _converter;

        protected MudFormComponent(Converter<T, U> converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _converter.OnError = OnConversionError;
        }

        [CascadingParameter] internal IForm Form { get; set; }

        /// <summary>
        /// If true, this is a top-level form component. If false, this input is a sub-component of another input (i.e. TextField, Select, etc).
        /// If it is sub-component, it will NOT do form validation!!
        /// </summary>
        [CascadingParameter(Name = "Standalone")]
        internal bool Standalone { get; set; } = true;

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

        [Parameter]
        public Converter<T, U> Converter
        {
            get => _converter;
            set => SetConverter(value);
        }

        protected virtual bool SetConverter(Converter<T, U> value)
        {
            var changed = (_converter != value);
            if (changed)
            {
                _converter = value ?? throw new ArgumentNullException(nameof(value));   // converter is mandatory at all times
                _converter.OnError = OnConversionError;
            }
            return changed;
        }

        [Parameter]
        public CultureInfo Culture
        {
            get => _converter.Culture;
            set => SetCulture(value);
        }

        protected virtual bool SetCulture(CultureInfo value)
        {
            var changed = (_converter.Culture != value);
            if (changed)
            {
                _converter.Culture = value;
            }
            return changed;
        }

        private void OnConversionError(string error)
        {
            // note: we need to update the form here because the conversion error might lead to not updating the value
            // ... which leads to not updating the form
            Touched = true;
            Form?.Update(this);
            OnConversionErrorOccurred(error);
        }

        protected virtual void OnConversionErrorOccurred(string error)
        {
            /* Descendants can override this method to catch conversion errors */
        }

        /// <summary>
        /// True if the conversion from string to T failed
        /// </summary>
        public bool ConversionError => _converter.GetError;

        /// <summary>
        /// The error message of the conversion error from string to T. Null otherwise
        /// </summary>
        public string ConversionErrorMessage => _converter.GetErrorMessage;

        /// <summary>
        /// True if the input has any of the following errors: An error set from outside, a conversion error or
        /// one or more validation errors
        /// </summary>
        public bool HasErrors => Error || ConversionError || ValidationErrors.Count > 0;

        public string GetErrorText()
        {
            // ErrorText is either set from outside or the first validation error
            if (!IsNullOrWhiteSpace(ErrorText))
                return ErrorText;

            if (!IsNullOrWhiteSpace(ConversionErrorMessage))
                return ConversionErrorMessage;

            return null;
        }

        /// <summary>
        /// This manages the state of having been "touched" by the user. A form control always starts out untouched
        /// but becomes touched when the user performed input or the blur event was raised.
        ///
        /// The touched state is only relevant for inputs that have no value (i.e. empty text fields). Being untouched will
        /// suppress RequiredError
        /// </summary>
        public bool Touched { get; protected set; }

        #region MudForm Validation

        public List<string> ValidationErrors { get; set; } = new List<string>();

        /// <summary>
        /// A validation func or a validation attribute. Supported types are:
        /// <para>Func&lt;T, bool&gt; ... will output the standard error message "Invalid" if false</para>
        /// <para>Func&lt;T, string&gt; ... outputs the result as error message, no error if null </para>
        /// <para>Func&lt;T, IEnumerable&lt; string &gt;&gt; ... outputs all the returned error messages, no error if empty</para>
        /// <para>Func&lt;T, Task&lt; bool &gt;&gt; ... will output the standard error message "Invalid" if false</para>
        /// <para>Func&lt;T, Task&lt; string &gt;&gt; ... outputs the result as error message, no error if null</para>
        /// <para>Func&lt;T, Task&lt;IEnumerable&lt; string &gt;&gt;&gt; ... outputs all the returned error messages, no error if empty</para>
        /// <para>System.ComponentModel.DataAnnotations.ValidationAttribute instances</para>
        /// </summary>
        [Parameter]
        public object Validation { get; set; }

        /// <summary>
        /// This is the form component's value.
        /// </summary>
        protected T _value;

        // These are the fire-and-forget methods to launch an async validation process.
        // After each async step, we make sure the current Value of the component has not changed while
        // async code was executed to avoid race condition which could lead to incorrect validation results.
        protected void BeginValidateAfter(Task task)
        {
            Func<Task> execute = async () =>
            {
                var value = _value;

                await task;

                // we validate only if the value hasn't changed while we waited for task.
                // if it has in fact changed, another validate call will follow anyway
                if (EqualityComparer<T>.Default.Equals(value, _value))
                {
                    BeginValidate();
                }
            };
            execute().AndForget();
        }

        protected void BeginValidate()
        {
            Func<Task> execute = async () =>
            {
                var value = _value;

                await ValidateValue();

                if (EqualityComparer<T>.Default.Equals(value, _value))
                {
                    EditFormValidate();
                }
            };
            execute().AndForget();
        }

        /// <summary>
        /// Causes this component to validate its value
        /// </summary>
        public Task Validate()
        {
            // when a validation is forced, we must set Touched to true, because for untouched fields with
            // no value, validation does nothing due to the way forms are expected to work (display errors
            // only after fields have been touched).
            Touched = true;
            return ValidateValue();
        }

        protected virtual async Task ValidateValue()
        {
            var changed = false;
            var errors = new List<string>();
            try
            {
                // conversion error
                if (ConversionError)
                    errors.Add(ConversionErrorMessage);
                // validation errors
                if (Validation is ValidationAttribute)
                    ValidateWithAttribute(Validation as ValidationAttribute, _value, errors);
                else if (Validation is Func<T, bool>)
                    ValidateWithFunc(Validation as Func<T, bool>, _value, errors);
                else if (Validation is Func<T, string>)
                    ValidateWithFunc(Validation as Func<T, string>, _value, errors);
                else if (Validation is Func<T, IEnumerable<string>>)
                    ValidateWithFunc(Validation as Func<T, IEnumerable<string>>, _value, errors);
                else
                {
                    var value = _value;

                    if (Validation is Func<T, Task<bool>>)
                        await ValidateWithFunc(Validation as Func<T, Task<bool>>, _value, errors);
                    else if (Validation is Func<T, Task<string>>)
                        await ValidateWithFunc(Validation as Func<T, Task<string>>, _value, errors);
                    else if (Validation is Func<T, Task<IEnumerable<string>>>)
                        await ValidateWithFunc(Validation as Func<T, Task<IEnumerable<string>>>, _value, errors);

                    changed = !EqualityComparer<T>.Default.Equals(value, _value);
                }

                // Run each validation attributes of the property targetted with `For`
                if (_validationAttrsFor is IEnumerable<ValidationAttribute> validationAttrs)
                {
                    foreach (var attr in validationAttrs)
                    {
                        ValidateWithAttribute(attr, _value, errors);
                    }
                }

                // required error (must be last, because it is least important!)
                var hasValue = HasValue(_value);
                if (Required)
                {
                    if (!hasValue && Touched)
                    {
                        errors.Add(RequiredError);
                    }
                }
            }
            finally
            {
                // If Value has changed while we were validating it, ignore results and exit
                if (!changed)
                {
                    // this must be called in any case, because even if Validation is null the user might have set Error and ErrorText manually
                    // if Error and ErrorText are set by the user, setting them here will have no effect.
                    ValidationErrors = errors;
                    Error = errors.Count > 0;
                    ErrorText = errors.FirstOrDefault();
                    Form?.Update(this);
                    StateHasChanged();
                }
            }
        }

        protected virtual bool HasValue(T value)
        {
            if (typeof(T) == typeof(string))
                return !string.IsNullOrWhiteSpace((string)(object)value);

            return value != null;
        }

        protected virtual void ValidateWithAttribute(ValidationAttribute attr, T value, List<string> errors)
        {
            if (!attr.IsValid(value))
                errors.Add(attr.ErrorMessage);
        }

        protected virtual void ValidateWithFunc(Func<T, bool> func, T value, List<string> errors)
        {
            try
            {
                if (!func(value))
                    errors.Add("Invalid");
            }
            catch (Exception e)
            {
                errors.Add("Error in validation func: " + e.Message);
            }
        }

        protected virtual void ValidateWithFunc(Func<T, string> func, T value, List<string> errors)
        {
            try
            {
                var error = func(value);
                if (error != null)
                    errors.Add(error);
            }
            catch (Exception e)
            {
                errors.Add("Error in validation func: " + e.Message);
            }
        }

        protected virtual void ValidateWithFunc(Func<T, IEnumerable<string>> func, T value, List<string> errors)
        {
            try
            {
                foreach (var error in func(value))
                    errors.Add(error);
            }
            catch (Exception e)
            {
                errors.Add("Error in validation func: " + e.Message);
            }
        }

        protected virtual async Task ValidateWithFunc(Func<T, Task<bool>> func, T value, List<string> errors)
        {
            try
            {
                if (!await func(value))
                    errors.Add("Invalid");
            }
            catch (Exception e)
            {
                errors.Add("Error in validation func: " + e.Message);
            }
        }

        protected virtual async Task ValidateWithFunc(Func<T, Task<string>> func, T value, List<string> errors)
        {
            try
            {
                var error = await func(value);
                if (error != null)
                    errors.Add(error);
            }
            catch (Exception e)
            {
                errors.Add("Error in validation func: " + e.Message);
            }
        }

        protected virtual async Task ValidateWithFunc(Func<T, Task<IEnumerable<string>>> func, T value, List<string> errors)
        {
            try
            {
                foreach (var error in await func(value))
                    errors.Add(error);
            }
            catch (Exception e)
            {
                errors.Add("Error in validation func: " + e.Message);
            }
        }

        public void Reset()
        {
            ResetValue();
            ResetValidation();
        }

        protected virtual void ResetValue()
        {
            /* to be overridden */
            _value = default;
            Touched = false;
            StateHasChanged();
        }

        public void ResetValidation()
        {
            Error = false;
            ValidationErrors.Clear();
            ErrorText = null;
            StateHasChanged();
        }

        #endregion


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
            if (_fieldIdentifier.FieldName != null)
            {
                EditContext?.NotifyFieldChanged(_fieldIdentifier);
            }
        }

        /// <summary>
        /// Specify an expression which returns the model's field for which validation messages should be displayed.
        /// Currently only string fields are supported.
        /// </summary>
#nullable enable
        [Parameter]
        public Expression<Func<T>>? For { get; set; }
#nullable disable

        /// <summary>
        /// Stores the list of validation attributes attached to the property targeted by <seealso cref="For"/>. If <seealso cref="For"/> is null, this property is null too.
        /// </summary>
#nullable enable
        private IEnumerable<ValidationAttribute>? _validationAttrsFor;
#nullable disable

        private void OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
        {
            if (EditContext != null)
            {
                var error_msgs = EditContext.GetValidationMessages(_fieldIdentifier).ToArray();
                Error = error_msgs.Length > 0;
                ErrorText = (Error ? error_msgs[0] : null);
                StateHasChanged();
            }
        }

        /// <summary>
        /// Points to a field of the model for which validation messages should be displayed.
        /// </summary>
        private FieldIdentifier _fieldIdentifier;

        /// <summary>
        /// To find out whether or not For parameter has changed we keep a separate reference
        /// </summary>
#nullable enable
        private Expression<Func<T>>? _currentFor;
#nullable disable

        /// <summary>
        /// To find out whether or not EditContext parameter has changed we keep a separate reference
        /// </summary>
#nullable enable
        private EditContext? _currentEditContext;
#nullable disable

        protected override void OnParametersSet()
        {
            if (EditContext != null && For != null)
            {
                if (For != _currentFor)
                {
                    // Extract validation attributes
                    // Sourced from https://stackoverflow.com/a/43076222/4839162
                    var expression = (MemberExpression)For.Body;
                    var propertyInfo = (PropertyInfo)expression.Member;
                    _validationAttrsFor = propertyInfo.GetCustomAttributes(typeof(ValidationAttribute), true).Cast<ValidationAttribute>();

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
        }

        private void DetachValidationStateChangedListener()
        {
            if (_currentEditContext != null)
                _currentEditContext.OnValidationStateChanged -= OnValidationStateChanged;
        }

        #endregion


        protected override Task OnInitializedAsync()
        {
            RegisterAsFormComponent();
            return base.OnInitializedAsync();
        }

        protected virtual void RegisterAsFormComponent()
        {
            Form?.Add(this);
        }

        /// <summary>
        /// Called to dispose this instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if called within <see cref="IDisposable.Dispose"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        void IDisposable.Dispose()
        {
            try
            {
                Form?.Remove(this);
            }
            catch { /* ignore */ }
            DetachValidationStateChangedListener();
            Dispose(disposing: true);
        }
    }
}
