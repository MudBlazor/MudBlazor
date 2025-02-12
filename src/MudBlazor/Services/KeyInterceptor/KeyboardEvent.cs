// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Services;

/// <summary>
/// Represents a method that will handle keyboard events.
/// </summary>
/// <param name="args">The <see cref="KeyboardEventArgs"/> instance containing the event data.</param>
public delegate void KeyboardEvent(KeyboardEventArgs args);
