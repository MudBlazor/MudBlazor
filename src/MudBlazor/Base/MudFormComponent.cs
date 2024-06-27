// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor.Interfaces;
using static System.String;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a base class for designing form input components.
    /// </summary>
    /// <typeparam name="T">The complex type managed by this input.</typeparam>
    /// <typeparam name="U">The value type managed by this input.</typeparam>
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
        /// Requires an input value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, an error with the text in <see cref="RequiredError"/> will be shown during validation if no input was given.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public bool Required { get; set; }

        /// <summary>
        /// The text displayed during validation if no input was given.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>"Required"</c>.  This text is only shown when <see cref="Required"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public string RequiredError { get; set; } = "Required";

        /// <summary>
        /// The text displayed if the <see cref="Error"/> property is <c>true</c>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public string? ErrorText { get; set; }

        /// <summary>
        /// Displays an error.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the text in <see cref="ErrorText"/> is displayed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public bool Error { get; set; }

        /// <summary>
        /// The ID of the error description element, for use by <c>aria-describedby</c> when <see cref="Error"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set and the <see cref="Error"/> property is <c>true</c>, an <c>aria-describedby</c> attribute is rendered to improve accessibility for users.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public string? ErrorId { get; set; }

        /// <summary>
        /// The type converter for this input.
        /// </summary>
        /// <remarks>
        /// This property provides a way to customize conversions between <typeparamref name="T"/> objects and <typeparamref name="U"/> values.  If no converter is specified, a default will be chosen based on the kind of input.
        /// </remarks>
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
        /// The culture used to format and interpret values such as dates and currency.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="CultureInfo.InvariantCulture"/>.
        /// </remarks>
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
        /// Indicates a problem has occurred during conversion.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the <see cref="Converter"/> was unable to convert values, usually due to invalid input.
        /// </remarks>
        [MemberNotNullWhen(true, nameof(ConversionErrorMessage))]
        public bool ConversionError => _converter.GetError;

        /// <summary>
        /// The error describing why type conversion failed.
        /// </summary>
        /// <remarks>
        /// When set, returns the reason that the <see cref="Converter"/> was unable to convert values, usually due to invalid input.
        /// </remarks>
        public string? ConversionErrorMessage => _converter.GetErrorMessage;

        /// <summary>
        /// Indicates any error, conversion error, or validation error with this input.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the <see cref="Error"/> property is <c>true</c>, or <see cref="ConversionError"/> is <c>true</c>, or one or more <see cref="ValidationErrors"/> exists.
        /// </remarks>
        public bool HasErrors => Error || ConversionError || ValidationErrors.Count > 0;

        /// <summary>
        /// The current error or conversion error.
        /// </summary>
        /// <returns>
        /// This property returns the value in <see cref="ErrorText"/> or <see cref="ConversionErrorMessage"/>.
        /// </returns>
        public string? GetErrorText()
        {
            // ErrorText is either set from outside or the first validation error
            if (!IsNullOrWhiteSpace(ErrorText))
            {
                return ErrorText;
            }

            if (!IsNullOrWhiteSpace(ConversionErrorMessage))
            {
                return ConversionErrorMessage;
            }

            return null;
        }

        /// <summary>
        /// Indicates whether the user has interacted with this input or the focus has been released.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the user has performed input, or focus has moved away from this input.  This property is typically used to show the <see cref="RequiredError"/> text only after the user has interacted with this input.
        /// </remarks>
        public bool Touched { get; protected set; }

        #region MudForm Validation

        /// <summary>
        /// The list of problems with the current input value.
        /// </summary>
        /// <remarks>
        /// When using a <see cref="MudForm"/>, this property is updated when validation has been performed.  Use the <see cref="Validation"/> property to control what validations are performed.
        /// </remarks>
        public List<string> ValidationErrors { get; set; } = new();

        /// <summary>
        /// The function used to detect problems with the input.
        /// </summary>
        /// <remarks>
        /// When using a <see cref="MudForm"/>, this property can be any of several kinds of functions:
        /// <para>
        /// 1. A <c>Func&lt;T,bool&gt;</c> or <c>Func&lt;T,Task&lt;bool&gt;&gt;</c> function.  Returns <c>true</c> if valid.  When <c>false</c>, a standard <c>"Invalid"</c> message is shown.
        /// </para>
        /// <para>
        /// 2. A <c>Func&lt;T,string&gt;</c> or <c>Func&lt;T,Task&lt;string&gt;&gt;</c> function.  Returns <c>null</c> if valid, or a string explaining the error.
        /// </para>
        /// <para>
        /// 3. A <c>Func&lt;T,IEnumerable&lt;string&gt;&gt;</c> or <c>Func&lt;T,Task&lt;IEnumerable&lt;string&gt;&gt;&gt;</c> function.  Returns an empty list if valid, or a list of validation errors.
        /// </para>
        /// <para>
        /// 3. A <c>Func&lt;object,string,IEnumerable&lt;string&gt;&gt;</c> or <c>Func&lt;object,string,Task&lt;IEnumerable&lt;string&gt;&gt;&gt;</c> function.  Given the form model and path to the member, returns an empty list if valid, or a list of validation errors.
        /// </para>
        /// <para>
        /// 4. A <see cref="ValidationAttribute"/> object.
        /// </para>
        /// </remarks>
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

        /// <summary>
        /// Causes validation to be performed for this input.
        /// </summary>
        /// <remarks>
        /// When using a <see cref="MudForm"/>, the input is validated via the function set in the <see cref="Validation"/> property.
        /// </remarks>
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
                {
                    errors.Add(ConversionErrorMessage);
                }
                // validation errors
                if (Validation is ValidationAttribute validationAttribute)
                {
                    ValidateWithAttribute(validationAttribute, _value, errors);
                }
                else if (Validation is Func<T?, bool> funcBooleanValidation)
                {
                    ValidateWithFunc(funcBooleanValidation, _value, errors);
                }
                else if (Validation is Func<T?, string?> funcStringValidation)
                {
                    ValidateWithFunc(funcStringValidation, _value, errors);
                }
                else if (Validation is Func<T?, IEnumerable<string?>> funcEnumerableValidation)
                {
                    ValidateWithFunc(funcEnumerableValidation, _value, errors);
                }
                else if (Validation is Func<object, string, IEnumerable<string?>> funcModelWithFullPathOfMember)
                {
                    ValidateModelWithFullPathOfMember(funcModelWithFullPathOfMember, errors);
                }
                else
                {
                    var value = _value;

                    if (Validation is Func<T?, Task<bool>> funcTaskBooleanValidation)
                    {
                        await ValidateWithFunc(funcTaskBooleanValidation, _value, errors);
                    }
                    else if (Validation is Func<T?, Task<string?>> funcTaskStringValidation)
                    {
                        await ValidateWithFunc(funcTaskStringValidation, _value, errors);
                    }
                    else if (Validation is Func<T?, Task<IEnumerable<string?>>> funcTaskEnumerableValidation)
                    {
                        await ValidateWithFunc(funcTaskEnumerableValidation, _value, errors);
                    }
                    else if (Validation is Func<object, string, Task<IEnumerable<string?>>> funcTaskModelWithFullPathOfMember)
                    {
                        await ValidateModelWithFullPathOfMember(funcTaskModelWithFullPathOfMember, errors);
                    }

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
            {
                return !IsNullOrWhiteSpace(valueString);
            }

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
                {
                    validationContext.MemberName = _fieldIdentifier.FieldName;
                }

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
                {
                    errors.Add("Invalid");
                }
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
                {
                    errors.Add(error);
                }
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
                {
                    if (!IsNullOrEmpty(error))
                    {
                        errors.Add(error);
                    }
                }
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
                {
                    if (!IsNullOrEmpty(error))
                    {
                        errors.Add(error);
                    }
                }
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
                {
                    errors.Add("Invalid");
                }
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
                {
                    errors.Add(error);
                }
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
                {
                    if (!IsNullOrEmpty(error))
                    {
                        errors.Add(error);
                    }
                }
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
                {
                    if (!IsNullOrEmpty(error))
                    {
                        errors.Add(error);
                    }
                }
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
            {
                Form?.FieldChanged(this, newValue);
            }
        }

        /// <summary>
        /// Clears the input and any validation errors.
        /// </summary>
        /// <remarks>
        /// When called, the <c>Value</c>, <see cref="Error"/>, <see cref="ErrorText"/>, and <see cref="ValidationErrors"/> properties are all reset.
        /// </remarks>
        public async Task ResetAsync()
        {
            await ResetValueAsync();
            ResetValidation();
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
        /// Clears any validation errors.
        /// </summary>
        /// <remarks>
        /// When called, the <see cref="Error"/>, <see cref="ErrorText"/>, and <see cref="ValidationErrors"/> properties are all reset.
        /// </remarks>
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
        /// The context used to perform validation.
        /// </summary>
        /// <remarks>
        /// When using an <see cref="EditForm"/>, gets a context used to perform validation.
        /// </remarks>
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
        /// The model field containing validation attributes.
        /// </summary>
        /// <remarks>
        /// When using an <see cref="EditForm"/>, this property is used to find data annotation validation attributes such as <see cref="MaxLengthAttribute"/> used to perform validation.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public Expression<Func<T>>? For { get; set; }

        /// <summary>
        /// Indicates whether the <see cref="For"/> property is <c>null</c>.
        /// </summary>
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
            base.OnParametersSet();
            if (For is not null && For != _currentFor)
            {
                // Extract validation attributes
                // Sourced from https://stackoverflow.com/a/43076222/4839162
                // and also https://stackoverflow.com/questions/59407225/getting-a-custom-attribute-from-a-property-using-an-expression
                var expression = (MemberExpression)For.Body;

                // Currently we have no solution for this which is trimming incompatible
                // A possible solution is to use source gen
#pragma warning disable IL2075
                var propertyInfo = expression.Expression?.Type.GetProperty(expression.Member.Name);
#pragma warning restore IL2075
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
    }
}
