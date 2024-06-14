// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// A link to an API type, property, method, event, or field.
/// </summary>
public class ApiTypeLink : ComponentBase
{
    /// <summary>
    /// The name of the type to link.
    /// </summary>
    [Parameter]
    public string? TypeName { get; set; }

    /// <summary>
    /// The type to link.
    /// </summary>
    [Parameter]
    public DocumentedType? Type { get; set; }

    protected override void OnParametersSet()
    {
        if (Type == null || Type.Name.Equals(TypeName, StringComparison.OrdinalIgnoreCase))
        {
            Type = ApiDocumentation.GetType(TypeName);
        }
    }

    protected override bool ShouldRender() => !string.IsNullOrEmpty(TypeName) || Type != null;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Is there a linkable type?
        if (Type != null)
        {
            builder.AddDocumentedTypeLink(0, Type);
        }
        // Is this an internal type?
        else if (TypeName != null && (TypeName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) || TypeName.StartsWith("System", StringComparison.OrdinalIgnoreCase)))
        {
            builder.AddMudLink(0, $"https://learn.microsoft.com/en-us/dotnet/api/{TypeName}", TypeName, "docs-link docs-code docs-code-primary", "_external");
        }
        // Is there some text to link?
        else if (!string.IsNullOrEmpty(TypeName))
        {
            builder.AddCode(0, TypeName);
        }
    }
}
