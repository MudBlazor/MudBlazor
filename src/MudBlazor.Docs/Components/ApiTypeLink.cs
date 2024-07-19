// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using LoxSmoke.DocXml;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services.XmlDocs;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// A link to a type.
/// </summary>
public class ApiTypeLink : ComponentBase
{
    /// <summary>
    /// The service for XML documentation.
    /// </summary>
    [Inject]
    public IXmlDocsService? Docs { get; set; }

    /// <summary>
    /// The type to link.
    /// </summary>
    public Type? Type { get; set; }

    /// <summary>
    /// The name of the type to link.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string? TypeFullName { get; set; }

    /// <summary>
    /// The name of the type to display.
    /// </summary>
    public string? TypeFriendlyName { get; set; }

    /// <summary>
    /// The XML documentation for this type.
    /// </summary>
    public TypeComments? TypeComments { get; set; }

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(TypeFullName) && (Type == null || TypeFriendlyName == null || TypeComments == null || Type.Name != TypeFullName))
        {
            Type = Docs?.GetType(TypeFullName);
            if (Type == null)
            {
                TypeComments = null;
                TypeFriendlyName = TypeFullName switch
                {
                    "System.Boolean" => "bool",
                    "System.Boolean[]" => "bool[]",
                    "System.Int32" => "int",
                    "System.Int32[]" => "int[]",
                    "System.Int64" => "long",
                    "System.Int64[]" => "long[]",
                    "System.String" => "string",
                    "System.String[]" => "string[]",
                    "System.Double" => "double",
                    "System.Double[]" => "double[]",
                    "System.Single" => "float",
                    "System.Single[]" => "float[]",
                    "System.Object" => "object",
                    "System.Void" => "void",
                    _ => TypeFullName.Substring(TypeFullName.LastIndexOf('.') + 1)
                };
            }
            else
            {
                TypeComments = Docs?.GetTypeComments(Type);
                TypeFriendlyName = Type.GetFriendlyName();
            }
        }
    }

    protected override bool ShouldRender() => !string.IsNullOrEmpty(TypeFullName) || Type != null;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Is this an internal type?
        if (TypeFullName != null && (TypeFullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) || TypeFullName.StartsWith("System", StringComparison.OrdinalIgnoreCase)))
        {
            // Is this a native type?
            switch (TypeFullName)
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
                    if (!TypeFullName.Contains("[["))
                    {
                        builder.AddMudLink(0, $"https://learn.microsoft.com/en-us/dotnet/api/{TypeFullName}", TypeFriendlyName, "docs-link docs-code docs-code-secondary", "_external");
                    }
                    else
                    {
                        builder.AddCode(0, TypeFriendlyName);
                    }
                    break;
            }
        }
        // Is there a linkable type?
        else if (Type != null && (Type.FullName == null || Type.FullName.StartsWith("MudBlazor.")))
        {
            builder.AddMudLink(0, $"api/{TypeFullName}", Type.GetFriendlyName(), "docs-link docs-code docs-code-primary");
        }
        // Is there some text to link?
        else if (!string.IsNullOrEmpty(TypeFriendlyName))
        {
            builder.AddCode(0, TypeFriendlyName);
        }
        // Is there some text to link?
        else if (!string.IsNullOrEmpty(TypeFullName))
        {
            builder.AddCode(0, TypeFullName);
        }
    }
}
