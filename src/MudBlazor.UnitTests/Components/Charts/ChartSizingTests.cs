// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts;
using MudBlazor.Interop;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts;

public class ChartSizingTests : BunitTest
{
    /// <summary>
    /// Pending and legacy overlays retain their local title margin until a host supplies a margin for issue #13606.
    /// </summary>
    [Test]
    public async Task LineOverlay_PendingAndLegacyGridPreserveLocalMargin()
    {
        var host = Context.Render<AxisChartProbe>();
        var context = Context.Render<CascadingValue<AxisGridData<double>?>>(parameters => parameters
            .AddCascadingValue<IMudChart<double>>(host.Instance)
            .Add(p => p.Value, null)
            .AddChildContent<AxisChartProbe>(overlay => overlay
                .Add(p => p.ChartSeries, new List<ChartSeries<double>>
                {
                    new() { Name = "Revenue", Data = new double[] { 40, 80 } }
                })
                .Add(p => p.ChartOptions, new LineChartOptions { YAxisTitle = "Revenue", ShowDataMarkers = true })));
        var overlay = context.FindComponent<AxisChartProbe>();

        overlay.Instance.IsOverlayChart.Should().BeTrue();
        overlay.Instance.LocalStartSpace.Should().Be(50, "a pending overlay still reserves its minimum margin and title space");
        host.Instance.SharedData.Should().BeNull();

        await host.InvokeAsync(host.Instance.RenderPendingOverlay);

        overlay.Instance.SharedData.Should().BeNull("an empty host must not fabricate shared grid data");
        host.FindAll(".mud-charts-line-series path").Should().BeEmpty("the overlay waits for grid data before plotting");

        await context.SetParametersAndRenderAsync(parameters => parameters
            .Add(p => p.Value, new AxisGridData<double>(0, 5, 20, 700, 350)));

        var content = Context.Render(host.Instance.OverlayContent!);
        var markers = content.FindAll("circle.mud-chart-point:not([opacity])");
        markers.Should().HaveCount(2);
        markers[0].GetAttribute("cx").Should().Be("50", "legacy grid data preserves the overlay's own title margin");
        markers[1].GetAttribute("cx").Should().Be("670", "the last point respects the shared width and right margin");
        markers[0].GetAttribute("cy").Should().Be("172.5");
        markers[1].GetAttribute("cy").Should().Be("25");
    }

