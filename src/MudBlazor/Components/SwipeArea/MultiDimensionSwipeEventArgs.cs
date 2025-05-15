// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor;

#nullable enable
public class MultiDimensionSwipeEventArgs
{
    /// <summary>
    /// The information about the pointer.
    /// </summary>
    public PointerEventArgs TouchEventArgs { get; }

    /// <summary>
    /// The distance of the swipe gestures in pixels. Has two values, one for the x-axis and one for the y-axis.
    /// </summary>
    public IReadOnlyList<double?> SwipeDeltas { get; }

    /// <summary>
    /// The <see cref="MudSwipeArea"/> which raised the swipe event.
    /// </summary>
    public MudSwipeArea Sender { get; }

    /// <summary>
    /// The direction list of the swipe. Has two values, one for the x-axis and one for the y-axis.
    /// </summary>
    public IReadOnlyList<SwipeDirection> SwipeDirections { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwipeEventArgs"/> class.
    /// </summary>
    /// <param name="touchEventArgs">The size, pressure, and tilt of the pointer.</param>
    /// <param name="swipeDirections">The direction of the swipe.</param>
    /// <param name="swipeDeltas">The distance of the swipe movement, in pixels.</param>
    /// <param name="sender">The <see cref="MudSwipeArea" /> which originated the swipe event.</param>
    public MultiDimensionSwipeEventArgs(PointerEventArgs touchEventArgs, IReadOnlyList<SwipeDirection> swipeDirections, IReadOnlyList<double?> swipeDeltas, MudSwipeArea sender)
    {
        TouchEventArgs = touchEventArgs;
        SwipeDirections = swipeDirections;
        SwipeDeltas = swipeDeltas;
        Sender = sender;
    }
}
