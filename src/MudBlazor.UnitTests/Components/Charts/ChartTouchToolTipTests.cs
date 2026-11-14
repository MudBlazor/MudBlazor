// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Charts;
using MudBlazor.Interop;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    public class ChartTouchToolTipTests : BunitTest
    {
        private static List<ChartSeries<double>> LineSeries() =>
        [
            new() { Name = "Series 1", Data = new double[] { 10, 20, 30, 40 } }
        ];

        private IRenderedComponent<MudChart<double>> RenderLine(LineChartOptions options) =>
            Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.ChartSeries, LineSeries())
                .Add(p => p.ChartOptions, options));

        private IReadOnlyList<JSRuntimeInvocation> Invocations(string identifier) => Context.JSInterop.Invocations[identifier];

        /// <summary>
        /// ShowToolTipsOnTouch hands the chart's svg to the touch script once, to pick the nearest point along x, and marks every hover target for it.
        /// </summary>
        [Test]
        public async Task LineChartShowToolTipsOnTouchAttachesTouchOnce()
        {
            var comp = RenderLine(new LineChartOptions { ShowToolTipsOnTouch = true, ShowDataMarkers = true });

            // A hover re-renders the chart, which must not attach the script a second time.
            await comp.FindAll("circle.mud-chart-hover-point")[1].MouseOverAsync(new());

            var attach = Invocations("mudChartTouch.attach").Should().ContainSingle().Subject;
            attach.Arguments[0].Should().BeOfType<ElementReference>();
            attach.Arguments[1].Should().Be(true);
            // The script points at hover targets, never at the data markers drawn on the same points.
            comp.FindAll("circle.mud-chart-hover-point").Should().HaveCount(4).And.OnlyContain(x => x.HasAttribute("opacity"));
        }

        /// <summary>
        /// Touch stays the browser's own unless a chart asks for it, so a line chart attaches nothing by default.
        /// </summary>
        [Test]
        public void LineChartLeavesTouchAloneByDefault()
        {
            RenderLine(new LineChartOptions());

            Invocations("mudChartTouch.attach").Should().BeEmpty();
        }

        /// <summary>
        /// Without tooltips there's nothing for a finger to open, so ShowToolTipsOnTouch attaches nothing.
        /// </summary>
        [Test]
        public void LineChartWithoutToolTipsLeavesTouchAlone()
        {
            var comp = RenderLine(new LineChartOptions { ShowToolTipsOnTouch = true, ShowToolTips = false });

            Invocations("mudChartTouch.attach").Should().BeEmpty();
            comp.FindAll("circle.mud-chart-hover-point").Should().BeEmpty();
        }

        /// <summary>
        /// Turning ShowToolTipsOnTouch off detaches the touch script from the same svg, and turning it back on attaches it again.
        /// </summary>
        [Test]
        public async Task LineChartFollowsShowToolTipsOnTouchChanges()
        {
            var comp = RenderLine(new LineChartOptions { ShowToolTipsOnTouch = true });

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.ChartOptions, new LineChartOptions()));

            var detach = Invocations("mudChartTouch.detach").Should().ContainSingle().Subject;
            detach.Arguments[0].Should().Be(Invocations("mudChartTouch.attach")[0].Arguments[0]);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.ChartOptions, new LineChartOptions { ShowToolTipsOnTouch = true }));

            Invocations("mudChartTouch.attach").Should().HaveCount(2);
            Invocations("mudChartTouch.detach").Should().ContainSingle();
        }

        /// <summary>
        /// A lost circuit while attaching or detaching the touch script leaves the chart rendering.
        /// </summary>
        [Test]
        public async Task TouchScriptDisconnectedIsSwallowed()
        {
            Context.JSInterop.SetupVoid("mudChartTouch.attach", _ => true).SetException(new JSDisconnectedException("disconnected"));
            Context.JSInterop.SetupVoid("mudChartTouch.detach", _ => true).SetException(new JSDisconnectedException("disconnected"));

            var comp = RenderLine(new LineChartOptions { ShowToolTipsOnTouch = true });
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.ChartOptions, new LineChartOptions()));

            Invocations("mudChartTouch.attach").Should().ContainSingle();
            Invocations("mudChartTouch.detach").Should().ContainSingle();
            comp.FindAll("circle.mud-chart-hover-point").Should().HaveCount(4);
        }

        /// <summary>
        /// A time series chart reads left to right like a line chart, so touch picks its nearest point along x.
        /// </summary>
        [Test]
        public void TimeSeriesChartShowToolTipsOnTouchFollowsX()
        {
            // A time series lays its points out from the measured axis labels.
            Context.JSInterop.Setup<ElementSize>("mudGetSvgBBox", _ => true).SetResult(new ElementSize { Width = 30, Height = 15 });
            var time = new DateTime(2000, 1, 1);

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.ChartSeries, [
                    new()
                    {
                        Name = "Series 1",
                        Data = new[] { 0, 1, 2 }.Select(x => new TimeValue<double>(time.AddHours(x), x * 10)).ToList(),
                    }
                ])
                .Add(p => p.ChartOptions, new TimeSeriesChartOptions { ShowToolTipsOnTouch = true }));

            Invocations("mudChartTouch.attach").Should().ContainSingle().Which.Arguments[1].Should().Be(true);
            comp.FindAll("circle.mud-chart-hover-point").Should().HaveCount(3);
        }

        /// <summary>
        /// Scatter points have no left-to-right order, so touch picks the point nearest the finger in both directions.
        /// </summary>
        [Test]
        public void ScatterPlotShowToolTipsOnTouchPicksTheNearestPoint()
        {
            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.ScatterPlot)
                .Add(p => p.ChartSeries, [
                    new() { Name = "Series 1", Data = new ChartData<double>([(10, 20), (30, 40), (50, 10)]) }
                ])
                .Add(p => p.ChartOptions, new ScatterPlotChartOptions { ShowToolTipsOnTouch = true }));

            Invocations("mudChartTouch.attach").Should().ContainSingle().Which.Arguments[1].Should().Be(false);
            comp.FindAll("circle.mud-chart-hover-point").Should().HaveCount(3);
        }
    }
}
