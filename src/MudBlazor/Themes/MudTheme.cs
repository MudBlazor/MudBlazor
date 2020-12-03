using System;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor
{
    public class MudTheme
    {
        //public Breakpoints Breakpoints { get; set; }
        public Palette Palette { get; set; }
        public Shadow Shadows { get; set; }
        //public Typography Typography { get; set; }
        public LayoutProperties LayoutProperties { get; set; }
        public ZIndex ZIndex { get; set; }

        public MudTheme()
        {
            Palette = new Palette();
            LayoutProperties = new LayoutProperties();
            Shadows = new Shadow();
            ZIndex = new ZIndex();
        }
    }
}

namespace MudBlazor.Theme.Defaults
{
    //Old added so it dosent break projects on update durin minor
}
