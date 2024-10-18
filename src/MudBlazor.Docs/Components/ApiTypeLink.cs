﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    private DocumentedType? _type;
    private string? _typeName;

    /// <summary>
    /// The type to link.
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
                _typeName = _type?.Name;
                StateHasChanged();
            }
        }
    }

    /// <summary>
    /// The name of the type to link.
    /// </summary>
    [Parameter]
    public string? TypeName
    {
        get => _typeName;
        set
        {
            if (_typeName != value)
            {
                _typeName = value;
                _type = ApiDocumentation.GetType(_typeName);
                StateHasChanged();
            }
        }
    }

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

    /// <summary>
    /// The size of the text.
    /// </summary>
    [Parameter]
    public Typo Typo { get; set; } = Typo.caption;

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
                        builder.AddMudTooltip(0, Placement.Top, $"External Link: https://learn.microsoft.com/dotnet/api/{TypeName}", (tooltipSequence, tooltipBuilder) =>
                        {
                            tooltipBuilder.AddMudLink(tooltipSequence, $"https://learn.microsoft.com/dotnet/api/{TypeName}", TypeFriendlyName, Typo, "docs-link docs-code docs-code-primary", "_external", (linkSequence, linkBuilder) =>
                            {
                                linkBuilder.AddMudIcon(linkSequence++, "MudBlazor.Icons.Material.Filled.Link", Color.Default, Size.Small);
                                linkBuilder.AddContent(linkSequence++, TypeFriendlyName);
                            });
                        });
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
