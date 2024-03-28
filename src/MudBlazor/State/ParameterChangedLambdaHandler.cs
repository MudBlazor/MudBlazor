// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents a wrapper class for implementing the <see cref="IParameterChangedHandler{T}"/> interface 
/// using an Action lambda expression instead of directly implementing the interface.
/// </summary>
/// <typeparam name="T">The type of the component's property value.</typeparam>
internal class ParameterChangedLambdaHandler<T> : IParameterChangedHandler<T>
{
    private readonly Action _lambda;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterChangedLambdaHandler{T}"/> class with the specified lambda expression.
    /// </summary>
    /// <param name="lambda">The Action lambda expression to be executed when handling parameter change.</param>
    public ParameterChangedLambdaHandler(Action lambda)
    {
        _lambda = lambda;
    }

    /// <summary>
    /// Invokes the specified lambda expression when handling parameter change.
    /// </summary>
    /// <param name="parameterChangedEventArgs">The <see cref="ParameterChangedEventArgs{T}"/> containing the information about the last and current values of a parameter.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task HandleAsync(ParameterChangedEventArgs<T> parameterChangedEventArgs)
    {
        _lambda();

        return Task.CompletedTask;
    }
}
