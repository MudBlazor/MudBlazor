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

        public bool IsOutOfViewPort =>
            Bottom > WindowHeight
            || Top < 0
            || Left < 0
            || Right > WindowWidth;

        public void SetRectInsideViewPort()
        {

            if (WindowHeight - Bottom < 0) Bottom = 0;
            if (WindowWidth - Right < 0) Right = 0;
            if (Top < 0) Top = 0;
            if (Left < 0) Left = 0;

        }
    }

}

º