    /// <summary>
    /// Removing options from an empty standalone chart restores the minimum untitled margin for issue #13606.
    /// </summary>
    [Test]
    public async Task LineChart_RemovingOptionsRestoresMinimumMargin()
    {
        var comp = Context.Render<Line<double>>(parameters => parameters
            .Add(p => p.ChartOptions, new LineChartOptions { YAxisTitle = "Revenue" }));
        comp.Find("g.mud-charts-yaxis text").GetAttribute("x").Should().Be("40");

        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.ChartOptions, null));

        comp.Find("g.mud-charts-yaxis text").GetAttribute("x").Should().Be("20");
        comp.FindAll("text.mud-charts-yaxis").Should().BeEmpty();
        comp.FindAll("path.mud-chart-line").Should().BeEmpty();
    }

    // Bar and Line rebuilds return before reading the margin when shared data is missing.
    // Host rebuilds create grid data before calling RenderOverlay, even with no series.
    private sealed class AxisChartProbe : Line<double>
    {
        public double LocalStartSpace => HorizontalStartSpace;

        public void RenderPendingOverlay() => RenderOverlay();
    }

    /// <summary>
    /// Identical host and overlay bars share measured margins after label growth and resizing for issue #13606.
    /// </summary>
    [TestCase(10)]
    [TestCase(80)]
    public async Task BarOverlay_MatchesHostGeometryAfterLabelMeasurementAndResize(double labelWidth)
    {
        var series = new List<ChartSeries<double>>
        {
            new() { Name = "Revenue", Data = new double[] { 40, 60, 80 } }
        };
        var options = new BarChartOptions();
        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Bar)
            .Add(p => p.MatchBoundsToSize, true)
            .Add(p => p.Width, "800px")
            .Add(p => p.Height, "400px")
            .Add(p => p.ChartSeries, series)
            .Add(p => p.ChartOptions, options)
            .AddChildContent<Bar<double>>(overlay => overlay
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, options)));

        var host = comp.FindComponents<Bar<double>>().Single(chart => !chart.Instance.IsOverlayChart);
        host.Instance.OverlayChart.Should().NotBeNull();
        var axis = comp.FindComponent<BaseAxisChart<double, BarChartOptions>>();

        await comp.InvokeAsync(async () =>
        {
            await axis.Instance.YAxisLabelSizeChanged.InvokeAsync(new ElementSize { Width = labelWidth, Height = 20 });
            host.Instance.OnElementSizeChanged(new ElementSize { Width = 800, Height = 400, Timestamp = 1 });
            host.Instance.RebuildChart();
        });

        comp.Find("svg").GetAttribute("viewBox").Should().Be("0 0 800 400");
        var grid = comp.Find(".mud-charts-gridlines-yaxis path").GetAttribute("d")!;
        if (labelWidth == 10)
        {
            grid.Should().StartWith("M 30 ", "narrow labels retain the minimum margin");
        }
        else
        {
            grid.Should().NotStartWith("M 30 ", "wide labels expand the host margin");
        }

        AssertMatchingBarGeometry();

        await comp.InvokeAsync(async () =>
        {
            await axis.Instance.YAxisLabelSizeChanged.InvokeAsync(new ElementSize { Width = 120, Height = 20 });
            host.Instance.OnElementSizeChanged(new ElementSize { Width = 960, Height = 400, Timestamp = 2 });
            host.Instance.RebuildChart();
        });

        comp.Find("svg").GetAttribute("viewBox").Should().Be("0 0 960 400");
        comp.Find(".mud-charts-gridlines-yaxis path").GetAttribute("d").Should().NotBe(grid);
        AssertMatchingBarGeometry();

        void AssertMatchingBarGeometry()
        {
            var groups = comp.FindAll("svg > g.mud-charts-bar-series");
            groups.Count.Should().Be(2);
            var hostBars = groups[0].QuerySelectorAll("path.mud-chart-bar");
            var overlayBars = groups[1].QuerySelectorAll("path.mud-chart-bar");
            hostBars.Should().HaveCount(3);
            overlayBars.Should().HaveCount(3);
            for (var i = 0; i < hostBars.Length; i++)
            {
                overlayBars[i].GetAttribute("d").Should().Be(hostBars[i].GetAttribute("d"),
                    "identical overlay bars must use the host's measured plot area");
                overlayBars[i].GetAttribute("stroke-width").Should().Be(hostBars[i].GetAttribute("stroke-width"));
            }
        }
    }

    /// <summary>
    /// A line overlay spans the host's measured horizontal grid bounds for issue #13606.
    /// </summary>
    [TestCase(10)]
    [TestCase(80)]
    public async Task LineOverlay_MatchesHostHorizontalBoundsAfterLabelMeasurement(double labelWidth)
    {
        var series = new List<ChartSeries<double>>
        {
            new() { Name = "Revenue", Data = new double[] { 40, 60, 80 } }
        };
        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Bar)
            .Add(p => p.MatchBoundsToSize, true)
            .Add(p => p.Width, "800px")
            .Add(p => p.Height, "400px")
            .Add(p => p.ChartSeries, series)
            .AddChildContent<Line<double>>(overlay => overlay
                .Add(p => p.ChartSeries, series)
                .Add(p => p.ChartOptions, new LineChartOptions { ShowDataMarkers = true })));
        var host = comp.FindComponent<Bar<double>>();
        var axis = comp.FindComponent<BaseAxisChart<double, BarChartOptions>>();

        await comp.InvokeAsync(async () =>
        {
            await axis.Instance.YAxisLabelSizeChanged.InvokeAsync(new ElementSize { Width = labelWidth, Height = 20 });
            host.Instance.OnElementSizeChanged(new ElementSize { Width = 800, Height = 400, Timestamp = 1 });
            host.Instance.RebuildChart();
        });

        var grid = comp.Find(".mud-charts-gridlines-yaxis path").GetAttribute("d")!.Split(' ');
        var markers = comp.FindAll("circle.mud-chart-point:not([opacity])");
        markers.Count.Should().Be(3);
        markers[0].GetAttribute("cx").Should().Be(grid[1]);
        markers[^1].GetAttribute("cx").Should().Be(grid[4]);
    }

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

    [Test]
    public async Task MatchBoundsToSize_WithPercentageHeight_ShouldNotLoop()
    {
        var series = new List<ChartSeries<double>>
            {
                new() { Data = new double[] { 12.2, 14.3, 11.5 } }
            };
        var labels = new[] { "1/1/26", "2/1/26", "3/1/26" };

        var initialSize = new ElementSize { Width = 700, Height = 350, Timestamp = 1 };

        Context.JSInterop.Setup<ElementSize>("mudObserveElementSize", _ => true)
            .SetResult(initialSize);

        Context.JSInterop.Setup<ElementSize>("mudGetSvgBBox", _ => true)
            .SetResult(new ElementSize { Width = 50, Height = 20 });

        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Line)
            .Add(p => p.ChartSeries, series)
            .Add(p => p.ChartLabels, labels)
            .Add(p => p.MatchBoundsToSize, true)
            .Add(p => p.Width, "100%")
            .Add(p => p.Height, "80%")
        );

        var chartBase = comp.FindComponent<Line<double>>().Instance;

        var svg = comp.Find("svg");
        svg.GetAttribute("viewBox").Should().Be("0 0 700 350");

        var largerSize = new ElementSize { Width = 700, Height = 400, Timestamp = 2 };

        await comp.InvokeAsync(() => chartBase.OnElementSizeChanged(largerSize));

        await comp.WaitForAssertionAsync(() =>
        {
            var svg = comp.Find("svg");
            svg.GetAttribute("viewBox").Should().Be("0 0 700 400");
        });
    }

    [Test]
    public async Task MatchBoundsToSize_WithPixelHeight_ShouldUpdate()
    {
        var series = new List<ChartSeries<double>>
            {
                new() { Data = new double[] { 12.2, 14.3, 11.5 } }
            };
        var labels = new[] { "1/1/26", "2/1/26", "3/1/26" };

        var initialSize = new ElementSize { Width = 700, Height = 350, Timestamp = 1 };

        Context.JSInterop.Setup<ElementSize>("mudObserveElementSize", _ => true)
            .SetResult(initialSize);

        Context.JSInterop.Setup<ElementSize>("mudGetSvgBBox", _ => true)
            .SetResult(new ElementSize { Width = 50, Height = 20 });

        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Line)
            .Add(p => p.ChartSeries, series)
            .Add(p => p.ChartLabels, labels)
            .Add(p => p.MatchBoundsToSize, true)
            .Add(p => p.Width, "700px")
            .Add(p => p.Height, "350px")
        );

        var chartBase = comp.FindComponent<Line<double>>().Instance;

        var svg = comp.Find("svg");
        svg.GetAttribute("viewBox").Should().Be("0 0 700 350");

        var largerSize = new ElementSize { Width = 800, Height = 400, Timestamp = 2 };

        await comp.InvokeAsync(() => chartBase.OnElementSizeChanged(largerSize));

        await comp.WaitForAssertionAsync(() =>
        {
            var svg = comp.Find("svg");
            svg.GetAttribute("viewBox").Should().Be("0 0 800 400");
        });
    }

    [Test]
    public async Task MudChart_MatchBoundsToSize_NoFixedParent_ShouldUseFallbackHeight()
    {
        var series = new List<ChartSeries<double>> { new() { Data = new double[] { 10, 20 } } };

        var jsInterop = Context.JSInterop.Setup<bool>("hasDefinedParentHeight", _ => true);
        jsInterop.SetResult(false);

        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Line)
            .Add(p => p.MatchBoundsToSize, true)
            .Add(p => p.Height, "100%")
            .Add(p => p.ChartSeries, series));

        await comp.WaitForAssertionAsync(() =>
        {
            var fallbackDiv = comp.Find("div[style*='height:350px']");
            fallbackDiv.Should().NotBeNull();
            fallbackDiv.GetAttribute("style").Should().Contain("height:350px");
        });
    }

    [Test]
    public async Task MudChart_MatchBoundsToSize_WithFixedParent_ShouldNotUseFallbackHeight()
    {
        var series = new List<ChartSeries<double>> { new() { Data = new double[] { 10, 20 } } };

        var jsInterop = Context.JSInterop.Setup<bool>("hasDefinedParentHeight", _ => true);
        jsInterop.SetResult(true);

        var comp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Line)
            .Add(p => p.MatchBoundsToSize, true)
            .Add(p => p.Height, "100%")
            .Add(p => p.ChartSeries, series));

        await comp.WaitForAssertionAsync(() => Context.JSInterop.Invocations["hasDefinedParentHeight"].Should().HaveCount(1));

        comp.FindAll("div[style*='height:400px']").Should().BeEmpty();
    }

    [Test]
    public async Task MudRadialChartBase_OnElementSizeChanged_RespectsMatchBoundsAndTimestamp()
    {
        var initialSize = new ElementSize { Width = 300, Height = 300, Timestamp = 10 };
        Context.JSInterop.Setup<ElementSize>("mudObserveElementSize", _ => true).SetResult(initialSize);

        var comp = Context.Render<Pie<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.MatchBoundsToSize, true)
            .Add(p => p.Width, "100%")
            .Add(p => p.Height, "100%"));

        var radialChartBase = comp.Instance;

        // A newer, larger size triggers the MatchBoundsToSize branch (min dimension wins):
        // min(240, 180) = 180 -> viewBox 0 0 180 180.
        await comp.InvokeAsync(() => radialChartBase.OnElementSizeChanged(new ElementSize { Width = 240, Height = 180, Timestamp = 20 }));
        await comp.WaitForAssertionAsync(() =>
        {
            comp.Instance._paths.Should().NotBeEmpty();
            comp.Find("svg").GetAttribute("viewBox").Should().Be("0 0 180 180");
        });

        // A stale (older) timestamp is ignored: the early return keeps the existing viewBox.
        await comp.InvokeAsync(() => radialChartBase.OnElementSizeChanged(new ElementSize { Width = 50, Height = 50, Timestamp = 1 }));
        comp.Find("svg").GetAttribute("viewBox").Should().Be("0 0 180 180");
    }

    [Test]
    public async Task MudRadialChartBase_OnElementSizeChanged_NotMatchBounds_DoesNotChangeBounds()
    {
        var comp = Context.Render<Pie<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.MatchBoundsToSize, false)
            .Add(p => p.Width, "300px")
            .Add(p => p.Height, "300px"));

        var viewBoxBefore = comp.Find("svg").GetAttribute("viewBox");

        // MatchBoundsToSize is false -> early return after recording the size; bounds unchanged.
        await comp.InvokeAsync(() => comp.Instance.OnElementSizeChanged(new ElementSize { Width = 123, Height = 456, Timestamp = 99 }));

        comp.Find("svg").GetAttribute("viewBox").Should().Be(viewBoxBefore);
    }

    [TestCase("200px", "160px", 80)]
    [TestCase("100%", "100%", 140)]
    public async Task MudRadialChartBase_SetBounds_ParsesPixelsAndFallsBackToDefault(string width, string height, int expectedRadius)
    {
        // When _elementSize is null (no JS interop), RebuildChart -> SetBounds parses px Width/Height.
        // Non-px values cannot be parsed, so the 280x280 defaults remain (Radius 140).
        var comp = Context.Render<Pie<double>>(parameters => parameters
            .Add(p => p.ChartSeries, new List<ChartSeries<double>> { new() { Data = new double[] { 10, 20, 30 } } })
            .Add(p => p.MatchBoundsToSize, true)
            .Add(p => p.Width, width)
            .Add(p => p.Height, height));

        List<SvgPath> paths = null!;
        await comp.InvokeAsync(() =>
        {
            comp.Instance.RebuildChart();
            paths = new List<SvgPath>(comp.Instance._paths);
        });

        paths.Should().HaveCount(3);
        paths.Should().OnlyContain(p => p.Data.Contains($"A {expectedRadius} {expectedRadius}"));
    }
}
