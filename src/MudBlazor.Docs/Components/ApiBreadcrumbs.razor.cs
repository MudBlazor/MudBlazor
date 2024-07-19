// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Extensions;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// Represents a set of links for a documented type's base classes.
/// </summary>
public partial class ApiBreadcrumbs
{
    /// <summary>
    /// The type to display links for.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public Type? Type { get; set; }

    /// <summary>
    /// Gets the breadcrumb items.
    /// </summary>
    public List<BreadcrumbItem> Items { get; set; } = [];

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Items = [];

        if (Type == null)
        {
            return;
        }

        // Add the type breadcrumb
        var parent = Type.BaseType;
        // Walk up the hierarchy and add base type breadcrumbs
        while (parent != null && parent.Name != "Object" && parent.Name != "ComponentBase")
        {
            Items.Insert(0, new(parent.GetFriendlyName(), $"/api/{parent.Name}"));
            parent = parent.BaseType;
        }
        Items.Add(new(Type.GetFriendlyName(), $"/api/{Type.Name}"));
    }
}
