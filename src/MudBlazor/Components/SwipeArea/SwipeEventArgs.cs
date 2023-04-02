// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public class SwipeEventArgs
    {
        public TouchEventArgs TouchEventArgs { get; set; }
        public double? SwipeDelta { get; set; }
        public MudSwipeArea Sender { get; set; }
        public SwipeDirection SwipeDirection { get; set; }
    }
}
