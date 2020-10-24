using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using MudBlazor.Charts;

namespace MudBlazor
{
    public class MudChartBase : MudComponentBase
    {
        [Parameter] public string InputData { get; set; }

        [Parameter] public string InputLabels { get; set; }



        // Mud Stuff
        [Parameter] public ChartType ChartType { get; set; }

    }
    public enum ChartType
    {
        Donut,
        Bar,
        Line,
        Pie
    }
}
