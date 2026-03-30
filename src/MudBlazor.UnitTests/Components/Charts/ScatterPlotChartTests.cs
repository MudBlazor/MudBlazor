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
    public class ScatterPlotChartTests : BunitTest
    {
        private readonly string[] _palette = { "#2979FF", "#1DE9B6", "#FFC400" };

        private static ChartData<double> Points(params (double x, double y)[] pts) => new(pts);

        [SetUp]
        public void Init() { }

        [Test]
        public void ScatterPlotEmptyData()
        {
            var comp = Context.Render<ScatterPlot<double>>();
            comp.Markup.Should().Contain("mud-chart");
        }

        [Test]
        public void ScatterPlotRendersAxesAndLegend()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = Points((10, 20), (30, 40), (50, 10)) }
            };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.ScatterPlot)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions { ChartPalette = _palette }));

            comp.Markup.Should().Contain("mud-chart-scatter");
            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");
            comp.Markup.Should().Contain("Series 1");
        }

        [Test]
        public void ScatterPlotRendersCirclesForEachDataPoint()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = Points((10, 20), (30, 40), (50, 10)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                }));

            // Each data point produces one visible circle + one hoverable circle
            var circles = comp.FindAll("circle.mud-chart-point");
            circles.Count.Should().Be(6, because: "3 data points × 2 circles each (visible + hoverable)");
        }

        [Test]
        public void ScatterPlotShowsCorrectXAxisLabels()
        {
            // X range 10–50, XAxisTicks=20 → grid lines at 0, 20, 40, 60
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((10, 5), (50, 5)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions { XAxisTicks = 20 }));

            var xAxis = comp.Find(".mud-charts-xaxis");
            xAxis.InnerHtml.Should().Contain(">0<");
            xAxis.InnerHtml.Should().Contain(">20<");
            xAxis.InnerHtml.Should().Contain(">40<");
            xAxis.InnerHtml.Should().Contain(">60<");
        }

        [Test]
        public void ScatterPlotShowsCorrectYAxisLabels()
        {
            // Y range 10–40, default YAxisTicks=20 → grid lines at 0, 20, 40
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, 10), (2, 40)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions()));

            var yAxis = comp.Find(".mud-charts-yaxis");
            yAxis.InnerHtml.Should().Contain(">0<");
            yAxis.InnerHtml.Should().Contain(">20<");
            yAxis.InnerHtml.Should().Contain(">40<");
        }

        [Test]
        public void ScatterPlotAppliesColorPaletteToCircles()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = Points((1, 1)) },
                new() { Name = "Series 2", Data = Points((2, 2)) },
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                }));

            comp.Markup.Should().Contain(_palette[0]);
            comp.Markup.Should().Contain(_palette[1]);
        }

        [Test]
        public void ScatterPlotColorPaletteWrapsAround()
        {
            // 4 series, 3-color palette → S4 reuses _palette[0]
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, 1)) },
                new() { Name = "S2", Data = Points((2, 2)) },
                new() { Name = "S3", Data = Points((3, 3)) },
                new() { Name = "S4", Data = Points((4, 4)) },
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                }));

            var circles = comp.FindAll($"circle[fill='{_palette[0]}']");
            circles.Count.Should().Be(2, because: "Series 1 and Series 4 wrap around to the same palette color");
        }

        [Test]
        public async Task ScatterPlotCanHideAndShowSeries()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Series 1", Data = Points((10, 10), (20, 20)) },
                new() { Name = "Series 2", Data = Points((15, 15), (25, 25)) },
            };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.ScatterPlot)
                .Add(p => p.ChartSeries, series)
                .Add(p => p.CanHideSeries, true)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                }));

            var checkboxes = comp.FindAll(".mud-checkbox-input");
            checkboxes.Count.Should().Be(2);
            checkboxes[0].IsChecked().Should().BeTrue("Series 1 initially visible");
            checkboxes[1].IsChecked().Should().BeTrue("Series 2 initially visible");

            // 2 series × 2 points × 2 circles = 8
            comp.FindAll("circle.mud-chart-point").Count.Should().Be(8);

            // Hide Series 1
            await checkboxes[0].ChangeAsync(false);
            series[0].Visible.Should().BeFalse();
            comp.FindAll("circle.mud-chart-point").Count.Should().Be(4, because: "only Series 2 remains");

            // Show Series 1 again
            checkboxes = comp.FindAll(".mud-checkbox-input");
            await checkboxes[0].ChangeAsync(true);
            series[0].Visible.Should().BeTrue();
            comp.FindAll("circle.mud-chart-point").Count.Should().Be(8);
        }

        [Test]
        public void ScatterPlotHandlesNegativeCoordinates()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((-50, -30), (0, 0), (50, 30)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions { ShowToolTips = true }));

            comp.Markup.Should().Contain("mud-chart-scatter");
            comp.FindAll("circle.mud-chart-point").Count.Should().Be(6);

            comp.Find(".mud-charts-xaxis").InnerHtml.Should().Contain("-");
            comp.Find(".mud-charts-yaxis").InnerHtml.Should().Contain("-");
        }

        [Test]
        public void ScatterPlotMultipleSeriesRenderAllPoints()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Group A", Data = Points((1, 2), (3, 4), (5, 6)) },
                new() { Name = "Group B", Data = Points((2, 1), (4, 3)) },
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions { ShowToolTips = true }));

            // (3 + 2) points × 2 circles = 10
            comp.FindAll("circle.mud-chart-point").Count.Should().Be(10);
            comp.Markup.Should().Contain("Group A");
            comp.Markup.Should().Contain("Group B");
        }

        [Test]
        public async Task ScatterPlotUpdateSeriesRebuildsChart()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, 1)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions { ShowToolTips = true }));

            comp.FindAll("circle.mud-chart-point").Count.Should().Be(2);

            var extended = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, 1), (2, 2), (3, 3)) }
            };

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.ChartSeries, extended));

            comp.FindAll("circle.mud-chart-point").Count.Should().Be(6);
        }

        [Test]
        public void ScatterPlotCustomPointRadiusApplied()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((5, 5)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    PointRadius = 8,
                    ShowToolTips = true,
                }));

            comp.Markup.Should().Contain("r=\"8\"");
        }

        [Test]
        public void ScatterPlotInitiallyHiddenSeriesNotRendered()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "Visible", Data = Points((1, 1), (2, 2)) },
                new() { Name = "Hidden",  Data = Points((3, 3)), Visible = false },
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions { ShowToolTips = true }));

            // Only Visible series: 2 points × 2 circles = 4
            comp.FindAll("circle.mud-chart-point").Count.Should().Be(4);
        }

        [Test]
        public void ScatterPlotXAxisFormatApplied()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((10, 5), (20, 5)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    XAxisTicks = 10,
                    XAxisFormat = "F1",
                }));

            // F1 format adds a decimal digit; the separator is locale-specific ("." or ",")
            var xAxisHtml = comp.Find(".mud-charts-xaxis").InnerHtml;
            (xAxisHtml.Contains(".0") || xAxisHtml.Contains(",0")).Should().BeTrue("F1 format should produce a decimal in the X axis labels");
        }
    }
}
