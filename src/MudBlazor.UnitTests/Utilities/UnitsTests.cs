// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using AwesomeAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;
using NUnit.VisualStudio.TestAdapter.NUnitEngine;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class UnitsTests : BunitTest
    {
        public record PrimitiveUnitTestCase(Func<double, ICssUnit> Factory, string Suffix);

        private static IEnumerable<PrimitiveUnitTestCase> PrimitiveTestCases()
        {
            yield return new(Units.Px, "px");
            yield return new(Units.Rem, "rem");
            yield return new(Units.Pct, "%");
            yield return new(Units.Em, "em");
            yield return new(Units.Vw, "vw");
            yield return new(Units.Vh, "vh");
            yield return new(Units.VMin, "vmin");
            yield return new(Units.VMax, "vmax");
            yield return new(Units.Cap, "cap");
            yield return new(Units.Rcap, "rcap");
            yield return new(Units.Ch, "ch");
            yield return new(Units.Rch, "rch");
            yield return new(Units.Ic, "ic");
            yield return new(Units.Ric, "ric");
            yield return new(Units.In, "in");
            yield return new(Units.Ex, "ex");
            yield return new(Units.Rex, "rex");
            yield return new(Units.Cm, "cm");
            yield return new(Units.Q, "q");
            yield return new(Units.Vi, "vi");
            yield return new(Units.Vb, "vb");
            yield return new(Units.Pt, "pt");
            yield return new(Units.Pc, "pc");
            yield return new(Units.Mm, "mm");
            yield return new(Units.Lh, "lh");
            yield return new(Units.Rlh, "rlh");
            yield return new(Units.Fr, "fr");
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(25)]
        [TestCase(50)]
        public void Units_Primitives_ReturnCorrectString(double value)
        {
            foreach (var testCase in PrimitiveTestCases())
            {
                testCase.Factory(value).ToString().Should().Be($"{value}{testCase.Suffix}");
            }
        }

        [Test]
        public void Units_Auto_ReturnsCorrectString()
        {
            Units.Auto().ToString().Should().Be("auto");
        }

        [Test]
        public void Units_MinContent_ReturnsCorrectString()
        {
            Units.MinContent().ToString().Should().Be("min-content");
        }

        [Test]
        public void Units_MaxContent_ReturnsCorrectString()
        {
            Units.MaxContent().ToString().Should().Be("max-content");
        }

        private record MinMaxTestCase(Func<double, string> Result, Func<double, string> Expected);

        private static IEnumerable<MinMaxTestCase> MinMaxTestCases()
        {
            // tests MinMaxFixedMin
            yield return new(
                dbl => Units.Min(Units.Px(dbl)).Max(Units.Fr()).ToString(),
                dbl => $"minmax({dbl}px, 1fr)");

            // tests MinMaxFixedMax
            yield return new(
                dbl => Units.Min(Units.MinContent()).Max(Units.Px(dbl)).ToString(),
                dbl => $"minmax(min-content, {dbl}px)");

            // tests MinMax
            yield return new(
                dbl => Units.Min(Units.MinContent()).Max(Units.Fr(dbl)).ToString(),
                dbl => $"minmax(min-content, {dbl}fr)");
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(25)]
        [TestCase(50)]
        public void Units_MinMax_ReturnsCorrectString(double value)
        {
            foreach (var testCase in MinMaxTestCases())
            {
                testCase.Result(value).Should().Be(testCase.Expected(value));
            }
        }

        [TestCase(1, "min(1px, 1rem)")]
        [TestCase(10, "min(10px, 1rem)")]
        [TestCase(25, "min(25px, 1rem)")]
        [TestCase(50, "min(50px, 1rem)")]
        public void Units_Min_ReturnsCorrectString(double px, string expected)
        {
            Units.Min(Units.Px(px), Units.Rem(1)).ToString().Should().Be(expected);
        }

        [TestCase(1, "max(1px, 1rem)")]
        [TestCase(10, "max(10px, 1rem)")]
        [TestCase(25, "max(25px, 1rem)")]
        [TestCase(50, "max(50px, 1rem)")]
        public void Units_Max_ReturnsCorrectString(double px, string expected)
        {
            Units.Max(Units.Px(px), Units.Rem(1)).ToString().Should().Be(expected);
        }

        [TestCase(1, "calc(1px + 1rem)")]
        [TestCase(10, "calc(10px + 1rem)")]
        [TestCase(25, "calc(25px + 1rem)")]
        [TestCase(50, "calc(50px + 1rem)")]
        public void Units_Add_ReturnsCorrectString(double px, string expected)
        {
            (Units.Px(px) + Units.Rem(1)).ToString().Should().Be(expected);
        }

        [TestCase(1, "calc(1px - 1rem)")]
        [TestCase(10, "calc(10px - 1rem)")]
        [TestCase(25, "calc(25px - 1rem)")]
        [TestCase(50, "calc(50px - 1rem)")]
        public void Units_Subtract_ReturnsCorrectString(double px, string expected)
        {
            (Units.Px(px) - Units.Rem(1)).ToString().Should().Be(expected);
        }

        [TestCase(1, "calc(1px * 2)")]
        [TestCase(10, "calc(10px * 2)")]
        [TestCase(25, "calc(25px * 2)")]
        [TestCase(50, "calc(50px * 2)")]
        public void Units_Multiply_ReturnsCorrectString(double px, string expected)
        {
            (Units.Px(px) * 2).ToString().Should().Be(expected);
        }

        [TestCase(1, "calc(1px / 2)")]
        [TestCase(10, "calc(10px / 2)")]
        [TestCase(25, "calc(25px / 2)")]
        [TestCase(50, "calc(50px / 2)")]
        public void Units_Divide_ReturnsCorrectString(double px, string expected)
        {
            (Units.Px(px) / 2).ToString().Should().Be(expected);
        }

        [TestCase(1, 1, true)]
        [TestCase(1, 10, false)]
        [TestCase(25, 25, true)]
        [TestCase(25, 50, false)]
        public void Units_Equality_ReturnsCorrectResult(double a, double b, bool expected)
        {
            Units.Px(a).Equals(Units.Px(b)).Should().Be(expected);
        }
    }
}
