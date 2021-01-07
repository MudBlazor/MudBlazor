using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System.Collections.Generic;

namespace MudBlazor
{
    public partial class MudBreadcrumbs : MudComponentBase
    {
        [Parameter] public List<BreadcrumbItem> Items { get; set; }

        [Parameter] public string Separator { get; set; } = "/";

        [Parameter] public RenderFragment SeparatorTemplate { get; set; }

        private static string GetItemClassname(BreadcrumbItem item)
        {
            return new CssBuilder("mud-breadcrumb-item")
                .AddClass("mud-disabled", item.Disabled)
                .Build();
        }
    }
}
