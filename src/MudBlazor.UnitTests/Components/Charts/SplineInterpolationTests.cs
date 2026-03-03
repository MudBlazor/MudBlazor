// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    [TestFixture]
    public class SplineInterpolationTests : BunitTest
    {
        [TestCase(InterpolationOption.NaturalSpline)]
        [TestCase(InterpolationOption.EndSlope)]
        [TestCase(InterpolationOption.Periodic)]
        public void SplineInterpolation_ShouldNotThrow_WithOnePoint(InterpolationOption option)
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new ChartSeries<double>() { Name = "Series 1", Data = new double[] { 10 } }
            };

            var action = () => Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartOptions, new LineChartOptions { InterpolationOption = option }));

            action.Should().NotThrow();
        }

        [TestCase(InterpolationOption.NaturalSpline)]
        [TestCase(InterpolationOption.EndSlope)]
        [TestCase(InterpolationOption.Periodic)]
        public void SplineInterpolation_ShouldNotThrow_WithTwoPoints(InterpolationOption option)
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new ChartSeries<double>() { Name = "Series 1", Data = new double[] { 10, 20 } }
            };

            var action = () => Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartOptions, new LineChartOptions { InterpolationOption = option }));

            action.Should().NotThrow();
        }

        [TestCase(InterpolationOption.NaturalSpline)]
        [TestCase(InterpolationOption.EndSlope)]
        [TestCase(InterpolationOption.Periodic)]
        public void SplineInterpolation_ShouldNotThrow_WithThreePoints(InterpolationOption option)
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new ChartSeries<double>() { Name = "Series 1", Data = new double[] { 10, 20, 15 } }
            };

            var action = () => Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartOptions, new LineChartOptions { InterpolationOption = option }));

            action.Should().NotThrow();
        }

        [TestCase(InterpolationOption.NaturalSpline)]
        [TestCase(InterpolationOption.EndSlope)]
        [TestCase(InterpolationOption.Periodic)]
        public void SplineInterpolation_ShouldNotThrow_WithManyPoints(InterpolationOption option)
        {
            var data = new double[600];
            for (var i = 0; i < 600; i++)
            {
                data[i] = Math.Sin(i * 0.1);
            }

            var chartSeries = new List<ChartSeries<double>>()
            {
                new ChartSeries<double>() { Name = "Series 1", Data = data }
            };

            var action = () => Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartOptions, new LineChartOptions { InterpolationOption = option }));

            action.Should().NotThrow();
        }

        [Test]
        public void NaturalSpline_LargeData_ShouldBeStable()
        {
            var n = 1000;
            var xs = new double[n];
            var ys = new double[n];
            for (var i = 0; i < n; i++)
            {
                xs[i] = i;
                ys[i] = i % 2 == 0 ? 0 : 100;
            }

            var action = () => new Interpolation.NaturalSpline(xs, ys);
            action.Should().NotThrow();

            var spline = new Interpolation.NaturalSpline(xs, ys);
            spline.InterpolatedYs.Should().NotContain(double.NaN);
            spline.InterpolatedYs.Should().NotContain(double.PositiveInfinity);
            spline.InterpolatedYs.Should().NotContain(double.NegativeInfinity);
        }

        [Test]
        public void SplineInterpolation_ShouldClampToZero_WhenAllValuesAreNonNegative()
        {
            // [1, 0, 0, 1] for natural spline will typically dip below zero between indices 1 and 2
            var xs = new double[] { 0, 1, 2, 3 };
            var ys = new double[] { 1, 0, 0, 1 };

            var spline = new Interpolation.NaturalSpline(xs, ys, resolution: 100);

            spline.InterpolatedYs.Should().OnlyContain(y => y >= 0, "All interpolated values should be non-negative when input values are non-negative");
        }
    }
}
