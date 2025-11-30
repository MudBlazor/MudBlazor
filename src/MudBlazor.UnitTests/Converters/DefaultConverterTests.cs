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
    public void BigIntegerConvert_ShouldReturnStringUsingProvidedCultureAndFormat()
    {
        var conv = CreateBigIntegerConverter(() => CultureInfo.InvariantCulture, () => null);
        var value = BigInteger.Parse("123456789012345678901234567890");
        var expected = value.ToString(null, CultureInfo.InvariantCulture);

        var result = conv.Convert(value);

        result.Should().Be(expected);
    }

    [Test]
    public void BigIntegerConvert_NullableNull_ReturnsNull()
    {
        var conv = CreateBigIntegerConverter();
        var result = conv.Convert(null);
        result.Should().BeNull();
    }

    [Test]
    public void BigIntegerConvertBack_EmptyOrNull_ReturnsZero()
    {
        var conv = CreateBigIntegerConverter();
        conv.ConvertBack(string.Empty).Should().Be(BigInteger.Zero);
        conv.ConvertBack(null).Should().Be(BigInteger.Zero);
    }

    [Test]
    public void BigIntegerConvertBack_ValidNumber_ReturnsParsedBigInteger()
    {
        var conv = CreateBigIntegerConverter(() => CultureInfo.InvariantCulture);
        var text = "98765432109876543210987654321";
        var expected = BigInteger.Parse(text, CultureInfo.InvariantCulture);

        var result = conv.ConvertBack(text);

        result.Should().Be(expected);
    }

    [Test]
    public void BigIntegerConvertBack_Invalid_ThrowsConversionException_WithExpectedKey()
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
    public void BoolConvert_ShouldReturnInvariantBooleanStrings()
    {
        var conv = DefaultConverter.BoolConverter.Instance;

        conv.Convert(true).Should().Be("True");
        conv.Convert(false).Should().Be("False");
    }

    [Test]
    public void BoolConvert_NullableNull_ReturnsNull()
    {
        var conv = DefaultConverter.BoolConverter.Instance;
        conv.Convert(null).Should().BeNull();
    }

    [Test]
    public void BoolConvertBack_NonNullable_TrueInputs_ReturnTrue()
    {
        var conv = DefaultConverter.BoolConverter.Instance;
        var trueInputs = new[] { "true", "True", "TrUe", "1", "on", "ON" };

        foreach (var input in trueInputs)
            conv.ConvertBack(input).Should().BeTrue();
    }

    [Test]
    public void BoolConvertBack_NonNullable_OtherInputs_ReturnFalse()
    {
        var conv = DefaultConverter.BoolConverter.Instance;
        var falseInputs = new[] { "false", "0", "off", "OFF", "random", string.Empty, null };

        foreach (var input in falseInputs)
            conv.ConvertBack(input).Should().BeFalse();
    }

    [Test]
    public void BoolNullableInterface_ConvertBack_MapsExpectedValues()
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

    [Test]
    public void BoolInstance_StaticProperty_IsAvailable()
    {
        // Ensure the static Instance exists and behaves the same as a new instance
        var instance = DefaultConverter.BoolConverter.Instance;
        var created = new DefaultConverter.BoolConverter();

        instance.Convert(true).Should().Be(created.Convert(true));
        instance.Convert((bool?)null).Should().Be(created.Convert((bool?)null));
    }

    #endregion
}
