// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Services;

#nullable enable
internal class KeyCommandObserver(IReadOnlyList<IKeyCommand> commands) :
    IKeyDownObserver,
    IKeyUpObserver
{
    public Task NotifyOnKeyDownAsync(KeyboardEventArgs args)
        => DispatchAsync(KeyEventKind.Down, args);

    public Task NotifyOnKeyUpAsync(KeyboardEventArgs args)
        => DispatchAsync(KeyEventKind.Up, args);

    private Task DispatchAsync(KeyEventKind kind, KeyboardEventArgs args)
    {
        foreach (var command in commands)
        {
            if (command.Kind == kind && command.CanExecute(args))
            {
                return command.ExecuteAsync(args);
            }
        }

        return Task.CompletedTask;
    }
}
