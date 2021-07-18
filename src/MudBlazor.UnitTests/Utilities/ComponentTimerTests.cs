using System;
using System.Diagnostics;
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
        private CallbackExecuteTest _testCallback_1;
        private CallbackExecuteTest _testCallback_2;
        private CallbackExecuteTest _testCallback_3;
        private CallbackExecuteTest _testCallback_4;
        private EventCallback CallbackEvent_3;
        private EventCallback<object> CallbackEvent_4;
        private class CallbackExecuteTest { public int ExecuteCount { get; set; } = -1; public bool HasExecuted { get; set; } };
        private void CallbackMethod_1<T>(T state) { CallbackMethodCore(state); }
        private async ValueTask CallbackMethod_2<T>(T state) { await ValueTask.CompletedTask; CallbackMethodCore(state); }
        private async Task CallbackMethod_3<T>(T state) { await Task.CompletedTask; CallbackMethodCore(state); }
        private async Task CallbackMethod_4<T>(T state) { await Task.CompletedTask; CallbackMethodCore(state); }
        private static void CallbackMethodCore<T>(T state, [CallerMemberName] string methodName = "???")
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
        private static async ValueTask LongRunningCallbackMethod_2Async<T>(T state)
        {
            CallbackMethodCore(state);
            var sw = Stopwatch.StartNew();
            await Task.Delay(TimeSpan.FromSeconds(10));
            Console.WriteLine($"Delay duration: {sw.Elapsed}");
        }
        private static async Task LongRunningEventCallbackMethod_4Async<T>(T state)
        {
            CallbackMethodCore(state);
            var sw = Stopwatch.StartNew();
            await Task.Delay(TimeSpan.FromSeconds(10));
            Console.WriteLine($"Delay duration: {sw.Elapsed}");
        }
        [SetUp]
        public void Setup()
        {
            _testCallback_1 = new CallbackExecuteTest { ExecuteCount = 0 };
            _testCallback_2 = new CallbackExecuteTest { ExecuteCount = 0 };

            _testCallback_3 = new CallbackExecuteTest { ExecuteCount = 0 };
            CallbackEvent_3 = EventCallback.Factory.Create(this, CallbackMethod_3);

            _testCallback_4 = new CallbackExecuteTest { ExecuteCount = 0 };
            CallbackEvent_4 = EventCallback.Factory.Create<object>(this, CallbackMethod_4);
        }

        #endregion Setup


        #region Constructors

        [Test]
        public void Constructor_ConcreteClass()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, state: _testCallback_1);
            timer_1.Should().As<ComponentTimer>();
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, state: _testCallback_2);
            timer_2.Should().As<ComponentTimer>();
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, state: _testCallback_3);
            timer_3.Should().As<ComponentTimer>();
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, state: _testCallback_4);
            timer_4.Should().As<ComponentTimer>();
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }

        #endregion Constructors


        #region Properties

        [Test]
        public async ValueTask Property_Enabled()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, period: _1msTimeSpan, state: _testCallback_1);
            timer_1.Enabled.Should().BeFalse();
            timer_1.Enabled = true;
            timer_1.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            timer_1.Enabled = false;
            timer_1.Enabled.Should().BeFalse();
            _testCallback_1.HasExecuted.Should().BeTrue();
            _testCallback_1.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, period: _1msTimeSpan, state: _testCallback_2);
            timer_2.Enabled.Should().BeFalse();
            timer_2.Enabled = true;
            timer_2.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            timer_2.Enabled = false;
            timer_2.Enabled.Should().BeFalse();
            _testCallback_2.HasExecuted.Should().BeTrue();
            _testCallback_2.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, period: _1msTimeSpan, state: _testCallback_3);
            timer_3.Enabled.Should().BeFalse();
            timer_3.Enabled = true;
            timer_3.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            timer_3.Enabled = false;
            timer_3.Enabled.Should().BeFalse();
            _testCallback_3.HasExecuted.Should().BeTrue();
            _testCallback_3.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, period: _1msTimeSpan, state: _testCallback_4);
            timer_4.Enabled.Should().BeFalse();
            timer_4.Enabled = true;
            timer_4.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            timer_4.Enabled = false;
            timer_4.Enabled.Should().BeFalse();
            _testCallback_4.HasExecuted.Should().BeTrue();
            _testCallback_4.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Property_IsTicking()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, true, period: _1msTimeSpan, state: _testCallback_1);
            timer_1.IsTicking.Should().BeTrue();
            await Task.Delay(_delayMs);
            timer_1.Stop();
            timer_1.IsTicking.Should().BeFalse();
            _testCallback_1.HasExecuted.Should().BeTrue();
            _testCallback_1.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, true, period: _1msTimeSpan, state: _testCallback_2);
            timer_2.IsTicking.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timer_2.StopAsync();
            timer_2.IsTicking.Should().BeFalse();
            _testCallback_2.HasExecuted.Should().BeTrue();
            _testCallback_2.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, true, period: _1msTimeSpan, state: _testCallback_3);
            timer_3.IsTicking.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timer_3.StopAsync();
            timer_3.IsTicking.Should().BeFalse();
            _testCallback_3.HasExecuted.Should().BeTrue();
            _testCallback_3.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, true, period: _1msTimeSpan, state: _testCallback_4);
            timer_4.IsTicking.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timer_4.StopAsync();
            timer_4.IsTicking.Should().BeFalse();
            _testCallback_4.HasExecuted.Should().BeTrue();
            _testCallback_4.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public void Property_IsTicking_With_InfiniteTimeSpan()
        {
            var interval = Timeout.InfiniteTimeSpan;

            using var timer_1 = new ComponentTimer(CallbackMethod_1, true, period: interval, state: _testCallback_1);
            timer_1.IsTicking.Should().BeFalse();
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, true, period: interval, state: _testCallback_2);
            timer_2.IsTicking.Should().BeFalse();
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, true, period: interval, state: _testCallback_3);
            timer_3.IsTicking.Should().BeFalse();
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, true, period: interval, state: _testCallback_4);
            timer_4.IsTicking.Should().BeFalse();
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_IsTicking_With_TimeSpanZero()
        {
            var interval = TimeSpan.Zero;

            using var timer_1 = new ComponentTimer(CallbackMethod_1, true, period: interval, state: _testCallback_1);
            timer_1.IsTicking.Should().BeFalse();
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, true, period: interval, state: _testCallback_2);
            timer_2.IsTicking.Should().BeFalse();
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, true, period: interval, state: _testCallback_3);
            timer_3.IsTicking.Should().BeFalse();
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, true, period: interval, state: _testCallback_4);
            timer_4.IsTicking.Should().BeFalse();
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_DueTime()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, dueTime: _1msTimeSpan, state: _testCallback_1);
            timer_1.DueTime.Should().NotBe(TimeSpan.Zero);
            timer_1.DueTime.Should().NotBe(Timeout.InfiniteTimeSpan);
            timer_1.DueTime.TotalMilliseconds.Should().Be(1);
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, dueTime: _1msTimeSpan, state: _testCallback_2);
            timer_2.DueTime.Should().NotBe(TimeSpan.Zero);
            timer_2.DueTime.Should().NotBe(Timeout.InfiniteTimeSpan);
            timer_2.DueTime.TotalMilliseconds.Should().Be(1);
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, dueTime: _1msTimeSpan, state: _testCallback_3);
            timer_3.DueTime.Should().NotBe(TimeSpan.Zero);
            timer_3.DueTime.Should().NotBe(Timeout.InfiniteTimeSpan);
            timer_3.DueTime.TotalMilliseconds.Should().Be(1);
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, dueTime: _1msTimeSpan, state: _testCallback_4);
            timer_4.DueTime.Should().NotBe(TimeSpan.Zero);
            timer_4.DueTime.Should().NotBe(Timeout.InfiniteTimeSpan);
            timer_4.DueTime.TotalMilliseconds.Should().Be(1);
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_DueTime_With_InfiniteTimeSpan()
        {
            var startDelay = Timeout.InfiniteTimeSpan;

            using var timer_1 = new ComponentTimer(CallbackMethod_1, dueTime: startDelay, state: _testCallback_1);
            timer_1.DueTime.Should().Be(TimeSpan.Zero);
            timer_1.DueTime.TotalMilliseconds.Should().Be(0);
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, dueTime: startDelay, state: _testCallback_2);
            timer_2.DueTime.Should().Be(TimeSpan.Zero);
            timer_2.DueTime.TotalMilliseconds.Should().Be(0);
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, dueTime: startDelay, state: _testCallback_3);
            timer_3.DueTime.Should().Be(TimeSpan.Zero);
            timer_3.DueTime.TotalMilliseconds.Should().Be(0);
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, dueTime: startDelay, state: _testCallback_4);
            timer_4.DueTime.Should().Be(TimeSpan.Zero);
            timer_4.DueTime.TotalMilliseconds.Should().Be(0);
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_DueTime_With_TimeSpanZero()
        {
            var startDelay = TimeSpan.Zero;

            using var timer_1 = new ComponentTimer(CallbackMethod_1, dueTime: startDelay, state: _testCallback_1);
            timer_1.DueTime.Should().Be(TimeSpan.Zero);
            timer_1.DueTime.TotalMilliseconds.Should().Be(0);
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, dueTime: startDelay, state: _testCallback_2);
            timer_2.DueTime.Should().Be(TimeSpan.Zero);
            timer_2.DueTime.TotalMilliseconds.Should().Be(0);
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, dueTime: startDelay, state: _testCallback_3);
            timer_3.DueTime.Should().Be(TimeSpan.Zero);
            timer_3.DueTime.TotalMilliseconds.Should().Be(0);
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, dueTime: startDelay, state: _testCallback_4);
            timer_4.DueTime.Should().Be(TimeSpan.Zero);
            timer_4.DueTime.TotalMilliseconds.Should().Be(0);
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_Period()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, period: _1msTimeSpan, state: _testCallback_1);
            timer_1.Period.Should().NotBe(TimeSpan.Zero);
            timer_1.Period.Should().NotBe(Timeout.InfiniteTimeSpan);
            timer_1.Period.TotalMilliseconds.Should().Be(1);
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, period: _1msTimeSpan, state: _testCallback_2);
            timer_2.Period.Should().NotBe(TimeSpan.Zero);
            timer_2.Period.Should().NotBe(Timeout.InfiniteTimeSpan);
            timer_2.Period.TotalMilliseconds.Should().Be(1);
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, period: _1msTimeSpan, state: _testCallback_3);
            timer_3.Period.Should().NotBe(TimeSpan.Zero);
            timer_3.Period.Should().NotBe(Timeout.InfiniteTimeSpan);
            timer_3.Period.TotalMilliseconds.Should().Be(1);
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, period: _1msTimeSpan, state: _testCallback_4);
            timer_4.Period.Should().NotBe(TimeSpan.Zero);
            timer_4.Period.Should().NotBe(Timeout.InfiniteTimeSpan);
            timer_4.Period.TotalMilliseconds.Should().Be(1);
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_Period_With_InfiniteTimeSpan()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, period: _1msTimeSpan, state: _testCallback_1);
            timer_1.Period.Should().NotBe(TimeSpan.Zero);
            timer_1.Period.TotalMilliseconds.Should().Be(1);
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, period: _1msTimeSpan, state: _testCallback_2);
            timer_2.Period.Should().NotBe(TimeSpan.Zero);
            timer_2.Period.TotalMilliseconds.Should().Be(1);
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, period: _1msTimeSpan, state: _testCallback_3);
            timer_3.Period.Should().NotBe(TimeSpan.Zero);
            timer_3.Period.TotalMilliseconds.Should().Be(1);
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, period: _1msTimeSpan, state: _testCallback_4);
            timer_4.Period.Should().NotBe(TimeSpan.Zero);
            timer_4.Period.TotalMilliseconds.Should().Be(1);
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }

        [Test]
        public void Property_Period_With_TimeSpanZero()
        {
            var interval = TimeSpan.Zero;

            using var timer_1 = new ComponentTimer(CallbackMethod_1, period: interval, state: _testCallback_1);
            timer_1.Period.Should().Be(TimeSpan.Zero);
            timer_1.Period.TotalMilliseconds.Should().Be(0);
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, period: interval, state: _testCallback_2);
            timer_2.Period.Should().Be(TimeSpan.Zero);
            timer_2.Period.TotalMilliseconds.Should().Be(0);
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, period: interval, state: _testCallback_3);
            timer_3.Period.Should().Be(TimeSpan.Zero);
            timer_3.Period.TotalMilliseconds.Should().Be(0);
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, period: interval, state: _testCallback_4);
            timer_4.Period.Should().Be(TimeSpan.Zero);
            timer_4.Period.TotalMilliseconds.Should().Be(0);
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }

        [Test]
        public async ValueTask Property_Elapsed()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, period: _1msTimeSpan, state: _testCallback_1);
            timer_1.Elapsed.Should().Be(TimeSpan.Zero);
            timer_1.Start();
            await Task.Delay(_delayMs);
            timer_1.Stop();
            timer_1.Elapsed.TotalMilliseconds.Should().BeGreaterThan(0);
            _testCallback_1.HasExecuted.Should().BeTrue();
            _testCallback_1.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, period: _1msTimeSpan, state: _testCallback_2);
            timer_2.Elapsed.Should().Be(TimeSpan.Zero);
            await timer_2.StartAsync();
            await Task.Delay(_delayMs);
            await timer_2.StopAsync();
            timer_2.Elapsed.TotalMilliseconds.Should().BeGreaterThan(0);
            _testCallback_2.HasExecuted.Should().BeTrue();
            _testCallback_2.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, period: _1msTimeSpan, state: _testCallback_3);
            timer_3.Elapsed.Should().Be(TimeSpan.Zero);
            await timer_3.StartAsync();
            await Task.Delay(_delayMs);
            await timer_3.StopAsync();
            timer_3.Elapsed.TotalMilliseconds.Should().BeGreaterThan(0);
            _testCallback_3.HasExecuted.Should().BeTrue();
            _testCallback_3.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, period: _1msTimeSpan, state: _testCallback_4);
            timer_4.Elapsed.Should().Be(TimeSpan.Zero);
            await timer_4.StartAsync();
            await Task.Delay(_delayMs);
            await timer_4.StopAsync();
            timer_4.Elapsed.TotalMilliseconds.Should().BeGreaterThan(0);
            _testCallback_4.HasExecuted.Should().BeTrue();
            _testCallback_4.ExecuteCount.Should().BeGreaterThan(0);
        }

        #endregion Properties


        #region Methods

        [Test]
        public async ValueTask Method_Start_MultipleCalls_Return_Initial_DateTimeOffset()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, period: _1msTimeSpan, state: _testCallback_1);
            var initialTimestamp_1 = timer_1.Start();
            Console.WriteLine($"{nameof(initialTimestamp_1)}: {initialTimestamp_1}");
            await Task.Delay(_delayMs);
            var newerTimestamp_1 = timer_1.Start();
            timer_1.Stop();
            Console.WriteLine($"{nameof(newerTimestamp_1)}: {newerTimestamp_1}");
            newerTimestamp_1.Should().BeSameDateAs((DateTimeOffset)initialTimestamp_1);
            _testCallback_1.HasExecuted.Should().BeTrue();
            _testCallback_1.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, period: _1msTimeSpan, state: _testCallback_2);
            var initialTimestamp_2 = await timer_2.StartAsync();
            Console.WriteLine($"{nameof(initialTimestamp_2)}: {initialTimestamp_2}");
            await Task.Delay(_delayMs);
            var newerTimestamp_2 = await timer_2.StartAsync();
            await timer_2.StopAsync();
            Console.WriteLine($"{nameof(newerTimestamp_2)}: {newerTimestamp_2}");
            newerTimestamp_2.Should().BeSameDateAs((DateTimeOffset)initialTimestamp_2);
            _testCallback_2.HasExecuted.Should().BeTrue();
            _testCallback_2.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, period: _1msTimeSpan, state: _testCallback_3);
            var initialTimestamp_3 = await timer_3.StartAsync();
            Console.WriteLine($"{nameof(initialTimestamp_3)}: {initialTimestamp_3}");
            await Task.Delay(_delayMs);
            var newerTimestamp_3 = await timer_3.StartAsync();
            await timer_3.StopAsync();
            Console.WriteLine($"{nameof(newerTimestamp_3)}: {newerTimestamp_3}");
            newerTimestamp_3.Should().BeSameDateAs((DateTimeOffset)initialTimestamp_3);
            _testCallback_3.HasExecuted.Should().BeTrue();
            _testCallback_3.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, period: _1msTimeSpan, state: _testCallback_4);
            var initialTimestamp_4 = await timer_4.StartAsync();
            Console.WriteLine($"{nameof(initialTimestamp_4)}: {initialTimestamp_4}");
            await Task.Delay(_delayMs);
            var newerTimestamp_4 = await timer_4.StartAsync();
            await timer_4.StopAsync();
            Console.WriteLine($"{nameof(newerTimestamp_4)}: {newerTimestamp_4}");
            newerTimestamp_4.Should().BeSameDateAs((DateTimeOffset)initialTimestamp_4);
            _testCallback_4.HasExecuted.Should().BeTrue();
            _testCallback_4.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Method_Start_Sets_Enabled_True()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, period: _1msTimeSpan, state: _testCallback_1);
            timer_1.Start();
            timer_1.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            timer_1.Stop();
            _testCallback_1.HasExecuted.Should().BeTrue();
            _testCallback_1.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, period: _1msTimeSpan, state: _testCallback_2);
            await timer_2.StartAsync();
            timer_2.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timer_2.StopAsync();
            _testCallback_2.HasExecuted.Should().BeTrue();
            _testCallback_2.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, period: _1msTimeSpan, state: _testCallback_3);
            await timer_3.StartAsync();
            timer_3.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timer_3.StopAsync();
            _testCallback_3.HasExecuted.Should().BeTrue();
            _testCallback_3.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, period: _1msTimeSpan, state: _testCallback_4);
            await timer_4.StartAsync();
            timer_4.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timer_4.StopAsync();
            _testCallback_4.HasExecuted.Should().BeTrue();
            _testCallback_4.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Method_Start_Updates_DueTime()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, dueTime: _1msTimeSpan, period: _1msTimeSpan, state: _testCallback_1);
            timer_1.DueTime.TotalMilliseconds.Should().Be(1);
            timer_1.Start(_3msTimeSpan);
            timer_1.DueTime.TotalMilliseconds.Should().Be(1);
            timer_1.Stop();
            timer_1.Start(_3msTimeSpan, true);
            timer_1.DueTime.TotalMilliseconds.Should().Be(3);
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, dueTime: _1msTimeSpan, period: _1msTimeSpan, state: _testCallback_2);
            timer_2.DueTime.TotalMilliseconds.Should().Be(1);
            await timer_2.StartAsync(_3msTimeSpan);
            timer_2.DueTime.TotalMilliseconds.Should().Be(1);
            await timer_2.StopAsync();
            await timer_2.StartAsync(_3msTimeSpan, true);
            timer_2.DueTime.TotalMilliseconds.Should().Be(3);
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, dueTime: _1msTimeSpan, period: _1msTimeSpan, state: _testCallback_3);
            timer_3.DueTime.TotalMilliseconds.Should().Be(1);
            await timer_3.StartAsync(_3msTimeSpan);
            timer_3.DueTime.TotalMilliseconds.Should().Be(1);
            await timer_3.StopAsync();
            await timer_3.StartAsync(_3msTimeSpan, true);
            timer_3.DueTime.TotalMilliseconds.Should().Be(3);
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, dueTime: _1msTimeSpan, period: _1msTimeSpan, state: _testCallback_4);
            timer_4.DueTime.TotalMilliseconds.Should().Be(1);
            await timer_4.StartAsync(_3msTimeSpan);
            timer_4.DueTime.TotalMilliseconds.Should().Be(1);
            await timer_4.StopAsync();
            await timer_4.StartAsync(_3msTimeSpan, true);
            timer_4.DueTime.TotalMilliseconds.Should().Be(3);
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }


        [Test]
        public async ValueTask Method_Stop_MultipleCalls_Return_Last_DateTimeOffset()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, true, period: _1msTimeSpan, state: _testCallback_1);
            await Task.Delay(_delayMs);
            var initialTimestamp_1 = timer_1.Stop();
            Console.WriteLine($"{nameof(initialTimestamp_1)}: {initialTimestamp_1}");
            await Task.Delay(_delayMs);
            var newerTimestamp_1 = timer_1.Stop();
            Console.WriteLine($"{nameof(newerTimestamp_1)}: {newerTimestamp_1}");
            newerTimestamp_1.Should().BeSameDateAs((DateTimeOffset)initialTimestamp_1);
            _testCallback_1.HasExecuted.Should().BeTrue();
            _testCallback_1.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, true, period: _1msTimeSpan, state: _testCallback_2);
            await Task.Delay(_delayMs);
            var initialTimestamp_2 = await timer_2.StopAsync();
            Console.WriteLine($"{nameof(initialTimestamp_2)}: {initialTimestamp_2}");
            await Task.Delay(_delayMs);
            var newerTimestamp_2 = await timer_2.StopAsync();
            Console.WriteLine($"{nameof(newerTimestamp_2)}: {newerTimestamp_2}");
            newerTimestamp_2.Should().BeSameDateAs((DateTimeOffset)initialTimestamp_2);
            _testCallback_2.HasExecuted.Should().BeTrue();
            _testCallback_2.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, true, period: _1msTimeSpan, state: _testCallback_3);
            await Task.Delay(_delayMs);
            var initialTimestamp_3 = await timer_3.StopAsync();
            Console.WriteLine($"{nameof(initialTimestamp_3)}: {initialTimestamp_3}");
            await Task.Delay(_delayMs);
            var newerTimestamp_3 = await timer_3.StopAsync();
            Console.WriteLine($"{nameof(newerTimestamp_3)}: {newerTimestamp_3}");
            newerTimestamp_3.Should().BeSameDateAs((DateTimeOffset)initialTimestamp_3);
            _testCallback_3.HasExecuted.Should().BeTrue();
            _testCallback_3.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, true, period: _1msTimeSpan, state: _testCallback_4);
            await Task.Delay(_delayMs);
            var initialTimestamp_4 = await timer_4.StopAsync();
            Console.WriteLine($"{nameof(initialTimestamp_4)}: {initialTimestamp_4}");
            await Task.Delay(_delayMs);
            var newerTimestamp_4 = await timer_4.StopAsync();
            Console.WriteLine($"{nameof(newerTimestamp_4)}: {newerTimestamp_4}");
            newerTimestamp_4.Should().BeSameDateAs((DateTimeOffset)newerTimestamp_4);
            _testCallback_4.HasExecuted.Should().BeTrue();
            _testCallback_4.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Method_Stop_Sets_Enabled_False()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, true, period: _1msTimeSpan, state: _testCallback_1);
            await Task.Delay(_delayMs);
            timer_1.Stop();
            timer_1.Enabled.Should().BeFalse();
            _testCallback_1.HasExecuted.Should().BeTrue();
            _testCallback_1.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, true, period: _1msTimeSpan, state: _testCallback_2);
            await Task.Delay(_delayMs);
            await timer_2.StopAsync();
            timer_2.Enabled.Should().BeFalse();
            _testCallback_2.HasExecuted.Should().BeTrue();
            _testCallback_2.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, true, period: _1msTimeSpan, state: _testCallback_3);
            await Task.Delay(_delayMs);
            await timer_3.StopAsync();
            timer_3.Enabled.Should().BeFalse();
            _testCallback_3.HasExecuted.Should().BeTrue();
            _testCallback_3.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, true, period: _1msTimeSpan, state: _testCallback_4);
            await Task.Delay(_delayMs);
            await timer_4.StopAsync();
            timer_4.Enabled.Should().BeFalse();
            _testCallback_4.HasExecuted.Should().BeTrue();
            _testCallback_4.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Method_Stop_TimerNeverRan_Returns_DefaultDateTimeOffset()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, period: _1msTimeSpan, state: _testCallback_1);
            timer_1.Stop().Should().Be(default(DateTimeOffset));
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, period: _1msTimeSpan, state: _testCallback_2);
            (await timer_2.StopAsync()).Should().Be(default(DateTimeOffset));
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, period: _1msTimeSpan, state: _testCallback_3);
            (await timer_3.StopAsync()).Should().Be(default(DateTimeOffset));
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, period: _1msTimeSpan, state: _testCallback_4);
            (await timer_4.StopAsync()).Should().Be(default(DateTimeOffset));
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }

        [Test]
        public async ValueTask Method_Stop_With_Enabled_True()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, true, period: _1msTimeSpan, state: _testCallback_1);
            timer_1.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            timer_1.Stop();
            timer_1.Enabled.Should().BeFalse();
            _testCallback_1.HasExecuted.Should().BeTrue();
            _testCallback_1.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, true, period: _1msTimeSpan, state: _testCallback_2);
            timer_2.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timer_2.StopAsync();
            timer_2.Enabled.Should().BeFalse();
            _testCallback_2.HasExecuted.Should().BeTrue();
            _testCallback_2.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, true, period: _1msTimeSpan, state: _testCallback_3);
            timer_3.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timer_3.StopAsync();
            timer_3.Enabled.Should().BeFalse();
            _testCallback_3.HasExecuted.Should().BeTrue();
            _testCallback_3.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, true, period: _1msTimeSpan, state: _testCallback_4);
            timer_4.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            await timer_4.StopAsync();
            timer_4.Enabled.Should().BeFalse();
            _testCallback_4.HasExecuted.Should().BeTrue();
            _testCallback_4.ExecuteCount.Should().BeGreaterThan(0);
        }


        [Test]
        public async ValueTask Method_Restart_Sets_Restarts()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, period: _1msTimeSpan, state: _testCallback_1);
            timer_1.Restarts.Should().Be(0);
            timer_1.Restart();
            timer_1.Restart();
            timer_1.Stop();
            timer_1.Restarts.Should().Be(2);
            timer_1.Start();
            await Task.Delay(_delayMs);
            timer_1.Stop();
            timer_1.Restarts.Should().Be(2);
            _testCallback_1.HasExecuted.Should().BeTrue();
            _testCallback_1.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, period: _1msTimeSpan, state: _testCallback_2);
            timer_2.Restarts.Should().Be(0);
            await timer_2.RestartAsync();
            await timer_2.RestartAsync();
            await timer_2.StopAsync();
            timer_2.Restarts.Should().Be(2);
            await timer_2.StartAsync();
            await Task.Delay(_delayMs);
            await timer_2.StopAsync();
            timer_2.Restarts.Should().Be(2);
            _testCallback_2.HasExecuted.Should().BeTrue();
            _testCallback_2.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, period: _1msTimeSpan, state: _testCallback_3);
            timer_3.Restarts.Should().Be(0);
            await timer_3.RestartAsync();
            await timer_3.RestartAsync();
            await timer_3.StopAsync();
            timer_3.Restarts.Should().Be(2);
            await timer_3.StartAsync();
            await Task.Delay(_delayMs);
            await timer_3.StopAsync();
            timer_3.Restarts.Should().Be(2);
            _testCallback_3.HasExecuted.Should().BeTrue();
            _testCallback_3.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, period: _1msTimeSpan, state: _testCallback_4);
            timer_4.Restarts.Should().Be(0);
            await timer_4.RestartAsync();
            await timer_4.RestartAsync();
            await timer_4.StopAsync();
            timer_4.Restarts.Should().Be(2);
            await timer_4.StartAsync();
            await Task.Delay(_delayMs);
            await timer_4.StopAsync();
            timer_4.Restarts.Should().Be(2);
            _testCallback_4.HasExecuted.Should().BeTrue();
            _testCallback_4.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Method_Restart_With_Enabled_False()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, period: _1msTimeSpan, state: _testCallback_1);
            timer_1.Restart();
            timer_1.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            _testCallback_1.HasExecuted.Should().BeTrue();
            _testCallback_1.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, period: _1msTimeSpan, state: _testCallback_2);
            await timer_2.RestartAsync();
            timer_2.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            _testCallback_2.HasExecuted.Should().BeTrue();
            _testCallback_2.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, period: _1msTimeSpan, state: _testCallback_3);
            await timer_3.RestartAsync();
            timer_3.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            _testCallback_3.HasExecuted.Should().BeTrue();
            _testCallback_3.ExecuteCount.Should().BeGreaterThan(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, period: _1msTimeSpan, state: _testCallback_4);
            await timer_4.RestartAsync();
            timer_4.Enabled.Should().BeTrue();
            await Task.Delay(_delayMs);
            _testCallback_4.HasExecuted.Should().BeTrue();
            _testCallback_4.ExecuteCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async ValueTask Method_Restart_Updates_DueTime()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_2, dueTime: _1msTimeSpan, state: _testCallback_1);
            timer_1.DueTime.TotalMilliseconds.Should().Be(1);
            timer_1.Restart(_3msTimeSpan);
            timer_1.DueTime.TotalMilliseconds.Should().Be(1);
            timer_1.Restart(_3msTimeSpan, true);
            timer_1.DueTime.TotalMilliseconds.Should().Be(3);
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, dueTime: _1msTimeSpan, state: _testCallback_2);
            timer_2.DueTime.TotalMilliseconds.Should().Be(1);
            await timer_2.RestartAsync(_3msTimeSpan);
            timer_2.DueTime.TotalMilliseconds.Should().Be(1);
            await timer_2.RestartAsync(_3msTimeSpan, true);
            timer_2.DueTime.TotalMilliseconds.Should().Be(3);
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, dueTime: _1msTimeSpan, state: _testCallback_3);
            timer_3.DueTime.TotalMilliseconds.Should().Be(1);
            await timer_3.RestartAsync(_3msTimeSpan);
            timer_3.DueTime.TotalMilliseconds.Should().Be(1);
            await timer_3.RestartAsync(_3msTimeSpan, true);
            timer_3.DueTime.TotalMilliseconds.Should().Be(3);
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, dueTime: _1msTimeSpan, state: _testCallback_4);
            timer_4.DueTime.TotalMilliseconds.Should().Be(1);
            await timer_4.RestartAsync(_3msTimeSpan);
            timer_4.DueTime.TotalMilliseconds.Should().Be(1);
            await timer_4.RestartAsync(_3msTimeSpan, true);
            timer_4.DueTime.TotalMilliseconds.Should().Be(3);
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }


        [Test]
        public async ValueTask Method_Dispose()
        {
            using var timer_1 = new ComponentTimer(CallbackMethod_1, state: _testCallback_1);
            timer_1.Dispose();
            timer_1.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timer_1.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timer_1.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timer_1.Enabled.Should().BeFalse();
            timer_1.IsTicking.Should().BeFalse();
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);

            using var timer_2 = new ComponentTimer(CallbackMethod_2, state: _testCallback_2);
            await timer_2.DisposeAsync();
            timer_2.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timer_2.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timer_2.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timer_2.Enabled.Should().BeFalse();
            timer_2.IsTicking.Should().BeFalse();
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, state: _testCallback_3);
            await timer_3.DisposeAsync();
            timer_3.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timer_3.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timer_3.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timer_3.Enabled.Should().BeFalse();
            timer_3.IsTicking.Should().BeFalse();
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, state: _testCallback_4);
            await timer_4.DisposeAsync();
            timer_4.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timer_4.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timer_4.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timer_4.Enabled.Should().BeFalse();
            timer_4.IsTicking.Should().BeFalse();
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }

        [Test]
        public async ValueTask Method_Dispose_StartedTimer()
        {
            var startTimer = true;

            using var timer_1 = new ComponentTimer(CallbackMethod_1, startTimer, state: _testCallback_1);
            timer_1.Dispose();
            timer_1.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timer_1.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timer_1.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timer_1.Enabled.Should().BeFalse();
            timer_1.IsTicking.Should().BeFalse();
            timer_1.Enabled.Should().BeFalse();
            timer_1.IsTicking.Should().BeFalse();
            _testCallback_1.HasExecuted.Should().BeFalse();
            _testCallback_1.ExecuteCount.Should().Be(0);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, startTimer, state: _testCallback_2);
            await timer_2.DisposeAsync();
            timer_2.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timer_2.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timer_2.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timer_2.Enabled.Should().BeFalse();
            timer_2.IsTicking.Should().BeFalse();
            timer_2.Enabled.Should().BeFalse();
            timer_2.IsTicking.Should().BeFalse();
            _testCallback_2.HasExecuted.Should().BeFalse();
            _testCallback_2.ExecuteCount.Should().Be(0);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, startTimer, state: _testCallback_3);
            await timer_3.DisposeAsync();
            timer_3.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timer_3.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timer_3.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timer_3.Enabled.Should().BeFalse();
            timer_3.IsTicking.Should().BeFalse();
            timer_3.Enabled.Should().BeFalse();
            timer_3.IsTicking.Should().BeFalse();
            _testCallback_3.HasExecuted.Should().BeFalse();
            _testCallback_3.ExecuteCount.Should().Be(0);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, startTimer, state: _testCallback_4);
            await timer_4.DisposeAsync();
            timer_4.DueTime.Should().Be(Timeout.InfiniteTimeSpan);
            timer_4.Period.Should().Be(Timeout.InfiniteTimeSpan);
            timer_4.Elapsed.TotalMilliseconds.Should().Be(Timeout.Infinite);
            timer_4.Enabled.Should().BeFalse();
            timer_4.IsTicking.Should().BeFalse();
            timer_4.Enabled.Should().BeFalse();
            timer_4.IsTicking.Should().BeFalse();
            _testCallback_4.HasExecuted.Should().BeFalse();
            _testCallback_4.ExecuteCount.Should().Be(0);
        }

        #endregion Methods


        #region Iterations

        [Test]
        public async ValueTask Timer_Iterations_Sets_Enabled_False()
        {
            var repeat = 3;

            using var timer_1 = new ComponentTimer(CallbackMethod_1, true, iterations: repeat, period: _1msTimeSpan, state: _testCallback_1);
            await Task.Delay(TimeSpan.FromSeconds(1));
            timer_1.Enabled.Should().BeFalse();
            timer_1.Iterations.Should().Be(3);
            _testCallback_1.HasExecuted.Should().BeTrue();
            _testCallback_1.ExecuteCount.Should().Be(3);


            using var timer_2 = new ComponentTimer(CallbackMethod_2, true, iterations: repeat, period: _1msTimeSpan, state: _testCallback_2);
            await Task.Delay(TimeSpan.FromSeconds(1));
            timer_2.Enabled.Should().BeFalse();
            timer_2.Iterations.Should().Be(3);
            _testCallback_2.HasExecuted.Should().BeTrue();
            _testCallback_2.ExecuteCount.Should().Be(3);


            using var timer_3 = new ComponentTimer(CallbackEvent_3, true, iterations: repeat, period: _1msTimeSpan, state: _testCallback_3);
            await Task.Delay(TimeSpan.FromSeconds(1));
            timer_3.Enabled.Should().BeFalse();
            timer_3.Iterations.Should().Be(3);
            _testCallback_3.HasExecuted.Should().BeTrue();
            _testCallback_3.ExecuteCount.Should().Be(3);


            using var timer_4 = new ComponentTimer(CallbackEvent_4, true, iterations: repeat, period: _1msTimeSpan, state: _testCallback_4);
            await Task.Delay(TimeSpan.FromSeconds(1));
            timer_4.Enabled.Should().BeFalse();
            timer_4.Iterations.Should().Be(3);
            _testCallback_4.HasExecuted.Should().BeTrue();
            _testCallback_4.ExecuteCount.Should().Be(3);
        }

        #endregion Iterations


        #region NoReentrancy

        [Test]
        public async ValueTask Timer_NoReentrancy()
        {
            using var timer_2 = new ComponentTimer(LongRunningCallbackMethod_2Async, true, period: _1msTimeSpan, state: _testCallback_2);
            await Task.Delay(TimeSpan.FromSeconds(1));
            timer_2.Stop();
            _testCallback_2.HasExecuted.Should().BeTrue();
            _testCallback_2.ExecuteCount.Should().Be(1);


            var longRunningCallbackEvent = EventCallback.Factory.Create(this, LongRunningEventCallbackMethod_4Async);
            using var timer_4 = new ComponentTimer(longRunningCallbackEvent, true, period: _1msTimeSpan, state: _testCallback_4);
            await Task.Delay(TimeSpan.FromSeconds(1));
            timer_4.Stop();
            _testCallback_4.HasExecuted.Should().BeTrue();
            _testCallback_4.ExecuteCount.Should().Be(1);
        }

        #endregion NoReentrancy
    }
}
