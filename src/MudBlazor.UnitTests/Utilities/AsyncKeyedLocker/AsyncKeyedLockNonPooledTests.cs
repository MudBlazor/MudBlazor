// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.Utilities.AsyncKeyedLocker;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.AsyncKeyedLocker;

#nullable enable
[TestFixture]
public class AsyncKeyedLockNonPooledTests
{
    private readonly AsyncKeyedLocker<string> _keyedLocker = new();
    private readonly TimeSpan _defaultSynchronousWaitDuration = TimeSpan.FromMilliseconds(10);

    public class Async : AsyncKeyedLockNonPooledTests
    {
        [Test]
        [TestCase(100, 100, 10, 100)]
        [TestCase(100, 10, 2, 10)]
        [TestCase(100, 50, 5, 50)]
        [TestCase(100, 1, 1, 1)]
        public async Task ShouldApplyParallelismCorrectly(int numberOfThreads, int numberOfKeys, int minParallelism, int maxParallelism)
        {
            // Arrange
            var runningTasksIndex = new ConcurrentDictionary<int, int>();
            var parallelismLock = new object();
            var currentParallelism = 0;
            var peakParallelism = 0;

            var threads = Enumerable.Range(0, numberOfThreads)
                .Select(i =>
                    Task.Run(async () => await OccupyTheLockALittleBit(i % numberOfKeys)))
                .ToList();

            // Act + Assert
            await Task.WhenAll(threads);

            peakParallelism.Should().BeLessOrEqualTo(maxParallelism);
            peakParallelism.Should().BeGreaterOrEqualTo(minParallelism);

            return;

            async Task OccupyTheLockALittleBit(int key)
            {
                using (await _keyedLocker.LockAsync(key.ToString()))
                {
                    var incrementedCurrentParallelism = Interlocked.Increment(ref currentParallelism);

                    lock (parallelismLock)
                    {
                        peakParallelism = Math.Max(incrementedCurrentParallelism, peakParallelism);
                    }

                    var currentTaskId = Task.CurrentId ?? -1;

                    if (!runningTasksIndex.TryAdd(key, currentTaskId))
                    {
                        throw new InvalidOperationException(
                            $"Task #{currentTaskId} acquired a lock using key ${key} but another thread is also still running using this key!");
                    }

                    const int Delay = 10;

                    await Task.Delay(Delay);

                    if (!runningTasksIndex.TryRemove(key, out var value))
                    {
                        throw new InvalidOperationException($"Task #{currentTaskId} has just finished " +
                                                            $"but the running tasks index does not contain an entry for key {key}");
                    }

                    if (value != currentTaskId)
                    {
                        var ex = new InvalidOperationException($"Task #{currentTaskId} has just finished " +
                                                               $"but the running threads index has linked task #{value} to key {key}!");

                        throw ex;
                    }

                    Interlocked.Decrement(ref currentParallelism);
                }
            }
        }
    }

    public class Sync : AsyncKeyedLockNonPooledTests
    {
        [Test]
        [TestCase(100, 100, 10, 100)]
        [TestCase(100, 10, 2, 10)]
        [TestCase(100, 50, 5, 50)]
        [TestCase(100, 1, 1, 1)]
        public void ShouldApplyParallelismCorrectly(int numberOfThreads, int numberOfKeys, int minParallelism, int maxParallelism)
        {
            // Arrange
            var currentParallelism = 0;
            var peakParallelism = 0;
            var parallelismLock = new object();
            var runningThreadsIndex = new ConcurrentDictionary<int, int>();

            var threads = Enumerable.Range(0, numberOfThreads)
                .Select(i => new Thread(() => OccupyTheLockALittleBit(i % numberOfKeys)))
                .ToList();

            // Act
            foreach (var thread in threads) thread.Start();

            foreach (var thread in threads) thread.Join();

            peakParallelism.Should().BeGreaterThanOrEqualTo(minParallelism);
            peakParallelism.Should().BeLessThanOrEqualTo(maxParallelism);

            return;

            void OccupyTheLockALittleBit(int key)
            {
                using (_keyedLocker.Lock(key.ToString()))
                {
                    var incrementedCurrentParallelism = Interlocked.Increment(ref currentParallelism);

                    lock (parallelismLock)
                    {
                        peakParallelism = Math.Max(incrementedCurrentParallelism, peakParallelism);
                    }

                    var currentThreadId = Thread.CurrentThread.ManagedThreadId;

                    if (!runningThreadsIndex.TryAdd(key, currentThreadId))
                    {
                        throw new InvalidOperationException(
                            $"Thread #{currentThreadId} acquired a lock using key ${key} but another thread is also still running using this key!");
                    }

                    const int Delay = 10;

                    Thread.Sleep(Delay);

                    if (!runningThreadsIndex.TryRemove(key, out var value))
                    {
                        throw new InvalidOperationException($"Thread #{currentThreadId} has just finished " +
                                                            $"but the running threads index does not contain an entry for key {key}");
                    }

                    if (value != currentThreadId)
                    {
                        var ex = new InvalidOperationException($"Thread #{currentThreadId} has just finished " +
                                                               $"but the running threads index has linked thread #{value} to key {key}!");

                        throw ex;
                    }

                    Interlocked.Decrement(ref currentParallelism);
                }
            }
        }
    }

