using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// A component for collecting and validating user input. Every input derived from MudFormComponent 
    /// within it is monitored and validated.
    /// </summary>
    public partial class MudForm : MudComponentBase, IDisposable, IForm
    {
        protected string Classname =>
            new CssBuilder("mud-form")
            .AddClass($"gap-{Spacing}", Spacing >= 0)
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The content within this form..
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Form.ValidatedData)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Whether all inputs and child forms passed validation.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When this value changes, <see cref="IsValidChanged"/> occurs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Form.ValidationResult)]
        public bool IsValid
        {
            get => _valid && ChildForms.All(x => x.IsValid);
            set
            {
                _valid = value;
            }
        }

        // Note: w/o any children the form is automatically valid.
        // It stays valid, as long as non-required fields are added or
        // a required field is added or the user touches a field that fails validation.
        private bool _valid = true;

        private void SetIsValid(bool value)
        {
            if (IsValid == value)
                return;
            IsValid = value;
            IsValidChanged.InvokeAsync(IsValid).CatchAndLog();
        }

        // Note: w/o any children the form is automatically valid.
        // It stays valid, as long as non-required fields are added or
        // a required field is added or the user touches a field that fails validation.

        /// <summary>
        /// Whether any input's value has changed.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, an input has changed in this form or any child forms.  Becomes <c>false</c> when input values have been reset.  When this value changes, <see cref="IsTouchedChanged"/> occurs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public bool IsTouched { get => _touched; set {/* readonly parameter! */ } }

        private bool _touched = false;

        /// <summary>
        /// Prevents the user from interacting with this form.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public bool Disabled { get; set; }

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }

        protected bool GetDisabledState() => Disabled || ParentDisabled;

        /// <summary>
        /// Prevents the user from changing any inputs.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public bool ReadOnly { get; set; }

        [CascadingParameter(Name = "ParentReadOnly")]
        private bool ParentReadOnly { get; set; }

        protected bool GetReadOnlyState() => ReadOnly || ParentReadOnly;

        /// <summary>
        /// The delay, in milliseconds, before performing validation.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>300</c> (300 milliseconds).  This delay can improve rendering performance for larger forms with inputs which set <see cref="MudBaseInput{T}.Immediate"/> to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public int ValidationDelay { get; set; } = 300;

        /// <summary>
        /// Prevents child components from rendering when <see cref="IsValid"/> changes.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, rendering performance may improve for larger forms and older devices.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public bool SuppressRenderingOnValidation { get; set; } = false;

        /// <summary>
        /// Prevents this form from being submitted when <c>Enter</c> is pressed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>false</c>, the form will submit when <c>Enter</c> is pressed, and any parent dialog will close.  See: 
        /// <see href="https://www.w3.org/TR/2018/SPSD-html5-20180327/forms.html#implicit-submission">Implicit Form Submission</see>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public bool SuppressImplicitSubmission { get; set; } = true;

        /// <summary>
        /// The amount of spacing between input components, in increments of <c>4px</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  A spacing of <c>1</c> means <c>4px</c>, <c>2</c> means <c>8px</c>, and so on.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public int Spacing { set; get; }

        /// <summary>
        /// Occurs when <see cref="IsValid"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> IsValidChanged { get; set; }

        /// <summary>
        /// Occurs when <see cref="IsTouched"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> IsTouchedChanged { get; set; }

        /// <summary>
        /// Occurs when an <see cref="IFormComponent"/> within this form has changed.
        /// </summary>
        [Parameter]
        public EventCallback<FormFieldChangedEventArgs> FieldChanged { get; set; }

        // keeps track of validation. if the input was validated at least once the value will be true
        protected HashSet<IFormComponent> _formControls = new();
        protected HashSet<string> _errors = new();

        /// <summary>
        /// The default function or attribute used to validate form components which cannot validate themselves.
        /// </summary>
        /// <remarks>
        /// Supported values are:
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
        /// 4. A <see cref="System.ComponentModel.DataAnnotations.ValidationAttribute"/> object.
        /// </para>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public object Validation { get; set; }

        /// <summary>
        /// Overrides input validation with the function or attribute in <see cref="Validation"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public bool? OverrideFieldValidation { get; set; }

        /// <summary>
        /// The validation errors for inputs within this form.
        /// </summary>
        /// <remarks>
        /// When this property changes, <see cref="ErrorsChanged"/> occurs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Form.ValidationResult)]
        public string[] Errors
        {
            get => _errors.ToArray();
            set { /* readonly */ }
        }

        /// <summary>
        /// Occurs when <see cref="Errors"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<string[]> ErrorsChanged { get; set; }

        /// <summary>
        /// The model populated by this form.
        /// </summary>
        /// <remarks>
        /// Properties of this model are typically linked to form input components via their <see cref="MudFormComponent{T, U}.For"/>.
        /// </remarks>
#nullable enable
        [Parameter]
        [Category(CategoryTypes.Form.ValidatedData)]
        public object? Model { get; set; }
