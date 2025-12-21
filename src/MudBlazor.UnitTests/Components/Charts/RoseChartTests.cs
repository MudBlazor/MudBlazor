// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Charts;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts;

[TestFixture]
public class RoseChartTests : BunitTest
{
    private readonly string[] _baseChartPalette =
    {
            "#2979FF", "#1DE9B6", "#FFC400", "#FF9100", "#651FFF", "#00E676", "#00B0FF", "#26A69A", "#FFCA28",
            "#FFA726", "#EF5350", "#EF5350", "#7E57C2", "#66BB6A", "#29B6F6", "#FFA000", "#F57C00", "#D32F2F",
            "#512DA8", "#616161"
    };

    [Test]
    public void RoseChart_BasicRendering_NoData()
    {
        var comp = Context.Render<Rose<double>>();
        comp.Markup.Should().Contain("<svg");
        comp.FindAll("path.mud-chart-series").Count.Should().Be(0); // No data, no series paths
    }

    [Test]
    public void RoseChart_BasicRendering_WithData()
    {
        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
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
        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10 } } })
            .Add(p => p.ChartOptions, options)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        var path = comp.Find("path.mud-chart-serie");
        path.Should().NotBeNull();
        // Check that the path data is not empty.
        path.GetAttribute("d").Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void RoseChart_Option_ScaleFactor()
    {
        var series = new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20 } } };
        var optionsSmall = new RoseChartOptions { ScaleFactor = 0.5 };
        var optionsLarge = new RoseChartOptions { ScaleFactor = 1.0 };

        var compSmall = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, series)
            .Add(p => p.ChartOptions, optionsSmall)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        var pathDataSmall = compSmall.Find("path.mud-chart-serie").GetAttribute("d");

        var compLarge = Context.Render<Rose<double>>(parameters => parameters
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
        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20 } } })
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
        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20 } } })
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
        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 30 } } }) // Total 40
            .Add(p => p.ChartOptions, new RoseChartOptions { ShowValues = true, ShowAsPercentage = true })
            .Add(p => p.ChartLabels, new string[] { "A", "B" })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.FindAll("text.mud-chart-label-value tspan").Count(ts => ts.TextContent == "25%").Should().Be(1);
        comp.FindAll("text.mud-chart-label-value tspan").Count(ts => ts.TextContent == "75%").Should().Be(1);
    }

    [Test]
    public void RoseChart_Data_EmptySeries()
    {
        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>>()) // Empty list of series
            .Add(p => p.ChartOptions, new RoseChartOptions())
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
        comp.FindAll("path.mud-chart-series").Count.Should().Be(0);
    }

    [Test]
    public void RoseChart_Data_SeriesWithEmptyData()
    {
        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { } } })
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
        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartOptions, new RoseChartOptions())
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
            .Add(p => p.SelectedIndex, selectedIndex)
            .Add(p => p.SelectedIndexChanged, EventCallback.Factory.Create<int>(this, val => selectedIndex = val))
        );

        // Click on the first path segment (index 0)
        comp.FindAll("path.mud-chart-serie").First().Click();
        selectedIndex.Should().Be(0);

        // Click on the third path segment (index 2)
        comp.FindAll("path.mud-chart-serie").Last().Click();
        selectedIndex.Should().Be(2);
    }

    [Test]
    public void RoseChart_CanHideSeries_Test()
    {
        var chartData = new double[] { 10, 20, 30, 40 };
        string[] chartLabels = { "Petal 1", "Petal 2", "Petal 3", "Petal 4" };
        var chartSeriesList = new List<ChartSeries<double>>() { new() { Data = chartData } };

        var comp = Context.Render<MudChart<double>>(parameters => parameters
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

    [Test]
    public void RoseChart_ChartLabels_NotSet()
    {
        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20 } } })
            .Add(p => p.ChartOptions, new RoseChartOptions { ShowValues = true, ShowLegend = true })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.FindAll("path.mud-chart-serie").Count.Should().Be(2); // Petals should render

        var valueLabels = comp.FindAll("text.mud-chart-label-value tspan");
        valueLabels.Should().NotBeNull();
        valueLabels.Count(ts => !string.IsNullOrWhiteSpace(ts.TextContent) && (ts.TextContent == "10" || ts.TextContent == "20")).Should().Be(2); // only values, no extra labels

        var legendItems = comp.FindAll(".mud-chart-legend-item");
        legendItems.Count.Should().Be(0);
    }

    [Test]
    public void RoseChart_ChartLabels_MoreLabelsThanData()
    {
        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20 } } })
            .Add(p => p.ChartOptions, new RoseChartOptions { ShowValues = true, ShowLegend = true })
            .Add(p => p.ChartLabels, new string[] { "LabelA", "LabelB", "LabelC" })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        comp.FindAll("path.mud-chart-serie").Count.Should().Be(2);

        var valueLabels = comp.FindAll("text.mud-chart-label-value");
        valueLabels.Count.Should().Be(2);
        valueLabels.Any(vl => vl.TextContent.Contains("LabelA")).Should().BeFalse();
        valueLabels.Any(vl => vl.TextContent.Contains("10")).Should().BeTrue();
        valueLabels.Any(vl => vl.TextContent.Contains("LabelB")).Should().BeFalse();
        valueLabels.Any(vl => vl.TextContent.Contains("20")).Should().BeTrue();

        // Check legend items
        var legendItems = comp.FindAll(".mud-chart-legend-item");
        legendItems.Count.Should().Be(3);
    }

    [Test]
    public void RoseChart_CustomGraphics_ShouldRenderCustomSvg()
    {
        RenderFragment customSvg = builder =>
        {
            builder.OpenElement(0, "rect");
            builder.AddAttribute(1, "id", "custom-rect-test");
            builder.AddAttribute(2, "x", "5");
            builder.AddAttribute(3, "y", "5");
            builder.AddAttribute(4, "width", "15");
            builder.AddAttribute(5, "height", "15");
            builder.AddAttribute(6, "fill", "green");
            builder.CloseElement();

            builder.OpenElement(7, "text");
            builder.AddAttribute(8, "id", "custom-text-test");
            builder.AddAttribute(9, "x", "50");
            builder.AddAttribute(10, "y", "50");
            builder.AddContent(11, "Custom SVG Text");
            builder.CloseElement();
        };

        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartOptions, new RoseChartOptions())
            .Add(p => p.CustomGraphics, customSvg)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        // Assert that standard chart elements are present
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(3); // 3 petals

        // Assert that custom SVG elements are present
        var customRect = comp.Find("#custom-rect-test");
        customRect.Should().NotBeNull();
        customRect.GetAttribute("fill").Should().Be("green");

        var customText = comp.Find("#custom-text-test");
        customText.Should().NotBeNull();
        customText.TextContent.Should().Be("Custom SVG Text");
    }

    [Test]
    public void RoseChart_Dimensions_ShouldRenderWithDefaults()
    {
        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20 } } })
        );

        var chartDiv = comp.Find("div.mud-chart");
        chartDiv.Should().NotBeNull();

        // Check the svg element
        var svgElement = comp.Find("svg");
        svgElement.Should().NotBeNull();

        svgElement.GetAttribute("width").Should().Be("80%");
        svgElement.GetAttribute("height").Should().Be("80%");


        // Assert that chart content (petals) are defined
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(2);
    }

    [Test]
    public void RoseChart_MatchBoundsToSize_False_ShouldRespectExplicitDimensions()
    {
        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.MatchBoundsToSize, false)
            .Add(p => p.Width, "250px")
            .Add(p => p.Height, "250px")
        );

        // Check the div wrapper style
        var chartDiv = comp.Find("svg.mud-chart-rose");
        chartDiv.Should().NotBeNull();
        chartDiv.GetAttribute("width").Should().Be("250px");
        chartDiv.GetAttribute("height").Should().Be("250px");

        // Check that SVG and petals are rendered
        comp.Find("svg").Should().NotBeNull();
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(3);
    }

    private IRenderedComponent<MudChart<double>> RenderRoseChartWithLegend(Position legendPosition, bool rtl = false)
    {
        return Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Rose)
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Name = "Series1", Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.ChartLabels, new[] { "LabelA", "LabelB", "LabelC" })
            .Add(p => p.LegendPosition, legendPosition)
            .Add(p => p.RightToLeft, rtl)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );
    }

    [Test]
    public void RoseChart_LegendPosition_Top_ShouldRenderLegendAtTop()
    {
        var comp = RenderRoseChartWithLegend(Position.Top);
        comp.Find("div.mud-chart").ClassList.Should().Contain("mud-chart-legend-top");
    }

    [Test]
    public void RoseChart_LegendPosition_Left_ShouldRenderLegendAtLeft()
    {
        var comp = RenderRoseChartWithLegend(Position.Left);
        comp.Find("div.mud-chart").ClassList.Should().Contain("mud-chart-legend-left");
    }

    [Test]
    public void RoseChart_LegendPosition_Right_ShouldRenderLegendAtRight()
    {
        var comp = RenderRoseChartWithLegend(Position.Right);
        comp.Find("div.mud-chart").ClassList.Should().Contain("mud-chart-legend-right");
    }

    [Test]
    public void RoseChart_LegendPosition_Bottom_ShouldRenderLegendAtBottom()
    {
        var comp = RenderRoseChartWithLegend(Position.Bottom);
        comp.Find("div.mud-chart").ClassList.Should().Contain("mud-chart-legend-bottom");
    }

    [Test]
    public void RoseChart_LegendPosition_Start_RTL_False_ShouldRenderLegendAtLeft()
    {
        var comp = RenderRoseChartWithLegend(Position.Start, rtl: false);
        comp.Find("div.mud-chart").ClassList.Should().Contain("mud-chart-legend-left");
    }

    [Test]
    public void RoseChart_LegendPosition_Start_RTL_True_ShouldRenderLegendAtRight()
    {
        var comp = RenderRoseChartWithLegend(Position.Start, rtl: true);
        comp.Find("div.mud-chart").ClassList.Should().Contain("mud-chart-legend-right");
    }

    [Test]
    public void RoseChart_LegendPosition_End_RTL_False_ShouldRenderLegendAtRight()
    {
        var comp = RenderRoseChartWithLegend(Position.End, rtl: false);
        comp.Find("div.mud-chart").ClassList.Should().Contain("mud-chart-legend-right");
    }

    [Test]
    public void RoseChart_LegendPosition_End_RTL_True_ShouldRenderLegendAtLeft()
    {
        var comp = RenderRoseChartWithLegend(Position.End, rtl: true);
        comp.Find("div.mud-chart").ClassList.Should().Contain("mud-chart-legend-left");
    }

    [Test]
    public void RoseChart_CanHideSeries_WithAggregationByDataSet_ShouldHideCorrectSeriesPetal()
    {
        var chartSeries = new List<ChartSeries<double>>
        {
            new() { Name = "Set A", Data = new double[] { 10, 20 } }, // Sum = 30
            new() { Name = "Set B", Data = new double[] { 30 } }      // Sum = 30
        };

        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Rose)
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartOptions, new RoseChartOptions { AggregationOption = AggregationOption.GroupByDataSet, ChartPalette = _baseChartPalette })
            .Add(p => p.CanHideSeries, true)
            .Add(p => p.Width, "400px")
            .Add(p => p.Height, "400px")
        );

        var legendItems = comp.FindAll(".mud-chart-legend-item");
        legendItems.Count.Should().Be(2, "Should be one legend item per ChartSeries");
        var legendCheckboxes = comp.FindAll(".mud-checkbox-input");
        legendCheckboxes.Count.Should().Be(2);

        var legendItemSetA = legendItems.FirstOrDefault(li => li.TextContent.Contains("Set A"));
        legendItemSetA.Should().NotBeNull();
        var checkboxSetA = legendItemSetA.QuerySelector(".mud-checkbox-input");
        checkboxSetA.Should().NotBeNull();
        checkboxSetA.IsChecked().Should().BeTrue();

        var legendItemSetB = legendItems.FirstOrDefault(li => li.TextContent.Contains("Set B"));
        legendItemSetB.Should().NotBeNull();
        var checkboxSetB = legendItemSetB.QuerySelector(".mud-checkbox-input");
        checkboxSetB.Should().NotBeNull();
        checkboxSetB.IsChecked().Should().BeTrue();

        var petals = comp.FindAll("path.mud-chart-serie");
        petals.Count.Should().Be(2, "Should be one petal per ChartSeries with GroupByDataSet");

        comp.FindAll($"path.mud-chart-serie[stroke='{_baseChartPalette[0]}']").Count.Should().Be(1, "Petal for Set A with correct color should exist.");
        comp.FindAll($"path.mud-chart-serie[stroke='{_baseChartPalette[1]}']").Count.Should().Be(1, "Petal for Set B with correct color should exist.");


        // Hide "Set A"
        comp.InvokeAsync(() => checkboxSetA.Change(false));

        legendCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-query after change
        legendCheckboxes[0].IsChecked().Should().BeFalse("Checkbox for Set A should be unchecked");
        legendCheckboxes[1].IsChecked().Should().BeTrue("Checkbox for Set B should remain checked");


        comp.FindAll($"path.mud-chart-serie[stroke='{_baseChartPalette[0]}']").Count.Should().Be(0, "Petal for Set A should be hidden.");
        comp.FindAll($"path.mud-chart-serie[stroke='{_baseChartPalette[1]}']").Count.Should().Be(1, "Petal for Set B should still be visible.");
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(1); // Only Set B's petal remains

        // Show "Set A" Again
        comp.InvokeAsync(() => checkboxSetA.Change(true));

        legendCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-query
        legendCheckboxes[0].IsChecked().Should().BeTrue("Checkbox for Set A should be checked again");

        comp.FindAll($"path.mud-chart-serie[stroke='{_baseChartPalette[0]}']").Count.Should().Be(1, "Petal for Set A should be visible again.");
        comp.FindAll($"path.mud-chart-serie[stroke='{_baseChartPalette[1]}']").Count.Should().Be(1, "Petal for Set B should still be visible.");
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(2); // Both petals visible again
    }

    [Test]
    public void RoseChart_AggregationOption_GroupByDataSet_ShouldAggregateDataAndRenderPetalsPerSeries()
    {
        var chartSeries = new List<ChartSeries<double>>
        {
            new() { Name = "Alpha", Data = new double[] { 10, 5 } },  // Sum = 15
            new() { Name = "Beta",  Data = new double[] { 20, 20 } }, // Sum = 40
            new() { Name = "Gamma", Data = new double[] { 5 } }       // Sum = 5
        };

        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartOptions, new RoseChartOptions { AggregationOption = AggregationOption.GroupByDataSet, ShowValues = true, ShowLegend = true })
            .Add(p => p.Width, "400px")
            .Add(p => p.Height, "400px")
        );

        // Assertions
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(3, "Should render one petal per series");

        var legendItems = comp.FindAll(".mud-chart-legend-item");
        legendItems.Count.Should().Be(3);
        legendItems.Any(li => li.TextContent.Trim() == "Alpha").Should().BeTrue();
        legendItems.Any(li => li.TextContent.Trim() == "Beta").Should().BeTrue();
        legendItems.Any(li => li.TextContent.Trim() == "Gamma").Should().BeTrue();

        var valueLabels = comp.FindAll("text.mud-chart-label-value tspan");
        valueLabels.Count.Should().Be(3, "Should display one value label per series");
        valueLabels.Any(vl => vl.TextContent.Trim() == "15").Should().BeTrue("Value for Alpha (10+5) should be 15");
        valueLabels.Any(vl => vl.TextContent.Trim() == "40").Should().BeTrue("Value for Beta (20+20) should be 40");
        valueLabels.Any(vl => vl.TextContent.Trim() == "5").Should().BeTrue("Value for Gamma (5) should be 5");

        var petalPaths = comp.FindAll("path.mud-chart-serie").Select(p => p.GetAttribute("d")).ToList();
        var valueLabelElements = comp.FindAll("text.mud-chart-label-value").ToList();

        // Find the path associated with the "40" label (Beta series)
        string pathForBeta = null;
        for (int i = 0; i < valueLabelElements.Count; i++)
        {
            if (valueLabelElements[i].TextContent.Contains("40")) // Check if the tspan inside contains "40"
            {
                pathForBeta = petalPaths[i];
                break;
            }
        }
        pathForBeta.Should().NotBeNull("Path for Beta series (value 40) should be found");

        string pathForGamma = null;
        for (int i = 0; i < valueLabelElements.Count; i++)
        {
            if (valueLabelElements[i].TextContent.Contains("5"))
            {
                pathForGamma = petalPaths[i];
                break;
            }
        }
        pathForGamma.Should().NotBeNull("Path for Gamma series (value 5) should be found");
    }

    [Test]
    public void RoseChart_AggregationOption_GroupByLabel_ShouldAggregateDataAndRenderPetalsPerLabel()
    {
        var chartLabels = new[] { "X", "Y" };
        var chartSeries = new List<ChartSeries<double>>
        {
            new() { Name = "s1", Data = new double[] { 10, 20 } }, // X=10, Y=20
            new() { Name = "s2", Data = new double[] { 5,  15 } }  // X=5,  Y=15
        };

        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, new RoseChartOptions { AggregationOption = AggregationOption.GroupByLabel, ShowValues = true, ShowLegend = true })
            .Add(p => p.Width, "400px")
            .Add(p => p.Height, "400px")
        );

        // Assertions
        comp.FindAll("path.mud-chart-serie").Count.Should().Be(2, "Should render one petal per label");

        var legendItems = comp.FindAll(".mud-chart-legend-item");
        legendItems.Count.Should().Be(2);
        legendItems.Any(li => li.TextContent.Trim() == "X").Should().BeTrue();
        legendItems.Any(li => li.TextContent.Trim() == "Y").Should().BeTrue();

        var valueLabels = comp.FindAll("text.mud-chart-label-value tspan");
        valueLabels.Count.Should().Be(2, "Should display one value label per aggregated label");
        valueLabels.Any(vl => vl.TextContent.Trim() == "15").Should().BeTrue("Aggregated value for X (10+5) should be 15");
        valueLabels.Any(vl => vl.TextContent.Trim() == "35").Should().BeTrue("Aggregated value for Y (20+15) should be 35");
    }

    [Test]
    public void RoseChart_Tooltips_ShouldDisplayDefaultTooltip_OnPetalHover_When_ShowToolTipsTrue()
    {
        var chartSeries = new List<ChartSeries<double>> { new() { Data = new[] { 10.0, 20.0 } } };
        var chartLabels = new[] { "A", "B" };

        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Rose)
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, new RoseChartOptions { ShowToolTips = true })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var petals = comp.FindAll("path.mud-chart-serie");
        petals.Count.Should().Be(2);
        var firstPetal = petals.First();

        firstPetal.TriggerEvent("onmouseover", new MouseEventArgs());

        var tooltip = comp.Find("g.svg-tooltip");
        tooltip.Should().NotBeNull("Tooltip should be present in the DOM after mouseover");

        tooltip.QuerySelector("text tspan").InnerHtml.Should().Be("A - 10");

        firstPetal.TriggerEvent("onmouseout", new MouseEventArgs());
        comp.FindAll("g.svg-tooltip").Should().BeEmpty();
    }

    [Test]
    public void RoseChart_Tooltips_ShouldNotDisplayDefaultTooltip_When_ShowToolTipsFalse()
    {
        var chartSeries = new List<ChartSeries<double>> { new() { Data = new[] { 10.0, 20.0 } } };
        var chartLabels = new[] { "A", "B" };

        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Rose)
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, new RoseChartOptions { ShowToolTips = false })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var petals = comp.FindAll("path.mud-chart-serie");
        petals.Count.Should().Be(2);
        var firstPetal = petals.First();

        firstPetal.TriggerEvent("onmouseover", new MouseEventArgs());

        var tooltip = comp.FindAll("g.svg-tooltip");
        tooltip.Should().BeEmpty();
    }

    [Test]
    public void RoseChart_Tooltips_ShouldRenderCustomTooltip_When_TooltipTemplateIsProvided()
    {
        var chartSeries = new List<ChartSeries<double>> { new() { Data = new[] { 10.0, 20.0 } } };
        var chartLabels = new[] { "A", "B" };

        RenderFragment<(SvgPath Segment, string Color)> customTooltipTemplate = context => builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "custom-tooltip-content");
            builder.AddContent(2, $"Custom: {context.Segment.LabelYValue} - Value {context.Segment.LabelXValue} - Color {context.Color}");
            builder.CloseElement();
        };

        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Rose)
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, new RoseChartOptions { ShowToolTips = true, ChartPalette = _baseChartPalette })
            .Add(p => p.TooltipTemplate, customTooltipTemplate)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var petals = comp.FindAll("path.mud-chart-serie");
        var firstPetal = petals.First();

        firstPetal.TriggerEvent("onmouseover", new MouseEventArgs());

        var customContent = comp.Find("div.custom-tooltip-content");
        customContent.Should().NotBeNull();
        customContent.TextContent.Should().Be($"Custom: A - Value 10 - Color {_baseChartPalette[0]}");
    }

    [Test]
    public void RoseChart_Tooltips_ShouldPositionTooltipWithCustomLogic_When_TooltipPositionFuncIsProvided()
    {
        var chartSeries = new List<ChartSeries<double>> { new() { Data = new[] { 10.0 } } };
        var chartLabels = new[] { "A" };

        Func<SvgPath, (double X, double Y)> customPositionFunc = _ => (123.0, 456.0);

        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Rose)
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, new RoseChartOptions { ShowToolTips = true })
            .Add(p => p.TooltipPositionFunc, customPositionFunc)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var firstPetal = comp.Find("path.mud-chart-serie");

        firstPetal.TriggerEvent("onmouseover", new MouseEventArgs());

        var tooltipDiv = comp.Find("g.svg-tooltip text");
        tooltipDiv.Should().NotBeNull();

        var expectedLeft = (123.0).ToString(System.Globalization.CultureInfo.InvariantCulture);
        tooltipDiv.GetAttribute("x").Should().Be(expectedLeft);
    }

    [Test]
    public void RoseChart_Option_ShowValues_True_And_ShowAsPercentage_False_ShouldDisplayActualValues()
    {
        var chartSeries = new List<ChartSeries<double>> { new() { Data = new[] { 10.5, 20.77, 30.0 } } };
        var chartLabels = new[] { "A", "B", "C" };

        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, new RoseChartOptions { ShowValues = true, ShowAsPercentage = false })
            .Add(p => p.Width, "400px")
            .Add(p => p.Height, "400px")
        );

        comp.FindAll("path.mud-chart-serie").Count.Should().Be(3, "Should render three petals");

        var chartLabelElements = comp.FindAll(".mud-chart-labels");
        chartLabelElements.Count.Should().Be(3, "Should render three text label groups");

        var expectedValues = new[] { "10.5", "20.77", "30" };

        foreach (var labelElement in chartLabelElements)
        {
            var value = labelElement.QuerySelector(".mud-chart-label-value");

            value.Should().NotBeNull("Value should exist");

            var valueText = value.TextContent;

            expectedValues.Should().Contain(valueText);
        }
    }

    [Test]
    public void RoseChart_Option_ChartPalette_ShouldApplyCustomPaletteColorsToPetals()
    {
        var chartSeries = new List<ChartSeries<double>> { new() { Data = new[] { 10.0, 20.0, 30.0 } } };
        var customPalette = new[] { "rgb(255, 0, 0)", "rgb(0, 255, 0)", "rgb(0, 0, 255)" }; // Red, Green, Blue

        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartOptions, new RoseChartOptions { ChartPalette = customPalette })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var petals = comp.FindAll("path.mud-chart-serie");
        petals.Count.Should().Be(3);

        for (var i = 0; i < petals.Count; i++)
        {
            var markup = petals[i].ToMarkup();
            var expectedColor = customPalette[i];
            markup.Should().Contain($"stroke=\"{expectedColor}\"");
            markup.Should().Contain($"fill=\"{expectedColor}\"");
        }
    }

    [Test]
    public void RoseChart_Option_ChartPalette_ShouldCycleColors_When_DataPointsExceedPaletteSize()
    {
        var chartSeries = new List<ChartSeries<double>> { new() { Data = new[] { 10.0, 20.0, 30.0, 40.0 } } };
        var customPalette = new[] { "rgb(255, 0, 0)", "rgb(0, 255, 0)" };

        var comp = Context.Render<Rose<double>>(parameters => parameters
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartOptions, new RoseChartOptions { ChartPalette = customPalette })
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px")
        );

        var petals = comp.FindAll("path.mud-chart-serie");
        petals.Count.Should().Be(4);

        for (var i = 0; i < petals.Count; i++)
        {
            var markup = petals[i].ToMarkup();
            var expectedColor = customPalette[i % customPalette.Length];
            markup.Should().Contain($"stroke=\"{expectedColor}\"");
            markup.Should().Contain($"fill=\"{expectedColor}\"");
        }
    }
}
