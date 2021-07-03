using System;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor.Interop
{
    public class BoundingClientRect
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }

        /// <summary>
        /// height of the viewport
        /// </summary>
        public double WindowHeight { get; set; }

        /// <summary>
        /// width of the viewport
        /// </summary>
        public double WindowWidth { get; set; }

        /// <summary>
        /// the horizontal offset since the left of the page
        /// </summary>
        public double ScrollX { get; set; }

        /// <summary>
        /// the vertical offset since the top of the page
        /// </summary>
        public double ScrollY { get; set; }


        public double AbsoluteLeft => Left + ScrollX;

        public double AbsoluteTop => Top + ScrollY;

        public double AbsoluteRight => Right + ScrollX;

        public double AbsoluteBottom => Bottom + ScrollY;

        //check if the rect is outside of the viewport
        public bool IsOutsideBottom => Bottom > WindowHeight;

        public bool IsOutsideLeft => Left < 0;

        public bool IsOutsideTop => Top < 0;

        public bool IsOutsideRight => Right > WindowWidth;

    }

}

