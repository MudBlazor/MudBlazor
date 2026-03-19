// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Converters;

#nullable enable
[TestFixture]
public class BoolConverterTests
{
    [Test]
    public void StringConverter()
    {
        var conv = BoolConverter<string>.Instance;
        conv.Convert("on").Should().BeTrue();
        conv.Convert("off").Should().BeFalse();

        conv.Convert("maybe").Should().BeNull();
        conv.Convert(null).Should().BeNull();

        conv.ConvertBack(true).Should().Be("on");
        conv.ConvertBack(false).Should().Be("off");
        conv.ConvertBack(null).Should().BeNull();
    }

    [Test]
    public void BoolConverter()
    {
        var conv = BoolConverter<bool>.Instance;

        conv.Convert(true).Should().BeTrue();
        conv.Convert(false).Should().BeFalse();

        conv.ConvertBack(true).Should().BeTrue();
        conv.ConvertBack(false).Should().BeFalse();
    }

    [Test]
    public void BoolIdentityConverter()
    {
        var conv = BoolConverter<bool?>.Instance;

        conv.Convert(true).Should().BeTrue();
        conv.Convert(false).Should().BeFalse();
        conv.Convert(null).Should().BeNull();

        conv.ConvertBack(true).Should().BeTrue();
        conv.ConvertBack(false).Should().BeFalse();
        conv.ConvertBack(null).Should().BeNull();
    }

