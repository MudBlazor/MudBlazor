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
        /// Validaton status. True if the form is valid
        /// </summary>
        public bool IsValid
        {
            get => _valid;
            private set => _valid = value;
        }
        private bool _valid;

        /// <summary>
        /// Raised when IsValid changes.
        /// </summary>
        [Parameter] public EventCallback<bool> IsValidChanged { get; set; }

        public IEnumerable<MudBaseInputText> FormControls => _formControls.ToArray();
        protected HashSet<MudBaseInputText> _formControls = new HashSet<MudBaseInputText>();

        public void Add(MudBaseInputText formControl)
        {
            _formControls.Add(formControl);
        }

        public void Remove(MudBaseInputText formControl)
        {
            _formControls.Remove(formControl);
        }

        private bool _update_required;
        public void Update(MudBaseInputText formControl)
        {
            _update_required=true;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (_update_required)
            {
                _update_required = false;
                var old_valid = _valid;
                IsValid = _formControls.All(x => x.Error == false);
                if (old_valid != _valid)
                    IsValidChanged.InvokeAsync(_valid);
            }
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
