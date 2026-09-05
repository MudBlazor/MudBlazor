// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;


/// <summary>
/// The clickable link rendered for each <see cref="BreadcrumbItem"/> in a <see cref="MudBreadcrumbs"/> trail.
/// </summary>
/// <seealso cref="BreadcrumbItem" />
/// <seealso cref="BreadcrumbSeparator" />
/// <seealso cref="MudBreadcrumbs" />
public partial class BreadcrumbLink
{
    /// <summary>
    /// The item to display.
    /// </summary>
    [Parameter]
    public BreadcrumbItem? Item { get; set; }

    /// <summary>
    /// The parent breadcrumb component.
    /// </summary>
    [CascadingParameter]
    public MudBreadcrumbs? Parent { get; set; }

    /// <summary>
    /// Whether this item is the last one in the trail and therefore represents the current page.
    /// </summary>
    private bool IsCurrentPage => Item is not null && Parent?.Items is { Count: > 0 } items && ReferenceEquals(items[^1], Item);

    private string Classname => new CssBuilder("mud-breadcrumb-item")
        .AddClass("mud-disabled", Item?.Disabled)
        .Build();
}
