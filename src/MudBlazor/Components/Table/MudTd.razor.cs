using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTd : MudComponentBase
    {

        protected string Classname =>
        new CssBuilder("mud-table-cell")
            .AddClass("mud-table-cell-hide", HideSmall)
            .AddClass(Class)
        .Build();

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public string DataLabel { get; set; }

        /// <summary>
        /// Hide cell when breakpoint is smaller than the defined value in table.
        /// </summary>
        [Parameter] public bool HideSmall { get; set; }
    }
}
