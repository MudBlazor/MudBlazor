using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudBreadcrumbs : MudComponentBase
    {
        private string Classname => new CssBuilder("mud-breadcrumbs")
            .AddClass("mud-typography-body1")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// A list of breadcrumb items/links.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Behavior)]
        public List<BreadcrumbItem>? Items { get; set; }

        /// <summary>
        /// Specifies the separator between the items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Appearance)]
        public string Separator { get; set; } = "/";

        /// <summary>
        /// Specifies a RenderFragment to use as the separator.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Appearance)]
        public RenderFragment? SeparatorTemplate { get; set; }

        /// <summary>
        /// Specifies a RenderFragment to use as the items' contents.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Behavior)]
        public RenderFragment<BreadcrumbItem>? ItemTemplate { get; set; }

        /// <summary>
        /// Controls when (and if) the breadcrumbs will automatically collapse.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Behavior)]
        public byte? MaxItems { get; set; }

        /// <summary>
        /// Custom expander icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Appearance)]
        public string ExpanderIcon { get; set; } = Icons.Material.Filled.SettingsEthernet;

        public bool Collapsed { get; private set; } = true;

        internal static string GetItemClassname(BreadcrumbItem item)
        {
            return new CssBuilder("mud-breadcrumb-item")
                .AddClass("mud-disabled", item.Disabled)
                .Build();
        }

        internal void Expand()
        {
            if (!Collapsed)
            {
                return;
            }

            Collapsed = false;
            StateHasChanged();
        }
    }
}
