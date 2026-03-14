// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts;

[TestFixture]
public class SplineInterpolationTests : BunitTest
{
    [TestCase(InterpolationOption.NaturalSpline)]
    [TestCase(InterpolationOption.EndSlope)]
    [TestCase(InterpolationOption.Periodic)]
    public void SplineInterpolator_ShouldReturnOriginalPoint_ForSinglePoint(InterpolationOption option)
    {
        var spline = CreateInterpolator(option, [0], [10]);

        spline.InterpolatedXs.Should().Equal([0]);
        spline.InterpolatedYs.Should().Equal([10]);
    }

    [TestCase(InterpolationOption.NaturalSpline)]
    [TestCase(InterpolationOption.EndSlope)]
    [TestCase(InterpolationOption.Periodic)]
    public void SplineInterpolator_ShouldPreserveEndpoints_ForTwoPoints(InterpolationOption option)
    {
        var spline = CreateInterpolator(option, [0, 1], [10, 20]);

        spline.InterpolatedXs.Should().HaveCount(11);
        spline.InterpolatedYs.Should().HaveCount(11);
        spline.InterpolatedXs[0].Should().Be(0);
        spline.InterpolatedXs[^1].Should().Be(1);
        spline.InterpolatedYs[0].Should().Be(10);
        spline.InterpolatedYs[^1].Should().Be(20);

        AssertFinite(spline.InterpolatedXs);
        AssertFinite(spline.InterpolatedYs);
    }

    [TestCase(InterpolationOption.NaturalSpline)]
    [TestCase(InterpolationOption.EndSlope)]
    [TestCase(InterpolationOption.Periodic)]
    public void SplineInterpolator_ShouldReturnFiniteValues_ForThreePoints(InterpolationOption option)
    {
        var spline = CreateInterpolator(option, [0, 1, 2], [10, 20, 15]);

        spline.InterpolatedXs.Should().HaveCount(21);
        spline.InterpolatedYs.Should().HaveCount(21);
        spline.InterpolatedXs[0].Should().Be(0);
        spline.InterpolatedXs[^1].Should().Be(2);
        spline.InterpolatedYs[0].Should().Be(10);
        spline.InterpolatedYs[^1].Should().Be(15);

        AssertFinite(spline.InterpolatedXs);
        AssertFinite(spline.InterpolatedYs);
    }

    [TestCase(InterpolationOption.NaturalSpline)]
    [TestCase(InterpolationOption.EndSlope)]
    [TestCase(InterpolationOption.Periodic)]
    public void SplineInterpolator_LargeData_ShouldRemainFinite(InterpolationOption option)
    {
        var n = 1000;
        var xs = new double[n];
        var ys = new double[n];
        for (var i = 0; i < n; i++)
        {
            xs[i] = i;
            ys[i] = i % 2 == 0 ? 0 : 100;
        }

        var spline = CreateInterpolator(option, xs, ys);

        spline.InterpolatedXs.Should().HaveCount(9991);
        spline.InterpolatedYs.Should().HaveCount(9991);
        spline.InterpolatedXs[0].Should().Be(0);
        spline.InterpolatedXs[^1].Should().Be(n - 1);
        spline.InterpolatedYs[0].Should().Be(0);
        spline.InterpolatedYs[^1].Should().Be(100);

        AssertFinite(spline.InterpolatedXs);
        AssertFinite(spline.InterpolatedYs);
    }

    [Test]
    public void SplineInterpolation_ShouldNotClampToZero_InBaseInterpolator()
    {
        // [1, 0, 0, 1] for natural spline will typically dip below zero between indices 1 and 2
        var xs = new double[] { 0, 1, 2, 3 };
        var ys = new double[] { 1, 0, 0, 1 };

        var spline = new Interpolation.NaturalSpline(xs, ys, resolution: 100);

        spline.InterpolatedYs.Should().Contain(y => y < 0, "Base interpolator should NOT clamp, it should be done at the chart level");
    }

    [Test]
    public void SplineInterpolation_ClampToZero_ShouldIncludeZeroOnYAxis_ForPositiveOnlyData()
    {
        var chartSeries = new List<ChartSeries<double>>()
        {
            new() { Name = "Series 1", Data = new double[] { 30, 21, 21, 30 } }
        };
        var chartLabels = new[] { "A", "B", "C", "D" };

        var unclampedComp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Line)
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, new LineChartOptions
            {
                InterpolationOption = InterpolationOption.NaturalSpline,
                YAxisTicks = 10,
                YAxisToStringFunc = value => value.ToString("F0", CultureInfo.InvariantCulture)
            }));

        var unclampedYAxisLabels = unclampedComp.FindAll("g.mud-charts-yaxis text").Select(e => e.TextContent.Trim()).ToList();
        unclampedYAxisLabels.Should().NotContain(label => label == "0");

        var clampedComp = Context.Render<MudChart<double>>(parameters => parameters
            .Add(p => p.ChartType, ChartType.Line)
            .Add(p => p.ChartSeries, chartSeries)
            .Add(p => p.ChartLabels, chartLabels)
            .Add(p => p.ChartOptions, new LineChartOptions
            {
                ClampToZero = true,
                InterpolationOption = InterpolationOption.NaturalSpline,
                YAxisTicks = 10,
                YAxisToStringFunc = value => value.ToString("F0", CultureInfo.InvariantCulture)
            }));

        var clampedYAxisLabels = clampedComp.FindAll("g.mud-charts-yaxis text").Select(e => e.TextContent.Trim()).ToList();
        clampedYAxisLabels.Should().Contain(label => label == "0");
    }

    [Test]
    public void TridiagonalSolver_ShouldThrow_WhenSingular()
    {
        // A singular system: b=0
        var a = new double[] { 0, 1, 1 };
        var b = new double[] { 0, 2, 2 };
        var c = new double[] { 1, 1, 0 };
        var d = new double[] { 1, 2, 3 };

        var action = () => Interpolation.TridiagonalSolver.Solve(a, b, c, d);
        action.Should().Throw<InvalidOperationException>().WithMessage("*zero or near-zero*");
    }

    private static Interpolation.ILineInterpolator CreateInterpolator(InterpolationOption option, double[] xs, double[] ys, int resolution = 10)
    {
        return option switch
        {
            InterpolationOption.NaturalSpline => new Interpolation.NaturalSpline(xs, ys, resolution),
            InterpolationOption.EndSlope => new Interpolation.EndSlopeSpline(xs, ys, resolution),
            InterpolationOption.Periodic => new Interpolation.PeriodicSpline(xs, ys, resolution),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, "Unsupported spline interpolation option")
        };
    }

    private static void AssertFinite(IEnumerable<double> values)
    {
        values.Should().OnlyContain(value => !double.IsNaN(value) && !double.IsInfinity(value));
    }
}