    [Test]
    public async Task ThreeDifferentLocksShouldWork()
    {
        // Arrange
        var keyedSemaphores = new AsyncKeyedLocker<int>();

        // Act
        using var _1 = await keyedSemaphores.LockAsync(1);
        using var _2 = await keyedSemaphores.LockAsync(2);
        using var _3 = await keyedSemaphores.LockAsync(3);

        // Assert
        _1.Should().NotBeNull();
        _2.Should().NotBeNull();
        _3.Should().NotBeNull();
    }

    [Test]
    public async Task ThreeIdenticalLocksShouldWork()
    {
        // Arrange
        var keyedSemaphores = new AsyncKeyedLocker<int>();

        // Act
        var t1 = Task.Run(async () =>
        {
            using var _ = await keyedSemaphores.LockAsync(1);
        });
        var t2 = Task.Run(async () =>
        {
            using var _ = await keyedSemaphores.LockAsync(1);
        });
        var t3 = Task.Run(async () =>
        {
            using var _ = await keyedSemaphores.LockAsync(1);
        });
        await t1;
        await t2;
        await t3;

        // Assert
        t1.Should().NotBeNull();
        t2.Should().NotBeNull();
        t3.Should().NotBeNull();
    }

    [Test]
    public async Task ShouldRunThreadsWithDistinctKeysInParallel()
    {
        // Arrange
        var currentParallelism = 0;
        var maxParallelism = 0;
        var parallelismLock = new object();
        var keyedSemaphores = new AsyncKeyedLocker<int>();

        // 100 threads, 100 keys
        var threads = Enumerable.Range(0, 100)
            .Select(i => Task.Run(async () => await OccupyTheLockALittleBit(i)))
            .ToList();

        // Act
        await Task.WhenAll(threads);

        maxParallelism.Should().BeGreaterThan(10);
        foreach (var key in Enumerable.Range(0, 100))
        {
            keyedSemaphores.IsInUse(key).Should().BeFalse();
        }

        async Task OccupyTheLockALittleBit(int key)
        {
            using (await keyedSemaphores.LockAsync(key))
            {
                var incrementedCurrentParallelism = Interlocked.Increment(ref currentParallelism);

                lock (parallelismLock)
                {
                    maxParallelism = Math.Max(incrementedCurrentParallelism, maxParallelism);
                }

                const int Delay = 250;

                await Task.Delay(TimeSpan.FromMilliseconds(Delay));

                Interlocked.Decrement(ref currentParallelism);
            }
        }
    }

