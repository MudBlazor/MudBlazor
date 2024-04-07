// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.Utilities.Debounce;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Debounce;

#nullable enable
[TestFixture]
public class DebounceDispatcherTests
{
    [Test]
    public async Task DebounceAsync_MultipleCallsWithinInterval_ExecutesOnce()
    {
        // Arrange
        var debounceDispatcher = new DebounceDispatcher(100);
        var counter = 0;
        Task Invoke()
        {
            counter++;

            return Task.CompletedTask;
        }

        // Act
        var tasks = new[]
        {
            debounceDispatcher.DebounceAsync(Invoke),
            debounceDispatcher.DebounceAsync(Invoke),
            debounceDispatcher.DebounceAsync(Invoke)
        };

        // Assert
        await Task.WhenAll(tasks);
        counter.Should().Be(1);
    }

    [Test]
    public async Task DebounceAsync_MultipleCallsOutsideInterval_ExecutesMultipleTimes()
    {
        // Arrange
        var debounceDispatcher = new DebounceDispatcher(100);
        var counter = 0;
        Task Invoke()
        {
            counter++;

            return Task.CompletedTask;
        }

        async Task CallDebounceAsyncAfterDelay(int delay)
        {
            await Task.Delay(delay).ConfigureAwait(false);
            await debounceDispatcher.DebounceAsync(Invoke);
        };

        // Act
        var tasks = new[]
        {
            debounceDispatcher.DebounceAsync(Invoke),
            CallDebounceAsyncAfterDelay(150),
            CallDebounceAsyncAfterDelay(300)
        };

        // Assert
        await Task.WhenAll(tasks);
        counter.Should().Be(3);
    }
}
