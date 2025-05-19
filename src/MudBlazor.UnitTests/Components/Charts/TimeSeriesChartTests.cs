// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using Bunit;
using FluentAssertions;
using MudBlazor.Charts;
using MudBlazor.Interop;
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
            var mockYAxisLabelSize = new ElementSize
            {
                Width = 27.5,
                Height = 14.8,
            };
            var mockXAxisLabelSize = new ElementSize
            {
                Width = 670.5,
                Height = 14.8,
            };

            var counter = 1;

            Context.JSInterop.Setup<ElementSize>("mudGetSvgBBox", args =>
            {
                if (counter % 2 == 0)
                {
                    counter++;
                    return true;
                }

                return false;
            }).SetResult(mockXAxisLabelSize);

            Context.JSInterop.Setup<ElementSize>("mudGetSvgBBox", args =>
            {
                if (counter % 2 == 1)
                {
                    counter++;
                    return true;
                }

                return false;
            }).SetResult(mockYAxisLabelSize);

            var time = new DateTime(2000, 1, 1);

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeSeries.DataPoint(time.AddHours(x), 1000)).ToList(),
                        Visible = true,
                    }
                ])
                .Add(p => p.ChartOptions, new TimeSeriesChartOptions() { TimeLabelSpacing = TimeSpan.FromHours(1) }));

            // check the line path
            comp.Markup.Should().ContainEquivalentOf("<path class=\"mud-chart-serie mud-chart-line\" blazor:onclick=\"15\" fill=\"none\" stroke=\"#2979FF\" stroke-opacity=\"1\" stroke-width=\"3\" d=\"M 38 320 L 248.6667 320 L 459.3333 320 L 670 320\"></path>");

            // check the axis
            comp.Markup.Should().ContainEquivalentOf("<g class=\"mud-charts-gridlines-yaxis\"><path stroke=\"#e0e0e0\" stroke-width=\"0.3\" d=\"M 38 320 L 670 320\"></path></g></g>");
            comp.Markup.Should().ContainEquivalentOf("<text x='28' y='325' font-size='12px' text-anchor='end' dominant-baseline='auto'>1000</text></g>");
            comp.Markup.Should().ContainEquivalentOf("<text x='38' y='342.5' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 38 342.5)'>23:00</text><text x='248.6667' y='342.5' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 248.6667 342.5)'>00:00</text><text x='459.3333' y='342.5' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 459.3333 342.5)'>01:00</text><text x='670' y='342.5' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 670 342.5)'>02:00</text></g>");
        }

        [Test]
        public void TimeSeriesChartMatchBounds()
        {
            var time = new DateTime(2000, 1, 1);

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeSeries.DataPoint(time.AddHours(x), 1000)).ToList(),
                        Visible = true,
                    }
                ])
                .Add(p => p.ChartOptions, new TimeSeriesChartOptions() { TimeLabelSpacing = TimeSpan.FromHours(1), LineDisplayType = LineDisplayType.Line })
                .Add(p => p.Width, "1000px")
                .Add(p => p.Height, "400px")
                .Add(p => p.MatchBoundsToSize, true));

            // check the size/bounds
            comp.Markup.Should().ContainEquivalentOf("<svg class=\"mud-chart-line mud-ltr\" width=\"1000px\" height=\"400px\" viewBox=\"0 0 1000 400\"");
        }

        [Test]
        public void TimeSeriesChartTimeLabelSpacingRounding()
        {
            var time = new DateTime(2000, 1, 1);

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeSeries.DataPoint(time.AddHours(x).AddMinutes(10), 1000)).ToList(),
                        Visible = true,
                    }
                ])
                .Add(p => p.ChartOptions, new TimeSeriesChartOptions()
                {
                    TimeLabelSpacing = TimeSpan.FromHours(1),
                    LineDisplayType = LineDisplayType.Line,
                    TimeLabelSpacingRounding = true
                }));

            // check the axis
            comp.Markup.Should().ContainEquivalentOf("<text x='20' y='325' font-size='12px' text-anchor='end' dominant-baseline='auto'>1000</text>");
            comp.Markup.Should().ContainEquivalentOf("<text x='207.7778' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 207.7778 340)'>00:00</text><text x='421.1111' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 421.1111 340)'>01:00</text><text x='634.4444' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 634.4444 340)'>02:00</text>");
        }

        [Test]
        public void TimeSeriesChartTimeLabelSpacingRoundingPadSeries()
        {
            var time = new DateTime(2000, 1, 1);

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeSeries.DataPoint(time.AddHours(x).AddMinutes(10), 1000)).ToList(),
                        Visible = true,
                    }
                ])
                .Add(p => p.ChartOptions, new TimeSeriesChartOptions()
                {
                    TimeLabelSpacingRoundingPadSeries = true,
                    TimeLabelSpacing = TimeSpan.FromHours(1),
                    LineDisplayType = LineDisplayType.Line,
                    TimeLabelSpacingRounding = true
                }));

            // check the axis
            comp.Markup.Should().ContainEquivalentOf("<text x='20' y='325' font-size='12px' text-anchor='end' dominant-baseline='auto'>1000</text>");
            comp.Markup.Should().ContainEquivalentOf("<text x='30' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 30 340)'>23:00</text><text x='190' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 190 340)'>00:00</text><text x='350' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 350 340)'>01:00</text><text x='510' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 510 340)'>02:00</text><text x='670' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 670 340)'>03:00</text>");

            // check the line path
            comp.Markup.Should().ContainEquivalentOf("d=\"M 30 320 L 670 320\"");
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

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.ChartSeries, new() {
                    new ChartSeries()
                    {
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeSeries.DataPoint(time.AddDays(x), 1000)).ToList(),
                        Visible = true,
                    }
                })
                .Add(p => p.ChartOptions, new TimeSeriesChartOptions()
                {
                    TimeLabelSpacing = TimeSpan.FromDays(1),
                    LineDisplayType = LineDisplayType.Line,
                    TimeLabelFormat = format
                }));

            for (var i = -1; i < 2; i++)
            {
                var expectedTimeString = time.AddDays(i).ToString(format);
                comp.Markup.Should().Contain(expectedTimeString);
            }
        }
    }
}
