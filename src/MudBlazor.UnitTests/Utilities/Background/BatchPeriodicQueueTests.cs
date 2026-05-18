// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using MudBlazor.Utilities.Background.Batch;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Background;

#nullable enable
[TestFixture]
public class BatchPeriodicQueueTests
{
    [Test]
    [CancelAfter(5000)]
    public async Task PeriodicExecution_ShouldOccurWithExpectedItems()
    {
        // Define the expected items
        var expectedItems = new List<int> { 1, 2, 3 };

        // Arrange
        var stoppingTokenSource = new CancellationTokenSource();
        var batchCompletion = new TaskCompletionSource<IReadOnlyCollection<int>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var period = TimeSpan.FromSeconds(0.5);
        var timeProvider = new FakeTimeProvider();
        var mockHandler = new Mock<IBatchTimerHandler<int>>();
        using var batchPeriodicQueue = new BatchPeriodicQueue<int>(mockHandler.Object, period, timeProvider);

        mockHandler
            .Setup(h => h.OnBatchTimerElapsedAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .Returns((IReadOnlyCollection<int> items, CancellationToken _) =>
            {
                batchCompletion.TrySetResult(items);
                return Task.CompletedTask;
            });

        // Act
        await batchPeriodicQueue.StartAsync(stoppingTokenSource.Token);
        foreach (var expectedItem in expectedItems)
        {
            batchPeriodicQueue.QueueItem(expectedItem);
        }

        timeProvider.Advance(period);
        var processedItems = await batchCompletion.Task.WaitAsync(TestContext.CurrentContext.CancellationToken);

        // Assert
        processedItems.VerifyItemsMatch(expectedItems).Should().BeTrue();
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
    public async Task Dispose_ShouldNotOccurWithExpectedItems()
    {
        // Define the expected items
        var expectedItems = new List<int> { 1, 2, 3 };

        // Arrange
        var batchCompletion = new TaskCompletionSource<IReadOnlyCollection<int>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var period = TimeSpan.FromSeconds(0.5);
        var timeProvider = new FakeTimeProvider();
        var mockHandler = new Mock<IBatchTimerHandler<int>>();
        var batchPeriodicQueue = new BatchPeriodicQueue<int>(mockHandler.Object, period, timeProvider);

        mockHandler
            .Setup(h => h.OnBatchTimerElapsedAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .Returns((IReadOnlyCollection<int> items, CancellationToken _) =>
            {
                batchCompletion.TrySetResult(items);
                return Task.CompletedTask;
            });

        // Act
        await batchPeriodicQueue.StartAsync();
        foreach (var expectedItem in expectedItems)
        {
            batchPeriodicQueue.QueueItem(expectedItem);
        }

        batchPeriodicQueue.Dispose();
        timeProvider.Advance(period);
        await batchPeriodicQueue.ExecuteTask!;

        // Assert
        batchCompletion.Task.IsCompleted.Should().BeFalse();
        batchPeriodicQueue.Count.Should().Be(3);
        //NB! Use It.IsAny<CancellationToken>() instead of stoppingTokenSource.Token because it case of DisposeAsync the token will be default
        mockHandler.Verify(
            h => h.OnBatchTimerElapsedAsync(
                It.Is<IReadOnlyCollection<int>>(items => items.VerifyItemsMatch(expectedItems)),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
