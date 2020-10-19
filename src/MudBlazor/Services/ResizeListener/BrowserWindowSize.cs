using System;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor.Services
{
    public class BrowserWindowSize : EventArgs
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
