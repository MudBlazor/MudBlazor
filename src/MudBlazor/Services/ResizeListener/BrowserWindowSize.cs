using System;

namespace MudBlazor.Services
{
    public class BrowserWindowSize : EventArgs
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