    [Test]
    public async Task ShouldRunThreadsWithSameKeysLinearly()
    {
        // Arrange
        var runningTasksIndex = new ConcurrentDictionary<int, int>();
        var parallelismLock = new object();
        var currentParallelism = 0;
        var maxParallelism = 0;
        var keyedSemaphores = new AsyncKeyedLocker<int>();

        // 100 threads, 10 keys
        var threads = Enumerable.Range(0, 100)
            .Select(i => Task.Run(async () => await OccupyTheLockALittleBit(i % 10)))
            .ToList();

        // Act + Assert
        await Task.WhenAll(threads);

        maxParallelism.Should().BeLessOrEqualTo(10);
        foreach (var key in Enumerable.Range(0, 100))
        {
            keyedSemaphores.IsInUse(key % 10).Should().BeFalse();
        }

        async Task OccupyTheLockALittleBit(int key)
        {
            using (await keyedSemaphores.LockAsync(key))
            {
                var incrementedCurrentParallelism = Interlocked.Increment(ref currentParallelism);

                lock (parallelismLock)
                {
                    maxParallelism = Math.Max(incrementedCurrentParallelism, maxParallelism);
                }

                var currentTaskId = Task.CurrentId ?? -1;
                if (runningTasksIndex.TryGetValue(key, out var otherThread))
                    throw new Exception($"Thread #{currentTaskId} acquired a lock using key ${key} " +
                                        $"but another thread #{otherThread} is also still running using this key!");

                runningTasksIndex[key] = currentTaskId;

                const int Delay = 10;

                await Task.Delay(TimeSpan.FromMilliseconds(Delay));

                if (!runningTasksIndex.TryRemove(key, out var value))
                {
                    var ex = new Exception($"Thread #{currentTaskId} has finished " +
                                           "but when trying to cleanup the running threads index, the value is already gone");

                    throw ex;
                }

                if (value != currentTaskId)
                {
                    var ex = new Exception(
                        $"Thread #{currentTaskId} has finished and has removed itself from the running threads index," +
                        $" but that index contained an incorrect value: #{value}!");

                    throw ex;
                }

                Interlocked.Decrement(ref currentParallelism);
            }
        }
    }

    [Test]
    public async Task ShouldNeverCreateTwoSemaphoresForTheSameKey()
    {
        // Arrange
        var runningTasksIndex = new ConcurrentDictionary<int, int>();
        var parallelismLock = new object();
        var currentParallelism = 0;
        var maxParallelism = 0;
        var random = new Random();
        var keyedSemaphores = new AsyncKeyedLocker<int>();

        // Many threads, 1 key
        var threads = Enumerable.Range(0, 100)
            .Select(_ => Task.Run(async () => await OccupyTheLockALittleBit(1)))
            .ToList();

        // Act + Assert
        await Task.WhenAll(threads);

        maxParallelism.Should().Be(1);
        keyedSemaphores.IsInUse(1).Should().BeFalse();


        async Task OccupyTheLockALittleBit(int key)
        {
            var currentTaskId = Task.CurrentId ?? -1;
            var delay = random.Next(500);

            await Task.Delay(delay);

            using (await keyedSemaphores.LockAsync(key))
            {
                var incrementedCurrentParallelism = Interlocked.Increment(ref currentParallelism);

                lock (parallelismLock)
                {
                    maxParallelism = Math.Max(incrementedCurrentParallelism, maxParallelism);
                }

                if (runningTasksIndex.TryGetValue(key, out var otherThread))
                    throw new Exception($"Task [{currentTaskId,3}] has a lock for key ${key} " +
                                        $"but another task [{otherThread,3}] also has an active lock for this key!");

                runningTasksIndex[key] = currentTaskId;

                if (!runningTasksIndex.TryRemove(key, out var value))
                {
                    var ex = new Exception($"Task [{currentTaskId,3}] has finished " +
                                           "but when trying to cleanup the running tasks index, the value is already gone");

                    throw ex;
                }

                if (value != currentTaskId)
                {
                    var ex = new Exception(
                        $"Task [{currentTaskId,3}] has finished and has removed itself from the running tasks index," +
                        $" but that index contained a task ID of another task: [{value}]!");

                    throw ex;
                }

                Interlocked.Decrement(ref currentParallelism);
            }
        }
    }

