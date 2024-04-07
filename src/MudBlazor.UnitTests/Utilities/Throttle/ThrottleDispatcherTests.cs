// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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

        Task Invoke()
        {
            counter++;

            return Task.CompletedTask;
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
        counter.Should().Be(10);
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
            counter++;
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
        counter.Should().Be(7);
    }

    [Test]
    public async Task ThrottleAsync_ResetsIntervalOnException()
    {
        // Arrange
        var dispatcher = new ThrottleDispatcher(100, resetIntervalOnException: true);
        var callCount = 0;

        // Act & Assert
        var throttledAction = async () =>
        {
            await dispatcher.ThrottleAsync(async () =>
            {
                await Task.Delay(50);
                callCount++;

                throw new InvalidOperationException();
            });
        };
        await throttledAction.Should().ThrowAsync<InvalidOperationException>();
        await dispatcher.ThrottleAsync(async () =>
        {
            await Task.Delay(50);
            callCount++;
        });
        callCount.Should().Be(2);
    }
}
