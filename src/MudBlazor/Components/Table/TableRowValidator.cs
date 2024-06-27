using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor.Interfaces;

namespace MudBlazor
{
#nullable enable
    public class TableRowValidator : IForm
    {
        public bool IsValid
        {
            get
            {
                ValidateAsync().Wait();
                return Errors.Length <= 0;
            }
        }

        public string[] Errors => _errors.ToArray();

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
            //Validate(formControl);
        }

        protected HashSet<IFormComponent> _formControls = new();

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
