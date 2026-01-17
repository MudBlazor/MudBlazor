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

namespace MudBlazor.UnitTests.Charts;

[TestFixture]
public class RadarChartTests : BunitTest
{
    [Test]
    public void RadarChart_BasicRendering_NoData()
    {
        var comp = Context.Render<Radar<double>>();
        comp.Markup.Should().Contain("<svg");
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(0);
        comp.FindAll("path.mud-chart-axis-line").Count.Should().Be(0); // No labels, no data, so no axes.
    }

    [Test]
    public async Task RadarChart_Should_UpdateSelectedPointIndex_OnDataMarkerClickAsync()
    {
        var seriesData = new double[] { 10, 20, 30 };
        var chartLabels = new[] { "A", "B", "C" };
        var options = new RadarChartOptions { ShowDataMarkers = true, AggregationOption = AggregationOption.GroupByDataSet };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> {
                new() { Name = "Series", Data = seriesData },
                new() { Name = "Other Series", Data = new double[] { 5,15,25 } } // Add another series
            })
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
        );

        var dataMarkers = comp.FindAll("circle.mud-chart-series-point");
        // Markers for "Test Series": indices 0, 1, 2
        // Markers for "Other Series": indices 0, 1, 2 (but associated with series index 1)

        // Click on the second data marker of the first series (PointIndex 1, SeriesIndex 0)
        await dataMarkers[1].ClickAsync();
        comp.Instance.SelectedPointIndex.Should().Be(1);
        comp.Instance.GetState(x => x.SelectedIndex).Should().Be(0);

        dataMarkers = comp.FindAll("circle.mud-chart-series-point");

        // Click on the first data marker of the second series (PointIndex 0, SeriesIndex 1)
        // Total markers = 3 (for series 1) + 3 (for series 2) = 6. So index 3 is first marker of 2nd series.
        await dataMarkers[3].ClickAsync();
        comp.Instance.SelectedPointIndex.Should().Be(0);
        comp.Instance.GetState(x => x.SelectedIndex).Should().Be(1);
    }

    [Test]
    public void RadarChart_BasicRendering_WithData_InferAxesFromData()
    {
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartOptions, new RadarChartOptions() { AggregationOption = AggregationOption.GroupByLabel })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(3);
        comp.FindAll("path.mud-chart-axis-line").Count.Should().Be(1);
    }

    [Test]
    public void RadarChart_BasicRendering_WithData_AndLabels()
    {
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30, 40 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C", "D" })
            .Add(p => p.ChartOptions, new RadarChartOptions() { AggregationOption = AggregationOption.GroupByLabel })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(4);
        comp.FindAll("path.mud-chart-axis-line").Count.Should().Be(1);
    }

    [Test]
    public void RadarChart_Option_ShowGridLines_And_GridLevels()
    {
        var options = new RadarChartOptions { ShowGridLines = true, GridLevels = 3, AggregationOption = AggregationOption.GroupByLabel };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-grid-line").Count.Should().Be(3); // 3 levels
    }

    [Test]
    public void RadarChart_Option_ShowGridLines_False()
    {
        var options = new RadarChartOptions { ShowGridLines = false };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-grid-line").Count.Should().Be(0);
    }

    [Test]
    public void RadarChart_Option_ShowAxisLabels_True()
    {
        var options = new RadarChartOptions { ShowAxisLabels = true, AggregationOption = AggregationOption.GroupByDataSet };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "Axis1", "Axis2", "Axis3" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("text.mud-chart-axis-label").Count.Should().Be(3);
        comp.FindAll("text.mud-chart-axis-label").Any(t => t.TextContent == "Axis1").Should().BeTrue();
    }

    [Test]
    public void RadarChart_Option_ShowAxisLabels_False()
    {
        var options = new RadarChartOptions { ShowAxisLabels = false };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "Axis1", "Axis2", "Axis3" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("text.mud-chart-axis-label").Count.Should().Be(0);
    }

    [Test]
    public void RadarChart_Option_ShowDataPoints_True()
    {
        var options = new RadarChartOptions { ShowDataMarkers = true, DataPointRadius = 4 };
        var seriesData = new double[] { 10, 20, 30, 40 };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = seriesData } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C", "D" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("circle.mud-chart-series-point").Count.Should().Be(seriesData.Length);
    }

    [Test]
    public void RadarChart_Option_ShowDataPoints_False()
    {
        var options = new RadarChartOptions { ShowDataMarkers = false };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30, 40 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C", "D" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("circle.mud-chart-series-point").Count.Should().Be(0);
    }

    [Test]
    public void RadarChart_MultipleSeries()
    {
        var series = new List<ChartSeries<double>>
        {
            new() { Name = "Series1", Data = new double[] { 10, 20, 30 } },
            new() { Name = "Series2", Data = new double[] { 15, 25, 35 } }
        };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, series)
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, new RadarChartOptions() { AggregationOption = AggregationOption.GroupByDataSet })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(2);
    }

    [Test]
    public async Task RadarChart_Interaction_SelectedIndexAsync()
    {
        var selectedIndex = -1;
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> {
                new() { Name = "Series1", Data = new double[] { 10, 20, 30 } },
                new() { Name = "Series2", Data = new double[] { 15, 25, 35 } }
            })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, new RadarChartOptions() { AggregationOption = AggregationOption.GroupByDataSet })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
            .Add(p => p.SelectedIndex, selectedIndex)
            .Add(p => p.SelectedIndexChanged, EventCallback.Factory.Create<int>(this, val => selectedIndex = val))
        );

        // Simulate click on the first series path (index 0)
        await comp.FindAll("path.mud-chart-serie").First().ClickAsync();
        selectedIndex.Should().Be(0);

        // Simulate click on the second series path (index 1)
        await comp.FindAll("path.mud-chart-serie").Last().ClickAsync();
        selectedIndex.Should().Be(1);
    }

    [Test]
    public void RadarChart_Option_AngleOffset()
    {
        var options = new RadarChartOptions { AngleOffset = 45, AggregationOption = AggregationOption.GroupByLabel };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(3);
        comp.Find("path.mud-chart-serie").GetAttribute("d").Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task RadarChart_CanHideSeries_TestAsync()
    {
        var chartSeries = new List<ChartSeries<double>>()
        {
            new () { Name = "Series 1", Data = new double[] { 90, 79, 72, 69 } },
            new () { Name = "Series 2", Data = new double[] { 10, 41, 35, 51 } },
            new () { Name = "Series 3", Data = new double[] { 60, 20, 85, 30 }, Visible = false } // Initially hidden
        };
        string[] xAxisLabels = { "Cat A", "Cat B", "Cat C", "Cat D" };

        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Radar)
            .Add(p => p.Height, "400px")
            .Add(p => p.Width, "400px")
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, xAxisLabels)
            .Add(p => p.CanHideSeries, true)
            .Add(p => p.ChartOptions, new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet })
        );

        // Initial state assertions
        var seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
        seriesCheckboxes.Count.Should().Be(chartSeries.Count, "Number of checkboxes should match number of series");

        seriesCheckboxes[0].IsChecked().Should().BeTrue("Series 1 should be initially visible");
        seriesCheckboxes[1].IsChecked().Should().BeTrue("Series 2 should be initially visible");
        seriesCheckboxes[2].IsChecked().Should().BeFalse("Series 3 should be initially hidden");

        var series1 = "[stroke='#2979FF']";
        var series2 = "[stroke='#1DE9B6']";
        var series3 = "[stroke='#FFC400']";

        // A visible radar series should have 1 path. A hidden one should have 0.
        comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(1, "Series 1 path should initially be visible");
        comp.FindAll($"path.mud-chart-serie{series2}").Count.Should().Be(1, "Series 2 path should initially be visible");
        comp.FindAll($"path.mud-chart-serie{series3}").Count.Should().Be(0, "Series 3 path should initially be hidden");

        // Hide Series 1
        await comp.InvokeAsync(() => seriesCheckboxes[0].ChangeAsync(false));
        seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
        seriesCheckboxes[0].IsChecked().Should().BeFalse("Series 1 checkbox should be unchecked after hiding");
        chartSeries[0].Visible.Should().BeFalse("Series 1 Visible property should be false");
        comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(0, "Series 1 path should be hidden after unchecking");

        // Show Series 1 again
        await comp.InvokeAsync(() => seriesCheckboxes[0].ChangeAsync(true));
        seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
        seriesCheckboxes[0].IsChecked().Should().BeTrue("Series 1 checkbox should be checked after showing");
        chartSeries[0].Visible.Should().BeTrue("Series 1 Visible property should be true");
        comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(1, "Series 1 path should be visible again after re-checking");

        // Hide Series 2
        await comp.InvokeAsync(() => seriesCheckboxes[1].ChangeAsync(false));
        seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
        seriesCheckboxes[1].IsChecked().Should().BeFalse("Series 2 checkbox should be unchecked after hiding");
        chartSeries[1].Visible.Should().BeFalse("Series 2 Visible property should be false");
        comp.FindAll($"path.mud-chart-serie{series2}").Count.Should().Be(0, "Series 2 path should be hidden");
        comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(1, "Series 1 path should still be visible"); // Ensure other series not affected

        // Show Series 3 (which was initially hidden)
        await comp.InvokeAsync(() => seriesCheckboxes[2].ChangeAsync(true));
        seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
        seriesCheckboxes[2].IsChecked().Should().BeTrue("Series 3 checkbox should be checked after showing");
        chartSeries[2].Visible.Should().BeTrue("Series 3 Visible property should be true");
        comp.FindAll($"path.mud-chart-serie{series3}").Count.Should().Be(1, "Series 3 path should be visible after checking");
    }

    [Test]
    public void RadarChart_Should_ApplyWidthAndHeight()
    {
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.Width, "500px")
            .Add(p => p.Height, "400px")
        );

        var svgElement = comp.Find("svg");
        svgElement.GetAttribute("width").Should().Be("500px");
        svgElement.GetAttribute("height").Should().Be("400px");
    }

    [Test]
    public void RadarChart_Should_ApplyCustomPaletteToSeries()
    {
        var customPalette = new[] { "#FF0000", "#00FF00", "#0000FF" }; // Red, Green, Blue
        var series = new List<ChartSeries<double>>
        {
            new() { Name = "Series1", Data = new double[] { 10, 20, 30 } },
            new() { Name = "Series2", Data = new double[] { 15, 25, 35 } },
            new() { Name = "Series3", Data = new double[] { 20, 30, 40 } },
            new() { Name = "Series4", Data = new double[] { 25, 35, 45 } } // Fourth series to test color wrapping
        };
        var options = new RadarChartOptions
        {
            ChartPalette = customPalette,
            AggregationOption = AggregationOption.GroupByDataSet
        };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, series)
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var seriesPaths = comp.FindAll("path.mud-chart-serie");
        seriesPaths.Count.Should().Be(series.Count);

        // Check colors for the first three series
        seriesPaths[0].GetAttribute("stroke").Should().Be(customPalette[0]);
        seriesPaths[1].GetAttribute("stroke").Should().Be(customPalette[1]);
        seriesPaths[2].GetAttribute("stroke").Should().Be(customPalette[2]);

        // Check color wrapping for the fourth series (should wrap back to the first color)
        seriesPaths[3].GetAttribute("stroke").Should().Be(customPalette[0]);
    }

    [Test]
    [TestCase(Position.Top)]
    [TestCase(Position.Bottom)]
    [TestCase(Position.Start)]
    [TestCase(Position.End)]
    public void RadarChart_Should_RespectLegendPosition(Position position)
    {
        var options = new RadarChartOptions
        {
            ShowLegend = true,
            AggregationOption = AggregationOption.GroupByDataSet
        };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
            .Add(p => p.LegendPosition, position)
        );

        var div = comp.Find("div.mud-chart");
        div.Should().NotBeNull();

        var expectedClass = position switch
        {
            Position.Top => "mud-chart-legend-top",
            Position.Bottom => "mud-chart-legend-bottom",
            Position.Start => "mud-chart-legend-left",
            Position.End => "mud-chart-legend-right",
            _ => string.Empty
        };
        div.ClassList.Should().Contain(expectedClass);
    }

    [Test]
    public void RadarChart_Should_RenderGracefully_WhenChartOptionsIsNull()
    {
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, null) // Set ChartOptions to null
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        // Assert that the component renders the basic SVG structure
        comp.Markup.Should().Contain("<svg");
        // Check for presence of series path, indicating basic rendering logic still runs
        comp.FindAll("path.mud-chart-serie").Count.Should().BeGreaterThan(0);
    }

    [Test]
    public void RadarChart_Should_Gracefully_WhenPaletteIsNullOrEmpty()
    {
        var options = new RadarChartOptions
        {
            ChartPalette = null,
            AggregationOption = AggregationOption.GroupByDataSet
        };
        var series = new List<ChartSeries<double>>
        {
            new() { Name = "Series1", Data = new double[] { 10, 20, 30 } },
            new() { Name = "Series2", Data = new double[] { 15, 25, 35 } }
        };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, series)
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var seriesPaths = comp.FindAll("path.mud-chart-serie");
        seriesPaths.Count.Should().Be(series.Count);

        foreach (var path in seriesPaths)
        {
            path.GetAttribute("stroke").Should().BeEmpty(); // no color should be applied if palette is null
        }
    }

    [Test]
    public void RadarChart_Should_ApplyGridLineColorAndWidth()
    {
        var options = new RadarChartOptions
        {
            ShowGridLines = true,
            GridLevels = 2,
            GridLineColor = "#ABCDEF",
            GridLineWidth = 3,
            AggregationOption = AggregationOption.GroupByLabel // Ensures grid lines are typically generated
        };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var gridLines = comp.FindAll("path.mud-chart-grid-line");
        gridLines.Count.Should().Be(options.GridLevels);

        foreach (var line in gridLines)
        {
            line.GetAttribute("stroke").Should().Be("#ABCDEF");
            line.GetAttribute("stroke-width").Should().Be("3");
        }
    }

    [Test]
    public void RadarChart_Should_ApplyAxisLineColorAndWidth()
    {
        var options = new RadarChartOptions
        {
            AxisLineColor = "#FEDCBA",
            AxisLineWidth = 2,
            AggregationOption = AggregationOption.GroupByLabel // Ensures axis lines are rendered
        };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" }) // Labels are needed for axes to be drawn
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var axisLines = comp.FindAll("path.mud-chart-axis-line");
        axisLines.Count.Should().Be(1);

        foreach (var line in axisLines)
        {
            line.GetAttribute("stroke").Should().Be("#FEDCBA");
            line.GetAttribute("stroke-width").Should().Be("2");
        }
    }

    [Test]
    public void RadarChart_Should_ApplyDataPointRadius()
    {
        var seriesData = new double[] { 10, 20, 30, 40 };
        var options = new RadarChartOptions
        {
            ShowDataMarkers = true,
            DataPointRadius = 6,
            AggregationOption = AggregationOption.GroupByLabel
        };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = seriesData } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C", "D" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var dataPoints = comp.FindAll("circle.mud-chart-series-point");
        dataPoints.Count.Should().Be(seriesData.Length);

        foreach (var point in dataPoints)
        {
            point.GetAttribute("r").Should().Be("6");
        }
    }

    [Test]
    [TestCase(0.0)]
    [TestCase(90.0)]
    [TestCase(-45.0)]
    [TestCase(360.0)]
    [TestCase(720.0)]
    public void RadarChart_Should_ApplyAngleOffset_WithValue(double angleOffset)
    {
        var options = new RadarChartOptions
        {
            AngleOffset = angleOffset,
            AggregationOption = AggregationOption.GroupByDataSet
        };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var seriesPath = comp.Find("path.mud-chart-serie");
        seriesPath.Should().NotBeNull();
        seriesPath.GetAttribute("d").Should().NotBeNullOrWhiteSpace("Path 'd' attribute should not be empty with angle offset.");
    }

    [Test]
    public void RadarChart_Should_RenderAxisValues_WhenShowAxisValuesIsTrue()
    {
        var options = new RadarChartOptions
        {
            ShowAxisValues = true,
            GridLevels = 2,
            AggregationOption = AggregationOption.GroupByLabel
        };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.FindAll("text.mud-chart-axis-value").Count.Should().BeGreaterThan(0);
    }

    [Test]
    public void RadarChart_Should_NotRenderAxisValues_WhenShowAxisValuesIsFalse()
    {
        var options = new RadarChartOptions
        {
            ShowAxisValues = false,
            GridLevels = 2,
            AggregationOption = AggregationOption.GroupByLabel
        };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.FindAll("text.mud-chart-axis-value").Count.Should().Be(0);
    }

    [Test]
    public void RadarChart_Should_RenderEmpty_WhenChartSeriesIsNull()
    {
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, null)
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" }) // Provide labels to define axes
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.Markup.Should().Contain("<svg");
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(0);
    }

    [Test]
    public void RadarChart_Should_RenderEmpty_WhenChartSeriesIsEmpty()
    {
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>>())
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" }) // Provide labels to define axes
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.Markup.Should().Contain("<svg");
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(0);
    }

    [Test]
    public void RadarChart_Should_HandleSeriesWithNullData()
    {
        var chartSeries = new List<ChartSeries<double>>
        {
            new() { Name = "Series1", Data = null },
            new() { Name = "Series2", Data = new double[] { 10, 20, 30 } }
        };
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.Markup.Should().Contain("<svg");
        // Only the valid series should be rendered
        var seriesPaths = comp.FindAll("path.mud-chart-serie");
        seriesPaths.Count.Should().Be(1);
    }

    [Test]
    public void RadarChart_Should_HandleSeriesWithEmptyData()
    {
        var chartSeries = new List<ChartSeries<double>>
        {
            new() { Name = "Series1", Data = new double[0] },
            new() { Name = "Series2", Data = new double[] { 10, 20, 30 } }
        };
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.Markup.Should().Contain("<svg");
        // Only the valid series should be rendered
        var seriesPaths = comp.FindAll("path.mud-chart-serie");
        seriesPaths.Count.Should().Be(1);
    }

    [Test]
    public void RadarChart_Should_HandleNonUniformDataLengths_WithGroupByLabel()
    {
        var chartSeries = new List<ChartSeries<double>>
        {
            new() { Name = "SeriesA", Data = new double[] { 10, 20, 30 } }, // Full data
            new() { Name = "SeriesB", Data = new double[] { 15, 25 } }      // Shorter data
        };
        var chartLabels = new string[] { "Label1", "Label2", "Label3" };
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByLabel };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "400px")
            .Add(p => p.Height, "400px")
        );

        comp.Markup.Should().Contain("<svg");
        var seriesPaths = comp.FindAll("path.mud-chart-serie");

        seriesPaths.Count.Should().Be(chartLabels.Length);

        foreach (var path in seriesPaths)
        {
            path.GetAttribute("d").Should().NotBeNullOrWhiteSpace();
        }
    }

    [Test]
    public void RadarChart_Should_HandleExtremeDataValues()
    {
        var chartSeries = new List<ChartSeries<double>>
        {
            new() { Name = "Series1", Data = new[] { double.MaxValue / 1000, 0, double.MinValue / 1000, 50 } }
        };
        var chartLabels = new string[] { "Max", "Zero", "Min", "Normal" };
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "400px")
            .Add(p => p.Height, "400px")
        );

        comp.Markup.Should().Contain("<svg");
        var seriesPath = comp.Find("path.mud-chart-serie");
        seriesPath.Should().NotBeNull();
        var pathData = seriesPath.GetAttribute("d");
        pathData.Should().NotBeNullOrWhiteSpace();
        pathData.Should().NotContain("NaN", "Path data should not contain NaN for extreme values.");
    }

    [Test]
    public void RadarChart_Should_RenderCorrectly_WithAllZeroDataValues()
    {
        var chartSeries = new List<ChartSeries<double>>
        {
            new() { Name = "Series1", Data = new double[] { 0, 0, 0, 0 } }
        };
        var chartLabels = new string[] { "A", "B", "C", "D" };
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.Markup.Should().Contain("<svg");
        var seriesPath = comp.Find("path.mud-chart-serie");
        seriesPath.Should().NotBeNull();
        var pathData = seriesPath.GetAttribute("d");
        pathData.Should().NotBeNullOrWhiteSpace();
        pathData.Should().NotContain("NaN");
    }

    [Test]
    public void RadarChart_Should_RenderCorrectly_WithAllIdenticalDataValues()
    {
        var chartSeries = new List<ChartSeries<double>>
        {
            new() { Name = "Series1", Data = new double[] { 50, 50, 50, 50 } }
        };
        var chartLabels = new string[] { "A", "B", "C", "D" };
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.Markup.Should().Contain("<svg");
        var seriesPath = comp.Find("path.mud-chart-serie");
        seriesPath.Should().NotBeNull();
        var pathData = seriesPath.GetAttribute("d");
        pathData.Should().NotBeNullOrWhiteSpace();
        pathData.Should().NotContain("NaN");
        pathData.Should().StartWith("M").And.EndWith("Z");
        var commands = pathData.Split(new[] { 'M', 'L', 'Z', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        commands.Length.Should().Be(chartLabels.Length * 2);
    }

    [Test]
    public void RadarChart_Should_RenderCorrectNumberOfAxisLines_BasedOnLabels_GroupByDataSet()
    {
        var chartLabels = new[] { "Axis One", "Axis Two", "Axis Three" };
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet, ShowAxisLabels = true };
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.FindAll("path.mud-chart-axis-line").Count.Should().Be(3);
        comp.FindAll("text.mud-chart-axis-label").Count.Should().Be(chartLabels.Length);
    }

    [Test]
    public void RadarChart_Should_RenderCorrectNumberOfAxisLines_BasedOnLabels_GroupByLabel()
    {
        var chartLabels = new[] { "Category X", "Category Y", "Category Z", "Category W" };
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByLabel, ShowAxisLabels = true };
        var chartSeries = new List<ChartSeries<double>>
        {
            new() { Name = "Series1", Data = new double[] { 10, 20, 30, 40 } },
            new() { Name = "Series2", Data = new double[] { 15, 25, 35, 45 } }
        };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.FindAll("path.mud-chart-axis-line").Count.Should().Be(2);
        comp.FindAll("text.mud-chart-axis-label").Count.Should().Be(chartSeries.Count);
        comp.FindAll("circle.mud-chart-series-point").Count.Should().Be(chartSeries.Sum(x => x.Data.Count));
    }

    [Test]
    public void RadarChart_Should_RenderAxisLabels_WithLongNamesAndSpecialChars()
    {
        var chartLabels = new[] { "Very Long Axis Label Name That Might Cause Wrapping or Truncation Issues", "Axis with !@#$%^&*()_+[]{};:'\",.<>/?\\|`~", "Short" };
        var options = new RadarChartOptions { ShowAxisLabels = true, AggregationOption = AggregationOption.GroupByDataSet }; // GroupByDataSet for simpler label mapping
        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "500px")
            .Add(p => p.Height, "500px")
        );

        var renderedLabels = comp.FindAll("text.mud-chart-axis-label");
        renderedLabels.Count.Should().Be(chartLabels.Length);

        for (var i = 0; i < chartLabels.Length; i++)
        {
            renderedLabels[i].TextContent.Should().Be(chartLabels[i]);
        }
    }

    [Test]
    public void RadarChart_Should_HandleMismatchedLabels_LessThanData_GroupByDataSet()
    {
        var seriesData = new double[] { 10, 20, 30, 40 };
        var chartLabels = new[] { "A", "B" }; // Fewer labels than data points
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet, ShowAxisLabels = true };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = seriesData } })
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var renderedAxisLines = comp.FindAll("path.mud-chart-axis-line");
        renderedAxisLines.Count.Should().Be(2);

        var renderedLabels = comp.FindAll("text.mud-chart-axis-label");
        renderedLabels.Count.Should().Be(2);

        renderedLabels[0].TextContent.Should().Be("A");
        renderedLabels[1].TextContent.Should().Be("B");
    }

    [Test]
    public void RadarChart_Should_HandleMismatchedLabels_MoreThanData_GroupByDataSet()
    {
        var seriesData = new double[] { 10, 20, 30 };
        var chartLabels = new[] { "A", "B", "C", "D", "E" }; // More labels than data points
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet, ShowAxisLabels = true };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = seriesData } })
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var renderedAxisLines = comp.FindAll("path.mud-chart-axis-line");
        renderedAxisLines.Count.Should().Be(5);

        var renderedLabels = comp.FindAll("text.mud-chart-axis-label");
        renderedLabels.Count.Should().Be(chartLabels.Length);

        renderedLabels[0].TextContent.Should().Be("A");
        renderedLabels[1].TextContent.Should().Be("B");
        renderedLabels[4].TextContent.Should().Be("E");
    }

    [Test]
    public async Task RadarChart_Should_ShowDefaultTooltip_OnSeriesPathHoverAsync()
    {
        var seriesName = "My Series";
        var seriesData = new double[] { 10, 20, 30 };
        var chartLabels = new[] { "Cost", "Performance", "Usability" };
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = seriesName, Data = seriesData } })
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var seriesPath = comp.Find("path.mud-chart-serie");
        seriesPath.Should().NotBeNull();

        // Simulate mouseover to trigger tooltip
        await seriesPath.TriggerEventAsync("onmouseover", new MouseEventArgs());

        var tooltip = comp.Find("g.svg-tooltip");
        tooltip.Should().NotBeNull();
        tooltip.TextContent.Should().Contain(seriesName);
    }

    [Test]
    public async Task RadarChart_Should_ShowCustomTooltip_WithTooltipTemplateAsync()
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var seriesName = "Custom Series";
        var seriesData = new double[] { 42, 69, 88 };
        var chartLabels = new[] { "X", "Y", "Z" };
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet };

        RenderFragment CustomTooltip((SvgPath Segment, string Color) info) => builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "custom-tooltip-test");
            builder.AddContent(2, $"Series: {info.Segment.LabelYValue}, Index: {info.Segment.Index}, Value: {info.Segment.LabelXValue}");
            builder.CloseElement();
        };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = seriesName, Data = seriesData } })
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.TooltipTemplate, CustomTooltip)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var seriesPath = comp.Find("path.mud-chart-serie");
        seriesPath.Should().NotBeNull();
        await seriesPath.TriggerEventAsync("onmouseover", new MouseEventArgs());

        var tooltipContent = comp.Find("div.custom-tooltip-test");
        tooltipContent.Should().NotBeNull();
        tooltipContent.TextContent.Should().Contain($"Series: {seriesName}");
        tooltipContent.TextContent.Should().Contain("Index: 0");
    }

    [Test]
    public async Task RadarChart_Should_ShowTooltip_OnDataMarkerHover()
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var seriesName = "Marker Series";
        var seriesData = new double[] { 15, 25, 35 };
        var chartLabels = new[] { "Alpha", "Beta", "Gamma" };
        var options = new RadarChartOptions { ShowDataMarkers = true, DataPointRadius = 3, AggregationOption = AggregationOption.GroupByDataSet };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = seriesName, Data = seriesData } })
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var dataMarkers = comp.FindAll("circle.mud-chart-series-point");
        dataMarkers.Count.Should().Be(seriesData.Length);

        var firstMarker = dataMarkers.First();
        await firstMarker.TriggerEventAsync("onmouseover", new MouseEventArgs());

        var tooltip = comp.Find("g.svg-tooltip");
        tooltip.Should().NotBeNull();

        tooltip.TextContent.Should().Contain(seriesName);
        tooltip.TextContent.Should().Contain(seriesData[0].ToString());

        // Test with Custom Tooltip Template
        RenderFragment CustomTooltip((SvgPath Segment, string Color) info) => builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "custom-tooltip-test");
            builder.AddContent(2, $"{info.Segment.LabelYValue}, Value: {info.Segment.LabelXValue}");
            builder.CloseElement();
        };

        await comp.SetParametersAndRenderAsync(parameters => parameters
                  .Add(p => p.TooltipTemplate, CustomTooltip));

        // Re-find marker and re-trigger hover after re-render
        dataMarkers = comp.FindAll("circle.mud-chart-series-point");
        firstMarker = dataMarkers.First();
        await firstMarker.TriggerEventAsync("onmouseover", new MouseEventArgs());

        var customTooltipContent = comp.Find("div.custom-tooltip-test");
        customTooltipContent.Should().NotBeNull();
        customTooltipContent.TextContent.Should().Be($"{seriesName}, Value: {seriesData[0]}");

        // Hover on the second marker
        dataMarkers = comp.FindAll("circle.mud-chart-series-point");
        var secondMarker = dataMarkers[1];
        await secondMarker.TriggerEventAsync("onmouseover", new MouseEventArgs());
        customTooltipContent = comp.Find("div.custom-tooltip-test"); // Re-find, content should update
        customTooltipContent.TextContent.Should().Be($"{seriesName}, Value: {seriesData[1]}");
    }

    [Test]
    public async Task RadarChart_Should_HideTooltip_OnMouseOutAsync()
    {
        Context.JSInterop.Mode = JSRuntimeMode.Loose;
        var seriesName = "Hide Test Series";
        var seriesData = new double[] { 5, 10, 15 };
        var chartLabels = new[] { "P1", "P2", "P3" };
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = seriesName, Data = seriesData } })
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var seriesPath = comp.Find("path.mud-chart-serie");
        seriesPath.Should().NotBeNull();

        // Mouse over to show tooltip
        await seriesPath.TriggerEventAsync("onmouseover", new MouseEventArgs());
        var tooltip = comp.Find("g.svg-tooltip");
        tooltip.Should().NotBeNull("Tooltip should be visible on mouseover.");

        // Mouse out to hide tooltip
        await seriesPath.TriggerEventAsync("onmouseout", new MouseEventArgs());
        comp.FindAll("g.svg-tooltip").Count.Should().Be(0, "Tooltip content should be removed or hidden on mouseout.");
    }

    [Test]
    public async Task RadarChart_Should_ReRender_WhenChartSeriesIsUpdated()
    {
        var initialSeries = new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } };
        var chartLabels = new[] { "A", "B", "C" };
        var options = new RadarChartOptions { AggregationOption = AggregationOption.GroupByDataSet };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, initialSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var seriesPaths = comp.FindAll("path.mud-chart-serie");
        seriesPaths.Count.Should().Be(1, "Initial render should have one series path.");
        var initialPathD = seriesPaths[0].GetAttribute("d");
        initialPathD.Should().NotBeNullOrWhiteSpace();

        var updatedSeries = new List<ChartSeries<double>>
        {
            new() { Name = "Series1", Data = new double[] { 40, 50, 60 } }, // Modified data
            new() { Name = "Series2", Data = new double[] { 15, 25, 35 } }  // New series
        };

        await comp.SetParametersAndRenderAsync(parameters => parameters
                  .Add(p => p.ChartSeries, updatedSeries));

        seriesPaths = comp.FindAll("path.mud-chart-serie");
        seriesPaths.Count.Should().Be(2, "After update, should have two series paths.");

        var updatedPathD1 = seriesPaths[0].GetAttribute("d");
        updatedPathD1.Should().NotBeNullOrWhiteSpace();
        // Check that the path for the first series has changed due to data modification
        updatedPathD1.Should().NotBe(initialPathD, "Path data for Series1 should change after data update.");

        var updatedPathD2 = seriesPaths[1].GetAttribute("d");
        updatedPathD2.Should().NotBeNullOrWhiteSpace("Path data for new Series2 should exist.");
    }

    [Test]
    public async Task RadarChart_Should_ReRender_WhenChartLabelsAreUpdated()
    {
        var initialLabels = new[] { "Cost", "Performance", "Usability" };
        var seriesData = new List<ChartSeries<double>> { new() { Name = "ProductX", Data = new double[] { 10, 20, 30 } } };
        var options = new RadarChartOptions { ShowAxisLabels = true, AggregationOption = AggregationOption.GroupByDataSet };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, seriesData)
            .Add(p => p.ChartLabels, initialLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var axisLabels = comp.FindAll("text.mud-chart-axis-label");
        axisLabels.Count.Should().Be(initialLabels.Length);
        axisLabels[0].TextContent.Should().Be(initialLabels[0]);

        var newLabels = new[] { "Price", "Speed", "Ease of Use", "Support" }; // Changed names and count
        var updatedSeriesData = new List<ChartSeries<double>> { new() { Name = "ProductX", Data = new double[] { 10, 20, 30, 40 } } };

        await comp.SetParametersAndRenderAsync(parameters => parameters
            .Add(p => p.ChartLabels, newLabels)
            .Add(p => p.ChartSeries, updatedSeriesData));

        axisLabels = comp.FindAll("text.mud-chart-axis-label");
        axisLabels.Count.Should().Be(newLabels.Length);
        axisLabels[0].TextContent.Should().Be(newLabels[0]);
        axisLabels[3].TextContent.Should().Be(newLabels[3]);
    }

    [Test]
    public async Task RadarChart_Should_ReRender_WhenChartOptionsAreUpdated()
    {
        var initialOptions = new RadarChartOptions
        {
            ShowGridLines = true,
            GridLevels = 2,
            AngleOffset = 0,
            AggregationOption = AggregationOption.GroupByDataSet
        };
        var series = new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } };
        var labels = new[] { "A", "B", "C" };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, series)
            .Add(p => p.ChartLabels, labels)
            .Add(p => p.ChartOptions, initialOptions)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.FindAll("path.mud-chart-grid-line").Count.Should().Be(2);
        var initialSeriesPathD = comp.Find("path.mud-chart-serie").GetAttribute("d");

        var newOptions = new RadarChartOptions
        {
            ShowGridLines = false,
            GridLevels = 4,
            AngleOffset = 45,
            AggregationOption = AggregationOption.GroupByDataSet
        };

        await comp.SetParametersAndRenderAsync(parameters => parameters
                  .Add(p => p.ChartOptions, newOptions));

        comp.FindAll("path.mud-chart-grid-line").Count.Should().Be(0, "Grid lines should be hidden after option update.");

        var updatedSeriesPathD = comp.Find("path.mud-chart-serie").GetAttribute("d");
        updatedSeriesPathD.Should().NotBeNullOrWhiteSpace();
        // Path should change due to AngleOffset
        updatedSeriesPathD.Should().NotBe(initialSeriesPathD, "Series path 'd' attribute should change due to AngleOffset update.");

        // Verify GridLevels change if ShowGridLines were true with newOptions
        var optionsGridLevelsTest = new RadarChartOptions
        {
            ShowGridLines = true,
            GridLevels = 5, // Different from initial
            AngleOffset = 45,
            AggregationOption = AggregationOption.GroupByDataSet
        };
        await comp.SetParametersAndRenderAsync(parameters => parameters
                  .Add(p => p.ChartOptions, optionsGridLevelsTest));
        comp.FindAll("path.mud-chart-grid-line").Count.Should().Be(5, "Grid lines should update to new count when shown.");
    }

    [Test]
    public void RadarChart_Should_RenderPathsPerSeries_WhenAggregationIsGroupByDataSet()
    {
        var chartSeries = new List<ChartSeries<double>>
        {
            new() { Name = "Series X", Data = new double[] { 10, 20, 30, 40 } },
            new() { Name = "Series Y", Data = new double[] { 15, 25, 35, 45 } },
            new() { Name = "Series Z", Data = new double[] { 12, 22, 32, 42 } }
        };
        var chartLabels = new[] { "A", "B", "C", "D" };
        var options = new RadarChartOptions
        {
            AggregationOption = AggregationOption.GroupByDataSet,
            ShowAxisLabels = true // To verify axis labels
        };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "400px")
            .Add(p => p.Height, "400px")
        );

        // Assert number of series paths
        var seriesPaths = comp.FindAll("path.mud-chart-serie");
        seriesPaths.Count.Should().Be(chartSeries.Count, "Should render one path per series in GroupByDataSet mode.");

        var axisLabels = comp.FindAll("text.mud-chart-axis-label");
        axisLabels.Count.Should().Be(chartLabels.Length);
        for (var i = 0; i < chartLabels.Length; i++)
        {
            axisLabels[i].TextContent.Should().Be(chartLabels[i]);
        }

        var axisLinePath = comp.Find("path.mud-chart-axis-line");
        axisLinePath.Should().NotBeNull();
    }

    [Test]
    public void RadarChart_Should_RenderPathsPerLabel_WhenAggregationIsGroupByLabel()
    {
        var originalChartSeries = new List<ChartSeries<double>>
        {
            new() { Name = "SeriesX", Data = new double[] { 10, 20 } }, // Original Series 1
            new() { Name = "SeriesY", Data = new double[] { 15, 25 } }, // Original Series 2
            new() { Name = "SeriesZ", Data = new double[] { 12, 22 } }  // Original Series 3
        };
        var originalChartLabels = new[] { "Category Alpha", "Category Beta" };

        var options = new RadarChartOptions
        {
            AggregationOption = AggregationOption.GroupByLabel,
            ShowAxisLabels = true
        };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, originalChartSeries)
            .Add(p => p.ChartLabels, originalChartLabels)
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "400px")
            .Add(p => p.Height, "400px")
        );

        var renderedSeriesPaths = comp.FindAll("path.mud-chart-serie");
        renderedSeriesPaths.Count.Should().Be(originalChartLabels.Length);

        var renderedAxisLabels = comp.FindAll("text.mud-chart-axis-label");
        renderedAxisLabels.Count.Should().Be(originalChartSeries.Count);

        for (var i = 0; i < originalChartSeries.Count; i++)
        {
            renderedAxisLabels[i].TextContent.Should().Be(originalChartSeries[i].Name);
        }

        var axisLinePath = comp.Find("path.mud-chart-axis-line");
        axisLinePath.Should().NotBeNull();
    }

    [Test]
    public void RadarChart_Should_RenderCustomGraphics_WhenProvided()
    {
        RenderFragment customGraphicsFragment = builder =>
        {
            builder.OpenElement(0, "rect");
            builder.AddAttribute(1, "x", "10");
            builder.AddAttribute(2, "y", "10");
            builder.AddAttribute(3, "width", "50");
            builder.AddAttribute(4, "height", "50");
            builder.AddAttribute(5, "fill", "red");
            builder.AddAttribute(6, "class", "custom-graphic-test-rect");
            builder.CloseElement();
        };

        var comp = Context.Render<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.CustomGraphics, customGraphicsFragment)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        // Find the main SVG element of the chart
        var chartSvg = comp.Find("svg");
        chartSvg.Should().NotBeNull();

        // Find the custom graphic element within the chart's SVG
        var customRect = chartSvg.QuerySelector("rect.custom-graphic-test-rect");
        customRect.Should().NotBeNull("Custom graphic rect should be rendered inside the chart SVG.");
        customRect.GetAttribute("fill").Should().Be("red");
        customRect.GetAttribute("x").Should().Be("10");
    }
}
