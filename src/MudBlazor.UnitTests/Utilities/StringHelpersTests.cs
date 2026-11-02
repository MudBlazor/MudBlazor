using System.Globalization;
using AwesomeAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities;

[TestFixture]
public class StringHelpersTests
{
    [Test]
    public void ToS_WithoutFormat_RoundsToFourDecimalsUsingInvariantCulture()
    {
        StringHelpers.ToS(12.34567).Should().Be("12.3457");
        StringHelpers.ToS(1000.5).Should().Be("1000.5");
    }

    [Test]
    public void ToS_WithFormat_AppliesFormatAfterRounding()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        var currentUICulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

            StringHelpers.ToS(9.996, "F2").Should().Be("10.00");
        }
        finally
        {
            CultureInfo.CurrentCulture = currentCulture;
            CultureInfo.CurrentUICulture = currentUICulture;
        }
    }

    [Test]
    public void ToStr_UsesSameRoundingAsToS()
    {
        const double Value = 1.23456;

        Value.ToStr().Should().Be(StringHelpers.ToS(Value));
    }

    [Test]
    public void ToS_WithoutFormat_IsIndependentOfCurrentCulture()
    {
        // The no-format path is the one used to build SVG paths; it must emit '.' regardless of culture.
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");

            StringHelpers.ToS(1234.5).Should().Be("1234.5");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

}
