// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Reflection.Emit;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.Charts;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    public class StackedBarChartTests : BunitTest
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
            var comp = Context.RenderComponent<StackedBar>();
            comp.Markup.Should().Contain("mud-chart");
        }

        [Test]
        public void BarChartExampleData()
        {
            var chartSeries = new List<ChartSeries>()
            {
                new () { Name = "United States", Data = new double[] { 40, 20, 25, 27, 46, 60, 48, 80, 15 } },
                new () { Name = "Germany", Data = new double[] { 19, 24, 35, 13, 28, 15, 13, 16, 31 } },
                new () { Name = "Sweden", Data = new double[] { 8, 6, 11, 13, 4, 16, 10, 16, 18 } },
            };
            string[] xAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "650px")
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = _baseChartPalette })
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels));


            comp.Instance.ChartSeries.Should().NotBeEmpty();

            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");

            // find legend
            var legend = comp.FindComponent<Legend>();
            const string LEGEND_CSS_SELECTOR = "div.mud-chart-legend-item";
            legend.Should().NotBeNull(because: "we have a legend");
            legend.FindAll(LEGEND_CSS_SELECTOR).Should().HaveCount(chartSeries.Count, because: "the number series should match the legend item count");
            // click second item of legend (because SelectedIndex starts with 0)
            legend.FindAll(LEGEND_CSS_SELECTOR).Skip(1).First().Click();
            comp.Instance.GetState(x => x.SelectedIndex).Should().Be(1, because: "second legend item was clicked");
            // click first item of legend (to check, if get's back to 0)
            legend.FindAll(LEGEND_CSS_SELECTOR).Skip(0).First().Click();
            comp.Instance.GetState(x => x.SelectedIndex).Should().Be(0, because: "first legend item was clicked");

            if (chartSeries.Count <= 3)
            {
                comp.Markup.Should().
                    Contain("United States").And.Contain("Germany").And.Contain("Sweden");
            }

            comp.FindAll("path.mud-chart-bar").Count.Should().Be(3 * 9, because: "3 series with 9 data points each");


            comp.Markup.Should()
                .Contain("d=\"M 44 320 L 44 235.2143\"");

            comp.Markup.Should()
                .Contain("d=\"M 656 223.0714 L 656 184.6429\"");

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _modifiedPalette }));

            comp.Markup.Should().Contain(_modifiedPalette[0]);
        }

        [Test]
        public void StackedBarChartColoring()
        {
            var chartSeries = new List<ChartSeries>()
            {
                new ChartSeries() { Name = "Deep Sea Blue", Data = new double[] { 40, 20, 25, 27, 46 } },
                new ChartSeries() { Name = "Venetian Red", Data = new double[] { 19, 24, 35, 13, 28 } },
                new ChartSeries() { Name = "Banana Yellow", Data = new double[] { 8, 6, 11, 13, 4 } },
                new ChartSeries() { Name = "La Salle Green", Data = new double[] { 18, 9, 7, 10, 7 } },
                new ChartSeries() { Name = "Rich Carmine", Data = new double[] { 9, 14, 6, 15, 20 } },
                new ChartSeries() { Name = "Shiraz", Data = new double[] { 9, 4, 11, 5, 19 } },
                new ChartSeries() { Name = "Cloud Burst", Data = new double[] { 14, 9, 20, 16, 6 } },
                new ChartSeries() { Name = "Neon Pink", Data = new double[] { 14, 8, 4, 14, 8 } },
                new ChartSeries() { Name = "Ocean", Data = new double[] { 11, 20, 13, 5, 5 } },
                new ChartSeries() { Name = "Orangey Red", Data = new double[] { 6, 6, 19, 20, 6 } },
                new ChartSeries() { Name = "Catalina Blue", Data = new double[] { 3, 2, 20, 3, 10 } },
                new ChartSeries() { Name = "Fountain Blue", Data = new double[] { 3, 18, 11, 12, 3 } },
                new ChartSeries() { Name = "Irish Green", Data = new double[] { 20, 5, 15, 16, 13 } },
                new ChartSeries() { Name = "Wild Strawberry", Data = new double[] { 15, 9, 12, 12, 1 } },
                new ChartSeries() { Name = "Geraldine", Data = new double[] { 5, 13, 19, 15, 8 } },
                new ChartSeries() { Name = "Grey Teal", Data = new double[] { 12, 16, 20, 16, 17 } },
                new ChartSeries() { Name = "Baby Pink", Data = new double[] { 1, 18, 10, 19, 8 } },
                new ChartSeries() { Name = "Thunderbird", Data = new double[] { 15, 16, 10, 8, 5 } },
                new ChartSeries() { Name = "Navy", Data = new double[] { 16, 2, 3, 5, 5 } },
                new ChartSeries() { Name = "Aqua Marina", Data = new double[] { 17, 6, 11, 19, 6 } },
                new ChartSeries() { Name = "Lavender Pinocchio", Data = new double[] { 1, 11, 4, 18, 1 } },
                new ChartSeries() { Name = "Deep Sea Blue", Data = new double[] { 1, 11, 4, 18, 1 } }
            };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = new string[] { "#1E9AB0" } })
                .Add(p => p.ChartSeries, chartSeries));

            var paths1 = comp.FindAll("path");

            int count;
            count = paths1.Count(p => p.OuterHtml.Contains($"fill=\"none\"") && p.OuterHtml.Contains($"stroke=\"{"#1E9AB0"}\""));
            count.Should().Be(5 * 22);

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _customPalette }));

            var paths2 = comp.FindAll("path");

            foreach (var color in _customPalette)
            {
                count = paths2.Count(p => p.OuterHtml.Contains($"fill=\"none\"") && p.OuterHtml.Contains($"stroke=\"{color}\""));
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
        public void StackedBarChart_CanHideSeries_Test()
        {
            var chartSeries = new List<ChartSeries>()
            {
                new () { Name = "Series 1", Data = new double[] { 20, 30, 40 } },
                new () { Name = "Series 2", Data = new double[] { 10, 15, 20 } },
                new () { Name = "Series 3", Data = new double[] { 5, 10, 15 }, Visible = false } // Initially hidden
            };
            string[] xAxisLabels = { "Label A", "Label B", "Label C" };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.CanHideSeries, true)
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = _baseChartPalette }) // Using existing palette from the class
            );

            // Initial state assertions
            var seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
            seriesCheckboxes.Count.Should().Be(chartSeries.Count, "Number of checkboxes should match number of series");

            seriesCheckboxes[0].IsChecked().Should().BeTrue("Series 1 checkbox should be initially checked");
            seriesCheckboxes[1].IsChecked().Should().BeTrue("Series 2 checkbox should be initially checked");
            seriesCheckboxes[2].IsChecked().Should().BeFalse("Series 3 checkbox should be initially unchecked");

            var series1 = "[stroke='#2979FF']";
            var series2 = "[stroke='#1DE9B6']";
            var series3 = "[stroke='#FFC400']";

            comp.FindAll($"path.mud-chart-bar{series1}").Count.Should().Be(chartSeries[0].Data.Values.Length, "Series 1 should have its bar segments visible initially");
            comp.FindAll($"path.mud-chart-bar{series2}").Count.Should().Be(chartSeries[1].Data.Values.Length, "Series 2 should have its bar segments visible initially");
            comp.FindAll($"path.mud-chart-bar{series3}").Count.Should().Be(0, "Series 3 should have no bar segments visible initially");

            // Hide Series 1
            comp.InvokeAsync(() => seriesCheckboxes[0].Change(false));
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeFalse("Series 1 checkbox should be unchecked after hiding");
            chartSeries[0].Visible.Should().BeFalse("Series 1 Visible property should be false after hiding");
            comp.FindAll($"path.mud-chart-bar{series1}").Count.Should().Be(0, "Series 1 bar segments should be hidden");

            // Show Series 1 again
            comp.InvokeAsync(() => seriesCheckboxes[0].Change(true));
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeTrue("Series 1 checkbox should be checked after re-showing");
            chartSeries[0].Visible.Should().BeTrue("Series 1 Visible property should be true after re-showing");
            comp.FindAll($"path.mud-chart-bar{series1}").Count.Should().Be(chartSeries[0].Data.Values.Length, "Series 1 bar segments should be visible again");

            // Hide Series 2
            comp.InvokeAsync(() => seriesCheckboxes[1].Change(false));
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[1].IsChecked().Should().BeFalse("Series 2 checkbox should be unchecked after hiding");
            chartSeries[1].Visible.Should().BeFalse("Series 2 Visible property should be false after hiding");
            comp.FindAll($"path.mud-chart-bar{series2}").Count.Should().Be(0, "Series 2 bar segments should be hidden");
            comp.FindAll($"path.mud-chart-bar{series1}").Count.Should().Be(chartSeries[0].Data.Values.Length, "Series 1 bar segments should remain visible");

            // Show Series 3
            comp.InvokeAsync(() => seriesCheckboxes[2].Change(true));
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[2].IsChecked().Should().BeTrue("Series 3 checkbox should be checked after showing");
            chartSeries[2].Visible.Should().BeTrue("Series 3 Visible property should be true after showing");
            comp.FindAll($"path.mud-chart-bar{series3}").Count.Should().Be(chartSeries[2].Data.Values.Length, "Series 3 bar segments should be visible");
        }
    }
}
