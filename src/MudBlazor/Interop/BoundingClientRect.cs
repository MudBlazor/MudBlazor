namespace MudBlazor.Interop
{
    public class BoundingClientRect
    {
        public double Top { get; set; }
        public double Left { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }

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

        // computed properties, read only
        public double X => Left;
        public double Y => Top;
        public double Bottom => Top + Height;
        public double Right => Left + Width;

        public double AbsoluteLeft => Left + ScrollX;

        public double AbsoluteTop => Top + ScrollY;

        public double AbsoluteRight => Right + ScrollX;

        public double AbsoluteBottom => Bottom + ScrollY;

        //check if the rect is outside of the viewport
        public bool IsOutsideBottom => Bottom > WindowHeight;

        public bool IsOutsideLeft => Left < 0;

        public bool IsOutsideTop => Top < 0;

        public bool IsOutsideRight => Right > WindowWidth;

        public BoundingClientRect Clone()
        {
            return new BoundingClientRect
            {
                Left = Left,
                Top = Top,
                Width = Width,
                Height = Height,
                WindowHeight = WindowHeight,
                WindowWidth = WindowWidth,
                ScrollX = ScrollX,
                ScrollY = ScrollY
            };
        }
    }
    public static class BoundingClientRectExtensions
    {
        public static bool IsEqualTo(this BoundingClientRect sourceRect, BoundingClientRect targetRect)
        {
            if (sourceRect is null || targetRect is null) return false;
            return sourceRect.Top == targetRect.Top
                && sourceRect.Left == targetRect.Left
                && sourceRect.Width == targetRect.Width
                && sourceRect.Height == targetRect.Height
                && sourceRect.WindowHeight == targetRect.WindowHeight
                && sourceRect.WindowWidth == targetRect.WindowWidth
                && sourceRect.ScrollX == targetRect.ScrollX
                && sourceRect.ScrollY == targetRect.ScrollY;
        }
    }
}
