// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.Utilities.Throttle;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Throttle;

#nullable enable
[TestFixture]
public class ThrottleDispatcherTests
{
    [Test]
    public async Task ThrottleDispatcher_ThrottleAsync()
    {
        // Arrange
        var counter = 0;
        var dispatcher = new ThrottleDispatcher(100);
        var tasks = new List<Task>();

        async Task Invoke()
        {
            Interlocked.Increment(ref counter);
            await Task.Yield();
        }

        Task CallThrottleAsyncAfterDelay(int delay)
        {
            Task.Delay(delay).ConfigureAwait(false).GetAwaiter().GetResult();

            return dispatcher.ThrottleAsync(Invoke);
        }

        // Act
        for (var i = 0; i < 20; i++)
        {
            tasks.Add(CallThrottleAsyncAfterDelay(50));
        }

        // Assert
        await Task.WhenAll(tasks);
        counter.Should().BeInRange(9, 11);
    }

    [Test]
    public async Task ThrottleDispatcher_ThrottleDelayAfterExecutionAsync()
    {
        // Arrange
        var counter = 0;
        var dispatcher = new ThrottleDispatcher(100, delayAfterExecution: true);
        var tasks = new List<Task>();

        async Task Invoke()
        {
            Interlocked.Increment(ref counter);
            await Task.Delay(50);
        }

        Task CallThrottleAsyncAfterDelay(int delay)
        {
            Task.Delay(delay).ConfigureAwait(false).GetAwaiter().GetResult();

            return dispatcher.ThrottleAsync(Invoke);
        }

        // Act
        for (var i = 0; i < 20; i++)
        {
            tasks.Add(CallThrottleAsyncAfterDelay(50));
        }

        // Assert
        await Task.WhenAll(tasks);
        counter.Should().BeInRange(6, 8);
    }

    [Test]
    public async Task ThrottleAsync_ResetsIntervalOnException()
    {
        // Arrange
        var dispatcher = new ThrottleDispatcher(100, resetIntervalOnException: true);
        var counter = 0;

        // Act & Assert
        var throttledAction = async () =>
        {
            await dispatcher.ThrottleAsync(async () =>
            {
                await Task.Delay(50);
                Interlocked.Increment(ref counter);

                throw new InvalidOperationException();
            });
        };
        await throttledAction.Should().ThrowAsync<InvalidOperationException>();
        await dispatcher.ThrottleAsync(async () =>
        {
            await Task.Delay(50);
            Interlocked.Increment(ref counter);
        });
        counter.Should().Be(2);
    }
}
