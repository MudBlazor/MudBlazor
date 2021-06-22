using MudBlazor.Interop;

namespace MudBlazor
{
    public static class Viewport
    {
        public static bool IsOutOfView(BoundingClientRect rect) =>
            rect.Bottom > rect.WindowHeight
            || rect.Top < 0
            || rect.Left < 0
            || rect.Right > rect.WindowWidth;
    }
}

