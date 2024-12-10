using System;
using System.Collections.Generic;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Services;

#nullable enable
public interface IMenuService
{
    IEnumerable<MudComponent> Components { get; }

    IEnumerable<MudComponent> Api { get; }

    MudComponent? GetParent(Type? type);

    MudComponent? GetComponent(Type? type);

    /// <summary>
    /// Gets a component by its type name.
    /// </summary>
    /// <param name="typeName">The name of the type, such as <c>MudAlert</c>.</param>
    /// <returns>The matching component, or <c>null</c> if none was found.</returns>
    string? GetComponentName(string typeName);

    IEnumerable<DocsLink> Features { get; }

    IEnumerable<DocsLink> Customization { get; }

    IEnumerable<DocsLink> Utilities { get; }

    /// <summary>
    /// Gets the menu for example for the specified type.
    /// </summary>
    /// <param name="type">The type to examine.</param>
    /// <returns>When <c>true</c>, the menu service has a record of this type.</returns>
    MudComponent? GetExample(DocumentedType type);
}
