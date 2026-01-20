// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Controls what happens when duplicate snackbars are detected.
/// </summary>
public enum SnackbarDuplicatesBehavior
{
    /// <summary>
    /// Duplicate snackbars are allowed.
    /// </summary>
    Allow,

    /// <summary>
    /// Duplicate snackbars will not be displayed.
    /// </summary>
    Prevent,

    /// <summary>
    /// The global default is used to control duplicate snackbars.
    /// </summary>
    GlobalDefault,
}
