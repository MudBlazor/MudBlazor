// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Bunit;
using FluentAssertions;
using MudBlazor.Charts;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    public class DonutChartTests : BunitTest
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


        private readonly string[] _customPalette =
        {
            "#015482", "#CC1512", "#FFE135", "#087830", "#D70040", "#B20931", "#202E54", "#F535AA", "#017B92",
            "#FA4224", "#062A78", "#56B4BE", "#207000", "#FF43A4", "#FB8989", "#5E9B8A", "#FFB7CE", "#C02B18",
            "#01153E", "#2EE8BB", "#EBDDE2"
        };

        [SetUp]
        public void Init()
        {

        }

        [Test]
        public void DonutChartEmptyData()
        {
            var comp = Context.RenderComponent<Donut>();
            comp.Markup.Should().Contain("mud-chart-donut");
        }

        [Test]
        [TestCase(new double[] { 50, 25, 20, 5 })]
        [TestCase(new double[] { 50, 25, 20, 5, 12 })]
        public void DonutChartExampleData(double[] data)
        {
            string[] labels = { "Fossil", "Nuclear", "Solar", "Wind", "Oil", "Coal", "Gas", "Biomass",
                "Hydro", "Geothermal", "Fossil", "Nuclear", "Solar", "Wind", "Oil",
                "Coal", "Gas", "Biomass", "Hydro", "Geothermal" };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Donut)
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "300px")
                .Add(p => p.InputData, data)
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = _baseChartPalette })
                .Add(p => p.InputLabels, labels));

            comp.Markup.Should().Contain("class=\"mud-chart-donut\"");
            comp.Markup.Should().Contain("class=\"mud-chart-serie mud-donut-segment\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");

            if (data.Length <= 4)
            {
                comp.Markup.Should().
                    Contain("Fossil").And.Contain("Nuclear").And.Contain("Solar").And.Contain("Wind");
            }

            if (data.Length >= 5)
            {
                comp.Markup.Should()
                    .Contain("Oil");
            }

            if (data.Length == 4 && data.Contains(50))
            {
                comp.Markup.Should()
                    .Contain("stroke-dasharray=\"50 50\" stroke-dashoffset=\"125\"");
            }

            if (data.Length == 4 && data.Contains(5))
            {
                comp.Markup.Should()
                    .Contain("stroke-dasharray=\"5 95\" stroke-dashoffset=\"30\"");
            }

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _modifiedPalette }));

            comp.Markup.Should().Contain(_modifiedPalette[0]);
        }

        [Test]
        [TestCase(new double[] { 50, 25, 20, 5 })]
        [TestCase(new double[] { 50, 25, 20, 5, 12 })]
        public void DonutCirclePosition(double[] data)
        {
            string[] labels = { "Fossil", "Nuclear", "Solar", "Wind", "Oil", "Coal", "Gas", "Biomass",
                "Hydro", "Geothermal", "Fossil", "Nuclear", "Solar", "Wind", "Oil",
                "Coal", "Gas", "Biomass", "Hydro", "Geothermal" };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Donut)
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "300px")
                .Add(p => p.InputData, data)
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = _baseChartPalette })
                .Add(p => p.InputLabels, labels));

            var svgViewBox = comp.Find("svg").GetAttribute("viewBox")?.Split(" ")?.Select(s => int.Parse(s))?.ToArray();
            var circles = comp.FindAll("circle");

            svgViewBox.Should().NotBeNullOrEmpty("must have a valid viewbox", svgViewBox);

            foreach (var c in circles)
            {
                var cx = int.Parse(c.GetAttribute("cx") ?? "0");
                var cy = int.Parse(c.GetAttribute("cy") ?? "0");

                cx.Should().Be(svgViewBox[2] / 2);

                cx.Should().Be(svgViewBox[3] / 2);
            }
        }

        [Test]
        public void DonutChartColoring()
        {
            double[] data = { 50, 25, 20, 5, 16, 14, 8, 4, 2, 8, 10, 19, 8, 17, 6, 11, 19, 24, 35, 13, 20, 12 };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Donut)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = new string[] { "#1E9AB0" } })
                .Add(p => p.InputData, data));

            var circles1 = comp.FindAll("circle");

            int count;
            count = circles1.Count(p => p.OuterHtml.Contains($"stroke=\"{"#1E9AB0"}\""));
            count.Should().Be(22);

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _customPalette }));

            var circles2 = comp.FindAll("circle");

            foreach (var color in _customPalette)
            {
                count = circles2.Count(p => p.OuterHtml.Contains($"stroke=\"{color}\""));
                if (color == _customPalette[0])
                {
                    count.Should().Be(2, because: "the number of data points defined exceeds the number of colors in the chart palette, thus, any new defined data point takes the color from the chart palette in the same fashion as the previous data points starting from the beginning");
                }
                else
                {
                    count.Should().Be(1);
                }
            }
        }
    }
}
