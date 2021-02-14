﻿using System;
using System.Collections.Generic;
using System.Linq;
using MudBlazor.Interfaces;

namespace MudBlazor
{
    public class TableRowValidator : IForm
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
        protected HashSet<string> _errors = new HashSet<string>();

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
