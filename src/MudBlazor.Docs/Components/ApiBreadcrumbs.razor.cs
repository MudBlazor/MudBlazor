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
public partial class ApiBreadcrumbs
{
    private DocumentedType? _type;

    /// <summary>
    /// The type to display links for.
    /// </summary>
    [Parameter]
    public DocumentedType? Type
    {
        get => _type;
        set
        {
            if (_type != value)
            {
                _type = value;
                OnTypeChanged(_type);
            }
        }
    }

    /// <summary>
    /// Gets the breadcrumb items.
    /// </summary>
    public List<BreadcrumbItem> Items { get; set; } = [];

    /// <inheritdoc />
    protected void OnTypeChanged(DocumentedType? type)
    {
        // Start with the top-level link
        Items = [new("Index", "/api")];

        if (Type == null)
        {
            return;
        }

        // Add the type breadcrumb
        Items.Add(new(Type.NameFriendly, Type.ApiUrl));
        var parent = Type.BaseType;
        // Walk up the hierarchy and add base type breadcrumbs
        while (parent != null)
        {
            Items.Insert(1, new(parent.NameFriendly, parent.ApiUrl));
            parent = parent.BaseType;
        }
    }
}
