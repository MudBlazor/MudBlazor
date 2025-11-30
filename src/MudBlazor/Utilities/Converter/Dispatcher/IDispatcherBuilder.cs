// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace MudBlazor.Utilities.Converter.Dispatcher;

/// <summary>
/// Builder API used to register per-type converters and produce a dispatcher that routes conversions
/// to the appropriate registered converter.
/// </summary>
/// <typeparam name="TIn">The general input type the resulting dispatcher will accept.</typeparam>
/// <typeparam name="TOut">
/// The output type produced by registered converters. Declared <c>in</c> to allow builders that accept
/// more derived output types where appropriate.
/// </typeparam>
/// <typeparam name="TConverter">
/// The type of converter produced by <see cref="Build"/> (for example <c>IConverter&lt;TIn,TOut&gt;</c> or a reversible variant).
/// Declared <c>out</c> to allow covariance of the produced converter type.
/// </typeparam>
/// <remarks>
/// Implementations of this interface typically accumulate converters for concrete input types (via <see cref="Add"/>)
/// and then produce a composite dispatcher (via <see cref="Build"/>) that routes conversion requests to the registered handlers.
/// </remarks>
public interface IDispatcherBuilder<TIn, in TOut, out TConverter>
{
    /// <summary>
    /// Register a converter that handles conversions for the specific concrete input type <typeparamref name="TSpecific"/>.
    /// </summary>
    /// <typeparam name="TSpecific">The concrete input type this converter handles.</typeparam>
    /// <param name="conv">The converter instance that performs conversions from <typeparamref name="TSpecific"/> to <typeparamref name="TOut"/>.</param>
    /// <returns>The same builder instance to allow fluent registrations.</returns>
    IDispatcherBuilder<TIn, TOut, TConverter> Add<TSpecific>(IConverter<TSpecific, TOut> conv);

    /// <summary>
    /// Builds the dispatcher that routes conversions to the registered per-type converters.
    /// </summary>
    /// <returns>An instance of <typeparamref name="TConverter"/> which implements the dispatching behaviour.</returns>
    TConverter Build();
}
