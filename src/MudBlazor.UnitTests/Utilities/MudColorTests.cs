// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers.Binary;
using System.Globalization;
using System.IO;
using System.Text;
using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class MudColorTests
    {
        [Test]
        public void MudColor_STJ_Serialization()
        {
            var originalMudColor = new MudColor("#f6f9fb");

            var jsonString = System.Text.Json.JsonSerializer.Serialize(originalMudColor);
            var deserializeMudColor = System.Text.Json.JsonSerializer.Deserialize<MudColor>(jsonString);

            jsonString.Should().Be("{\"R\":246,\"G\":249,\"B\":251,\"A\":255}");
            deserializeMudColor.Should().Be(originalMudColor);
        }

        [Test]
        public void MudColor_Newtonsoft_Serialization()
        {
            var originalMudColor = new MudColor("#f6f9fb");

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(originalMudColor);
            var deserializeMudColor = Newtonsoft.Json.JsonConvert.DeserializeObject<MudColor>(jsonString);

            jsonString.Should().Be("{\"R\":246,\"G\":249,\"B\":251,\"A\":255}");
            deserializeMudColor.Should().Be(originalMudColor);
        }

        [Test]
        public void MudColor_Default_Ctor()
        {
            var defaultMudColor = new MudColor();
            var blackMudColor = new MudColor("#000000ff");

            defaultMudColor.R.Should().Be(0);
            defaultMudColor.G.Should().Be(0);
            defaultMudColor.B.Should().Be(0);
            defaultMudColor.A.Should().Be(255);
            defaultMudColor.H.Should().Be(0);
            defaultMudColor.L.Should().Be(0);
            defaultMudColor.S.Should().Be(0);
            defaultMudColor.APercentage.Should().Be(1);
            blackMudColor.Should().Be(defaultMudColor);
        }

        [Test]
        public void MudColor_XMLDataContract_Serialization()
        {
            var dataContractSerializer = new System.Runtime.Serialization.DataContractSerializer(typeof(MudColor));

            MudColor DeserializeXml(string toDeserialize)
            {
                using var textReader = new StringReader(toDeserialize);
                using var reader = System.Xml.XmlReader.Create(textReader);

                return (MudColor)dataContractSerializer.ReadObject(reader);
            }

            string SerializeXml(MudColor mudColorObject)
            {
                using var memoryStream = new MemoryStream();
                dataContractSerializer.WriteObject(memoryStream, mudColorObject);

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            var originalMudColor = new MudColor("#f6f9fb");

            var xmlString = SerializeXml(originalMudColor);
            var deserializeMudColor = DeserializeXml(xmlString);

            xmlString.Should().Be("<MudColor xmlns=\"http://schemas.datacontract.org/2004/07/MudBlazor.Utilities\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:x=\"http://www.w3.org/2001/XMLSchema\"><R i:type=\"x:unsignedByte\" xmlns=\"\">246</R><G i:type=\"x:unsignedByte\" xmlns=\"\">249</G><B i:type=\"x:unsignedByte\" xmlns=\"\">251</B><A i:type=\"x:unsignedByte\" xmlns=\"\">255</A></MudColor>");
            deserializeMudColor.Should().Be(originalMudColor);
        }

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
            var cultures = new[] { new CultureInfo("en", false), new CultureInfo("se", false) };

            foreach (var item in cultures)
            {
                CultureInfo.CurrentCulture = item;

                MudColor color = new(input);

                color.R.Should().Be(r);
                color.G.Should().Be(g);
                color.B.Should().Be(b);
                color.A.Should().Be(alpha);

                MudColor implicitCasted = input;

                implicitCasted.R.Should().Be(r);
                implicitCasted.G.Should().Be(g);
                implicitCasted.B.Should().Be(b);
                implicitCasted.A.Should().Be(alpha);
            }
        }

        [Test]
        [TestCase("rgb(12,204,210)", 12, 204, 210, 255)]
        [TestCase("rgb(0,0,0)", 0, 0, 0, 255)]
        [TestCase("rgb(255,255,255)", 255, 255, 255, 255)]
        public void FromString_RGB(string input, byte r, byte g, byte b, byte alpha)
        {
            var cultures = new[] { new CultureInfo("en", false), new CultureInfo("se", false) };

            foreach (var item in cultures)
            {
                CultureInfo.CurrentCulture = item;

                MudColor color = new(input);

                color.R.Should().Be(r);
                color.G.Should().Be(g);
                color.B.Should().Be(b);
                color.A.Should().Be(alpha);

                MudColor implicitCasted = input;

                implicitCasted.R.Should().Be(r);
                implicitCasted.G.Should().Be(g);
                implicitCasted.B.Should().Be(b);
                implicitCasted.A.Should().Be(alpha);
            }
        }

        [Test]
        [TestCase("rgba(12,204,210,0.5)", 12, 204, 210, 127)]
        [TestCase("rgba(0,0,0,0)", 0, 0, 0, 0)]
        [TestCase("rgba(255,255,255,1)", 255, 255, 255, 255)]
        public void FromString_RGBA(string input, byte r, byte g, byte b, byte alpha)
        {
            var cultures = new[] { new CultureInfo("en", false), new CultureInfo("se", false) };

            foreach (var item in cultures)
            {
                CultureInfo.CurrentCulture = item;

                MudColor color = new(input);

                color.R.Should().Be(r);
                color.G.Should().Be(g);
                color.B.Should().Be(b);
                color.A.Should().Be(alpha);

                MudColor implicitCasted = input;

                implicitCasted.R.Should().Be(r);
                implicitCasted.G.Should().Be(g);
                implicitCasted.B.Should().Be(b);
                implicitCasted.A.Should().Be(alpha);
            }
        }

        [Test]
        [TestCase("rgba(12,204,210,0.5)", 12, 204, 210, 127)]
        [TestCase("rgba(67,160,71,1)", 67, 160, 71, 1)]
        [TestCase("#43a047", 67, 160, 71, 1)]
        [TestCase("rgba(255,255,255,1)", 255, 255, 255, 255)]
        public void FromString_RGBA_And_Darken(string input, byte r, byte g, byte b, byte alpha)
        {
            var cultures = new[] { new CultureInfo("en", false), new CultureInfo("se", false) };

            foreach (var item in cultures)
            {
                CultureInfo.CurrentCulture = item;

                MudColor color = new(input);

                //lets darken it
                var darkenColor = color.ColorRgbDarken();

                //use as reference
                var colorFromRGB = new MudColor(color.R, color.G, color.B, color.A);
                var darkenColorFromRGB = colorFromRGB.ColorRgbDarken();

                darkenColor.R.Should().Be(darkenColorFromRGB.R);
                darkenColor.G.Should().Be(darkenColorFromRGB.G);
                darkenColor.B.Should().Be(darkenColorFromRGB.B);

                //MudColor implicitCasted = input;

                //implicitCasted.R.Should().Be(r);
                //implicitCasted.G.Should().Be(g);
                //implicitCasted.B.Should().Be(b);
                //implicitCasted.A.Should().Be(alpha);
            }
        }

        [Test]
        public void FromRGB_Byte()
        {
            MudColor color = new((byte)123, (byte)240, (byte)130, (byte)76);

            color.R.Should().Be(123);
            color.G.Should().Be(240);
            color.B.Should().Be(130);
            color.A.Should().Be(76);
        }

        [Test]
        public void FromRGB_Byte_AndAlphaDouble()
        {
            MudColor color = new((byte)123, (byte)240, (byte)130, 0.8);

            color.R.Should().Be(123);
            color.G.Should().Be(240);
            color.B.Should().Be(130);
            color.A.Should().Be(204);
        }

        [Test]
        public void FromRGB_Int()
        {
            MudColor color = new((int)123, (int)240, (int)130, (int)76);

            color.R.Should().Be(123);
            color.G.Should().Be(240);
            color.B.Should().Be(130);
            color.A.Should().Be(76);
        }

        [Test]
        public void FromRGB_Int_CapsToMaximum()
        {
            MudColor color = new((int)300, (int)2152525, (int)266, (int)25555);

            color.R.Should().Be(255);
            color.G.Should().Be(255);
            color.B.Should().Be(255);
            color.A.Should().Be(255);
        }

        [Test]
        public void FromRGB_Int_EnsureMinimum()
        {
            MudColor color = new((int)-300, (int)-2152525, (int)-266, (int)-25555);

            color.R.Should().Be(0);
            color.G.Should().Be(0);
            color.B.Should().Be(0);
            color.A.Should().Be(0);
        }

        [Test]
        public void FromRGB_Int_WithDoubleAlpha()
        {
            MudColor color = new((int)123, (int)240, (int)130, 0.8);

            color.R.Should().Be(123);
            color.G.Should().Be(240);
            color.B.Should().Be(130);
            color.A.Should().Be(204);
        }

        [Test]
        public void FromRGB_Int_WithDoubleAlpha_CapsToMaximum()
        {
            MudColor color = new((int)300, (int)2152525, (int)266, 2.4);

            color.R.Should().Be(255);
            color.G.Should().Be(255);
            color.B.Should().Be(255);
            color.A.Should().Be(255);
        }

        [Test]
        public void FromRGB_Int_WithDoubleAlpha_EnsureMinimum()
        {
            MudColor color = new((int)-300, (int)-2152525, (int)-266, -0.8);

            color.R.Should().Be(0);
            color.G.Should().Be(0);
            color.B.Should().Be(0);
            color.A.Should().Be(0);
        }

        [Test]
        public void FromHLS_AlphaAsInt()
        {
            MudColor color = new(113.2424, 0.624, 0.2922525, 115);

            color.H.Should().Be(113.0);
            color.S.Should().Be(0.62);
            color.L.Should().Be(0.29);

            color.R.Should().Be(39);
            color.G.Should().Be(120);
            color.B.Should().Be(28);

            color.A.Should().Be(115);
        }

        [Test]
        public void FromHLS_AlphaAsInt_CapsToMaximum()
        {
            MudColor color = new(450.0, 1.4, 1.2, 266);

            color.H.Should().Be(360);
            color.S.Should().Be(1);
            color.L.Should().Be(1);

            color.A.Should().Be(255);
        }

        [Test]
        public void FromHLS_AlphaAsInt_EnsureMinimum()
        {
            MudColor color = new(-450.0, -1.4, -1.2, -266);

            color.H.Should().Be(0);
            color.S.Should().Be(0);
            color.L.Should().Be(0);

            color.A.Should().Be(0);
        }

        [Test]
        public void FromHLS_AlphaAsDouble_CapsToMaximum()
        {
            MudColor color = new(450.0, 1.4, 1.2, 1.2);

            color.H.Should().Be(360);
            color.S.Should().Be(1);
            color.L.Should().Be(1);

            color.A.Should().Be(255);
        }

        [Test]
        public void FromHLS_AlphaAsDouble_EnsureMinimum()
        {
            MudColor color = new(-450.0, -1.4, -1.2, -1.2);

            color.H.Should().Be(0);
            color.S.Should().Be(0);
            color.L.Should().Be(0);

            color.A.Should().Be(0);
        }

        [Test]
        [TestCase(130, 150, 240, 130, 229, 0.79, 0.73)]
        [TestCase(71, 88, 99, 222, 204, 0.16, 0.33)]
        public void TransformHLSFromRGB(byte r, byte g, byte b, byte a, double expectedH, double expectedS, double expectedL)
        {
            MudColor color = new(r, g, b, a);

            color.R.Should().Be(r);
            color.G.Should().Be(g);
            color.B.Should().Be(b);
            color.A.Should().Be(a);

            color.H.Should().Be(expectedH);
            color.S.Should().Be(expectedS);
            color.L.Should().Be(expectedL);
        }

        [Test]
        public void SetH()
        {
            MudColor color = new(120, 0.15, 0.25, 255);

            color.SetH(-12).H.Should().Be(0);
            color.SetH(0).H.Should().Be(0);
            color.SetH(120).H.Should().Be(120);
            color.SetH(350).H.Should().Be(350);
            color.SetH(370).H.Should().Be(360);
        }

        [Test]
        public void SetS()
        {
            MudColor color = new(120, 0.15, 0.25, 255);

            color.SetS(-0.1).S.Should().Be(0);
            color.SetS(0).S.Should().Be(0);
            color.SetS(0.37).S.Should().Be(0.37);
            color.SetS(0.67).S.Should().Be(0.67);
            color.SetS(1.2).S.Should().Be(1);
        }

        [Test]
        public void SetL()
        {
            MudColor color = new(120, 0.15, 0.25, 255);

            color.SetL(-0.1).L.Should().Be(0);
            color.SetL(0).L.Should().Be(0);
            color.SetL(0.37).L.Should().Be(0.37);
            color.SetL(0.67).L.Should().Be(0.67);
            color.SetL(1.2).L.Should().Be(1);
        }

        [Test]
        public void SetR()
        {
            MudColor color = new((byte)25, (byte)50, (byte)70, (byte)255);

            color.SetR(-4).R.Should().Be(0);
            color.SetR(0).R.Should().Be(0);
            color.SetR(20).R.Should().Be(20);
            color.SetR(250).R.Should().Be(250);
            color.SetR(256).R.Should().Be(255);
        }

        [Test]
        public void SetG()
        {
            MudColor color = new((byte)25, (byte)50, (byte)70, (byte)255);

            color.SetG(-4).G.Should().Be(0);
            color.SetG(0).G.Should().Be(0);
            color.SetG(20).G.Should().Be(20);
            color.SetG(250).G.Should().Be(250);
            color.SetG(256).G.Should().Be(255);
        }

        [Test]
        public void SetB()
        {
            MudColor color = new((byte)25, (byte)50, (byte)70, (byte)255);

            color.SetB(-4).B.Should().Be(0);
            color.SetB(0).B.Should().Be(0);
            color.SetB(20).B.Should().Be(20);
            color.SetB(250).B.Should().Be(250);
            color.SetB(256).B.Should().Be(255);
        }

        [Test]
        public void SetAlpha_Byte()
        {
            MudColor color = new((byte)25, (byte)50, (byte)70, (byte)170);

            color.SetAlpha(-4).A.Should().Be(0);
            color.SetAlpha(0).A.Should().Be(0);
            color.SetAlpha(20).A.Should().Be(20);
            color.SetAlpha(250).A.Should().Be(250);
            color.SetAlpha(256).A.Should().Be(255);
        }

        [Test]
        public void SetAlpha_Double()
        {
            MudColor color = new((byte)25, (byte)50, (byte)70, (byte)170);

            color.SetAlpha(-0.4).A.Should().Be(0);
            color.SetAlpha(0.0).A.Should().Be(0);
            color.SetAlpha(0.4).A.Should().Be(102);
            color.SetAlpha(0.8).A.Should().Be(204);
            color.SetAlpha(1.2).A.Should().Be(255);
        }

        [Test]
        public void ChangeLightness()
        {
            MudColor color = new(140.0, 0.2, 0.4, (byte)170);

            color.ChangeLightness(-0.4).L.Should().Be(0.0);
            color.ChangeLightness(-0.5).L.Should().Be(0.0);
            color.ChangeLightness(+0.5).L.Should().Be(0.9);
            color.ChangeLightness(+0.6).L.Should().Be(1.0);
            color.ChangeLightness(+0.7).L.Should().Be(1.0);
            color.ChangeLightness(+2.7).L.Should().Be(1.0);
        }

        [Test]
        public void ColorLighten()
        {
            MudColor color = new(140.0, 0.2, 0.4, (byte)170);

            color.ChangeLightness(0.4).L.Should().Be(0.8);
            color.ChangeLightness(0.5).L.Should().Be(0.9);
            color.ChangeLightness(0.6).L.Should().Be(1.0);
            color.ChangeLightness(0.7).L.Should().Be(1.0);
            color.ChangeLightness(-0.4).L.Should().Be(0.0);
            color.ChangeLightness(-0.5).L.Should().Be(0.0);
        }

        [Test]
        public void ColorDarken()
        {
            MudColor color = new(140.0, 0.2, 0.4, (byte)170);

            color.ColorDarken(0.4).L.Should().Be(0.0);
            color.ColorDarken(0.5).L.Should().Be(0.0);
            color.ColorDarken(0.2).L.Should().Be(0.2);
            color.ColorDarken(-0.6).L.Should().Be(1.0);
            color.ColorDarken(-0.7).L.Should().Be(1.0);
        }

        [Test]
        public void ColorRgbLighten()
        {
            MudColor color = new(140.0, 0.2, 0.5, (byte)170);
            color.ColorRgbLighten().L.Should().Be(0.57);
        }

        [Test]
        public void ColorRgbDarken()
        {
            MudColor color = new(140.0, 0.2, 0.5, (byte)170);
            color.ColorRgbDarken().L.Should().Be(0.42);
        }

        [Test]
        [TestCase(130, 150, 240, 170, "#8296f0aa")]
        [TestCase(71, 88, 99, 204, "#475863cc")]
        public void ValueAndExplicitCast(byte r, byte g, byte b, byte a, string expectedValue)
        {
            MudColor color = new(r, g, b, a);

            color.Value.ToLowerInvariant().Should().Be(expectedValue);
            color.ToString(MudColorOutputFormats.HexA).ToLowerInvariant().Should().Be(expectedValue);
            ((string)color).ToLowerInvariant().Should().Be(expectedValue);
        }

        [Test]
        [TestCase(130, 150, 240, 255, "rgb(130,150,240)")]
        [TestCase(71, 88, 99, 255, "rgb(71,88,99)")]
        public void ToRGB(byte r, byte g, byte b, byte a, string expectedValue)
        {
            var cultures = new[] { new CultureInfo("en", false), new CultureInfo("se", false) };

            foreach (var item in cultures)
            {
                CultureInfo.CurrentCulture = item;

                MudColor color = new(r, g, b, a);

                color.ToString(MudColorOutputFormats.RGB).Should().Be(expectedValue);
            }
        }

        [Test]
        [TestCase(130, 150, 240, 255, "rgba(130,150,240,1)")]
        [TestCase(71, 88, 99, 0, "rgba(71,88,99,0)")]
        [TestCase(71, 88, 99, 204, "rgba(71,88,99,0.8)")]
        public void ToRGBA(byte r, byte g, byte b, byte a, string expectedValue)
        {
            var cultures = new[] { new CultureInfo("en", false), new CultureInfo("se", false) };

            foreach (var item in cultures)
            {
                CultureInfo.CurrentCulture = item;

                MudColor color = new(r, g, b, a);

                color.ToString(MudColorOutputFormats.RGBA).Should().Be(expectedValue);
            }
        }

        [Test]
        [TestCase(130, 150, 240, 255, "130,150,240")]
        [TestCase(71, 88, 99, 255, "71,88,99")]
        public void ToColorRgbElements(byte r, byte g, byte b, byte a, string expectedValue)
        {
            var cultures = new[] { new CultureInfo("en", false), new CultureInfo("se", false) };

            foreach (var item in cultures)
            {
                CultureInfo.CurrentCulture = item;

                MudColor color = new(r, g, b, a);

                color.ToString(MudColorOutputFormats.ColorElements).Should().Be(expectedValue);
            }
        }

        [Test]
        [TestCase(130, 150, 240, 170, "#8296f0")]
        [TestCase(71, 88, 99, 204, "#475863")]
        public void ToHex(byte r, byte g, byte b, byte a, string expectedValue)
        {
            var cultures = new[] { new CultureInfo("en", false), new CultureInfo("se", false) };

            foreach (var item in cultures)
            {
                CultureInfo.CurrentCulture = item;

                MudColor color = new(r, g, b, a);

                color.ToString(MudColorOutputFormats.Hex).Should().Be(expectedValue);
            }
        }

#pragma warning disable CS1718 // Comparison made to same variable

        [Test]
        public void Equals_SameType()
        {
            MudColor color1 = new(10, 20, 50, 255);
            MudColor color2 = new(10, 20, 50, 255);
            MudColor color3 = null;
            MudColor color4 = null;

            (color1 == color1).Should().BeTrue();
            (color2 == color2).Should().BeTrue();
            (color1 == color2).Should().BeTrue();
            (color2 == color1).Should().BeTrue();
            (color3 == color4).Should().BeTrue();

            color1.Equals(color1).Should().BeTrue();
            color2.Equals(color2).Should().BeTrue();
            color1.Equals(color2).Should().BeTrue();
            color2.Equals(color1).Should().BeTrue();
            Equals(color3, color4).Should().BeTrue();
        }

        [Test]
        public void NotEquals_SameType()
        {
            MudColor color1 = new(10, 20, 50, 255);
            MudColor color2 = new(10, 20, 50, 10);
            MudColor color3 = null;

            (color1 != color2).Should().BeTrue();
            (color2 != color1).Should().BeTrue();
            (color2 != color3).Should().BeTrue();
            (color3 != color2).Should().BeTrue();

            color1.Equals(color2).Should().BeFalse();
            color2.Equals(color1).Should().BeFalse();
            color2.Equals(color3).Should().BeFalse();
            Equals(color3, color2).Should().BeFalse();
            Equals(color2, color3).Should().BeFalse();
        }

#pragma warning restore CS1718 // Comparison made to same variable

        [Test]
        public void Equals_null()
        {
            MudColor color1 = new(10, 20, 50, 255);
            (color1 == null).Should().BeFalse();
            color1.Equals(null as MudColor).Should().BeFalse();

            MudColor color2 = null;

            (color2 == null).Should().BeTrue();
            (null == color2).Should().BeTrue();
        }

        [Test]
        public void Equals_DifferentObjectType()
        {
            MudColor color1 = new(10, 20, 50, 255);
            color1.Equals(124).Should().BeFalse();
        }

        [Test]
        [TestCase(130, 150, 240, 255, 775)]
        [TestCase(71, 88, 99, 100, 358)]
        public void GetHashCode(byte r, byte g, byte b, byte a, int expectedValue)
        {
            MudColor color = new(r, g, b, a);

            color.GetHashCode().Should().Be(expectedValue);
        }

        [Test]
        public void HLSChanged_HChanged()
        {
            MudColor first = new(120, 0.5, 0.4, 1);
            MudColor second = new(121, 0.5, 0.4, 1);

            first.HslChanged(second).Should().BeTrue();
            second.HslChanged(first).Should().BeTrue();

            first.HslChanged(first).Should().BeFalse();
            second.HslChanged(second).Should().BeFalse();
        }

        [Test]
        public void HLSChanged_SChanged()
        {
            MudColor first = new(120, 0.5, 0.4, 1);
            MudColor second = new(120, 0.51, 0.4, 1);

            first.HslChanged(second).Should().BeTrue();
            second.HslChanged(first).Should().BeTrue();

            first.HslChanged(first).Should().BeFalse();
            second.HslChanged(second).Should().BeFalse();
        }

        [Test]
        public void HLSChanged_LChanged()
        {
            MudColor first = new(120, 0.5, 0.4, 1);
            MudColor second = new(120, 0.5, 0.41, 1);

            first.HslChanged(second).Should().BeTrue();
            second.HslChanged(first).Should().BeTrue();

            first.HslChanged(first).Should().BeFalse();
            second.HslChanged(second).Should().BeFalse();
        }

        [Test]
        [TestCase("en-us")]
        [TestCase("de-DE")]
        [TestCase("he-IL")]
        [TestCase("ar-ER")]
        public void CheckPaletteInDifferentCultures(string cultureString)
        {
            var culture = new CultureInfo(cultureString, false);

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            Palette palette = new PaletteLight();

            palette.Should().NotBeNull();
        }

        [Test]
        [TestCase(0x000000FFu)]//Black
        [TestCase(0xFF0000FFu)]//Red
        [TestCase(0x00FF00FFu)]//Green
        [TestCase(0x0000FFFFu)]//Blue
        public void UInt32(uint rgba)
        {
            MudColor mudColor = new(rgba);

            mudColor.Value.Should().BeEquivalentTo($"#{rgba:X8}");
            ((uint)mudColor).Should().Be(rgba);
            mudColor.UInt32.Should().Be(rgba);
        }

        [Test]
        public void UInt32_CheckAgainstBinaryPrimitive()
        {
            const byte R = 255, G = 128, B = 64, A = 192;
            var mudColor = new MudColor(R, G, B, A);
            var expectedUint = BinaryPrimitives.ReadUInt32BigEndian([mudColor.R, mudColor.G, mudColor.B, mudColor.A]);

            var actualUint = (uint)mudColor;

            actualUint.Should().Be(expectedUint);
            mudColor.UInt32.Should().Be(mudColor.UInt32);
        }
    }
}
