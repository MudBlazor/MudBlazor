// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MudBlazor.Utilities.Background.Batch;

#nullable enable
/// <summary>
/// Represents a handler for batch timer events in conjunction with <see cref="BatchPeriodicQueue{T}"/>.
/// </summary>
/// <typeparam name="TItems">The type of items handled by the batch timer.</typeparam>
internal interface IBatchTimerHandler<in TItems>
{
    /// <summary>
    /// Handles the batch timer elapsed event asynchronously.
    /// </summary>
    /// <param name="items">The collection of items to handle.</param>
    /// <param name="stoppingToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnBatchTimerElapsedAsync(IReadOnlyCollection<TItems> items, CancellationToken stoppingToken = default);
}
