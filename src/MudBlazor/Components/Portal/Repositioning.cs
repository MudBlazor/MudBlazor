using System;
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
            if (portalItem.FragmentRect is null || portalItem.AnchorRect is null) return;

            var rectified = portalItem.AnchorRect.Clone();
            if (rectified.IsOutsideBottom)
            {
                rectified.Top =
                    portalItem.AnchorRect.WindowHeight - portalItem.AnchorRect.Height;
                if (portalItem.FragmentRect.Top > portalItem.AnchorRect.Top) rectified.Top -= portalItem.FragmentRect.Height;
            }

            if (rectified.IsOutsideTop)
            {
                rectified.Top = 0;
                if (portalItem.FragmentRect.Top < portalItem.AnchorRect.Top) rectified.Top += portalItem.FragmentRect.Height + portalItem.AnchorRect.Height;
            }

            if (rectified.IsOutsideLeft)
            {
                rectified.Left = 0;
            }

            if (rectified.IsOutsideRight)
            {
                rectified.Left =
                    portalItem.AnchorRect.WindowWidth
                    - portalItem.AnchorRect.Width - portalItem.FragmentRect.Width;
            }
            if (portalItem.AnchorRect.IsOutsideBottom || portalItem.AnchorRect.IsOutsideTop || portalItem.AnchorRect.IsOutsideLeft || portalItem.AnchorRect.IsOutsideRight)
            {
                portalItem.AnchorRect = rectified;
                return;
            }

            var FragmentIsAboveorBelowAnchor
                = portalItem.FragmentRect.Top > portalItem.AnchorRect.Bottom
                || portalItem.FragmentRect.Bottom < portalItem.AnchorRect.Top;

            // comes out at the bottom
            if (portalItem.FragmentRect.IsOutsideBottom)
            {
                rectified.Top -=
                   2 * (portalItem.FragmentRect.Top - portalItem.AnchorRect.Bottom)
                    + portalItem.AnchorRect.Height
                    + portalItem.FragmentRect.Height;
            }

            // comes out at the top
            if (portalItem.FragmentRect.IsOutsideTop)
            {
                rectified.Top +=
                    2 * (Math.Abs(portalItem.AnchorRect.Top - portalItem.FragmentRect.Bottom))
                  + portalItem.AnchorRect.Height
                  + portalItem.FragmentRect.Height;
            }

            // comes out at the left
            if (portalItem.FragmentRect.IsOutsideLeft)
            {
                rectified.Left +=
                     FragmentIsAboveorBelowAnchor
                        ? portalItem.AnchorRect.Left - portalItem.FragmentRect.Left
                        : 2 * (Math.Abs(portalItem.AnchorRect.Left - portalItem.FragmentRect.Right))
                            + portalItem.FragmentRect.Width
                            + portalItem.AnchorRect.Width;
            }

            // comes out at the right
            if (portalItem.FragmentRect.IsOutsideRight)
            {
                rectified.Left -=
                    FragmentIsAboveorBelowAnchor
                    ? portalItem.FragmentRect.Right - portalItem.AnchorRect.Right
                    : 2 * (Math.Abs(portalItem.FragmentRect.Left - portalItem.AnchorRect.Right))
                        + portalItem.FragmentRect.Width
                        + portalItem.AnchorRect.Width;
            }

            //in case that due the screen is so small that the menu can be outside from both sides
            // so the correction didn't work. We just put the fragment inside the screen
            portalItem.AnchorRect = rectified;
        }
    }
}
