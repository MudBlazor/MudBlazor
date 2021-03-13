// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Utilities;

namespace MudBlazor.UnitTests.Components.EnchancedChart
{
    public static class MudEnchancedChartTestHelper
    {
        public static String ToCssColor(this String input) => ("#" + (String)new CssColor(input)).ToLower();
    }
}
