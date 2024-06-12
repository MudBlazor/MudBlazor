﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Pages.Api;

/// <summary>
/// Represents a page for viewing the members of a documented type.
/// </summary>
public partial class Api
{
    /// <summary>
    /// The name of the type to display.
    /// </summary>
    [Parameter]
    public string TypeName { get; set; }

    /// <summary>
    /// Shows the inheritance hierarchy.
    /// </summary>
    public bool ShowInheritance => false;

    /// <summary>
    /// The type being displayed.
    /// </summary>
    public DocumentedType DocumentedType { get; set; }

    protected override void OnParametersSet()
    {
        if (DocumentedType == null || DocumentedType.Name != TypeName)
        {
            DocumentedType = ApiDocumentation.GetType(TypeName);
        }
    }
}
