using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Services;

namespace MudBlazor
{
    public static class Repositioning
    {
        /// <summary>
        /// If the PortalItem doesn't fit in the viewport,
        /// it mutates  the PortalItem rects to the new corrected position inside the viewport
        /// </summary>

        public static void CorrectAnchorBoundaries(PortalItem portalItem)
        {
            var FragmentIsAboveorBelowAnchor
                = portalItem.FragmentRect.Top > portalItem.AnchorRect.Bottom
                || portalItem.FragmentRect.Bottom < portalItem.AnchorRect.Top;

            if (portalItem.FragmentRect is null || portalItem.AnchorRect is null) return;

            // comes out at the bottom 
            if (portalItem.FragmentRect.IsOutsideBottom)
            {
                portalItem.AnchorRect.Top -=
                   2 * (portalItem.FragmentRect.Top - portalItem.AnchorRect.Bottom)
                    + portalItem.AnchorRect.Height
                    + portalItem.FragmentRect.Height;
            }

            // comes out at the top
            if (portalItem.FragmentRect.IsOutsideTop)
            {
                portalItem.AnchorRect.Top +=
                    2 * (Math.Abs(portalItem.AnchorRect.Top - portalItem.FragmentRect.Bottom))
                  + portalItem.AnchorRect.Height
                  + portalItem.FragmentRect.Height;
            }

            // comes out at the left
            if (portalItem.FragmentRect.IsOutsideLeft)
            {
                portalItem.AnchorRect.Left +=
                     FragmentIsAboveorBelowAnchor
                        ? portalItem.AnchorRect.Left - portalItem.FragmentRect.Left
                        : 2 * (Math.Abs(portalItem.AnchorRect.Left - portalItem.FragmentRect.Right))
                            + portalItem.FragmentRect.Width
                            + portalItem.AnchorRect.Width;
            }

            // comes out at the right
            if (portalItem.FragmentRect.IsOutsideRight)
            {
                portalItem.AnchorRect.Left -=
                    FragmentIsAboveorBelowAnchor
                    ? portalItem.FragmentRect.Right - portalItem.AnchorRect.Right
                    : 2 * (Math.Abs(portalItem.FragmentRect.Left - portalItem.AnchorRect.Right))
                        + portalItem.FragmentRect.Width
                        + portalItem.AnchorRect.Width;
            }
        }

    }
}
