// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MudBlazor.Utilities.Background.Batch;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Background;

#nullable enable
[TestFixture]
public class BatchPeriodicQueueTests
{
    [Test]
    public async Task PeriodicExecution_ShouldOccurWithExpectedItems()
    {
        // Define the expected items
        var expectedItems = new List<int> { 1, 2, 3 };

        // Arrange
        var stoppingTokenSource = new CancellationTokenSource();
        var signalEvent = new ManualResetEventSlim(false);
        var period = TimeSpan.FromSeconds(0.5);
        var mockHandler = new Mock<IBatchTimerHandler<int>>();
        using var batchPeriodicQueue = new BatchPeriodicQueue<int>(mockHandler.Object, period);

        // Configure the periodic timer to execute immediately
        mockHandler
            .Setup(h => h.OnBatchTimerElapsedAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(signalEvent.Set);

        // Act
        await batchPeriodicQueue.StartAsync(stoppingTokenSource.Token);
        foreach (var expectedItem in expectedItems)
        {
            batchPeriodicQueue.QueueItem(expectedItem);
        }

        // Wait for the timer to be signaled, consider test failed if we didn't receive signal in period + 2 minutes
        var signalEventWaitTime = period.Add(TimeSpan.FromMinutes(2));
        var eventSignaled = signalEvent.Wait(signalEventWaitTime);

        // Assert
        eventSignaled.Should().BeTrue();
        batchPeriodicQueue.Count.Should().Be(0);
        //NB! Use It.IsAny<CancellationToken>() instead of stoppingTokenSource.Token because it creates a linked token via CancellationTokenSource.CreateLinkedTokenSource, therefore the reference won't match
        mockHandler.Verify(
            h => h.OnBatchTimerElapsedAsync(
                It.Is<IReadOnlyCollection<int>>(items => items.VerifyItemsMatch(expectedItems)),
                It.IsAny<CancellationToken>()),
            Times.AtLeastOnce,
            "The periodic handler method was not called.");
    }

    [Test]
    public void Dispose_ShouldNotOccurWithExpectedItems()
    {
        // Define the expected items
        var expectedItems = new List<int> { 1, 2, 3 };

        // Arrange
        var signalEvent = new ManualResetEventSlim(false);
        var period = TimeSpan.FromSeconds(0.5);
        var mockHandler = new Mock<IBatchTimerHandler<int>>();
        var batchPeriodicQueue = new BatchPeriodicQueue<int>(mockHandler.Object, period);

        // Configure the periodic timer to execute immediately
        mockHandler
            .Setup(h => h.OnBatchTimerElapsedAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(signalEvent.Set);

        // Act
        foreach (var expectedItem in expectedItems)
        {
            batchPeriodicQueue.QueueItem(expectedItem);
        }

        batchPeriodicQueue.Dispose();

        // Wait for the event to be signaled, let's not add time as the even won't be ever received
        var eventSignaled = signalEvent.Wait(period);

        // Assert
        eventSignaled.Should().BeFalse();
        batchPeriodicQueue.Count.Should().Be(3);
        //NB! Use It.IsAny<CancellationToken>() instead of stoppingTokenSource.Token because it case of DisposeAsync the token will be default
        mockHandler.Verify(
            h => h.OnBatchTimerElapsedAsync(
                It.Is<IReadOnlyCollection<int>>(items => items.VerifyItemsMatch(expectedItems)),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
