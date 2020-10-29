using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MudBlazor
{
    public abstract class MudBaseInput<TValue> : MudComponentBase, IFormComponent
    {

        private TValue _value;

        /// <summary>
        /// Fired when the Value property changes. 
        /// </summary>
        [Parameter] public EventCallback<TValue> ValueChanged { get; set; }

        /// <summary>
        /// The value of this input element. This property is two-way bindable.
        /// </summary>
        [Parameter]
        public TValue Value
        {
            get => _value;
            set
            {
                if (!object.Equals(value,_value))
                {
                    _value = value;
                    ValidateValue(value);
                    ValueChanged.InvokeAsync(value);
                }
            }
        }

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

        #region Validation

        public List<string> ValidationErrors { get; set; } = new List<string>();

        /// <summary>
        /// A validation func or a validation attribute. Supported types are:
        /// Func<string, bool> ... will output the standard error message "Invalid" if false
        /// Func<string, string> ... outputs the result as error message, no error if null
        /// Func<string, IEnumerable<string>> ... outputs all the returned error messages, no error if empty
        /// System.ComponentModel.DataAnnotations.ValidationAttribute instances
        /// </summary>
        [Parameter] public object Validation { get; set; }


        /// <summary>
        /// Causes this component to validate its value
        /// </summary>
        public void Validate() => ValidateValue(Value);

        internal virtual void ValidateValue(TValue value)
        {
            if (Form == null || !Standalone)
                return;
            ValidationErrors = new List<string>();
            try
            {
                if (Required && value == null)
                {
                    ValidationErrors.Add(RequiredError);
                    return;
                }
                if (Validation is ValidationAttribute)
                    ValidateWithAttribute(Validation as ValidationAttribute, value);
                else if (Validation is Func<TValue, bool>)
                    ValidateWithFunc(Validation as Func<TValue, bool>, value);
                else if (Validation is Func<TValue, string>)
                    ValidateWithFunc(Validation as Func<TValue, string>, value);
                else if (Validation is Func<TValue, IEnumerable<string>>)
                    ValidateWithFunc(Validation as Func<TValue, IEnumerable<string>>, value);
            }
            finally
            {
                // this must be called in any case, because even if Validation is null the user might have set Error and ErrorText manually
                // if Error and ErrorText are set by the user, setting them here will have no effect. 
                Error = ValidationErrors.Count > 0;
                ErrorText = ValidationErrors.FirstOrDefault();
                Form.Update(this);
            }
        }

        protected virtual void ValidateWithAttribute(ValidationAttribute attr, TValue value)
        {
            if (attr.IsValid(value))
                return;
            ValidationErrors.Add(attr.ErrorMessage);
        }

        protected virtual void ValidateWithFunc(Func<TValue, bool> func, TValue value)
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

        protected virtual void ValidateWithFunc(Func<TValue, string> func, TValue value)
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

        protected virtual void ValidateWithFunc(Func<TValue, IEnumerable<string>> func, TValue value)
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
    }
}
