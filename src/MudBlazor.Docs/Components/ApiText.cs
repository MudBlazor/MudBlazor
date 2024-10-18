// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// Represents the summary or remarks of an object, with linking.
/// </summary>
public partial class ApiText : ComponentBase
{
    /// <summary>
    /// The XML documentation text to parse.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = "";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var sequence = 0;

        // Convert XML documentation text, links, and HTML to MudBlazor equivalents
        var xml = XElement.Parse("<xml>" + Text + "</xml>");
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
                                        // Is this an external type?
                                        if (linkRef != null && (linkRef.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) || linkRef.StartsWith("System", StringComparison.OrdinalIgnoreCase)))
                                        {
                                            // Get the class name and member name
                                            var parts = linkRef.Split(".");
                                            var className = parts[parts.Length - 1].Replace("`1", "<T>").Replace("`2", "<T, U>");
                                            // Calculate the Microsoft Docs link
                                            var msLink = linkRef.Replace("`1", "-1").Replace("`2", "-2").ToLowerInvariant();
                                            builder.AddMudLink(0, $"https://learn.microsoft.com/dotnet/api/{msLink}", className, "docs-link docs-code docs-code-secondary", "_external");
                                        }
                                        else
                                        {
                                            // Add a link to the type
                                            builder.OpenComponent<ApiTypeLink>(sequence++);
                                            builder.AddComponentParameter(sequence++, "TypeName", linkRef);
                                            builder.CloseComponent();
                                        }
                                    }
                                    else // Property, Method, Field, or Event
                                    {
                                        var member = ApiDocumentation.GetMember(linkRef);
                                        if (member != null)
                                        {
                                            builder.AddDocumentedMemberLink(sequence++, member);
                                        }
                                        else if (linkRef.StartsWith("MudBlazor.Icons"))
                                        {
                                            builder.AddMudIcon(sequence++, linkRef, Color.Primary, Size.Medium);
                                        }
                                        else if (linkRef != null && (linkRef.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) || linkRef.StartsWith("System", StringComparison.OrdinalIgnoreCase)))
                                        {
                                            // Get the class name and member name
                                            var parts = linkRef.Split(".");
                                            var className = parts[parts.Length - 2].Replace("`1", "<T>").Replace("`2", "<T, U>");
                                            var memberName = parts[parts.Length - 1];
                                            // Calculate the Microsoft Docs link
                                            var msLink = linkRef.Replace("`1", "-1").Replace("`2", "-2").ToLowerInvariant();
                                            builder.AddMudLink(0, $"https://learn.microsoft.com/dotnet/api/{msLink}", className + "." + memberName, "docs-link docs-code docs-code-secondary", "_external");
                                        }
                                        else
                                        {
                                            builder.AddCode(sequence++, linkRef);
                                        }
                                    }
                                    break;
                                case "href":
                                    if (reader.IsEmptyElement)
                                    {
                                        builder.AddMudLink(sequence++, link, link, "docs-link docs-code docs-code-secondary", "_external");
                                    }
                                    else
                                    {
                                        // Move to the href text
                                        reader.Read();
                                        builder.AddMudLink(sequence++, link, reader.Value, "docs-link docs-code docs-code-secondary", "_external");
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
                    }
                    break;
                case XmlNodeType.Text:
                    // <MudText Typo="Typo.caption">{value}</MudText>
                    builder.AddMudText(sequence++, Typo.caption, reader.Value);
                    break;
            }
        }
    }
}
