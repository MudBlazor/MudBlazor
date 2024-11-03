// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Controls how a component behaves when the browser is scrolled.
/// </summary>
public enum OverflowBehavior
{
    /// <summary>
    /// No special behavior will occur as the browser is scrolled.
    /// </summary>
    [Description("flip-never")]
    FlipNever,

    /// <summary>
    /// The component will display on-screen when opened.
    /// </summary>
    [Description("flip-onopen")]
    FlipOnOpen,

    /// <summary>
    /// The component will remain visible even when the browser is scrolled.
    /// </summary>
    [Description("flip-always")]
    FlipAlways,
}
