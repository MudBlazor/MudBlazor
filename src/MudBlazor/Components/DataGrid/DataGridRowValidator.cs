// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MudBlazor.Interfaces;

namespace MudBlazor
{
    /// <summary>
    /// Validates the input fields of a <see cref="MudDataGrid{T}"/> row during inline or form editing.
    /// </summary>
    public class DataGridRowValidator : IForm
    {
        /// <summary>
        /// Indicates whether the row is valid.
        /// </summary>
        /// <remarks>
        /// Reading this drives a validation pass that only completes inline for synchronous validators.
        /// Callers that must respect asynchronous validators should await <see cref="ValidateAsync"/> and read <see cref="Errors"/>.
        /// </remarks>
        public bool IsValid
        {
            get
            {
                // IForm.IsValid must remain synchronous, so drive validation without awaiting; exceptions are forwarded to MudGlobal.UnhandledExceptionHandler.
                ValidateAsync().CatchAndLog();
                return Errors.Length <= 0;
            }
        }

        /// <summary>
        /// Any validation errors for this row.
        /// </summary>
        public string[] Errors
        {
            get => _errors.ToArray();
        }

        /// <summary>
        /// The data to validate for this row.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public virtual object? Model { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
#nullable disable

        protected HashSet<string> _errors = new HashSet<string>();

        void IForm.FieldChanged(IFormComponent formControl, object newValue)
        {
            //implement in future for DataGrid
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

        protected HashSet<IFormComponent> _formControls = new HashSet<IFormComponent>();

        /// <summary>
        /// Checks this row for any validation errors.
        /// </summary>
        [Obsolete("Use ValidateAsync instead.")]
        [ExcludeFromCodeCoverage]
        public void Validate()
        {
            ValidateAsync().CatchAndLog();
        }

        /// <summary>
        /// Checks this row for any validation errors, awaiting asynchronous validators before collecting their errors.
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
