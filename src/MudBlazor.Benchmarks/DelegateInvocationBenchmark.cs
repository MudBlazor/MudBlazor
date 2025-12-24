// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace MudBlazor.Benchmarks;

/// <summary>
/// Micro-benchmark to measure the actual cost of Func invocation vs cached field access.
/// This helps determine if the comparer caching optimization is worthwhile.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class DelegateInvocationBenchmark
{
    private readonly IEqualityComparer<int> _cachedComparer = EqualityComparer<int>.Default;
    private readonly Func<IEqualityComparer<int>> _comparerFunc = () => EqualityComparer<int>.Default;
    private int _value1 = 42;
    private int _value2 = 42;

    /// <summary>
    /// Baseline: Direct comparer usage (no func, no field)
    /// </summary>
    [Benchmark(Baseline = true)]
    public bool DirectComparer()
    {
        return EqualityComparer<int>.Default.Equals(_value1, _value2);
    }

    /// <summary>
    /// Cached field access
    /// </summary>
    [Benchmark]
    public bool CachedField()
    {
        return _cachedComparer.Equals(_value1, _value2);
    }

    /// <summary>
    /// Func invocation every time
    /// </summary>
    [Benchmark]
    public bool FuncInvocation()
    {
        return _comparerFunc().Equals(_value1, _value2);
    }

    /// <summary>
    /// Repeated equality checks with cached field (1000 iterations)
    /// </summary>
    [Benchmark]
    public bool RepeatedCachedField_1000()
    {
        bool result = false;
        for (int i = 0; i < 1000; i++)
        {
            result = _cachedComparer.Equals(_value1, _value2);
        }
        return result;
    }

    /// <summary>
    /// Repeated equality checks with Func invocation (1000 iterations)
    /// </summary>
    [Benchmark]
    public bool RepeatedFuncInvocation_1000()
    {
        bool result = false;
        for (int i = 0; i < 1000; i++)
        {
            result = _comparerFunc().Equals(_value1, _value2);
        }
        return result;
    }

    /// <summary>
    /// Lambda capture scenario (more realistic for dynamic comparers)
    /// </summary>
    [Benchmark]
    public bool LambdaCaptureInvocation()
    {
        var localComparer = EqualityComparer<int>.Default;
        Func<IEqualityComparer<int>> func = () => localComparer;
        return func().Equals(_value1, _value2);
    }
}
