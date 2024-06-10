// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// Represents the summary or remarks of an object, with linking.
/// </summary>
public partial class ApiText
{
    /// <summary>
    /// The XML documentation text to parse.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = "";

    /// <summary>
    /// The documented type related to this text.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public DocumentedType? Type { get; set; }

    /// <summary>
    /// The HTML representation of the XML documentation text.
    /// </summary>
    public string? Html { get; set; }

    /// <summary>
    /// The XML documentation elements and their HTML equivalents.
    /// </summary>
    public static Dictionary<string, string> XmlToHtmlElements { get; private set; } = new()
    {
        { "<c>", "<code class=\"docs-code docs-code-primary\">" },
        { "</c>", "</code>" },
        { "<para>", "<p>" },
        { "</para>", "</p>" }
    };

    /// <summary>
    /// Gets the specified XML documentation string as HTML.
    /// </summary>
    /// <param name="xml">The XML content to convert.</param>
    /// <returns></returns>
    public string? ToHtml(string? xml)
    {
        // Anything to do?
        if (string.IsNullOrEmpty(xml)) { return null; }
        // Convert common XML documentation elements to HTML
        foreach (var pair in XmlToHtmlElements)
        {
            xml = xml.Replace(pair.Key, pair.Value, StringComparison.OrdinalIgnoreCase);
        }
        // Replace "see cref" with docs links
        foreach (var match in SeeCrefRegEx().Matches(xml).Cast<Match>())
        {
            // If it's a type, we can link to it
            if (match.Groups[1].Value.StartsWith("T:"))
            {
                // Try to find the type
                var typeName = match.Groups[1].Value.Substring(2);
                var existingType = ApiDocumentation.GetType(typeName);
                if (existingType == null)
                {
                    // Calculate the text to display (removes "MudBlazor." and the current type)
                    var friendlyName = typeName
                        .Replace("MudBlazor.", "")                  // Remove the namespace
                        .Replace(Type?.Name + ".", "")              // Exclude the type
                        .Replace(Type?.BaseType?.Name + ".", "");   // Exclude the base type
                                                                    // Property, Method, or Event  (no link)
                    xml = xml.Replace(match.Groups[0].Value, $"<code class=\"docs-code docs-code-primary\">{friendlyName}</code>");
                }
                else
                {
                    xml = xml.Replace(match.Groups[0].Value, $"<a class=\"docs-code docs-code-primary\" href=\"{existingType.ApiUrl}\">{existingType.Name}</a>");
                }
            }
            else
            {
                // Try to find the member
                var memberName = match.Groups[1].Value.Substring(2);
                var existingMember = ApiDocumentation.GetMember(memberName);
                if (existingMember == null)
                {
                    // Calculate the text to display (removes "MudBlazor." and the current type)
                    var friendlyName = match.Groups[1].Value.Substring(2)
                        .Replace("MudBlazor.", "")                  // Remove the namespace
                        .Replace(Type?.Name + ".", "")              // Exclude the type
                        .Replace(Type?.BaseType?.Name + ".", "");   // Exclude the base type

                    // Property, Method, or Event  (no link)
                    xml = xml.Replace(match.Groups[0].Value, $"<code class=\"docs-code docs-code-primary\">{friendlyName}</code>");
                }
                else
                {
                    xml = xml.Replace(match.Groups[0].Value, $"<a class=\"docs-code docs-code-primary\" href=\"{existingMember.DeclaringTypeApiUrl}#{existingMember.Name}\">{existingMember.Name}</a>");
                }
            }
        }

        // Replace "see href" with links to a new browser tab
        foreach (var match in SeeHrefRegEx().Matches(xml).Cast<Match>())
        {
            xml = xml.Replace(match.Groups[0].Value, $"<a class=\"docs-link\" target=\"_external\" href=\"{match.Groups[1].Value}\"><code class=\"docs-code docs-code-primary\">{match.Groups[1].Value}</code></a>");
        }

        return xml;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Html = ToHtml(Text);
        StateHasChanged();
    }

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