    [Test]
    public async Task ShouldRunThreadsWithDistinctStringKeysInParallel()
    {
        // Arrange
        var currentParallelism = 0;
        var maxParallelism = 0;
        var parallelismLock = new object();
        var keyedSemaphores = new AsyncKeyedLocker<int>();

        // 100 threads, 100 keys
        var threads = Enumerable.Range(0, 100)
            .Select(i => Task.Run(async () => await OccupyTheLockALittleBit(i)))
            .ToList();

        // Act
        await Task.WhenAll(threads);

        maxParallelism.Should().BeGreaterThan(10);
        foreach (var key in Enumerable.Range(0, 100))
        {
            keyedSemaphores.IsInUse(key).Should().BeFalse();
        }

        async Task OccupyTheLockALittleBit(int key)
        {
            using (await keyedSemaphores.LockAsync(key))
            {
                var incrementedCurrentParallelism = Interlocked.Increment(ref currentParallelism);

                lock (parallelismLock)
                {
                    maxParallelism = Math.Max(incrementedCurrentParallelism, maxParallelism);
                }

                const int Delay = 250;

                await Task.Delay(TimeSpan.FromMilliseconds(Delay));

                Interlocked.Decrement(ref currentParallelism);
            }
        }
    }

    [Test]
    public async Task IsInUseShouldReturnTrueWhenLockedAndFalseWhenNotLocked()
    {
        // Arrange
        var keyedSemaphores = new AsyncKeyedLocker<int>();

        // 10 threads, 10 keys
        var threads = Enumerable.Range(0, 10)
            .Select(i => Task.Run(async () => await OccupyTheLockALittleBit(i)))
            .ToList();

        // Act
        await Task.WhenAll(threads);
        foreach (var key in Enumerable.Range(0, 10))
        {
            keyedSemaphores.IsInUse(key).Should().BeFalse();
        }

        async Task OccupyTheLockALittleBit(int key)
        {
            keyedSemaphores.IsInUse(key).Should().BeFalse();

            using (await keyedSemaphores.LockAsync(key))
            {
                const int Delay = 250;

                await Task.Delay(TimeSpan.FromMilliseconds(Delay));

                keyedSemaphores.IsInUse(key).Should().BeTrue();
            }

            keyedSemaphores.IsInUse(key).Should().BeFalse();
        }
    }

