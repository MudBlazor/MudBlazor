# MudBlazor.Benchmarks

Performance benchmarking suite for MudBlazor's ParameterState framework using BenchmarkDotNet.

## Overview

This project contains comprehensive benchmarks to measure and optimize the performance of the `ParameterState` system in MudBlazor. The ParameterState framework is central to MudBlazor's rendering and parameter-binding system, used by hundreds of components throughout the library.

## Running Benchmarks

### Prerequisites

- .NET 9.0 SDK or later
- Release build configuration (required for accurate measurements)

### Run All Benchmarks

```bash
dotnet run -c Release --project src/MudBlazor.Benchmarks/MudBlazor.Benchmarks.csproj
```

### Run Specific Benchmark Suites

```bash
# Basic operations (register, lifecycle, SetValueAsync)
dotnet run -c Release --project src/MudBlazor.Benchmarks/MudBlazor.Benchmarks.csproj -- --basic

# Large-scale scenarios (100, 1000 parameters)
dotnet run -c Release --project src/MudBlazor.Benchmarks/MudBlazor.Benchmarks.csproj -- --largescale

# Comparer strategies
dotnet run -c Release --project src/MudBlazor.Benchmarks/MudBlazor.Benchmarks.csproj -- --comparer
```

## Benchmark Suites

### 1. ParameterStateBasicOperationsBenchmark

Measures performance of fundamental ParameterState operations:

- **RegisterSingleIntParameter**: Cost of registering a single int parameter
- **RegisterSingleStringParameter**: Cost of registering a single string parameter
- **RegisterTenParameters**: Overhead of registering 10 parameters
- **FullLifecycleSingleParameter**: Complete lifecycle simulation (first render)
- **SetValueAsync_NoCallback**: SetValueAsync without EventCallback
- **SetValueAsync_WithCallback**: SetValueAsync with EventCallback invocation
- **SetValueAsync_SameValue**: SetValueAsync with unchanged value (equality check path)
- **SetValueAsync_Repeated100Times**: Repeated value updates (100 iterations)

### 2. ParameterStateLargeScaleBenchmark

Stress tests with large parameter counts:

- **RegisterManyParameters**: Register N parameters (100, 1000)
- **FullLifecycleManyParameters**: Full lifecycle with N parameters
- **OnParametersSet_NoChanges**: OnParametersSet with no value changes (best case)
- **OnParametersSet_AllChanged**: OnParametersSet with all values changed (worst case)
- **SetValueAsync_AllParameters**: SetValueAsync on all N parameters

### 3. ParameterStateComparerBenchmark

Compares different comparer strategies:

- **Default comparer** vs **custom comparer** vs **dynamic comparer**
- **Reference equality** vs **deep equality** for complex types
- **Repeated equality checks** with different comparer types
- Impact of custom comparers on strings, lists, and complex objects

## Architecture

### SyntheticParameterStateContainer

The benchmarks use a synthetic container that simulates Blazor component lifecycle **without** depending on the Blazor runtime. This allows isolated performance testing of ParameterState operations.

#### Blazor Lifecycle Simulation

The container properly simulates the Blazor lifecycle as documented at https://blazor-university.com/components/component-lifecycles/:

**First Render (new instance):**
1. `SetParametersAsync` - receives parameters
2. `base.SetParametersAsync(parameters)` - assigns [Parameter] properties
3. `OnInitialized` - called once for new instances
4. `OnParametersSet` - called after OnInitialized

**Re-Render (existing instance):**
1. `SetParametersAsync` - receives parameters
2. `base.SetParametersAsync(parameters)` - assigns [Parameter] properties
3. `OnParametersSet` - called directly (OnInitialized is skipped)

## Performance Findings

### Identified Hotspots

Based on code analysis and benchmark results, the following areas have been identified as potential optimization targets:

1. **Comparer Invocation**
   - `ParameterEqualityComparerSwappable<T>.Equals()` invokes `UnderlyingComparer()` on every equality check
   - For dynamic comparers (lambdas), this creates delegate invocations overhead
   - **Impact**: Medium - Called during `SetValueAsync` and `OnParametersSet`

2. **Dictionary Lookups**
   - `FrozenDictionary` used in `ParameterScopeContainer` is already optimized
   - `SelectMany` operations in `ParameterContainer.SetParametersAsync` iterate all parameters
   - **Impact**: Low-Medium - Only affects components with many parameters

3. **Lazy Initialization**
   - `RegisterParameterBuilder<T>` uses `Lazy<T>` for parameter state creation
   - `ParameterScopeContainer` uses `Lazy<FrozenDictionary<>>` for parameters
   - **Impact**: Low - One-time cost, already optimized

4. **Equality Comparisons**
   - Three equality comparisons in hot paths:
     - `SetValueAsync`: `_comparer.Equals(_value, value)`
     - `OnParametersSet`: `_comparer.Equals(_lastValue, currentParameterValue)`
     - `HasParameterChanged`: `comparer.Equals(...)` via ParameterView extension
   - **Impact**: High - Called frequently during re-renders

5. **EventCallback Invocation**
   - `eventCallback.InvokeAsync(value)` when HasDelegate is true
   - Already uses Task-based async pattern
   - **Impact**: Low - Inherent cost of two-way binding

6. **ParameterView Extensions**
   - `HasParameterChanged` performs manual parameter lookup
   - Special case handling for comparer parameter swapping
   - **Impact**: Medium - Called for every parameter during SetParametersAsync

### Optimization Opportunities

1. **Cache Comparer Results**
   - For static comparers, cache the comparer instance instead of invoking delegate
   - Detect static vs dynamic comparers at registration time
   - **Expected Gain**: 5-15% for equality-heavy workloads

2. **Reduce Allocations**
   - `ParameterChangedEventArgs<T>` created on every change
   - Consider object pooling for high-frequency scenarios
   - **Expected Gain**: Reduced GC pressure in large forms

3. **Optimize Batch Updates**
   - `ToHashSet(ParameterHandlerUniquenessComparer.Default)` allocates
   - Consider pre-sized collections when parameter count is known
   - **Expected Gain**: 2-5% for components with many parameters

4. **Inline Fast Paths**
   - Add fast path for `SetValueAsync` when value hasn't changed (already done)
   - Add fast path for `OnParametersSet` when no handlers exist
   - **Expected Gain**: 10-20% for read-only/display components

## Benchmarking Best Practices

1. **Always use Release configuration**: Debug builds include assertions and extra checks
2. **Close other applications**: Minimize background noise
3. **Run multiple times**: Statistical variance can affect results
4. **Use `[MemoryDiagnoser]`**: Track allocations, not just CPU time
5. **Baseline comparisons**: Mark one test as `[Benchmark(Baseline = true)]` for relative comparisons

## Contributing

When adding new benchmarks:

1. Place related benchmarks in the same class
2. Use `[MemoryDiagnoser]` to track allocations
3. Use `[Params(...)]` for varying test sizes
4. Document what each benchmark measures
5. Add a summary of expected findings

## See Also

- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
- [Blazor Component Lifecycles](https://blazor-university.com/components/component-lifecycles/)
- [MudBlazor Contributing Guide](../../CONTRIBUTING.md)
