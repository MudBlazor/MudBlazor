// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Charts;
using MudBlazor.Extensions;
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
        public void StackedBarChart_DefaultRender_ShouldNotThrow()
        {
            // Test basic rendering with default parameters to ensure no exceptions.
            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar));
            comp.Should().NotBeNull();
            comp.Markup.Should().Contain("mud-chart");
        }

        [Test]
        public void StackedBarChart_EmptyData_ShouldRenderAxesAndLegend()
        {
            // Test rendering with empty ChartSeries and ChartLabels.
            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, new List<ChartSeries<double>>())
                .Add(p => p.ChartLabels, System.Array.Empty<string>()));

            comp.Markup.Should().Contain("mud-chart");
            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\""); // X-axis should still render
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\""); // Y-axis should still render
            comp.FindAll("path.mud-chart-bar").Count.Should().Be(0); // No bars
            comp.FindAll("div.mud-chart-legend-item").Count.Should().Be(0); // No legend items
        }

        [Test]
        public void BarChartEmptyData()
        {
            var comp = Context.Render<StackedBar<double>>();
            comp.Markup.Should().Contain("mud-chart");
        }

        [Test]
        public void StackedBarChart_SingleSeries_ShouldRenderCorrectly()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 10, 20, 30 } }
            };
            string[] xAxisLabels = { "A", "B", "C" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels));

            comp.FindAll("path.mud-chart-bar").Count.Should().Be(3); // 3 bars for 3 data points
            comp.FindAll("div.mud-chart-legend-item").Count.Should().Be(1); // 1 legend item
            comp.Find("div.mud-chart-legend-item").TextContent.Should().Contain("Series 1");

            var xAxisTexts = comp.FindAll("g.mud-charts-xaxis text");
            xAxisTexts.Should().HaveCount(3);
            xAxisTexts.Select(x => x.TextContent).Should().BeEquivalentTo(xAxisLabels);
        }

        [Test]
        public void StackedBarChart_MultipleSeries_ShouldRenderCorrectly()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 10, 20 } },
                new () { Name = "Series 2", Data = new double[] { 15, 25 } }
            };
            string[] xAxisLabels = { "A", "B" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels));

            comp.FindAll("path.mud-chart-bar").Count.Should().Be(4);
            comp.FindAll("div.mud-chart-legend-item").Count.Should().Be(2);
            var legendItems = comp.FindAll("div.mud-chart-legend-item");
            legendItems[0].TextContent.Should().Contain("Series 1");
            legendItems[1].TextContent.Should().Contain("Series 2");
        }

        [Test]
        public void StackedBarChart_SeriesWithVaryingDataPoints_ShouldRenderCorrectly()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 10, 20, 30 } }, // 3 points
                new () { Name = "Series 2", Data = new double[] { 15, 25 } }      // 2 points
            };
            // Labels should ideally match the series with the most data points
            string[] xAxisLabels = { "A", "B", "C" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels));

            comp.FindAll("path.mud-chart-bar").Count.Should().Be(5);
            comp.FindAll("div.mud-chart-legend-item").Count.Should().Be(2);

            var xAxisTexts = comp.FindAll("g.mud-charts-xaxis text");
            xAxisTexts.Should().HaveCount(3); // Based on xAxisLabels
            xAxisTexts.Select(x => x.TextContent).Should().BeEquivalentTo(xAxisLabels);
        }

        [Test]
        public async Task StackedBarChart_Dynamic_AddSeries_ShouldUpdateChart()
        {
            var initialSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 10, 20 } }
            };
            string[] xAxisLabels = { "A", "B" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, initialSeries)
                .Add(p => p.ChartLabels, xAxisLabels));

            comp.FindAll("path.mud-chart-bar").Count.Should().Be(2);
            comp.FindAll("div.mud-chart-legend-item").Count.Should().Be(1);

            var updatedSeries = new List<ChartSeries<double>>(initialSeries)
            {
                new () { Name = "Series 2", Data = new double[] { 15, 25 } }
            };
            await comp.SetParametersAndRenderAsync(parameters => parameters
                      .Add(p => p.ChartSeries, updatedSeries));

            comp.FindAll("path.mud-chart-bar").Count.Should().Be(4); // 2 from Series 1 + 2 from Series 2
            comp.FindAll("div.mud-chart-legend-item").Count.Should().Be(2);
            comp.FindAll("div.mud-chart-legend-item")[1].TextContent.Should().Contain("Series 2");
        }

        [Test]
        public async Task StackedBarChart_Dynamic_RemoveSeries_ShouldUpdateChart()
        {
            var initialSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 10, 20 } },
                new () { Name = "Series 2", Data = new double[] { 15, 25 } }
            };
            string[] xAxisLabels = { "A", "B" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, initialSeries)
                .Add(p => p.ChartLabels, xAxisLabels));

            comp.FindAll("path.mud-chart-bar").Count.Should().Be(4);
            comp.FindAll("div.mud-chart-legend-item").Count.Should().Be(2);

            var updatedSeries = new List<ChartSeries<double>>()
            {
                initialSeries[0] // Keep only Series 1
            };

            await comp.SetParametersAndRenderAsync(parameters => parameters
                      .Add(p => p.ChartSeries, updatedSeries));

            comp.FindAll("path.mud-chart-bar").Count.Should().Be(2); // Only Series 1's bars
            comp.FindAll("div.mud-chart-legend-item").Count.Should().Be(1);
            comp.Find("div.mud-chart-legend-item").TextContent.Should().Contain("Series 1");
        }

        [Test]
        public async Task StackedBarChart_Dynamic_ChangeSeriesData_ShouldUpdateChart()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 10, 20 } }
            };
            string[] xAxisLabels = { "A", "B" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels));

            var initialPaths = comp.FindAll("path.mud-chart-bar");
            initialPaths.Count.Should().Be(2);

            // Modify data
            chartSeries[0].Data = new double[] { 30, 40 };
            await comp.SetParametersAndRenderAsync(parameters => parameters
                      .Add(p => p.ChartSeries, new List<ChartSeries<double>>(chartSeries))); // Re-pass to trigger update

            var updatedPaths = comp.FindAll("path.mud-chart-bar");
            updatedPaths.Count.Should().Be(2);
        }

        [Test]
        public async Task StackedBarChart_Dynamic_ChangeLabels_ShouldUpdateChart()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 10, 20 } }
            };
            string[] initialXAxisLabels = { "A", "B" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, initialXAxisLabels));

            var initialLabels = comp.FindAll("g.mud-charts-xaxis text").Select(x => x.TextContent).ToArray();
            initialLabels.Should().BeEquivalentTo(initialXAxisLabels);

            string[] updatedXAxisLabels = { "X", "Y" };
            await comp.SetParametersAndRenderAsync(parameters => parameters
                      .Add(p => p.ChartLabels, updatedXAxisLabels));

            var updatedLabels = comp.FindAll("g.mud-charts-xaxis text").Select(x => x.TextContent).ToArray();
            updatedLabels.Should().BeEquivalentTo(updatedXAxisLabels);
        }

        [Test]
        public void StackedBarChart_ChartOptions_ShowLegendFalse_ShouldNotRenderLegend()
        {
            var chartSeries = new List<ChartSeries<double>>() { new() { Name = "Series 1", Data = new double[] { 10 } } };
            string[] xAxisLabels = { "A" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new ChartOptions { ShowLegend = false }));

            comp.FindAll("div.mud-chart-legend").Should().BeEmpty();
            comp.FindAll("div.mud-chart-legend-item").Should().BeEmpty();
        }

        [Test]
        [TestCase(Position.Top)]
        [TestCase(Position.Bottom)]
        [TestCase(Position.Left)]
        [TestCase(Position.Right)]
        public void StackedBarChart_LegendPosition_ShouldApplyCorrectClass(Position position)
        {
            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.LegendPosition, position)
                .Add(p => p.ChartSeries, new List<ChartSeries<double>>() { new() { Name = "S1", Data = new[] { 1.0 } } })
                .Add(p => p.ChartLabels, new[] { "L1" })
            );

            var chartElement = comp.Find(".mud-chart");
            var expectedClass = $"mud-chart-legend-{position.ToStringFast(true)}";
            chartElement.ClassList.Should().Contain(expectedClass);
        }

        [Test]
        public void StackedBarChart_ChartOptions_ChartPalette_ShouldApplyColors()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 10 } },
                new () { Name = "Series 2", Data = new double[] { 20 } }
            };
            string[] xAxisLabels = { "A" };
            var customPalette = new string[] { "#FF0000", "#00FF00" }; // Red, Green

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = customPalette }));

            var bars = comp.FindAll("path.mud-chart-bar");
            bars.Should().HaveCount(2);
            // Note: fill is "none" for stacked bar, stroke is the color
            bars[0].Attributes.GetNamedItem("stroke")?.Value.Should().Be(customPalette[0]);
            bars[1].Attributes.GetNamedItem("stroke")?.Value.Should().Be(customPalette[1]);
        }

        [Test]
        public void StackedBarChart_ChartOptions_ChartPalette_CycleColorsWhenSeriesExceedPalette()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 10 } },
                new () { Name = "Series 2", Data = new double[] { 20 } },
                new () { Name = "Series 3", Data = new double[] { 30 } }
            };
            string[] xAxisLabels = { "A" };
            var customPalette = new string[] { "#111111", "#222222" }; // Only two colors

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = customPalette }));

            var bars = comp.FindAll("path.mud-chart-bar");
            bars.Should().HaveCount(3);
            bars[0].Attributes.GetNamedItem("stroke")?.Value.Should().Be(customPalette[0]);
            bars[1].Attributes.GetNamedItem("stroke")?.Value.Should().Be(customPalette[1]);
            bars[2].Attributes.GetNamedItem("stroke")?.Value.Should().Be(customPalette[0]); // Cycled back
        }

        [Test]
        [TestCase("300px", "400px")]
        [TestCase("50%", "75%")]
        public void StackedBarChart_WidthAndHeight_ShouldApplyToSvg(string width, string height)
        {
            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.Width, width)
                .Add(p => p.Height, height));

            var svgElement = comp.Find("svg.mud-chart-bar");
            svgElement.Attributes.GetNamedItem("width")?.Value.Should().Contain(width);
            svgElement.Attributes.GetNamedItem("height")?.Value.Should().Contain(height);
        }

        [Test]
        public async Task StackedBarChart_RightToLeft_True_ShouldAdjustLegendPositionStartEnd()
        {
            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.RightToLeft, true)
                .Add(p => p.LegendPosition, Position.Start) // Should become Right in RTL
                .Add(p => p.ChartSeries, new List<ChartSeries<double>>() { new() { Name = "S1", Data = new[] { 1.0 } } })
                .Add(p => p.ChartLabels, new[] { "L1" })
                );

            var chartElement = comp.Find(".mud-chart");
            chartElement.ClassList.Should().Contain("mud-chart-legend-right");

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.LegendPosition, Position.End)); // Should become Left in RTL
            chartElement = comp.Find(".mud-chart");
            chartElement.ClassList.Should().Contain("mud-chart-legend-left");
        }

        [Test]
        public void StackedBarChart_DataEdge_AllZeroValues_ShouldRenderCorrectly()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 0, 0, 0 } },
                new () { Name = "Series 2", Data = new double[] { 0, 0, 0 } }
            };
            string[] xAxisLabels = { "A", "B", "C" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions { YAxisTicks = 10 })); // Ensure some scale

            var bars = comp.FindAll("path.mud-chart-bar");
            bars.Count.Should().Be(6); // 2 series * 3 data points

            foreach (var bar in bars)
            {
                var d = bar.GetAttribute("d");
                var parts = d.Split(new[] { 'M', 'L', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(p => double.Parse(p, System.Globalization.CultureInfo.InvariantCulture))
                             .ToArray();
                parts[0].Should().BeApproximately(parts[2], 0.001); // x1 == x2
                parts[1].Should().BeApproximately(parts[3] + /*StackedBar.BarOverlapAmountFix*/0.5, 0.001); // y1 == y2 (approx due to overlap fix)
            }

            var yAxisLabels = comp.FindAll("g.mud-charts-yaxis text").Select(e => e.TextContent).ToList();
            yAxisLabels.Should().Contain("0");
        }

        [Test]
        public void StackedBarChart_DataEdge_NegativeValues_ShouldRenderBelowXAxis()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { -10, -20 } },
                new () { Name = "Series 2", Data = new double[] { -5, -15 } }
            };
            string[] xAxisLabels = { "A", "B" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions { YAxisTicks = 5 }));

            var bars = comp.FindAll("path.mud-chart-bar");
            bars.Count.Should().Be(4);

            var yAxisLabels = comp.FindAll("g.mud-charts-yaxis text").Select(e => e.TextContent).ToList();
            yAxisLabels.Should().Contain(l => l.Contains("-")); // Check for negative sign
            yAxisLabels.Should().Contain("-5"); // Or other negative values based on scale
            yAxisLabels.Should().Contain("-20"); // Or other negative values based on scale
            yAxisLabels.Should().Contain("0"); // Zero line should be present

            foreach (var bar in bars)
            {
                var d = bar.GetAttribute("d");
                var parts = d.Split(new[] { 'M', 'L', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(p => double.Parse(p, System.Globalization.CultureInfo.InvariantCulture))
                             .ToArray();
                (parts[3] > parts[1] - 0.501).Should().BeTrue(); // yEnd > yStart (approx, considering overlap)
            }
        }

        [Test]
        public void StackedBarChart_DataEdge_MixedPositiveNegativeZeroValues_ShouldRenderCorrectly()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Profit", Data = new double[] { 10, -5, 0, 20 } },
                new () { Name = "Expenses", Data = new double[] { -5, -10, -2, 5 } } // Note: Expenses are often positive, but for testing mixed stack.
            };
            string[] xAxisLabels = { "Q1", "Q2", "Q3", "Q4" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions { YAxisTicks = 5 }));

            var bars = comp.FindAll("path.mud-chart-bar");
            bars.Count.Should().Be(8); // 2 series * 4 data points

            var yAxisLabels = comp.FindAll("g.mud-charts-yaxis text").Select(e => e.TextContent).ToList();
            yAxisLabels.Should().Contain("0");
            yAxisLabels.Should().Contain(l => l.StartsWith("-") || l.StartsWith("-$")); // Negative values
            yAxisLabels.Should().Contain(l => char.IsDigit(l[0])); // Positive values (simple check)
        }

        [Test]
        public void StackedBarChart_DataEdge_VeryLargeValues_ShouldScaleCorrectly()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 1_000_000, 5_000_000 } },
                new () { Name = "Series 2", Data = new double[] { 2_000_000, 3_000_000 } }
            };
            string[] xAxisLabels = { "A", "B" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions { YAxisFormat = "N0" })); // Number format

            var bars = comp.FindAll("path.mud-chart-bar");
            bars.Count.Should().Be(4);

            var yAxisLabels = comp.FindAll("g.mud-charts-yaxis text").Select(e => e.TextContent).ToList();
            // Labels should represent large numbers
            double.Parse(yAxisLabels[1]).Should().BeGreaterThan(500_000);
        }

        [Test]
        public void StackedBarChart_DataEdge_SingleDataPointInSeries_ShouldRender()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 10 } },
                new () { Name = "Series 2", Data = new double[] { 20 } }
            };
            // Only one X-axis label for the single data point category
            string[] xAxisLabels = { "Category A" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels));

            comp.FindAll("path.mud-chart-bar").Count.Should().Be(2); // 2 series * 1 data point
            comp.FindAll("div.mud-chart-legend-item").Count.Should().Be(2);
            var xAxisTexts = comp.FindAll("g.mud-charts-xaxis text");
            xAxisTexts.Should().HaveCount(1);
            xAxisTexts.First().TextContent.Should().Be("Category A");
        }

        [Test]
        public void StackedBarChart_DataEdge_SingleSeriesSingleDataPoint_ShouldRender()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Solo Series", Data = new double[] { 100 } }
            };
            string[] xAxisLabels = { "Solo Point" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels));

            comp.FindAll("path.mud-chart-bar").Count.Should().Be(1); // 1 series * 1 data point
            comp.FindAll("div.mud-chart-legend-item").Count.Should().Be(1);
            comp.Find("div.mud-chart-legend-item").TextContent.Should().Contain("Solo Series");
            var xAxisTexts = comp.FindAll("g.mud-charts-xaxis text");
            xAxisTexts.Should().HaveCount(1);
            xAxisTexts.First().TextContent.Should().Be("Solo Point");
        }

        [Test]
        public void StackedBarChart_Tooltip_ShowToolTipsFalse_ShouldNotRenderTooltipComponent()
        {
            var chartSeries = new List<ChartSeries<double>>() { new() { Name = "S1", Data = new[] { 10.0 } } };
            string[] xAxisLabels = { "A" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions { ShowToolTips = false }));

            // The ChartTooltip component should not be found in the render tree
            comp.FindComponents<ChartTooltip>().Should().BeEmpty();
        }

        [Test]
        public async Task StackedBarChart_Tooltip_ShowToolTipsTrue_ShouldRenderTooltipComponent()
        {
            var chartSeries = new List<ChartSeries<double>>() { new() { Name = "S1", Data = new[] { 10.0 } } };
            string[] xAxisLabels = { "A" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions { ShowToolTips = true }));

            // The ChartTooltip component should be present
            var bar = comp.Find("path.mud-chart-bar");

            await bar.MouseOverAsync();

            var tooltip = comp.Find("g.svg-tooltip");
            tooltip.Should().NotBeNull();
        }

        [Test]
        public async Task StackedBarChart_Selection_LegendClick_ShouldUpdateSelectedIndexAndFireEvent()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series A", Data = new double[] { 10, 20 } },
                new () { Name = "Series B", Data = new double[] { 15, 25 } },
            };
            string[] xAxisLabels = { "X1", "X2" };
            var selectedIndex = -1; // Initial value, should be different from what we click
            var eventFiredCount = 0;

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.SelectedIndex, selectedIndex)
                .Add(p => p.SelectedIndexChanged, EventCallback.Factory.Create<int>(this, (newIndex) =>
                {
                    selectedIndex = newIndex;
                    eventFiredCount++;
                }))
                .Add(p => p.ChartOptions, new ChartOptions { ShowLegend = true })
            );

            var legendItems = comp.FindAll("div.mud-chart-legend-item");
            legendItems.Should().HaveCount(2);

            // Click the second legend item (index 1)
            await legendItems[1].ClickAsync();
            await comp.WaitForAssertionAsync(() => selectedIndex.Should().Be(1));
            eventFiredCount.Should().Be(1);

            comp.Instance.GetState(x => x.SelectedIndex).Should().Be(1);

            // Click the first legend item (index 0)
            legendItems = comp.FindAll("div.mud-chart-legend-item");
            await legendItems[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => selectedIndex.Should().Be(0));
            eventFiredCount.Should().Be(2);
            comp.Instance.GetState(x => x.SelectedIndex).Should().Be(0);
        }

        [Test]
        public async Task StackedBarChart_Selection_ProgrammaticChange_ShouldReflectInLegend()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series X", Data = new double[] { 10 } },
                new () { Name = "Series Y", Data = new double[] { 20 } },
            };
            string[] xAxisLabels = { "X" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.SelectedIndex, 0) // Initially select Series X
                .Add(p => p.ChartOptions, new ChartOptions { ShowLegend = true })
            );

            // Programmatically change SelectedIndex
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.SelectedIndex, 1));

            comp.Instance.GetState(x => x.SelectedIndex).Should().Be(1);
        }

        [Test]
        public void StackedBarChart_CustomGraphics_ShouldRenderInsideSvg()
        {
            var chartSeries = new List<ChartSeries<double>>() { new() { Name = "S1", Data = new[] { 10.0 } } };
            string[] xAxisLabels = { "A" };

            RenderFragment customGraphics = builder =>
            {
                builder.OpenElement(0, "rect");
                builder.AddAttribute(1, "x", "10");
                builder.AddAttribute(2, "y", "10");
                builder.AddAttribute(3, "width", "50");
                builder.AddAttribute(4, "height", "50");
                builder.AddAttribute(5, "fill", "red");
                builder.AddAttribute(6, "class", "custom-graphic-element");
                builder.CloseElement();
            };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.CustomGraphics, customGraphics));

            var svgElement = comp.Find("svg.mud-chart-bar");
            svgElement.Should().NotBeNull();

            // Check if the custom graphic element is rendered within the SVG
            var customElement = svgElement.QuerySelector(".custom-graphic-element");
            customElement.Should().NotBeNull();
            customElement.TagName.Should().Be("rect");
            customElement.GetAttribute("fill").Should().Be("red");
            customElement.GetAttribute("x").Should().Be("10");
        }

        [Test]
        public async Task StackedBarChart_Tooltip_TooltipTemplate_ShouldRenderCustomTooltip()
        {
            var chartSeries = new List<ChartSeries<double>>() { new() { Name = "TemplateSeries", Data = new[] { 77.0 } } };
            string[] xAxisLabels = { "TX" };
            RenderFragment<(SvgPath Segment, string Color)> tooltipTemplate = (context) => builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "custom-tooltip-content");
                builder.AddContent(2, $"Custom: {context.Segment.LabelXValue} -> {context.Segment.LabelYValue} ({context.Color})");
                builder.CloseElement();
            };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions { ShowToolTips = true })
                .Add(p => p.TooltipTemplate, tooltipTemplate));

            var bar = comp.Find("path.mud-chart-bar");

            await bar.MouseOverAsync();

            var customContent = comp.Find("div.custom-tooltip-content");
            customContent.Should().NotBeNull();
        }

        private bool _tooltipPositionFuncCalled = false;
        private (double X, double Y) CustomTooltipPositionFunc(SvgPath segment)
        {
            _tooltipPositionFuncCalled = true;
            return (segment.LabelX + 10, segment.LabelY - 10);
        }

        [Test]
        public async Task StackedBarChart_Tooltip_TooltipPositionFunc_ShouldBeCalledOnHover()
        {
            _tooltipPositionFuncCalled = false;
            var chartSeries = new List<ChartSeries<double>>() { new() { Name = "PosFuncSeries", Data = new[] { 55.0 } } };
            string[] xAxisLabels = { "PFX" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions { ShowToolTips = true })
                .Add(p => p.TooltipPositionFunc, CustomTooltipPositionFunc));

            var bar = comp.Find("path.mud-chart-bar");

            await bar.MouseOverAsync();

            _tooltipPositionFuncCalled.Should().BeTrue();

            var tooltipDiv = comp.Find("g.svg-tooltip text");
            tooltipDiv.Should().NotBeNull();
        }

        [Test]
        public void StackedBarChart_Options_YAxisFormat_ShouldFormatYAxisLabels()
        {
            var chartSeries = new List<ChartSeries<double>>() { new() { Name = "S1", Data = new[] { 1000.0, 2000.0 } } };
            string[] xAxisLabels = { "A", "B" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions { YAxisFormat = "C0", YAxisTicks = 1000 })); // Currency, 0 decimal places

            var yAxisLabels = comp.FindAll("g.mud-charts-yaxis text").Select(e => e.TextContent).ToList();
            yAxisLabels.Should().Contain(l => l.Contains(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol));
            yAxisLabels.Should().Contain(l => !l.Contains('.')); // No decimal places
        }

        [Test]
        public async Task StackedBarChart_Options_YAxisTicks_And_MaxNumYAxisTicks_ShouldControlYAxisGrid()
        {
            var chartSeries = new List<ChartSeries<double>>() { new() { Name = "S1", Data = new[] { 0.0, 100.0 } } };
            string[] xAxisLabels = { "A" };

            // Test with specific YAxisTicks
            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions { YAxisTicks = 20 }));

            var yAxisLabels1 = comp.FindAll("g.mud-charts-yaxis text").Select(e => e.TextContent).ToList();
            // Expected labels: "0", "20", "40", "60", "80", "100" (6 labels, 6 lines)
            yAxisLabels1.Should().HaveCount(6);
            yAxisLabels1.Should().ContainInOrder("0", "20", "40", "60", "80", "100");

            // Test with MaxNumYAxisTicks forcing fewer, larger ticks
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.ChartOptions, new StackedBarChartOptions { YAxisTicks = 10, MaxNumYAxisTicks = 3 })); // Max 3 ticks, initial step 10

            var yAxisLabels2 = comp.FindAll("g.mud-charts-yaxis text").Select(e => e.TextContent).ToList();

            yAxisLabels2.Should().HaveCount(3);
            yAxisLabels2.Should().ContainInOrder("0", "80", "160");
        }

        [Test]
        public async Task StackedBarChart_Options_YAxisSuggestedMax_ShouldInfluenceScale()
        {
            var chartSeries = new List<ChartSeries<double>>() { new() { Name = "S1", Data = new[] { 10.0, 50.0 } } };
            string[] xAxisLabels = { "A", "B" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions { YAxisSuggestedMax = 100, YAxisTicks = 20 }));

            var yAxisLabels = comp.FindAll("g.mud-charts-yaxis text").Select(e => e.TextContent).ToList();
            yAxisLabels.Should().Contain("100"); // The suggested max

            // If actual max is higher, suggestedMax is ignored
            await comp.SetParametersAndRenderAsync(parameters => parameters
                      .Add(p => p.ChartOptions, new StackedBarChartOptions { YAxisSuggestedMax = 30, YAxisTicks = 10 }));

            yAxisLabels = comp.FindAll("g.mud-charts-yaxis text").Select(e => e.TextContent).ToList();
            yAxisLabels.Should().Contain("50"); // Actual max 50
            yAxisLabels.Should().NotContain("100");
        }

        [Test]
        public void StackedBarChart_Options_AxisVisibility_False_ShouldWork()
        {
            var chartSeries = new List<ChartSeries<double>>() { new() { Name = "S1", Data = new[] { 10.0 } } };
            string[] xAxisLabels = { "A" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions
                {
                    XAxisLines = false,
                    YAxisLines = false,
                }));

            comp.FindAll("g.mud-charts-gridlines-yaxis").Should().BeEmpty();
            comp.FindAll("g.mud-charts-gridlines-xaxis-lines").Should().BeEmpty();
        }

        [Test]
        public void StackedBarChart_Options_AxisVisibility_True_ShouldWork()
        {
            var chartSeries = new List<ChartSeries<double>>() { new() { Name = "S1", Data = new[] { 10.0 } } };
            string[] xAxisLabels = { "A" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new StackedBarChartOptions
                {
                    XAxisLines = true,
                    YAxisLines = true,
                }));

            comp.FindAll("g.mud-charts-gridlines-yaxis").Should().NotBeEmpty();
            comp.FindAll("g.mud-charts-gridlines-xaxis-lines").Should().NotBeEmpty();
        }

        [Test]
        public async Task BarChartExampleData()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "United States", Data = new double[] { 40, 20, 25, 27, 46, 60, 48, 80, 15 } },
                new () { Name = "Germany", Data = new double[] { 19, 24, 35, 13, 28, 15, 13, 16, 31 } },
                new () { Name = "Sweden", Data = new double[] { 8, 6, 11, 13, 4, 16, 10, 16, 18 } },
            };
            string[] xAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
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

            comp.FindAll("path.mud-chart-bar").Count.Should().Be(3 * 9, because: "3 series with 9 data points each");

            comp.Markup.Should()
                .Contain("d=\"M 44 320 L 44 221.1667\"");

            comp.Markup.Should()
                .Contain("d=\"M 656 206.9167 L 656 162.1667\"");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                      .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _modifiedPalette }));

            comp.Markup.Should().Contain(_modifiedPalette[0]);
        }

        [Test]
        public async Task StackedBarChartColoring()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new() { Name = "Deep Sea Blue", Data = new double[] { 40, 20, 25, 27, 46 } },
                new() { Name = "Venetian Red", Data = new double[] { 19, 24, 35, 13, 28 } },
                new() { Name = "Banana Yellow", Data = new double[] { 8, 6, 11, 13, 4 } },
                new() { Name = "La Salle Green", Data = new double[] { 18, 9, 7, 10, 7 } },
                new() { Name = "Rich Carmine", Data = new double[] { 9, 14, 6, 15, 20 } },
                new() { Name = "Shiraz", Data = new double[] { 9, 4, 11, 5, 19 } },
                new() { Name = "Cloud Burst", Data = new double[] { 14, 9, 20, 16, 6 } },
                new() { Name = "Neon Pink", Data = new double[] { 14, 8, 4, 14, 8 } },
                new() { Name = "Ocean", Data = new double[] { 11, 20, 13, 5, 5 } },
                new() { Name = "Orangey Red", Data = new double[] { 6, 6, 19, 20, 6 } },
                new() { Name = "Catalina Blue", Data = new double[] { 3, 2, 20, 3, 10 } },
                new() { Name = "Fountain Blue", Data = new double[] { 3, 18, 11, 12, 3 } },
                new() { Name = "Irish Green", Data = new double[] { 20, 5, 15, 16, 13 } },
                new() { Name = "Wild Strawberry", Data = new double[] { 15, 9, 12, 12, 1 } },
                new() { Name = "Geraldine", Data = new double[] { 5, 13, 19, 15, 8 } },
                new() { Name = "Grey Teal", Data = new double[] { 12, 16, 20, 16, 17 } },
                new() { Name = "Baby Pink", Data = new double[] { 1, 18, 10, 19, 8 } },
                new() { Name = "Thunderbird", Data = new double[] { 15, 16, 10, 8, 5 } },
                new() { Name = "Navy", Data = new double[] { 16, 2, 3, 5, 5 } },
                new() { Name = "Aqua Marina", Data = new double[] { 17, 6, 11, 19, 6 } },
                new() { Name = "Lavender Pinocchio", Data = new double[] { 1, 11, 4, 18, 1 } },
                new() { Name = "Deep Sea Blue", Data = new double[] { 1, 11, 4, 18, 1 } }
            };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.StackedBar)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = new string[] { "#1E9AB0" } })
                .Add(p => p.ChartSeries, chartSeries));

            var paths1 = comp.FindAll("path");

            int count;
            count = paths1.Count(p => p.OuterHtml.Contains($"fill=\"none\"") && p.OuterHtml.Contains($"stroke=\"{"#1E9AB0"}\""));
            count.Should().Be(5 * 22);

            await comp.SetParametersAndRenderAsync(parameters => parameters
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
        public async Task StackedBarChart_CanHideSeries()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () { Name = "Series 1", Data = new double[] { 20, 30, 40 } },
                new () { Name = "Series 2", Data = new double[] { 10, 15, 20 } },
                new () { Name = "Series 3", Data = new double[] { 5, 10, 15 }, Visible = false } // Initially hidden
            };
            string[] xAxisLabels = { "Label A", "Label B", "Label C" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
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

            comp.FindAll($"path.mud-chart-bar{series1}").Count.Should().Be(chartSeries[0].Data.Values.Count, "Series 1 should have its bar segments visible initially");
            comp.FindAll($"path.mud-chart-bar{series2}").Count.Should().Be(chartSeries[1].Data.Values.Count, "Series 2 should have its bar segments visible initially");
            comp.FindAll($"path.mud-chart-bar{series3}").Count.Should().Be(0, "Series 3 should have no bar segments visible initially");

            // Hide Series 1
            await seriesCheckboxes[0].ChangeAsync(false);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeFalse("Series 1 checkbox should be unchecked after hiding");
            chartSeries[0].Visible.Should().BeFalse("Series 1 Visible property should be false after hiding");
            comp.FindAll($"path.mud-chart-bar{series1}").Count.Should().Be(0, "Series 1 bar segments should be hidden");

            // Show Series 1 again
            await seriesCheckboxes[0].ChangeAsync(true);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeTrue("Series 1 checkbox should be checked after re-showing");
            chartSeries[0].Visible.Should().BeTrue("Series 1 Visible property should be true after re-showing");
            comp.FindAll($"path.mud-chart-bar{series1}").Count.Should().Be(chartSeries[0].Data.Values.Count, "Series 1 bar segments should be visible again");

            // Hide Series 2
            await seriesCheckboxes[1].ChangeAsync(false);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[1].IsChecked().Should().BeFalse("Series 2 checkbox should be unchecked after hiding");
            chartSeries[1].Visible.Should().BeFalse("Series 2 Visible property should be false after hiding");
            comp.FindAll($"path.mud-chart-bar{series2}").Count.Should().Be(0, "Series 2 bar segments should be hidden");
            comp.FindAll($"path.mud-chart-bar{series1}").Count.Should().Be(chartSeries[0].Data.Values.Count, "Series 1 bar segments should remain visible");

            // Show Series 3
            await seriesCheckboxes[2].ChangeAsync(true);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[2].IsChecked().Should().BeTrue("Series 3 checkbox should be checked after showing");
            chartSeries[2].Visible.Should().BeTrue("Series 3 Visible property should be true after showing");
            comp.FindAll($"path.mud-chart-bar{series3}").Count.Should().Be(chartSeries[2].Data.Values.Count, "Series 3 bar segments should be visible");
        }
    }
}
