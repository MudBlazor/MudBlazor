// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using LoxSmoke.DocXml;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Docs.Extensions;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// Represents the summary or remarks of an object, with linking.
/// </summary>
public partial class ApiText : ComponentBase
{
    /// <summary>
    /// The type currently being documented.
    /// </summary>
    [Parameter]
    public Type? Context { get; set; }

    /// <summary>
    /// The XML documentation text to parse.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public CommonComments? Comments { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Anything to do?
        if (Comments == null || string.IsNullOrEmpty(Comments.Summary + Comments.Remarks))
        {
            return;
        }

        var sequence = 0;

        // Convert XML documentation text, links, and HTML to MudBlazor equivalents
        var xml = XElement.Parse("<xml>" + Comments.Summary + " " + Comments.Remarks + "</xml>");

        using var reader = xml.CreateReader();
        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:

                    #region See Cref/Href Element

                    switch (reader.Name)
                    {
                        case "see":
                            var isEmptyElement = reader.IsEmptyElement;
                            reader.MoveToFirstAttribute();
                            var link = reader.Value;
                            switch (reader.Name)
                            {
                                case "cref":
                                    // Get the link type
                                    var linkType = link.Substring(0, 1);
                                    var linkRef = link.Substring(2);
                                    if (linkType == "T") // Type
                                    {
                                        // Add a link to the type
                                        builder.OpenComponent<ApiTypeLink>(sequence++);
                                        builder.AddComponentParameter(sequence++, nameof(ApiTypeLink.TypeFullName), linkRef);
                                        builder.CloseComponent();
                                    }
                                    else // Property, Method, Field, or Event
                                    {
                                        if (linkRef.StartsWith("MudBlazor.Icons"))
                                        {
                                            builder.AddMudIconWithTooltip(sequence++, linkRef, Color.Primary, Size.Medium);
                                        }
                                        else if (linkRef.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) || linkRef.StartsWith("System", StringComparison.OrdinalIgnoreCase))
                                        {
                                            // Extract the type and member
                                            var values = linkRef.Split(".");
                                            var typeAndMember = values[values.Length - 2] + "." + values[values.Length - 1];
                                            builder.AddMudLink(0, $"https://learn.microsoft.com/en-us/dotnet/api/{linkRef}", typeAndMember, "docs-link docs-code docs-code-primary", "_external");
                                        }
                                        else if (linkRef.StartsWith("MudBlazor."))
                                        {
                                            builder.OpenComponent<ApiMemberLink>(sequence++);
                                            builder.AddComponentParameter(sequence++, nameof(ApiMemberLink.Context), Context);
                                            builder.AddComponentParameter(sequence++, nameof(ApiMemberLink.MemberName), linkRef);
                                            builder.CloseComponent();
                                        }
                                        else
                                        {
                                            builder.AddCode(sequence++, linkRef);
                                        }
                                    }
                                    break;
                                case "href":

                                    // Is this a self contained (<see />) element?
                                    if (isEmptyElement)
                                    {
                                        // Yes
                                        builder.AddMudLink(sequence++, link, link, "docs-link docs-code docs-code-primary", "_external");
                                    }
                                    else
                                    {
                                        // No.  Get the link text
                                        reader.Read();
                                        while (reader.NodeType != XmlNodeType.Text)
                                        {
                                            reader.Read();
                                        }
                                        builder.AddExternalMudLink(sequence++, link, reader.Value, "docs-link docs-code docs-code-secondary");
                                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "see"))
                                        {
                                            reader.Read();
                                        }
                                    }

                                    break;
                            }
                            break;
                        case "c": // Constant
                            builder.OpenElement(sequence++, "code");
                            builder.AddAttribute(sequence++, "class", "docs-code docs-code-primary");
                            if (reader.IsEmptyElement)
                            {
                                builder.CloseElement();
                            }
                            break;
                        case "para": // Paragraph
                            builder.OpenElement(sequence++, "p");
                            if (reader.IsEmptyElement)
                            {
                                builder.CloseElement();
                            }
                            break;
                        case "xml": // Start of document
                            break;
                        case "br": // Line break
                            builder.OpenElement(sequence++, "br");
                            if (reader.IsEmptyElement)
                            {
                                builder.CloseElement();
                            }
                            break;
                        default:
                            //Debugger.Break();
                            break;
                    }

                    #endregion

                    break;
                case XmlNodeType.EndElement:
                    switch (reader.Name)
                    {
                        case "c": // </c>
                            builder.CloseElement();
                            break;
                        case "para":  // </p>
                            builder.CloseElement();
                            break;
                        case "xml":
                            // End of the document
                            break;
                        default:
                            //Debugger.Break();
                            break;
                    }
                    break;
                case XmlNodeType.Text:
                    // <MudText Typo="Typo.caption">{value}</MudText>
                    builder.AddMudText(sequence++, Typo.caption, reader.Value);
                    break;
            }
        }
    }

    protected override bool ShouldRender() => Comments != null && !string.IsNullOrWhiteSpace(Comments.FullCommentText);
}