    [Test]
    public void BoolConverter_Int_NonZeroIsTrue_ZeroIsFalse_Roundtrip()
    {
        var conv = BoolConverter<int>.Instance;
        conv.Convert(0).Should().BeFalse();
        conv.Convert(1).Should().BeTrue();
        conv.Convert(-1).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1);
        conv.ConvertBack(false).Should().Be(0);
        conv.ConvertBack(null).Should().Be(0);
    }

    [Test]
    public void BoolConverter_NullableInt_Behavior()
    {
        var conv = BoolConverter<int?>.Instance;
        conv.Convert(null).Should().BeNull();
        conv.Convert(0).Should().BeFalse();
        conv.Convert(5).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1);
        conv.ConvertBack(false).Should().Be(0);
        conv.ConvertBack(null).Should().BeNull();
    }

    // uint
    [Test]
    public void BoolConverter_UInt_NonZeroIsTrue_ZeroIsFalse_Roundtrip()
    {
        var conv = BoolConverter<uint>.Instance;
        conv.Convert(0u).Should().BeFalse();
        conv.Convert(1u).Should().BeTrue();
        conv.Convert(42u).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1u);
        conv.ConvertBack(false).Should().Be(0u);
        conv.ConvertBack(null).Should().Be(0u);
    }

    [Test]
    public void BoolConverter_NullableUInt_Behavior()
    {
        var conv = BoolConverter<uint?>.Instance;
        conv.Convert(null).Should().BeNull();
        conv.Convert(0u).Should().BeFalse();
        conv.Convert(2u).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1u);
        conv.ConvertBack(false).Should().Be(0u);
        conv.ConvertBack(null).Should().BeNull();
    }

    // short
    [Test]
    public void BoolConverter_Short_NonZeroIsTrue_ZeroIsFalse_Roundtrip()
    {
        var conv = BoolConverter<short>.Instance;
        conv.Convert(0).Should().BeFalse();
        conv.Convert(1).Should().BeTrue();
        conv.Convert(-3).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1);
        conv.ConvertBack(false).Should().Be(0);
        conv.ConvertBack(null).Should().Be(0);
    }

    [Test]
    public void BoolConverter_NullableShort_Behavior()
    {
        var conv = BoolConverter<short?>.Instance;
        conv.Convert(null).Should().BeNull();
        conv.Convert(0).Should().BeFalse();
        conv.Convert(7).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1);
        conv.ConvertBack(false).Should().Be(0);
        conv.ConvertBack(null).Should().BeNull();
    }

    // ushort
    [Test]
    public void BoolConverter_UShort_NonZeroIsTrue_ZeroIsFalse_Roundtrip()
    {
        var conv = BoolConverter<ushort>.Instance;
        conv.Convert(0).Should().BeFalse();
        conv.Convert(1).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1);
        conv.ConvertBack(false).Should().Be(0);
        conv.ConvertBack(null).Should().Be(0);
    }

    [Test]
    public void BoolConverter_NullableUShort_Behavior()
    {
        var conv = BoolConverter<ushort?>.Instance;
        conv.Convert(null).Should().BeNull();
        conv.Convert(0).Should().BeFalse();
        conv.Convert(9).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1);
        conv.ConvertBack(false).Should().Be(0);
        conv.ConvertBack(null).Should().BeNull();
    }

    // long
    [Test]
    public void BoolConverter_Long_NonZeroIsTrue_ZeroIsFalse_Roundtrip()
    {
        var conv = BoolConverter<long>.Instance;
        conv.Convert(0L).Should().BeFalse();
        conv.Convert(1L).Should().BeTrue();
        conv.Convert(-100L).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1L);
        conv.ConvertBack(false).Should().Be(0L);
        conv.ConvertBack(null).Should().Be(0L);
    }

    [Test]
    public void BoolConverter_NullableLong_Behavior()
    {
        var conv = BoolConverter<long?>.Instance;
        conv.Convert(null).Should().BeNull();
        conv.Convert(0L).Should().BeFalse();
        conv.Convert(123L).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1L);
        conv.ConvertBack(false).Should().Be(0L);
        conv.ConvertBack(null).Should().BeNull();
    }

    // ulong
    [Test]
    public void BoolConverter_ULong_NonZeroIsTrue_ZeroIsFalse_Roundtrip()
    {
        var conv = BoolConverter<ulong>.Instance;
        conv.Convert(0ul).Should().BeFalse();
        conv.Convert(1ul).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1ul);
        conv.ConvertBack(false).Should().Be(0ul);
        conv.ConvertBack(null).Should().Be(0ul);
    }

    [Test]
    public void BoolConverter_NullableULong_Behavior()
    {
        var conv = BoolConverter<ulong?>.Instance;
        conv.Convert(null).Should().BeNull();
        conv.Convert(0ul).Should().BeFalse();
        conv.Convert(5ul).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1ul);
        conv.ConvertBack(false).Should().Be(0ul);
        conv.ConvertBack(null).Should().BeNull();
    }

    // byte
    [Test]
    public void BoolConverter_Byte_NonZeroIsTrue_ZeroIsFalse_Roundtrip()
    {
        var conv = BoolConverter<byte>.Instance;
        conv.Convert(0).Should().BeFalse();
        conv.Convert(1).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1);
        conv.ConvertBack(false).Should().Be(0);
        conv.ConvertBack(null).Should().Be(0);
    }

    [Test]
    public void BoolConverter_NullableByte_Behavior()
    {
        var conv = BoolConverter<byte?>.Instance;
        conv.Convert(null).Should().BeNull();
        conv.Convert(0).Should().BeFalse();
        conv.Convert(3).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1);
        conv.ConvertBack(false).Should().Be(0);
        conv.ConvertBack(null).Should().BeNull();
    }

    // sbyte
    [Test]
    public void BoolConverter_SByte_NonZeroIsTrue_ZeroIsFalse_Roundtrip()
    {
        var conv = BoolConverter<sbyte>.Instance;
        conv.Convert(0).Should().BeFalse();
        conv.Convert(1).Should().BeTrue();
        conv.Convert(-1).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1);
        conv.ConvertBack(false).Should().Be(0);
        conv.ConvertBack(null).Should().Be(0);
    }

    [Test]
    public void BoolConverter_NullableSByte_Behavior()
    {
        var conv = BoolConverter<sbyte?>.Instance;
        conv.Convert(null).Should().BeNull();
        conv.Convert(0).Should().BeFalse();
        conv.Convert(-5).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1);
        conv.ConvertBack(false).Should().Be(0);
        conv.ConvertBack(null).Should().BeNull();
    }

    // float
    [Test]
    public void BoolConverter_Float_NonZeroIsTrue_ZeroIsFalse_Roundtrip()
    {
        var conv = BoolConverter<float>.Instance;
        conv.Convert(0f).Should().BeFalse();
        conv.Convert(1f).Should().BeTrue();
        conv.Convert(-1.5f).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1f);
        conv.ConvertBack(false).Should().Be(0f);
        conv.ConvertBack(null).Should().Be(0f);
    }

    [Test]
    public void BoolConverter_NullableFloat_Behavior()
    {
        var conv = BoolConverter<float?>.Instance;
        conv.Convert(null).Should().BeNull();
        conv.Convert(0f).Should().BeFalse();
        conv.Convert(2.5f).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1f);
        conv.ConvertBack(false).Should().Be(0f);
        conv.ConvertBack(null).Should().BeNull();
    }

    // double
    [Test]
    public void BoolConverter_Double_NonZeroIsTrue_ZeroIsFalse_Roundtrip()
    {
        var conv = BoolConverter<double>.Instance;
        conv.Convert(0d).Should().BeFalse();
        conv.Convert(1d).Should().BeTrue();
        conv.Convert(-3.14d).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1d);
        conv.ConvertBack(false).Should().Be(0d);
        conv.ConvertBack(null).Should().Be(0d);
    }

    [Test]
    public void BoolConverter_NullableDouble_Behavior()
    {
        var conv = BoolConverter<double?>.Instance;
        conv.Convert(null).Should().BeNull();
        conv.Convert(0d).Should().BeFalse();
        conv.Convert(3.14d).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1d);
        conv.ConvertBack(false).Should().Be(0d);
        conv.ConvertBack(null).Should().BeNull();
    }

    // decimal
    [Test]
    public void BoolConverter_Decimal_NonZeroIsTrue_ZeroIsFalse_Roundtrip()
    {
        var conv = BoolConverter<decimal>.Instance;
        conv.Convert(0m).Should().BeFalse();
        conv.Convert(1m).Should().BeTrue();
        conv.Convert(-10m).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1m);
        conv.ConvertBack(false).Should().Be(0m);
        conv.ConvertBack(null).Should().Be(0m);
    }

    [Test]
    public void BoolConverter_NullableDecimal_Behavior()
    {
        var conv = BoolConverter<decimal?>.Instance;
        conv.Convert(null).Should().BeNull();
        conv.Convert(0m).Should().BeFalse();
        conv.Convert(7.5m).Should().BeTrue();

        conv.ConvertBack(true).Should().Be(1m);
        conv.ConvertBack(false).Should().Be(0m);
        conv.ConvertBack(null).Should().BeNull();
    }

    // char
    [Test]
    public void BoolConverter_Char_NonZeroIsTrue_ZeroIsFalse_Roundtrip()
    {
        var conv = BoolConverter<char>.Instance;
        conv.Convert((char)0).Should().BeFalse();
        conv.Convert('A').Should().BeTrue();
        conv.Convert((char)1).Should().BeTrue();

        conv.ConvertBack(true).Should().Be((char)1);
        conv.ConvertBack(false).Should().Be((char)0);
        conv.ConvertBack(null).Should().Be((char)0);
    }

    [Test]
    public void BoolConverter_NullableChar_Behavior()
    {
        var conv = BoolConverter<char?>.Instance;
        conv.Convert(null).Should().BeNull();
        conv.Convert((char)0).Should().BeFalse();
        conv.Convert('Z').Should().BeTrue();

        conv.ConvertBack(true).Should().Be((char)1);
        conv.ConvertBack(false).Should().Be((char)0);
        conv.ConvertBack(null).Should().BeNull();
    }

    [Test]
    public void ObjectConverter_HandlesBoxedValues_And_RoundTrips()
    {
        var conv = BoolConverter<object>.Instance;

        // boxed string
        conv.Convert("on").Should().BeTrue();
        conv.Convert("off").Should().BeFalse();

        // null
        conv.Convert(null).Should().BeNull();

        // boxed number
        // int
        conv.Convert(-1).Should().BeTrue();
        conv.Convert(1).Should().BeTrue();
        conv.Convert(2).Should().BeTrue();
        conv.Convert(0).Should().BeFalse();

        // uint
        conv.Convert((uint)1).Should().BeTrue();
        conv.Convert((uint)2).Should().BeTrue();
        conv.Convert((uint)0).Should().BeFalse();

        // short
        conv.Convert((short)-1).Should().BeTrue();
        conv.Convert((short)1).Should().BeTrue();
        conv.Convert((short)2).Should().BeTrue();
        conv.Convert((short)0).Should().BeFalse();

        // ushort
        conv.Convert((ushort)1).Should().BeTrue();
        conv.Convert((ushort)2).Should().BeTrue();
        conv.Convert((ushort)0).Should().BeFalse();

        // long
        conv.Convert((long)-1).Should().BeTrue();
        conv.Convert((long)1).Should().BeTrue();
        conv.Convert((long)2).Should().BeTrue();
        conv.Convert((long)0).Should().BeFalse();

        // ulong
        conv.Convert((ulong)1).Should().BeTrue();
        conv.Convert((ulong)2).Should().BeTrue();
        conv.Convert((ulong)0).Should().BeFalse();

        // byte
        conv.Convert((byte)1).Should().BeTrue();
        conv.Convert((byte)2).Should().BeTrue();
        conv.Convert((byte)0).Should().BeFalse();

        // sbyte
        conv.Convert((sbyte)-1).Should().BeTrue();
        conv.Convert((sbyte)1).Should().BeTrue();
        conv.Convert((sbyte)2).Should().BeTrue();
        conv.Convert((sbyte)0).Should().BeFalse();

        // float
        conv.Convert((float)-1).Should().BeTrue();
        conv.Convert((float)1).Should().BeTrue();
        conv.Convert((float)2).Should().BeTrue();
        conv.Convert((float)0).Should().BeFalse();

        // double
        conv.Convert((double)-1).Should().BeTrue();
        conv.Convert((double)1).Should().BeTrue();
        conv.Convert((double)2).Should().BeTrue();
        conv.Convert((double)0).Should().BeFalse();

        // decimal
        conv.Convert((decimal)-1).Should().BeTrue();
        conv.Convert((decimal)1).Should().BeTrue();
        conv.Convert((decimal)2).Should().BeTrue();
        conv.Convert((decimal)0).Should().BeFalse();

        // char
        conv.Convert((char)1).Should().BeTrue();
        conv.Convert((char)2).Should().BeTrue();
        conv.Convert((char)0).Should().BeFalse();

        // boxed nullable number
        conv.Convert((int?)-1).Should().BeTrue();
        conv.Convert((int?)1).Should().BeTrue();
        conv.Convert((int?)2).Should().BeTrue();
        conv.Convert((int?)0).Should().BeFalse();

        conv.Convert((uint?)1).Should().BeTrue();
        conv.Convert((uint?)2).Should().BeTrue();
        conv.Convert((uint?)0).Should().BeFalse();

        conv.Convert((ushort?)1).Should().BeTrue();
        conv.Convert((ushort?)2).Should().BeTrue();
        conv.Convert((ushort?)0).Should().BeFalse();

        conv.Convert((long?)-1).Should().BeTrue();
        conv.Convert((long?)1).Should().BeTrue();
        conv.Convert((long?)2).Should().BeTrue();
        conv.Convert((long?)0).Should().BeFalse();

        conv.Convert((ulong?)1).Should().BeTrue();
        conv.Convert((ulong?)2).Should().BeTrue();
        conv.Convert((ulong?)0).Should().BeFalse();

        conv.Convert((byte?)1).Should().BeTrue();
        conv.Convert((byte?)2).Should().BeTrue();
        conv.Convert((byte?)0).Should().BeFalse();

        conv.Convert((sbyte?)-1).Should().BeTrue();
        conv.Convert((sbyte?)1).Should().BeTrue();
        conv.Convert((sbyte?)2).Should().BeTrue();
        conv.Convert((sbyte?)0).Should().BeFalse();

        conv.Convert((float?)-1).Should().BeTrue();
        conv.Convert((float?)1).Should().BeTrue();
        conv.Convert((float?)2).Should().BeTrue();
        conv.Convert((float?)0).Should().BeFalse();

        conv.Convert((double?)-1).Should().BeTrue();
        conv.Convert((double?)1).Should().BeTrue();
        conv.Convert((double?)2).Should().BeTrue();
        conv.Convert((double?)0).Should().BeFalse();

        conv.Convert((decimal?)-1).Should().BeTrue();
        conv.Convert((decimal?)1).Should().BeTrue();
        conv.Convert((decimal?)2).Should().BeTrue();
        conv.Convert((decimal?)0).Should().BeFalse();

        conv.Convert((char?)1).Should().BeTrue();
        conv.Convert((char?)2).Should().BeTrue();
        conv.Convert((char?)0).Should().BeFalse();

        // Roundtrip via ConvertBack(true) -> object -> Convert(...) == true
        var back = conv.ConvertBack(true);
        back.Should().NotBeNull();
        conv.Convert(back).Should().BeTrue();

        conv.ConvertBack(false).Should().Be(false);
        conv.ConvertBack(true).Should().Be(true);

        // unknown object -> exception
        Action act = () => conv.Convert(new object());

        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Cannot convert type System.Object to bool?");
    }
}
