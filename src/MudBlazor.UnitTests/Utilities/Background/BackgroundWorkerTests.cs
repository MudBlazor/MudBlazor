// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.UnitTests.Utilities.Background.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Background;

#nullable enable
[TestFixture]
public class BackgroundWorkerTests
{
    [Test]
    public void StartReturnsCompletedTaskIfLongRunningTaskIsIncomplete()
    {
        var tcs = new TaskCompletionSource<object>();
        var worker = new MyBackgroundWorkerMock(tcs.Task);

        var startTask = worker.StartAsync(CancellationToken.None);

        startTask.IsCompleted.Should().BeTrue();
        tcs.Task.IsCompleted.Should().BeFalse();

        // Complete the task
        tcs.TrySetResult(null!);
    }

    [Test]
    public void StartAsync_ReturnsCompletedTaskIfCancelled()
    {
        var tcs = new TaskCompletionSource<object>();
        tcs.TrySetCanceled();
        var worker = new MyBackgroundWorkerMock(tcs.Task);

        var stopTask = worker.StartAsync(CancellationToken.None);

        stopTask.IsCompleted.Should().BeTrue();
        worker.ExecuteTask.Should().BeSameAs(stopTask);
    }

    [Test]
    public void StartAsync_ReturnsLongRunningTaskIfFailed()
    {
        var tcs = new TaskCompletionSource<object>();
        tcs.TrySetException(new Exception("fail!"));
        var worker = new MyBackgroundWorkerMock(tcs.Task);

        var exception = Assert.ThrowsAsync<Exception>(() => worker.StartAsync(CancellationToken.None));

        exception?.Message.Should().Be("fail!");
    }

    [Test]
    public async Task StopAsync_WithoutStartAsyncNoops()
    {
        var tcs = new TaskCompletionSource<object>();
        var worker = new MyBackgroundWorkerMock(tcs.Task);

        await worker.StopAsync(CancellationToken.None);

        worker.ExecuteTask.Should().BeNull();
    }

    [Test]
    public async Task StopAsync_StopsBackgroundService()
    {
        var tcs = new TaskCompletionSource<object>();
        var worker = new MyBackgroundWorkerMock(tcs.Task);

        await worker.StartAsync(CancellationToken.None);

        worker.ExecuteTask?.IsCompleted.Should().BeFalse();

        await worker.StopAsync(CancellationToken.None);

        worker.ExecuteTask?.IsCompleted.Should().BeTrue();
    }

    [Test]
    public async Task StopAsync_StopsEvenIfTaskNeverEnds()
    {
        var worker = new IgnoreCancellationWorkerMock();

        await worker.StartAsync(CancellationToken.None);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        await worker.StopAsync(cts.Token);
    }

    [Test]
    public async Task StopAsync_ThrowsIfCancellationCallbackThrows()
    {
        var worker = new ThrowOnCancellationWorkerMock();

        await worker.StartAsync(CancellationToken.None);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        Assert.ThrowsAsync<AggregateException>(() => worker.StopAsync(cts.Token));
        worker.TokenCalls.Should().Be(2);
    }

    [Test]
    public async Task StartAsyncThenDisposeTriggersCancelledToken()
    {
        var worker = new WaitForCancelledTokenWorkerMock();

        await worker.StartAsync(CancellationToken.None);

        worker.Dispose();
    }

    [Test]
    public async Task StartAsync_ThenCancelShouldCancelExecutingTask()
    {
        var tokenSource = new CancellationTokenSource();

        var worker = new WaitForCancelledTokenWorkerMock();

        await worker.StartAsync(tokenSource.Token);

        await tokenSource.CancelAsync();

        Assert.ThrowsAsync<TaskCanceledException>(() => worker.ExecutingTask);
    }

    [Test]
    public void Dispose_ShouldNotThrow()
    {
        var worker = new WaitForCancelledTokenWorkerMock();

        worker.Dispose();
    }
}
