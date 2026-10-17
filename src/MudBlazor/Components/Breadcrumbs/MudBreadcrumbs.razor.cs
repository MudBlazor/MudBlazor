using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Resources;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Breadcrumbs show a navigation trail of links marking the user's current location within a page hierarchy.
    /// </summary>
    /// <seealso cref="BreadcrumbItem" />
    /// <seealso cref="BreadcrumbLink" />
    /// <seealso cref="BreadcrumbSeparator" />
    public partial class MudBreadcrumbs : MudComponentBase
    {
        private string Classname => new CssBuilder("mud-breadcrumbs")
            .AddClass("mud-typography-body1")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The ARIA label for the navigation landmark element.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. When not set, the localized default ("Breadcrumb") is used.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Breadcrumbs.Behavior)]
        public string? AriaLabel { get; set; }

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
        /// The maximum number of items to display.
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
        /// Defaults to <see cref="Icons.Material.Filled.SettingsEthernet" />.  Displays when <see cref="Collapsed"/> and the number of items exceeds <see cref="MaxItems"/>.
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

        /// <summary>
        /// Whether <paramref name="item"/> is the last item in the trail and therefore represents the current page.
        /// </summary>
        internal bool IsLastItem(BreadcrumbItem? item) => Items is { Count: > 0 } && ReferenceEquals(Items[^1], item);

        internal static string GetItemClassname(BreadcrumbItem item)
        {
            return new CssBuilder("mud-breadcrumb-item")
                .AddClass("mud-disabled", item.Disabled)
                .Build();
        }

        private string? GetNavAriaLabel()
        {
            if (!string.IsNullOrEmpty(AriaLabel))
            {
                return AriaLabel;
            }

            foreach (var (key, value) in UserAttributes)
            {
                if (string.Equals(key, "aria-label", StringComparison.OrdinalIgnoreCase) && value is not null)
                {
                    return value.ToString();
                }
            }

            return Localizer[LanguageResource.MudBreadcrumbs_Label];
        }

        private IReadOnlyDictionary<string, object?> GetListUserAttributes()
        {
            if (UserAttributes.Any(x => string.Equals(x.Key, "aria-label", StringComparison.OrdinalIgnoreCase)))
            {
                var filtered = new Dictionary<string, object?>(UserAttributes);
                var keyToRemove = filtered.Keys.FirstOrDefault(k => string.Equals(k, "aria-label", StringComparison.OrdinalIgnoreCase));
                if (keyToRemove is not null)
                {
                    filtered.Remove(keyToRemove);
                }

                return filtered;
            }

            return UserAttributes;
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
