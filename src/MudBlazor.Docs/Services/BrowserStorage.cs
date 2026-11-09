// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using Microsoft.JSInterop;

#nullable enable

namespace MudBlazor.Docs.Services;

/// <summary>
/// Stores JSON values in the browser's local storage.
/// </summary>
/// <remarks>
/// Calls go through JS interop, so they can only run once a component is interactive, such as in <c>OnAfterRenderAsync</c>, and never during prerendering.
/// </remarks>
public interface IBrowserStorage
{
    /// <summary>
    /// Reads the value stored under <paramref name="key"/>.
    /// </summary>
    /// <returns>
    /// The stored value, or <c>default</c> when the key is missing, the browser blocks storage, or the stored text is not valid JSON for <typeparamref name="T"/>.
    /// </returns>
    ValueTask<T?> GetAsync<T>(string key);

    /// <summary>
    /// Stores <paramref name="value"/> as JSON under <paramref name="key"/>.
    /// </summary>
    /// <remarks>
    /// Does nothing when the browser blocks storage.
    /// </remarks>
    ValueTask SetAsync<T>(string key, T value);
}

/// <inheritdoc cref="IBrowserStorage"/>
public sealed class BrowserStorage(IJSRuntime jsRuntime) : IBrowserStorage
{
    public async ValueTask<T?> GetAsync<T>(string key)
    {
        var json = await jsRuntime.InvokeAsync<string?>("mudBlazorDocs.getStorageItem", key);
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (JsonException)
        {
            // A value the current type can't read is treated as missing, so the next save replaces it.
            return default;
        }
    }

    public ValueTask SetAsync<T>(string key, T value)
    {
        return jsRuntime.InvokeVoidAsync("mudBlazorDocs.setStorageItem", key, JsonSerializer.Serialize(value));
    }
}
