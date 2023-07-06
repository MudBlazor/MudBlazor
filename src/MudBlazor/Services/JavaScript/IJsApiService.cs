// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace MudBlazor;

#nullable enable
/// <summary>
/// An interface for interacting with JavaScript APIs.
/// </summary>
public interface IJsApiService
{
    /// <summary>
    /// Calls JS method "navigator.clipboard.writeText". Writes the specified text string to the system clipboard
    /// </summary>
    /// <param name="text">The string to be written to the clipboard.</param>
    ValueTask CopyToClipboardAsync(string text);

    /// <summary>
    /// Calls JS method "window.open". Opens specified url in new tab.
    /// </summary>
    /// <param name="url">A string indicating the URL or path of the resource to be loaded.</param>
    ValueTask OpenInNewTabAsync(string url);

    /// <summary>
    /// Calls JS method "window.open". The open() method of the Window interface loads a specified resource into a new or existing browsing context (that is, a tab, a window, or an iframe) under a specified name.
    /// </summary>
    /// <param name="url">A string indicating the URL or path of the resource to be loaded. If an empty string ("") is specified, a blank page is opened into the targeted browsing context.</param>
    /// <param name="target">A string, without whitespace, specifying the name of the browsing context the resource is being loaded into. If the name doesn't identify an existing context, a new context is created and given the specified name. The special target keywords, _self, _blank, _parent, and _top, can also be used.</param>
    ValueTask Open(string url, string target);
}
