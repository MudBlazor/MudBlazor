using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a series of links used to show the user's current location.
    /// </summary>
    public partial class MudBreadcrumbs : MudComponentBase
    {
        private string Classname => new CssBuilder("mud-breadcrumbs")
            .AddClass("mud-typography-body1")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The list of items to display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Behavior)]
        public IReadOnlyList<BreadcrumbItem>? Items { get; set; }

        /// <summary>
        /// The separator shown between items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>/</c>.  Will not be shown if <see cref="SeparatorTemplate"/> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Appearance)]
        public string Separator { get; set; } = "/";

        /// <summary>
        /// The content shown between items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Appearance)]
        public RenderFragment? SeparatorTemplate { get; set; }

        /// <summary>
        /// The custom template used to display items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Behavior)]
        public RenderFragment<BreadcrumbItem>? ItemTemplate { get; set; }

        /// <summary>
        /// The maximum number of items to dislpay.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  If <see cref="Collapsed"/> is <c>true</c> and the number of items exceeds this value, the breadcrumbs will automatically collapse.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Behavior)]
        public byte? MaxItems { get; set; }

        /// <summary>
        /// The icon to display when items are collapsed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>Icons.Material.Filled.SettingsEthernet</c>.  Displays when <see cref="Collapsed"/> and the number of items exceeds <see cref="MaxItems"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Appearance)]
        public string ExpanderIcon { get; set; } = Icons.Material.Filled.SettingsEthernet;

        /// <summary>
        /// Collapses items when the number of items exceeds <see cref="MaxItems"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
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
