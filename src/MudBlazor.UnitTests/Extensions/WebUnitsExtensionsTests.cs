// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using AwesomeAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{
    [TestFixture]
    public class WebUnitsExtensionsTests
    {
        [Test]
        public void All_ToPxMethods_Work()
        {
            0.0.ToPx().Should().Be("0px");
            3.3333.ToPx().Should().Be("3.33px");
            (-3.3333).ToPx().Should().Be("-3.33px");
            ((double?)3.3333).ToPx().Should().Be("3.33px");
            ((double?)null).ToPx().Should().Be(string.Empty);

            0.ToPx().Should().Be("0px");
            3.ToPx().Should().Be("3px");
            (-3).ToPx().Should().Be("-3px");
            ((int?)3).ToPx().Should().Be("3px");
            ((int?)null).ToPx().Should().Be(string.Empty);

            0L.ToPx().Should().Be("0px");
            3L.ToPx().Should().Be("3px");
            (-3L).ToPx().Should().Be("-3px");
            ((long?)3L).ToPx().Should().Be("3px");
            ((long?)null).ToPx().Should().Be(string.Empty);
        }

        [Test]
        public void All_ToPercentMethods_Work()
        {
            0.0.ToPercent().Should().Be("0%");
            3.3333.ToPercent().Should().Be("3.33%");
            (-3.3333).ToPercent().Should().Be("-3.33%");
            ((double?)3.3333).ToPercent().Should().Be("3.33%");
            ((double?)null).ToPercent().Should().Be(string.Empty);

            0.ToPercent().Should().Be("0%");
            3.ToPercent().Should().Be("3%");
            (-3).ToPercent().Should().Be("-3%");
            ((int?)3).ToPercent().Should().Be("3%");
            ((int?)null).ToPercent().Should().Be(string.Empty);

            0L.ToPercent().Should().Be("0%");
            3L.ToPercent().Should().Be("3%");
            (-3L).ToPercent().Should().Be("-3%");
            ((long?)3L).ToPercent().Should().Be("3%");
            ((long?)null).ToPercent().Should().Be(string.Empty);
        }

        // The "0.##" format trims trailing zeros and rounds anything past the second decimal.
        [TestCase(3.0, "3px")]
        [TestCase(3.5, "3.5px")]
        [TestCase(3.05, "3.05px")]
        [TestCase(1234.5678, "1234.57px")]
        public void ToPx_Double_RoundsToTwoDecimalsAndTrimsZeros(double value, string expected)
        {
            value.ToPx().Should().Be(expected);
        }

        [TestCase(3.0, "3%")]
        [TestCase(3.5, "3.5%")]
        [TestCase(3.05, "3.05%")]
        [TestCase(1234.5678, "1234.57%")]
        public void ToPercent_Double_RoundsToTwoDecimalsAndTrimsZeros(double value, string expected)
        {
            value.ToPercent().Should().Be(expected);
        }

        // de-DE uses ',' as the decimal separator; the output must stay invariant ('.').
        [Test]
        public void ToPx_Double_UsesInvariantCulture()
        {
            var originalCulture = CultureInfo.CurrentCulture;
            var originalUiCulture = CultureInfo.CurrentUICulture;

            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");

                3.3333.ToPx().Should().Be("3.33px");
                ((double?)3.3333).ToPx().Should().Be("3.33px");
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
                CultureInfo.CurrentUICulture = originalUiCulture;
            }
        }

        [Test]
        public void ToPercent_Double_UsesInvariantCulture()
        {
            var originalCulture = CultureInfo.CurrentCulture;
            var originalUiCulture = CultureInfo.CurrentUICulture;

            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");

                3.3333.ToPercent().Should().Be("3.33%");
                ((double?)3.3333).ToPercent().Should().Be("3.33%");
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
                CultureInfo.CurrentUICulture = originalUiCulture;
            }
        }
    }
}
