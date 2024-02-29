// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace MudBlazor.State;

#nullable enable
internal class ParameterChangedLambdaTaskHandler : IParameterChangedHandler
{
    private readonly Func<Task> _lambda;

    public ParameterChangedLambdaTaskHandler(Func<Task> lambda)
    {
        _lambda = lambda;
    }

    public Task HandleAsync()
    {
        return _lambda();
    }
}
