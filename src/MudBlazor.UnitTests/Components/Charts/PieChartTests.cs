// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using MudBlazor.Charts;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    public class PieChartTests : BunitTest
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
        public void PieChartEmptyData()
        {
            var comp = Context.Render<Pie<double>>(parameters => parameters
                .Add(p => p.ChartSeries, null));

            comp.Markup.Should().Contain("mud-chart-pie");
            comp.Instance.ChartSeries.Should().BeNull();
        }

        [Theory]
        [TestCase(new double[] { 77, 25, 20, 5 })]
        [TestCase(new double[] { 77, 25, 20, 5, 8 })]
        public async Task PieChartExampleData(double[] data)
        {
            string[] labels = { "Uranium", "Plutonium", "Thorium", "Caesium", "Technetium", "Promethium",
                "Polonium", "Astatine", "Radon", "Francium", "Radium", "Actinium", "Protactinium",
                "Neptunium", "Americium", "Curium", "Berkelium", "Californium", "Einsteinium", "Mudblaznium" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Pie)
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = _baseChartPalette })
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "300px")
                .Add(p => p.ChartSeries, [data])
                .Add(p => p.ChartLabels, labels));

            comp.Markup.Should().Contain("class=\"mud-chart-pie mud-ltr\"");
            comp.Markup.Should().Contain("class=\"mud-chart-serie\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");

            if (data.Length <= 4)
            {
                comp.Markup.Should().
                    Contain("Uranium").And.Contain("Plutonium").And.Contain("Thorium").And.Contain("Caesium");
            }

            if (data.Length >= 5)
            {
                comp.Markup.Should()
                    .Contain("Technetium");
            }
            if (data.Length == 4 && data.Contains(77))
            {
                comp.Markup.Should()
                    .Contain("d=\"M 0 -140 A 140 140 0 1 1 -86.7071 109.9176 L 0 0 Z\"");
            }

            if (data.Length == 4 && data.Contains(5))
            {
                comp.Markup.Should()
                    .Contain("d=\"M -34.2796 -135.7384 A 140 140 0 0 1 -0 -140 L 0 0 Z\"");
            }

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _modifiedPalette }));

            comp.Markup.Should().Contain(_modifiedPalette[0]);
        }

        [Test]
        public async Task PieChartColoring()
        {
            double[] data = { 50, 25, 20, 5, 16, 14, 8, 4, 2, 8, 10, 19, 8, 17, 6, 11, 19, 24, 35, 13, 20, 12 };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Pie)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = new string[] { "#1E9AB0" } })
                .Add(p => p.ChartSeries, [data]));

            var paths1 = comp.FindAll("path");

            int count;
            count = paths1.Count(p => p.OuterHtml.Contains($"fill=\"{"#1E9AB0"}\""));
            count.Should().Be(22);

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _customPalette }));

            var paths2 = comp.FindAll("path");

            foreach (var color in _customPalette)
            {
                count = paths2.Count(p => p.OuterHtml.Contains($"fill=\"{color}\""));
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
        public void PieChart100Percent()
        {
            double[] data = { 50, 0, 0 };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Pie)
                .Add(p => p.ChartSeries, [data]));

            comp.Markup.Should().Contain("d=\"M 0 -140 A 140 140 0 1 1 0 140 A 140 140 0 1 1 -0 -140 L 0 0 Z\"");
        }

        [Test]
        public async Task PieChart_CanHideSeries()
        {
            var chartData = new double[] { 10, 20, 30, 40 };
            string[] chartLabels = { "Slice 1", "Slice 2", "Slice 3", "Slice 4" };
            var chartSeries = new List<ChartSeries<double>>() { new() { Data = chartData } };
            // For Pie charts, individual data points (slices) are hidden, not the whole series object usually.
            // The `ChartSeries.Visible` property might not apply here in the same way as for other charts if we want to hide slices.
            // Instead, the legend interaction directly controls visibility of slices.

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Pie)
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "300px")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, chartLabels)
                .Add(p => p.CanHideSeries, true) // This enables legend item clicking to hide/show
                .Add(p => p.ChartOptions, new PieChartOptions { ChartPalette = _baseChartPalette })
            );

            var seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
            seriesCheckboxes.Count.Should().Be(chartLabels.Length, "Number of checkboxes should match number of labels (slices)");

            var series1 = "[stroke='#2979FF']";
            var series2 = "[stroke='#1DE9B6']";
            var series3 = "[stroke='#FFC400']";
            var series4 = "[stroke='#FF9100']";

            string[] series = [series1, series2, series3, series4];

            // Initially, all slices should be visible and their checkboxes checked
            for (var i = 0; i < chartLabels.Length; i++)
            {
                seriesCheckboxes[i].IsChecked().Should().BeTrue($"{chartLabels[i]} checkbox should be initially checked");
                comp.FindAll($"path.mud-chart-serie{series[i]}").Count.Should().Be(1, $"{chartLabels[i]} path should be initially visible");
            }

            // Hide "Slice 1"
            await seriesCheckboxes[0].ChangeAsync(false);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeFalse("Slice 1 checkbox should be unchecked after hiding");
            comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(0, "Slice 1 path should be hidden");
            comp.FindAll($"path.mud-chart-serie{series2}").Count.Should().Be(1, "Slice 2 path should remain visible"); // Check other slices

            // Show "Slice 1" again
            await seriesCheckboxes[0].ChangeAsync(true);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeTrue("Slice 1 checkbox should be checked after re-showing");
            comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(1, "Slice 1 path should be visible again");

            // Hide "Slice 3"
            await seriesCheckboxes[2].ChangeAsync(false);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[2].IsChecked().Should().BeFalse("Slice 3 checkbox should be unchecked after hiding");
            comp.FindAll($"path.mud-chart-serie{series3}").Count.Should().Be(0, "Slice 3 path should be hidden");
            comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(1, "Slice 1 path should still be visible");
            comp.FindAll($"path.mud-chart-serie{series2}").Count.Should().Be(1, "Slice 2 path should still be visible");
            comp.FindAll($"path.mud-chart-serie{series4}").Count.Should().Be(1, "Slice 4 path should still be visible");

            // Show "Slice 3" again
            await seriesCheckboxes[2].ChangeAsync(true);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[2].IsChecked().Should().BeTrue("Slice 3 checkbox should be checked after re-showing");
            comp.FindAll($"path.mud-chart-serie{series3}").Count.Should().Be(1, "Slice 3 path should be visible again");
        }
    }
}
