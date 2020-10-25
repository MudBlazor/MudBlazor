using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudForm : MudComponentBase
    {
        private bool _valid;

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
            set => _valid = value;
        }

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
    }
}
