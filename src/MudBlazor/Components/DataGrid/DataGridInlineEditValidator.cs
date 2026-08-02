// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using MudBlazor.Interfaces;

namespace MudBlazor
{
    /// <summary>
    /// Collects the editors of the row being edited so a commit can validate that row on its own.
    /// </summary>
    /// <remarks>
    /// <see cref="MudDataGrid{T}.Validator"/> cascades to every rendered cell and may be an outer form, so it holds far more than the edited row.
    /// This cascades to the editors alone and re-implements the registration members to keep forwarding them to that validator, which stays responsible for grid-wide validity and touched state.
    /// </remarks>
    internal sealed class DataGridInlineEditValidator : DataGridRowValidator, IForm
    {
        private readonly Func<IForm?> _outerValidator;

        /// <param name="outerValidator">Reads the grid's current validator, which consumers may replace at any time.</param>
        public DataGridInlineEditValidator(Func<IForm?> outerValidator)
        {
            _outerValidator = outerValidator;
        }

        void IForm.FieldChanged(IFormComponent formControl, object? newValue)
        {
            _outerValidator()?.FieldChanged(formControl, newValue);
        }

        void IForm.Add(IFormComponent formControl)
        {
            _formControls.Add(formControl);
            _outerValidator()?.Add(formControl);
        }

        void IForm.Remove(IFormComponent formControl)
        {
            _formControls.Remove(formControl);
            _outerValidator()?.Remove(formControl);
        }

        void IForm.Update(IFormComponent formControl)
        {
            _outerValidator()?.Update(formControl);
        }
    }
}
