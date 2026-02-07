// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Services;

/// <summary>
/// Represents a keyboard command that can be executed based on keyboard events.
/// </summary>
internal interface IKeyCommand
{
    /// <summary>
    /// Gets the kind of keyboard event this command handles (Down or Up).
    /// </summary>
    KeyEventKind Kind { get; }

    /// <summary>
    /// Gets whether this command is a hook that should always execute without stopping the command chain.
    /// </summary>
    bool IsHook { get; }

    /// <summary>
    /// Determines whether this command can be executed for the given keyboard event.
    /// </summary>
    /// <param name="args">The keyboard event arguments.</param>
    /// <returns><c>true</c> if the command can execute; otherwise, <c>false</c>.</returns>
    bool CanExecute(KeyboardEventArgs args);

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="args">The keyboard event arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync(KeyboardEventArgs args);
}
