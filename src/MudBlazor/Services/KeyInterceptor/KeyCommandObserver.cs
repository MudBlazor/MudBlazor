// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Services;

/// <summary>
/// Efficiently dispatches keyboard events to registered commands.
/// Uses early-exit pattern for optimal performance.
/// </summary>
internal sealed class KeyCommandObserver :
    IKeyDownObserver,
    IKeyUpObserver
{
    private readonly IReadOnlyList<IKeyCommand> _downCommands;
    private readonly IReadOnlyList<IKeyCommand> _upCommands;

    public KeyCommandObserver(IReadOnlyList<IKeyCommand> commands)
    {
        // Split commands by kind at construction time for faster dispatch
        var downCommands = new List<IKeyCommand>();
        var upCommands = new List<IKeyCommand>();

        foreach (var command in commands)
        {
            if (command.Kind == KeyEventKind.Down)
            {
                downCommands.Add(command);
            }
            else
            {
                upCommands.Add(command);
            }
        }

        _downCommands = downCommands;
        _upCommands = upCommands;
    }

    public Task NotifyOnKeyDownAsync(KeyboardEventArgs args)
        => DispatchAsync(_downCommands, args);

    public Task NotifyOnKeyUpAsync(KeyboardEventArgs args)
        => DispatchAsync(_upCommands, args);

    private static async Task DispatchAsync(IReadOnlyList<IKeyCommand> commands, KeyboardEventArgs args)
    {
        foreach (var command in commands)
        {
            if (command.CanExecute(args))
            {
                await command.ExecuteAsync(args);

                // If this is not a hook, stop processing (early-exit pattern)
                if (!command.IsHook)
                {
                    return;
                }
                // If it's a hook, continue to the next command
            }
        }
    }
}
