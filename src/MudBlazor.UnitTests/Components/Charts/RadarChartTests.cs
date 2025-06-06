// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts;

[TestFixture]
public class RadarChartTests : BunitTest
{
    [Test]
    public void RadarChart_BasicRendering_NoData()
    {
        var comp = Context.RenderComponent<Radar<double>>();
        comp.Markup.Should().Contain("<svg");
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(0);
        comp.FindAll("path.mud-chart-axis-line").Count.Should().Be(0); // No labels, no data, so no axes.
    }

    [Test]
    public void RadarChart_BasicRendering_WithData_InferAxesFromData()
    {
        var comp = Context.RenderComponent<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            // No ChartLabels provided, should infer 3 axes from data
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
        var comp = Context.RenderComponent<Radar<double>>(parameters => parameters
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
        var comp = Context.RenderComponent<Radar<double>>(parameters => parameters
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
        var comp = Context.RenderComponent<Radar<double>>(parameters => parameters
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
        var comp = Context.RenderComponent<Radar<double>>(parameters => parameters
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
        var comp = Context.RenderComponent<Radar<double>>(parameters => parameters
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
        var comp = Context.RenderComponent<Radar<double>>(parameters => parameters
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
        var comp = Context.RenderComponent<Radar<double>>(parameters => parameters
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
        var comp = Context.RenderComponent<Radar<double>>(parameters => parameters
            .Add(p => p.ChartSeries, series)
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, new RadarChartOptions() { AggregationOption = AggregationOption.GroupByDataSet })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(2);
    }

    [Test]
    public void RadarChart_Interaction_SelectedIndex()
    {
        var selectedIndex = -1;
        var comp = Context.RenderComponent<Radar<double>>(parameters => parameters
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
        comp.FindAll("path.mud-chart-serie").First().Click();
        selectedIndex.Should().Be(0);

        // Simulate click on the second series path (index 1)
        comp.FindAll("path.mud-chart-serie").Last().Click();
        selectedIndex.Should().Be(1);
    }

    [Test]
    public void RadarChart_Option_AngleOffset()
    {
        // Exact path data validation for AngleOffset is complex and brittle.
        // This test primarily ensures that the component renders without error when AngleOffset is used.
        // Visual inspection during development is important for verifying the geometric correctness.
        var options = new RadarChartOptions { AngleOffset = 45, AggregationOption = AggregationOption.GroupByLabel };
        var comp = Context.RenderComponent<Radar<double>>(parameters => parameters
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
    public void RadarChart_CanHideSeries_Test()
    {
        var chartSeries = new List<ChartSeries<double>>()
        {
            new () { Name = "Series 1", Data = new double[] { 90, 79, 72, 69 } },
            new () { Name = "Series 2", Data = new double[] { 10, 41, 35, 51 } },
            new () { Name = "Series 3", Data = new double[] { 60, 20, 85, 30 }, Visible = false } // Initially hidden
        };
        string[] xAxisLabels = { "Cat A", "Cat B", "Cat C", "Cat D" };

        var comp = Context.RenderComponent<MudChart<double>>(parameters => parameters
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
        comp.InvokeAsync(() => seriesCheckboxes[0].Change(false));
        seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
        seriesCheckboxes[0].IsChecked().Should().BeFalse("Series 1 checkbox should be unchecked after hiding");
        chartSeries[0].Visible.Should().BeFalse("Series 1 Visible property should be false");
        comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(0, "Series 1 path should be hidden after unchecking");

        // Show Series 1 again
        comp.InvokeAsync(() => seriesCheckboxes[0].Change(true));
        seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
        seriesCheckboxes[0].IsChecked().Should().BeTrue("Series 1 checkbox should be checked after showing");
        chartSeries[0].Visible.Should().BeTrue("Series 1 Visible property should be true");
        comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(1, "Series 1 path should be visible again after re-checking");

        // Hide Series 2
        comp.InvokeAsync(() => seriesCheckboxes[1].Change(false));
        seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
        seriesCheckboxes[1].IsChecked().Should().BeFalse("Series 2 checkbox should be unchecked after hiding");
        chartSeries[1].Visible.Should().BeFalse("Series 2 Visible property should be false");
        comp.FindAll($"path.mud-chart-serie{series2}").Count.Should().Be(0, "Series 2 path should be hidden");
        comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(1, "Series 1 path should still be visible"); // Ensure other series not affected

        // Show Series 3 (which was initially hidden)
        comp.InvokeAsync(() => seriesCheckboxes[2].Change(true));
        seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
        seriesCheckboxes[2].IsChecked().Should().BeTrue("Series 3 checkbox should be checked after showing");
        chartSeries[2].Visible.Should().BeTrue("Series 3 Visible property should be true");
        comp.FindAll($"path.mud-chart-serie{series3}").Count.Should().Be(1, "Series 3 path should be visible after checking");
    }
}
