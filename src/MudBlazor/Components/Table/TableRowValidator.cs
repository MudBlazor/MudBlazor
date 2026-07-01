using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MudBlazor.Interfaces;

namespace MudBlazor
{

    /// <summary>
    /// A validator for rows within a <see cref="MudTable{T}"/>.
    /// </summary>
    public class TableRowValidator : IForm
    {
        /// <summary>
        /// Whether the table row is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                // IForm.IsValid is synchronous, so drive validation without awaiting.
                // Synchronous validators complete inline; asynchronous ones should use ValidateAsync.
                _ = ValidateAsync();
                return Errors.Length <= 0;
            }
        }

        /// <summary>
        /// The validation errors for this row.
        /// </summary>
        public string[] Errors => _errors.ToArray();

        /// <summary>
        /// The model being edited by the form.
        /// </summary>
        public object? Model { get; set; }

        protected HashSet<string> _errors = new();

        void IForm.FieldChanged(IFormComponent formControl, object? newValue)
        {
            //implement in future for table
        }

        void IForm.Add(IFormComponent formControl)
        {
            _formControls.Add(formControl);
        }

        void IForm.Remove(IFormComponent formControl)
        {
            _formControls.Remove(formControl);
        }

        void IForm.Update(IFormComponent formControl)
        {
        }

        protected HashSet<IFormComponent> _formControls = new();

        /// <summary>
        /// Checks for data errors within this row.
        /// </summary>
        [Obsolete("Use ValidateAsync instead.")]
        [ExcludeFromCodeCoverage]
        public void Validate()
        {
            _ = ValidateAsync();
        }

        /// <summary>
        /// Checks for data errors within this row.
        /// </summary>
        public async Task ValidateAsync()
        {
            _errors.Clear();
            foreach (var formControl in _formControls.ToArray())
            {
                await formControl.ValidateAsync();
                foreach (var err in formControl.ValidationErrors)
                {
                    _errors.Add(err);
                }
            }
        }
    }
}
