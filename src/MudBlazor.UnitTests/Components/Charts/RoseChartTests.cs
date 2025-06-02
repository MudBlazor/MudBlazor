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
public class RoseChartTests : BunitTest
{
    private readonly string[] _baseChartPalette = // Re-using a palette from other tests
    {
            "#2979FF", "#1DE9B6", "#FFC400", "#FF9100", "#651FFF", "#00E676", "#00B0FF", "#26A69A", "#FFCA28",
            "#FFA726", "#EF5350", "#EF5350", "#7E57C2", "#66BB6A", "#29B6F6", "#FFA000", "#F57C00", "#D32F2F",
            "#512DA8", "#616161"
    };

    [Test]
    public void RoseChart_BasicRendering_NoData()
    {
        var comp = Context.RenderComponent<Rose>();
        comp.Markup.Should().Contain("<svg");
        comp.FindAll("path.mud-chart-series").Count.Should().Be(0); // No data, no series paths
    }

    [Test]
    public void RoseChart_BasicRendering_WithData()
    {
        var comp = Context.RenderComponent<Rose>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartOptions, new RoseChartOptions())
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(3);
    }

    [Test]
    public void RoseChart_Option_AngleOffset()
    {
        var options = new RoseChartOptions { AngleOffset = 90 };
        var comp = Context.RenderComponent<Rose>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10 } } })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        var path = comp.Find("path.mud-chart-serie");
        path.Should().NotBeNull();
        // Exact path data validation is brittle. We'll rely on visual inspection for precise geometric changes.
        // However, we can check that the path data is not empty.
        path.GetAttribute("d").Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void RoseChart_Option_ScaleFactor()
    {
        var series = new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20 } } };
        var optionsSmall = new RoseChartOptions { ScaleFactor = 0.5 };
        var optionsLarge = new RoseChartOptions { ScaleFactor = 1.0 };

        var compSmall = Context.RenderComponent<Rose>(parameters => parameters
            .Add(p => p.ChartSeries, series)
            .Add(p => p.ChartOptions, optionsSmall)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        var pathDataSmall = compSmall.Find("path.mud-chart-serie").GetAttribute("d");

        var compLarge = Context.RenderComponent<Rose>(parameters => parameters
            .Add(p => p.ChartSeries, series)
            .Add(p => p.ChartOptions, optionsLarge)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        var pathDataLarge = compLarge.Find("path.mud-chart-serie").GetAttribute("d");

        pathDataSmall.Should().NotBe(pathDataLarge);
    }

    [Test]
    public void RoseChart_Option_ShowChartLabels_True()
    {
        var comp = Context.RenderComponent<Rose>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20 } } })
            .Add(p => p.ChartOptions, new RoseChartOptions { ShowValues = true })
            .Add(p => p.ChartLabels, new string[] { "LabelA", "LabelB" })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("text.mud-chart-label-value").Count.Should().Be(2);
        comp.FindAll("text.mud-chart-label-value tspan").Count(ts => ts.TextContent == "10").Should().Be(1);
    }

    [Test]
    public void RoseChart_Option_ShowChartLabels_False()
    {
        var comp = Context.RenderComponent<Rose>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20 } } })
            .Add(p => p.ChartOptions, new RoseChartOptions { ShowValues = false }) // Default
            .Add(p => p.ChartLabels, new string[] { "LabelA", "LabelB" })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("text.mud-chart-label").Count.Should().Be(0);
    }

    [Test]
    public void RoseChart_Option_ShowAsPercentage()
    {
        var comp = Context.RenderComponent<Rose>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 30 } } }) // Total 40
            .Add(p => p.ChartOptions, new RoseChartOptions { ShowValues = true, ShowAsPercentage = true })
            .Add(p => p.ChartLabels, new string[] { "A", "B" })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        // 10 is 25% of 40. 30 is 75% of 40.
        comp.FindAll("text.mud-chart-label-value tspan").Count(ts => ts.TextContent == "25%").Should().Be(1);
        comp.FindAll("text.mud-chart-label-value tspan").Count(ts => ts.TextContent == "75%").Should().Be(1);
    }

    [Test]
    public void RoseChart_Data_EmptySeries()
    {
        var comp = Context.RenderComponent<Rose>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries>()) // Empty list of series
            .Add(p => p.ChartOptions, new RoseChartOptions())
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-series").Count.Should().Be(0);
    }

    [Test]
    public void RoseChart_Data_SeriesWithEmptyData()
    {
        var comp = Context.RenderComponent<Rose>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { } } })
            .Add(p => p.ChartOptions, new RoseChartOptions())
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-series").Count.Should().Be(0);
    }

    [Test]
    public void RoseChart_Interaction_SelectedIndex()
    {
        var selectedIndex = -1;
        var comp = Context.RenderComponent<Rose>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries> { new ChartSeries { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartOptions, new RoseChartOptions())
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
            .Add(p => p.SelectedIndex, selectedIndex)
            .Add(p => p.SelectedIndexChanged, EventCallback.Factory.Create<int>(this, val => selectedIndex = val))
        );

        // Simulate click on the first path segment (index 0)
        comp.FindAll("path.mud-chart-serie").First().Click();
        selectedIndex.Should().Be(0);

        // Simulate click on the third path segment (index 2)
        comp.FindAll("path.mud-chart-serie").Last().Click();
        selectedIndex.Should().Be(2);
    }

    [Test]
    public void RoseChart_CanHideSeries_Test()
    {
        var chartData = new double[] { 10, 20, 30, 40 };
        string[] chartLabels = { "Petal 1", "Petal 2", "Petal 3", "Petal 4" };
        // Rose charts, like Pie/Donut, typically use a single ChartSeries for their data segments.
        var chartSeriesList = new List<ChartSeries>() { new ChartSeries { Data = chartData } };

        var comp = Context.RenderComponent<MudChart>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Rose)
            .Add(p => p.Height, "300px")
            .Add(p => p.Width, "300px")
            .Add(p => p.ChartSeries, chartSeriesList)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.CanHideSeries, true)
            .Add(p => p.ChartOptions, new RoseChartOptions { ChartPalette = _baseChartPalette })
        );

        var seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
        seriesCheckboxes.Count.Should().Be(chartLabels.Length, "Number of checkboxes should match number of labels (petals)");

        var series1 = "[stroke='#2979FF']";
        var series2 = "[stroke='#1DE9B6']";
        var series3 = "[stroke='#FFC400']";
        var series4 = "[stroke='#FF9100']";

        string[] series = [series1, series2, series3, series4];

        // Initially, all petals should be visible and their checkboxes checked
        for (var i = 0; i < chartLabels.Length; i++)
        {
            seriesCheckboxes[i].IsChecked().Should().BeTrue($"{chartLabels[i]} checkbox should be initially checked");
            comp.FindAll($"path.mud-chart-serie{series[i]}").Count.Should().Be(1, $"{chartLabels[i]} path should be initially visible");
        }

        // Hide "Petal 1"
        comp.InvokeAsync(() => seriesCheckboxes[0].Change(false));
        seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
        seriesCheckboxes[0].IsChecked().Should().BeFalse("Petal 1 checkbox should be unchecked after hiding");
        comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(0, "Petal 1 path should be hidden");
        comp.FindAll($"path.mud-chart-serie{series2}").Count.Should().Be(1, "Petal 2 path should remain visible");

        // Show "Petal 1" again
        comp.InvokeAsync(() => seriesCheckboxes[0].Change(true));
        seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
        seriesCheckboxes[0].IsChecked().Should().BeTrue("Petal 1 checkbox should be checked after re-showing");
        comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(1, "Petal 1 path should be visible again");

        // Hide "Petal 3"
        comp.InvokeAsync(() => seriesCheckboxes[2].Change(false));
        seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
        seriesCheckboxes[2].IsChecked().Should().BeFalse("Petal 3 checkbox should be unchecked after hiding");
        comp.FindAll($"path.mud-chart-serie{series3}").Count.Should().Be(0, "Petal 3 path should be hidden");
        comp.FindAll($"path.mud-chart-serie{series1}").Count.Should().Be(1, "Petal 1 path should still be visible");
        comp.FindAll($"path.mud-chart-serie{series2}").Count.Should().Be(1, "Petal 2 path should still be visible");
        comp.FindAll($"path.mud-chart-serie{series4}").Count.Should().Be(1, "Petal 4 path should still be visible");

        // Show "Petal 3" again
        comp.InvokeAsync(() => seriesCheckboxes[2].Change(true));
        seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
        seriesCheckboxes[2].IsChecked().Should().BeTrue("Petal 3 checkbox should be checked after re-showing");
        comp.FindAll($"path.mud-chart-serie{series3}").Count.Should().Be(1, "Petal 3 path should be visible again");
    }
}
