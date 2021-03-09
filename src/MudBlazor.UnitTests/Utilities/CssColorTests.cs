// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    public class CssColorTests
    {
        [TestCase("#12131415", new Byte[] { 0x12, 0x13, 0x14, 0x15 })]
        [TestCase("12131415", new Byte[] { 0x12, 0x13, 0x14, 0x15 })]
        [TestCase("#121314", new Byte[] { 0x12, 0x13, 0x14, 0xFF })]
        [TestCase("121314", new Byte[] { 0x12, 0x13, 0x14, 0xFF })]
        [TestCase("123A", new Byte[] { 0x11, 0x22, 0x33, 0xAA })]
        [TestCase("123a", new Byte[] { 0x11, 0x22, 0x33, 0xAA })]
        [TestCase("123", new Byte[] { 0x11, 0x22, 0x33, 0xFF })]
        [TestCase("rgb(125,245,180)", new Byte[] { 125, 245, 180, 0xFF })]
        [TestCase("rgba(125,245,180,27)", new Byte[] { 125, 245, 180, 27 })]
        public void FromInputToByteArray(String input, Byte[] expectedbyte)
        {
            CssColor color = new CssColor(input);

            Byte r = color.R;
            Byte g = color.G;
            Byte b = color.B;
            Byte a = color.A;

            expectedbyte[0].Should().Be(r);
            expectedbyte[1].Should().Be(g);
            expectedbyte[2].Should().Be(b);
            expectedbyte[3].Should().Be(a);

        }

        [TestCase("#1213141")]
        [TestCase("#AABBHH")]
        [TestCase("#12122")]
        [TestCase("0x12122")]
        [TestCase("rgb(125,245,256)")]
        [TestCase("rgba(125,245,256)")]
        [TestCase("rgba(125,245)")]
        [TestCase("rgb(125,245,252,243)")]
        public void FromInput_Failed(String input)
        {
            Assert.That(() => new CssColor(input), Throws.Exception);
        }

        [Test]
        public void ImplicitFromString()
        {
            String colorString = "12131415";

            CssColor color = colorString;

            color.R.Should().Be(0x12);
            color.G.Should().Be(0x13);
            color.B.Should().Be(0x14);
            color.A.Should().Be(0x15);
        }

        [Test]
        public void ExplicitToStringString()
        {
            String colorString = "12131415";

            CssColor color = new CssColor(colorString);

            ((String)color).Should().Be(colorString);
        }

        [Test]
        public void ExplicitFromString_WhenNull()
        {
            CssColor color = null;
            String asString = (String)color;

            Assert.True(String.IsNullOrEmpty(asString));
        }

        [Test]
        public void Test_ToString()
        {
            String colorString = "12131415";
            CssColor color = new CssColor(colorString);

            color.ToString().Should().Be(colorString);
        }
    }
}
