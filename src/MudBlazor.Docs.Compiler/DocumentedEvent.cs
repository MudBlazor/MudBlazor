// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// Represents a documented event (including EventCallback properties).
/// </summary>
public class DocumentedEvent : DocumentedMember
{
    /// <summary>
    /// Whether this property is a parameter.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, the <see cref="Microsoft.AspNetCore.Components.ParameterAttribute"/> is applied to this property.
    /// </remarks>
    public bool IsParameter { get; set; }

    /// <summary>
    /// The property which triggers this event.
    /// </summary>
    /// <remarks>
    /// When set, this event enables binding for a property via <c>@bind-[Property]</c> in Razor.
    /// </remarks>
    public DocumentedProperty? Property { get; set; }
}
