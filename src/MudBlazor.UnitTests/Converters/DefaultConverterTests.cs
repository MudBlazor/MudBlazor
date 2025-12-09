// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Numerics;
using FluentAssertions;
using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Converters;

#nullable enable
[TestFixture]
public class DefaultConverterTests
{
    #region DefeultConverter

    [Test]
    public void DefaultConverter_Roundtrip_AllSupportedTypes_ForwardAndBack()
    {
        // string
        {
            var conv = new DefaultConverter<string>();
            const string StringValue = "hello world";
            var text = conv.Convert(StringValue);
            text.Should().Be(StringValue);
            conv.ConvertBack(text).Should().Be(StringValue);
            conv.ConvertBack(null).Should().BeNull();
        }

        // char
        {
            var conv = new DefaultConverter<char>();
            const char CharValue = 'Z';
            conv.Convert(CharValue).Should().Be("Z");
            conv.ConvertBack("Z").Should().Be(CharValue);
        }

        // bool
        {
            var conv = new DefaultConverter<bool>();
            const bool BoolValue = true;
            conv.Convert(BoolValue).Should().Be(BoolValue.ToString(CultureInfo.InvariantCulture));
            conv.ConvertBack("true").Should().BeTrue();
            conv.ConvertBack("off").Should().BeFalse(); // non-true -> false for non-nullable
        }

        // Guid
        {
            var conv = new DefaultConverter<Guid>();
            var guidValue = Guid.NewGuid();
            conv.Convert(guidValue).Should().Be(guidValue.ToString());
            conv.ConvertBack(guidValue.ToString()).Should().Be(guidValue);
        }

        // integers
        {
            var conv = new DefaultConverter<int>();
            const int IntValue = 123456;
            conv.Convert(IntValue).Should().Be(IntValue.ToString(null, CultureInfo.InvariantCulture));
            conv.ConvertBack(IntValue.ToString(CultureInfo.InvariantCulture)).Should().Be(IntValue);
            conv.ConvertBack(null).Should().Be(0); // non-nullable numeric returns zero for null/empty
            conv.ConvertBack(string.Empty).Should().Be(0);
        }

        // unsigned
        {
            var conv = new DefaultConverter<uint>();
            const uint UnsignedIntValue = 4000000000u;
            conv.Convert(UnsignedIntValue).Should().Be(UnsignedIntValue.ToString(null, CultureInfo.InvariantCulture));
            conv.ConvertBack(UnsignedIntValue.ToString(CultureInfo.InvariantCulture)).Should().Be(UnsignedIntValue);
        }

        // long/ulong
        {
            var convLong = new DefaultConverter<long>();
            const long LongValue = -123456789012345;
            convLong.Convert(LongValue).Should().Be(LongValue.ToString(null, CultureInfo.InvariantCulture));
            convLong.ConvertBack(LongValue.ToString(CultureInfo.InvariantCulture)).Should().Be(LongValue);

            var convULong = new DefaultConverter<ulong>();
            const ulong UnsignedLongValue = 123456789012345ul;
            convULong.Convert(UnsignedLongValue).Should().Be(UnsignedLongValue.ToString(null, CultureInfo.InvariantCulture));
            convULong.ConvertBack(UnsignedLongValue.ToString(CultureInfo.InvariantCulture)).Should().Be(UnsignedLongValue);
        }

        // floating point
        {
            var convDouble = new DefaultConverter<double>();
            const double DoubleValue = 3.14159265358979;
            convDouble.Convert(DoubleValue).Should().Be(DoubleValue.ToString(null, CultureInfo.InvariantCulture));
            convDouble.ConvertBack(DoubleValue.ToString(CultureInfo.InvariantCulture)).Should().BeApproximately(DoubleValue, 1e-10);

            var convFloat = new DefaultConverter<float>();
            const float FloatValue = 1.2345f;
            convFloat.Convert(FloatValue).Should().Be(FloatValue.ToString(null, CultureInfo.InvariantCulture));
            convFloat.ConvertBack(FloatValue.ToString(CultureInfo.InvariantCulture)).Should().BeApproximately(FloatValue, 1e-6f);
        }

        // decimal
        {
            var conv = new DefaultConverter<decimal>();
            const decimal DecimalValue = 12345.6789m;
            conv.Convert(DecimalValue).Should().Be(DecimalValue.ToString(null, CultureInfo.InvariantCulture));
            conv.ConvertBack(DecimalValue.ToString(CultureInfo.InvariantCulture)).Should().Be(DecimalValue);
        }

        // BigInteger
        {
            var conv = new DefaultConverter<BigInteger>();
            var bigIntegerValue = BigInteger.Parse("123456789012345678901234567890");
            conv.Convert(bigIntegerValue).Should().Be(bigIntegerValue.ToString(null, CultureInfo.InvariantCulture));
            conv.ConvertBack(bigIntegerValue.ToString(CultureInfo.InvariantCulture)).Should().Be(bigIntegerValue);
        }

        // DateTime
        {
            var conv = new DefaultConverter<DateTime> { Format = () => "yyyy-MM-dd HH:mm:ss", Culture = () => CultureInfo.InvariantCulture };
            var dateTimeValue = new DateTime(2025, 11, 30, 13, 45, 12);
            var text = conv.Convert(dateTimeValue);
            text.Should().Be(dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            conv.ConvertBack(text).Should().Be(dateTimeValue);
        }

        // DateTimeOffset
        {
            var conv = new DefaultConverter<DateTimeOffset> { Format = () => "o", Culture = () => CultureInfo.InvariantCulture };
            var dateTimeOffsetValue = new DateTimeOffset(2025, 11, 30, 13, 45, 12, TimeSpan.FromHours(2));
            var text = conv.Convert(dateTimeOffsetValue);
            text.Should().Be(dateTimeOffsetValue.ToString("o", CultureInfo.InvariantCulture));
            conv.ConvertBack(text).Should().Be(dateTimeOffsetValue);
        }

        // DateOnly
        {
            var conv = new DefaultConverter<DateOnly> { Format = () => "yyyy-MM-dd", Culture = () => CultureInfo.InvariantCulture };
            var dateOnlyValue = new DateOnly(2025, 11, 30);
            var text = conv.Convert(dateOnlyValue);
            text.Should().Be(dateOnlyValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            conv.ConvertBack(text).Should().Be(dateOnlyValue);
        }

        // TimeOnly
        {
            var conv = new DefaultConverter<TimeOnly> { Format = () => "HH:mm:ss", Culture = () => CultureInfo.InvariantCulture };
            var timeOnlyValue = new TimeOnly(14, 5, 6);
            var text = conv.Convert(timeOnlyValue);
            text.Should().Be(timeOnlyValue.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
            conv.ConvertBack(text).Should().Be(timeOnlyValue);
        }

        // TimeSpan
        {
            var conv = new DefaultConverter<TimeSpan> { Format = () => "c", Culture = () => CultureInfo.InvariantCulture };
            var timeSpanValue = new TimeSpan(1, 2, 3, 4, 567);
            var text = conv.Convert(timeSpanValue);
            text.Should().Be(timeSpanValue.ToString("c", CultureInfo.InvariantCulture));
            conv.ConvertBack(text).Should().Be(timeSpanValue);
        }

        // Nullable examples: int?, double?, DateTime?, Guid?
        {
            var convIntN = new DefaultConverter<int?>();
            int? intValue = 42;
            convIntN.Format = () => null;
            convIntN.Culture = () => CultureInfo.InvariantCulture;
            convIntN.Convert(intValue).Should().Be(intValue.Value.ToString(null, CultureInfo.InvariantCulture));
            convIntN.ConvertBack("42").Should().Be(42);
            convIntN.ConvertBack(null).Should().BeNull();

            var convDoubleN = new DefaultConverter<double?>();
            double? doubleValue = 2.5;
            convDoubleN.Format = () => "F1";
            convDoubleN.Culture = () => CultureInfo.InvariantCulture;
            convDoubleN.Convert(doubleValue).Should().Be(doubleValue.Value.ToString("F1", CultureInfo.InvariantCulture));
            convDoubleN.ConvertBack(null).Should().BeNull();
            convDoubleN.ConvertBack("2.5").Should().BeApproximately(2.5, 1e-10);

            var convDateN = new DefaultConverter<DateTime?> { Format = () => "yyyy-MM-dd", Culture = () => CultureInfo.InvariantCulture };
            var dateTimeValue = new DateTime(2025, 11, 30);
            convDateN.Convert(dateTimeValue).Should().Be(dateTimeValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            convDateN.ConvertBack(null).Should().BeNull();

            var convGuidN = new DefaultConverter<Guid?>();
            var guidValue = Guid.NewGuid();
            convGuidN.Convert(guidValue).Should().Be(guidValue.ToString());
            convGuidN.ConvertBack(null).Should().BeNull();
        }
    }

    [Test]
    public void DefaultConverter_CultureAndFormat_DynamicSwap_AffectsConversionAtRuntime()
    {
        // double: switching culture and format should change decimal separator and digits
        var convDouble = new DefaultConverter<double> { Format = () => "F2", Culture = () => CultureInfo.InvariantCulture };
        const double DoubleValue = 1234.5678;
        var textInvariant = convDouble.Convert(DoubleValue);
        textInvariant.Should().Be(DoubleValue.ToString("F2", CultureInfo.InvariantCulture)); // "1234.57"

        // Change culture to fr-FR and format to F3 at runtime
        convDouble.Format = () => "F3";
        convDouble.Culture = () => new CultureInfo("fr-FR");
        var textFrench = convDouble.Convert(DoubleValue);
        textFrench.Should().Be(DoubleValue.ToString("F3", new CultureInfo("fr-FR"))); // "1234,568"

        // ConvertBack should parse with current culture/format (the converter uses TryParse with NumberStyles.Any and current culture)
        convDouble.ConvertBack(textFrench).Should().BeApproximately(DoubleValue, 1e-2);

        // DateOnly: switching format/culture changes parsing/formatting
        var convDate = new DefaultConverter<DateOnly> { Format = () => "yyyy-MM-dd", Culture = () => CultureInfo.InvariantCulture };
        var date = new DateOnly(2025, 11, 30);
        convDate.Convert(date).Should().Be("2025-11-30");

        // Now change to en-GB short date pattern by leaving Format = null and setting Culture
        convDate.Format = () => null;
        convDate.Culture = () => new CultureInfo("en-GB"); // ShortDatePattern "dd/MM/yyyy"
        var gbString = date.ToString(CultureInfo.GetCultureInfo("en-GB").DateTimeFormat.ShortDatePattern, CultureInfo.GetCultureInfo("en-GB"));
        // Convert should follow the new culture's short date pattern
        convDate.Convert(date).Should().Be(date.ToString(convDate.Culture().DateTimeFormat.ShortDatePattern, convDate.Culture()));

        // And ConvertBack should parse the GB string
        var parsed = convDate.ConvertBack(gbString);
        parsed.Should().Be(date);
    }

    #endregion

    #region Enum

    [Test]
    public void DefaultConverter_Enum_ConvertAndConvertBack_NonNullable()
    {
        var conv = new DefaultConverter<DayOfWeek>();
        const DayOfWeek Value = DayOfWeek.Wednesday;
        const DayOfWeek Invalid = (DayOfWeek)(-1);

        // forward
        conv.Convert(Value).Should().Be("Wednesday");
        conv.Convert(Invalid).Should().Be("-1");

        // back
        conv.ConvertBack("Wednesday").Should().Be(Value);
        conv.ConvertBack(string.Empty).Should().Be(default);

        // invalid should throw ConversionException with expected key
        Action act = () => conv.ConvertBack("NotAValue");

        act.Should()
            .Throw<ConversionException>()
            .Which.ErrorMessageKey
            .Should()
            .Be(LanguageResource.Converter_NotValueOf);
    }

    [Test]
    public void DefaultConverter_Enum_ConvertAndConvertBack_Nullable()
    {
        var conv = new DefaultConverter<DayOfWeek?>();

        // null forward -> null string
        conv.Convert(DayOfWeek.Wednesday).Should().Be("Wednesday");
        conv.Convert((DayOfWeek)(-1)).Should().Be("-1");
        conv.Convert(null).Should().BeNull();

        // valid back
        conv.ConvertBack("Monday").Should().Be(DayOfWeek.Monday);
        conv.ConvertBack(string.Empty).Should().BeNull();
        conv.ConvertBack(null).Should().BeNull();

        // invalid back should throw ConversionException
        Action act = () => conv.ConvertBack("NoSuchDay");
        act.Should()
            .Throw<ConversionException>()
            .Which.ErrorMessageKey
            .Should()
            .Be(LanguageResource.Converter_NotValueOf);
    }

    #endregion

    #region BigInteger

    private static DefaultConverter.BigIntegerConverter CreateBigIntegerConverter(Func<CultureInfo>? culture = null, Func<string?>? format = null)
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
        nullableConv.ConvertBack(string.Empty).Should().Be(null);
        nullableConv.ConvertBack("Xyz").Should().Be('X');
    }

    #endregion

    #region DateOnly

    private static DefaultConverter.DateOnlyConverter CreateDateOnlyConverter(Func<CultureInfo>? culture = null, Func<string?>? format = null)
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

    #region DateTime

    private static DefaultConverter.DateTimeConverter CreateDateTimeConverter(Func<CultureInfo>? culture = null, Func<string?>? format = null)
    {
        return new DefaultConverter.DateTimeConverter(culture ?? (() => CultureInfo.InvariantCulture), format ?? (() => null));
    }

    [Test]
    public void DateTime_Convert_ShouldUseProvidedFormatAndCulture()
    {
        var conv = CreateDateTimeConverter(() => CultureInfo.InvariantCulture, () => "yyyy-MM-dd HH:mm");
        var value = new DateTime(2025, 11, 30, 13, 45, 0);
        var expected = value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

        var result = conv.Convert(value);

        result.Should().Be(expected);
    }

    [Test]
    public void DateTime_Convert_NullableNull_ReturnsNull()
    {
        var conv = CreateDateTimeConverter();
        conv.Convert(null).Should().BeNull();
    }

    [Test]
    public void DateTime_ConvertBack_EmptyOrNull_ReturnsDefaultDateTime()
    {
        var conv = CreateDateTimeConverter();
        conv.ConvertBack(null).Should().Be(default(DateTime));
        conv.ConvertBack(string.Empty).Should().Be(default(DateTime));
    }

    [Test]
    public void DateTime_ConvertBack_ValidExactFormat_ReturnsParsedDateTime()
    {
        var conv = CreateDateTimeConverter(() => CultureInfo.InvariantCulture, () => "yyyy-MM-dd HH:mm");
        const string Text = "2025-11-30 13:45";
        var expected = new DateTime(2025, 11, 30, 13, 45, 0);

        var result = conv.ConvertBack(Text);

        result.Should().Be(expected);
    }

    [Test]
    public void DateTime_ConvertBack_WhenFormatIsNull_UsesCultureShortDatePattern()
    {
        var culture = new CultureInfo("en-GB"); // ShortDatePattern = "dd/MM/yyyy"
        var conv = CreateDateTimeConverter(() => culture, () => null);
        const string Text = "30/11/2025";
        var expected = new DateTime(2025, 11, 30);

        conv.ConvertBack(Text).Should().Be(expected);
    }

    [Test]
    public void DateTime_ConvertBack_Invalid_ThrowsConversionException_WithExpectedKey()
    {
        var conv = CreateDateTimeConverter(() => CultureInfo.InvariantCulture, () => "yyyy-MM-dd");

        Action act = () => conv.ConvertBack("not-a-datetime");

        act.Should()
            .Throw<ConversionException>()
            .Which.ErrorMessageKey
            .Should()
            .Be(LanguageResource.Converter_InvalidDateTime);
    }

    [Test]
    public void DateTime_NullableInterfaceConvertBack_EmptyOrNull_ReturnsNull_And_ParsesValidValue()
    {
        var conv = CreateDateTimeConverter(() => CultureInfo.InvariantCulture, () => "yyyy-MM-dd");
        IReversibleConverter<DateTime?, string?> nullableConv = conv;

        nullableConv.ConvertBack(null).Should().BeNull();
        nullableConv.ConvertBack(string.Empty).Should().BeNull();

        var parsed = nullableConv.ConvertBack("2025-11-30");
        parsed.Should().Be(new DateTime(2025, 11, 30));
    }

    #endregion

    #region DateTimeOffset

    private static DefaultConverter.DateTimeOffsetConverter CreateDateTimeOffsetConverter(Func<CultureInfo>? culture = null, Func<string?>? format = null)
    {
        return new DefaultConverter.DateTimeOffsetConverter(culture ?? (() => CultureInfo.InvariantCulture), format ?? (() => null));
    }

    [Test]
    public void DateTimeOffset_Convert_ShouldUseProvidedFormatAndCulture()
    {
        var conv = CreateDateTimeOffsetConverter(() => CultureInfo.InvariantCulture, () => "o");
        var value = new DateTimeOffset(2025, 11, 30, 13, 45, 0, TimeSpan.FromHours(-5));
        var expected = value.ToString("o", CultureInfo.InvariantCulture);

        var result = conv.Convert(value);

        result.Should().Be(expected);
    }

    [Test]
    public void DateTimeOffset_Convert_NullableNull_ReturnsNull()
    {
        var conv = CreateDateTimeOffsetConverter();
        conv.Convert(null).Should().BeNull();
    }

    [Test]
    public void DateTimeOffset_ConvertBack_EmptyOrNull_ReturnsDefault()
    {
        var conv = CreateDateTimeOffsetConverter();
        conv.ConvertBack(null).Should().Be(default);
        conv.ConvertBack(string.Empty).Should().Be(default);
    }

    [Test]
    public void DateTimeOffset_ConvertBack_ValidExactFormat_ReturnsParsedDateTimeOffset()
    {
        var conv = CreateDateTimeOffsetConverter(() => CultureInfo.InvariantCulture, () => "o");
        var value = new DateTimeOffset(2025, 11, 30, 13, 45, 0, TimeSpan.FromHours(2));
        var text = value.ToString("o", CultureInfo.InvariantCulture);

        var result = conv.ConvertBack(text);

        result.Should().Be(value);
    }

    [Test]
    public void DateTimeOffset_ConvertBack_WhenFormatIsNull_UsesCultureShortDatePattern()
    {
        var culture = new CultureInfo("en-GB"); // ShortDatePattern = "dd/MM/yyyy"
        var conv = CreateDateTimeOffsetConverter(() => culture, () => null);
        var text = "30/11/2025";
        // Obtain expected using the same parsing logic as the converter
        var pattern = culture.DateTimeFormat.ShortDatePattern;
        var parsed = DateTimeOffset.TryParseExact(text, pattern, culture, DateTimeStyles.None, out var expected);
        parsed.Should().BeTrue("the test input must be parseable with the culture's ShortDatePattern");

        conv.ConvertBack(text).Should().Be(expected);
    }

    [Test]
    public void DateTimeOffset_ConvertBack_Invalid_ThrowsConversionException_WithExpectedKey()
    {
        var conv = CreateDateTimeOffsetConverter(() => CultureInfo.InvariantCulture, () => "o");

        Action act = () => conv.ConvertBack("not-a-datetime");

        act.Should()
            .Throw<ConversionException>()
            .Which.ErrorMessageKey
            .Should()
            .Be(LanguageResource.Converter_InvalidDateTime);
    }

    [Test]
    public void DateTimeOffset_NullableInterfaceConvertBack_EmptyOrNull_ReturnsNull_And_ParsesValidValue()
    {
        var conv = CreateDateTimeOffsetConverter(() => CultureInfo.InvariantCulture, () => "o");
        IReversibleConverter<DateTimeOffset?, string?> nullableConv = conv;

        nullableConv.ConvertBack(null).Should().BeNull();
        nullableConv.ConvertBack(string.Empty).Should().BeNull();

        var value = new DateTimeOffset(2025, 11, 30, 10, 0, 0, TimeSpan.Zero);
        var text = value.ToString("o", CultureInfo.InvariantCulture);
        nullableConv.ConvertBack(text).Should().Be(value);
    }

    #endregion

    #region Guid

    [Test]
    public void Guid_Convert_ShouldReturnString()
    {
        var conv = DefaultConverter.GuidConverter.Instance;
        var guid = Guid.NewGuid();

        conv.Convert(guid).Should().Be(guid.ToString());
    }

    [Test]
    public void Guid_Convert_NullableNull_ReturnsNull()
    {
        var conv = DefaultConverter.GuidConverter.Instance;
        conv.Convert(null).Should().BeNull();
    }

    [Test]
    public void Guid_ConvertBack_NullOrEmpty_ReturnsEmptyGuid()
    {
        var conv = DefaultConverter.GuidConverter.Instance;
        conv.ConvertBack(null).Should().Be(Guid.Empty);
        conv.ConvertBack(string.Empty).Should().Be(Guid.Empty);
    }

    [Test]
    public void Guid_ConvertBack_ValidGuidString_ReturnsParsedGuid()
    {
        var conv = DefaultConverter.GuidConverter.Instance;
        var guid = Guid.NewGuid();
        var text = guid.ToString();

        conv.ConvertBack(text).Should().Be(guid);
    }

    [Test]
    public void Guid_ConvertBack_Invalid_ThrowsConversionException_WithExpectedKey()
    {
        var conv = DefaultConverter.GuidConverter.Instance;

        Action act = () => conv.ConvertBack("not-a-guid");

        act.Should()
            .Throw<ConversionException>()
            .Which.ErrorMessageKey
            .Should()
            .Be(LanguageResource.Converter_InvalidGUID);
    }

    [Test]
    public void Guid_NullableInterfaceConvertBack_EmptyOrNull_ReturnsNull_And_ParsesValidValue()
    {
        var conv = DefaultConverter.GuidConverter.Instance;
        IReversibleConverter<Guid?, string?> nullableConv = conv;

        nullableConv.ConvertBack(null).Should().BeNull();
        nullableConv.ConvertBack(string.Empty).Should().BeNull();

        var guid = Guid.NewGuid();
        nullableConv.ConvertBack(guid.ToString()).Should().Be(guid);
    }

    #endregion

    #region NullableNumber

    private static DefaultConverter.NullableNumberConverter<TNumber> CreateNullableNumberConverter<TNumber>(Func<CultureInfo>? culture = null, Func<string?>? format = null)
        where TNumber : struct, INumber<TNumber>
    {
        return new DefaultConverter.NullableNumberConverter<TNumber>(culture ?? (() => CultureInfo.InvariantCulture), format ?? (() => null));
    }

    [Test]
    public void NullableNumber_Convert_Int_ReturnsStringUsingCulture()
    {
        var conv = CreateNullableNumberConverter<int>(() => CultureInfo.InvariantCulture, () => null);
        int? value = 12345;
        var expected = value.Value.ToString(null, CultureInfo.InvariantCulture);

        conv.Convert(value).Should().Be(expected);
    }

    [Test]
    public void NullableNumber_Convert_Double_WithFormat_ReturnsFormattedString()
    {
        var conv = CreateNullableNumberConverter<double>(() => CultureInfo.InvariantCulture, () => "F2");
        double? value = 3.14159;
        conv.Convert(value).Should().Be(value.Value.ToString("F2", CultureInfo.InvariantCulture));
    }

    [Test]
    public void NullableNumber_Convert_NullableNull_ReturnsNull()
    {
        var conv = CreateNullableNumberConverter<int>();
        conv.Convert(null).Should().BeNull();
    }

    [Test]
    public void NullableNumber_ConvertBack_NullOrEmpty_ReturnsNull()
    {
        var conv = CreateNullableNumberConverter<int>();
        conv.ConvertBack(null).Should().BeNull();
        conv.ConvertBack(string.Empty).Should().BeNull();
    }

    [Test]
    public void NullableNumber_ConvertBack_ValidInt_ReturnsParsedValue()
    {
        var conv = CreateNullableNumberConverter<int>(() => CultureInfo.InvariantCulture);
        const string Text = "999";
        conv.ConvertBack(Text).Should().Be(999);
    }

    [Test]
    public void NullableNumber_ConvertBack_ValidDouble_WithCulture_ReturnsParsedValue()
    {
        // french culture uses comma as decimal separator
        var culture = new CultureInfo("fr-FR");
        var conv = CreateNullableNumberConverter<double>(() => culture);
        const string Text = "3,14";
        conv.ConvertBack(Text).Should().BeApproximately(3.14, 1e-10);
    }

    [Test]
    public void NullableNumber_ConvertBack_Invalid_ThrowsConversionException_WithExpectedKey()
    {
        var conv = CreateNullableNumberConverter<int>();

        Action act = () => conv.ConvertBack("not-a-number");

        act.Should()
            .Throw<ConversionException>()
            .Which.ErrorMessageKey
            .Should()
            .Be(LanguageResource.Converter_InvalidNumber);
    }

    #endregion

    #region Number

    private static DefaultConverter.NumberConverter<TNumber> CreateNumberConverter<TNumber>(Func<CultureInfo>? culture = null, Func<string?>? format = null)
        where TNumber : struct, INumber<TNumber>
    {
        return new DefaultConverter.NumberConverter<TNumber>(culture ?? (() => CultureInfo.InvariantCulture), format ?? (() => null));
    }

    [Test]
    public void Number_Convert_Int_ShouldUseProvidedCultureAndFormat()
    {
        var conv = CreateNumberConverter<int>(() => CultureInfo.InvariantCulture, () => "N0");
        const int Value = 12345;
        var expected = Value.ToString("N0", CultureInfo.InvariantCulture);

        conv.Convert(Value).Should().Be(expected);
    }

    [Test]
    public void Number_Convert_Double_WithFormat_ReturnsFormattedString()
    {
        var conv = CreateNumberConverter<double>(() => CultureInfo.InvariantCulture, () => "F2");
        const double Value = 3.14159;
        conv.Convert(Value).Should().Be(Value.ToString("F2", CultureInfo.InvariantCulture));
    }

    [Test]
    public void Number_ConvertBack_Int_NullOrEmpty_ReturnsZero()
    {
        var conv = CreateNumberConverter<int>();
        conv.ConvertBack(null).Should().Be(0);
        conv.ConvertBack(string.Empty).Should().Be(0);
    }

    [Test]
    public void Number_ConvertBack_Double_ValidValue_ReturnsParsedDouble_WithCulture()
    {
        var culture = new CultureInfo("fr-FR"); // comma decimal separator
        var conv = CreateNumberConverter<double>(() => culture);
        const string Text = "3,14";
        conv.ConvertBack(Text).Should().BeApproximately(3.14, 1e-10);
    }

    [Test]
    public void Number_ConvertBack_Int_ValidValue_ReturnsParsedInt()
    {
        var conv = CreateNumberConverter<int>(() => CultureInfo.InvariantCulture);
        conv.ConvertBack("999").Should().Be(999);
    }

    [Test]
    public void Number_ConvertBack_Invalid_ThrowsConversionException_WithExpectedKey()
    {
        var conv = CreateNumberConverter<int>();

        Action act = () => conv.ConvertBack("not-a-number");

        act.Should()
            .Throw<ConversionException>()
            .Which.ErrorMessageKey
            .Should()
            .Be(LanguageResource.Converter_InvalidNumber);
    }

    #endregion

    #region String

    [Test]
    public void String_Convert_ShouldReturnSameString()
    {
        var conv = DefaultConverter.StringConverter.Instance;
        conv.Convert("hello").Should().Be("hello");
        conv.Convert(string.Empty).Should().Be(string.Empty);
    }

    [Test]
    public void String_Convert_Null_ReturnsNull()
    {
        var conv = DefaultConverter.StringConverter.Instance;
        conv.Convert(null).Should().BeNull();
    }

    [Test]
    public void String_ConvertBack_ShouldReturnSameString()
    {
        var conv = DefaultConverter.StringConverter.Instance;
        conv.ConvertBack("world").Should().Be("world");
        conv.ConvertBack(string.Empty).Should().Be(string.Empty);
    }

    [Test]
    public void String_ConvertBack_Null_ReturnsNull()
    {
        var conv = DefaultConverter.StringConverter.Instance;
        conv.ConvertBack(null).Should().BeNull();
    }

    #endregion

    #region TimeOnly

    private static DefaultConverter.TimeOnlyConverter CreateTimeOnlyConverter(Func<CultureInfo>? culture = null, Func<string?>? format = null)
    {
        return new DefaultConverter.TimeOnlyConverter(culture ?? (() => CultureInfo.InvariantCulture), format ?? (() => null));
    }

    [Test]
    public void TimeOnly_Convert_ShouldUseProvidedFormatAndCulture()
    {
        var conv = CreateTimeOnlyConverter(() => CultureInfo.InvariantCulture, () => "HH:mm");
        var value = new TimeOnly(13, 45);
        var expected = value.ToString("HH:mm", CultureInfo.InvariantCulture);

        conv.Convert(value).Should().Be(expected);
    }

    [Test]
    public void TimeOnly_Convert_NullableNull_ReturnsNull()
    {
        var conv = CreateTimeOnlyConverter();
        conv.Convert(null).Should().BeNull();
    }

    [Test]
    public void TimeOnly_ConvertBack_EmptyOrNull_ReturnsDefaultTimeOnly()
    {
        var conv = CreateTimeOnlyConverter();
        conv.ConvertBack(null).Should().Be(default);
        conv.ConvertBack(string.Empty).Should().Be(default);
    }

    [Test]
    public void TimeOnly_ConvertBack_ValidExactFormat_ReturnsParsedTimeOnly()
    {
        var conv = CreateTimeOnlyConverter(() => CultureInfo.InvariantCulture, () => "HH:mm:ss");
        const string Text = "08:30:15";
        var expected = new TimeOnly(8, 30, 15);

        conv.ConvertBack(Text).Should().Be(expected);
    }

    [Test]
    public void TimeOnly_ConvertBack_Invalid_ThrowsConversionException_WithExpectedKey()
    {
        var conv = CreateTimeOnlyConverter(() => CultureInfo.InvariantCulture, () => "HH:mm");

        Action act = () => conv.ConvertBack("not-a-time");

        act.Should()
            .Throw<ConversionException>()
            .Which.ErrorMessageKey
            .Should()
            .Be(LanguageResource.Converter_InvalidDateTime);
    }

    [Test]
    public void TimeOnly_NullableInterfaceConvertBack_EmptyOrNull_ReturnsNull_And_ParsesValidValue()
    {
        var conv = CreateTimeOnlyConverter(() => CultureInfo.InvariantCulture, () => "HH:mm");
        IReversibleConverter<TimeOnly?, string?> nullableConv = conv;

        nullableConv.ConvertBack(null).Should().BeNull();
        nullableConv.ConvertBack(string.Empty).Should().BeNull();

        var parsed = nullableConv.ConvertBack("14:05");
        parsed.Should().Be(new TimeOnly(14, 5));
    }

    #endregion

    #region TimeSpan

    private static DefaultConverter.TimeSpanConverter CreateTimeSpanConverter(Func<CultureInfo>? culture = null, Func<string?>? format = null)
    {
        return new DefaultConverter.TimeSpanConverter(culture ?? (() => CultureInfo.InvariantCulture), format ?? (() => null));
    }

    [Test]
    public void TimeSpan_Convert_ShouldUseProvidedFormatAndCulture()
    {
        var conv = CreateTimeSpanConverter(() => CultureInfo.InvariantCulture, () => "c");
        var value = new TimeSpan(1, 2, 3, 4, 567); // 1 day, 2 hours, 3 minutes, 4 seconds + milliseconds
        var expected = value.ToString("c", CultureInfo.InvariantCulture);

        conv.Convert(value).Should().Be(expected);
    }

    [Test]
    public void TimeSpan_Convert_NullableNull_ReturnsNull()
    {
        var conv = CreateTimeSpanConverter();
        conv.Convert(null).Should().BeNull();
    }

    [Test]
    public void TimeSpan_ConvertBack_EmptyOrNull_ReturnsZero()
    {
        var conv = CreateTimeSpanConverter();
        conv.ConvertBack(null).Should().Be(TimeSpan.Zero);
        conv.ConvertBack(string.Empty).Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void TimeSpan_ConvertBack_ValidExactFormat_ReturnsParsedTimeSpan()
    {
        // Use a custom format (hours:minutes)
        var conv = CreateTimeSpanConverter(() => CultureInfo.InvariantCulture, () => "hh\\:mm");
        var value = new TimeSpan(14, 5, 0); // 14:05
        var text = value.ToString("hh\\:mm", CultureInfo.InvariantCulture);

        conv.ConvertBack(text).Should().Be(value);
    }

    [Test]
    public void TimeSpan_ConvertBack_DefaultFormat_CanParse_c()
    {
        // When format is null the converter should use the default "c" format
        var conv = CreateTimeSpanConverter(() => CultureInfo.InvariantCulture, () => null);
        var value = new TimeSpan(0, 1, 2, 3, 456);
        var text = value.ToString("c", CultureInfo.InvariantCulture);

        conv.ConvertBack(text).Should().Be(value);
    }

    [Test]
    public void TimeSpan_ConvertBack_Invalid_ThrowsConversionException_WithExpectedKey()
    {
        var conv = CreateTimeSpanConverter(() => CultureInfo.InvariantCulture, () => "c");
        FluentActions.Invoking(() => conv.ConvertBack("not-a-timespan"))
            .Should()
            .Throw<ConversionException>()
            .Which.ErrorMessageKey
            .Should()
            .Be(LanguageResource.Converter_InvalidTimeSpan);
    }

    [Test]
    public void TimeSpan_NullableInterfaceConvertBack_EmptyOrNull_ReturnsNull_And_ParsesValidValue()
    {
        var conv = CreateTimeSpanConverter(() => CultureInfo.InvariantCulture, () => "hh\\:mm");
        IReversibleConverter<TimeSpan?, string?> nullableConv = conv;

        nullableConv.ConvertBack(null).Should().BeNull();
        nullableConv.ConvertBack(string.Empty).Should().BeNull();

        var parsed = nullableConv.ConvertBack("09:30");
        parsed.Should().Be(new TimeSpan(9, 30, 0));
    }

    #endregion
}
