﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using MudBlazor.UnitTests.Components;
using System.Linq;
using FluentAssertions;
using MudBlazor.Charts;
using NUnit.Framework;
using Bunit;

namespace MudBlazor.UnitTests.Charts 
{
    public class BarChartTests : BunitTest
    { 
        private readonly string[] _baseChartPalette = 
        {
            "#2979FF", "#1DE9B6", "#FFC400", "#FF9100", "#651FFF", "#00E676", "#00B0FF", "#26A69A", "#FFCA28",
            "#FFA726", "#EF5350", "#EF5350", "#7E57C2", "#66BB6A", "#29B6F6", "#FFA000", "#F57C00", "#D32F2F",
            "#512DA8", "#616161"
        };
        
        private readonly string[] _modifiedPalette =
        {
            "#264653", "#2a9d8f", "#e9c46a", "#f4a261", "#e76f51"
        };
        
        [SetUp]
        public void Init()
        {
 
        }

        [Test]
        public void BarChartEmptyData()
        {
            var comp = Context.RenderComponent<Bar>();
            comp.Markup.Should().Contain("mud-chart");
        }

        [Test]
        public void BarChartExampleData()
        {
            List<ChartSeries> chartSeries = new List<ChartSeries>()
            {
                new () { Name = "United States", Data = new double[] { 40, 20, 25, 27, 46, 60, 48, 80, 15 } },
                new () { Name = "Germany", Data = new double[] { 19, 24, 35, 13, 28, 15, 13, 16, 31 } },
                new () { Name = "Sweden", Data = new double[] { 8, 6, 11, 13, 4, 16, 10, 16, 18 } },
            };
            string[] xAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };
            
            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Bar)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartOptions, new ChartOptions {ChartPalette = _baseChartPalette})
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.XAxisLabels, xAxisLabels));

            comp.Instance.ChartSeries.Should().NotBeEmpty();
            
            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");
            
            if (chartSeries.Count <= 3)
            {
                comp.Markup.Should().
                    Contain("United States").And.Contain("Germany").And.Contain("Sweden");
            }
            
            if (chartSeries.Count == 3 && chartSeries.Any(x => x.Data.Contains(40)))
            {
                comp.Markup.Should()
                    .Contain("d=\"M 30 325 L 30 205\"");
            }

            if (chartSeries.Count == 3 && chartSeries.Any(x => x.Data.Contains(80)))
            {
                comp.Markup.Should()
                    .Contain("d=\"M 546.25 325 L 546.25 85\"");
            }
            
            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions(){ChartPalette = _modifiedPalette}));

            comp.Markup.Should().Contain(_modifiedPalette[0]);
        }
    }
}
