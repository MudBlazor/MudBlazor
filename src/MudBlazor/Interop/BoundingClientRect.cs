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

        public double WindowHeight { get; set; }
        public double WindowWidth { get; set; }

        public double ScrollX { get; set; }
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


        public void SetRectInsideViewPort()
        {

            if (IsOutsideBottom)
            {
                Bottom = WindowHeight;
                Top = Bottom - Height;//top must be corrected
            }

            if (IsOutsideRight)
            {
                Right = WindowWidth;
                Left = Right - Width; //left must be corrected
            }

            if (IsOutsideTop)
            {
                Top = 0;
                Bottom = Height;
            }

            if (IsOutsideLeft)
            {
                Left = 0;
                Right = Width;
            }

        }
    }

}

