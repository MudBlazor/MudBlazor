// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using FluentAssertions;
using MudBlazor.UnitTests.Dummy;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class MudColorTests
    {
        [Test]
        public void MudColor_STJ_SourceGen_Serialization()
        {
            var originalMudColor = new MudColor("#f6f9fb");

            var mudColorType = typeof(MudColor);
            var context = MudColorSerializerContext.Default;

            var jsonString = System.Text.Json.JsonSerializer.Serialize(originalMudColor, mudColorType, context);
            var deserializeMudColor = System.Text.Json.JsonSerializer.Deserialize(jsonString, mudColorType, context);

            jsonString.Should().Be("{\"R\":246,\"G\":249,\"B\":251,\"A\":255}");
            deserializeMudColor.Should().Be(originalMudColor);
        }

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
                var colorFromRgb = new MudColor(color.R, color.G, color.B, color.A);
                var darkenColorFromRgb = colorFromRgb.ColorRgbDarken();

                darkenColor.R.Should().Be(darkenColorFromRgb.R);
                darkenColor.G.Should().Be(darkenColorFromRgb.G);
                darkenColor.B.Should().Be(darkenColorFromRgb.B);
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
        public void TransformHlsFromRgb(byte r, byte g, byte b, byte a, double expectedH, double expectedS, double expectedL)
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


        [Test]
        public void ToStringFormat()
        {
            // Arrange
            var color = new MudColor(130, 150, 240, 170);

            // Act
            var rgb = color.ToString(MudColorOutputFormats.RGB);
            var rgba = color.ToString(MudColorOutputFormats.RGBA);
            var hex = color.ToString(MudColorOutputFormats.Hex);
            var hexA = color.ToString(MudColorOutputFormats.HexA);
            var colorElements = color.ToString(MudColorOutputFormats.ColorElements);
            var unknown = color.ToString((MudColorOutputFormats)9999);

            // Assert
            rgb.Should().Be("rgb(130,150,240)");
            rgba.Should().Be("rgba(130,150,240,0.6666666666666666)");
            hex.Should().Be("#8296f0");
            hexA.Should().Be("#8296f0aa");
            colorElements.Should().Be("130,150,240");
            unknown.Should().Be("#8296f0aa");
        }

        [Test]
        public void ToStringFormatProvider()
        {
            // Arrange
            var color = new MudColor(130, 150, 240, 170);

            // Act
            var normal = color.ToString(null, null);
            var rgb = $"{color:RGB}";
            var rgba = $"{color:RGBA}";
            var hex = $"{color:HEX}";
            var hexA = $"{color:HEXA}";
            var colorElements = $"{color:COLORELEMENTS}";
            var unknown = $"{color:F2}";

            // Assert
            normal.Should().Be("rgba(130,150,240,0.6666666666666666)");
            rgb.Should().Be("rgb(130,150,240)");
            rgba.Should().Be("rgba(130,150,240,0.6666666666666666)");
            hex.Should().Be("#8296f0");
            hexA.Should().Be("#8296f0aa");
            colorElements.Should().Be("130,150,240");
            unknown.Should().Be("#8296f0aa");
        }

        [Test]
        public void GenerateMultiGradientPalette_ShouldThrowArgumentException_WhenColorsCollectionContainsOnlyOneColor()
        {
            // Arrange
            IReadOnlyList<MudColor> colors = ["#FF0000"];

            // Act
            var act = () =>
            {
                _ = MudColor.GenerateMultiGradientPalette(colors).ToList();
            };

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("The colors collection must contain at least two colors. (Parameter 'colors')");
        }

        [Test]
        [TestCaseSource(nameof(_multiGradientTestCases))]
        public void GenerateMultiGradientPalette_ShouldGenerateCorrectGradient(MudColor[] colors, int numberOfColors, MudColor[] expectedColors)
        {
            // Arrange & Act
            var multiGradientPalette = MudColor.GenerateMultiGradientPalette(colors, numberOfColors).ToList();

            // Assert
            multiGradientPalette.Should().HaveCount(numberOfColors);
            multiGradientPalette.Should().Equal(expectedColors);
        }

        [Test]
        [TestCaseSource(nameof(_gradientTestCases))]
        public void GenerateGradientPalette_ShouldGenerateCorrectGradient(MudColor startColor, MudColor endColor, int numberOfColors, MudColor[] expectedColors)
        {
            // Arrange & Act
            var gradientPalette = MudColor.GenerateGradientPalette(startColor, endColor, numberOfColors).ToList();

            // Assert
            gradientPalette.Should().HaveCount(numberOfColors);
            gradientPalette.Should().Equal(expectedColors);
        }

        [Test]
        public void GenerateAnalogousPalette_ShouldGenerateCorrectAnalogousColors()
        {
            // Arrange
            var baseColor = new MudColor("#FF0000FF"); // Red
            IReadOnlyList<MudColor> expectedColors = ["#FF0000FF", "#FFFF00FF", "#00FF00FF", "#00FFFFFF", "#0000FFFF"];

            // Act
            var analogousPalette = MudColor.GenerateAnalogousPalette(baseColor, angle: 60).ToList();

            // Assert
            analogousPalette.Should().HaveCount(5);
            analogousPalette.Should().Equal(expectedColors);
        }

        [Test]
        public void GenerateTintShadePalette_ShouldGenerateCorrectTintsAndShades()
        {
            // Arrange
            var baseColor = new MudColor("#808080FF"); // Gray

            // Only tints
            IReadOnlyList<MudColor> expectedTints = ["#808080FF", "#999999FF", "#B3B3B3FF", "#CCCCCCFF", "#E6E6E6FF"];

            // Only shades
            IReadOnlyList<MudColor> expectedShades = ["#808080FF", "#666666FF", "#4D4D4DFF", "#333333FF", "#1A1A1AFF"];

            // Both tints and shades (odd number of colors)
            IReadOnlyList<MudColor> expectedBothOdd = ["#CCCCCCFF", "#B3B3B3FF", "#999999FF", "#808080FF", "#666666FF", "#4D4D4DFF", "#333333FF"];

            // Both tints and shades (even number of colors)
            IReadOnlyList<MudColor> expectedBothEven = ["#CCCCCCFF", "#B3B3B3FF", "#999999FF", "#808080FF", "#666666FF", "#4D4D4DFF"];

            // Act
            var tintsPalette = MudColor.GenerateTintShadePalette(baseColor, tintStep: 0.1, shadeStep: 0).ToList();
            var shadesPalette = MudColor.GenerateTintShadePalette(baseColor, tintStep: 0, shadeStep: 0.1).ToList();
            var bothPaletteOdd = MudColor.GenerateTintShadePalette(baseColor, 7, tintStep: 0.1, shadeStep: 0.1).ToList();
            var bothPaletteEven = MudColor.GenerateTintShadePalette(baseColor, 6, tintStep: 0.1, shadeStep: 0.1).ToList();

            // Assert
            tintsPalette.Should().HaveCount(5);
            tintsPalette.Should().Equal(expectedTints);

            shadesPalette.Should().HaveCount(5);
            shadesPalette.Should().Equal(expectedShades);

            bothPaletteOdd.Should().HaveCount(7);
            bothPaletteOdd.Should().Equal(expectedBothOdd);

            bothPaletteEven.Should().HaveCount(6);
            bothPaletteEven.Should().Equal(expectedBothEven);
        }

        [Test]
        [TestCaseSource(nameof(_lerpTestCases))]
        public void Lerp_ShouldInterpolateCorrectly(MudColor colorStart, MudColor colorEnd, float t, MudColor expectedColor)
        {
            // Arrange & Act
            var result = MudColor.Lerp(colorStart, colorEnd, t);

            // Assert
            result.Should().Be(expectedColor);
        }

#pragma warning disable CS1718 // Comparison made to same variable

        [Test]
        public void Equals_SameType()
        {
            // Arrange
            MudColor color1 = new(10, 20, 50, 255);
            MudColor color2 = new(10, 20, 50, 255);
            MudColor color3 = null;
            MudColor color4 = null;

            // Act & Assert
            // Self-comparison
            color1.Equals(color1).Should().BeTrue();
            color2.Equals(color2).Should().BeTrue();

            // Comparison with another instance with the same values
            color1.Equals(color2).Should().BeTrue();
            color2.Equals(color1).Should().BeTrue();

            // Null comparisons
            (color3 == color4).Should().BeTrue();
            Equals(color3, color4).Should().BeTrue();

            // Operator overloads
            (color1 == color2).Should().BeTrue();
            (color2 == color1).Should().BeTrue();
            (color1 != color3).Should().BeTrue();
            (color3 != color1).Should().BeTrue();
        }

        [Test]
        public void NotEquals_SameType()
        {
            // Arrange
            MudColor color1 = new(10, 20, 50, 255);
            MudColor color2 = new(10, 20, 50, 10);
            MudColor color3 = null;

            // Act
            var result1 = color1 != color2;
            var result2 = color2 != color1;
            var result3 = color2 != color3;
            var result4 = color3 != color2;

            var equalsResult1 = color1.Equals(color2);
            var equalsResult2 = color2.Equals(color1);
            var equalsResult3 = color2.Equals(color3);
            var equalsResult4 = Equals(color3, color2);
            var equalsResult5 = Equals(color2, color3);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeTrue();
            result4.Should().BeTrue();

            equalsResult1.Should().BeFalse();
            equalsResult2.Should().BeFalse();
            equalsResult3.Should().BeFalse();
            equalsResult4.Should().BeFalse();
            equalsResult5.Should().BeFalse();
        }

#pragma warning restore CS1718 // Comparison made to same variable

        [Test]
        public void Equals_Null()
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
            // Arrange
            MudColor color1 = new(10, 20, 50, 255);
            object obj = 124;

            // Act
            var equals = color1.Equals(obj);

            // Assert
            equals.Should().BeFalse();
        }

        [Test]
        public void Equals_SameRgba_DifferentHsl()
        {
            // Arrange
            var color1 = new MudColor(245, 0.34, 0.95, 1);
            var color2 = new MudColor(245, 0.35, 0.95, 1);

            // Act
            var equals = color1.Equals(color2);
            var hslEquals = color1.HslEquals(color2);
            var rgbaEquals = color1.RgbaEquals(color2);

            // Assert
            equals.Should().BeFalse();
            hslEquals.Should().BeFalse();
            rgbaEquals.Should().BeTrue();
        }

        [Test]
        public void HslEquals_Null_Test()
        {
            // Arrange
            MudColor color = new(120, 0.5, 0.4, 1);

            // Act
            var equals = color.HslEquals(null);

            // Assert
            equals.Should().BeFalse();
        }

        [Test]
        [TestCase(120, 0.5, 0.4, 1, 121, 0.5, 0.4, 1, false)] // Hue differs
        [TestCase(120, 0.5, 0.4, 1, 120, 0.51, 0.4, 1, false)] // Saturation differs
        [TestCase(120, 0.5, 0.4, 1, 120, 0.5, 0.41, 1, false)] // Lightness differs
        public void HslEquals_Test(double h1, double s1, double l1, double a1, double h2, double s2, double l2, double a2, bool expected)
        {
            // Arrange
            MudColor first = new(h1, s1, l1, a1);
            MudColor second = new(h2, s2, l2, a2);

            // Act
            var result = first.HslEquals(second);

            // Assert
            result.Should().Be(expected);
        }

        [Test]
        public void RgbaEquals_Null_Test()
        {
            // Arrange
            MudColor color = new(10, 20, 30, 255);

            // Act
            var equals = color.RgbaEquals(null);

            // Assert
            equals.Should().BeFalse();
        }

        [Test]
        [TestCase(10, 20, 30, 255, 10, 20, 30, 254, false)] // Alpha differs
        [TestCase(10, 20, 30, 255, 10, 20, 31, 255, false)] // Blue differs
        [TestCase(10, 20, 30, 255, 10, 21, 30, 255, false)] // Green differs
        [TestCase(10, 20, 30, 255, 11, 20, 30, 255, false)] // Red differs
        [TestCase(10, 20, 30, 255, 10, 20, 30, 255, true)]  // All equal
        public void RgbaEquals_Test(byte r1, byte g1, byte b1, byte a1, byte r2, byte g2, byte b2, byte a2, bool expected)
        {
            // Arrange
            MudColor first = new(r1, g1, b1, a1);
            MudColor second = new(r2, g2, b2, a2);

            // Act
            var result = first.RgbaEquals(second);

            // Assert
            result.Should().Be(expected);
        }

        [Test]
        public void GetHashCode_SameRgba()
        {
            // Arrange
            var color1 = new MudColor(130, 150, 240, 255);
            var color2 = new MudColor(130, 150, 240, 255);

            // Act
            var areEqualGetHashCode = color1.GetHashCode() == color2.GetHashCode();

            // Assert
            areEqualGetHashCode.Should().BeTrue();
        }

        [Test]
        public void GetHasCode_DifferentRgba()
        {
            // Arrange
            var color1 = new MudColor(130, 150, 240, 255);
            var color2 = new MudColor(131, 150, 240, 255);

            // Act
            var areEqualGetHashCode = color1.GetHashCode() == color2.GetHashCode();

            // Assert
            areEqualGetHashCode.Should().BeFalse();
        }

        [Test]
        public void GetHasCode_SameRgba_DifferentHsl()
        {
            // Arrange
            var color1 = new MudColor(245, 0.34, 0.95, 1);
            var color2 = new MudColor(245, 0.35, 0.95, 1);

            // Act
            var getHashCodeEquals = color1.GetHashCode() == color2.GetHashCode();

            // Assert
            getHashCodeEquals.Should().BeFalse();
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

        [Test]
        [TestCase("rgba(130,150,240,0.52)", 130, 150, 240, 132)]
        [TestCase("rgb(71,88,99)", 71, 88, 99, 255)]
        [TestCase("#8296f0ff", 130, 150, 240, 255)]
        [TestCase("#475863", 71, 88, 99, 255)]
        public void ParseTest(string value, byte r, byte g, byte b, byte a)
        {
            // Arrange
            var expected = new MudColor(r, g, b, a);

            // Act
            var result = MudColor.Parse(value);

            // Assert
            result.Should().Be(expected);
        }

        [Test]
        [TestCase("rgba(130,150,240,0.52,50)")]
        [TestCase("rgb(71,88,99,63)")]
        [TestCase("#8296f0ffff")]
        public void ParseIncorrectFormatTest(string value)
        {
            // Act & Arrange
            var act = () => MudColor.Parse(value);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        [TestCase("rgba(130,150,240,0.52)", 130, 150, 240, 132)]
        [TestCase("rgb(71,88,99)", 71, 88, 99, 255)]
        [TestCase("#8296f0ff", 130, 150, 240, 255)]
        [TestCase("#475863", 71, 88, 99, 255)]
        public void TryParseTest(string value, byte r, byte g, byte b, byte a)
        {
            // Arrange
            var expected = new MudColor(r, g, b, a);

            // Act
            var success = MudColor.TryParse(value, out var result);

            // Assert
            success.Should().BeTrue();
            result.Should().Be(expected);
        }

        [Test]
        [TestCase("rgba(130,150,240,0.52,50)")]
        [TestCase("rgb(71,88,99, 63)")]
        [TestCase("#8296f0ffff")]
        [TestCase("")]
        [TestCase(null)]
        public void TryParseIncorrectFormatTest(string value)
        {
            // Act
            var success = MudColor.TryParse(value, out var result);

            // Assert
            success.Should().BeFalse();
            result.Should().BeNull();
        }

        [Test]
        public void DeconstructTest()
        {
            // Arrange
            var mudColor = new MudColor(255, 128, 64, 192);

            // Act
            var (r, g, b, a) = mudColor;
            var (r2, g2, b2) = mudColor;

            // Assert
            r.Should().Be(255);
            g.Should().Be(128);
            b.Should().Be(64);
            a.Should().Be(192);
            r2.Should().Be(255);
            g2.Should().Be(128);
            b2.Should().Be(64);
        }

        [Test]
        public void ExplicitMudColorToStringCastTest()
        {
            // Arrange
            var mudColor1 = new MudColor(71, 88, 99, 1);

            // Act
            var result1 = (string)mudColor1;
            var result2 = (string)(MudColor)null;

            // Assert
            result1.Should().Be("#47586301");
            result2.Should().Be(string.Empty);
        }

        private static readonly object[] _multiGradientTestCases =
        [
            // Should return the same colors when list colors count is same as number of colors
            new object[] { new MudColor[] { "#FF4500FF", "#FFD700FF", "#32CD32FF", "#1E90FFFF", "#8A2BE2FF" }, 5, new MudColor[] { "#FF4500FF", "#FFD700FF", "#32CD32FF", "#1E90FFFF", "#8A2BE2FF" } },
            // Should act as "MudColor.GenerateGradientPalette" when there are only two colors (end and start), testing with odd number of colors
            new object[] { new MudColor[] { "#FF0000FF", "#0000FFFF" }, 5, new MudColor[] { "#FF0000FF", "#BF003FFF", "#7F007FFF", "#3F00BFFF", "#0000FFFF" } },
            // Should act as "MudColor.GenerateGradientPalette" when there are only two colors (end and start), testing with even number of colors
            new object[] { new MudColor[] { "#FF0000FF", "#0000FFFF" }, 6, new MudColor[] { "#FF0000FF", "#CC0033FF", "#990066FF", "#650099FF", "#3200CCFF", "#0000FFFF" } },
            // Should return first color, then lerp between first and middle one, then middle one, then lerp between middle and last one, and last one
            new object[] { new MudColor[] { "#FF0000FF", "#7F007FFF", "#0000FFFF" }, 5, new MudColor[] { "#FF0000FF", "#BF003FFF", "#7F007FFF", "#3F00BFFF", "#0000FFFF" } },
            new object[] { new MudColor[] { "#FF4500FF", "#32CD32FF", "#8A2BE2FF" }, 5, new MudColor[] { "#FF4500FF", "#988919FF", "#32CD32FF", "#5E7C8AFF", "#8A2BE2FF" } },
            new object[] { new MudColor[] { "#FF4500FF", "#32CD32FF", "#8A2BE2FF" }, 6, new MudColor[] { "#FF4500FF", "BA7210FD", "769F21FF", "32CD32FF", "5E7C8AFF", "#8A2BE2FF" } }
        ];

        private static readonly object[] _gradientTestCases =
        [
            // Should just lerp between two colors when numbers of colors is 1
            new object[] { new MudColor("#FF0000FF"), new MudColor("#0000FFFF"), 1, new MudColor[] { "#7F007FFF" } },
            // Should return start and end colors when numbers of colors is 2
            new object[] { new MudColor("#FF0000FF"), new MudColor("#0000FFFF"), 2, new MudColor[] { "#FF0000FF", "#0000FFFF" }},
            // Should return start, then evenly lerped colors, and end color when numbers of colors are more than 3, testing with odd number of colors
            new object[] { new MudColor("#FF0000FF"), new MudColor("#0000FFFF"), 5, new MudColor[] { "#FF0000FF", "#BF003FFF", "#7F007FFF", "#3F00BFFF", "#0000FFFF" }},
            // Should return start, then evenly lerped colors, testing with even number of colors
            new object[] { new MudColor("#FF0000FF"), new MudColor("#0000FFFF"), 6, new MudColor[] { "#FF0000FF", "#CC0033FF", "#990066FF", "#650099FF", "#3200CCFF", "#0000FFFF" }},
        ];

        private static readonly object[] _lerpTestCases =
        [
            // Tested expected also with https://www.colourblender.io/
            new object[] { new MudColor(255, 0, 0, 255), new MudColor(0, 0, 255, 255), 0.0f, new MudColor(255, 0, 0, 255) }, // t = 0, should return start color
            new object[] { new MudColor(255, 0, 0, 255), new MudColor(0, 0, 255, 255), 1.0f, new MudColor(0, 0, 255, 255) }, // t = 1, should return end color
            new object[] { new MudColor(255, 0, 0, 255), new MudColor(0, 0, 255, 255), 0.5f, new MudColor(127, 0, 127, 255) }, // t = 0.5, should interpolate between colors
            new object[] { new MudColor(255, 0, 0, 128), new MudColor(0, 0, 255, 64), 0.5f, new MudColor(127, 0, 127, 95) }, // t = 0.5, with alpha interpolation
            new object[] { new MudColor(0, 64, 128, 0), new MudColor(254, 0, 203, 0), 0.3f, new MudColor(76, 44, 150, 0) },
            new object[] { new MudColor(255, 255, 255, 0), new MudColor(254, 0, 203, 0), 0.15f, new MudColor(254, 216, 247, 0) }
        ];
    }
}
