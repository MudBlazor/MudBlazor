// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a service for managing popovers.
/// </summary>
public interface IPopoverService : IAsyncDisposable
{
    /// <summary>
    /// Gets the current popover options.
    /// </summary>
    PopoverOptions PopoverOptions { get; }

    /// <summary>
    /// Gets the collection of active popovers that were created via <see cref="CreatePopoverAsync"/>. Disappears from collection after calling <see cref="DestroyPopoverAsync"/>.
    /// </summary>
    IEnumerable<IMudPopoverHolder> ActivePopovers { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="IPopoverService"/> is initialized.
    /// </summary>
    /// <value>
    /// <c>true</c> if the <see cref="IPopoverService"/> is initialized; otherwise, <c>false</c>.
    /// </value>
    bool IsInitialized { get; }

    /// <summary>
    /// Subscribes an observer to receive popover update notifications.
    /// </summary>
    /// <param name="observer">The observer to subscribe.</param>
    void Subscribe(IPopoverObserver observer);

    /// <summary>
    /// Unsubscribes an observer from receiving popover update notifications.
    /// </summary>
    /// <param name="observer">The observer to unsubscribe.</param>
    void Unsubscribe(IPopoverObserver observer);

    /// <summary>
    /// Creates a popover.
    /// </summary>
    /// <param name="popover">The popover to create.</param>
    Task CreatePopoverAsync(IPopover popover);

    /// <summary>
    /// Updates an existing popover.
    /// </summary>
    /// <param name="popover">The popover to update.</param>
    /// <returns><c>true</c> if the update was successful; otherwise, <c>false</c>.</returns>
    Task<bool> UpdatePopoverAsync(IPopover popover);

    /// <summary>
    /// Destroys a popover.
    /// </summary>
    /// <param name="popover">The popover to destroy.</param>
    /// <returns>The task result indicates whether the popover was successfully destroyed.</returns>
    Task<bool> DestroyPopoverAsync(IPopover popover);

    /// <summary>
    /// Counts the number of popover providers.
    /// </summary>
    /// <returns>The task result contains the count of popover providers.</returns>
    ValueTask<int> GetProviderCountAsync();
}
