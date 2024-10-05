// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable

/// <summary>
/// Specifies the mode for displaying popovers.
/// </summary>
public enum PopoverMode
{
    /// <summary>
    /// The default popover mode that uses the <see cref="IPopoverService"/>.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The legacy popover mode used for backward compatibility, which utilizes the old <see cref="IMudPopoverService"/> instead of <see cref="IPopoverService"/>.
    /// </summary>
    /// <remarks>
    /// This property is only for backward compatibility with old behaviour. This will be removed in a future version. 
    /// </remarks>
    Legacy = 1,
}
