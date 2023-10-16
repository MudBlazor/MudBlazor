using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor.Interfaces;
using static System.String;

namespace MudBlazor
{
#nullable enable
    public abstract class MudFormComponent<T, U> : MudComponentBase, IFormComponent, IDisposable
    {
        private Converter<T, U> _converter;

        protected MudFormComponent(Converter<T, U> converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _converter.OnError = OnConversionError;
        }

        [CascadingParameter]
        internal IForm? Form { get; set; }

        /// <summary>
        /// If true, this is a top-level form component. If false, this input is a sub-component of another input (i.e. TextField, Select, etc).
        /// If it is sub-component, it will NOT do form validation!!
        /// </summary>
        [CascadingParameter(Name = "SubscribeToParentForm")]
        internal bool SubscribeToParentForm { get; set; } = true;

        /// <summary>
        /// If true, this form input is required to be filled out.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public bool Required { get; set; }

        /// <summary>
        /// The error text that will be displayed if the input is not filled out but required.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public string RequiredError { get; set; } = "Required";

        /// <summary>
        /// The ErrorText that will be displayed if Error true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public string? ErrorText { get; set; }

        /// <summary>
        /// If true, the label will be displayed in an error state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public bool Error { get; set; }

        /// <summary>
        /// The ErrorId that will be used by aria-describedby if Error true
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public string? ErrorId { get; set; }

        /// <summary>
        /// The generic converter of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Converter<T, U> Converter
        {
            get => _converter;
            set => SetConverter(value);
        }

        protected virtual bool SetConverter(Converter<T, U> value)
        {
            var changed = _converter != value;
            if (changed)
            {
                _converter = value ?? throw new ArgumentNullException(nameof(value));   // converter is mandatory at all times
                _converter.OnError = OnConversionError;
            }
            return changed;
        }

        /// <summary>
        /// The culture of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public CultureInfo Culture
        {
            get => _converter.Culture;
            set => SetCulture(value);
        }

        protected virtual bool SetCulture(CultureInfo value)
        {
            var changed = _converter.Culture != value;
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
        [MemberNotNullWhen(true, nameof(ConversionErrorMessage))]
        public bool ConversionError => _converter.GetError;

        /// <summary>
        /// The error message of the conversion error from string to T. Null otherwise
        /// </summary>
        public string? ConversionErrorMessage => _converter.GetErrorMessage;

        /// <summary>
        /// True if the input has any of the following errors: An error set from outside, a conversion error or
        /// one or more validation errors
        /// </summary>
        public bool HasErrors => Error || ConversionError || ValidationErrors.Count > 0;

        /// <summary>
        /// Return the validation error text or the conversion error message.
        /// </summary>
        /// <returns>Error text/message</returns>
        public string? GetErrorText()
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

        public List<string> ValidationErrors { get; set; } = new();

        /// <summary>
        /// A validation func or a validation attribute. Supported types are:
        /// <para>Func&lt;T, bool&gt; ... will output the standard error message "Invalid" if false</para>
        /// <para>Func&lt;T, string&gt; ... outputs the result as error message, no error if null </para>
        /// <para>Func&lt;T, IEnumerable&lt; string &gt;&gt; ... outputs all the returned error messages, no error if empty</para>
        /// <para>Func&lt;object, string, IEnumerable&lt; string &gt;&gt; input Form.Model, Full Path of Member ... outputs all the returned error messages, no error if empty</para>
        /// <para>Func&lt;T, Task&lt; bool &gt;&gt; ... will output the standard error message "Invalid" if false</para>
        /// <para>Func&lt;T, Task&lt; string &gt;&gt; ... outputs the result as error message, no error if null</para>
        /// <para>Func&lt;T, Task&lt;IEnumerable&lt; string &gt;&gt;&gt; ... outputs all the returned error messages, no error if empty</para>
        /// <para>Func&lt;object, string, Task&lt;IEnumerable&lt; string &gt;&gt;&gt; input Form.Model, Full Path of Member ... outputs all the returned error messages, no error if empty</para>
        /// <para>System.ComponentModel.DataAnnotations.ValidationAttribute instances</para>
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public object? Validation { get; set; }

        /// <summary>
        /// This is the form component's value.
        /// </summary>
        protected T? _value;

        protected Task BeginValidationAfterAsync(Task task)
        {
            Func<Task> execute = async () =>
            {
                var value = _value;

                await task;

                // we validate only if the value hasn't changed while we waited for task.
                // if it has in fact changed, another validate call will follow anyway
                if (EqualityComparer<T>.Default.Equals(value, _value))
                {
                    await BeginValidateAsync();
                }
            };

            return execute();
        }

        // These are the fire-and-forget methods to launch an async validation process.
        // After each async step, we make sure the current Value of the component has not changed while
        // async code was executed to avoid race condition which could lead to incorrect validation results.
        [Obsolete($"Use {nameof(BeginValidationAfterAsync)} instead, this will be removed in v7")]
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

        protected Task BeginValidateAsync()
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

            return execute();
        }

