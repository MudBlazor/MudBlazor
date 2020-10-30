
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Interfaces;

namespace MudBlazor
{


    public class MudTextField<T> : MudBaseTextField, IFormComponent, IDisposable
    {
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

        protected override void StringValueChanged(string text)
        {
            Value = Converter.Get(text);
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
                Text = Converter.Set(Value);
            }
        }

        #region Validation

        public List<string> ValidationErrors { get; set; } = new List<string>();

        /// <summary>
        /// A validation func or a validation attribute. Supported types are:
        /// Func<string, bool> ... will output the standard error message "Invalid" if false
        /// Func<string, string> ... outputs the result as error message, no error if null
        /// Func<string, IEnumerable<string>> ... outputs all the returned error messages, no error if empty
        /// System.ComponentModel.DataAnnotations.ValidationAttribute instances
        /// </summary>
        [Parameter]
        public object Validation { get; set; }

        protected override void OnBlurred(FocusEventArgs obj)
        {
            ValidateValue(Value);
            base.OnBlurred(obj);
        }

        /// <summary>
        /// Causes this component to validate its value
        /// </summary>
        public void Validate() => ValidateValue(Value);

        internal virtual void ValidateValue(T value)
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
                else if (Validation is Func<T, bool>)
                    ValidateWithFunc(Validation as Func<T, bool>, value);
                else if (Validation is Func<T, string>)
                    ValidateWithFunc(Validation as Func<T, string>, value);
                else if (Validation is Func<T, IEnumerable<string>>)
                    ValidateWithFunc(Validation as Func<T, IEnumerable<string>>, value);
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

        protected override Task OnInitializedAsync()
        {
            if (Standalone)
            {
                Form?.Add(this);
            }

            return base.OnInitializedAsync();
        }

        #region --> Blazor EditForm validation support

        /// <summary>
        /// This is the form validation context for Blazor's <EditForm></EditForm> component
        /// </summary>
        [CascadingParameter]
        EditContext EditContext { get; set; } = default!;

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