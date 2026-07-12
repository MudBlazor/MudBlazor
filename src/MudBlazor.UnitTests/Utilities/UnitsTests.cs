// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class UnitsTests : BunitTest
    {
        [TestCase(1, "1fr")]
        [TestCase(10, "10fr")]
        [TestCase(25, "25fr")]
        [TestCase(50, "50fr")]
        public void Units_Fr_ReturnsCorrectString(double value, string expected)
        {
            Units.Fr(value).ToString().Should().Be(expected);
        }

        [Test]
        public void Units_Fr_DefaultsToOne()
        {
            Units.Fr().ToString().Should().Be("1fr");
        }

        [TestCase(1, "1px")]
        [TestCase(10, "10px")]
        [TestCase(25, "25px")]
        [TestCase(50, "50px")]
        public void Units_Px_ReturnsCorrectString(double value, string expected)
        {
            Units.Px(value).ToString().Should().Be(expected);
        }

        [TestCase(1, "1rem")]
        [TestCase(10, "10rem")]
        [TestCase(25, "25rem")]
        [TestCase(50, "50rem")]
        public void Units_Rem_ReturnsCorrectString(double value, string expected)
        {
            Units.Rem(value).ToString().Should().Be(expected);
        }

        [TestCase(1, "1%")]
        [TestCase(10, "10%")]
        [TestCase(25, "25%")]
        [TestCase(50, "50%")]
        public void Units_Pct_ReturnsCorrectString(double value, string expected)
        {
            Units.Pct(value).ToString().Should().Be(expected);
        }

        [TestCase(1, "1em")]
        [TestCase(10, "10em")]
        [TestCase(25, "25em")]
        [TestCase(50, "50em")]
        public void Units_Em_ReturnsCorrectString(double value, string expected)
        {
            Units.Em(value).ToString().Should().Be(expected);
        }

        [TestCase(1, "1vw")]
        [TestCase(10, "10vw")]
        [TestCase(25, "25vw")]
        [TestCase(50, "50vw")]
        public void Units_Vw_ReturnsCorrectString(double value, string expected)
        {
            Units.Vw(value).ToString().Should().Be(expected);
        }

        [TestCase(1, "1vh")]
        [TestCase(10, "10vh")]
        [TestCase(25, "25vh")]
        [TestCase(50, "50vh")]
        public void Units_Vh_ReturnsCorrectString(double value, string expected)
        {
            Units.Vh(value).ToString().Should().Be(expected);
        }

        [TestCase(1, "1vmin")]
        [TestCase(10, "10vmin")]
        [TestCase(25, "25vmin")]
        [TestCase(50, "50vmin")]
        public void Units_VMin_ReturnsCorrectString(double value, string expected)
        {
            Units.VMin(value).ToString().Should().Be(expected);
        }

        [TestCase(1, "1vmax")]
        [TestCase(10, "10vmax")]
        [TestCase(25, "25vmax")]
        [TestCase(50, "50vmax")]
        public void Units_VMax_ReturnsCorrectString(double value, string expected)
        {
            Units.VMax(value).ToString().Should().Be(expected);
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

        [TestCase(1, "minmax(1px, 1fr)")]
        [TestCase(10, "minmax(10px, 1fr)")]
        [TestCase(25, "minmax(25px, 1fr)")]
        [TestCase(50, "minmax(50px, 1fr)")]
        public void Units_MinMax_ReturnsCorrectString(double min, string expected)
        {
            Units.MinMax(Units.Px(min), Units.Fr()).ToString().Should().Be(expected);
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