        [Obsolete($"Use {nameof(BeginValidateAsync)} instead, this will be removed in v7")]
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
        /// Cause this component to validate its value.
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
                if (Validation is ValidationAttribute validationAttribute)
                    ValidateWithAttribute(validationAttribute, _value, errors);
                else if (Validation is Func<T?, bool> funcBooleanValidation)
                    ValidateWithFunc(funcBooleanValidation, _value, errors);
                else if (Validation is Func<T?, string?> funcStringValidation)
                    ValidateWithFunc(funcStringValidation, _value, errors);
                else if (Validation is Func<T?, IEnumerable<string?>> funcEnumerableValidation)
                    ValidateWithFunc(funcEnumerableValidation, _value, errors);
                else if (Validation is Func<object, string, IEnumerable<string?>> funcModelWithFullPathOfMember)
                    ValidateModelWithFullPathOfMember(funcModelWithFullPathOfMember, errors);
                else
                {
                    var value = _value;

                    if (Validation is Func<T?, Task<bool>> funcTaskBooleanValidation)
                        await ValidateWithFunc(funcTaskBooleanValidation, _value, errors);
                    else if (Validation is Func<T?, Task<string?>> funcTaskStringValidation)
                        await ValidateWithFunc(funcTaskStringValidation, _value, errors);
                    else if (Validation is Func<T?, Task<IEnumerable<string?>>> funcTaskEnumerableValidation)
                        await ValidateWithFunc(funcTaskEnumerableValidation, _value, errors);
                    else if (Validation is Func<object, string, Task<IEnumerable<string?>>> funcTaskModelWithFullPathOfMember)
                        await ValidateModelWithFullPathOfMember(funcTaskModelWithFullPathOfMember, errors);

                    changed = !EqualityComparer<T>.Default.Equals(value, _value);
                }

                // Run each validation attributes of the property targeted with `For`
                if (_validationAttrsFor is not null)
                {
                    foreach (var attr in _validationAttrsFor)
                    {
                        ValidateWithAttribute(attr, _value, errors);
                    }
                }

