// Copyright © 2021 Jeffrey Jangli

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MudBlazor.Utilities
{
    /// <summary>
    /// Provides a mechanism for executing a method on a thread pool thread at specified intervals.
    /// </summary>
    public interface IComponentTimer : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the timer is enabled.
        /// </summary>
        /// <remarks>
        /// Setting the value to <c>true</c> is the same as calling the <see cref="Start()"/> method.
        /// Setting the value to <c>false</c> is the same as calling the <see cref="Stop()"/> method.
        /// </remarks>
        /// <returns>
        /// <c>true</c> if the timer is enabled; otherwise, <c>false</c>.
        /// </returns>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether the timer is running.
        /// </summary>
        /// <remarks>
        /// <c>true</c> is returned when <see cref="Enabled"/> is <c>true</c> and <see cref="DueTime"/> and <see cref="Period"/> have valid values.
        /// </remarks>
        /// <returns>
        /// A value indicating whether the timer is running.
        /// </returns>
        bool IsTicking { get; }

        /// <summary>
        /// Gets the number of times that the timer was restarted.
        /// </summary>
        /// <remarks>
        /// This information is available even when the timer is already disposed.
        /// </remarks>
        /// <returns>
        /// The number of times that the timer was restarted.
        /// </returns>
        int Restarts { get; }

        /// <summary>
        /// Gets or sets the timer delay before executing the callback method.
        /// </summary>
        /// <remarks>
        /// The timer needs to be restarted to effectuate the new value.
        /// </remarks>
        /// <returns>
        /// The timer delay before executing the callback method.
        /// </returns>
        TimeSpan DueTime { get; set; }

        /// <summary>
        /// Gets or sets the amount of time to wait between subsequent executions.
        /// </summary>
        /// <remarks>
        /// The timer needs to be restarted to effectuate the new value.
        /// </remarks>
        /// <returns>
        /// The amount of time to wait between subsequent executions.
        /// </returns>
        TimeSpan Period { get; set; }

        /// <summary>
        /// Gets the total elapsed time since the timer was last started.
        /// </summary>
        /// <returns>
        /// The total elapsed time since the timer was last started.
        /// </returns>
        TimeSpan Elapsed { get; }


        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <remarks>
        /// The timer will only start when <see cref="Enabled"/> is <c>false</c>.
        /// Calling this method is the same as setting <see cref="Enabled"/> to <c>true</c>.
        /// </remarks>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was started.
        /// <c>null</c> is returned if the timer failed to start.
        /// </returns>
        DateTimeOffset? Start();

        /// <summary>
        /// Starts the timer specifying a due time (start delay).
        /// </summary>
        /// <remarks>
        /// The timer will only start when <see cref="Enabled"/> is <c>false</c>.
        /// </remarks>
        /// <param name="dueTime">The amount of time to delay before invoking the callback method specified when the Timer was constructed, in milliseconds. Specify Infinite to prevent the timer from restarting. Specify zero (0) to restart the timer immediately.</param>
        /// <param name="update">When <c>true</c> the instance property is updated when the timer is started successfully.</param>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was started.
        /// <c>null</c> is returned if the timer failed to start.
        /// </returns>
        DateTimeOffset? Start(int dueTime, bool update = false);

        /// <summary>
        /// Starts the timer specifying a due time (start delay).
        /// </summary>
        /// <remarks>
        /// The timer will only start when <see cref="Enabled"/> is <c>false</c>.
        /// </remarks>
        /// <param name="dueTime">The amount of time to delay before invoking the callback method specified when the Timer was constructed, in milliseconds. Specify Infinite to prevent the timer from restarting. Specify zero (0) to restart the timer immediately.</param>
        /// <param name="update">When <c>true</c> the instance property is updated when the timer is started successfully.</param>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was started.
        /// <c>null</c> is returned if the timer failed to start.
        /// </returns>
        DateTimeOffset? Start(TimeSpan dueTime, bool update = false);

        /// <summary>
        /// Asynchronously starts the timer.
        /// </summary>
        /// <remarks>
        /// The timer will only start when <see cref="Enabled"/> is <c>false</c>.
        /// Calling this method is the same as setting <see cref="Enabled"/> to <c>true</c>.
        /// </remarks>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was started.
        /// <c>null</c> is returned if the timer failed to start.
        /// </returns>
        ValueTask<DateTimeOffset?> StartAsync();

        /// <summary>
        /// Asynchronously starts the timer specifying a due time (start delay).
        /// </summary>
        /// <remarks>
        /// The timer will only start when <see cref="Enabled"/> is <c>false</c>.
        /// </remarks>
        /// <param name="dueTime">The amount of time to delay before invoking the callback method specified when the Timer was constructed, in milliseconds. Specify Infinite to prevent the timer from restarting. Specify zero (0) to restart the timer immediately.</param>
        /// <param name="update">When <c>true</c> the instance property is updated when the timer is started successfully.</param>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was started.
        /// <c>null</c> is returned if the timer failed to start.
        /// </returns>
        ValueTask<DateTimeOffset?> StartAsync(int dueTime, bool update = false);

        /// <summary>
        /// Asynchronously starts the timer specifying a due time (start delay).
        /// </summary>
        /// <remarks>
        /// The timer will only start when <see cref="Enabled"/> is <c>false</c>.
        /// </remarks>
        /// <param name="dueTime">The amount of time to delay before invoking the callback method specified when the Timer was constructed, in milliseconds. Specify Infinite to prevent the timer from restarting. Specify zero (0) to restart the timer immediately.</param>
        /// <param name="update">When <c>true</c> the instance property is updated when the timer is started successfully.</param>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was started.
        /// <c>null</c> is returned if the timer failed to start.
        /// </returns>
        ValueTask<DateTimeOffset?> StartAsync(TimeSpan dueTime, bool update = false);

        /// <summary>
        /// Stops the timer.
        /// </summary>
        /// <remarks>
        /// The timer will only stop when <see cref="Enabled"/> is <c>true</c>.
        /// Calling this method is the same as setting the value of <see cref="Enabled"/> to <c>false</c>.
        /// </remarks>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was stopped.
        /// A default <see cref="DateTimeOffset"/> is returned when the timer never ran.
        /// <c>null</c> is returned if the timer failed to stop.
        /// </returns>
        DateTimeOffset? Stop();

        /// <summary>
        /// Asynchronously stops the timer.
        /// </summary>
        /// <remarks>
        /// The timer will only stop when <see cref="Enabled"/> is <c>true</c>.
        /// Calling this method is the same as setting the value of <see cref="Enabled"/> to <c>false</c>.
        /// </remarks>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was stopped.
        /// A default <see cref="DateTimeOffset"/> is returned when the timer never ran.
        /// <c>null</c> is returned if the timer failed to stop.
        /// </returns>
        ValueTask<DateTimeOffset?> StopAsync();

        /// <summary>
        /// Restarts the timer.
        /// </summary>
        /// <remarks>
        /// This method ignores <see cref="Enabled"/> and is always executed.
        /// </remarks>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was restarted.
        /// <c>null</c> is returned if the timer failed to restart.
        /// </returns>
        DateTimeOffset? Restart();

        /// <summary>
        /// Restarts the timer specifying a due time (start delay).
        /// </summary>
        /// <remarks>
        /// This method ignores <see cref="Enabled"/> and is always executed.
        /// </remarks>
        /// <param name="dueTime">The amount of time to delay before invoking the callback method specified when the Timer was constructed, in milliseconds. Specify Infinite to prevent the timer from restarting. Specify zero (0) to restart the timer immediately.</param>
        /// <param name="update">When <c>true</c> the instance property is updated when the timer is started successfully.</param>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was restarted.
        /// <c>null</c> is returned if the timer failed to restart.
        /// </returns>
        DateTimeOffset? Restart(int dueTime, bool update = false);

        /// <summary>
        /// Restarts the timer specifying a due time (start delay).
        /// </summary>
        /// <remarks>
        /// This method ignores <see cref="Enabled"/> and is always executed.
        /// </remarks>
        /// <param name="dueTime">A <see cref="TimeSpan"/> representing the amount of time to delay before invoking the callback method specified when the Timer was constructed. Specify <see cref="Timeout.InfiniteTimeSpan"/> to prevent the timer from restarting. Specify Zero to restart the timer immediately.</param>
        /// <param name="update">When <c>true</c> the instance property is updated when the timer is started successfully.</param>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was restarted.
        /// <c>null</c> is returned if the timer failed to restart.
        /// </returns>
        DateTimeOffset? Restart(TimeSpan dueTime, bool update = false);

        /// <summary>
        /// Asynchronously restarts the timer.
        /// </summary>
        /// <remarks>
        /// This method ignores <see cref="Enabled"/> and is always executed.
        /// </remarks>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was restarted.
        /// <c>null</c> is returned if the timer failed to restart.
        /// </returns>
        ValueTask<DateTimeOffset?> RestartAsync();

        /// <summary>
        /// Asynchronously restarts the timer specifying a due time (start delay).
        /// </summary>
        /// <remarks>
        /// This method ignores <see cref="Enabled"/> and is always executed.
        /// </remarks>
        /// <param name="dueTime">The amount of time to delay before invoking the callback method specified when the Timer was constructed, in milliseconds. Specify Infinite to prevent the timer from restarting. Specify zero (0) to restart the timer immediately.</param>
        /// <param name="update">When <c>true</c> the instance property is updated when the timer is started successfully.</param>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was restarted.
        /// <c>null</c> is returned if the timer failed to restart.
        /// </returns>
        ValueTask<DateTimeOffset?> RestartAsync(int dueTime, bool update = false);

        /// <summary>
        /// Asynchronously restarts the timer specifying a due time (start delay).
        /// </summary>
        /// <remarks>
        /// This method ignores <see cref="Enabled"/> and is always executed.
        /// </remarks>
        /// <param name="dueTime">A <see cref="TimeSpan"/> representing the amount of time to delay before invoking the callback method specified when the Timer was constructed. Specify <see cref="Timeout.InfiniteTimeSpan"/> to prevent the timer from restarting. Specify Zero to restart the timer immediately.</param>
        /// <param name="update">When <c>true</c> the instance property is updated when the timer is started successfully.</param>
        /// <returns>
        /// A UTC <see cref="DateTimeOffset"/> that references a point in time when the timer was restarted.
        /// <c>null</c> is returned if the timer failed to restart.
        /// </returns>
        ValueTask<DateTimeOffset?> RestartAsync(TimeSpan dueTime, bool update = false);

        /// <summary>
        /// Releases all resources used by the current instance of <see cref="IComponentTimer"/>.
        /// </summary>
        new void Dispose();

        /// <summary>
        /// Asynchronously releases all resources used by the current instance of <see cref="IComponentTimer"/>.
        /// </summary>
        new ValueTask DisposeAsync();
    }


    /// <summary>
    /// Provides a mechanism for executing a method on a thread pool thread at specified intervals. This class cannot be inherited.
    /// </summary>
    public sealed class ComponentTimer : IComponentTimer
    {
        private enum TimerAction { Start, Stop, Restart }
        private Timer _timer;
        private long _startUtcTicks;
        private bool? _enabled;
        private int _restarts;


        private ComponentTimer(TimeSpan? dueTime = null, TimeSpan? period = null)
        {
            DueTime = HasStartDelay(dueTime) ? (TimeSpan) dueTime : TimeSpan.Zero;
            Period = HasInterval(period) ? (TimeSpan)period : TimeSpan.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentTimer"/> class.
        /// </summary>
        /// <param name="callback">A delegate representing a method to be executed.</param>
        /// <param name="enabled">When <c>true</c>, starts the timer.</param>
        /// <param name="dueTime">The amount of time to delay before the callback is invoked. The default is <see cref="TimeSpan.Zero"/>).</param>
        /// <param name="period">The time interval between invocations of callback. The default is <see cref="TimeSpan.Zero"/>.</param>
        /// <param name="state">An object containing information to be used by the callback method, or <c>null</c>.</param>
        public ComponentTimer(Action<object> callback, bool enabled = false, TimeSpan? dueTime = null, TimeSpan? period = null, object state = null) : this(dueTime, period)
        {
            _timer = new Timer(async (_state) => { await TimerCallbackAsync(callback, _state); }, state, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            if (enabled)
                Enabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentTimer"/> class.
        /// </summary>
        /// <param name="callback">An async delegate representing a method to be executed.</param>
        /// <param name="enabled">When <c>true</c>, starts the timer.</param>
        /// <param name="dueTime">The amount of time to delay before the callback is invoked. The default is <see cref="TimeSpan.Zero"/>).</param>
        /// <param name="period">The time interval between invocations of callback. The default is <see cref="TimeSpan.Zero"/>.</param>
        /// <param name="state">An object containing information to be used by the callback method, or <c>null</c>.</param>
        public ComponentTimer(Func<object, ValueTask> callback, bool enabled = false, TimeSpan? dueTime = null, TimeSpan? period = null, object state = null) : this(dueTime, period)
        {
            _timer = new Timer(async (_state) => { await TimerCallbackAsync(callback, _state); }, state, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            if (enabled)
                Enabled = true;
        }


        public bool Enabled
        {
            get => _enabled != null ? _enabled.Value : false;
            set => _enabled = value ? Start() != null : Stop() == null;
        }

        public int Restarts => _restarts;

        public TimeSpan DueTime { get; set; }

        public TimeSpan Period { get; set; }

        public TimeSpan Elapsed => _timer == null ? Timeout.InfiniteTimeSpan : GetElapsedUtcTime();

        public bool IsTicking => _timer != null && Enabled && HasStartDelay(DueTime) && HasInterval(Period);


        private bool Change(TimerAction timerAction, TimeSpan? dueTime = null, TimeSpan? period = null, bool update = false)
        {
            var success = false;

            switch (timerAction)
            {
                case TimerAction.Start:
                    if (!Enabled)
                    {
                        dueTime = null != dueTime && HasStartDelay(dueTime) ? dueTime : DueTime;

                        success = HasStartDelay(dueTime) && HasInterval(Period) && _timer != null && _timer.Change(DueTime, Period);
                        if (success)
                        {
                            if (update)
                                DueTime = (TimeSpan) dueTime;

                            _enabled = true;
                            _startUtcTicks = GetNowUtcTicks();
                        }
                        else
                            _enabled = null;
                    }
                    break;

                case TimerAction.Restart:
                    _enabled = false;

                    if (null == dueTime)
                        dueTime = TimeSpan.Zero;

                    success = HasStartDelay(dueTime) && _timer != null && _timer.Change((TimeSpan) dueTime, Timeout.InfiniteTimeSpan);
                    if (success)
                    {
                        if (update)
                            DueTime = (TimeSpan) dueTime;

                        Interlocked.Increment(ref _restarts);

                        _enabled = true;
                        _startUtcTicks = GetNowUtcTicks();
                    }
                    else
                        _enabled = null;
                    break;

                case TimerAction.Stop:
                    if (Enabled)
                    {
                        success = _timer != null && _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                        _enabled = success ? false : null;
                    }
                    break;
            }

            return success;
        }


        public DateTimeOffset? Start() { Change(TimerAction.Start); return GetStartUtcTime(); }

        public DateTimeOffset? Start(int dueTime, bool update = false) { Change(TimerAction.Start, TimeSpan.FromMilliseconds(dueTime), update: update); return GetStartUtcTime(); }

        public DateTimeOffset? Start(TimeSpan dueTime, bool update = false) { Change(TimerAction.Start, dueTime, update: update); return GetStartUtcTime(); }

        public async ValueTask<DateTimeOffset?> StartAsync()
        {
            await ValueTask.CompletedTask;
            return Start();
        }

        public async ValueTask<DateTimeOffset?> StartAsync(int dueTime, bool update = false)
        {
            await ValueTask.CompletedTask;
            return Start(dueTime, update);
        }

        public async ValueTask<DateTimeOffset?> StartAsync(TimeSpan dueTime, bool update = false)
        {
            await ValueTask.CompletedTask;
            return Start(dueTime, update);
        }


        public DateTimeOffset? Restart() { Change(TimerAction.Restart); return GetStartUtcTime(); }

        public DateTimeOffset? Restart(int dueTime, bool update = false) { Change(TimerAction.Restart, TimeSpan.FromMilliseconds(dueTime), update: update); return GetStartUtcTime(); }

        public DateTimeOffset? Restart(TimeSpan dueTime, bool update = false) { Change(TimerAction.Restart, dueTime, update: update); return GetStartUtcTime(); }

        public async ValueTask<DateTimeOffset?> RestartAsync()
        {
            await ValueTask.CompletedTask;
            return Restart();
        }

        public async ValueTask<DateTimeOffset?> RestartAsync(int dueTime, bool update = false)
        {
            await ValueTask.CompletedTask;
            return Restart(dueTime, update);
        }

        public async ValueTask<DateTimeOffset?> RestartAsync(TimeSpan dueTime, bool update = false)
        {
            await ValueTask.CompletedTask;
            return Restart(dueTime, update);
        }


        public DateTimeOffset? Stop()
        {
            var success = Change(TimerAction.Stop);

            if (success)
                return _startUtcTicks > 0 ? GetStopUtcTime() : default(DateTimeOffset);

            else
            {
                var neverRan = _enabled == null && _startUtcTicks == 0;

                return neverRan ? default(DateTimeOffset) : GetStopUtcTime();
            }
        }

        public async ValueTask<DateTimeOffset?> StopAsync()
        {
            await ValueTask.CompletedTask;
            return Stop();
        }


        private static async ValueTask TimerCallbackAsync<T>(Action<T> callback, T state) { await ValueTask.CompletedTask; callback.Invoke(state); }

        private static async ValueTask TimerCallbackAsync<T>(Func<T, ValueTask> callback, T state) { await callback.Invoke(state); }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;

            var timer = _timer;
            if (timer != null)
            {
                DisposeCleanup();
                timer?.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        private async ValueTask DisposeAsync(bool disposing)
        {
            if (!disposing) return;

            var timer = _timer;
            if (timer != null)
            {
                DisposeCleanup();
                await timer.DisposeAsync();
            }
        }

        private void DisposeCleanup()
        {
            _enabled = null;
            _timer = null;

            DueTime = Timeout.InfiniteTimeSpan;
            Period = Timeout.InfiniteTimeSpan;
        }


        private static bool HasDueTime(TimeSpan? dueTime = null) => dueTime != null && dueTime.Value != Timeout.InfiniteTimeSpan;

        private static bool HasPeriod(TimeSpan? period = null) => period != null && period.Value != Timeout.InfiniteTimeSpan;

        private static bool HasStartDelay(TimeSpan? dueTime = null) => HasDueTime(dueTime) && dueTime.Value.TotalMilliseconds >= 0;

        private static bool HasInterval(TimeSpan? period = null) => HasPeriod(period) && period.Value != TimeSpan.Zero;


        private static long GetNowUtcTicks() => DateTime.UtcNow.Ticks;

        private TimeSpan GetElapsedUtcTime() => TimeSpan.FromTicks(_startUtcTicks > 0 ? GetNowUtcTicks() - _startUtcTicks : 0);

        private DateTimeOffset? GetStartUtcTime() => _startUtcTicks > 0 ? new DateTime(_startUtcTicks, DateTimeKind.Utc) : null;

        private static DateTimeOffset? GetStopUtcTime() => new DateTime(GetNowUtcTicks(), DateTimeKind.Utc);
    }
}
