// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor;

#nullable enable

/// <summary>
/// The context for a <see cref="MudMenu"/> component.
/// </summary>
/// <remarks>
/// This context is used to manage activation and state for menu components, centralizing associated logic.
/// </remarks>
public class MenuContext
{
    private readonly MudMenu _menu;

    /// <summary>
    /// Creates a new instance of <see cref="MenuContext"/>.
    /// </summary>
    /// <param name="menu">The menu associated with this context.</param>
    public MenuContext(MudMenu menu)
    {
        _menu = menu;
    }

    /// <summary>
    /// Activates the menu, toggling its open or closed state.
    /// </summary>
    /// <param name="activator">The object which raised the activation event.</param>
    /// <param name="args">The mouse event arguments for the activation event.</param>
    public void Activate(object activator, MouseEventArgs args)
    {
        // Prevent activation if the activator button has a specific CSS class that marks it as non-activatable.
        if (activator is MudBaseButton activatorButton &&
            (activatorButton.Class?.Contains("mud-no-activator") ?? false))
        {
            return;
        }

        _menu.ToggleMenuAsync(args).CatchAndLog();
    }
}
