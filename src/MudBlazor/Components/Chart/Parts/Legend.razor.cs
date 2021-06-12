﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    public partial class Legend : MudChartBase
    {
        [CascadingParameter] public MudChart MudChartParent { get; set; }
        [Parameter] public List<SvgLegend> Data { get; set; } = new List<SvgLegend>();
    }
}
