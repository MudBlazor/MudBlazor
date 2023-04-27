// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public class SwipeEventArgs
    {
        public TouchEventArgs TouchEventArgs { get; private set; }
        public double? SwipeDelta { get; private set; }
        public MudSwipeArea Sender { get; private set; }
        public SwipeDirection SwipeDirection { get; private set; }

        public SwipeEventArgs(TouchEventArgs touchEventArgs, SwipeDirection swipeDirection, double? swipeDelta, MudSwipeArea sender)
        {
            TouchEventArgs = touchEventArgs;
            SwipeDirection = swipeDirection;
            SwipeDelta = swipeDelta;
            Sender = sender;
        }
    }
}
