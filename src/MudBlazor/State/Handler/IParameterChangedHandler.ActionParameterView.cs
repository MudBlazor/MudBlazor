// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

/// <summary>
/// Represents a wrapper class for implementing the <see cref="IParameterChangedHandler{T}"/> interface
/// </summary>
/// <typeparam name="T">The type of the component's property value.</typeparam>
internal class ParameterChangedLambdaParameterViewHandler<T> : IParameterChangedHandler<T>
{
    private readonly Action<ParameterChangedContext> _lambda;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterChangedLambdaParameterViewHandler{T}"/> class
    /// with the specified lambda that accepts a <see cref="ParameterChangedContext"/>.
    /// </summary>
    /// <param name="lambda">
    /// An <see cref="Action"/> lambda to be executed when handling a parameter change.
    /// The lambda receives the <see cref="ParameterChangedContext"/> containing both the <see cref="ParameterView"/> 
    /// snapshot and the <see cref="ParameterStateCollection"/> with last and current values.
    /// </param>
    public ParameterChangedLambdaParameterViewHandler(Action<ParameterChangedContext> lambda)
    {
        _lambda = lambda;
    }

    /// <summary>
    /// Invokes the provided lambda, passing the <see cref="ParameterChangedContext"/>.
    /// </summary>
    /// <param name="parameterChangedEventArgs">
    /// The <see cref="ParameterChangedEventArgs{T}"/> containing the <see cref="ParameterView"/> snapshot
    /// and the last/current values for the changed parameter.
    /// </param>
    /// <param name="context">The parameter changed context.</param>
    /// <returns>A completed <see cref="Task"/>.</returns>
    public Task HandleAsync(ParameterChangedEventArgs<T> parameterChangedEventArgs, ParameterChangedContext context)
    {
        _lambda(context);

        return Task.CompletedTask;
    }
}
