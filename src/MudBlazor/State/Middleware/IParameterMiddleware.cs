// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
namespace MudBlazor.State.Middleware;

#nullable enable
/// <summary>
/// Middleware that can intercept and modify reads and writes performed by <see cref="ParameterState{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the parameter value.</typeparam>
/// <remarks>
/// Middlewares are invoked in the order they are registered:
/// <list type="bullet">
///   <item>
///     <description>
///       <see cref="OnRead(T?)"/> is called for every read and may transform or validate the value before it is returned.
///     </description>
///   </item>
///   <item>
///     <description>
///       <see cref="OnWriteAsync(T, Func{T, Task})"/> participates in the write pipeline and can modify, validate or short-circuit the write operation.
///       When participating in the write pipeline, a middleware should call the provided <c>next</c> delegate to continue processing; omitting the call short-circuits the pipeline.
///     </description>
///   </item>
/// </list>
/// Middlewares provide a stable extensibility point — you can add new behaviour (for example: transformation, logging, add <see cref="System.ComponentModel.INotifyPropertyChanged"/>, etc.)
/// to parameter reads/writes without modifying the core <see cref="ParameterState{T}"/> implementation.
/// The order of registration determines invocation order and therefore can affect overall behaviour.
/// </remarks>
public interface IParameterMiddleware<T>
{
    /// <summary>
    /// Called when the parameter value is read via <see cref="ParameterState{T}.Value"/>.
    /// </summary>
    /// <param name="currentValue">The current value as seen by the pipeline. May be <c>null</c>.</param>
    /// <returns>
    /// The value that should be returned to the caller after middleware processing.
    /// The middleware may return the same value or a transformed value.
    /// </returns>
    T? OnRead(T? currentValue);

    /// <summary>
    /// Called when the parameter value is being written (set) as part of the <see cref="ParameterState{T}"/> write pipeline.
    /// </summary>
    /// <param name="newValue">The incoming value requested to be set.</param>
    /// <param name="next">
    /// A delegate that invokes the next middleware (or the final setter) in the pipeline.
    /// Call <c>await next(newValue)</c> to continue the pipeline and pass the (optionally modified) value.
    /// </param>
    /// <returns>A task that completes when this middleware and any downstream processing has finished.</returns>
    /// <remarks>
    /// The write pipeline preserves the registration order of middlewares. A middleware may short-circuit the pipeline by not calling <paramref name="next"/>.
    /// </remarks>
    Task OnWriteAsync(T newValue, Func<T, Task> next);
}
