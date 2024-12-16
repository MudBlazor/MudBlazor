// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// A cross-reference link from a type to another type.
/// </summary>
[DebuggerDisplay("Cref={Cref}, Href={Href}, Text={Text}")]
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
    /// The link to a type or member.
    /// </summary>
    public string? Cref { get; set; }

    /// <summary>
    /// The external link to link to.
    /// </summary>
    public string? Href { get; set; }

    /// <summary>
    /// Any custom text for the link.
    /// </summary>
    public string? Text { get; set; }
}
