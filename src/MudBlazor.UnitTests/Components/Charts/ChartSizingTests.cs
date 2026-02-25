// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using MudBlazor.Charts;
using MudBlazor.Interop;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts;

public class ChartSizingTests : BunitTest
{
    [Test]
    public async Task MudAxisChartBase_MatchBoundsToSize_ShouldMatchMeasuredSizeExactly()
    {
        var chartSeries = new List<ChartSeries<double>> { new() { Name = "Series 1", Data = new double[] { 10, 20 } }, };
        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Bar)
            .Add(p => p.MatchBoundsToSize, true)
            .Add(p => p.ChartSeries, chartSeries));

        // Find the internal Bar component
        var barComponent = comp.FindComponent<Bar<double>>();
        MudAxisChartBase<double, BarChartOptions> axisChartBase = barComponent.Instance;

        // Manually invoke OnElementSizeChanged with a specific width
        const double MeasuredWidth = 800.0;
        const double MeasuredHeight = 400.0;

        await comp.InvokeAsync(() => axisChartBase.OnElementSizeChanged(new ElementSize { Width = MeasuredWidth, Height = MeasuredHeight, Timestamp = DateTime.Now.Ticks }));

        // The viewBox should match the measured size exactly
        await comp.WaitForAssertionAsync(() =>
        {
            var svg = comp.Find("svg.mud-chart-bar");
            svg.GetAttribute("viewBox").Should().Be($"0 0 {MeasuredWidth} {MeasuredHeight}");
        });
    }

    [Test]
    public async Task MudAxisChartBase_LineChart_MatchBoundsToSize_ShouldMatchMeasuredSizeExactly()
    {
        var chartSeries = new List<ChartSeries<double>> { new() { Name = "Series 1", Data = new double[] { 10, 20 } }, };
        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Line)
            .Add(p => p.MatchBoundsToSize, true)
            .Add(p => p.ChartSeries, chartSeries));

        // Find the internal Line component
        var lineComponent = comp.FindComponent<Line<double>>();
        MudAxisChartBase<double, LineChartOptions> axisChartBase = lineComponent.Instance;

        // Manually invoke OnElementSizeChanged with a specific width
        const double MeasuredWidth = 900.0;
        const double MeasuredHeight = 500.0;

        await comp.InvokeAsync(() => axisChartBase.OnElementSizeChanged(new ElementSize { Width = MeasuredWidth, Height = MeasuredHeight, Timestamp = DateTime.Now.Ticks }));

        // The viewBox should match the measured size exactly
        await comp.WaitForAssertionAsync(() =>
        {
            var svg = comp.Find("svg.mud-chart-line");
            svg.GetAttribute("viewBox").Should().Be($"0 0 {MeasuredWidth} {MeasuredHeight}");
        });
    }

    [Test]
    public void MudAxisChartBase_YAxisTitle_ShouldAllocateSpaceAndPositionCorrectly()
    {
        var chartSeries = new List<ChartSeries<double>> { new() { Name = "Series 1", Data = new double[] { 10, 20 } } };

        // Render without title
        var compWithoutTitle = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Bar)
            .Add(p => p.ChartSeries, chartSeries));

        var gridWithoutTitle = compWithoutTitle.Find("g.mud-charts-gridlines-yaxis path");
        var dWithoutTitle = gridWithoutTitle.GetAttribute("d")!;
        // Extract the first X coordinate from "M X Y ..."
        var xWithoutTitle = double.Parse(dWithoutTitle.Split(' ')[1]);

        // Render with title
        var compWithTitle = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Bar)
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartOptions, new BarChartOptions { YAxisTitle = "Title" }));

        var gridWithTitle = compWithTitle.Find("g.mud-charts-gridlines-yaxis path");
        var dWithTitle = gridWithTitle.GetAttribute("d")!;
        var xWithTitle = double.Parse(dWithTitle.Split(' ')[1]);

        // xWithTitle should be larger than xWithoutTitle if space was allocated
        // Note: Since labels might be small, they both might fall into the 30px minimum.
        // But we added 20px, so it should definitely exceed 30px if the original was 30px.
        xWithTitle.Should().BeGreaterThan(xWithoutTitle);

        // Also check that the title is at X=0
        var titleGroup = compWithTitle.Find("g[transform^='translate(0,']");
        titleGroup.Should().NotBeNull();
        titleGroup.InnerHtml.Should().Contain("Title");
    }
}
