// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor;


/// <summary>
/// The divider rendered between items in a <see cref="MudBreadcrumbs"/> trail, showing the separator character or a custom template.
/// </summary>
/// <seealso cref="BreadcrumbItem" />
/// <seealso cref="BreadcrumbLink" />
/// <seealso cref="MudBreadcrumbs" />
public partial class BreadcrumbSeparator
{
    /// <summary>
    /// The parent breadcrumb component.
    /// </summary>
    [CascadingParameter]
    public MudBreadcrumbs? Parent { get; set; }
}
