// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MudBlazor.Utilities.Background.Periodic;

#nullable enable
/// <summary>
/// Represents a background worker that executes a periodic task at specified intervals.
/// </summary>
internal class PeriodicWorker : BackgroundWorkerBase
{
    private readonly PeriodicTimer _periodicTimer;
    private readonly IPeriodicTimerElapsedHandler _handler;
    private bool _isInitialized;
    private bool _isInitializing;

    /// <summary>
    /// Initializes a new instance of the <see cref="PeriodicWorker"/> class with the specified periodic timer handler and interval.
    /// </summary>
    /// <param name="handler">The handler for the periodic timer elapsed event.</param>
    /// <param name="period">The interval at which the periodic task should execute.</param>
    public PeriodicWorker(IPeriodicTimerElapsedHandler handler, TimeSpan period)
    {
        _handler = handler;
        _periodicTimer = new PeriodicTimer(period);
    }

    /// <inheritdoc/>
    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialized)
        {
            return;
        }

        if (_isInitializing)
        {
            return;
        }

        try
        {
            _isInitializing = true;

            // Double-check if initialization has been completed by another thread.
            if (_isInitialized)
            {
                return;
            }

            await base.StartAsync(cancellationToken);

            _isInitialized = true;
        }
        finally
        {
            _isInitializing = false;
        }
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _periodicTimer.WaitForNextTickAsync(stoppingToken))
        {
            await _handler.OnPeriodicTimerElapsedAsync(stoppingToken);
        }
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        _periodicTimer.Dispose();
        base.Dispose();
    }
}
