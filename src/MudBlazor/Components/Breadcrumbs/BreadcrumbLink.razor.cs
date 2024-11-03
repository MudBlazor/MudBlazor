// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents a segment in a list of breadcrumbs.
/// </summary>
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

    private string Classname => new CssBuilder("mud-breadcrumb-item")
        .AddClass("mud-disabled", Item?.Disabled)
        .Build();
}
