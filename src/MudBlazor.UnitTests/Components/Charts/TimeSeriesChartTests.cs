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
            comp.Markup.Should().ContainEquivalentOf("<path class=\"mud-chart-serie mud-chart-line\" blazor:onclick=\"1\" fill=\"none\" stroke=\"#2979FF\" stroke-opacity=\"1\" stroke-width=\"3\" d=\"M 80 325 L 293.3333 325 L 506.6667 325 L 720 325\"></path>");

            // check the axis
            comp.Markup.Should().ContainEquivalentOf("<g class=\"mud-charts-gridlines-yaxis\"><path stroke=\"#e0e0e0\" stroke-width=\"0.3\" d=\"M 80 325 L 720 325\"></path></g></g>\n    <g class=\"mud-charts-yaxis\"><text x='70' y='330' font-size='12px' text-anchor='end' dominant-baseline='auto'>1000</text></g>\n    <g class=\"mud-charts-xaxis\"><text x='80' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 80 340)'>23:00</text><text x='400' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 400 340)'>00:00</text><text x='720' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 720 340)'>01:00</text></g>");
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
            comp.Markup.Should().ContainEquivalentOf("<g class=\"mud-charts-gridlines-yaxis\"><path stroke=\"#e0e0e0\" stroke-width=\"0.3\" d=\"M 80 325 L 720 325\"></path></g></g>\n    <g class=\"mud-charts-yaxis\"><text x='70' y='330' font-size='12px' text-anchor='end' dominant-baseline='auto'>1000</text></g>\n    <g class=\"mud-charts-xaxis\"><text x='257.77777777777777' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 257.77777777777777 340)'>00:00</text><text x='577.7777777777778' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 577.7777777777778 340)'>01:00</text></g>");
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
            comp.Markup.Should().ContainEquivalentOf("<g class=\"mud-charts-grid\"><g class=\"mud-charts-gridlines-yaxis\"><path stroke=\"#e0e0e0\" stroke-width=\"0.3\" d=\"M 80 325 L 720 325\"></path></g></g>\n    <g class=\"mud-charts-yaxis\"><text x='70' y='330' font-size='12px' text-anchor='end' dominant-baseline='auto'>1000</text></g>\n    <g class=\"mud-charts-xaxis\"><text x='80' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 80 340)'>23:00</text><text x='293.33333333333337' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 293.33333333333337 340)'>00:00</text><text x='506.6666666666667' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 506.6666666666667 340)'>01:00</text><text x='720' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 720 340)'>02:00</text></g>");

            // check the line path
            comp.Markup.Should().ContainEquivalentOf("<path class=\"mud-chart-serie mud-chart-line\" blazor:onclick=\"1\" fill=\"none\" stroke=\"#2979FF\" stroke-opacity=\"1\" stroke-width=\"3\" d=\"M 106.6667 325 L 266.6667 325 L 426.6667 325 L 586.6667 325\"></path>");
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
