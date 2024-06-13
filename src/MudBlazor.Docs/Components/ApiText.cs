// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public string Text { get; set; } = "";

    /// <summary>
    /// Occurs when <see cref="Text"/> as changed.
    /// </summary>
    public EventCallback<string> TextChanged { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
       
    }


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
                                        var type = ApiDocumentation.GetType(linkRef);
                                        if (type != null)
                                        {
                                            builder.AddDocumentedTypeLink(sequence++, type);
                                        }
                                        else
                                        {
                                            builder.AddCode(sequence++, linkRef);
                                        }
                                    }
                                    else // Property, Method, Field, or Event
                                    {
                                        var member = ApiDocumentation.GetMember(linkRef);
                                        if (member != null)
                                        {
                                            builder.AddDocumentedMemberLink(sequence++, member);
                                        }
                                        else
                                        {
                                            builder.AddCode(sequence++, linkRef);
                                        }
                                    }
                                    break;
                                case "href":
                                    builder.AddMudLink(sequence++, link.Substring(2), link.Substring(2), "docs-link docs-code docs-code-primary");
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
                        case "c": // Constant
                            builder.CloseElement();
                            break;
                        case "para":
                            builder.CloseElement();
                            break;
                    }
                    break;
                case XmlNodeType.Text:
                    builder.AddMudText(sequence++, Typo.caption, reader.Value);
                    break;
            }
        }
    }

    protected override bool ShouldRender() => !string.IsNullOrEmpty(Text);

    /// <summary>
    /// The regular expression for "see cref" XML links.
    /// </summary>
    [GeneratedRegex("<see cref=\"([FTEPM]:[\\S.]*)\"[ ]\\/>")]
    private static partial Regex SeeCrefRegEx();

    /// <summary>
    /// The regular expression for "see href" XML links.
    /// </summary>
    [GeneratedRegex("<see href=\"([\\S.]*)\"[ ]\\/>")]
    private static partial Regex SeeHrefRegEx();
}
