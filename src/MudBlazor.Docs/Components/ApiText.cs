// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;
using MudBlazor.State;

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
                                        // Add a link to the type
                                        builder.OpenComponent<ApiTypeLink>(sequence++);
                                        builder.AddComponentParameter(sequence++, "TypeName", linkRef);
                                        builder.CloseComponent();
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
                                            builder.AddMudLink(0, $"https://learn.microsoft.com/en-us/dotnet/api/{linkRef}", linkRef, "docs-link docs-code docs-code-primary", "_external");
                                        }
                                        else
                                        {
                                            builder.AddCode(sequence++, linkRef);
                                        }
                                    }
                                    break;
                                case "href":
                                    builder.AddMudLink(sequence++, link, link, "docs-link docs-code docs-code-primary", "_external");
                                    break;
                            }
                            break;
                        case "c": // Constant
                            builder.OpenElement(sequence++, "code");
                            builder.AddAttribute(sequence++, "class", "docs-code docs-code-primary");
                            break;
                        case "para": // Paragraph
                            builder.OpenElement(sequence++, "p");
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

    protected override bool ShouldRender() => !string.IsNullOrEmpty(Text);
}
