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
    public partial class MudForm : MudComponentBase, IDisposable, IForm
    {
        protected string Classname =>
            new CssBuilder("mud-form")
            .AddClass($"gap-{Spacing}", Spacing >= 0)
            .AddClass(Class)
       .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Form.ValidatedData)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Validation status. True if the form is valid and without errors. This parameter is two-way bindable.
        /// </summary>
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
        /// True if any field of the field was touched. This parameter is readonly.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public bool IsTouched { get => _touched; set {/* readonly parameter! */ } }

        private bool _touched = false;

        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public bool Disabled { get; set; }
        [CascadingParameter(Name = "ParentDisabled")] private bool ParentDisabled { get; set; }
        protected bool GetDisabledState() => Disabled || ParentDisabled;

        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public bool ReadOnly { get; set; }
        [CascadingParameter(Name = "ParentReadOnly")] private bool ParentReadOnly { get; set; }
        protected bool GetReadOnlyState() => ReadOnly || ParentReadOnly;

        /// <summary>
        /// Validation debounce delay in milliseconds. This can help improve rendering performance of forms with real-time validation of inputs
        /// i.e. when textfields have Immediate="true".
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public int ValidationDelay { get; set; } = 300;

        /// <summary>
        /// When true, the form will not re-render its child contents on validation updates (i.e. when IsValid changes).
        /// This is an optimization which can be necessary especially for larger forms on older devices.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public bool SuppressRenderingOnValidation { get; set; } = false;

        /// <summary>
        /// When true, will not cause a page refresh on Enter if any input has focus.
        /// </summary>
        /// <remarks>
        /// https://www.w3.org/TR/2018/SPSD-html5-20180327/forms.html#implicit-submission
        /// Usually this is not wanted, as it can cause a page refresh in the middle of editing a form. 
        /// When the form is in a dialog this will cause the dialog to close. So by default we suppress it.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public bool SuppressImplicitSubmission { get; set; } = true;

        /// <summary>
        /// The gap between items, measured in increments of <c>4px</c>.
        /// <br/>
        /// Maximum is <c>20</c>.
        /// <br/>
        /// Default is no spacing.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Form.Behavior)]
        public int Spacing { set; get; }

        /// <summary>
        /// Raised when IsValid changes.
        /// </summary>
        [Parameter] public EventCallback<bool> IsValidChanged { get; set; }

        /// <summary>
        /// Raised when IsTouched changes.
        /// </summary>
        [Parameter] public EventCallback<bool> IsTouchedChanged { get; set; }

        /// <summary>
        /// Raised when a contained IFormComponent changes its value
        /// </summary>
        [Parameter] public EventCallback<FormFieldChangedEventArgs> FieldChanged { get; set; }

        // keeps track of validation. if the input was validated at least once the value will be true
        protected HashSet<IFormComponent> _formControls = new();
        protected HashSet<string> _errors = new();

        /// <summary>
        /// A default validation func or a validation attribute to use for form controls that don't have one.
        /// Supported types are:
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
        public object Validation { get; set; }

        /// <summary>
        /// If a field already has a validation, override it with <see cref="Validation"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public bool? OverrideFieldValidation { get; set; }

        /// <summary>
        /// Validation error messages.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Form.ValidationResult)]
        public string[] Errors
        {
            get => _errors.ToArray();
            set { /* readonly */ }
        }

        [Parameter] public EventCallback<string[]> ErrorsChanged { get; set; }

        /// <summary>
        /// Specifies the top-level model object for the form. Used with Fluent Validation
        /// </summary>
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
        /// Force a validation of all form controls, even if they haven't been touched by the user yet.
        /// </summary>
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
        /// Reset all form controls and reset their validation state.
        /// </summary>
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
        /// Reset the validation state but keep the values.
        /// </summary>
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
        /// Reset the isTouched property
        /// </summary>
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
