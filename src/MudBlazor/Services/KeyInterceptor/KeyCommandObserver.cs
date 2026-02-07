// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Services;

/// <summary>
/// Dispatches keyboard events to a precompiled command list with an early-exit pipeline.
/// </summary>
/// <remarks>
/// The key interceptor builds this observer after mapping user key bindings. It keeps the hot path small by splitting commands by event kind and stopping on the first non-hook match.
/// </remarks>
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
