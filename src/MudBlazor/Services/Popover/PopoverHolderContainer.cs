// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a container for <see cref="IMudPopoverHolder"/>, along with the associated <see cref="PopoverHolderOperation"/>.
/// </summary>
public class PopoverHolderContainer
{
    /// <summary>
    /// Gets the operation associated with the container.
    /// </summary>
    public PopoverHolderOperation Operation { get; }

    /// <summary>
    /// Gets the collection of popover holders in the container.
    /// </summary>
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
