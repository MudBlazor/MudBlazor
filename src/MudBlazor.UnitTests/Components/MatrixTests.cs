// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using AngleSharp.Diffing.Extensions;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class MatrixTests : BunitTest
    {
        [Test]
        public void Units_ReturnsCorrectString()
        {
            Units.Fr(10).ToString().Should().Be("10fr");
            Units.Fr().ToString().Should().Be("1fr");
            Units.Px(50).ToString().Should().Be("50px");
            Units.Rem(8).ToString().Should().Be("8rem");
            Units.Pct(15).ToString().Should().Be("15%");
            Units.Auto().ToString().Should().Be("auto");
            Units.MinContent().ToString().Should().Be("min-content");
            Units.MaxContent().ToString().Should().Be("max-content");
            Units.MinMax(Units.MinContent(), Units.Fr()).ToString().Should().Be("minmax(min-content, 1fr)");
            Units.Min(Units.Pct(20), Units.Rem(21)).ToString().Should().Be("min(20%, 21rem)");
            Units.Max(Units.Px(11), Units.Pct(15)).ToString().Should().Be("max(11px, 15%)");
            Units.MinMax(Units.Min(Units.Pct(100), Units.Px(50)), Units.Fr()).ToString().Should().Be("minmax(min(100%, 50px), 1fr)");
            (Units.Px(7) + Units.Pct(32)).ToString().Should().Be("calc(7px + 32%)");
            (Units.Min(Units.Px(47), Units.Pct(4)) - Units.Max(Units.Pct(5), Units.Px(67))).ToString().Should().Be("calc(min(47px, 4%) - max(5%, 67px))");
            (Units.Max(Units.Pct(12), Units.Rem(12)) * 2).ToString().Should().Be("calc(max(12%, 12rem) * 2)");
            (Units.Px(12) / 3).ToString().Should().Be("calc(12px / 3)");
            (Units.Px(50) == Units.Pct(12)).Should().Be(false);
            (Units.Px(50) == Units.Px(50)).Should().Be(true);
        }

        [Test]
        public void ExplicitMatrix_ReturnsCorrectString()
        {
            ExplicitMatrix.Pattern(Units.Fr()).ToString().Should().Be("1fr");
            ExplicitMatrix.Pattern(Units.Px(50), Units.Rem(10), Units.Pct(10)).ToString().Should().Be("50px 10rem 10%");
            ExplicitMatrix.Pattern(3, Units.Px(50), Units.Rem(10), Units.Pct(10)).ToString().Should().Be("repeat(3, 50px 10rem 10%)");
            ExplicitMatrix.Fit(Units.Px(50)).ToString().Should().Be("repeat(auto-fit, 50px)");
            ExplicitMatrix.Fill(Units.MinMax(Units.Px(50), Units.MinContent())).ToString().Should().Be("repeat(auto-fill, minmax(50px, min-content))");
        }

        [Test]
        public void ImplicitMatrix_ReturnsCorrectString()
        {
            ImplicitMatrix.Pattern(Units.Fr()).ToString().Should().Be("1fr");
            ImplicitMatrix.Pattern(Units.Px(50), Units.Rem(10), Units.Pct(10)).ToString().Should().Be("50px 10rem 10%");
        }
    }
}
