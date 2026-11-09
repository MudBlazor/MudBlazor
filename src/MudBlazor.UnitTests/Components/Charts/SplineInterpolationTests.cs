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

        if (option == InterpolationOption.NaturalSpline)
        {
            // Two-point natural spline reduces to a straight line: midpoint x=0.5 -> y=15.
            var midIndex = Array.FindIndex(spline.InterpolatedXs, x => Math.Abs(x - 0.5) < 1e-9);
            midIndex.Should().BeGreaterThanOrEqualTo(0);
            spline.InterpolatedYs[midIndex].Should().BeApproximately(15, 1e-9);
        }

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

    [Test]
    public void Solve_EmptyInput_ReturnsEmptyArray()
    {
        var result = Interpolation.TridiagonalSolver.Solve([], [], [], []);

        result.Should().BeEmpty();
    }

    [Test]
    public void Solve_MismatchedLengths_Throws()
    {
        var a = new double[] { 0, 1 };
        var b = new double[] { 2, 2 };
        var c = new double[] { 1, 0 };
        var d = new double[] { 1 };

        var action = () => Interpolation.TridiagonalSolver.Solve(a, b, c, d);

        action.Should().Throw<ArgumentException>().WithMessage("*same length*");
    }

    [Test]
    public void Solve_SingleEquation_ReturnsQuotient()
    {
        var result = Interpolation.TridiagonalSolver.Solve([0], [4], [0], [8]);

        result.Should().HaveCount(1);
        result[0].Should().BeApproximately(2.0, 1e-12);
    }

    [Test]
    public void Solve_SingleEquation_ZeroDiagonal_Throws()
    {
        var action = () => Interpolation.TridiagonalSolver.Solve([0], [0], [0], [1]);

        action.Should().Throw<InvalidOperationException>().WithMessage("*zero or near-zero*");
    }

    [Test]
    public void Solve_DenominatorZero_Throws()
    {
        // b[0]=1 -> cPrime[0] = 1, so denom at i==1 is b[1] - a[1]*cPrime[0] = 1 - 1*1 = 0.
        var a = new double[] { 0, 1, 0 };
        var b = new double[] { 1, 1, 1 };
        var c = new double[] { 1, 0, 0 };
        var d = new double[] { 1, 1, 1 };

        var action = () => Interpolation.TridiagonalSolver.Solve(a, b, c, d);

        action.Should().Throw<InvalidOperationException>().WithMessage("*Denominator at index 1*");
    }

    [Test]
    public void SolveCyclic_EmptyInput_ReturnsEmptyArray()
    {
        var result = Interpolation.TridiagonalSolver.SolveCyclic([], [], [], []);

        result.Should().BeEmpty();
    }

    [Test]
    public void SolveCyclic_MismatchedLengths_Throws()
    {
        var action = () => Interpolation.TridiagonalSolver.SolveCyclic([0, 0, 0], [1, 1], [0, 0, 0], [1, 1, 1]);

        action.Should().Throw<ArgumentException>().WithMessage("*same length*");
    }

    [Test]
    public void SolveCyclic_GammaNearZero_FallsBackToOne_AndReturnsFinite()
    {
        // gamma = -b[0]; b[0] near-zero forces the gamma = 1.0 fallback branch.
        var a = new double[] { 1, 1, 1, 1 };
        var b = new double[] { 0, 4, 4, 4 };
        var c = new double[] { 1, 1, 1, 1 };
        var d = new double[] { 1, 2, 3, 4 };

        double[] result = null!;
        var action = () => result = Interpolation.TridiagonalSolver.SolveCyclic(a, b, c, d);

        action.Should().NotThrow();
        result.Should().HaveCount(4);
        result.Should().OnlyContain(v => !double.IsNaN(v) && !double.IsInfinity(v));
    }

    [Test]
    public void SplineInterpolator_MismatchedXsYs_Throws()
    {
        var action = () => new Interpolation.NaturalSpline([0, 1, 2], [0, 1]);

        action.Should().Throw<ArgumentException>().WithMessage("*same length*");
    }

    [Test]
    public void SplineInterpolator_EmptyInput_Throws()
    {
        var action = () => new Interpolation.NaturalSpline([], []);

        action.Should().Throw<ArgumentException>().WithMessage("*length of 1 or greater*");
    }

    [TestCase(0)]
    [TestCase(-5)]
    public void SplineInterpolator_InvalidResolution_Throws(int resolution)
    {
        var action = () => new Interpolation.NaturalSpline([0, 1, 2], [0, 1, 2], resolution);

        action.Should().Throw<ArgumentException>().WithMessage("*resolution must be 1 or greater*");
    }

    [Test]
    public void EndSlopeSpline_NonZeroEndSlopes_ProducesFiniteCurve()
    {
        var spline = new Interpolation.EndSlopeSpline([0, 1, 2, 3], [0, 1, 0, 1], resolution: 10, firstSlopeDegrees: 30, lastSlopeDegrees: -30);

        spline.InterpolatedXs.Should().HaveCount((10 * 3) + 1);
        spline.InterpolatedXs[0].Should().BeApproximately(0, 1e-12);
        spline.InterpolatedXs[^1].Should().BeApproximately(3, 1e-12);
        AssertFinite(spline.InterpolatedYs);
    }

    [Test]
    public void NaturalSpline_Integrate_ReturnsFiniteValue()
    {
        var spline = new Interpolation.NaturalSpline([0, 1, 2, 3], [0, 1, 4, 9], resolution: 10);

        var integral = ((Interpolation.SplineInterpolator)spline).Integrate();

        integral.Should().NotBe(double.NaN);
        double.IsInfinity(integral).Should().BeFalse();
        integral.Should().BeGreaterThan(0);
    }

    [Test]
    public void NaturalSpline_DuplicateX_UsesZeroSpacingBranch_AndStaysFinite()
    {
        // Two consecutive equal X values make spacing h == 0, driving the guarded zero-spacing branch.
        var spline = new Interpolation.NaturalSpline([0, 1, 1, 2], [0, 1, 2, 3], resolution: 5);

        AssertFinite(spline.InterpolatedYs);
        spline.InterpolatedXs[0].Should().BeApproximately(0, 1e-12);
        spline.InterpolatedXs[^1].Should().BeApproximately(2, 1e-12);
    }

    [Test]
    public void EndSlopeSpline_DuplicateX_UsesZeroSpacingBranch_AndStaysFinite()
    {
        var spline = new Interpolation.EndSlopeSpline([0, 1, 1, 2], [0, 1, 2, 3], resolution: 5);

        AssertFinite(spline.InterpolatedYs);
        spline.InterpolatedXs[0].Should().BeApproximately(0, 1e-12);
        spline.InterpolatedXs[^1].Should().BeApproximately(2, 1e-12);
    }

    [Test]
    public void PeriodicSpline_DuplicateX_UsesZeroSpacingBranch_AndStaysFinite()
    {
        var spline = new Interpolation.PeriodicSpline([0, 1, 1, 2], [10, 20, 20, 10], resolution: 5);

        AssertFinite(spline.InterpolatedYs);
        spline.InterpolatedXs[0].Should().BeApproximately(0, 1e-12);
        spline.InterpolatedXs[^1].Should().BeApproximately(2, 1e-12);
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
