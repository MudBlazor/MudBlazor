// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace MudBlazor.Docs.Models;

#nullable enable

/// <summary>
/// A cross-reference link from a type to another type.
/// </summary>
[DebuggerDisplay("Text={Text}, Type={Type.Name}, Member={Member.Name}")]
public sealed class DocumentedLink
{
    /// <summary>
    /// The internal type to link to.
    /// </summary>
    public DocumentedType? Type { get; set; }

    /// <summary>
    /// The property to link to.
    /// </summary>
    public DocumentedProperty? Property { get; set; }

    /// <summary>
    /// The event to link to.
    /// </summary>
    public DocumentedMethod? Method { get; set; }

    /// <summary>
    /// The field to link to.
    /// </summary>
    public DocumentedField? Field { get; set; }

    /// <summary>
    /// The event to link to.
    /// </summary>
    public DocumentedEvent? Event { get; set; }

    /// <summary>
    /// The external link to link to.
    /// </summary>
    public string? Href { get; set; }

    /// <summary>
    /// Any custom text for the link.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// For external links, gets the link as a relative path.
    /// </summary>
    /// <returns></returns>
    public string? GetLink()
    {
        return Href?.Replace("https://www.mudblazor.com/", "/", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets a name for this link.
    /// </summary>
    /// <returns></returns>
    public string GetName()
    {
        if (!string.IsNullOrEmpty(Text))
        {
            return Text;
        }
        else if (Type != null)
        {
            return Type.NameFriendly;
        }
        else if (Property != null)
        {
            return Property.Name;
        }
        else if (Method != null)
        {
            return Method.Name;
        }
        else if (Field != null)
        {
            return Field.Name;
        }
        else if (Event != null)
        {
            return Event.Name;
        }
        else if (!string.IsNullOrEmpty(Href))
        {
            return Href;
        }
        else
        {
            return "(No text)";
        }
    }
}
