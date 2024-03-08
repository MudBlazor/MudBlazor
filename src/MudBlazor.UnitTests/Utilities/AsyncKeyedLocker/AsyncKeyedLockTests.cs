// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.Utilities.AsyncKeyedLocker;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.AsyncKeyedLocker;

#nullable enable
[TestFixture]
public class AsyncKeyedLockTests
{
    [Test]
    public void IsInUseRaceConditionCoverage()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>(o => o.PoolSize = 1);
        var releaser = asyncKeyedLocker.Dictionary.Pool?.GetObject("test");
        if (releaser is not null)
        {
            asyncKeyedLocker.Dictionary.Pool?.PutObject(releaser);
            asyncKeyedLocker.Lock("test");
            releaser.IsNotInUse = true; // internal
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void IsInUseKeyChangeRaceConditionCoverage()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>(o => o.PoolSize = 1);
        var releaser = asyncKeyedLocker.Dictionary.Pool?.GetObject("test");
        if (releaser is not null)
        {
            asyncKeyedLocker.Dictionary.Pool?.PutObject(releaser);
            asyncKeyedLocker.Lock("test");
            releaser.Key = "test2"; // internal
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void TryIncrementNoPoolingCoverage()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>(o => o.PoolSize = 0);
        var releaser = (AsyncKeyedLockReleaser<string>)asyncKeyedLocker.Lock("test");
        releaser.IsNotInUse = true;
        var timer = new System.Timers.Timer(1000);
        timer.Elapsed += (_, _) => { releaser.Dispose(); };
        timer.Start();
        asyncKeyedLocker.Lock("test");
    }

    [Test]
    public void TestMaxCountLessThan1()
    {
        var action = () =>
        {
            _ = new AsyncKeyedLocker<string>(o => o.MaxCount = 0);
        };

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void TestMaxCount1ShouldNotThrow()
    {
        var action = () =>
        {
            _ = new AsyncKeyedLocker<string>(o =>
            {
                o.MaxCount = 1;
                o.PoolSize = 1;
            });
        };

        action.Should().NotThrow();
    }

    [Test]
    public void TestComparerShouldBePossible()
    {
        var action = () =>
        {
            _ = new AsyncKeyedLocker<string>(EqualityComparer<string>.Default);
        };

        action.Should().NotThrow();
    }

    [Test]
    public void TestComparerAndMaxCount1ShouldBePossible()
    {
        var action = () =>
        {
            _ = new AsyncKeyedLocker<string>(o =>
            {
                o.MaxCount = 1;
                o.PoolSize = 1;
            }, EqualityComparer<string>.Default);
        };

        action.Should().NotThrow();
    }

    [Test]
    public void TestComparerAndMaxCount0ShouldNotBePossible()
    {
        var action = () =>
        {
            _ = new AsyncKeyedLocker<string>(o => o.MaxCount = 0, EqualityComparer<string>.Default);
        };

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void TestConcurrencyLevelAndCapacityShouldBePossible()
    {
        var action = () =>
        {
            _ = new AsyncKeyedLocker<string>(Environment.ProcessorCount, 100);
        };

        action.Should().NotThrow();
    }

    [Test]
    public void TestMaxCount0WithConcurrencyLevelAndCapacityShouldNotBePossible()
    {
        var action = () =>
        {
            _ = new AsyncKeyedLocker<string>(o => o.MaxCount = 0, Environment.ProcessorCount, 100);
        };

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void TestConcurrencyLevelAndCapacityAndComparerShouldBePossible()
    {
        var action = () =>
        {
            _ = new AsyncKeyedLocker<string>(Environment.ProcessorCount, 100, EqualityComparer<string>.Default);
        };

        action.Should().NotThrow();
    }

    [Test]
    public void TestMaxCount0AndConcurrencyLevelAndCapacityAndComparerShouldNotBePossible()
    {
        var action = () =>
        {
            _ = new AsyncKeyedLocker<string>(o => o.MaxCount = 0, Environment.ProcessorCount, 100, EqualityComparer<string>.Default);
        };

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void TestMaxCount1AndConcurrencyLevelAndCapacityAndComparerShouldBePossible()
    {
        var action = () =>
        {
            _ = new AsyncKeyedLocker<string>(o =>
            {
                o.MaxCount = 1;
                o.PoolSize = 1;
            }, Environment.ProcessorCount, 100, EqualityComparer<string>.Default);
        };

        action.Should().NotThrow();
    }

    [Test]
    public void TestDisposeDoesNotThrow()
    {
        var action = () =>
        {
            var asyncKeyedLocker = new AsyncKeyedLocker<string>(o =>
            {
                o.PoolSize = 20;
                o.PoolInitialFill = 1;
            });
            asyncKeyedLocker.Lock("test");
            asyncKeyedLocker.Dispose();
        };

        action.Should().NotThrow();
    }

    [Test]
    public void TestDisposeDoesNotThrowDespiteDisposedSemaphoreSlim()
    {
        var action = () =>
        {
            using var asyncKeyedLocker = new AsyncKeyedLocker<string>(o =>
            {
                o.PoolSize = 20;
                o.PoolInitialFill = 1;
            });
            var releaser = (AsyncKeyedLockReleaser<string>)asyncKeyedLocker.Lock("test");
            releaser.SemaphoreSlim.Dispose();
        };

        action.Should().NotThrow();
    }

    [Test]
    public void TestIndexDoesNotThrow()
    {
        using var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        asyncKeyedLocker.Lock("test");

        asyncKeyedLocker.Index.Count.Should().Be(1);
    }

    [Test]
    public void TestReadingMaxCount()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>(o => o.MaxCount = 2);

        asyncKeyedLocker.MaxCount.Should().Be(2);
    }

    [Test]
    public void TestReadingMaxCountViaParameter()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>(new AsyncKeyedLockOptions(2));

        asyncKeyedLocker.MaxCount.Should().Be(2);
    }

    [Test]
    public void TestReadingMaxCountViaParameterWithComparer()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>(new AsyncKeyedLockOptions(2), EqualityComparer<string>.Default);

        asyncKeyedLocker.MaxCount.Should().Be(2);
    }

    [Test]
    public void TestReadingMaxCountViaParameterWithConcurrencyLevelAndCapacity()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>(new AsyncKeyedLockOptions(2), Environment.ProcessorCount, 100);

        asyncKeyedLocker.MaxCount.Should().Be(2);
    }

    [Test]
    public void TestReadingMaxCountViaParameterWithConcurrencyLevelAndCapacityAndComparer()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>(new AsyncKeyedLockOptions(2), Environment.ProcessorCount, 100, EqualityComparer<string>.Default);

        asyncKeyedLocker.MaxCount.Should().Be(2);
    }

