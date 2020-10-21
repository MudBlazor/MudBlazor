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

    }
}
