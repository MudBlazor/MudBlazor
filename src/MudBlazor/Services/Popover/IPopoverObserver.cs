// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents an observer for popover updates.
/// </summary>
public interface IPopoverObserver
{
    /// <summary>
    /// Gets the unique identifier of the observer.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Notifies the observer of a popover collection update in <see cref="IPopoverService.ActivePopovers"/>.
    /// This notification is triggered only when <see cref="IPopoverService.CreatePopoverAsync"/> or <see cref="IPopoverService.DestroyPopoverAsync"/> is called.
    /// </summary>
    /// <param name="holders">Collection of the updated holder of the popover.</param>
    /// <remarks>
    /// Please note that this notification will not be triggered when <see cref="IPopoverService.UpdatePopoverAsync"/> is called, but this might change in future.
    /// Currently, the <paramref name="holders"/> collection always contains one item. However, in the future, the behavior might change, and a list of updated states could be sent if the decision is made to update by batches.
    /// </remarks>
    public Task PopoverCollectionUpdatedNotification(IEnumerable<IMudPopoverHolder> holders);
}
