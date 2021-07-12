// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class MudColorTests
    {
        [Test]
        [TestCase("12315aca", 18, 49, 90, 202)]
        [TestCase("12315a", 18, 49, 90, 255)]
        [TestCase("#12315a", 18, 49, 90, 255)]
        [TestCase("12315ACA", 18, 49, 90, 202)]
        [TestCase("12315Aca", 18, 49, 90, 202)]
        [TestCase("#12315Aca", 18, 49, 90, 202)]
        [TestCase("1ab", 17, 170, 187, 255)]
        [TestCase("1AB", 17, 170, 187, 255)]
        [TestCase("1abd", 17, 170, 187, 221)]
        public void FromString_Hex(string input, byte r, byte g, byte b, byte alpha)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en");

            {
                MudColor color = new(input);

                color.R.Should().Be(r);
                color.G.Should().Be(g);
                color.B.Should().Be(b);
                color.A.Should().Be(alpha);
            }

            CultureInfo.CurrentCulture = new CultureInfo("se");
            {
                MudColor color = new(input);

                color.R.Should().Be(r);
                color.G.Should().Be(g);
                color.B.Should().Be(b);
                color.A.Should().Be(alpha);
            }
        }

        [Test]
        [TestCase("rgb(12,204,210)", 12, 204, 210, 255)]
        [TestCase("rgb(0,0,0)", 0, 0, 0, 255)]
        [TestCase("rgb(255,255,255)", 255, 255, 255, 255)]
        public void FromString_RGB(string input, byte r, byte g, byte b, byte alpha)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en");

            {
                MudColor color = new(input);

                color.R.Should().Be(r);
                color.G.Should().Be(g);
                color.B.Should().Be(b);
                color.A.Should().Be(alpha);
            }

            CultureInfo.CurrentCulture = new CultureInfo("se");
            {
                MudColor color = new(input);

                color.R.Should().Be(r);
                color.G.Should().Be(g);
                color.B.Should().Be(b);
                color.A.Should().Be(alpha);
            }
        }

        [Test]
        [TestCase("rgba(12,204,210,0.5)", 12, 204, 210, 127)]
        [TestCase("rgba(0,0,0,0)", 0, 0, 0, 0)]
        [TestCase("rgba(255,255,255,1)", 255, 255, 255, 255)]
        public void FromString_RGBA(string input, byte r, byte g, byte b, byte alpha)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en");

            {
                MudColor color = new(input);

                color.R.Should().Be(r);
                color.G.Should().Be(g);
                color.B.Should().Be(b);
                color.A.Should().Be(alpha);
            }

            CultureInfo.CurrentCulture = new CultureInfo("se");
            {
                MudColor color = new(input);

                color.R.Should().Be(r);
                color.G.Should().Be(g);
                color.B.Should().Be(b);
                color.A.Should().Be(alpha);
            }
        }
    }
}
