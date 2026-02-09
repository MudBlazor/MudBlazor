// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor;

/// <summary>
/// Controls how a component behaves when the browser is scrolled.
/// </summary>
[EnumExtensions]
public enum OverflowBehavior
{
    /// <summary>
    /// Prevents any adjustment of the component, even if it would overflow the container.
    /// </summary>
    [Description("flip-never")]
    FlipNever,

    /// <summary>
    /// Flips the component if it would overflow its container, but only when it first opens.  Does not update dynamically if overflow changes afterwards.
    /// </summary>
    [Description("flip-onopen")]
    FlipOnOpen,

    /// <summary>
    /// Flips the component if it would overflow its container, dynamically adjusting as necessary to prevent overflow.
    /// </summary>
    /// <remarks>
    /// This is the default for popovers unless otherwise set.
    /// </remarks>
    [Description("flip-always")]
    FlipAlways,
}
