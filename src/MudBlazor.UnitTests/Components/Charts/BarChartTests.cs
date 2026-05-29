// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Globalization;
using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;

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
        public void BarChartEmptyData()
        {
            var comp = Context.Render<Bar<double>>();
            comp.Markup.Should().Contain("mud-chart");
        }

        [Test]
        public async Task BarChartExampleData()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "United States", Data = new double[] { 40, 20, 25, 27, 46, 60, 48, 80, 15 } },
                new () { Name = "Germany", Data = new double[] { 19, 24, 35, 13, 28, 15, -4, 16, 31 } },
                new () { Name = "Sweden", Data = new double[] { 8, 6, -11, 13, 4, 16, 10, 16, 18 } },
            };
            string[] xAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Bar)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartOptions, new BarChartOptions { ChartPalette = _baseChartPalette, FixedBarWidth = 8 })
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels));

            comp.Instance.ChartSeries.Should().NotBeEmpty();

            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");

            // find legend
            var legend = comp.FindComponent<Legend<double>>();
            const string LEGEND_CSS_SELECTOR = "div.mud-chart-legend-item";
            legend.Should().NotBeNull(because: "we have a legend");
            legend.FindAll(LEGEND_CSS_SELECTOR).Should().HaveCount(chartSeries.Count, because: "the number series should match the legend item count");
            // click second item of legend (because SelectedIndex starts with 0)
            await legend.FindAll(LEGEND_CSS_SELECTOR).Skip(1).First().ClickAsync();
            comp.Instance.GetState(x => x.SelectedIndex).Should().Be(1, because: "second legend item was clicked");
            // click first item of legend (to check, if get's back to 0)
            await legend.FindAll(LEGEND_CSS_SELECTOR).Skip(0).First().ClickAsync();
            comp.Instance.GetState(x => x.SelectedIndex).Should().Be(0, because: "first legend item was clicked");

            if (chartSeries.Count <= 3)
            {
                comp.Markup.Should().
                    Contain("United States").And.Contain("Germany").And.Contain("Sweden");
            }

            var bars = comp.FindAll("path.mud-chart-bar");
            bars.Count.Should().Be(3 * 9, because: "3 series with 9 data points each");

            if (chartSeries.TryGetIndexOfDataValue(0, 40, out var index))
            {
                bars[index].OuterHtml.Should()
                    .Contain("d=\"M 34 261 L 34 143\"");
            }

            if (chartSeries.TryGetIndexOfDataValue(0, 80, out index))
            {
                bars[index].OuterHtml.Should()
                    .Contain("d=\"M 569.5 261 L 569.5 25\"");
            }

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _modifiedPalette }));

            comp.Markup.Should().Contain(_modifiedPalette[0]);
        }

        [Test]
        public async Task BarChartExampleSingleXAxis()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "United States", Data = new double[] { 40, 20, 25, 27, 46, 60, 48, 80, 15 } },
                new () { Name = "Germany", Data = new double[] { 19, 24, 35, 13, 28, 15, -4, 16, 31 } },
                new () { Name = "Sweden", Data = new double[] { 8, 6, -11, 13, 4, 16, 10, 16, 18 } },
            };
            string[] xAxisLabels = { "Jan" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Bar)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = _baseChartPalette })
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels));

            comp.Instance.ChartSeries.Should().NotBeEmpty();

            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");

            // find legend
            var legend = comp.FindComponent<Legend<double>>();
            const string LEGEND_CSS_SELECTOR = "div.mud-chart-legend-item";
            legend.Should().NotBeNull(because: "we have a legend");
            legend.FindAll(LEGEND_CSS_SELECTOR).Should().HaveCount(chartSeries.Count, because: "the number series should match the legend item count");

            if (chartSeries.Count <= 3)
            {
                comp.Markup.Should().
                    Contain("United States").And.Contain("Germany").And.Contain("Sweden");
            }

            var bars = comp.FindAll("path.mud-chart-bar");
            bars.Count.Should().Be(3 * 9, because: "3 series with 9 data points each");

            if (chartSeries.TryGetIndexOfDataValue(0, 40, out var index))
            {
                bars[index].OuterHtml.Should()
                    .Contain("d=\"M 34.183 261 L 34.183 143\"");
            }

            if (chartSeries.TryGetIndexOfDataValue(0, 80, out index))
            {
                bars[index].OuterHtml.Should()
                    .Contain("d=\"M 569.2941 261 L 569.2941 25\"");
            }

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _modifiedPalette }));

            comp.Markup.Should().Contain(_modifiedPalette[0]);
        }

        [Test]
        public void BarChartXAxisLabelRotation90UsesRotatedLabelSpacing()
        {
            var chartSeries = new List<ChartSeries<double>>
            {
                new() { Name = "Sales", Data = new double[] { 40, 20 } },
            };
            string[] xAxisLabels = { "January", "February" };

            var unrotated = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Bar)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new BarChartOptions { XAxisLabelRotation = 0 }));

            var rotated = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Bar)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new BarChartOptions { XAxisLabelRotation = 90 }));

            var unrotatedXAxisLabel = unrotated.Find("g.mud-charts-xaxis text");
            var rotatedXAxisLabel = rotated.Find("g.mud-charts-xaxis text");

            rotatedXAxisLabel.GetAttribute("text-anchor").Should().Be("end");
            rotatedXAxisLabel.GetAttribute("transform").Should().StartWith("rotate(-90 ");
            rotatedXAxisLabel.GetAttribute("y").Should().Be("320");

            var unrotatedLabelY = double.Parse(unrotatedXAxisLabel.GetAttribute("y")!, CultureInfo.InvariantCulture);
            var rotatedLabelY = double.Parse(rotatedXAxisLabel.GetAttribute("y")!, CultureInfo.InvariantCulture);
            rotatedLabelY.Should().BeLessThan(unrotatedLabelY, because: "rotated labels need a larger bottom offset from the chart edge");

            static double GetYCoordinate(string pathData)
            {
                var coordinates = pathData.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return double.Parse(coordinates[2], CultureInfo.InvariantCulture);
            }

            var unrotatedPlotBottom = GetYCoordinate(unrotated.Find("g.mud-charts-gridlines-yaxis path").GetAttribute("d")!);
            var rotatedPlotBottom = GetYCoordinate(rotated.Find("g.mud-charts-gridlines-yaxis path").GetAttribute("d")!);
            rotatedPlotBottom.Should().BeLessThan(unrotatedPlotBottom, because: "rotated labels need more bottom plot spacing");
        }

        [Test]
        public async Task BarChartColoring()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new ChartSeries<double>() { Name = "Deep Sea Blue", Data = new double[] { 40, 20, 25, 27, 46 } },
                new ChartSeries<double>() { Name = "Venetian Red", Data = new double[] { 19, 24, 35, 13, 28 } },
                new ChartSeries<double>() { Name = "Banana Yellow", Data = new double[] { 8, 6, 11, 13, 4 } },
                new ChartSeries<double>() { Name = "La Salle Green", Data = new double[] { 18, 9, 7, 10, 7 } },
                new ChartSeries<double>() { Name = "Rich Carmine", Data = new double[] { 9, 14, 6, 15, 20 } },
                new ChartSeries<double>() { Name = "Shiraz", Data = new double[] { 9, 4, 11, 5, 19 } },
                new ChartSeries<double>() { Name = "Cloud Burst", Data = new double[] { 14, 9, 20, 16, 6 } },
                new ChartSeries<double>() { Name = "Neon Pink", Data = new double[] { 14, 8, 4, 14, 8 } },
                new ChartSeries<double>() { Name = "Ocean", Data = new double[] { 11, 20, 13, 5, 5 } },
                new ChartSeries<double>() { Name = "Orangey Red", Data = new double[] { 6, 6, 19, 20, 6 } },
                new ChartSeries<double>() { Name = "Catalina Blue", Data = new double[] { 3, 2, 20, 3, 10 } },
                new ChartSeries<double>() { Name = "Fountain Blue", Data = new double[] { 3, 18, 11, 12, 3 } },
                new ChartSeries<double>() { Name = "Irish Green", Data = new double[] { 20, 5, 15, 16, 13 } },
                new ChartSeries<double>() { Name = "Wild Strawberry", Data = new double[] { 15, 9, 12, 12, 1 } },
                new ChartSeries<double>() { Name = "Geraldine", Data = new double[] { 5, 13, 19, 15, 8 } },
                new ChartSeries<double>() { Name = "Grey Teal", Data = new double[] { 12, 16, 20, 16, 17 } },
                new ChartSeries<double>() { Name = "Baby Pink", Data = new double[] { 1, 18, 10, 19, 8 } },
                new ChartSeries<double>() { Name = "Thunderbird", Data = new double[] { 15, 16, 10, 8, 5 } },
                new ChartSeries<double>() { Name = "Navy", Data = new double[] { 16, 2, 3, 5, 5 } },
                new ChartSeries<double>() { Name = "Aqua Marina", Data = new double[] { 17, 6, 11, 19, 6 } },
                new ChartSeries<double>() { Name = "Lavender Pinocchio", Data = new double[] { 1, 11, 4, 18, 1 } },
                new ChartSeries<double>() { Name = "Deep Sea Blue", Data = new double[] { 1, 11, 4, 18, 1 } }
            };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Bar)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = new string[] { "#1E9AB0" } })
                .Add(p => p.ChartSeries, chartSeries));

            var paths1 = comp.FindAll("path");

            int count;
            count = paths1.Count(p => p.OuterHtml.Contains($"fill=\"{"#1E9AB0"}\"") && p.OuterHtml.Contains($"stroke=\"{"#1E9AB0"}\""));
            count.Should().Be(5 * 22);

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _customPalette }));

            var paths2 = comp.FindAll("path");

            foreach (var color in _customPalette)
            {
                count = paths2.Count(p => p.OuterHtml.Contains($"fill=\"{color}\"") && p.OuterHtml.Contains($"stroke=\"{color}\""));
                if (color == _customPalette[0])
                {
                    count.Should().Be(5 * 2, because: "the number of series defined exceeds the number of colors in the chart palette, thus, any new defined series takes the color from the chart palette in the same fashion as the previous series starting from the beginning");
                }
                else
                {
                    count.Should().Be(5);
                }
            }
        }

        [Test]
        public async Task BarChart_CanHideSeries()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 90, 79, 72, 69, 62, 62, 55, 65, 70 } },
                new () { Name = "Series 2", Data = new double[] { 10, 41, 35, 51, 49, 62, 69, 91, 148 } },
                new () { Name = "Series 3", Data = new double[] { 10, 41, 35, 51, 49, 62, 69, 91, 148 }, Visible = false } // Initially hidden
            };
            string[] xAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Bar)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.CanHideSeries, true) // Enable hiding series
                .Add(p => p.ChartOptions, new BarChartOptions { ChartPalette = _baseChartPalette }));

            // Initial state assertions
            var seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
            seriesCheckboxes.Count.Should().Be(chartSeries.Count, "Number of checkboxes should match number of series");

            seriesCheckboxes[0].IsChecked().Should().BeTrue("Series 1 should be initially visible");
            seriesCheckboxes[1].IsChecked().Should().BeTrue("Series 2 should be initially visible");
            seriesCheckboxes[2].IsChecked().Should().BeFalse("Series 3 should be initially hidden as per Visible = false");

            var series1 = "[fill='#2979FF']";
            var series2 = "[fill='#1DE9B6']";
            var series3 = "[fill='#FFC400']";

            // Check initial state of Series 3 (should be 0 bars as Visible = false)
            comp.FindAll($"path.mud-chart-bar{series3}").Count.Should().Be(0, "Series 3 should have 0 bars initially");

            // Hide Series 1
            comp.FindAll($"path.mud-chart-bar{series1}").Count.Should().Be(chartSeries[0].Data.Values.Count, "Series 1 bars should be visible");
            await seriesCheckboxes[0].ChangeAsync(new ChangeEventArgs() { Value = false });
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeFalse("Series 1 checkbox should be unchecked after hiding");
            chartSeries[0].Visible.Should().BeFalse("Series 1 Visible property should be false");
            comp.FindAll($"path.mud-chart-bar{series1}").Count.Should().Be(0, "Series 1 bars should be hidden after unchecking");

            // Show Series 1 again
            await seriesCheckboxes[0].ChangeAsync(new ChangeEventArgs() { Value = true });
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeTrue("Series 1 checkbox should be checked after showing");
            chartSeries[0].Visible.Should().BeTrue("Series 1 Visible property should be true");

            comp.FindAll($"path.mud-chart-bar{series1}").Count.Should().Be(chartSeries[0].Data.Values.Count, "Series 1 bars should be visible again after re-checking");

            // Now check Series 2 (which should have been visible from the start)
            comp.FindAll($"path.mud-chart-bar{series2}").Count.Should().Be(chartSeries[1].Data.Values.Count, "Series 2 should have bars from the start");

            // Hide Series 2
            await seriesCheckboxes[1].ChangeAsync(new ChangeEventArgs() { Value = false });
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[1].IsChecked().Should().BeFalse("Series 2 checkbox should be unchecked after hiding");
            chartSeries[1].Visible.Should().BeFalse("Series 2 Visible property should be false");
            comp.FindAll($"path.mud-chart-bar{series2}").Count.Should().Be(0, "Series 2 bars should be hidden");
            comp.FindAll($"path.mud-chart-bar{series1}").Count.Should().Be(chartSeries[0].Data.Values.Count, "Series 1 bars should still be visible"); // Ensure other series not affected

            // Show Series 3 (which was initially hidden)
            await seriesCheckboxes[2].ChangeAsync(new ChangeEventArgs() { Value = true });
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[2].IsChecked().Should().BeTrue("Series 3 checkbox should be checked after showing");
            chartSeries[2].Visible.Should().BeTrue("Series 3 Visible property should be true");
            comp.FindAll($"path.mud-chart-bar{series3}").Count.Should().Be(chartSeries[2].Data.Values.Count, "Series 3 bars should be visible");
        }
    }
}
