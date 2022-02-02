// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MudBlazor.Charts;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;
using Bunit;

namespace MudBlazor.UnitTests.Charts
{
    public class ScatterChartTests: BunitTest
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
        public void ScatterChartEmptyData()
        {
            var comp = Context.RenderComponent<Scatter>();
            comp.Markup.Should().Contain("mud-chart-scatter");
        }

        [Theory]
        public void ScatterChartExampleData()
        {
            List<XYChartSeries> xychartSeries = new List<XYChartSeries>()
            {
                new XYChartSeries() { 
                    Name = "Series 1",
                    XData = new double[] { -20, 0, 40 },
                    YData = new double[] { -20, 0, 40 }
                },
                new XYChartSeries() {
                    Name = "Series 2",
                    XData = new double[] { 5, 100, 200 },
                    YData = new double[] { 5, 100, 200 }
                },
            };
            
            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Scatter)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.XYChartSeries, xychartSeries)
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = _baseChartPalette }));

            comp.Instance.XYChartSeries.Should().NotBeEmpty();
            
            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");
            
            if (xychartSeries.Count <= 3)
            {
                comp.Markup.Should().
                    Contain("Series 1").And.Contain("Series 2");
            }

            comp.Markup.Should().Contain("cx=\"97.04545454545453\" cy=\"290.9090909090909\" r=\"5\"");
            comp.Markup.Should().Contain("cx=\"351.8181818181818\" cy=\"161.36363636363635\" r=\"5\"");
            comp.Markup.Should().Contain("cx=\"620\" cy=\"25\" r=\"5\"");
            comp.Markup.Should().Contain("cx=\"30\" cy=\"325\" r=\"5\"");
            comp.Markup.Should().Contain("cx=\"83.63636363636364\" cy=\"297.72727272727275\" r=\"5\"");
            comp.Markup.Should().Contain("cx=\"190.9090909090909\" cy=\"243.1818181818182\" r=\"5\"");
            


            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.XYChartOptions, new XYChartOptions(){ChartPalette = _modifiedPalette}));

            comp.Markup.Should().Contain(_modifiedPalette[0]);
        }
    }
}
