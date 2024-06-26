// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
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
    /// The type to link.
    /// </summary>
    [Parameter]
    public DocumentedType? Type { get; set; }

    /// <summary>
    /// The name of the type to link.
    /// </summary>
    [Parameter]
    public string? TypeName { get; set; }

    /// <summary>
    /// The name of the type to display.
    /// </summary>
    [Parameter]
    public string? TypeFriendlyName { get; set; }

    /// <summary>
    /// Shows a tooltip with the type's summary.
    /// </summary>
    [Parameter]
    public bool ShowTooltip { get; set; } = true;

    protected override void OnParametersSet()
    {
        if (Type == null || (TypeName != null && Type.Name.Equals(TypeName, StringComparison.OrdinalIgnoreCase)))
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
            // Is this a native type?
            switch (TypeName)
            {
                case "System.Boolean":
                case "System.Boolean[]":
                case "System.Int32":
                case "System.Int32[]":
                case "System.Int64":
                case "System.Int64[]":
                case "System.String":
                case "System.String[]":
                case "System.Double":
                case "System.Double[]":
                case "System.Single":
                case "System.Single[]":
                case "System.Object":
                case "System.Void":
                    builder.AddCode(0, TypeFriendlyName);
                    break;
                default:
                    // Is this a linkable type?
                    if (!TypeName.Contains("[["))
                    {
                        builder.AddMudLink(0, $"https://learn.microsoft.com/en-us/dotnet/api/{TypeName}", TypeFriendlyName, "docs-link docs-code docs-code-primary", "_external");
                    }
                    else
                    {
                        builder.AddCode(0, TypeFriendlyName);
                    }
                    break;
            }
        }
        // Is there some text to link?
        else if (!string.IsNullOrEmpty(TypeFriendlyName))
        {
            builder.AddCode(0, TypeFriendlyName);
        }
        // Is there some text to link?
        else if (!string.IsNullOrEmpty(TypeName))
        {
            builder.AddCode(0, TypeName);
        }
    }
}
