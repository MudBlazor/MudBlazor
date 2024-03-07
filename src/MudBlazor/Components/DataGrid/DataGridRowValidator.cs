// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MudBlazor.Interfaces;

namespace MudBlazor
{
    public class DataGridRowValidator : IForm
    {
        public bool IsValid
        {
            get
            {
                Validate();
                return Errors.Length <= 0;
            }
        }

        public string[] Errors
        {
            get => _errors.ToArray();
        }

#nullable enable
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
            //Validate(formControl);
        }

        protected HashSet<IFormComponent> _formControls = new HashSet<IFormComponent>();

        [ExcludeFromCodeCoverage]
        public void Validate()
        {
            _errors.Clear();
            foreach (var formControl in _formControls.ToArray())
            {
                formControl.Validate();
                foreach (var err in formControl.ValidationErrors)
                {
                    _errors.Add(err);
                }
            }
        }

    }
}
