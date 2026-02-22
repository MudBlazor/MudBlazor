// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
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
            var comp = Context.Render<Donut<double>>();
            comp.Markup.Should().Contain("mud-chart-donut");
        }

        [Test]
        [TestCase(new double[] { 50, 25, 20, 5 })]
        [TestCase(new double[] { 50, 25, 20, 5, 12 })]
        public async Task DonutChartExampleData(double[] data)
        {
            string[] labels = { "Fossil", "Nuclear", "Solar", "Wind", "Oil", "Coal", "Gas", "Biomass",
                "Hydro", "Geothermal", "Fossil", "Nuclear", "Solar", "Wind", "Oil",
                "Coal", "Gas", "Biomass", "Hydro", "Geothermal" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Donut)
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "300px")
                .Add(p => p.ChartSeries, [data])
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = _baseChartPalette })
                .Add(p => p.ChartLabels, labels));

            comp.Markup.Should().Contain("class=\"mud-chart-donut mud-ltr\"");
            comp.Markup.Should().Contain("class=\"mud-chart-serie\"");
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
                    .ContainEquivalentOf("fill=\"#2979FF\" d=\"M 0 -140 A 140 140 0 0 1 0 140 L 0 105 A 105 105 0 0 0 0 -105 Z\"");
            }

            if (data.Length == 4 && data.Contains(5))
            {
                comp.Markup.Should()
                    .ContainEquivalentOf("fill=\"#FF9100\" d=\"M -43.2624 -133.1479 A 140 140 0 0 1 -0 -140 L -0 -105 A 105 105 0 0 0 -32.4468 -99.8609 Z\"");
            }

            await comp.SetParametersAndRenderAsync(parameters => parameters
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

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Donut)
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "300px")
                .Add(p => p.ChartSeries, [data])
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = _baseChartPalette })
                .Add(p => p.ChartLabels, labels));

            var svgViewBox = comp.Find("svg").GetAttribute("viewBox")?.Split(" ")?.Select(s => int.Parse(s))?.ToArray();
            var circles = comp.FindAll("circle");

            svgViewBox.Should().NotBeNullOrEmpty("must have a valid viewbox", svgViewBox);

            foreach (var c in circles)
            {
                var cx = int.Parse(c.GetAttribute("cx") ?? "0");
                var cy = int.Parse(c.GetAttribute("cy") ?? "0");

                cx.Should().Be(0);

                cy.Should().Be(0);
            }
        }

        [Test]
        public async Task DonutChartColoring()
        {
            double[] data = { 50, 25, 20, 5, 16, 14, 8, 4, 2, 8, 10, 19, 8, 17, 6, 11, 19, 24, 35, 13, 20, 12 };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Donut)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = new string[] { "#1E9AB0" } })
                .Add(p => p.ChartSeries, [data]));

            var circles1 = comp.FindAll("path");

            int count;
            count = circles1.Count(p => p.OuterHtml.Contains($"fill=\"{"#1E9AB0"}\""));
            count.Should().Be(22);

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _customPalette }));

            var circles2 = comp.FindAll("path");

            foreach (var color in _customPalette)
            {
                count = circles2.Count(p => p.OuterHtml.Contains($"fill=\"{color}\""));
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

        [Test]
        public void DonutChart100Percent()
        {
            double[] data = { 50, 0, 0 };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Donut)
                .Add(p => p.ChartSeries, [data]));

            comp.Markup.Should().Contain("d=\"M 0 -140 A 140 140 0 1 1 0 140 A 140 140 0 1 1 -0 -140 L -0 -105 A 105 105 0 1 0 0 105 A 105 105 0 1 0 0 -105 Z\"");
        }

        [Test]
        public async Task DonutChart_CanHideSeries()
        {
            var chartData = new double[] { 25, 35, 15, 25 };
            string[] chartLabels = { "Area A", "Area B", "Area C", "Area D" };
            var chartSeries = new List<ChartSeries<double>>() { new() { Data = chartData } };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Donut)
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "300px")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, chartLabels)
                .Add(p => p.CanHideSeries, true)
                .Add(p => p.ChartOptions, new DonutChartOptions { ChartPalette = _baseChartPalette })
            );

            var seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
            seriesCheckboxes.Count.Should().Be(chartLabels.Length, "Number of checkboxes should match number of labels (segments)");

            var series1 = "[stroke='#2979FF']";
            var series2 = "[stroke='#1DE9B6']";
            var series3 = "[stroke='#FFC400']";
            var series4 = "[stroke='#FF9100']";

            string[] series = [series1, series2, series3, series4];

            // Initially, all segments should be visible and their checkboxes checked
            for (var i = 0; i < chartLabels.Length; i++)
            {
                seriesCheckboxes[i].IsChecked().Should().BeTrue($"{chartLabels[i]} checkbox should be initially checked");
                comp.FindAll($"path.mud-chart-serie{series[i]}").Count.Should().Be(1, $"{chartLabels[i]} path should be initially visible");
            }

            // Hide "Area A"
            await seriesCheckboxes[0].ChangeAsync(false);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeFalse("Area A checkbox should be unchecked after hiding");
            comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(0, "Area A path should be hidden");
            comp.FindAll($"path.mud-chart-serie{series2}").Count.Should().Be(1, "Area B path should remain visible");

            // Show "Area A" again
            await seriesCheckboxes[0].ChangeAsync(true);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeTrue("Area A checkbox should be checked after re-showing");
            comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(1, "Area A path should be visible again");

            // Hide "Area C"
            await seriesCheckboxes[2].ChangeAsync(false);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[2].IsChecked().Should().BeFalse("Area C checkbox should be unchecked after hiding");
            comp.FindAll($"path.mud-chart-serie{series3}").Count.Should().Be(0, "Area C path should be hidden");
            comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(1, "Area A path should still be visible");
            comp.FindAll($"path.mud-chart-serie{series2}").Count.Should().Be(1, "Area B path should still be visible");
            comp.FindAll($"path.mud-chart-serie{series4}").Count.Should().Be(1, "Area D path should still be visible");

            // Show "Area C" again
            await seriesCheckboxes[2].ChangeAsync(true);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[2].IsChecked().Should().BeTrue("Area C checkbox should be checked after re-showing");
            comp.FindAll($"path.mud-chart-serie{series3}").Count.Should().Be(1, "Area C path should be visible again");
        }
    }
}
