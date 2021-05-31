// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor.Interop
{
    public class MudPositionOptions
    {
        public Direction Direction { get; set; }

        public bool Center { get; set; }

        public bool ApplyReferenceWidth { get; set; }

        internal void SetDirection(Placement placement, bool rtl)
        {
            switch (placement)
            {
                case Placement.Start:
                    Direction = rtl ? Direction.Right : Direction.Left;
                    break;
                case Placement.End:
                    Direction = rtl ? Direction.Left : Direction.Right;
                    break;
                case Placement.Top:
                    Direction = Direction.Top;
                    break;
                case Placement.Bottom:
                    Direction = Direction.Bottom;
                    break;
                default: break;
            }
        }
    }
}
