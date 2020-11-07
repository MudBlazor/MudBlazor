using System;
using System.Collections.Generic;
using System.Text;
using MudBlazor.Theme.Defaults;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class DefaultTheme : MudTheme
    {
        public DefaultTheme()
        {
            Breakpoints = new Breakpoints();
            Palette = new Palette();
            LayoutProperties = new LayoutProperties();
            Shadow = new Shadow();
            ZIndex = new ZIndex();
        }
    }
}
