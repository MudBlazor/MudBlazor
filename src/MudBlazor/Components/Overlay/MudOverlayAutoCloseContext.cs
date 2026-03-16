// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Provides internal configuration for modeless overlay auto-close behavior.
/// </summary>
internal sealed class MudOverlayAutoCloseContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MudOverlayAutoCloseContext"/> class.
    /// </summary>
    /// <param name="excludeElementIds">Element IDs that should not trigger overlay auto-close hit testing.</param>
    public MudOverlayAutoCloseContext(string[]? excludeElementIds)
    {
        ExcludeElementIds = excludeElementIds;
    }

    /// <summary>
    /// Gets the element IDs that should not trigger overlay auto-close hit testing.
    /// </summary>
    public string[]? ExcludeElementIds { get; }
}
