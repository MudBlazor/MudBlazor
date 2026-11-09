// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Wraps popover holder updates so the popover service can batch or process operations consistently.
/// </summary>
/// <remarks>
/// This container travels through the popover update pipeline, letting the service communicate add/remove/update operations with the holders affected by the change.
/// </remarks>
public class PopoverHolderContainer
{
    /// <summary>
    /// Gets the operation associated with the container.
    /// </summary>
    public PopoverHolderOperation Operation { get; }

    /// <summary>
    /// Gets the collection of popover holders in the container.
    /// </summary>
    /// <remarks>
    /// Currently, the collection always contains one item.
    /// However, in the future, the behavior might change, and a list of updated states could be sent if the decision is made to update by batches.
    /// </remarks>
    public IReadOnlyCollection<IMudPopoverHolder> Holders { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PopoverHolderContainer"/> class.
    /// </summary>
    /// <param name="operation">The operation associated with the container.</param>
    /// <param name="holders">The collection of <see cref="IMudPopoverHolder"/>.</param>
    public PopoverHolderContainer(PopoverHolderOperation operation, IReadOnlyCollection<IMudPopoverHolder> holders)
    {
        Holders = holders;
        Operation = operation;
    }
}
