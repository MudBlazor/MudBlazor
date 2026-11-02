using System.Globalization;
using AwesomeAssertions;
using MudBlazor.Extensions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

[TestFixture]
public class DoubleExtensionsTests
{
    [Test]
    public void ToInvariantString_UsesInvariantCultureFormatting()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");

            // Act
            var result = 1234.5.ToInvariantString();

            // Assert
            result.Should().Be("1234.5");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }
}
