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
    public partial class MudForm : MudComponentBase, IDisposable
    {

        protected string Classname =>
            new CssBuilder("mud-form")
            .AddClass(Class)
       .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Validation status. True if the form is valid and without errors. This parameter is readonly.
        /// </summary>
        [Parameter]
        public bool IsValid
        {
            get => _valid;
            set { /* readonly parameter! */ }
        }
        private bool _valid;

        /// <summary>
        /// Validation debounce delay in milliseconds. This can help improve rendering performance of forms with real-time validation of inputs
        /// i.e. when textfields have Immediate="true"
        /// </summary>
        [Parameter] public int ValidationDelay { get; set; } = 300;

        /// <summary>
        /// When true, the form will not re-render its child contents on validation updates (i.e. when IsValid changes). This is an optimization which is necessary
        /// especially for larger forms on older devices.
        /// </summary>
        [Parameter] public bool SuppressRenderingOnValidation { get; set; } = true;

        /// <summary>
        /// Raised when IsValid changes.
        /// </summary>
        [Parameter] public EventCallback<bool> IsValidChanged { get; set; }

        // keeps track of validation. if the input was validated at least once the value will be true
        protected Dictionary<IFormComponent, bool> _formControls = new Dictionary<IFormComponent, bool>();
        protected HashSet<string> _errors = new HashSet<string>();

        /// <summary>
        /// Validation error messages
        /// </summary>
        [Parameter]
        public string[] Errors
        {
            get => _errors.ToArray();
            set { /* readonly */ }
        }

        [Parameter] public EventCallback<string[]> ErrorsChanged { get; set; }

        internal void Add(IFormComponent formControl)
        {
            _formControls[formControl] = false; // false means fresh, not yet validated!
        }

        internal void Remove(IFormComponent formControl)
        {
            _formControls.Remove(formControl);
        }

        private Timer _timer;

        /// <summary>
        /// Called by any input of the form to signal that its value changed. 
        /// </summary>
        /// <param name="formControl"></param>
        internal void Update(IFormComponent formControl)
        {
            _formControls[formControl] = true;
            EvaluateForm();
        }

        private void EvaluateForm(bool debounce=true)
        {
            _timer?.Dispose();
            if (debounce && ValidationDelay > 0)
                _timer = new Timer(OnTimerComplete, null, ValidationDelay, Timeout.Infinite);
            else
                OnEvaluateForm();
        }

        private void OnTimerComplete(object stateInfo) => InvokeAsync(OnEvaluateForm);

        private bool _shouldRender=true; // <-- default is true, we need the form children to render

        protected async Task OnEvaluateForm()
        {
            _errors.Clear();
            foreach (var error in _formControls.Keys.SelectMany(control => control.ValidationErrors))
                _errors.Add(error);
            var old_valid = _valid;
            // form can only be valid if none have an error and all have been validated at least once!
            var no_errors = _formControls.Keys.All(x => x.Error == false);
            var all_validated = _formControls.Values.All(x => x == true);
            _valid = no_errors && all_validated;
            try
            {
                _shouldRender = false;
                if (old_valid != _valid)
                    await IsValidChanged.InvokeAsync(_valid);
                await ErrorsChanged.InvokeAsync(Errors);
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
        /// Force a validation of all form controls, even if they haven't been touched by the user yet
        /// </summary>
        public void Validate()
        {
            foreach (var control in _formControls.Keys.ToArray())
            {
                control.Validate();
            }
            EvaluateForm(debounce:false);
        }

        /// <summary>
        /// Reset all form controls and reset their validation state
        /// </summary>
        public void Reset()
        {
            foreach (var control in _formControls.Keys.ToArray())
            {
                control.Reset();
                _formControls[control] = false;
            }
            EvaluateForm(debounce: false);
        }

        /// <summary>
        /// Reset the validation state but keep the values
        /// </summary>
        public void ResetValidation()
        {
            foreach (var control in _formControls.Keys.ToArray())
            {
                control.ResetValidation();
                _formControls[control] = false;
            }
            EvaluateForm(debounce: false);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
