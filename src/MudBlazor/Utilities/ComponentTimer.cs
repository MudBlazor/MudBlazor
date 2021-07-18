// Copyright © 2021 Jeffrey Jangli

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Utilities
{
    /// <summary>
    /// Provides a mechanism for executing a method on a thread pool thread at specified intervals. This class cannot be inherited.
    /// </summary>
    internal sealed class ComponentTimer : IAsyncDisposable, IDisposable
    {
        private static readonly object _locker = new();
        private bool? _callbackBusy;
        private enum TimerAction { Start, Stop, Restart }
        private Timer _timer;
        private long _startUtcTicks;
        private bool? _enabled;
        private int _restarts;
        private int _iterations;


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentTimer"/> class.
        /// </summary>
        /// <param name="callback">A delegate representing a method to be executed.</param>
        /// <param name="enabled">When <c>true</c>, starts the timer.</param>
        /// <param name="dueTime">The amount of time to delay before the callback is invoked. The default is <see cref="TimeSpan.Zero"/>).</param>
        /// <param name="period">The time interval between invocations of callback. The default is <see cref="TimeSpan.Zero"/>.</param>
        /// <param name="iterations">The maximum number of times that the callback method is allowed to run. <c>0</c> is unlimited.</param>
        /// <param name="state">An object containing information to be used by the callback method, or <c>null</c>.</param>
        /// <param name="token">A <see cref="CancellationTokenSource"/> token to cancel the timer. An <see cref="OperationCanceledException"/> is thrown.</param>
        internal ComponentTimer(Action<object> callback, bool enabled = false, TimeSpan? dueTime = null, TimeSpan? period = null, object state = null, int iterations = 0, CancellationToken token = default) : this(dueTime, period)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _timer = new Timer(async (_state) =>
            {
                if (token.IsCancellationRequested)
                    Enabled = false;
                else
                    await TimerCallbackAsync(callback, _state, iterations);

            }, state, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            
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
        /// <param name="iterations">The maximum number of times that the callback method is allowed to run. <c>0</c> is unlimited.</param>
        /// <param name="configureAwait"><c>true</c> to attempt to marshal the continuation back to the original context captured.</param>
        /// <param name="token">A <see cref="CancellationTokenSource"/> token to cancel the timer. An <see cref="OperationCanceledException"/> is thrown.</param>
        internal ComponentTimer(Func<object, ValueTask> callback, bool enabled = false, TimeSpan? dueTime = null, TimeSpan? period = null, object state = null, int iterations = 0, bool configureAwait = false, CancellationToken token = default) : this(dueTime, period)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _timer = new Timer(async (_state) =>
            {
                if (token.IsCancellationRequested)
                    Enabled = false;
                else
                    await TimerCallbackAsync(callback, _state, iterations, configureAwait);

            }, state, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            if (enabled)
                Enabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentTimer"/> class.
        /// </summary>
        /// <param name="callback">An <see cref="EventCallback"/> representing a method to be executed.</param>
        /// <param name="enabled">When <c>true</c>, starts the timer.</param>
        /// <param name="dueTime">The amount of time to delay before the callback is invoked. The default is <see cref="TimeSpan.Zero"/>).</param>
        /// <param name="period">The time interval between invocations of callback. The default is <see cref="TimeSpan.Zero"/>.</param>
        /// <param name="state">An object containing information to be used by the callback method, or <c>null</c>.</param>
        /// <param name="iterations">The maximum number of times that the callback method is allowed to run. <c>0</c> is unlimited.</param>
        /// <param name="configureAwait"><c>true</c> to attempt to marshal the continuation back to the original context captured.</param>
        /// <param name="token">A <see cref="CancellationTokenSource"/> token to cancel the timer. An <see cref="OperationCanceledException"/> is thrown.</param>
        internal ComponentTimer(EventCallback callback, bool enabled = false, TimeSpan? dueTime = null, TimeSpan? period = null, object state = null, int iterations = 0, bool configureAwait = false, CancellationToken token = default) : this(dueTime, period)
        {
            if (!callback.HasDelegate)
                throw new ArgumentNullException(nameof(callback));

            _timer = new Timer(async (_state) =>
            {
                if (token.IsCancellationRequested)
                    Enabled = false;
                else
                    await TimerCallbackAsync(callback, _state, iterations, configureAwait);

            }, state, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            if (enabled)
                Enabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentTimer"/> class.
        /// </summary>
        /// <param name="callback">An <see cref="EventCallback"/> object representing a method to be executed.</param>
        /// <param name="enabled">When <c>true</c>, starts the timer.</param>
        /// <param name="dueTime">The amount of time to delay before the callback is invoked. The default is <see cref="TimeSpan.Zero"/>).</param>
        /// <param name="period">The time interval between invocations of callback. The default is <see cref="TimeSpan.Zero"/>.</param>
        /// <param name="state">An object containing information to be used by the callback method, or <c>null</c>.</param>
        /// <param name="iterations">The maximum number of times that the callback method is allowed to run. <c>0</c> is unlimited.</param>
        /// <param name="configureAwait"><c>true</c> to attempt to marshal the continuation back to the original context captured.</param>
        /// <param name="token">A <see cref="CancellationTokenSource"/> token to cancel the timer. An <see cref="OperationCanceledException"/> is thrown.</param>
        internal ComponentTimer(EventCallback<object> callback, bool enabled = false, TimeSpan? dueTime = null, TimeSpan? period = null, object state = null, int iterations = 0, bool configureAwait = false, CancellationToken token = default) : this(dueTime, period)
        {
            if (!callback.HasDelegate)
                throw new ArgumentNullException(nameof(callback));

            _timer = new Timer(async (_state) =>
            {
                if (token.IsCancellationRequested)
                    Enabled = false;
                else
                    await TimerCallbackAsync(callback, _state, iterations, configureAwait);

            }, state, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            if (enabled)
                Enabled = true;
        }

        private ComponentTimer(TimeSpan? dueTime = null, TimeSpan? period = null)
        {
            DueTime = HasStartDelay(dueTime) ? (TimeSpan)dueTime : TimeSpan.Zero;
            Period = HasInterval(period) ? (TimeSpan)period : TimeSpan.Zero;
        }

        #endregion Constructors


        #region Properties

        public bool Enabled
        {
            get => _enabled != null && _enabled.Value;
            set => _enabled = value ? Start() != null : Stop() == null;
        }

        public int Iterations => _iterations;

        public int Restarts => _restarts;

        public TimeSpan DueTime { get; set; }

        public TimeSpan Period { get; set; }

        public TimeSpan Elapsed => _timer == null ? Timeout.InfiniteTimeSpan : GetElapsedUtcTime();

        public bool IsTicking => _timer != null && Enabled && HasStartDelay(DueTime) && HasInterval(Period);

        #endregion Properties


        #region Start

        public DateTimeOffset? Start()
        {
            Change(TimerAction.Start);
            return GetStartUtcTime();
        }

        public DateTimeOffset? Start(int dueTime, bool update = false)
        {
            Change(TimerAction.Start, TimeSpan.FromMilliseconds(dueTime), update: update);
            return GetStartUtcTime();
        }

        public DateTimeOffset? Start(TimeSpan dueTime, bool update = false)
        {
            Change(TimerAction.Start, dueTime, update: update);
            return GetStartUtcTime();
        }

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

        #endregion Start


        #region Restart

        public DateTimeOffset? Restart()
        {
            Change(TimerAction.Restart);
            return GetStartUtcTime();
        }

        public DateTimeOffset? Restart(int dueTime, bool update = false)
        {
            Change(TimerAction.Restart, TimeSpan.FromMilliseconds(dueTime), update: update);
            return GetStartUtcTime();
        }

        public DateTimeOffset? Restart(TimeSpan dueTime, bool update = false)
        {
            Change(TimerAction.Restart, dueTime, update: update);
            return GetStartUtcTime();
        }

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

        #endregion Restart


        #region Stop

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

        #endregion Stop


        #region TimerCallback

        private async ValueTask TimerCallbackAsync<T>(Action<T> callback, T state, int iterations = 0)
        {
            await ValueTask.CompletedTask;

            lock (_locker)
            {
                if (_callbackBusy == null)
                    _callbackBusy = true;

                else if (_callbackBusy.Value)
                    return;
            }

            if (_iterations <= iterations)
            {
                if (Enabled)
                {
                    callback.Invoke(state);
                    Interlocked.Increment(ref _iterations);
                }
            }

            lock (_locker)
            {
                if (_iterations == iterations)
                    Enabled = false;

                _callbackBusy = false;
            }
        }

        private async ValueTask TimerCallbackAsync<T>(Func<T, ValueTask> callback, T state, int iterations = 0, bool configureAwait = false)
        {
            lock (_locker)
            {
                if (_callbackBusy == null)
                    _callbackBusy = true;

                else if (_callbackBusy.Value)
                    return;
            }

            if (_iterations <= iterations)
            {
                if (Enabled)
                {
                    await callback.Invoke(state).ConfigureAwait(configureAwait);
                    Interlocked.Increment(ref _iterations);
                }
            }

            lock (_locker)
            {
                if (_iterations == iterations)
                    Enabled = false;

                _callbackBusy = false;
            }
        }

        private async ValueTask TimerCallbackAsync<T>(EventCallback callback, T state, int iterations = 0, bool configureAwait = false)
        {
            lock (_locker)
            {
                if (_callbackBusy == null)
                    _callbackBusy = true;

                else if (_callbackBusy.Value)
                    return;
            }

            if (_iterations <= iterations)
            {
                if (Enabled)
                {
                    await callback.InvokeAsync(state).ConfigureAwait(configureAwait);
                    Interlocked.Increment(ref _iterations);
                }
            }

            lock (_locker)
            {
                if (_iterations == iterations)
                    Enabled = false;

                _callbackBusy = false;
            }
        }

        private async ValueTask TimerCallbackAsync<T>(EventCallback<T> callback, T state, int iterations = 0, bool configureAwait = false)
        {
            lock (_locker)
            {
                if (_callbackBusy == null)
                    _callbackBusy = true;

                else if (_callbackBusy.Value)
                    return;
            }

            if (_iterations <= iterations)
            {
                if (Enabled)
                {
                    await callback.InvokeAsync(state).ConfigureAwait(configureAwait);
                    Interlocked.Increment(ref _iterations);
                }
            }

            lock (_locker)
            {
                if (_iterations == iterations)
                    Enabled = false;

                _callbackBusy = false;
            }
        }

        #endregion TimerCallback


        #region Dispose

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
            lock (_locker)
            {
                _enabled = null;
                _timer = null;

                DueTime = Timeout.InfiniteTimeSpan;
                Period = Timeout.InfiniteTimeSpan;
            }
        }

        #endregion Dispose


        #region Helpers

        private static bool HasDueTime(TimeSpan? dueTime = null) => dueTime != null && dueTime.Value != Timeout.InfiniteTimeSpan;

        private static bool HasPeriod(TimeSpan? period = null) => period != null && period.Value != Timeout.InfiniteTimeSpan;

        private static bool HasStartDelay(TimeSpan? dueTime = null) => HasDueTime(dueTime) && dueTime.Value.TotalMilliseconds >= 0;

        private static bool HasInterval(TimeSpan? period = null) => HasPeriod(period) && period.Value != TimeSpan.Zero;


        private static long GetNowUtcTicks() => DateTime.UtcNow.Ticks;

        private TimeSpan GetElapsedUtcTime() => TimeSpan.FromTicks(_startUtcTicks > 0 ? GetNowUtcTicks() - _startUtcTicks : 0);

        private DateTimeOffset? GetStartUtcTime() => _startUtcTicks > 0 ? new DateTime(_startUtcTicks, DateTimeKind.Utc) : null;

        private static DateTimeOffset? GetStopUtcTime() => new DateTime(GetNowUtcTicks(), DateTimeKind.Utc);


        private bool Change(TimerAction timerAction, TimeSpan? dueTime = null, bool update = false)
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

                        _restarts++;
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

        #endregion Helpers
    }
}
