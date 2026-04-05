// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
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

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeValue<double>(time.AddHours(x), 1000)).ToList(),
                        Visible = true,
                    }
                ])
                .Add(p => p.ChartOptions, new TimeSeriesChartOptions() { TimeLabelSpacing = TimeSpan.FromHours(1) }));

            // check the line path
            comp.Markup.Should().ContainEquivalentOf("<path class=\"mud-chart-serie mud-chart-line\" blazor:onclick=\"15\" fill=\"none\" stroke=\"#2979FF\" stroke-opacity=\"1\" stroke-width=\"3\" d=\"M 30 320 L 243.3333 320 L 456.6667 320 L 670 320\"></path>");

            // check the axis
            comp.Markup.Should().ContainEquivalentOf("<g class=\"mud-charts-gridlines-yaxis\"><path stroke=\"#e0e0e0\" stroke-width=\"0.3\" d=\"M 30 320 L 670 320\"></path></g></g>");
            comp.Markup.Should().ContainEquivalentOf("<text x='20' y='325' font-size='12px' text-anchor='end' dominant-baseline='auto'>1000</text></g>");
            comp.Markup.Should().ContainEquivalentOf("<text x='30' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 30 340)'>23:00</text><text x='243.3333' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 243.3333 340)'>00:00</text><text x='456.6667' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 456.6667 340)'>01:00</text><text x='670' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 670 340)'>02:00</text></g>");
        }

        [Test]
        public void TimeSeriesChartRendersWithIntData()
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

            var comp = Context.Render<MudChart<int>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeValue<int>(time.AddHours(x), 1000)).ToList(),
                        Visible = true,
                    }
                ])
                .Add(p => p.ChartOptions, new TimeSeriesChartOptions() { TimeLabelSpacing = TimeSpan.FromHours(1) }));

            // check the line path
            comp.Markup.Should().ContainEquivalentOf("<path class=\"mud-chart-serie mud-chart-line\" blazor:onclick=\"15\" fill=\"none\" stroke=\"#2979FF\" stroke-opacity=\"1\" stroke-width=\"3\" d=\"M 30 320 L 243.3333 320 L 456.6667 320 L 670 320\"></path>");

            // check the axis
            comp.Markup.Should().ContainEquivalentOf("<g class=\"mud-charts-gridlines-yaxis\"><path stroke=\"#e0e0e0\" stroke-width=\"0.3\" d=\"M 30 320 L 670 320\"></path></g></g>");
            comp.Markup.Should().ContainEquivalentOf("<text x='20' y='325' font-size='12px' text-anchor='end' dominant-baseline='auto'>1000</text></g>");
            comp.Markup.Should().ContainEquivalentOf("<text x='30' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 30 340)'>23:00</text><text x='243.3333' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 243.3333 340)'>00:00</text><text x='456.6667' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 456.6667 340)'>01:00</text><text x='670' y='340' font-size='12px' text-anchor='middle' dominant-baseline='middle' transform='rotate(0 670 340)'>02:00</text></g>");
        }

        [Test]
        public void TimeSeriesChartMatchBounds()
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

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeValue<double>(time.AddHours(x), 1000)).ToList(),
                        Visible = true,
                    }
                ])
                .Add(p => p.ChartOptions, new TimeSeriesChartOptions() { TimeLabelSpacing = TimeSpan.FromHours(1), LineDisplayType = LineDisplayType.Line })
                .Add(p => p.Width, "1000px")
                .Add(p => p.Height, "400px")
                .Add(p => p.MatchBoundsToSize, true));

            // check the size/bounds
            comp.Markup.Should().ContainEquivalentOf("<svg class=\"mud-chart-line mud-ltr\" width=\"100%\" height=\"400px\" viewBox=\"0 0 1000 400\"");
        }

        [Test]
        public void TimeSeriesChartTimeLabelSpacingRounding()
        {
            var time = new DateTime(2000, 1, 1);

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeValue < double >(time.AddHours(x).AddMinutes(10), 1000)).ToList(),
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

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.ChartSeries, [
                    new ()
                    {
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeValue<double>(time.AddHours(x).AddMinutes(10), 1000)).ToList(),
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
            var comp = Context.Render<TimeSeries<double>>();
            comp.Markup.Should().Contain("mud-chart-line mud-ltr");
        }

        [Test]
        public void TimeSeriesChartLabelFormats()
        {
            var time = new DateTime(2000, 1, 1);
            var format = "dd/MM HH:mm";

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.ChartSeries, new() {
                    new ChartSeries<double>()
                    {
                        Name = "Series 1",
                        Data = new[] {-1, 0, 1, 2}.Select(x => new TimeValue<double>(time.AddDays(x), 1000)).ToList(),
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

        [Test]
        public async Task TimeSeriesChart_CanHideSeries()
        {
            var mockYAxisLabelSize = new ElementSize { Width = 27.5, Height = 14.8 };
            var mockXAxisLabelSize = new ElementSize { Width = 50.5, Height = 14.8 }; // Adjusted width slightly
            var jsInteropCounter = 1; // Use a different name to avoid conflict if other tests run in parallel context

            Context.JSInterop.Setup<ElementSize>("mudGetSvgBBox", args =>
            {
                // This logic attempts to mimic distinct calls for X and Y, though it's a simplification.
                // The core idea is to provide *some* valid ElementSize for the chart to proceed.
                if (jsInteropCounter % 2 == 0)
                {
                    jsInteropCounter++;
                    return true; // Should match the specific call if the args were inspected
                }
                return false;
            }).SetResult(mockXAxisLabelSize);

            Context.JSInterop.Setup<ElementSize>("mudGetSvgBBox", args =>
            {
                if (jsInteropCounter % 2 == 1)
                {
                    jsInteropCounter++;
                    return true;
                }
                return false;
            }).SetResult(mockYAxisLabelSize);

            var time = new DateTime(2023, 1, 1);
            var chartSeries = new List<ChartSeries<double>>()
            {
                new () {
                    Name = "Temperature",
                    Data = new List<TimeValue<double>>() {
                        new(time, 20), new(time.AddHours(1), 22), new(time.AddHours(2), 21)
                    }
                },
                new () {
                    Name = "Humidity",
                    Data = new List<TimeValue<double>>() {
                        new(time, 60), new(time.AddHours(1), 65), new(time.AddHours(2), 62)
                    }
                },
                new () {
                    Name = "Pressure",
                    Data = new List<TimeValue<double>>() {
                        new(time, 1012), new(time.AddHours(1), 1010), new(time.AddHours(2), 1011)
                    },
                    Visible = false // Initially hidden
                }
            };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Timeseries)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.CanHideSeries, true)
                .Add(p => p.ChartOptions, new TimeSeriesChartOptions { TimeLabelSpacing = TimeSpan.FromHours(1), ChartPalette = ["#2979FF", "#1DE9B6", "#FFC400"] })
            );

            // Initial state assertions
            var seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
            seriesCheckboxes.Count.Should().Be(chartSeries.Count, "Number of checkboxes should match number of series");

            seriesCheckboxes[0].IsChecked().Should().BeTrue("Temperature checkbox should be initially checked");
            seriesCheckboxes[1].IsChecked().Should().BeTrue("Humidity checkbox should be initially checked");
            seriesCheckboxes[2].IsChecked().Should().BeFalse("Pressure checkbox should be initially unchecked");

            var series1 = "[stroke='#2979FF']";
            var series2 = "[stroke='#1DE9B6']";
            var series3 = "[stroke='#FFC400']";

            comp.FindAll($"path.mud-chart-line{series1}").Count.Should().Be(1, "Temperature series path should initially be visible");
            comp.FindAll($"path.mud-chart-line{series2}").Count.Should().Be(1, "Humidity series path should initially be visible");
            comp.FindAll($"path.mud-chart-line{series3}").Count.Should().Be(0, "Pressure series path should initially be hidden");

            // Hide Temperature series
            await seriesCheckboxes[0].ChangeAsync(false);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeFalse("Temperature checkbox should be unchecked after hiding");
            chartSeries[0].Visible.Should().BeFalse("Temperature Visible property should be false after hiding");
            comp.FindAll($"path.mud-chart-line{series1}").Count.Should().Be(0, "Temperature series path should be hidden");

            // Show Temperature series again
            await seriesCheckboxes[0].ChangeAsync(true);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[0].IsChecked().Should().BeTrue("Temperature checkbox should be checked after re-showing");
            chartSeries[0].Visible.Should().BeTrue("Temperature Visible property should be true after re-showing");
            comp.FindAll($"path.mud-chart-line{series1}").Count.Should().Be(1, "Temperature series path should be visible again");

            // Hide Humidity series
            await seriesCheckboxes[1].ChangeAsync(false);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[1].IsChecked().Should().BeFalse("Humidity checkbox should be unchecked after hiding");
            chartSeries[1].Visible.Should().BeFalse("Humidity Visible property should be false after hiding");
            comp.FindAll($"path.mud-chart-line{series2}").Count.Should().Be(0, "Humidity series path should be hidden");
            comp.FindAll($"path.mud-chart-line{series1}").Count.Should().Be(1, "Temperature series path should remain visible");

            // Show Pressure series
            await seriesCheckboxes[2].ChangeAsync(true);
            seriesCheckboxes = comp.FindAll(".mud-checkbox-input"); // Re-find
            seriesCheckboxes[2].IsChecked().Should().BeTrue("Pressure checkbox should be checked after showing");
            chartSeries[2].Visible.Should().BeTrue("Pressure Visible property should be true after showing");
            comp.FindAll($"path.mud-chart-line{series3}").Count.Should().Be(1, "Pressure series path should be visible");
        }
    }
}
