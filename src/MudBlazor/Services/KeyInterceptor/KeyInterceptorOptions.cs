// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor.Services;

#nullable enable
/// <summary>
/// Configuration options for key interception.
/// </summary>
public class KeyInterceptorOptions
{
    /// <summary>
    /// The CSS class of the target HTML element that should be observed for keyboard events.
    /// </summary>
    /// <remarks>
    /// Note: This must be a single class name.
    /// </remarks>
    public string? TargetClass { get; set; }

    /// <summary>
    /// Specifies whether resize events should be logged in the browser's console.
    /// </summary>
    public bool EnableLogging { get; set; } = false;

    /// <summary>
    /// A list of key options that define the keys to intercept and their respective configurations.
    /// </summary>
    public List<KeyOptions> Keys { get; set; } = new();
}
