// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace MudBlazor.Utilities.Converter.Dispatcher;

/// <summary>
/// Builder API used to register per-type converters and produce a dispatcher that routes conversions
/// to the appropriate registered converter.
/// </summary>
/// <typeparam name="TIn">The general input type the resulting dispatcher will accept.</typeparam>
/// <typeparam name="TOut">The output type produced by registered converters.</typeparam>
/// <remarks>
/// Implementations of this interface typically accumulate converters for concrete input types (via <see cref="Add"/>)
/// and then produce a composite dispatcher (via <see cref="Build"/>) that routes conversion requests to the registered handlers.
/// </remarks>
public interface IDispatcherBuilder<in TIn, TOut>
{
    /// <summary>
    /// Register a converter that handles conversions for the specific concrete input type <typeparamref name="TSpecific"/>.
    /// </summary>
    /// <typeparam name="TSpecific">The concrete input type this converter handles.</typeparam>
    /// <param name="converter">The converter instance that performs conversions from <typeparamref name="TSpecific"/> to <typeparamref name="TOut"/>.</param>
    /// <returns>The same builder instance to allow fluent registrations.</returns>
    IDispatcherBuilder<TIn, TOut> Add<TSpecific>(IConverter<TSpecific, TOut> converter);

    /// <summary>
    /// Register a converter instance for a concrete input type that is known only at runtime.
    /// </summary>
    /// <param name="specificType">The concrete input <see cref="System.Type"/> the supplied converter handles.</param>
    /// <param name="converter">
    /// A converter instance implementing the appropriate reversible converter contract for <paramref name="specificType"/>.
    /// Typically, the object should implement <c>IConverter&lt;TSpecific,TOut&gt;</c> for the supplied <paramref name="specificType"/>.
    /// </param>
    /// <returns>
    /// A builder typed to <see cref="IConverter{TIn,TOut}"/> to continue registrations. This overload is intended for
    /// dynamic scenarios where the concrete input type cannot be expressed at compile time.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="specificType"/> or <paramref name="converter"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the supplied <paramref name="converter"/> does not provide a compatible <c>Convert</c> method for <paramref name="specificType"/>.</exception>
    /// <remarks>
    /// Implementations will typically validate that <paramref name="converter"/> is compatible with the supplied <paramref name="specificType"/>.
    /// If the instance is incompatible a runtime exception may be thrown by the builder implementation.
    /// </remarks>
    IDispatcherBuilder<TIn, TOut> AddDynamic(Type specificType, object converter);

    /// <summary>
    /// Builds the dispatcher that routes conversions to the registered per-type converters.
    /// </summary>
    /// <returns>An instance of <see cref="IConverter{TIn, TOut}"/>.</returns>
    IConverter<TIn, TOut> Build();
}
