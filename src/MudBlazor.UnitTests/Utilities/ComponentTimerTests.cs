using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace UtilityTests
{
    public class ComponentTimerTests
    {
        #region Setup

        private static readonly object _locker = new();
        private static readonly TimeSpan _1msTimeSpan = TimeSpan.FromMilliseconds(1);
        private static readonly TimeSpan _3msTimeSpan = TimeSpan.FromMilliseconds(3);
        private static readonly TimeSpan _delayMs = TimeSpan.FromMilliseconds(50);
        private CallbackExecuteTest _testCallback;
        private CallbackExecuteTest _testCallbackAsync;
        private CallbackExecuteTest _testEventCallbackAsync;
        private EventCallback TestEventCallback;
        private class CallbackExecuteTest { public int ExecuteCount { get; set; } = -1; public bool HasExecuted { get; set; } }
        private void CallbackMethod<T>(T state) { CallbackCore(state); }
        private async ValueTask CallbackMethodAsync<T>(T state) { await ValueTask.CompletedTask; CallbackCore(state); }
        private async Task EventCallbackMethodAsync<T>(T state) { await Task.CompletedTask; CallbackCore(state); }
        private static void CallbackCore<T>(T state, [CallerMemberName] string methodName = "???")
        {
            if (state is CallbackExecuteTest executeTest)
            {
                lock (_locker)
                {
                    executeTest.ExecuteCount++;
                    executeTest.HasExecuted = true;
                }

                Console.WriteLine($"{methodName}: {executeTest.ExecuteCount}");
            }
            else
                Console.WriteLine($"{methodName}: {nameof(state)} = {state}");
        }

        [SetUp]
        public void Setup()
        {
            _testCallback = new CallbackExecuteTest { ExecuteCount = 0 };
            _testCallbackAsync = new CallbackExecuteTest { ExecuteCount = 0 };

            _testEventCallbackAsync = new CallbackExecuteTest { ExecuteCount = 0 };
            TestEventCallback = EventCallback.Factory.Create(this, EventCallbackMethodAsync);
        }

        #endregion Setup


        #region Constructors

        [Test]
        public void Constructor_ConcreteClass()
        {
            using var timer = new ComponentTimer(CallbackMethod, state: _testCallback);
            timer.Should().As<ComponentTimer>();
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using var timerAsync = new ComponentTimer(CallbackMethodAsync, state: _testCallbackAsync);
            timerAsync.Should().As<ComponentTimer>();
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using var timerEventAsync = new ComponentTimer(TestEventCallback, state: _testEventCallbackAsync);
            timerEventAsync.Should().As<ComponentTimer>();
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Constructor_Interface()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, state: _testCallback);
            timer.Should().As<IComponentTimer>();
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, state: _testCallbackAsync);
            timerAsync.Should().As<IComponentTimer>();
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, state: _testEventCallbackAsync);
            timerEventAsync.Should().As<IComponentTimer>();
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        #endregion Constructors


        #region Properties

        [Test]
        public async ValueTask Property_Enabled()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, period: _1msTimeSpan, state: _testCallback);
            timer.Enabled.Should().BeFalse();
            timer.Enabled = true;
            timer.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            timer.Enabled = false;
            timer.Enabled.Should().BeFalse();
            _testCallback.HasExecuted.Should().BeTrue();
            _testCallback.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, period: _1msTimeSpan, state: _testCallbackAsync);
            timerAsync.Enabled.Should().BeFalse();
            timerAsync.Enabled = true;
            timerAsync.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            timerAsync.Enabled = false;
            timerAsync.Enabled.Should().BeFalse();
            _testCallbackAsync.HasExecuted.Should().BeTrue();
            _testCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, period: _1msTimeSpan, state: _testEventCallbackAsync);
            timerEventAsync.Enabled.Should().BeFalse();
            timerEventAsync.Enabled = true;
            timerEventAsync.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            timerEventAsync.Enabled = false;
            timerEventAsync.Enabled.Should().BeFalse();
            _testEventCallbackAsync.HasExecuted.Should().BeTrue();
            _testEventCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Property_IsTicking()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, true, period: _1msTimeSpan, state: _testCallback);
            timer.IsTicking.Should().BeTrue();
            await Task.Delay(_delayMs);
            timer.Stop();
            timer.IsTicking.Should().BeFalse();
            _testCallback.HasExecuted.Should().BeTrue();
            _testCallback.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, true, period: _1msTimeSpan, state: _testCallbackAsync);
            timerAsync.IsTicking.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timerAsync.StopAsync();
            timerAsync.IsTicking.Should().BeFalse();
            _testCallbackAsync.HasExecuted.Should().BeTrue();
            _testCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, true, period: _1msTimeSpan, state: _testEventCallbackAsync);
            timerEventAsync.IsTicking.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timerEventAsync.StopAsync();
            timerEventAsync.IsTicking.Should().BeFalse();
            _testEventCallbackAsync.HasExecuted.Should().BeTrue();
            _testEventCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public void Property_IsTicking_With_InfiniteTimeSpan()
        {
            var interval = Timeout.InfiniteTimeSpan;

            using IComponentTimer timer = new ComponentTimer(CallbackMethod, true, period: interval, state: _testCallback);
            timer.IsTicking.Should().BeFalse();
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, true, period: interval, state: _testCallbackAsync);
            timerAsync.IsTicking.Should().BeFalse();
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, true, period: interval, state: _testEventCallbackAsync);
            timerEventAsync.IsTicking.Should().BeFalse();
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_IsTicking_With_TimeSpanZero()
        {
            var interval = TimeSpan.Zero;

            using IComponentTimer timer = new ComponentTimer(CallbackMethod, true, period: interval, state: _testCallback);
            timer.IsTicking.Should().BeFalse();
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, true, period: interval, state: _testCallbackAsync);
            timerAsync.IsTicking.Should().BeFalse();
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, true, period: interval, state: _testEventCallbackAsync);
            timerEventAsync.IsTicking.Should().BeFalse();
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_DueTime()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, dueTime: _1msTimeSpan, state: _testCallback);
            timer.DueTime.Should().NotBe(TimeSpan.Zero);
            timer.DueTime.Should().NotBe(Timeout.InfiniteTimeSpan);
            timer.DueTime.TotalMilliseconds.Should().Be(1);
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, dueTime: _1msTimeSpan, state: _testCallbackAsync);
            timerAsync.DueTime.Should().NotBe(TimeSpan.Zero);
            timerAsync.DueTime.Should().NotBe(Timeout.InfiniteTimeSpan);
            timerAsync.DueTime.TotalMilliseconds.Should().Be(1);
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, dueTime: _1msTimeSpan, state: _testEventCallbackAsync);
            timerEventAsync.DueTime.Should().NotBe(TimeSpan.Zero);
            timerEventAsync.DueTime.Should().NotBe(Timeout.InfiniteTimeSpan);
            timerEventAsync.DueTime.TotalMilliseconds.Should().Be(1);
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_DueTime_With_InfiniteTimeSpan()
        {
            var startDelay = Timeout.InfiniteTimeSpan;

            using IComponentTimer timer = new ComponentTimer(CallbackMethod, dueTime: startDelay, state: _testCallback);
            timer.DueTime.Should().Be(TimeSpan.Zero);
            timer.DueTime.TotalMilliseconds.Should().Be(0);
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, dueTime: startDelay, state: _testCallbackAsync);
            timerAsync.DueTime.Should().Be(TimeSpan.Zero);
            timerAsync.DueTime.TotalMilliseconds.Should().Be(0);
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, dueTime: startDelay, state: _testEventCallbackAsync);
            timerEventAsync.DueTime.Should().Be(TimeSpan.Zero);
            timerEventAsync.DueTime.TotalMilliseconds.Should().Be(0);
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_DueTime_With_TimeSpanZero()
        {
            var startDelay = TimeSpan.Zero;

            using IComponentTimer timer = new ComponentTimer(CallbackMethod, dueTime: startDelay, state: _testCallback);
            timer.DueTime.Should().Be(TimeSpan.Zero);
            timer.DueTime.TotalMilliseconds.Should().Be(0);
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, dueTime: startDelay, state: _testCallbackAsync);
            timerAsync.DueTime.Should().Be(TimeSpan.Zero);
            timerAsync.DueTime.TotalMilliseconds.Should().Be(0);
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, dueTime: startDelay, state: _testEventCallbackAsync);
            timerEventAsync.DueTime.Should().Be(TimeSpan.Zero);
            timerEventAsync.DueTime.TotalMilliseconds.Should().Be(0);
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_Period()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, period: _1msTimeSpan, state: _testCallback);
            timer.Period.Should().NotBe(TimeSpan.Zero);
            timer.Period.Should().NotBe(Timeout.InfiniteTimeSpan);
            timer.Period.TotalMilliseconds.Should().Be(1);
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, period: _1msTimeSpan, state: _testCallbackAsync);
            timerAsync.Period.Should().NotBe(TimeSpan.Zero);
            timerAsync.Period.Should().NotBe(Timeout.InfiniteTimeSpan);
            timerAsync.Period.TotalMilliseconds.Should().Be(1);
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, period: _1msTimeSpan, state: _testEventCallbackAsync);
            timerEventAsync.Period.Should().NotBe(TimeSpan.Zero);
            timerEventAsync.Period.Should().NotBe(Timeout.InfiniteTimeSpan);
            timerEventAsync.Period.TotalMilliseconds.Should().Be(1);
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_Period_With_InfiniteTimeSpan()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, period: _1msTimeSpan, state: _testCallback);
            timer.Period.Should().NotBe(TimeSpan.Zero);
            timer.Period.TotalMilliseconds.Should().Be(1);
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, period: _1msTimeSpan, state: _testCallbackAsync);
            timerAsync.Period.Should().NotBe(TimeSpan.Zero);
            timerAsync.Period.TotalMilliseconds.Should().Be(1);
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, period: _1msTimeSpan, state: _testEventCallbackAsync);
            timerEventAsync.Period.Should().NotBe(TimeSpan.Zero);
            timerEventAsync.Period.TotalMilliseconds.Should().Be(1);
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_Period_With_TimeSpanZero()
        {
            var interval = TimeSpan.Zero;

            using IComponentTimer timer = new ComponentTimer(CallbackMethod, period: interval, state: _testCallback);
            timer.Period.Should().Be(TimeSpan.Zero);
            timer.Period.TotalMilliseconds.Should().Be(0);
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, period: interval, state: _testCallbackAsync);
            timerAsync.Period.Should().Be(TimeSpan.Zero);
            timerAsync.Period.TotalMilliseconds.Should().Be(0);
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, period: interval, state: _testEventCallbackAsync);
            timerEventAsync.Period.Should().Be(TimeSpan.Zero);
            timerEventAsync.Period.TotalMilliseconds.Should().Be(0);
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        [Test]
        public async ValueTask Property_Elapsed()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, period: _1msTimeSpan, state: _testCallback);
            timer.Elapsed.Should().Be(TimeSpan.Zero);
            timer.Start();
            await Task.Delay(_delayMs);
            timer.Stop();
            timer.Elapsed.TotalMilliseconds.Should().BeGreaterThan(0);
            _testCallback.HasExecuted.Should().BeTrue();
            _testCallback.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, period: _1msTimeSpan, state: _testCallbackAsync);
            timerAsync.Elapsed.Should().Be(TimeSpan.Zero);
            await timerAsync.StartAsync();
            await Task.Delay(_delayMs);
            await timerAsync.StopAsync();
            timerAsync.Elapsed.TotalMilliseconds.Should().BeGreaterThan(0);
            _testCallbackAsync.HasExecuted.Should().BeTrue();
            _testCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, period: _1msTimeSpan, state: _testEventCallbackAsync);
            timerEventAsync.Elapsed.Should().Be(TimeSpan.Zero);
            await timerEventAsync.StartAsync();
            await Task.Delay(_delayMs);
            await timerEventAsync.StopAsync();
            timerEventAsync.Elapsed.TotalMilliseconds.Should().BeGreaterThan(0);
            _testEventCallbackAsync.HasExecuted.Should().BeTrue();
            _testEventCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);
        }

        #endregion Properties


        #region Methods

        [Test]
        public async ValueTask Method_Start_MultipleCalls_Return_Initial_DateTimeOffset()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, period: _1msTimeSpan, state: _testCallback);
            var initialTimestamp = timer.Start();
            Console.WriteLine($"{nameof(initialTimestamp)}: {initialTimestamp}");
            await Task.Delay(_delayMs);
            var newerTimestamp = timer.Start();
            timer.Stop();
            Console.WriteLine($"{nameof(newerTimestamp)}: {newerTimestamp}");
            newerTimestamp.Should().BeSameDateAs((DateTimeOffset)initialTimestamp);
            _testCallback.HasExecuted.Should().BeTrue();
            _testCallback.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, period: _1msTimeSpan, state: _testCallbackAsync);
            var initialTimestampAsync = await timerAsync.StartAsync();
            Console.WriteLine($"{nameof(initialTimestampAsync)}: {initialTimestampAsync}");
            await Task.Delay(_delayMs);
            var newerTimestampAsync = await timerAsync.StartAsync();
            await timerAsync.StopAsync();
            Console.WriteLine($"{nameof(newerTimestampAsync)}: {newerTimestampAsync}");
            newerTimestamp.Should().BeSameDateAs((DateTimeOffset)initialTimestamp);
            _testCallbackAsync.HasExecuted.Should().BeTrue();
            _testCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, period: _1msTimeSpan, state: _testEventCallbackAsync);
            var initialEventTimestampAsync = await timerEventAsync.StartAsync();
            Console.WriteLine($"{nameof(initialEventTimestampAsync)}: {initialEventTimestampAsync}");
            await Task.Delay(_delayMs);
            var newerEventTimestampAsync = await timerEventAsync.StartAsync();
            await timerEventAsync.StopAsync();
            Console.WriteLine($"{nameof(newerEventTimestampAsync)}: {newerEventTimestampAsync}");
            newerEventTimestampAsync.Should().BeSameDateAs((DateTimeOffset)initialEventTimestampAsync);
            _testEventCallbackAsync.HasExecuted.Should().BeTrue();
            _testEventCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Method_Start_Sets_Enabled_True()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, period: _1msTimeSpan, state: _testCallback);
            timer.Start();
            timer.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            timer.Stop();
            _testCallback.HasExecuted.Should().BeTrue();
            _testCallback.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, period: _1msTimeSpan, state: _testCallbackAsync);
            await timerAsync.StartAsync();
            timerAsync.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timerAsync.StopAsync();
            _testCallbackAsync.HasExecuted.Should().BeTrue();
            _testCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, period: _1msTimeSpan, state: _testEventCallbackAsync);
            await timerEventAsync.StartAsync();
            timerEventAsync.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timerEventAsync.StopAsync();
            _testEventCallbackAsync.HasExecuted.Should().BeTrue();
            _testEventCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Method_Start_Updates_DueTime()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, dueTime: _1msTimeSpan, period: _1msTimeSpan, state: _testCallback);
            timer.DueTime.TotalMilliseconds.Should().Be(1);
            timer.Start(_3msTimeSpan);
            timer.DueTime.TotalMilliseconds.Should().Be(1);
            timer.Stop();
            timer.Start(_3msTimeSpan, true);
            timer.DueTime.TotalMilliseconds.Should().Be(3);
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, dueTime: _1msTimeSpan, period: _1msTimeSpan, state: _testCallbackAsync);
            timerAsync.DueTime.TotalMilliseconds.Should().Be(1);
            await timerAsync.StartAsync(_3msTimeSpan);
            timerAsync.DueTime.TotalMilliseconds.Should().Be(1);
            await timerAsync.StopAsync();
            await timerAsync.StartAsync(_3msTimeSpan, true);
            timerAsync.DueTime.TotalMilliseconds.Should().Be(3);
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, dueTime: _1msTimeSpan, period: _1msTimeSpan, state: _testEventCallbackAsync);
            timerEventAsync.DueTime.TotalMilliseconds.Should().Be(1);
            await timerEventAsync.StartAsync(_3msTimeSpan);
            timerEventAsync.DueTime.TotalMilliseconds.Should().Be(1);
            await timerEventAsync.StopAsync();
            await timerEventAsync.StartAsync(_3msTimeSpan, true);
            timerEventAsync.DueTime.TotalMilliseconds.Should().Be(3);
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }


        [Test]
        public async ValueTask Method_Stop_MultipleCalls_Return_Last_DateTimeOffset()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, true, period: _1msTimeSpan, state: _testCallback);
            await Task.Delay(_delayMs);
            var initialTimestamp = timer.Stop();
            Console.WriteLine($"{nameof(initialTimestamp)}: {initialTimestamp}");
            await Task.Delay(_delayMs);
            var newerTimestamp = timer.Stop();
            Console.WriteLine($"{nameof(newerTimestamp)}: {newerTimestamp}");
            newerTimestamp.Should().BeSameDateAs((DateTimeOffset)initialTimestamp);
            _testCallback.HasExecuted.Should().BeTrue();
            _testCallback.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, true, period: _1msTimeSpan, state: _testCallbackAsync);
            await Task.Delay(_delayMs);
            var initialTimestampAsync = await timerAsync.StopAsync();
            Console.WriteLine($"{nameof(initialTimestampAsync)}: {initialTimestampAsync}");
            await Task.Delay(_delayMs);
            var newerTimestampAsync = await timerAsync.StopAsync();
            Console.WriteLine($"{nameof(newerTimestampAsync)}: {newerTimestampAsync}");
            newerTimestampAsync.Should().BeSameDateAs((DateTimeOffset)initialTimestampAsync);
            _testCallbackAsync.HasExecuted.Should().BeTrue();
            _testCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, true, period: _1msTimeSpan, state: _testEventCallbackAsync);
            await Task.Delay(_delayMs);
            var initialEventTimestampAsync = await timerEventAsync.StopAsync();
            Console.WriteLine($"{nameof(initialEventTimestampAsync)}: {initialEventTimestampAsync}");
            await Task.Delay(_delayMs);
            var newerEventTimestampAsync = await timerEventAsync.StopAsync();
            Console.WriteLine($"{nameof(newerEventTimestampAsync)}: {newerEventTimestampAsync}");
            newerEventTimestampAsync.Should().BeSameDateAs((DateTimeOffset)initialEventTimestampAsync);
            _testEventCallbackAsync.HasExecuted.Should().BeTrue();
            _testEventCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Method_Stop_Sets_Enabled_False()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, true, period: _1msTimeSpan, state: _testCallback);
            await Task.Delay(_delayMs);
            timer.Stop();
            timer.Enabled.Should().BeFalse();
            _testCallback.HasExecuted.Should().BeTrue();
            _testCallback.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, true, period: _1msTimeSpan, state: _testCallbackAsync);
            await Task.Delay(_delayMs);
            await timerAsync.StopAsync();
            timerAsync.Enabled.Should().BeFalse();
            _testCallbackAsync.HasExecuted.Should().BeTrue();
            _testCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, true, period: _1msTimeSpan, state: _testEventCallbackAsync);
            await Task.Delay(_delayMs);
            await timerEventAsync.StopAsync();
            timerEventAsync.Enabled.Should().BeFalse();
            _testEventCallbackAsync.HasExecuted.Should().BeTrue();
            _testEventCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Method_Stop_TimerNeverRan_Returns_DefaultDateTimeOffset()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, period: _1msTimeSpan, state: _testCallback);
            timer.Stop().Should().Be(default(DateTimeOffset));
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, period: _1msTimeSpan, state: _testCallbackAsync);
            (await timerAsync.StopAsync()).Should().Be(default(DateTimeOffset));
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, period: _1msTimeSpan, state: _testEventCallbackAsync);
            (await timerEventAsync.StopAsync()).Should().Be(default(DateTimeOffset));
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        [Test]
        public async ValueTask Method_Stop_With_Enabled_True()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, true, period: _1msTimeSpan, state: _testCallback);
            timer.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            timer.Stop();
            timer.Enabled.Should().BeFalse();
            _testCallback.HasExecuted.Should().BeTrue();
            _testCallback.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, true, period: _1msTimeSpan, state: _testCallbackAsync);
            timerAsync.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timerAsync.StopAsync();
            timerAsync.Enabled.Should().BeFalse();
            _testCallbackAsync.HasExecuted.Should().BeTrue();
            _testCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, true, period: _1msTimeSpan, state: _testEventCallbackAsync);
            timerEventAsync.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timerEventAsync.StopAsync();
            timerEventAsync.Enabled.Should().BeFalse();
            _testEventCallbackAsync.HasExecuted.Should().BeTrue();
            _testEventCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);
        }


        [Test]
        public async ValueTask Method_Restart_Sets_Restarts()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, period: _1msTimeSpan, state: _testCallback);
            timer.Restarts.Should().Be(0);
            timer.Restart();
            timer.Restart();
            timer.Restarts.Should().Be(2);
            timer.Start();
            await Task.Delay(_delayMs);
            timer.Stop();
            timer.Restarts.Should().Be(2);
            _testCallback.HasExecuted.Should().BeTrue();
            _testCallback.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, period: _1msTimeSpan, state: _testCallbackAsync);
            timerAsync.Restarts.Should().Be(0);
            await timerAsync.RestartAsync();
            await timerAsync.RestartAsync();
            await timerAsync.StopAsync();
            timerAsync.Restarts.Should().Be(2);
            await timerAsync.StartAsync();
            await Task.Delay(_delayMs);
            await timerAsync.StopAsync();
            timerAsync.Restarts.Should().Be(2);
            _testCallbackAsync.HasExecuted.Should().BeTrue();
            _testCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, period: _1msTimeSpan, state: _testEventCallbackAsync);
            timerEventAsync.Restarts.Should().Be(0);
            await timerEventAsync.RestartAsync();
            await timerEventAsync.RestartAsync();
            await timerEventAsync.StopAsync();
            timerEventAsync.Restarts.Should().Be(2);
            await timerEventAsync.StartAsync();
            await Task.Delay(_delayMs);
            await timerEventAsync.StopAsync();
            timerEventAsync.Restarts.Should().Be(2);
            _testEventCallbackAsync.HasExecuted.Should().BeTrue();
            _testEventCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Method_Restart_With_Enabled_False()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, period: _1msTimeSpan, state: _testCallback);
            timer.Restart();
            timer.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            _testCallback.HasExecuted.Should().BeTrue();
            _testCallback.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, period: _1msTimeSpan, state: _testCallbackAsync);
            await timerAsync.RestartAsync();
            timerAsync.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            _testCallbackAsync.HasExecuted.Should().BeTrue();
            _testCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, period: _1msTimeSpan, state: _testEventCallbackAsync);
            await timerEventAsync.RestartAsync();
            timerEventAsync.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            _testEventCallbackAsync.HasExecuted.Should().BeTrue();
            _testEventCallbackAsync.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Method_Restart_Updates_DueTime()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethodAsync, dueTime: _1msTimeSpan, state: _testCallback);
            timer.DueTime.TotalMilliseconds.Should().Be(1);
            timer.Restart(_3msTimeSpan);
            timer.DueTime.TotalMilliseconds.Should().Be(1);
            timer.Restart(_3msTimeSpan, true);
            timer.DueTime.TotalMilliseconds.Should().Be(3);
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, dueTime: _1msTimeSpan, state: _testCallbackAsync);
            timerAsync.DueTime.TotalMilliseconds.Should().Be(1);
            await timerAsync.RestartAsync(_3msTimeSpan);
            timerAsync.DueTime.TotalMilliseconds.Should().Be(1);
            await timerAsync.RestartAsync(_3msTimeSpan, true);
            timerAsync.DueTime.TotalMilliseconds.Should().Be(3);
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, dueTime: _1msTimeSpan, state: _testEventCallbackAsync);
            timerEventAsync.DueTime.TotalMilliseconds.Should().Be(1);
            await timerEventAsync.RestartAsync(_3msTimeSpan);
            timerEventAsync.DueTime.TotalMilliseconds.Should().Be(1);
            await timerEventAsync.RestartAsync(_3msTimeSpan, true);
            timerEventAsync.DueTime.TotalMilliseconds.Should().Be(3);
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }


        [Test]
        public async ValueTask Method_Dispose()
        {
            using IComponentTimer timer = new ComponentTimer(CallbackMethod, state: _testCallback);
            timer.Dispose();
            timer.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timer.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timer.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timer.Enabled.Should().BeFalse();
            timer.IsTicking.Should().BeFalse();
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);

            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethodAsync, state: _testCallbackAsync);
            await timerAsync.DisposeAsync();
            timerAsync.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timerAsync.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timerAsync.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timerAsync.Enabled.Should().BeFalse();
            timerAsync.IsTicking.Should().BeFalse();
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, state: _testEventCallbackAsync);
            await timerEventAsync.DisposeAsync();
            timerEventAsync.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timerEventAsync.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timerEventAsync.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timerEventAsync.Enabled.Should().BeFalse();
            timerEventAsync.IsTicking.Should().BeFalse();
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        [Test]
        public async ValueTask Method_Dispose_StartedTimer()
        {
            var startTimer = true;

            using IComponentTimer timer = new ComponentTimer(CallbackMethod, startTimer, state: _testCallback);
            timer.Dispose();
            timer.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timer.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timer.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timer.Enabled.Should().BeFalse();
            timer.IsTicking.Should().BeFalse();
            timer.Enabled.Should().BeFalse();
            timer.IsTicking.Should().BeFalse();
            _testCallback.HasExecuted.Should().BeFalse();
            _testCallback.ExecuteCount.Should().Be(0);


            using IComponentTimer timerAsync = new ComponentTimer(CallbackMethod, startTimer, state: _testCallbackAsync);
            await timerAsync.DisposeAsync();
            timerAsync.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timerAsync.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timerAsync.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timerAsync.Enabled.Should().BeFalse();
            timerAsync.IsTicking.Should().BeFalse();
            timerAsync.Enabled.Should().BeFalse();
            timerAsync.IsTicking.Should().BeFalse();
            _testCallbackAsync.HasExecuted.Should().BeFalse();
            _testCallbackAsync.ExecuteCount.Should().Be(0);


            using IComponentTimer timerEventAsync = new ComponentTimer(TestEventCallback, startTimer, state: _testEventCallbackAsync);
            await timerEventAsync.DisposeAsync();
            timerEventAsync.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timerEventAsync.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timerEventAsync.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timerEventAsync.Enabled.Should().BeFalse();
            timerEventAsync.IsTicking.Should().BeFalse();
            timerEventAsync.Enabled.Should().BeFalse();
            timerEventAsync.IsTicking.Should().BeFalse();
            _testEventCallbackAsync.HasExecuted.Should().BeFalse();
            _testEventCallbackAsync.ExecuteCount.Should().Be(0);
        }

        #endregion Methods
    }
}
