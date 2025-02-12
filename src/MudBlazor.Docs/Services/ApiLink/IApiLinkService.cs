using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor.Docs.Services;

#nullable enable
public interface IApiLinkService
{
    /// <summary>
    /// Registers a new page to the search index for a component.
    /// </summary>
    /// <param name="title">The title of the page</param>
    /// <param name="subtitle">The subtitle of the page</param>
    /// <param name="componentType">The type of the component</param>
    /// <param name="link">The link to the page</param>
    void RegisterPage(string title, string? subtitle, Type? componentType, string? link);

    /// <summary>
    /// Returns the search results for the specified text using a fuzzy algorithm.
    /// </summary>
    /// <param name="text">The search query</param>
    Task<IReadOnlyCollection<ApiLinkServiceEntry>> Search(string text);
}
