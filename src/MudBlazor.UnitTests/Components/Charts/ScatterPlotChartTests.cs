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

        [Test]
        public void ScatterPlotLineSeriesRendersPathNotCircles()
        {
            var scatterSeries = new ChartSeries<double> { Name = "Data", Data = Points((1, 2), (3, 4), (5, 6)) };
            var lineSeries = new ChartSeries<double> { Name = "Regression", Data = Points((1, 1.5), (5, 6.5)) };

            var series = new List<ChartSeries<double>> { scatterSeries, lineSeries };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                    SeriesDisplayOverrides = new Dictionary<IChartSeries, SeriesDisplayOverride>
                    {
                        [lineSeries] = new ScatterSeriesDisplayOverride { ScatterSeriesType = ScatterSeriesType.Line }
                    }
                }));

            // Only scatter series produces circles: 3 points × 2 circles each = 6
            comp.FindAll("circle.mud-chart-point").Count.Should().Be(6, because: "line series produces no scatter circles");

            // The line series produces an SVG path
            comp.FindAll("path.mud-chart-line").Count.Should().Be(1, because: "regression series renders as a line path");
        }

        [Test]
        public void ScatterPlotLineSeriesUsesCorrectPaletteColor()
        {
            var scatterSeries = new ChartSeries<double> { Name = "Data", Data = Points((1, 2)) };
            var lineSeries = new ChartSeries<double> { Name = "Regression", Data = Points((0, 0), (10, 10)) };

            var series = new List<ChartSeries<double>> { scatterSeries, lineSeries };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    SeriesDisplayOverrides = new Dictionary<IChartSeries, SeriesDisplayOverride>
                    {
                        [lineSeries] = new ScatterSeriesDisplayOverride { ScatterSeriesType = ScatterSeriesType.Line }
                    }
                }));

            // lineSeries is index 1 → _palette[1]
            var linePath = comp.Find("path.mud-chart-line");
            linePath.GetAttribute("stroke").Should().Be(_palette[1]);
        }

        [Test]
        public void ScatterPlotLineSeriesAppliesStrokeOpacityOverride()
        {
            var lineSeries = new ChartSeries<double> { Name = "Trend", Data = Points((0, 0), (10, 10)) };
            var series = new List<ChartSeries<double>> { lineSeries };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    SeriesDisplayOverrides = new Dictionary<IChartSeries, SeriesDisplayOverride>
                    {
                        [lineSeries] = new ScatterSeriesDisplayOverride
                        {
                            ScatterSeriesType = ScatterSeriesType.Line,
                            StrokeOpacity = 0.5
                        }
                    }
                }));

            var linePath = comp.Find("path.mud-chart-line");
            linePath.GetAttribute("stroke-opacity").Should().Be("0.5");
        }

        [Test]
        public void ScatterPlotLineSeriesAppliesLineStrokeWidth()
        {
            var lineSeries = new ChartSeries<double> { Name = "Trend", Data = Points((0, 0), (10, 10)) };
            var series = new List<ChartSeries<double>> { lineSeries };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    LineStrokeWidth = 2,
                    SeriesDisplayOverrides = new Dictionary<IChartSeries, SeriesDisplayOverride>
                    {
                        [lineSeries] = new ScatterSeriesDisplayOverride { ScatterSeriesType = ScatterSeriesType.Line }
                    }
                }));

            var linePath = comp.Find("path.mud-chart-line");
            linePath.GetAttribute("stroke-width").Should().Be("2");
        }

        [Test]
        public void ScatterPlotLineSeriesParticipatesInAxisScaling()
        {
            // Scatter data: x in [10, 50], line data extends x to 80 — axes must cover up to 80
            var scatterSeries = new ChartSeries<double> { Name = "Data", Data = Points((10, 5), (50, 5)) };
            var lineSeries = new ChartSeries<double> { Name = "Trend", Data = Points((10, 3), (80, 9)) };

            var series = new List<ChartSeries<double>> { scatterSeries, lineSeries };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    XAxisTicks = 20,
                    SeriesDisplayOverrides = new Dictionary<IChartSeries, SeriesDisplayOverride>
                    {
                        [lineSeries] = new ScatterSeriesDisplayOverride { ScatterSeriesType = ScatterSeriesType.Line }
                    }
                }));

            // X axis must contain 80 (from line series)
            comp.Find(".mud-charts-xaxis").InnerHtml.Should().Contain(">80<");
        }

        [Test]
        public void ScatterPlotLineSeriesAppearsInLegend()
        {
            var scatterSeries = new ChartSeries<double> { Name = "Data Points", Data = Points((1, 2)) };
            var lineSeries = new ChartSeries<double> { Name = "Regression Line", Data = Points((0, 0), (5, 5)) };

            var series = new List<ChartSeries<double>> { scatterSeries, lineSeries };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    SeriesDisplayOverrides = new Dictionary<IChartSeries, SeriesDisplayOverride>
                    {
                        [lineSeries] = new ScatterSeriesDisplayOverride { ScatterSeriesType = ScatterSeriesType.Line }
                    }
                }));

            comp.Markup.Should().Contain("Data Points");
            comp.Markup.Should().Contain("Regression Line");
        }

        [Test]
        public void ScatterPlotShowDataLabelsRendersTextElements()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((10, 20), (30, 40)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                    ShowDataLabels = true,
                }));

            // One text label per data point
            var labels = comp.FindAll("text.mud-chart-data-label");
            labels.Count.Should().Be(2, because: "one label per data point");
        }

        [Test]
        public void ScatterPlotShowDataLabelsContainsXAndYValues()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((10, 20)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                    ShowDataLabels = true,
                }));

            var label = comp.Find("text.mud-chart-data-label");
            label.TextContent.Should().Be("10, 20");
        }

        [Test]
        public void ScatterPlotShowDataLabelsFalseRendersNoTextElements()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((10, 20), (30, 40)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ShowToolTips = true,
                    ShowDataLabels = false,
                }));

            comp.FindAll("text.mud-chart-data-label").Count.Should().Be(0);
        }

        [Test]
        public void ScatterPlotShowDataLabelsWorksWithoutShowToolTips()
        {
            // ShowDataLabels and ShowToolTips are independent settings
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((10, 20), (30, 40)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ShowToolTips = false,
                    ShowDataLabels = true,
                }));

            comp.FindAll("text.mud-chart-data-label").Count.Should().Be(2,
                because: "data labels are independent of ShowToolTips");
        }

        [Test]
        public void ScatterPlotShowDataLabelsNotRenderedForLineSeries()
        {
            var lineSeries = new ChartSeries<double> { Name = "Trend", Data = Points((0, 0), (10, 10)) };
            var series = new List<ChartSeries<double>> { lineSeries };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                    ShowDataLabels = true,
                    SeriesDisplayOverrides = new Dictionary<IChartSeries, SeriesDisplayOverride>
                    {
                        [lineSeries] = new ScatterSeriesDisplayOverride { ScatterSeriesType = ScatterSeriesType.Line }
                    }
                }));

            // Line series is skipped in the scatter/label rendering pass
            comp.FindAll("text.mud-chart-data-label").Count.Should().Be(0,
                because: "line series does not produce scatter data labels");
        }

        [Test]
        public void ChartDataSinglePointConstructorCreatesSinglePoint()
        {
            var data = new ChartData<double>((3.0, 7.5));

            data.Points.Count.Should().Be(1);
            data.Points[0].X.Should().Be(3.0);
            data.Points[0].Y.Should().Be(7.5);
        }

        [Test]
        public void ChartDataSinglePointConstructorImplicitConversionFromArray()
        {
            ChartData<double> data = new (double, double)[] { (1.0, 2.0), (3.0, 4.0) };

            data.Points.Count.Should().Be(2);
            data.Points[0].X.Should().Be(1.0);
            data.Points[1].Y.Should().Be(4.0);
        }

        [Test]
        public void ChartDataSinglePointConstructorImplicitConversionFromList()
        {
            ChartData<double> data = new List<(double, double)> { (5.0, 6.0), (7.0, 8.0) };

            data.Points.Count.Should().Be(2);
            data.Points[0].X.Should().Be(5.0);
            data.Points[1].Y.Should().Be(8.0);
        }

        [Test]
        public void ScatterPlotChartOptionsDefaults()
        {
            var options = new ScatterPlotChartOptions();

            options.PointRadius.Should().Be(5);
            options.XAxisTicks.Should().Be(20);
            options.MaxNumXAxisTicks.Should().Be(20);
            options.XAxisFormat.Should().BeNull();
            options.ShowDataLabels.Should().BeFalse();
            options.TooltipTitleFormat.Should().Be("{{X_VALUE}}, {{Y_VALUE}}");
        }

        [Test]
        public void ScatterPlotChartOptionsImplicitConversionFromChartOptions()
        {
            var source = new ChartOptions
            {
                ShowLegend = true,
                ShowToolTips = true,
                TooltipTitleFormat = "X: {{X_VALUE}}",
                TooltipSubtitleFormat = "Y: {{Y_VALUE}}",
                ChartPalette = _palette,
            };

            ScatterPlotChartOptions options = source;

            options.ShowLegend.Should().BeTrue();
            options.ShowToolTips.Should().BeTrue();
            options.TooltipTitleFormat.Should().Be("X: {{X_VALUE}}");
            options.TooltipSubtitleFormat.Should().Be("Y: {{Y_VALUE}}");
            options.ChartPalette.Should().BeEquivalentTo(_palette);
        }

        // ── Y-axis scale ────────────────────────────────────────────────────────

        [Test]
        public void ScatterPlotYAxisTicksControlsGridInterval()
        {
            // Y range 0–40, YAxisTicks=10 → grid lines at 0,10,20,30,40
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, 0), (2, 40)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions { YAxisTicks = 10 }));

            var yAxis = comp.Find(".mud-charts-yaxis").InnerHtml;
            yAxis.Should().Contain(">10<");
            yAxis.Should().Contain(">20<");
            yAxis.Should().Contain(">30<");
            yAxis.Should().Contain(">40<");
        }

        [Test]
        public void ScatterPlotYAxisSuggestedMaxExtendsAxisWhenDataIsBelow()
        {
            // Data max Y=30, SuggestedMax=60 → axis must reach 60
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, 10), (2, 30)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    YAxisTicks = 20,
                    YAxisSuggestedMax = 60
                }));

            comp.Find(".mud-charts-yaxis").InnerHtml.Should().Contain(">60<");
        }

        [Test]
        public void ScatterPlotYAxisSuggestedMaxIgnoredWhenDataExceedsIt()
        {
            // Data max Y=80 exceeds SuggestedMax=60 → axis reaches 80, not capped at 60
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, 10), (2, 80)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    YAxisTicks = 20,
                    YAxisSuggestedMax = 60
                }));

            comp.Find(".mud-charts-yaxis").InnerHtml.Should().Contain(">80<");
        }

        [Test]
        public void ScatterPlotYAxisRequireZeroPointIncludesZeroWhenDataIsAllPositive()
        {
            // Data Y: 10–40, zero not in range → YAxisRequireZeroPoint adds it
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, 10), (2, 40)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    YAxisTicks = 10,
                    YAxisRequireZeroPoint = true
                }));

            comp.Find(".mud-charts-yaxis").InnerHtml.Should().Contain(">0<");
        }

        [Test]
        public void ScatterPlotYAxisRequireZeroPointIncludesZeroWhenDataIsAllNegative()
        {
            // Data Y: -40 to -10, zero not in range → YAxisRequireZeroPoint adds it as max
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, -40), (2, -10)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    YAxisTicks = 10,
                    YAxisRequireZeroPoint = true
                }));

            comp.Find(".mud-charts-yaxis").InnerHtml.Should().Contain(">0<");
        }

        [Test]
        public void ScatterPlotMaxNumYAxisTicksThinsOutGridWhenExceeded()
        {
            // Y range 0–100, YAxisTicks=1 → 101 lines normally
            // MaxNumYAxisTicks=5 forces gridYUnits to keep doubling until ≤5 lines fit
            // gridYUnits doubles: 1→2→4→8→16→32 until lines ≤5 → grid at multiples of 32
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, 0), (2, 100)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    YAxisTicks = 1,
                    MaxNumYAxisTicks = 5
                }));

            // With thinning, intermediate values like 1,2,3... must NOT all appear
            var yAxisHtml = comp.Find(".mud-charts-yaxis").InnerHtml;
            var tickCount = System.Text.RegularExpressions.Regex.Matches(yAxisHtml, @">\d+<").Count;
            tickCount.Should().BeLessThan(6);
        }

        // ── X-axis scale ────────────────────────────────────────────────────────

        [Test]
        public void ScatterPlotXAxisTicksControlsGridInterval()
        {
            // X range 0–40, XAxisTicks=10 → grid lines at 0,10,20,30,40
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((0, 1), (40, 2)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions { XAxisTicks = 10 }));

            var xAxis = comp.Find(".mud-charts-xaxis").InnerHtml;
            xAxis.Should().Contain(">10<");
            xAxis.Should().Contain(">20<");
            xAxis.Should().Contain(">30<");
            xAxis.Should().Contain(">40<");
        }

        [Test]
        public void ScatterPlotMaxNumXAxisTicksThinsOutGridWhenExceeded()
        {
            // X range 0–100, XAxisTicks=1 → 101 lines normally
            // MaxNumXAxisTicks=5 forces gridXUnits to keep doubling until ≤5 lines fit
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((0, 1), (100, 2)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    XAxisTicks = 1,
                    MaxNumXAxisTicks = 5
                }));

            var xAxisHtml = comp.Find(".mud-charts-xaxis").InnerHtml;
            var tickCount = System.Text.RegularExpressions.Regex.Matches(xAxisHtml, @">\d+<").Count;
            tickCount.Should().BeLessThan(6);
        }

        // ── Tooltips ─────────────────────────────────────────────────────────────

        [Test]
        public async Task ScatterPlotTooltipAppearsOnHover()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((10, 20)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                }));

            // Hoverable circle is the last circle rendered per point
            var hoverCircle = comp.FindAll("circle.mud-chart-point").Last();
            await hoverCircle.MouseOverAsync();

            comp.Find("g.svg-tooltip").Should().NotBeNull();
        }

        [Test]
        public async Task ScatterPlotTooltipDisappearsOnMouseOut()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((10, 20)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                }));

            await comp.FindAll("circle.mud-chart-point").Last().MouseOverAsync();
            comp.FindAll("g.svg-tooltip").Count.Should().Be(1);

            await comp.FindAll("circle.mud-chart-point").Last().MouseOutAsync();
            comp.FindAll("g.svg-tooltip").Count.Should().Be(0);
        }

        [Test]
        public async Task ScatterPlotTooltipShowsYValueByDefault()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((10, 20)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                }));

            var hoverCircle = comp.FindAll("circle.mud-chart-point").Last();
            await hoverCircle.MouseOverAsync();

            // Default TooltipTitleFormat is "{{X_VALUE}}, {{Y_VALUE}}"
            var tooltip = comp.Find("g.svg-tooltip");
            tooltip.InnerHtml.Should().Contain("10");
            tooltip.InnerHtml.Should().Contain("20");
        }

        [Test]
        public async Task ScatterPlotTooltipRespectsCustomTitleFormat()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((10, 20)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                    TooltipTitleFormat = "Y: {{Y_VALUE}}",
                }));

            await comp.FindAll("circle.mud-chart-point").Last().MouseOverAsync();

            var tooltipText = comp.Find("g.svg-tooltip tspan").TextContent;
            tooltipText.Should().Be("Y: 20");
            tooltipText.Should().NotContain("10");
        }

        [Test]
        public async Task ScatterPlotTooltipNotRenderedWhenHoveredPointNotFoundInChartDataPoints()
        {
            // This test covers the second null guard in RenderTooltip:
            //   if (dataPoint.Value is null) return;
            // Both HoveredDataPointPath (set via hover) and ChartDataPoints (populated by ShowToolTips)
            // are non-null, but after hovering the series is replaced with a completely different
            // dataset, so the stale HoveredDataPointPath no longer matches any entry in ChartDataPoints.
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, 2)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ChartPalette = _palette,
                    ShowToolTips = true,
                }));

            // Hover to populate HoveredDataPointPath
            var hoverCircle = comp.FindAll("circle.mud-chart-point").Last();
            await hoverCircle.MouseOverAsync();
            comp.FindAll("g.svg-tooltip").Count.Should().Be(1, because: "tooltip is visible after hover");

            // Replace the series with entirely different data — ChartDataPoints is rebuilt
            // with new SvgCircle instances that won't match the stale HoveredDataPointPath
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.ChartSeries, new List<ChartSeries<double>>
                {
                    new() { Name = "S1", Data = Points((99, 99)) }
                }));

            comp.FindAll("g.svg-tooltip").Count.Should().Be(0,
                because: "stale HoveredDataPointPath is no longer present in the rebuilt ChartDataPoints");
        }

        [Test]
        public void ScatterPlotNoTooltipWhenShowToolTipsFalse()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((10, 20)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions
                {
                    ShowToolTips = false,
                }));

            // No tooltip groups should be rendered when ShowToolTips is false
            comp.FindAll("g.svg-tooltip").Count.Should().Be(0);
            // Data points should still render as circles even when tooltips are disabled
            comp.FindAll("circle.mud-chart-point").Count.Should().Be(1);
        }

        [Test]
        public void ScatterPlotCreateInterpolatorThrowsNotSupportedException()
        {
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, 1), (2, 2), (3, 3)) }
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions()));

            var chart = comp.Instance;
            var act = () => chart.CreateInterpolator(0, 0, 1.0, 10.0, 10.0);

            act.Should().Throw<NotSupportedException>();
        }

        [Test]
        public void ScatterPlotDefaultSeriesTypeIsPoints()
        {
            // Without any SeriesDisplayOverride, all series render as scatter circles
            var series = new List<ChartSeries<double>>
            {
                new() { Name = "S1", Data = Points((1, 1), (2, 2)) },
                new() { Name = "S2", Data = Points((3, 3)) },
            };

            var comp = Context.Render<ScatterPlot<double>>(parameters => parameters
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions { ShowToolTips = true }));

            // No line paths rendered
            comp.FindAll("path.mud-chart-line").Count.Should().Be(0);
            // All points render as circles: (2+1) × 2 = 6
            comp.FindAll("circle.mud-chart-point").Count.Should().Be(6);
        }
    }
}
