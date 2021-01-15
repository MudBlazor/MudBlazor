using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudBlazor.Interfaces;

namespace MudBlazor
{
    class TableRowValidator : Interfaces.IForm
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
            _formControls[formControl] = false;
        }

        void IForm.Remove(IFormComponent formControl)
        {
            _formControls.Remove(formControl);
        }

        void IForm.Update(IFormComponent formControl)
        {
            //Validate(formControl);
        }

        protected Dictionary<IFormComponent, bool> _formControls = new Dictionary<IFormComponent, bool>();

        public void Validate()
        {
            _errors.Clear();
            foreach (var control in _formControls.Keys.ToArray())
            {
                control.Validate();
                foreach (var err in control.ValidationErrors)
                {
                    _errors.Add(err);
                }
            }
        }


    }
}
