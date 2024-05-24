// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
                        Data = Enumerable.Range(-5, 5).Select(x => new TimeSeriesChartSeries.TimeValue(now.AddDays(x), _random.Next(6000, 15000))).ToList(),
                        IsVisible = true,
                        Type = TimeSeriesDiplayType.Line
                    }
                })
                .Add(p => p.TimeLabelSpacing, TimeSpan.FromDays(1))
                .Add(p => p.TimeLabelFormat, format));
            
            for(var i = -5; i < 5; i++) {
                var expectedTimeString = time.AddDays(i).ToString(format);
                comp.Markup.Should().Contain(expectedTimeString);

            }
        }
    }
}
