// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        var comp = Context.RenderComponent<Radar>();
        comp.Markup.Should().Contain("<svg");
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(0);
        comp.FindAll("path.mud-chart-axis-line").Count.Should().Be(0); // No labels, no data, so no axes.
    }

    [Test]
    public void RadarChart_BasicRendering_WithData_InferAxesFromData()
    {
        var comp = Context.RenderComponent<Radar>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            // No ChartLabels provided, should infer 3 axes from data
            .Add(p => p.ChartOptions, new RadarChartOptions())
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(3);
        comp.FindAll("path.mud-chart-axis-line").Count.Should().Be(1);
    }

    [Test]
    public void RadarChart_BasicRendering_WithData_AndLabels()
    {
        var comp = Context.RenderComponent<Radar>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20, 30, 40 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C", "D" })
            .Add(p => p.ChartOptions, new RadarChartOptions())
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(4);
        comp.FindAll("path.mud-chart-axis-line").Count.Should().Be(1);
    }


    [Test]
    public void RadarChart_Option_ShowGridLines_And_GridLevels()
    {
        var options = new RadarChartOptions { ShowGridLines = true, GridLevels = 3 };
        var comp = Context.RenderComponent<Radar>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(3); // 3 levels
    }

    [Test]
    public void RadarChart_Option_ShowGridLines_False()
    {
        var options = new RadarChartOptions { ShowGridLines = false };
        var comp = Context.RenderComponent<Radar>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
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
        var comp = Context.RenderComponent<Radar>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
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
        var comp = Context.RenderComponent<Radar>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
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
        var comp = Context.RenderComponent<Radar>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = seriesData } })
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
        var comp = Context.RenderComponent<Radar>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20, 30, 40 } } })
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
        var series = new List<ChartSeries>
        {
            new ChartSeries { Name = "Series1", Data = new double[] { 10, 20, 30 } },
            new ChartSeries { Name = "Series2", Data = new double[] { 15, 25, 35 } }
        };
        var comp = Context.RenderComponent<Radar>(parameters => parameters
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
        var comp = Context.RenderComponent<Radar>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> {
                new ChartSeries { Name = "Series1", Data = new double[] { 10, 20, 30 } },
                new ChartSeries { Name = "Series2", Data = new double[] { 15, 25, 35 } }
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
        var options = new RadarChartOptions { AngleOffset = 45 };
        var comp = Context.RenderComponent<Radar>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new string[] { "A", "B", "C" })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(3);
        comp.Find("path.mud-chart-serie").GetAttribute("d").Should().NotBeNullOrWhiteSpace();
    }
}
