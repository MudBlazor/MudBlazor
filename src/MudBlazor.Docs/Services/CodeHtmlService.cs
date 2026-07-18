// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Services;

#nullable enable

/// <summary>
/// Fetches syntax-highlighted example markup that is shipped as static assets instead of being embedded in the assembly.
/// </summary>
public interface ICodeHtmlService
{
    /// <summary>
    /// Gets the highlighted HTML for an example by name, or <c>null</c> if it does not exist.
    /// </summary>
    Task<string?> GetHtmlAsync(string name);
}

/// <summary>
/// Loads example markup on demand from <c>_content/MudBlazor.Docs/code</c> and caches it.
/// </summary>
public sealed class CodeHtmlService : ICodeHtmlService
{
    private const string BasePath = "_content/MudBlazor.Docs/code/";

    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, string?> _cache = [];

    public CodeHtmlService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<string?> GetHtmlAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        if (_cache.TryGetValue(name, out var cached))
        {
            return cached;
        }

        string? html = null;
        try
        {
            html = await _httpClient.GetStringAsync($"{BasePath}{name}.html");
        }
        catch (HttpRequestException)
        {
            // The example markup does not exist; the code panel simply renders empty.
        }

        _cache[name] = html;
        return html;
    }
}