    [Test]
    public void Lock_WhenCancelled_ShouldReleaseKeyedSemaphoreAndThrowOperationCanceledException()
    {
        // Arrange
        var dictionary = new AsyncKeyedLocker<string>();
        var cancelledCancellationToken = new CancellationToken(true);

        // Act
        var action = () =>
        {
            using var _ = dictionary.Lock("test", cancelledCancellationToken);
        };
        action.Should().Throw<OperationCanceledException>();

        // Assert
        dictionary.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void Lock_WhenNotCancelled_ShouldReturnDisposable()
    {
        // Arrange
        var dictionary = new AsyncKeyedLocker<string>();
        var cancellationToken = default(CancellationToken);

        // Act
        var releaser = dictionary.Lock("test", cancellationToken);

        // Assert
        dictionary.IsInUse("test").Should().BeTrue();
        releaser.Dispose();
        dictionary.IsInUse("test").Should().BeFalse();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void TryLock_WhenCancelled_ShouldReleaseKeyedSemaphoreAndThrowOperationCanceledExceptionAndNotInvokeCallback(bool useShortTimeout)
    {
        // Arrange
        var isLockAcquired = false;
        var isCallbackInvoked = false;

        void Callback()
        {
            isCallbackInvoked = true;
        }

        var dictionary = new AsyncKeyedLocker<string>();
        var cancelledCancellationToken = new CancellationToken(true);
        var timeout = useShortTimeout
            ? _defaultSynchronousWaitDuration.Subtract(TimeSpan.FromMilliseconds(1))
            : _defaultSynchronousWaitDuration.Add(TimeSpan.FromMilliseconds(1));

        // Act
        Action action = () =>
            isLockAcquired = dictionary.TryLock("test", Callback, timeout, cancelledCancellationToken);
        action.Should().Throw<OperationCanceledException>();

        action = () =>
            isLockAcquired = dictionary.TryLock("test", Callback, Convert.ToInt32(timeout.TotalMilliseconds), cancelledCancellationToken);
        action.Should().Throw<OperationCanceledException>();

        // Assert
        dictionary.IsInUse("test").Should().BeFalse();
        isLockAcquired.Should().BeFalse();
        isCallbackInvoked.Should().BeFalse();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void TryLock_WhenNotCancelled_ShouldInvokeCallbackAndReturnDisposable(bool useShortTimeout)
    {
        // Arrange
        var isCallbackInvoked = false;

        void Callback()
        {
            isCallbackInvoked = true;
        }

        var dictionary = new AsyncKeyedLocker<string>();
        var cancellationToken = default(CancellationToken);
        var timeout = useShortTimeout
            ? _defaultSynchronousWaitDuration.Subtract(TimeSpan.FromMilliseconds(1))
            : _defaultSynchronousWaitDuration.Add(TimeSpan.FromMilliseconds(1));

        // Act
        var isLockAcquired = dictionary.TryLock("test", Callback, timeout, cancellationToken);

        // Assert
        dictionary.IsInUse("test").Should().BeFalse();
        isLockAcquired.Should().BeTrue();
        isCallbackInvoked.Should().BeTrue();

        isLockAcquired = dictionary.TryLock("test", Callback, Convert.ToInt32(timeout.TotalMilliseconds), cancellationToken);

        // Assert
        dictionary.IsInUse("test").Should().BeFalse();
        isLockAcquired.Should().BeTrue();
        isCallbackInvoked.Should().BeTrue();

    }

    [Test]
    public async Task LockAsync_WhenCancelled_ShouldReleaseKeyedSemaphoreAndThrowOperationCanceledException()
    {
        // Arrange
        var dictionary = new AsyncKeyedLocker<string>();
        var cancelledCancellationToken = new CancellationToken(true);

        // Act
        var action = async () =>
        {
            using var _ = await dictionary.LockAsync("test", cancelledCancellationToken);
        };
        await action.Should().ThrowAsync<OperationCanceledException>();

        // Assert
        dictionary.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public async Task LockAsync_WhenNotCancelled_ShouldReturnDisposable()
    {
        // Arrange
        var dictionary = new AsyncKeyedLocker<string>();
        var cancellationToken = default(CancellationToken);

        // Act
        var releaser = await dictionary.LockAsync("test", cancellationToken);

        // Assert
        dictionary.IsInUse("test").Should().BeTrue();
        releaser.Dispose();
        dictionary.IsInUse("test").Should().BeFalse();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task TryLockAsync_WithSynchronousCallback_WhenCancelled_ShouldReleaseKeyedSemaphoreAndThrowOperationCanceledExceptionAndNotInvokeCallback(bool useShortTimeout)
    {
        // Arrange
        var isLockAcquired = false;
        var isCallbackInvoked = false;

        void Callback()
        {
            isCallbackInvoked = true;
        }

        var dictionary = new AsyncKeyedLocker<string>();
        var cancelledCancellationToken = new CancellationToken(true);
        var timeout = useShortTimeout
            ? _defaultSynchronousWaitDuration.Subtract(TimeSpan.FromMilliseconds(1))
            : _defaultSynchronousWaitDuration.Add(TimeSpan.FromMilliseconds(1));

        // Act
        Func<Task> action = async () =>
            isLockAcquired = await dictionary.TryLockAsync("test", Callback, timeout, cancelledCancellationToken);
        await action.Should().ThrowAsync<OperationCanceledException>();

        action = async () =>
            isLockAcquired = await dictionary.TryLockAsync("test", Callback, Convert.ToInt32(timeout.TotalMilliseconds), cancelledCancellationToken);
        await action.Should().ThrowAsync<OperationCanceledException>();

        // Assert
        dictionary.IsInUse("test").Should().BeFalse();
        isLockAcquired.Should().BeFalse();
        isCallbackInvoked.Should().BeFalse();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task TryLockAsync_WithSynchronousCallback_WhenNotCancelled_ShouldInvokeCallbackAndReturnTrue(
        bool useShortTimeout)
    {
        // Arrange
        var isCallbackInvoked = false;

        void Callback()
        {
            isCallbackInvoked = true;
        }

        var dictionary = new AsyncKeyedLocker<string>();
        var cancellationToken = default(CancellationToken);
        var timeout = useShortTimeout
            ? _defaultSynchronousWaitDuration.Subtract(TimeSpan.FromMilliseconds(1))
            : _defaultSynchronousWaitDuration.Add(TimeSpan.FromMilliseconds(1));

        // Act
        var isLockAcquired = await dictionary.TryLockAsync("test", Callback, timeout, cancellationToken);

        // Assert
        dictionary.IsInUse("test").Should().BeFalse();
        isLockAcquired.Should().BeTrue();
        isCallbackInvoked.Should().BeTrue();

        isLockAcquired = await dictionary.TryLockAsync("test", Callback, Convert.ToInt32(timeout.TotalMilliseconds), cancellationToken);

        // Assert
        dictionary.IsInUse("test").Should().BeFalse();
        isLockAcquired.Should().BeTrue();
        isCallbackInvoked.Should().BeTrue();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task TryLockAsync_WithAsynchronousCallback_WhenCancelled_ShouldReleaseKeyedSemaphoreAndThrowOperationCanceledExceptionAndNotInvokeCallback(bool useShortTimeout)
    {
        // Arrange
        var isLockAcquired = false;
        var isCallbackInvoked = false;

        async Task Callback()
        {
            await Task.Delay(1);
            isCallbackInvoked = true;
        }

        var dictionary = new AsyncKeyedLocker<string>();
        var cancelledCancellationToken = new CancellationToken(true);
        var timeout = useShortTimeout
            ? _defaultSynchronousWaitDuration.Subtract(TimeSpan.FromMilliseconds(1))
            : _defaultSynchronousWaitDuration.Add(TimeSpan.FromMilliseconds(1));

        // Act
        Func<Task> action = async () =>
        {
            isLockAcquired =
                await dictionary.TryLockAsync("test", Callback, timeout, cancelledCancellationToken);
        };
        await action.Should().ThrowAsync<OperationCanceledException>();

        action = async () =>
        {
            isLockAcquired =
                await dictionary.TryLockAsync("test", Callback, Convert.ToInt32(timeout.TotalMilliseconds), cancelledCancellationToken);
        };
        await action.Should().ThrowAsync<OperationCanceledException>();

        // Assert
        dictionary.IsInUse("test").Should().BeFalse();
        isLockAcquired.Should().BeFalse();
        isCallbackInvoked.Should().BeFalse();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task TryLockAsync_WithAsynchronousCallback_WhenNotCancelled_ShouldInvokeCallbackAndReturnTrue(
        bool useShortTimeout)
    {
        // Arrange
        var isCallbackInvoked = false;

        async Task Callback()
        {
            await Task.Delay(1);
            isCallbackInvoked = true;
        }

        var dictionary = new AsyncKeyedLocker<string>();
        var cancellationToken = default(CancellationToken);
        var timeout = useShortTimeout
            ? _defaultSynchronousWaitDuration.Subtract(TimeSpan.FromMilliseconds(1))
            : _defaultSynchronousWaitDuration.Add(TimeSpan.FromMilliseconds(1));

        // Act
        var isLockAcquired = await dictionary.TryLockAsync("test", Callback, timeout, cancellationToken);

        // Assert
        dictionary.IsInUse("test").Should().BeFalse();
        isLockAcquired.Should().BeTrue();
        isCallbackInvoked.Should().BeTrue();

        isLockAcquired = await dictionary.TryLockAsync("test", Callback, Convert.ToInt32(timeout.TotalMilliseconds), cancellationToken);

        // Assert
        dictionary.IsInUse("test").Should().BeFalse();
        isLockAcquired.Should().BeTrue();
        isCallbackInvoked.Should().BeTrue();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void TryLock_WhenTimedOut_ShouldNotInvokeCallbackAndReturnFalse(bool useShortTimeout)
    {
        // Arrange
        var dictionary = new AsyncKeyedLocker<string>();
        var key = "test";
        using var _ = dictionary.Lock(key);
        var isCallbackInvoked = false;

        void Callback()
        {
            isCallbackInvoked = true;
        }

        var timeout = useShortTimeout
            ? _defaultSynchronousWaitDuration.Subtract(TimeSpan.FromMilliseconds(1))
            : _defaultSynchronousWaitDuration.Add(TimeSpan.FromMilliseconds(1));

        // Act
        var isLockAcquired = dictionary.TryLock(key, Callback, timeout);

        // Assert
        isLockAcquired.Should().BeFalse();

        isLockAcquired = dictionary.TryLock(key, Callback, Convert.ToInt32(timeout.TotalMilliseconds));

        isLockAcquired.Should().BeFalse();

        isCallbackInvoked.Should().BeFalse();
        dictionary.IsInUse(key).Should().BeTrue();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void TryLock_WhenNotTimedOut_ShouldInvokeCallbackAndReturnTrue(bool useShortTimeout)
    {
        // Arrange
        var dictionary = new AsyncKeyedLocker<string>();
        var key = "test";
        var isCallbackInvoked = false;
        var timeout = useShortTimeout
            ? _defaultSynchronousWaitDuration.Subtract(TimeSpan.FromMilliseconds(1))
            : _defaultSynchronousWaitDuration.Add(TimeSpan.FromMilliseconds(1));

        void Callback()
        {
            isCallbackInvoked = true;
        }

        // Act
        var isLockAcquired = dictionary.TryLock(key, Callback, timeout);

        // Assert
        isLockAcquired.Should().BeTrue();
        isCallbackInvoked.Should().BeTrue();

        isLockAcquired = dictionary.TryLock(key, Callback, Convert.ToInt32(timeout.TotalMilliseconds));

        // Assert
        isLockAcquired.Should().BeTrue();
        isCallbackInvoked.Should().BeTrue();

        dictionary.IsInUse(key).Should().BeFalse();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task TryLockAsync_WhenTimedOut_ShouldNotInvokeCallbackAndReturnFalse(bool useShortTimeout)
    {
        // Arrange
        var dictionary = new AsyncKeyedLocker<string>();
        var key = "test";
        using var _ = await dictionary.LockAsync(key);
        var isCallbackInvoked = false;
        var timeout = useShortTimeout
            ? _defaultSynchronousWaitDuration.Subtract(TimeSpan.FromMilliseconds(1))
            : _defaultSynchronousWaitDuration.Add(TimeSpan.FromMilliseconds(1));

        void Callback()
        {
            isCallbackInvoked = true;
        }

        // Act
        var isLockAcquired = await dictionary.TryLockAsync(key, Callback, timeout);

        // Assert
        isLockAcquired.Should().BeFalse();
        isCallbackInvoked.Should().BeFalse();
        dictionary.IsInUse(key).Should().BeTrue();

        isLockAcquired = await dictionary.TryLockAsync(key, Callback, Convert.ToInt32(timeout.TotalMilliseconds));

        // Assert
        isLockAcquired.Should().BeFalse();
        isCallbackInvoked.Should().BeFalse();
        dictionary.IsInUse(key).Should().BeTrue();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task TryLockAsync_WhenNotTimedOut_ShouldNotInvokeCallbackAndReturnFalse(bool useShortTimeout)
    {
        // Arrange
        var dictionary = new AsyncKeyedLocker<string>();
        var key = "test";
        var isCallbackInvoked = false;
        var timeout = useShortTimeout
            ? _defaultSynchronousWaitDuration.Subtract(TimeSpan.FromMilliseconds(1))
            : _defaultSynchronousWaitDuration.Add(TimeSpan.FromMilliseconds(1));

        void Callback()
        {
            isCallbackInvoked = true;
        }

        // Act
        var isLockAcquired = await dictionary.TryLockAsync(key, Callback, timeout);

        // Assert
        isLockAcquired.Should().BeTrue();
        isCallbackInvoked.Should().BeTrue();
        dictionary.IsInUse(key).Should().BeFalse();

        isLockAcquired = await dictionary.TryLockAsync(key, Callback, Convert.ToInt32(timeout.TotalMilliseconds));

        // Assert
        isLockAcquired.Should().BeTrue();
        isCallbackInvoked.Should().BeTrue();
        dictionary.IsInUse(key).Should().BeFalse();
    }
}
