// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents a divider between breadcrumb items.
/// </summary>
/// <seealso cref="MudBreadcrumbs" />
/// <seealso cref="BreadcrumbItem" />
/// <seealso cref="BreadcrumbLink" />
public partial class BreadcrumbSeparator
{
    /// <summary>
    /// The parent breadcrumb component.
    /// </summary>
    [CascadingParameter]
    public MudBreadcrumbs? Parent { get; set; }
}
