using System;
using System.Collections.Generic;
using System.Text;
using MudBlazor.Theme.Defaults;

namespace MudBlazor
{
    public class MudTheme
    {
        public Breakpoints Breakpoints { get; set; }
        public Palette Palette { get; set; }
        public Shadow Shadow { get; set; }
        //public Typography Typography { get; set; }
        public LayoutProperties LayoutProperties { get; set; }
        public ZIndex ZIndex { get; set; }

        public MudTheme()
        {
            Breakpoints = new Breakpoints();
            Palette = new Palette();
            LayoutProperties = new LayoutProperties();
            Shadow = new Shadow();
            ZIndex = new ZIndex();
        }
    }
}
