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

    IEnumerable<DocsLink> Features { get; }

    IEnumerable<DocsLink> Customization { get; }

    IEnumerable<DocsLink> Utilities { get; }
}