                // required error (must be last, because it is least important!)
                if (Required)
                {
                    if (Touched && !HasValue(_value))
                        errors.Add(RequiredError);
                }
            }
            finally
            {
                // If Value has changed while we were validating it, ignore results and exit
                if (!changed)
                {
                    // this must be called in any case, because even if Validation is null the user might have set Error and ErrorText manually
                    // if Error and ErrorText are set by the user, setting them here will have no effect.
                    // if Error, create an error id that can be used by aria-describedby on input control
                    ValidationErrors = errors;
                    Error = errors.Count > 0;
                    ErrorText = errors.FirstOrDefault();
                    ErrorId = HasErrors ? Guid.NewGuid().ToString() : null;
                    Form?.Update(this);
                    StateHasChanged();
                }
            }
        }

        protected virtual bool HasValue(T? value)
        {
            if (value is string valueString)
                return !IsNullOrWhiteSpace(valueString);

            return value is not null;
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "In the context of EditContext.Model / FieldIdentifier.Model they won't get trimmed.")]
        protected virtual void ValidateWithAttribute(ValidationAttribute attr, T? value, List<string> errors)
        {
            try
            {
                // The validation context is applied either on the `EditContext.Model`, '_fieldIdentifier.Model', or `this` as a stub subject.
                // Complex validation with fields references (like `CompareAttribute`) should use an EditContext or For when not using EditContext.
                var validationContextSubject = EditContext?.Model ?? _fieldIdentifier.Model ?? this;
                var validationContext = new ValidationContext(validationContextSubject);
                if (validationContext.MemberName is null && !IsNullOrEmpty(_fieldIdentifier.FieldName))
                    validationContext.MemberName = _fieldIdentifier.FieldName;
                var validationResult = attr.GetValidationResult(value, validationContext);
                if (validationResult != ValidationResult.Success)
                {
                    if (!IsNullOrEmpty(validationResult?.ErrorMessage))
                    {
                        errors.Add(validationResult.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                // Maybe conditionally add full error message if `IWebAssemblyHostEnvironment.IsDevelopment()`
                // Or log using proper logger.
                errors.Add($"An unhandled exception occurred: {e.Message}");
            }
        }

        protected virtual void ValidateWithFunc(Func<T?, bool> func, T? value, List<string> errors)
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

        protected virtual void ValidateWithFunc(Func<T?, string?> func, T? value, List<string> errors)
        {
            try
            {
                var error = func(value);
                if (!IsNullOrEmpty(error))
                    errors.Add(error);
            }
            catch (Exception e)
            {
                errors.Add("Error in validation func: " + e.Message);
            }
        }

        protected virtual void ValidateWithFunc(Func<T?, IEnumerable<string?>> func, T? value, List<string> errors)
        {
            try
            {
                foreach (var error in func(value))
                    if (!IsNullOrEmpty(error))
                        errors.Add(error);
            }
            catch (Exception e)
            {
                errors.Add("Error in validation func: " + e.Message);
            }
        }

        protected virtual void ValidateModelWithFullPathOfMember(Func<object, string, IEnumerable<string?>> func, List<string> errors)
        {
            try
            {
                if (Form?.Model is null)
                {
                    return;
                }

                if (For is null)
                {
                    errors.Add($"For is null, please set parameter For on the form input component of type {GetType().Name}");
                    return;
                }

                foreach (var error in func(Form.Model, For.GetFullPathOfMember()))
                    if (!IsNullOrEmpty(error))
                        errors.Add(error);
            }
            catch (Exception e)
            {
                errors.Add("Error in validation func: " + e.Message);
            }
        }

        protected virtual async Task ValidateWithFunc(Func<T?, Task<bool>> func, T? value, List<string> errors)
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

        protected virtual async Task ValidateWithFunc(Func<T?, Task<string?>> func, T? value, List<string> errors)
        {
            try
            {
                var error = await func(value);
                if (!IsNullOrEmpty(error))
                    errors.Add(error);
            }
            catch (Exception e)
            {
                errors.Add("Error in validation func: " + e.Message);
            }
        }

        protected virtual async Task ValidateWithFunc(Func<T?, Task<IEnumerable<string?>>> func, T? value, List<string> errors)
        {
            try
            {
                foreach (var error in await func(value))
                    if (!IsNullOrEmpty(error))
                        errors.Add(error);
            }
            catch (Exception e)
            {
                errors.Add("Error in validation func: " + e.Message);
            }
        }

        protected virtual async Task ValidateModelWithFullPathOfMember(Func<object, string, Task<IEnumerable<string?>>> func, List<string> errors)
        {
            try
            {
                if (Form?.Model is null)
                {
                    return;
                }

                if (For is null)
                {
                    errors.Add($"For is null, please set parameter For on the form input component of type {GetType().Name}");
                    return;
                }

                foreach (var error in await func(Form.Model, For.GetFullPathOfMember()))
                    if (!IsNullOrEmpty(error))
                        errors.Add(error);
            }
            catch (Exception e)
            {
                errors.Add("Error in validation func: " + e.Message);
            }
        }

        /// <summary>
        /// Notify the Form that a field has changed if SubscribeToParentForm is true
        /// </summary>
        protected void FieldChanged(object? newValue)
        {
            if (SubscribeToParentForm)
                Form?.FieldChanged(this, newValue);
        }

        /// <summary>
        /// Reset the value and the validation.
        /// </summary>
        [Obsolete($"Use {nameof(ResetValueAsync)} instead. This will be removed in v7")]
        [ExcludeFromCodeCoverage]
        public void Reset()
        {
            ResetValue();
            ResetValidation();
        }

        /// <summary>
        /// Reset the value and the validation.
        /// </summary>
        public async Task ResetAsync()
        {
            await ResetValueAsync();
            ResetValidation();
        }

        [Obsolete($"Use {nameof(ResetValueAsync)} instead. This will be removed in v7")]
        [ExcludeFromCodeCoverage]
        protected virtual void ResetValue()
        {
            /* to be overridden */
            _value = default;
            Touched = false;
            StateHasChanged();
        }

        protected virtual Task ResetValueAsync()
        {
            /* to be overridden */
            _value = default;
            Touched = false;
            StateHasChanged();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Reset the validation.
        /// </summary>
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
        private EditContext? EditContext { get; set; } = default!;

        /// <summary>
        /// Triggers field to be validated.
        /// </summary>
        internal void EditFormValidate()
        {
            if (!IsNullOrEmpty(_fieldIdentifier.FieldName))
            {
                EditContext?.NotifyFieldChanged(_fieldIdentifier);
            }
        }

        /// <summary>
        /// Specify an expression which returns the model's field for which validation messages should be displayed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public Expression<Func<T>>? For { get; set; }

        [MemberNotNullWhen(false, nameof(For))]
        public bool IsForNull => For is null;

        /// <summary>
        /// Stores the list of validation attributes attached to the property targeted by <seealso cref="For"/>. If <seealso cref="For"/> is null, this property is null too.
        /// </summary>
        private IEnumerable<ValidationAttribute>? _validationAttrsFor;

        private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
        {
            if (EditContext is not null && !_fieldIdentifier.Equals(default(FieldIdentifier)))
            {
                var errorMessages = EditContext.GetValidationMessages(_fieldIdentifier).ToArray();
                Error = errorMessages.Length > 0;
                ErrorText = Error ? errorMessages[0] : null;

                ValidationErrors.Clear();
                ValidationErrors.AddRange(errorMessages);

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
        private Expression<Func<T>>? _currentFor;

        /// <summary>
        /// To find out whether or not EditContext parameter has changed we keep a separate reference
        /// </summary>
        private EditContext? _currentEditContext;

        protected override void OnParametersSet()
        {
            if (For is not null && For != _currentFor)
            {
                // Extract validation attributes
                // Sourced from https://stackoverflow.com/a/43076222/4839162
                // and also https://stackoverflow.com/questions/59407225/getting-a-custom-attribute-from-a-property-using-an-expression
                var expression = (MemberExpression)For.Body;
                var propertyInfo = expression.Expression?.Type.GetProperty(expression.Member.Name);
                _validationAttrsFor = propertyInfo?.GetCustomAttributes(typeof(ValidationAttribute), true).Cast<ValidationAttribute>();

                _fieldIdentifier = FieldIdentifier.Create(For);
                _currentFor = For;
            }

            if (EditContext is not null && EditContext != _currentEditContext)
            {
                DetachValidationStateChangedListener();
                EditContext.OnValidationStateChanged += OnValidationStateChanged;
                _currentEditContext = EditContext;
            }
        }

        private void DetachValidationStateChangedListener()
        {
            if (_currentEditContext is not null)
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
            if (SubscribeToParentForm)
            {
                Form?.Add(this);
            }
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

        void IFormComponent.StateHasChanged() => StateHasChanged();
    }
}
