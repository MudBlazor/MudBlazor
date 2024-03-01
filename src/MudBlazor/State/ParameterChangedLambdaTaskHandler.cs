// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents a wrapper class for implementing the <see cref="IParameterChangedHandler"/> interface
/// using a Func&lt;Task&gt; lambda expression instead of directly implementing the interface.
/// </summary>
internal class ParameterChangedLambdaTaskHandler : IParameterChangedHandler
{
    private readonly Func<Task> _lambda;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterChangedLambdaTaskHandler"/> class with the specified lambda expression.
    /// </summary>
    /// <param name="lambda">The Func&lt;Task&gt; lambda expression to be executed when handling parameter changes.</param>
    public ParameterChangedLambdaTaskHandler(Func<Task> lambda)
    {
        _lambda = lambda;
    }

    /// <summary>
    /// Invokes the specified lambda expression when handling parameter changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task HandleAsync()
    {
        return _lambda();
    }
}
