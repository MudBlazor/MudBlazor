// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Threading;

namespace MudBlazor.Utilities.Background.Periodic;

#nullable enable
/// <summary>
/// Represents a handler for the elapsed event of a periodic timer.
/// </summary>
internal interface IPeriodicTimerElapsedHandler
{
    /// <summary>
    /// Handles the asynchronous event when the periodic timer elapses.
    /// </summary>
    /// <param name="stoppingToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OnPeriodicTimerElapsedAsync(CancellationToken stoppingToken = default);
}
