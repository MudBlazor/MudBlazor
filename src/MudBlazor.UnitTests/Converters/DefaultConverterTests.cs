// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Numerics;
using FluentAssertions;
using MudBlazor.Resources;
using MudBlazor.Utilities.Converter;
using MudBlazor.Utilities.Converter.Base;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Converters;

#nullable enable
[TestFixture]
public class DefaultConverterTests
{
    #region BigInteger

    private DefaultConverter.BigIntegerConverter CreateBigIntegerConverter(Func<CultureInfo>? culture = null, Func<string?>? format = null)
    {
        return new DefaultConverter.BigIntegerConverter(culture ?? (() => CultureInfo.InvariantCulture), format ?? (() => null));
    }

    [Test]
    public void BigInteger_Convert_ShouldReturnStringUsingProvidedCultureAndFormat()
    {
        var conv = CreateBigIntegerConverter(() => CultureInfo.InvariantCulture, () => null);
        var value = BigInteger.Parse("123456789012345678901234567890");
        var expected = value.ToString(null, CultureInfo.InvariantCulture);

        var result = conv.Convert(value);

        result.Should().Be(expected);
    }

    [Test]
    public void BigInteger_Convert_NullableNull_ReturnsNull()
    {
        var conv = CreateBigIntegerConverter();
        var result = conv.Convert(null);
        result.Should().BeNull();
    }

    [Test]
    public void BigInteger_ConvertBack_EmptyOrNull_ReturnsZero()
    {
        var conv = CreateBigIntegerConverter();
        conv.ConvertBack(string.Empty).Should().Be(BigInteger.Zero);
        conv.ConvertBack(null).Should().Be(BigInteger.Zero);
    }

    [Test]
    public void BigInteger_ConvertBack_ValidNumber_ReturnsParsedBigInteger()
    {
        var conv = CreateBigIntegerConverter(() => CultureInfo.InvariantCulture);
        var text = "98765432109876543210987654321";
        var expected = BigInteger.Parse(text, CultureInfo.InvariantCulture);

        var result = conv.ConvertBack(text);

        result.Should().Be(expected);
    }

    [Test]
    public void BigInteger_ConvertBack_Invalid_ThrowsConversionException_WithExpectedKey()
    {
        var conv = CreateBigIntegerConverter();
        Action act = () => conv.ConvertBack("not-a-number");

        act.Should()
           .Throw<ConversionException>()
           .Which.ErrorMessageKey
           .Should()
           .Be(LanguageResource.Converter_InvalidNumber);
    }

    [Test]
    public void BigIntegerNullableInterfaceConvertBack_EmptyOrNull_ReturnsNull()
    {
        var conv = CreateBigIntegerConverter();
        IReversibleConverter<BigInteger?, string?> nullableInterface = conv;

        nullableInterface.ConvertBack(string.Empty).Should().BeNull();
        nullableInterface.ConvertBack(null).Should().BeNull();
    }

    #endregion

    #region Bool

    [Test]
    public void Bool_Convert_ShouldReturnInvariantBooleanStrings()
    {
        var conv = DefaultConverter.BoolConverter.Instance;

        conv.Convert(true).Should().Be("True");
        conv.Convert(false).Should().Be("False");
    }

    [Test]
    public void Bool_Convert_NullableNull_ReturnsNull()
    {
        var conv = DefaultConverter.BoolConverter.Instance;
        conv.Convert(null).Should().BeNull();
    }

    [Test]
    public void Bool_ConvertBack_NonNullable_TrueInputs_ReturnTrue()
    {
        var conv = DefaultConverter.BoolConverter.Instance;
        var trueInputs = new[] { "true", "True", "TrUe", "1", "on", "ON" };

        foreach (var input in trueInputs)
            conv.ConvertBack(input).Should().BeTrue();
    }

    [Test]
    public void Bool_ConvertBack_NonNullable_OtherInputs_ReturnFalse()
    {
        var conv = DefaultConverter.BoolConverter.Instance;
        var falseInputs = new[] { "false", "0", "off", "OFF", "random", string.Empty, null };

        foreach (var input in falseInputs)
            conv.ConvertBack(input).Should().BeFalse();
    }

    [Test]
    public void Bool_NullableInterface_ConvertBack_MapsExpectedValues()
    {
        var conv = DefaultConverter.BoolConverter.Instance;
        IReversibleConverter<bool?, string?> nullableConv = conv;

        // true variants -> true
        foreach (var input in new[] { "true", "1", "on", "TrUe" })
            nullableConv.ConvertBack(input).Should().BeTrue();

        // false variants -> false
        foreach (var input in new[] { "false", "0", "off", "OFF" })
            nullableConv.ConvertBack(input).Should().BeFalse();

        // unknown / empty / null -> null
        foreach (var input in new[] { "maybe", string.Empty, null })
            nullableConv.ConvertBack(input).Should().BeNull();
    }

