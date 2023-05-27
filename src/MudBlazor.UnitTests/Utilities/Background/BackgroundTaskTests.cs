// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MudBlazor.UnitTests.Utilities.Background.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Background;

#nullable enable
[TestFixture]
public class BackgroundTaskTests
{
    [Test]
    public void StartReturnsCompletedTaskIfLongRunningTaskIsIncomplete()
    {
        var tcs = new TaskCompletionSource<object>();
        var task = new MyBackgroundTaskMock(tcs.Task);

        var startTask = task.StartAsync(CancellationToken.None);

        Assert.True(startTask.IsCompleted);
        Assert.False(tcs.Task.IsCompleted);

        // Complete the task
        tcs.TrySetResult(null!);
    }

    [Test]
    public void StartAsync_ReturnsCompletedTaskIfCancelled()
    {
        var tcs = new TaskCompletionSource<object>();
        tcs.TrySetCanceled();
        var task = new MyBackgroundTaskMock(tcs.Task);

        var stopTask = task.StartAsync(CancellationToken.None);

        Assert.True(stopTask.IsCompleted);
        Assert.AreSame(stopTask, task.ExecuteTask);
    }

    [Test]
    public void StartAsync_ReturnsLongRunningTaskIfFailed()
    {
        var tcs = new TaskCompletionSource<object>();
        tcs.TrySetException(new Exception("fail!"));
        var task = new MyBackgroundTaskMock(tcs.Task);

        var exception = Assert.ThrowsAsync<Exception>(() => task.StartAsync(CancellationToken.None));

        Assert.AreEqual("fail!", exception?.Message);
    }

    [Test]
    public async Task StopAsync_WithoutStartAsyncNoops()
    {
        var tcs = new TaskCompletionSource<object>();
        var task = new MyBackgroundTaskMock(tcs.Task);

        await task.StopAsync(CancellationToken.None);

        Assert.Null(task.ExecuteTask);
    }

    [Test]
    public async Task StopAsync_StopsBackgroundService()
    {
        var tcs = new TaskCompletionSource<object>();
        var task = new MyBackgroundTaskMock(tcs.Task);

        await task.StartAsync(CancellationToken.None);

        Assert.False(task.ExecuteTask?.IsCompleted);

        await task.StopAsync(CancellationToken.None);

        Assert.True(task.ExecuteTask?.IsCompleted);
    }

    [Test]
    public async Task StopAsync_StopsEvenIfTaskNeverEnds()
    {
        var task = new IgnoreCancellationTaskMock();

        await task.StartAsync(CancellationToken.None);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        await task.StopAsync(cts.Token);
    }

    [Test]
    public async Task StopAsync_ThrowsIfCancellationCallbackThrows()
    {
        var task = new ThrowOnCancellationTaskMock();

        await task.StartAsync(CancellationToken.None);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        Assert.ThrowsAsync<AggregateException>(() => task.StopAsync(cts.Token));
        Assert.AreEqual(2, task.TokenCalls);
    }

    [Test]
    public async Task StartAsyncThenDisposeTriggersCancelledToken()
    {
        var task = new WaitForCancelledTokenTaskMock();

        await task.StartAsync(CancellationToken.None);

        await task.DisposeAsync();
    }

    [Test]
    public async Task StartAsync_ThenCancelShouldCancelExecutingTask()
    {
        var tokenSource = new CancellationTokenSource();

        var task = new WaitForCancelledTokenTaskMock();

        await task.StartAsync(tokenSource.Token);

        tokenSource.Cancel();

        Assert.ThrowsAsync<TaskCanceledException>(() => task.ExecutingTask);
    }

    [Test]
    public async Task DisposeAsync_ShouldNotThrow()
    {
        var task = new WaitForCancelledTokenTaskMock();

        await task.DisposeAsync();
    }
}