    [Test]
    public void TestGetCurrentCount()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();

        asyncKeyedLocker.GetRemainingCount("test").Should().Be(0);
        asyncKeyedLocker.GetCurrentCount("test").Should().Be(1);
        asyncKeyedLocker.Lock("test");
        asyncKeyedLocker.GetRemainingCount("test").Should().Be(1);
        asyncKeyedLocker.GetCurrentCount("test").Should().Be(0);
    }

    [Test]
    public async Task TestTimeoutBasic()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        using (var myLock = await asyncKeyedLocker.LockAsync("test", 0))
        {
            myLock.EnteredSemaphore.Should().BeTrue();
            asyncKeyedLocker.IsInUse("test").Should().BeTrue();
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void TestTimeoutBasicWithOutParameter()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        using (var _ = asyncKeyedLocker.Lock("test", 0, out var entered))
        {
            entered.Should().BeTrue();
            asyncKeyedLocker.IsInUse("test").Should().BeTrue();
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public async Task TestTimeout()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        using (await asyncKeyedLocker.LockAsync("test"))
        {
            using (var myLock = await asyncKeyedLocker.LockAsync("test", 0))
            {
                myLock.EnteredSemaphore.Should().BeFalse();
            }

            asyncKeyedLocker.IsInUse("test").Should().BeTrue();
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void TestTimeoutWithTimeSpanSynchronous()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        using (asyncKeyedLocker.Lock("test"))
        {
            using (asyncKeyedLocker.Lock("test", TimeSpan.Zero, out var entered))
            {
                entered.Should().BeFalse();
            }

            asyncKeyedLocker.IsInUse("test").Should().BeTrue();
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void TestTimeoutWithInfiniteTimeoutSynchronous()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        using (asyncKeyedLocker.Lock("test", Timeout.Infinite, out var entered))
        {
            entered.Should().BeTrue();
            asyncKeyedLocker.IsInUse("test").Should().BeTrue();
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void TestTimeoutWithInfiniteTimeSpanSynchronous()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        using (asyncKeyedLocker.Lock("test", TimeSpan.FromMilliseconds(Timeout.Infinite), out var entered))
        {
            entered.Should().BeTrue();
            asyncKeyedLocker.IsInUse("test").Should().BeTrue();
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public async Task TestTimeoutWithTimeSpan()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        using (await asyncKeyedLocker.LockAsync("test"))
        {
            using (var myLock = await asyncKeyedLocker.LockAsync("test", TimeSpan.Zero))
            {
                myLock.EnteredSemaphore.Should().BeFalse();
            }

            asyncKeyedLocker.IsInUse("test").Should().BeTrue();
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void TestTimeoutWithInfiniteTimeoutAndCancellationToken()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        using (asyncKeyedLocker.Lock("test", Timeout.Infinite, new CancellationToken(false), out bool entered))
        {
            entered.Should().BeTrue();
            asyncKeyedLocker.IsInUse("test").Should().BeTrue();
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void TestTimeoutWithZeroTimeoutAndCancellationToken()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        using (asyncKeyedLocker.Lock("test", 0, new CancellationToken(false), out var entered))
        {
            entered.Should().BeTrue();
            asyncKeyedLocker.IsInUse("test").Should().BeTrue();
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void TestTimeoutWithZeroTimeoutAndCancelledToken()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        var action = () =>
        {
            asyncKeyedLocker.Lock("test", 0, new CancellationToken(true), out _);
        };

        action.Should().Throw<OperationCanceledException>();
        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void TestTimeoutWithInfiniteTimeSpanAndCancellationToken()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        using (asyncKeyedLocker.Lock("test", TimeSpan.FromMilliseconds(Timeout.Infinite), new CancellationToken(false), out var entered))
        {
            entered.Should().BeTrue();
            asyncKeyedLocker.IsInUse("test").Should().BeTrue();
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void TestTimeoutWithZeroTimeSpanAndCancellationToken()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        using (asyncKeyedLocker.Lock("test", TimeSpan.FromMilliseconds(0), new CancellationToken(false), out var entered))
        {
            entered.Should().BeTrue();
            asyncKeyedLocker.IsInUse("test").Should().BeTrue();
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void TestTimeoutWithZeroTimeSpanAndCancelledToken()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        var action = () =>
        {
            asyncKeyedLocker.Lock("test", TimeSpan.FromMilliseconds(0), new CancellationToken(true), out _);
        };

        action.Should().Throw<OperationCanceledException>();
        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public void TestTimeoutTryLock()
    {
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        using (asyncKeyedLocker.Lock("test"))
        {
            asyncKeyedLocker.IsInUse("test").Should().BeTrue();
            asyncKeyedLocker.TryLock("test", () => { }, 0, CancellationToken.None).Should().BeFalse();
            asyncKeyedLocker.TryLock("test", () => { }, TimeSpan.Zero, CancellationToken.None).Should().BeFalse();
        }

        asyncKeyedLocker.IsInUse("test").Should().BeFalse();
    }

    [Test]
    public async Task BasicTest()
    {
        var locks = 5000;
        var concurrency = 50;
        var asyncKeyedLocker = new AsyncKeyedLocker<object>();
        var concurrentQueue = new ConcurrentQueue<(bool entered, int key)>();

        var tasks = Enumerable.Range(1, locks * concurrency)
            .Select(async i =>
            {
                var key = Convert.ToInt32(Math.Ceiling((double)i / concurrency));
                using (await asyncKeyedLocker.LockAsync(key))
                {
                    await Task.Delay(20);
                    concurrentQueue.Enqueue((true, key));
                    await Task.Delay(80);
                    concurrentQueue.Enqueue((false, key));
                }
            });
        await Task.WhenAll(tasks.AsParallel());

        var valid = concurrentQueue.Count == locks * concurrency * 2;

        var entered = new HashSet<int>();

        while (valid && !concurrentQueue.IsEmpty)
        {
            concurrentQueue.TryDequeue(out var result);
            if (result.entered)
            {
                if (entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Add(result.key);
            }
            else
            {
                if (!entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Remove(result.key);
            }
        }

        valid.Should().BeTrue();
    }

    [Test]
    public async Task BasicTestGenerics()
    {
        var locks = 5000;
        var concurrency = 50;
        var asyncKeyedLocker = new AsyncKeyedLocker<int>();
        var concurrentQueue = new ConcurrentQueue<(bool entered, int key)>();

        var tasks = Enumerable.Range(1, locks * concurrency)
            .Select(async i =>
            {
                var key = Convert.ToInt32(Math.Ceiling((double)i / concurrency));
                using (await asyncKeyedLocker.LockAsync(key))
                {
                    await Task.Delay(20);
                    concurrentQueue.Enqueue((true, key));
                    await Task.Delay(80);
                    concurrentQueue.Enqueue((false, key));
                }
            });
        await Task.WhenAll(tasks.AsParallel());

        var valid = concurrentQueue.Count == locks * concurrency * 2;

        var entered = new HashSet<int>();

        while (valid && !concurrentQueue.IsEmpty)
        {
            concurrentQueue.TryDequeue(out var result);
            if (result.entered)
            {
                if (entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Add(result.key);
            }
            else
            {
                if (!entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Remove(result.key);
            }
        }

        valid.Should().BeTrue();
    }


    [Test]
    public async Task BasicTestGenericsPooling50k()
    {
        const int Locks = 50_000;
        const int Concurrency = 50;
        var asyncKeyedLocker = new AsyncKeyedLocker<int>(o => o.PoolSize = 50_000, Environment.ProcessorCount, 50_000);
        var concurrentQueue = new ConcurrentQueue<(bool entered, int key)>();

        var tasks = Enumerable.Range(1, Locks * Concurrency)
            .Select(async i =>
            {
                var key = Convert.ToInt32(Math.Ceiling((double)i / Concurrency));
                using (await asyncKeyedLocker.LockAsync(key))
                {
                    await Task.Delay(20);
                    concurrentQueue.Enqueue((true, key));
                    await Task.Delay(80);
                    concurrentQueue.Enqueue((false, key));
                }
            });
        await Task.WhenAll(tasks.AsParallel());

        var valid = concurrentQueue.Count == Locks * Concurrency * 2;

        var entered = new HashSet<int>();

        while (valid && !concurrentQueue.IsEmpty)
        {
            concurrentQueue.TryDequeue(out var result);
            if (result.entered)
            {
                if (entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Add(result.key);
            }
            else
            {
                if (!entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Remove(result.key);
            }
        }

        valid.Should().BeTrue();
    }

    [Test]
    public async Task BasicTestGenericsPooling50kUnfilled()
    {
        const int Locks = 50_000;
        const int Concurrency = 50;
        var asyncKeyedLocker = new AsyncKeyedLocker<int>(o =>
        {
            o.PoolSize = 50_000;
            o.PoolInitialFill = 0;
        }, Environment.ProcessorCount, 50_000);
        var concurrentQueue = new ConcurrentQueue<(bool entered, int key)>();

        var tasks = Enumerable.Range(1, Locks * Concurrency)
            .Select(async i =>
            {
                var key = Convert.ToInt32(Math.Ceiling((double)i / Concurrency));
                using (await asyncKeyedLocker.LockAsync(key))
                {
                    await Task.Delay(20);
                    concurrentQueue.Enqueue((true, key));
                    await Task.Delay(80);
                    concurrentQueue.Enqueue((false, key));
                }
            });
        await Task.WhenAll(tasks.AsParallel());

        var valid = concurrentQueue.Count == Locks * Concurrency * 2;

        var entered = new HashSet<int>();

        while (valid && !concurrentQueue.IsEmpty)
        {
            concurrentQueue.TryDequeue(out var result);
            if (result.entered)
            {
                if (entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Add(result.key);
            }
            else
            {
                if (!entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Remove(result.key);
            }
        }

        valid.Should().BeTrue();
    }

    [Test]
    public async Task BasicTestGenericsPoolingProcessorCount()
    {
        const int Locks = 50_000;
        const int Concurrency = 50;
        var asyncKeyedLocker = new AsyncKeyedLocker<int>(o => o.PoolSize = Environment.ProcessorCount, Environment.ProcessorCount, 50_000);
        var concurrentQueue = new ConcurrentQueue<(bool entered, int key)>();

        var tasks = Enumerable.Range(1, Locks * Concurrency)
            .Select(async i =>
            {
                var key = Convert.ToInt32(Math.Ceiling((double)i / Concurrency));
                using (await asyncKeyedLocker.LockAsync(key))
                {
                    await Task.Delay(20);
                    concurrentQueue.Enqueue((true, key));
                    await Task.Delay(80);
                    concurrentQueue.Enqueue((false, key));
                }
            });
        await Task.WhenAll(tasks.AsParallel());

        var valid = concurrentQueue.Count == Locks * Concurrency * 2;

        var entered = new HashSet<int>();

        while (valid && !concurrentQueue.IsEmpty)
        {
            concurrentQueue.TryDequeue(out var result);
            if (result.entered)
            {
                if (entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Add(result.key);
            }
            else
            {
                if (!entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Remove(result.key);
            }
        }

        valid.Should().BeTrue();
    }

    [Test]
    public async Task BasicTestGenericsPooling10k()
    {
        const int Locks = 50_000;
        const int Concurrency = 50;
        var asyncKeyedLocker = new AsyncKeyedLocker<int>(o => o.PoolSize = 10_000, Environment.ProcessorCount, 50_000);
        var concurrentQueue = new ConcurrentQueue<(bool entered, int key)>();

        var tasks = Enumerable.Range(1, Locks * Concurrency)
            .Select(async i =>
            {
                var key = Convert.ToInt32(Math.Ceiling((double)i / Concurrency));
                using (await asyncKeyedLocker.LockAsync(key))
                {
                    await Task.Delay(20);
                    concurrentQueue.Enqueue((true, key));
                    await Task.Delay(80);
                    concurrentQueue.Enqueue((false, key));
                }
            });
        await Task.WhenAll(tasks.AsParallel());

        var valid = concurrentQueue.Count == Locks * Concurrency * 2;

        var entered = new HashSet<int>();

        while (valid && !concurrentQueue.IsEmpty)
        {
            concurrentQueue.TryDequeue(out var result);
            if (result.entered)
            {
                if (entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Add(result.key);
            }
            else
            {
                if (!entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Remove(result.key);
            }
        }

        valid.Should().BeTrue();
    }

    [Test]
    public async Task BasicTestGenericsString()
    {
        const int Locks = 5000;
        const int Concurrency = 50;
        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        var concurrentQueue = new ConcurrentQueue<(bool entered, string key)>();

        var tasks = Enumerable.Range(1, Locks * Concurrency)
            .Select(async i =>
            {
                var key = Convert.ToInt32(Math.Ceiling((double)i / 5)).ToString();
                using (await asyncKeyedLocker.LockAsync(key))
                {
                    await Task.Delay(20);
                    concurrentQueue.Enqueue((true, key));
                    await Task.Delay(80);
                    concurrentQueue.Enqueue((false, key));
                }
            });
        await Task.WhenAll(tasks.AsParallel());

        var valid = concurrentQueue.Count == Locks * Concurrency * 2;

        var entered = new HashSet<string>();

        while (valid && !concurrentQueue.IsEmpty)
        {
            concurrentQueue.TryDequeue(out var result);
            if (result.entered)
            {
                if (entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Add(result.key);
            }
            else
            {
                if (!entered.Contains(result.key))
                {
                    valid = false;
                    break;
                }
                entered.Remove(result.key);
            }
        }

        valid.Should().BeTrue();
    }

    [Test]
    public async Task Test1AtATime()
    {
        const int Range = 25000;
        var asyncKeyedLocker = new AsyncKeyedLocker<object>();
        var concurrentQueue = new ConcurrentQueue<int>();

        var threadNum = 0;

        var tasks = Enumerable.Range(1, Range * 2)
            .Select(async _ =>
            {
                var key = Convert.ToInt32(Math.Ceiling((double)Interlocked.Increment(ref threadNum) / 2));
                using (await asyncKeyedLocker.LockAsync(key))
                {
                    concurrentQueue.Enqueue(key);
                }
            });
        await Task.WhenAll(tasks.AsParallel());

        var valid = true;
        var list = concurrentQueue.ToList();

        for (var i = 0; i < Range; i++)
        {
            if (list[i] == list[i + Range])
            {
                valid = false;
                break;
            }
        }

        valid.Should().BeTrue();
    }

    [Test]
    public async Task Test2AtATime()
    {
        const int Range = 4;
        var asyncKeyedLocker = new AsyncKeyedLocker<object>(o => o.MaxCount = 2);
        var concurrentQueue = new ConcurrentQueue<int>();

        var tasks = Enumerable.Range(1, Range * 4)
            .Select(async i =>
            {
                var key = Convert.ToInt32(Math.Ceiling((double)i / 4));
                using (await asyncKeyedLocker.LockAsync(key))
                {
                    concurrentQueue.Enqueue(key);
                    await Task.Delay((100 * key) + 1000);
                }
            });
        await Task.WhenAll(tasks.AsParallel());

        var valid = true;
        var list = concurrentQueue.ToList();

        for (var i = 0; i < Range * 2; i++)
        {
            if (list[i] != list[i + (Range * 2)])
            {
                valid = false;
                break;
            }
        }

        valid.Should().BeTrue();
    }

    [Test]
    public async Task Test1AtATimeGenerics()
    {
        const int Range = 25000;
        var asyncKeyedLocker = new AsyncKeyedLocker<int>();
        var concurrentQueue = new ConcurrentQueue<int>();

        var threadNum = 0;

        var tasks = Enumerable.Range(1, Range * 2)
            .Select(async _ =>
            {
                var key = Convert.ToInt32(Math.Ceiling((double)Interlocked.Increment(ref threadNum) / 2));
                using (await asyncKeyedLocker.LockAsync(key))
                {
                    concurrentQueue.Enqueue(key);
                }
            });
        await Task.WhenAll(tasks.AsParallel());

        var valid = true;
        var list = concurrentQueue.ToList();

        for (var i = 0; i < Range; i++)
        {
            if (list[i] == list[i + Range])
            {
                valid = false;
                break;
            }
        }

        valid.Should().BeTrue();
    }

    [Test]
    public async Task Test2AtATimeGenerics()
    {
        const int Range = 4;
        var asyncKeyedLocker = new AsyncKeyedLocker<int>(o => o.MaxCount = 2);
        var concurrentQueue = new ConcurrentQueue<int>();

        var tasks = Enumerable.Range(1, Range * 4)
            .Select(async i =>
            {
                var key = Convert.ToInt32(Math.Ceiling((double)i / 4));
                using (await asyncKeyedLocker.LockAsync(key))
                {
                    concurrentQueue.Enqueue(key);
                    await Task.Delay((100 * key) + 1000);
                }
            });
        await Task.WhenAll(tasks.AsParallel());

        var valid = true;
        var list = concurrentQueue.ToList();

        for (var i = 0; i < Range * 2; i++)
        {
            if (list[i] != list[i + (Range * 2)])
            {
                valid = false;
                break;
            }
        }

        valid.Should().BeTrue();
    }

    [Test]
    public Task TestContinueOnCapturedContextTrue()
        => TestContinueOnCapturedContext(true);

    [Test]
    public Task TestContinueOnCapturedContextFalse()
        => TestContinueOnCapturedContext(false);

    private async Task TestContinueOnCapturedContext(bool continueOnCapturedContext)
    {
        const string Key = "test";

        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        var testContext = new AsyncKeyedLockSynchronizationContext();

        void Callback()
        {
            if (continueOnCapturedContext)
            {
                Environment.CurrentManagedThreadId.Should().Be(testContext.LastPostThreadId);
            }
            else
            {
                testContext.LastPostThreadId.Should().Be(default);
            }
        }

        var previousContext = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(testContext);

        try
        {
            // This is just to make WaitAsync in TryLockAsync not finish synchronously
            var obj = asyncKeyedLocker.Lock(Key);

            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                obj.Dispose();
            });

            await asyncKeyedLocker.TryLockAsync(Key, Callback, 5000, continueOnCapturedContext);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }
    }

    [Test]
    public Task TestOptionContinueOnCapturedContext()
        => TestConfigureAwaitOptions(ConfigureAwaitOptions.ContinueOnCapturedContext);

    [Test]
    public Task TestOptionForceYielding()
        => TestConfigureAwaitOptions(ConfigureAwaitOptions.ForceYielding);

    private async Task TestConfigureAwaitOptions(ConfigureAwaitOptions configureAwaitOptions)
    {
        const string Key = "test";

        var asyncKeyedLocker = new AsyncKeyedLocker<string>();
        var testContext = new AsyncKeyedLockSynchronizationContext();

        void Callback()
        {
            if (configureAwaitOptions == ConfigureAwaitOptions.ContinueOnCapturedContext)
            {
                Environment.CurrentManagedThreadId.Should().Be(testContext.LastPostThreadId);
            }
            else
            {
                testContext.LastPostThreadId.Should().Be(default);
            }
        }

        var previousContext = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(testContext);

        try
        {
            // This is just to make WaitAsync in TryLockAsync not finish synchronously
            var obj = asyncKeyedLocker.Lock(Key);

            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                obj.Dispose();
            });

            await asyncKeyedLocker.TryLockAsync(Key, Callback, 5000, configureAwaitOptions);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }
    }
}
