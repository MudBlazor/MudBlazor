// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Services;

#nullable enable

/// <summary>
/// Fetches raw example sources that are shipped as static assets instead of being embedded in the assembly.
/// </summary>
public interface ISnippetsService
{
    /// <summary>
    /// Gets the raw source for an example by name, or <c>null</c> if it does not exist.
    /// </summary>
    Task<string?> GetSourceAsync(string name);
}

/// <summary>
/// Loads example sources on demand from <c>_content/MudBlazor.Docs/snippets</c> and caches them.
/// </summary>
public sealed class SnippetsService : ISnippetsService
{
    private const string BasePath = "_content/MudBlazor.Docs/snippets/";

    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, string?> _cache = [];

    public SnippetsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<string?> GetSourceAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        // A few small snippets (e.g. Element, Server) are still compiled-in constants.
        var compiledIn = Snippets.GetCode(name);
        if (compiledIn != null)
        {
            return compiledIn;
        }

        if (_cache.TryGetValue(name, out var cached))
        {
            return cached;
        }

        string? source = null;
        try
        {
            source = await _httpClient.GetStringAsync($"{BasePath}{name}.txt");
        }
        catch (HttpRequestException)
        {
            // The snippet does not exist; callers fall back to reading the rendered code from the DOM.
        }

        _cache[name] = source;
        return source;
    }
}
