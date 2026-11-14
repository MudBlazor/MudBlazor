// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor;


/// <summary>
/// Provides access to menu operations for external components and custom activators.
/// </summary>
/// <remarks>
/// This context is passed to <see cref="MudMenu.ActivatorContent"/> to allow custom activators
/// to control menu behavior through strongly-typed async methods.
/// </remarks>
public sealed class MenuContext
{
    private readonly MudMenu _menu;

    /// <summary>
    /// Creates a new instance of <see cref="MenuContext"/>.
    /// </summary>
    /// <param name="menu">The menu associated with this context.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="menu"/> is null.</exception>
    internal MenuContext(MudMenu menu)
    {
        ArgumentNullException.ThrowIfNull(menu);
        _menu = menu;
    }

    /// <summary>
    /// Opens the menu.
    /// </summary>
    /// <remarks>
    /// This opens the menu directly and does not filter the event against <see cref="MudMenu.ActivationEvent"/>.
    /// </remarks>
    /// <param name="args">
    /// Optional event arguments. When <see cref="MudMenu.PositionAtCursor"/> is <c>true</c>,
    /// the menu will be positioned at the coordinates from <see cref="MouseEventArgs"/> or <see cref="TouchEventArgs"/>.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task OpenAsync(EventArgs? args = null) => _menu.OpenMenuAsync(args ?? EventArgs.Empty);

    /// <summary>
    /// Closes the menu and any open sub-menus.
    /// </summary>
    /// <remarks>
    /// For hover activators, prefer <see cref="MudMenu.ActivationEvent"/> with <see cref="MouseEvent.MouseOver"/>
    /// instead of wiring this method to pointer leave events.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task CloseAsync() => _menu.CloseMenuAsync();

    /// <summary>
    /// Toggles the menu between open and closed states.
    /// </summary>
    /// <remarks>
    /// When <paramref name="args"/> is a <see cref="MouseEventArgs"/> and <see cref="MudMenu.ActivationEvent"/>
    /// is <see cref="MouseEvent.LeftClick"/> or <see cref="MouseEvent.RightClick"/>, the mouse button is checked
    /// before the menu is toggled.
    /// </remarks>
    /// <param name="args">
    /// Optional event arguments. When <see cref="MudMenu.PositionAtCursor"/> is <c>true</c>,
    /// the menu will be positioned at the coordinates from <see cref="MouseEventArgs"/> or <see cref="TouchEventArgs"/>.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ToggleAsync(EventArgs? args = null) => _menu.ToggleMenuAsync(args ?? EventArgs.Empty);

    /// <summary>
    /// Closes all menus in the hierarchy, starting from the top-most parent.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task CloseAllAsync() => _menu.CloseAllMenusAsync();
}