#nullable disable

        private HashSet<MudForm> ChildForms { get; set; } = new HashSet<MudForm>();

        [CascadingParameter] private MudForm ParentMudForm { get; set; }

        void IForm.FieldChanged(IFormComponent formControl, object newValue)
        {
            FieldChanged.InvokeAsync(new FormFieldChangedEventArgs { Field = formControl, NewValue = newValue }).CatchAndLog();
        }

        void IForm.Add(IFormComponent formControl)
        {
            if (formControl.Required)
                SetIsValid(false);
            _formControls.Add(formControl);
            SetDefaultControlValidation(formControl);
        }

        void IForm.Remove(IFormComponent formControl)
        {
            _formControls.Remove(formControl);
        }

        private Timer _timer;

        /// <summary>
        /// Called by any input of the form to signal that its value changed. 
        /// </summary>
        /// <param name="formControl"></param>
        void IForm.Update(IFormComponent formControl)
        {
            EvaluateForm();
        }

        private void EvaluateForm(bool debounce = true)
        {
            _timer?.Dispose();
            if (debounce && ValidationDelay > 0)
                _timer = new Timer(OnTimerComplete, null, ValidationDelay, Timeout.Infinite);
            else
                _ = OnEvaluateForm();
        }

        private void OnTimerComplete(object stateInfo)
        {
            try
            {
                InvokeAsync(OnEvaluateForm);
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured while executing {nameof(OnEvaluateForm)}: {e.Message}");
            }
        }

        private bool _shouldRender = true; // <-- default is true, we need the form children to render

        protected async Task OnEvaluateForm()
        {
            _errors.Clear();
            foreach (var error in _formControls.SelectMany(control => control.ValidationErrors))
                _errors.Add(error);
            // form can only be valid if:
            // - none have an error
            // - all required fields have been touched (and thus validated)
            var no_errors = _formControls.All(x => x.HasErrors == false);
            var required_all_touched = _formControls.Where(x => x.Required).All(x => x.Touched);
            var valid = no_errors && required_all_touched;

            var old_touched = _touched;
            _touched = _formControls.Any(x => x.Touched);
            try
            {
                _shouldRender = false;
                SetIsValid(valid);
                await ErrorsChanged.InvokeAsync(Errors);
                if (old_touched != _touched)
                    await IsTouchedChanged.InvokeAsync(_touched);
            }
            finally
            {
                _shouldRender = true;
            }
        }

        protected override bool ShouldRender()
        {
            if (!SuppressRenderingOnValidation)
                return true;
            return _shouldRender;
        }

        /// <summary>
        /// Forces a validation of all form controls (including in child forms).
        /// </summary>
        /// <remarks>
        /// Validation will occur even if form controls haven't changed yet.
        /// </remarks>
        public async Task Validate()
        {
            await Task.WhenAll(_formControls.Select(x => x.Validate()));

            if (ChildForms.Count > 0)
            {
                await Task.WhenAll(ChildForms.Select(x => x.Validate()));
            }

            EvaluateForm(debounce: false);
        }

        /// <summary>
        /// Resets all form controls and resets their validation state.
        /// </summary>
        /// <remarks>
        /// Any existing value in any form input component will be cleared.
        /// </remarks>
        public async Task ResetAsync()
        {
            foreach (var control in _formControls.ToArray())
            {
                await control.ResetAsync();
            }

            foreach (var form in ChildForms)
            {
                await form.ResetAsync();
            }

            EvaluateForm(debounce: false);
        }

        /// <summary>
        /// Resets the validation state of all form controls.
        /// </summary>
        /// <remarks>
        /// The values in each form input component will not be changed.
        /// </remarks>
        public void ResetValidation()
        {
            foreach (var control in _formControls.ToArray())
            {
                control.ResetValidation();
            }

            foreach (var form in ChildForms)
            {
                form.ResetValidation();
            }

            EvaluateForm(debounce: false);
        }

        /// <summary>
        /// Marks all form input components as unchanged.
        /// </summary>
        /// <remarks>
        /// When called, <see cref="IsTouched"/> becomes <c>false</c>.
        /// </remarks>
        public void ResetTouched()
        {
            _touched = false;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var valid = _formControls.All(x => x.Required == false);
                if (valid != IsValid)
                {
                    // the user probably bound a variable to IsValid and it conflicts with our state.
                    // let's set this right
                    SetIsValid(valid);
                }

            }
            return base.OnAfterRenderAsync(firstRender);
        }

        private void SetDefaultControlValidation(IFormComponent formComponent)
        {
            if (Validation == null) return;

            if (!formComponent.IsForNull && (formComponent.Validation == null || (OverrideFieldValidation ?? true)))
            {
                formComponent.Validation = Validation;
            }
        }

        protected override void OnInitialized()
        {
            if (ParentMudForm != null)
            {
                ParentMudForm.ChildForms.Add(this);
            }

            base.OnInitialized();
        }

        /// <summary>
        /// Releases resources used by this form.
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
            if (ParentMudForm != null)
            {
                ParentMudForm.ChildForms.Remove(this);
                ParentMudForm.EvaluateForm(); // Need this to refresh the form state
            }
        }
    }
}
