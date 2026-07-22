// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;

namespace MudBlazor.Docs.Models;

#nullable enable

/// <summary>
/// Represents a documented member of a type.
/// </summary>
[DebuggerDisplay("({DeclaringType.NameFriendly}) {Name}: {Summary}")]
public abstract class DocumentedMember
{
    /// <summary>
    /// The category of the member.
    /// </summary>
    /// <remarks>
    /// This value comes from the <see cref="CategoryAttribute"/> applied to the member.
    /// </remarks>
    public string Category { get; set; } = "General";

    /// <summary>
    /// The type which defines this member.
    /// </summary>
    public string? DeclaringTypeName { get; set; }

    /// <summary>
    /// The declaring type for this member.
    /// </summary>
    public DocumentedType? DeclaringType
    {
        get
        {
            if (string.IsNullOrEmpty(DeclaringTypeName))
            {
                return null;
            }
            var key = DeclaringTypeName;
            var genericsStart = DeclaringTypeName.IndexOf('[');
            if (genericsStart != -1)
            {
                key = DeclaringTypeName.Substring(0, genericsStart);
            }
            if (ApiDocumentation.Types.TryGetValue(key, out var type))
            {
                return type;
            }
            return null;
        }
    }

    /// <summary>
    /// Whether this member is only visible to inheritors.
    /// </summary>
    public bool IsProtected { get; set; }

    /// <summary>
    /// The name of this member.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The order of this member relative to other members.
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// The detailed description for this member, and any related information.
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// The brief summary of this member.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// The brief summary of this member as plain text.
    /// </summary>
    public string? SummaryPlain => Summary == null ? null : GetPlaintext(Summary);

    /// <summary>
    /// The name of the type of this member.
    /// </summary>
    public string TypeName { get; set; } = "";

    /// <summary>
    /// The type of this member.
    /// </summary>
    public DocumentedType Type => ApiDocumentation.GetType(TypeName);

    /// <summary>
    /// The user-facing name of this member's type.
    /// </summary>
    public string? TypeFriendlyName { get; set; }

    /// <summary>
    /// Extracts a plaintext version of XML documentation text.
    /// </summary>
    /// <param name="text">The XML documentation to parse.</param>
    /// <returns>The plaintext version of the specified XML documentation.</returns>
    public static string GetPlaintext(string text)
    {
        var plaintext = "";
        // Convert XML documentation text, links, and HTML to MudBlazor equivalents
        var xml = XElement.Parse("<xml>" + text + "</xml>");
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
                                            plaintext += type.NameFriendly;
                                        }
                                        else
                                        {
                                            plaintext += linkRef;
                                        }
                                    }
                                    else // Property, Method, Field, or Event
                                    {
                                        var member = ApiDocumentation.GetMember(linkRef);
                                        if (member != null)
                                        {
                                            plaintext += member.Name;
                                        }
                                        else
                                        {
                                            plaintext += linkRef;
                                        }
                                    }
                                    break;
                                case "href":
                                    plaintext += link;
                                    break;
                            }
                            break;
                    }

                    #endregion

                    break;
                case XmlNodeType.Text:
                    plaintext += reader.Value;
                    break;
            }
        }
        return plaintext;
    }
}
