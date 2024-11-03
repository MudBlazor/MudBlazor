using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudTh : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-table-cell")
            .AddClass(Class)
            .Build();

        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}

