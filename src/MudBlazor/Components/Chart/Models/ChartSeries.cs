﻿namespace MudBlazor
{
    public class ChartSeries
    {
        public string Name { get; set; }

        public double[] Data { get; set; }

        public bool Visible { get; set; } = true;

        public int Index { get; set; }
    }
}
