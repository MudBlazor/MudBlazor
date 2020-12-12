using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public partial class MudTextField<T> : MudDebouncedInput<T>
    {
        protected string Classname =>
           new CssBuilder("mud-input-input-control").AddClass(Class)
           .Build();

        private MudInput<string> _elementReference;

        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }
    }

    public class MudTextFieldString : MudTextField<string> {
        
    }
}
