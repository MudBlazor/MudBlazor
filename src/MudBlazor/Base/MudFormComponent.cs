using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor.Interfaces;

namespace MudBlazor
{
    public class MudFormComponent<T> : MudComponentBase, IFormComponent, IDisposable
    {       
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

        protected virtual void OnConversionError(string error)
        {
            /* to be overridden */
        }

        /// <summary>
        /// True if the conversion from string to T failed
        /// </summary>
        public virtual bool ConversionError
        {
            get
            {
                /* to be overridden */
                return false;
            }
        }

        /// <summary>
        /// The error message of the conversion error from string to T. Null otherwise
        /// </summary>
        public virtual string ConversionErrorMessage
        {
            get
            {
                /* to be overridden */
                return null;
            }
        }

        /// <summary>
        /// True if the input has any of the following errors: An error set from outside, a conversion error or
        /// one or more validation errors
        /// </summary>
        public bool HasErrors => Error || ConversionError || ValidationErrors.Count > 0;


        #region MudForm Validation

        public List<string> ValidationErrors { get; set; } = new List<string>();

        /// <summary>
        /// A validation func or a validation attribute. Supported types are:
        /// <![CDATA[Func<T, bool>]]> ... will output the standard error message "Invalid" if false
        /// <![CDATA[Func<T, string>]]> ... outputs the result as error message, no error if null
        /// <![CDATA[Func<T, IEnumerable<string>>]]> ... outputs all the returned error messages, no error if empty
        /// <![CDATA[Func<T, Task<bool>>]]> ... will output the standard error message "Invalid" if false
        /// <![CDATA[Func<T, Task<string>>]]> ... outputs the result as error message, no error if null
        /// <![CDATA[Func<T, Task<IEnumerable<string>>>]]> ... outputs all the returned error messages, no error if empty
        /// System.ComponentModel.DataAnnotations.ValidationAttribute instances
        /// </summary>
        [Parameter]
        public object Validation { get; set; }

        /// <summary>
        /// This is the form component's value.
        /// </summary>
        protected T _value;

        /// <summary>
        /// Causes this component to validate its value
        /// </summary>
        public async Task Validate() => await ValidateValue(_value);

        internal async virtual Task ValidateValue(T value)
        {
            if (Form == null)
                return;
            ValidationErrors = new List<string>();
            try
            {
                var hasValue = HasValue(value);
                if (Required)
                {
                    if (!hasValue)
                    {
                        ValidationErrors.Add(RequiredError);
                        return; // no need to call validation funcs if required value doesn't exist. we already have a required error.
                    }
                    // we have a required value, proceed to the validation funcs
                }
                else
                {
                    if (!hasValue)
                        return; // if nothing has been entered, we return OK without calling validation funcs
                    // proceed to the validation funcs
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

        protected virtual bool HasValue(T value)
        {
            var hasValue = true;
            if (typeof(T) == typeof(string))
                hasValue = !string.IsNullOrWhiteSpace((string)(object)value);
            else if (value == null)
                hasValue = false;
            return hasValue;
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
            ResetValue();
            ResetValidation();
        }

        protected virtual void ResetValue()
        {
            /* to be overridden */
            _value = default;
        }

        public void ResetValidation()
        {
            Error = false;
            ValidationErrors.Clear();
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
            //ParentForm?.Remove(this);
            DetachValidationStateChangedListener();
            Dispose(disposing: true);
        }
    }
}
