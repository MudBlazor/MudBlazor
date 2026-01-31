// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities;

#nullable enable

/// <summary>
/// A lightweight stopwatch implementation using <see cref="TimeProvider"/> for testability.
/// Unlike <see cref="System.Diagnostics.Stopwatch"/>, this struct uses the injected time provider,
/// allowing deterministic testing with <c>FakeTimeProvider</c>.
/// </summary>
internal struct ValueStopwatch
{
    private readonly TimeProvider _timeProvider;
    private long _startTimestamp;
    private long _accumulatedTicks;
    private bool _isRunning;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStopwatch"/> struct.
    /// </summary>
    /// <param name="timeProvider">The time provider to use for timestamps.</param>
    public ValueStopwatch(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
        _startTimestamp = 0;
        _accumulatedTicks = 0;
        _isRunning = false;
    }

    /// <summary>
    /// Gets a value indicating whether the stopwatch is currently running.
    /// </summary>
    public readonly bool IsRunning => _isRunning;

    /// <summary>
    /// Gets the total elapsed time measured by the stopwatch.
    /// </summary>
    public readonly TimeSpan Elapsed
    {
        get
        {
            var ticks = _accumulatedTicks;
            if (_isRunning)
            {
                var currentTimestamp = _timeProvider.GetTimestamp();
                ticks += currentTimestamp - _startTimestamp;
            }

            return _timeProvider.GetElapsedTime(0, ticks);
        }
    }

    /// <summary>
    /// Gets the total elapsed time in milliseconds.
    /// </summary>
    public readonly long ElapsedMilliseconds => (long)Elapsed.TotalMilliseconds;

    /// <summary>
    /// Starts or resumes measuring elapsed time.
    /// </summary>
    public void Start()
    {
        if (_isRunning)
        {
            return;
        }

        _startTimestamp = _timeProvider.GetTimestamp();
        _isRunning = true;
    }

    /// <summary>
    /// Stops measuring elapsed time.
    /// </summary>
    public void Stop()
    {
        if (!_isRunning)
        {
            return;
        }

        var currentTimestamp = _timeProvider.GetTimestamp();
        _accumulatedTicks += currentTimestamp - _startTimestamp;
        _isRunning = false;
    }

    /// <summary>
    /// Resets the stopwatch to zero and starts measuring.
    /// </summary>
    public void Restart()
    {
        _accumulatedTicks = 0;
        _startTimestamp = _timeProvider.GetTimestamp();
        _isRunning = true;
    }

    /// <summary>
    /// Resets the stopwatch to zero without starting it.
    /// </summary>
    public void Reset()
    {
        _accumulatedTicks = 0;
        _startTimestamp = 0;
        _isRunning = false;
    }

    /// <summary>
    /// Creates a new <see cref="ValueStopwatch"/> and starts it immediately.
    /// </summary>
    /// <param name="timeProvider">The time provider to use for timestamps.</param>
    /// <returns>A running stopwatch.</returns>
    public static ValueStopwatch StartNew(TimeProvider timeProvider)
    {
        var stopwatch = new ValueStopwatch(timeProvider);
        stopwatch.Start();
        return stopwatch;
    }
}
