// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities.Converter.Dispatcher;

/// <summary>
/// Determines how dispatcher builders handle registrations for the same specific input type.
/// </summary>
public enum DispatcherRegistrationPolicy
{
    /// <summary>
    /// The newest registration replaces the previous one.
    /// </summary>
    LastWins = 0,

    /// <summary>
    /// The first registration is kept and later duplicates are ignored.
    /// </summary>
    FirstWins = 1,

    /// <summary>
    /// Duplicate registrations throw an exception.
    /// </summary>
    Throw = 2
}
