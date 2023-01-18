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

        /// <summary>
        /// Used to control whether this column data is visible.
        /// Must be set on MudTh of same property to achieve
        /// proper toggling of columns
        /// </summary>
        [Parameter] public bool Visible { get; set; } = true;
        private string TdStyle => new StyleBuilder()
            .AddStyle("display", "none", !Visible)
            .AddStyle(Style)
            .Build();

    }
}
