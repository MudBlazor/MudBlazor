// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Provides event data for the swipe event.
    /// </summary>
    public class SwipeEventArgs
    {
        /// <summary>
        /// Gets information about a touch event that is being raised.
        /// </summary>
        public TouchEventArgs TouchEventArgs { get; }

        /// <summary>
        /// Gets the swipe delta value indicating the distance of the swipe movement.
        /// </summary>
        public double? SwipeDelta { get; }

        /// <summary>
        /// Gets the sender of the swipe event.
        /// </summary>
        public MudSwipeArea Sender { get; }

        /// <summary>
        /// Gets the direction of the swipe.
        /// </summary>
        public SwipeDirection SwipeDirection { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwipeEventArgs"/> class.
        /// </summary>
        /// <param name="touchEventArgs">The touch event arguments associated with the swipe event.</param>
        /// <param name="swipeDirection">The direction of the swipe.</param>
        /// <param name="swipeDelta">The swipe delta value indicating the distance of the swipe movement.</param>
        /// <param name="sender">The sender of the swipe event.</param>
        public SwipeEventArgs(TouchEventArgs touchEventArgs, SwipeDirection swipeDirection, double? swipeDelta, MudSwipeArea sender)
        {
            TouchEventArgs = touchEventArgs;
            SwipeDirection = swipeDirection;
            SwipeDelta = swipeDelta;
            Sender = sender;
        }
    }
}
