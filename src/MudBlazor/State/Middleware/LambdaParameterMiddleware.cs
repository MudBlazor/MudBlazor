// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.State.Middleware;

#nullable enable
internal class LambdaParameterMiddleware<T> : IParameterMiddleware<T>
{
    private readonly Func<T?, T>? _onRead;
    private readonly Func<T, Func<T, Task>, Task>? _onWriteAsync;

    public LambdaParameterMiddleware(Func<T?, T>? onRead = null, Func<T, Func<T, Task>, Task>? onWriteAsync = null)
    {
        _onRead = onRead;
        _onWriteAsync = onWriteAsync;
    }

    public T? OnRead(T? currentValue) => _onRead is not null ? _onRead(currentValue) : currentValue;

    public Task OnWriteAsync(T newValue, Func<T, Task> next)
    {
        return _onWriteAsync is not null
            ? _onWriteAsync(newValue, next)
            : next(newValue);
    }
}
