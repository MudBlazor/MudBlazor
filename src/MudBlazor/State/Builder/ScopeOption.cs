// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.State.Builder;

/// <summary>
/// Specifies the locking behavior for a scope.
/// </summary>
internal enum ScopeOption
{
    /// <summary>
    /// Lock the scope when it ends.
    /// </summary>
    Lock = 0,

    /// <summary>
    /// Keep the scope unlocked after it ends.
    /// </summary>
    /// <remarks>
    /// This option is useful when dealing with abstract classes or classes that will be inherited.
    /// <br/>
    /// It will remain unlocked until the parameters are read, or until the options are overridden with the <see cref="Lock"/> option and the scope ends.
    /// </remarks>
    KeepUnlocked = 1,
}
