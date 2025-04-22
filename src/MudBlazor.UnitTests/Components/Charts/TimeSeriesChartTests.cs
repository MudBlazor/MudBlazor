// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Diagnostics;
using Bunit;
using FluentAssertions;
using MudBlazor.Charts;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    public class TimeSeriesChartTests : BunitTest
    {
        [SetUp]
        public void Init()
        {

        }

        [Test]
        public void TimeSeriesChartBasicExample()
        {
            var time = new DateTime(2000, 1, 1);

            var comp = Context.RenderComponent<MudTimeSeriesChart>(parameters => parameters
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Index = 0,
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeSeriesChartSeries.TimeValue(time.AddHours(x), 1000)).ToList(),
                        IsVisible = true,
                        LineDisplayType = LineDisplayType.Line
                    }
                ])
                .Add(p => p.TimeLabelSpacing, TimeSpan.FromHours(1)));

            // check the line path
            comp.Markup.Should().ContainEquivalentOf("<path class=\"mud-chart-serie mud-chart-line\" blazor:onclick=\"1\" fill=\"none\" stroke=\"#2979FF\" stroke-opacity=\"1\" stroke-width=\"3\" d=\"M 30 320 L 276.6667 320 L 523.3333 320 L 770 320\"></path>");

            // check the axis
            comp.Markup.Should().ContainEquivalentOf("<g class=\"mud-charts-gridlines-yaxis\"><path stroke=\"#e0e0e0\" stroke-width=\"0.3\" d=\"M 30 320 L 770 320\"></path></g></g>");
            comp.Markup.Should().ContainEquivalentOf("<text x='20' y='325' font-size='12px' text-anchor='end' dominant-baseline='auto'>1000</text></g>");
            comp.Markup.Should().ContainEquivalentOf("<text x='30' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 30 340)'>23:00</text><text x='276.66666666666663' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 276.66666666666663 340)'>00:00</text><text x='523.3333333333333' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 523.3333333333333 340)'>01:00</text><text x='770' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 770 340)'>02:00</text></g>");
        }

        [Test]
        public void TimeSeriesChartMatchBounds()
        {
            var time = new DateTime(2000, 1, 1);

            var comp = Context.RenderComponent<MudTimeSeriesChart>(parameters => parameters
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Index = 0,
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeSeriesChartSeries.TimeValue(time.AddHours(x), 1000)).ToList(),
                        IsVisible = true,
                        LineDisplayType = LineDisplayType.Line,
                    }
                ])
                .Add(p => p.TimeLabelSpacing, TimeSpan.FromHours(1))
                .Add(p => p.Width, "1000px")
                .Add(p => p.Height, "400px")
                .Add(p => p.AxisChartOptions, new AxisChartOptions { MatchBoundsToSize = true }));

            // check the size/bounds
            comp.Markup.Should().ContainEquivalentOf("<svg class=\"mud-chart-line mud-ltr\" width=\"1000px\" height=\"400px\" viewBox=\"0 0 1000 400\"");
        }

        [Test]
        public void TimeSeriesChartTimeLabelSpacingRounding()
        {
            var time = new DateTime(2000, 1, 1);

            var comp = Context.RenderComponent<MudTimeSeriesChart>(parameters => parameters
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Index = 0,
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeSeriesChartSeries.TimeValue(time.AddHours(x).AddMinutes(10), 1000)).ToList(),
                        IsVisible = true,
                        LineDisplayType = LineDisplayType.Line
                    }
                ])
                .Add(p => p.TimeLabelSpacing, TimeSpan.FromHours(1))
                .Add(p => p.TimeLabelSpacingRounding, true));

            // check the axis
            comp.Markup.Should().ContainEquivalentOf("<text x='20' y='325' font-size='12px' text-anchor='end' dominant-baseline='auto'>1000</text>");
            comp.Markup.Should().ContainEquivalentOf("<text x='235.55555555555557' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 235.55555555555557 340)'>00:00</text><text x='482.22222222222223' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 482.22222222222223 340)'>01:00</text><text x='728.8888888888889' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 728.8888888888889 340)'>02:00</text>");
        }

        [Test]
        public void TimeSeriesChartTimeLabelSpacingRoundingPadSeries()
        {
            var time = new DateTime(2000, 1, 1);

            var comp = Context.RenderComponent<MudTimeSeriesChart>(parameters => parameters
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Index = 0,
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeSeriesChartSeries.TimeValue(time.AddHours(x).AddMinutes(10), 1000)).ToList(),
                        IsVisible = true,
                        LineDisplayType = LineDisplayType.Line
                    }
                ])
                .Add(p => p.TimeLabelSpacing, TimeSpan.FromHours(1))
                .Add(p => p.TimeLabelSpacingRounding, true)
                .Add(p => p.TimeLabelSpacingRoundingPadSeries, true));

            // check the axis
            comp.Markup.Should().ContainEquivalentOf("<text x='20' y='325' font-size='12px' text-anchor='end' dominant-baseline='auto'>1000</text>");
            comp.Markup.Should().ContainEquivalentOf("<text x='30' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 30 340)'>23:00</text><text x='215' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 215 340)'>00:00</text><text x='400' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 400 340)'>01:00</text><text x='585' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 585 340)'>02:00</text><text x='770' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 770 340)'>03:00</text>");

            // check the line path
            comp.Markup.Should().ContainEquivalentOf("d=\"M 60.8333 320 L 245.8333 320 L 430.8333 320 L 615.8333 320\"");
        }

        [Test]
        public void TimeSeriesChartEmptyData()
        {
            var comp = Context.RenderComponent<TimeSeries>();
            comp.Markup.Should().Contain("mud-chart-line");
        }

        [Test]
        public void TimeSeriesChartLabelFormats()
        {
            var time = new DateTime(2000, 1, 1);
            var format = "dd/MM HH:mm";

            var comp = Context.RenderComponent<MudTimeSeriesChart>(parameters => parameters
                .Add(p => p.ChartSeries, new List<TimeSeriesChartSeries>() {
                    new TimeSeriesChartSeries()
                    {
                        Index = 0,
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeSeriesChartSeries.TimeValue(time.AddDays(x), 1000)).ToList(),
                        IsVisible = true,
                        LineDisplayType = LineDisplayType.Line
                    }
                })
                .Add(p => p.TimeLabelSpacing, TimeSpan.FromDays(1))
                .Add(p => p.TimeLabelFormat, format));

            for (var i = -1; i < 2; i++)
            {
                var expectedTimeString = time.AddDays(i).ToString(format);
                comp.Markup.Should().Contain(expectedTimeString);
            }
        }
    }
}
