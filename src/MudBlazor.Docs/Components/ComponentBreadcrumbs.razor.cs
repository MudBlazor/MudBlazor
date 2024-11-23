// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// Represents a set of links for a documented type's base classes.
/// </summary>
public partial class ComponentBreadcrumbs
{
    private DocumentedType? type;
    private string? typeName;

    /// <summary>
    /// The title of the item representing the current page.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  When <c>null</c>, the name of the current <see cref="Type"/> is used.
    /// </remarks>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// The type to display links for.
    /// </summary>
    [Parameter]
    public DocumentedType? Type
    {
        get => type;
        set
        {
            type = value;
            typeName = value == null ? null : type!.Name;
            OnTypeChanged(type);
            StateHasChanged();
        }
    }

    /// <summary>
    /// The name of the type to display links for.
    /// </summary>
    [Parameter]
    public string? TypeName
    {
        get => typeName;
        set
        {
            typeName = value;
            type = value == null ? null : ApiDocumentation.GetType(typeName);
            OnTypeChanged(type);
            StateHasChanged();
        }
    }

    /// <summary>
    /// Gets the breadcrumb items.
    /// </summary>
    public List<BreadcrumbItem> Items { get; set; } = [];

    /// <summary>
    /// Occurs when <see cref="Type"/> or <see cref="TypeName"/> has changed.
    /// </summary>
    /// <param name="type"></param>
    protected void OnTypeChanged(DocumentedType? type)
    {
        // Start with the top-level link
        Items = [new("Explore", "/docs/overview")];
        // Is there a type to examine?
        if (type == null)
        {
            return;
        }
        // Add the type breadcrumb
        Items.Add(new(Title ?? type!.NameFriendly, null));
    }
}
