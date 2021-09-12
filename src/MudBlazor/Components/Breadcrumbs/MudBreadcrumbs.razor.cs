using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudBreadcrumbs : MudComponentBase
    {
        /// <summary>
        /// A list of breadcrumb items/links.
        /// </summary>
        [Parameter] public List<BreadcrumbItem> Items { get; set; }

        /// <summary>
        /// Specifies the separator between the items.
        /// </summary>
        [Parameter] public string Separator { get; set; } = "/";

        /// <summary>
        /// Specifies a RenderFragment to use as the separator.
        /// </summary>
        [Parameter] public RenderFragment SeparatorTemplate { get; set; }

        /// <summary>
        /// Specifies a RenderFragment to use as the items' contents.
        /// </summary>
        [Parameter] public RenderFragment<BreadcrumbItem> ItemTemplate { get; set; }

        /// <summary>
        /// Controls when (and if) the breadcrumbs will automatically collapse.
        /// </summary>
        [Parameter] public byte? MaxItems { get; set; }

        /// <summary>
        /// Custom expander icon.
        /// </summary>
        [Parameter] public string ExpanderIcon { get; set; } = Icons.Material.Filled.SettingsEthernet;

        public bool Collapsed { get; private set; } = true;

        private static string GetItemClassname(BreadcrumbItem item)
        {
            return new CssBuilder("mud-breadcrumb-item")
                .AddClass("mud-disabled", item.Disabled)
                .Build();
        }

        private void Expand()
        {
            if (!Collapsed)
                return;

            Collapsed = false;
            StateHasChanged();
        }
    }
}
