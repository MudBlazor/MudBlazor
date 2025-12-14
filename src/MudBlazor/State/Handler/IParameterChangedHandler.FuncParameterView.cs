// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents a wrapper class for implementing the <see cref="IParameterChangedHandler{T}"/> interface
/// </summary>
/// <typeparam name="T">The type of the component's property value.</typeparam>
internal class ParameterChangedLambdaTaskParameterViewHandler<T> : IParameterChangedHandler<T>
{
    private readonly Func<ParameterView, Task> _lambda;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterChangedLambdaTaskParameterViewHandler{T}"/> class
    /// with the specified lambda expression.
    /// </summary>
    /// <param name="lambda">
    /// The <c>Func&lt;ParameterView, Task&gt;</c> lambda expression to be executed when handling parameter changes.
    /// The lambda receives the <see cref="ParameterView"/> snapshot provided by Blazor when parameters were set.
    /// </param>
    public ParameterChangedLambdaTaskParameterViewHandler(Func<ParameterView, Task> lambda)
    {
        _lambda = lambda;
    }

    /// <summary>
    /// Invokes the specified lambda expression when handling parameter changes, passing the captured <see cref="ParameterView"/>.
    /// </summary>
    /// <param name="parameterChangedEventArgs">
    /// The <see cref="ParameterChangedEventArgs{T}"/> containing the captured <see cref="ParameterView"/> and the last/current values
    /// for the changed parameter.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task HandleAsync(ParameterChangedEventArgs<T> parameterChangedEventArgs)
    {
        return _lambda(parameterChangedEventArgs.ParameterView);
    }
}
