// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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

        // Consumers may swap the grid's validator while an editor is registered, so every forward after Add must target the instance the control actually joined.
        private readonly Dictionary<IFormComponent, IForm> _outerRegistrations = new();

        /// <param name="outerValidator">Reads the grid's current validator, which consumers may replace at any time.</param>
        public DataGridInlineEditValidator(Func<IForm?> outerValidator)
        {
            _outerValidator = outerValidator;
        }

        void IForm.FieldChanged(IFormComponent formControl, object? newValue)
        {
            if (_outerRegistrations.TryGetValue(formControl, out var outer))
            {
                outer.FieldChanged(formControl, newValue);
            }
        }

        void IForm.Add(IFormComponent formControl)
        {
            _formControls.Add(formControl);
            if (_outerValidator() is { } outer)
            {
                _outerRegistrations[formControl] = outer;
                outer.Add(formControl);
            }
        }

        void IForm.Remove(IFormComponent formControl)
        {
            _formControls.Remove(formControl);
            if (_outerRegistrations.Remove(formControl, out var outer))
            {
                outer.Remove(formControl);
            }
        }

        void IForm.Update(IFormComponent formControl)
        {
            if (_outerRegistrations.TryGetValue(formControl, out var outer))
            {
                outer.Update(formControl);
            }
        }
    }
}
