// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
public sealed partial class ApiText : ComponentBase
{
    /// <summary>
    /// The XML documentation text to parse.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = "";

    /// <summary>
    /// The color of the text.
    /// </summary>
    [Parameter]
    public Color Color { get; set; } = Color.Default;

    /// <summary>
    /// The size of the text.
    /// </summary>
    [Parameter]
    public Typo Typo { get; set; } = Typo.caption;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var sequence = 0;
        // Anything to do?
        if (string.IsNullOrWhiteSpace(Text))
        {
            return;
        }
        // Convert XML documentation text, links, and HTML to MudBlazor equivalents
        XElement xml;
        try
        {
            xml = XElement.Parse("<xml>" + Text + "</xml>");
        }
        catch
        {
            // The XML is malformed somehow.  Warn and exit
            builder.AddMudText(0, Typo, Color.Warning, "XML documentation error.");
            return;
        }
        // Start with a <span> to wrap properly on mobile
        builder.OpenElement(sequence++, "span");
        using var reader = xml.CreateReader();
        while (reader.Read())
        {
            var isEmptyElement = reader.IsEmptyElement;
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
                                            builder.AddMudLink(sequence++, $"https://learn.microsoft.com/dotnet/api/{msLink}", className, Typo, "docs-link docs-code docs-code-primary", "_external", (linkSequence, linkBuilder) =>
                                            {
                                                linkBuilder.AddContent(linkSequence++, className);
                                                linkBuilder.AddMudTooltip(linkSequence++, Placement.Top, $"External Link", (tooltipSequence, tooltipBuilder) =>
                                                {
                                                    tooltipBuilder.AddMudIcon(tooltipSequence++, "MudBlazor.Icons.Material.Filled.Link", Color.Default, Size.Small);
                                                });
                                            });
                                        }
                                        else
                                        {
                                            // Add a link to the type
                                            builder.OpenComponent<ApiTypeLink>(sequence++);
                                            builder.AddComponentParameter(sequence++, "TypeName", linkRef);
                                            builder.AddComponentParameter(sequence++, "Typo", Typo);
                                            builder.CloseComponent();
                                        }
                                    }
                                    else // Property, Method, Field, or Event
                                    {
                                        var member = ApiDocumentation.GetMember(linkRef);
                                        if (member != null)
                                        {
                                            builder.AddDocumentedMemberLink(sequence++, member, Typo);
                                        }
                                        else if (linkRef.StartsWith("MudBlazor.Icons"))
                                        {
                                            builder.AddMudTooltip(sequence++, Placement.Top, linkRef.Replace("MudBlazor.", ""), (childSequence, childBuilder) =>
                                            {
                                                childBuilder.AddMudIcon(childSequence++, linkRef, Color.Primary, Size.Medium);
                                            });
                                        }
                                        else if (linkRef != null && (linkRef.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) || linkRef.StartsWith("System", StringComparison.OrdinalIgnoreCase)))
                                        {
                                            // Get the class name and member name
                                            var parts = linkRef.Split(".");
                                            var className = parts[parts.Length - 2].Replace("`1", "<T>").Replace("`2", "<T, U>");
                                            var memberName = parts[parts.Length - 1];
                                            // Calculate the Microsoft Docs link
                                            var msLink = linkRef.Replace("`1", "-1").Replace("`2", "-2").ToLowerInvariant();
                                            builder.AddMudLink(sequence++, $"https://learn.microsoft.com/dotnet/api/{msLink}", className + "." + memberName, Typo, "docs-link docs-code docs-code-primary", "_external", (linkSequence, linkBuilder) =>
                                            {
                                                linkBuilder.AddContent(linkSequence++, className + "." + memberName);
                                                linkBuilder.AddMudTooltip(linkSequence++, Placement.Top, $"External Link", (tooltipSequence, tooltipBuilder) =>
                                                {
                                                    tooltipBuilder.AddMudIcon(tooltipSequence++, "MudBlazor.Icons.Material.Filled.Link", Color.Default, Size.Small);
                                                });
                                            });
                                        }
                                        else
                                        {
                                            builder.AddCode(sequence++, linkRef);
                                        }
                                    }
                                    break;
                                case "href":
                                    if (string.IsNullOrWhiteSpace(link))
                                    {
                                        continue;
                                    }
                                    if (isEmptyElement)
                                    {
                                        builder.AddMudLink(sequence++, link, link, Typo, "docs-link docs-code docs-code-primary", "_external", (linkSequence, linkBuilder) =>
                                        {
                                            linkBuilder.AddContent(linkSequence++, link);
                                            linkBuilder.AddMudTooltip(linkSequence++, Placement.Top, $"External Link", (tooltipSequence, tooltipBuilder) =>
                                            {
                                                tooltipBuilder.AddMudIcon(tooltipSequence++, "MudBlazor.Icons.Material.Filled.Link", Color.Default, Size.Small);
                                            });
                                        });
                                    }
                                    else
                                    {
                                        // Move to the link content
                                        reader.Read();
                                        var text = string.IsNullOrEmpty(reader.Value) ? link : reader.Value;

                                        builder.AddMudLink(sequence++, link, text, Typo, "docs-link docs-code docs-code-primary", "_external", (linkSequence, linkBuilder) =>
                                        {
                                            linkBuilder.AddContent(linkSequence++, text);
                                            linkBuilder.AddMudTooltip(linkSequence++, Placement.Top, $"External Link", (tooltipSequence, tooltipBuilder) =>
                                            {
                                                tooltipBuilder.AddMudIcon(tooltipSequence++, "MudBlazor.Icons.Material.Filled.Link", Color.Default, Size.Small);
                                            });
                                        });
                                    }
                                    break;
                            }
                            break;
                        case "c": // Constant
                            builder.OpenElement(sequence++, "code");
                            builder.AddAttribute(sequence++, "class", "docs-code docs-code-primary");
                            if (isEmptyElement)
                            {
                                builder.CloseElement();
                            }
                            break;
                        case "para": // Paragraph
                            builder.OpenElement(sequence++, "p");
                            if (isEmptyElement)
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
                    builder.AddMudText(sequence++, Typo, Color, reader.Value);
                    break;
            }
        }
        builder.CloseElement(); // </span>
    }
}
