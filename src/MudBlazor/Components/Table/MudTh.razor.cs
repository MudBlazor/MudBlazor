using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTh : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-table-cell")
            .AddClass(Class).Build();

        [Parameter] public RenderFragment ChildContent { get; set; }
        /// <summary>
        /// Used to control whether this header column is visible.
        /// Must be set on MudTd of same property to achieve
        /// proper toggling of columns
        /// </summary>
        [Parameter] public bool Visible { get; set; } = true;
        private string ThStyle => new StyleBuilder()
            .AddStyle("display", "none", !Visible)
            .AddStyle(Style)
            .Build();
    }
}