    #endregion

    #region Char

    [Test]
    public void Char_Convert_ShouldReturnString()
    {
        var conv = DefaultConverter.CharConverter.Instance;

        conv.Convert('A').Should().Be("A");
        conv.Convert('Ω').Should().Be("Ω");
    }

    [Test]
    public void Char_Convert_NullableNull_ReturnsNull()
    {
        var conv = DefaultConverter.CharConverter.Instance;
        conv.Convert(null).Should().BeNull();
    }

    [Test]
    public void Char_ConvertBack_EmptyOrNull_ReturnsNullChar()
    {
        var conv = DefaultConverter.CharConverter.Instance;

        conv.ConvertBack(string.Empty).Should().Be('\0');
        conv.ConvertBack(null).Should().Be('\0');
    }

    [Test]
    public void Char_ConvertBack_ValidString_ReturnsFirstCharacter()
    {
        var conv = DefaultConverter.CharConverter.Instance;

        conv.ConvertBack("Hello").Should().Be('H');
        conv.ConvertBack(" ").Should().Be(' ');
        conv.ConvertBack("Z").Should().Be('Z');
    }

    [Test]
    public void Char_NullableInterfaceConvertBack_Behavior()
    {
        var conv = DefaultConverter.CharConverter.Instance;
        IReversibleConverter<char?, string?> nullableConv = conv;

        nullableConv.ConvertBack(null).Should().Be(null);
        nullableConv.ConvertBack(string.Empty).Should().Be('\0');
        nullableConv.ConvertBack("Xyz").Should().Be('X');
    }

    #endregion

    #region DateOnly

    private DefaultConverter.DateOnlyConverter CreateDateOnlyConverter(Func<CultureInfo>? culture = null, Func<string?>? format = null)
    {
        return new DefaultConverter.DateOnlyConverter(culture ?? (() => CultureInfo.InvariantCulture), format ?? (() => null));
    }

    [Test]
    public void DateOnly_Convert_ShouldUseProvidedFormatAndCulture()
    {
        var conv = CreateDateOnlyConverter(() => CultureInfo.InvariantCulture, () => "yyyy-MM-dd");
        var value = new DateOnly(2025, 11, 30);
        var expected = value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        var result = conv.Convert(value);

        result.Should().Be(expected);
    }

    [Test]
    public void DateOnly_Convert_NullableNull_ReturnsNull()
    {
        var conv = CreateDateOnlyConverter();
        conv.Convert(null).Should().BeNull();
    }

    [Test]
    public void DateOnly_ConvertBack_EmptyOrNull_ReturnsDefaultDateOnly()
    {
        var conv = CreateDateOnlyConverter();
        conv.ConvertBack(null).Should().Be(default);
        conv.ConvertBack(string.Empty).Should().Be(default);
    }

    [Test]
    public void DateOnly_ConvertBack_ValidExactFormat_ReturnsParsedDateOnly()
    {
        var conv = CreateDateOnlyConverter(() => CultureInfo.InvariantCulture, () => "yyyy-MM-dd");
        const string Text = "2025-11-30";
        var expected = new DateOnly(2025, 11, 30);

        var result = conv.ConvertBack(Text);

        result.Should().Be(expected);
    }

    [Test]
    public void DateOnly_ConvertBack_WhenFormatIsNull_UsesCultureShortDatePattern()
    {
        var culture = new CultureInfo("en-GB"); // ShortDatePattern = "dd/MM/yyyy"
        var conv = CreateDateOnlyConverter(() => culture, () => null);
        const string Text = "30/11/2025";
        var expected = new DateOnly(2025, 11, 30);

        conv.ConvertBack(Text).Should().Be(expected);
    }

    [Test]
    public void DateOnly_ConvertBack_Invalid_ThrowsConversionException_WithExpectedKey()
    {
        var conv = CreateDateOnlyConverter(() => CultureInfo.InvariantCulture, () => "yyyy-MM-dd");

        Action act = () => conv.ConvertBack("not-a-date");

        act.Should()
            .Throw<ConversionException>()
            .Which.ErrorMessageKey
            .Should()
            .Be(LanguageResource.Converter_InvalidDateTime);
    }

    [Test]
    public void DateOnly_NullableInterfaceConvertBack_EmptyOrNull_ReturnsNull()
    {
        var conv = CreateDateOnlyConverter();
        IReversibleConverter<DateOnly?, string?> nullableConv = conv;

        nullableConv.ConvertBack(null).Should().BeNull();
        nullableConv.ConvertBack(string.Empty).Should().BeNull();
    }

    #endregion
}
