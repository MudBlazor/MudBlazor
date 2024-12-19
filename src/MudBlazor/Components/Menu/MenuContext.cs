// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor;

/// <summary>
/// Provides context for managing a menu, allowing for opening, closing, and toggling the menu state.
/// </summary>
public class MenuContext
{
    private readonly MudMenu _menu;

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuContext"/> class with the specified menu.
    /// </summary>
    /// <param name="menu">The <see cref="MudMenu"/> instance to manage.</param>
    public MenuContext(MudMenu menu)
    {
        _menu = menu;
    }

    /// <summary>
    /// Closes all menus in the hierarchy, starting from the top-most parent.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task CloseAllMenusAsync() => _menu.CloseAllMenusAsync();

    /// <summary>
    /// Closes the current menu.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task CloseMenuAsync() => _menu.CloseMenuAsync();

    /// <summary>
    /// Opens the current menu.
    /// </summary>
    /// <param name="args">
    /// <para>The event arguments for the activation event; <see cref="MouseEventArgs"/> or <see cref="TouchEventArgs"/>.</para>
    /// <para>When <see cref="MudMenu.PositionAtCursor"/> is <c>true</c>, the menu will be positioned at these coordinates.</para>
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task OpenMenuAsync(EventArgs args) => _menu.OpenMenuAsync(args);

    /// <summary>
    /// Toggles the current menu's open or closed state.
    /// </summary>
    /// <param name="args">
    /// <para>The event arguments for the activation event; <see cref="MouseEventArgs"/> or <see cref="TouchEventArgs"/>.</para>
    /// <para>When <see cref="MudMenu.PositionAtCursor"/> is <c>true</c>, the menu will be positioned at these coordinates.</para>
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task ToggleMenuAsync(EventArgs args) => _menu.ToggleMenuAsync(args);
}
