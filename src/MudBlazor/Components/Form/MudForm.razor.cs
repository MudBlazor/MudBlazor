using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudForm : MudComponentBase
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
        /// Raised when IsValid changes.
        /// </summary>
        [Parameter] public EventCallback<bool> IsValidChanged { get; set; }

        // keeps track of validation. if the input was validated at least once the value will be true
        protected Dictionary<MudBaseInputText, bool> _formControls = new Dictionary<MudBaseInputText, bool>();
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

        internal void Add(MudBaseInputText formControl)
        {
            _formControls[formControl]=false; // false means fresh, not yet validated!
        }

        internal void Remove(MudBaseInputText formControl)
        {
            _formControls.Remove(formControl);
        }

        private bool _update_required;
        /// <summary>
        /// Called by any input of the form to signal that its value changed. 
        /// </summary>
        /// <param name="formControl"></param>
        internal void Update(MudBaseInputText formControl)
        {
            // request delayed update after render so we can validate all at once
            _update_required=true;
            _formControls[formControl] = true;
            StateHasChanged();
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (_update_required)
            {
                _update_required = false;
                _errors.Clear();
                foreach (var error in _formControls.Keys.SelectMany(control => control.ValidationErrors))
                    _errors.Add(error);
                var old_valid = _valid;
                // form can only be valid if none have an error and all have been validated at least once!
                var no_errors = _formControls.Keys.All(x => x.Error == false);
                var all_validated = _formControls.Values.All(x => x == true);
                _valid = no_errors && all_validated;
                if (old_valid != _valid)
                    IsValidChanged.InvokeAsync(_valid);
                ErrorsChanged.InvokeAsync(Errors);
                StateHasChanged();
            }
            return base.OnAfterRenderAsync(firstRender);
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
            _update_required=true;
            StateHasChanged();
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
            _update_required = true;
            StateHasChanged();
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
            _update_required = true;
            StateHasChanged();
        }
    }
}
