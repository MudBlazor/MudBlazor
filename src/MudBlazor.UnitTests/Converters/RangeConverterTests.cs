// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Converters;

#nullable enable
[TestFixture]
public class RangeConverterTests
{
    [Test]
    public void Convert_NullRange_ReturnsEmptyString()
    {
        var conv = new RangeConverter<int>();
        conv.Convert(null).Should().BeEmpty();
    }

    [Test]
    public void Convert_IntRange_ProducesCanonicalString()
    {
        var conv = new RangeConverter<int>();
        var r = new Range<int>(1, 5);
        conv.Convert(r).Should().Be("[1;5]");
    }

    [Test]
    public void Convert_IntRange_WithNullStartOrEnd_ProducesEmptyPart()
    {
        var conv = new RangeConverter<int?>();
        var r1 = new Range<int?>(null, 5);
        conv.Convert(r1).Should().Be("[;5]");

        var r2 = new Range<int?>(7, null);
        conv.Convert(r2).Should().Be("[7;]");
    }

    [Test]
    public void ConvertBack_InvalidOrEmpty_ReturnsNull()
    {
        var conv = new RangeConverter<int>();
        conv.ConvertBack(null).Should().BeNull();
        conv.ConvertBack(string.Empty).Should().BeNull();
        conv.ConvertBack("bad-format").Should().BeNull();
    }

    [Test]
    public void ConvertBack_ValidIntRange_ReturnsParsedRange()
    {
        var conv = new RangeConverter<int>();
        var parsed = conv.ConvertBack("[10;20]");
        parsed.Should().NotBeNull();
        parsed!.Start.Should().Be(10);
        parsed.End.Should().Be(20);
    }

    [Test]
    public void ConvertBack_EmptyParts_ParsesToInnerDefaults()
    {
        var conv = new RangeConverter<int>();
        // "[;]" is valid canonical form -> inner DefaultConverter<int> treats empty as zero
        var parsed = conv.ConvertBack("[;]");
        parsed.Should().NotBeNull();
        parsed!.Start.Should().Be(0);
        parsed.End.Should().Be(0);
    }

    [Test]
    public void ConvertAndConvertBack_Roundtrip_WithStringElements()
    {
        var conv = new RangeConverter<string?>();
        var r = new Range<string?>("abc", "xyz");
        var text = conv.Convert(r);
        text.Should().Be("[abc;xyz]");

        var parsed = conv.ConvertBack(text);
        parsed.Should().NotBeNull();
        parsed!.Start.Should().Be("abc");
        parsed.End.Should().Be("xyz");
    }

    [Test]
    public void Convert_BothElementsNull_ReturnsEmptyString()
    {
        var conv = new RangeConverter<int?>();
        // Non-null range whose elements both stringify to null collapses to empty, like a null range.
        conv.Convert(new Range<int?>(null, null)).Should().BeEmpty();
    }

    [Test]
    public void ConvertBack_NullableEmptyParts_ParsesToNullElements()
    {
        var conv = new RangeConverter<int?>();
        // Unlike non-nullable int (empty -> 0), nullable elements parse empty parts back to null.
        var parsed = conv.ConvertBack("[;]");
        parsed.Should().NotBeNull();
        parsed!.Start.Should().BeNull();
        parsed.End.Should().BeNull();
    }

    [TestCase("5;10")]      // missing surrounding brackets
    [TestCase("[5]")]       // missing separator
    [TestCase("[;")]        // missing closing bracket
    [TestCase("5;10]")]     // missing opening bracket
    public void ConvertBack_StructurallyInvalid_ReturnsNull(string input)
    {
        var conv = new RangeConverter<int>();
        conv.ConvertBack(input).Should().BeNull();
    }

    [Test]
    public void ConvertBack_MultipleSeparators_SplitsOnFirst()
    {
        var conv = new RangeConverter<string?>();
        // Split uses the first ';', so the remainder (including further separators) belongs to the end.
        var parsed = conv.ConvertBack("[a;b;c]");
        parsed.Should().NotBeNull();
        parsed!.Start.Should().Be("a");
        parsed.End.Should().Be("b;c");
    }
}
