﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    public class PieBase : MudChartBase
    {
        [CascadingParameter] public MudChart MudChartParent { get; set; }

        public List<SvgPath> Paths = new List<SvgPath>();
        public List<SvgLegend> Legends = new List<SvgLegend>();

        protected override void OnParametersSet()
        {
            Paths.Clear();
            Legends.Clear();
            double startx, starty, endx, endy;
            var ndata = GetNormalizedData();
            double cumulativeRadians = 0;
            for (var i = 0; i < ndata.Length; i++)
            {
                var data = ndata[i];
                startx = Math.Cos(cumulativeRadians);
                starty = Math.Sin(cumulativeRadians);
                cumulativeRadians += 2 * Math.PI * data;
                endx = Math.Cos(cumulativeRadians);
                endy = Math.Sin(cumulativeRadians);
                var largeArcFlag = data > 0.5 ? 1 : 0;
                var path = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(startx)} {ToS(starty)} A 1 1 0 {ToS(largeArcFlag)} 1 {ToS(endx)} {ToS(endy)} L 0 0"
                };
                Paths.Add(path);
            }

            var counter = 0;
            foreach (var data in ndata)
            {
                var percent = data * 100;
                var labels = "";
                if (counter < InputLabels.Length)
                {
                    labels = InputLabels[counter];
                }
                var Legend = new SvgLegend()
                {
                    Index = counter,
                    Labels = labels,
                    Data = ToS(Math.Round(percent, 1))
                };
                Legends.Add(Legend);
                counter += 1;
            }
        }
    }
}
