// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// 
/// </summary>
internal class ParameterChangedLambdaHandler : IParameterChangedHandler
{
    private readonly Action _lambda;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lambda"></param>
    public ParameterChangedLambdaHandler(Action lambda)
    {
        _lambda = lambda;
    }

    /// <inheritdoc />
    public Task HandleAsync()
    {
        _lambda();

        return Task.CompletedTask;
    }
}
