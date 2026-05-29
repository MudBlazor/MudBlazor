// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using MudBlazor.Utilities.Exceptions;
using NUnit.Framework;

#nullable enable

namespace MudBlazor.UnitTests.Other;

[TestFixture]
public class DateRangeTests
{
    [Test]
    public void ToString_NullStart_ReturnsEmptyString()
    {
        var range = new DateRange(null, new DateTime(2024, 6, 1));
        range.ToString(new DateTimeConverter()).Should().BeEmpty();
    }

    [Test]
    public void ToString_NullEnd_ReturnsEmptyString()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), null);
        range.ToString(new DateTimeConverter()).Should().BeEmpty();
    }

    [Test]
    public void ToString_BothDatesSet_ReturnsParts()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        var result = range.ToString(new DateTimeConverter());
        result.Should().Contain("2024-01-01").And.Contain("2024-06-01");
    }

    [Test]
    public void ToIsoDateString_NullStart_ReturnsEmptyString()
    {
        var range = new DateRange(null, new DateTime(2024, 6, 1));
        range.ToIsoDateString().Should().BeEmpty();
    }

    [Test]
    public void ToIsoDateString_NullEnd_ReturnsEmptyString()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), null);
        range.ToIsoDateString().Should().BeEmpty();
    }

    [Test]
    public void TryParse_InvalidRangeFormat_ReturnsFalse()
    {
        var ok = DateRange.TryParse("not-a-range", new DateTimeConverter(), out var result);
        ok.Should().BeFalse();
        result.Should().BeNull();
    }

    [Test]
    public void TryParse_NullValue_ReturnsFalse()
    {
        var ok = DateRange.TryParse((string?)null, new DateTimeConverter(), out var result);
        ok.Should().BeFalse();
        result.Should().BeNull();
    }

    [Test]
    public void TryParse_EndParseFailure_ReturnsFalse()
    {
        var ok = DateRange.TryParse("2024-01-01", "not-a-date", new FailingConverter(), out var result);
        ok.Should().BeFalse();
        result.Should().BeNull();
    }

    [Test]
    public void TryParse_StartParseFailure_ReturnsFalse()
    {
        var ok = DateRange.TryParse("not-a-date", null, new PartiallyFailingConverter(), out var result);
        ok.Should().BeFalse();
        result.Should().BeNull();
    }

    [Test]
    public void TryParse_BothValid_ReturnsTrue()
    {
        var ok = DateRange.TryParse("2024-01-01", "2024-06-01", new DateTimeConverter(), out var result);
        ok.Should().BeTrue();
        result.Should().NotBeNull();
        result!.Start.Should().Be(new DateTime(2024, 1, 1));
        result.End.Should().Be(new DateTime(2024, 6, 1));
    }

    [Test]
    public void EqualityOperator_BothNull_ReturnsTrue()
    {
        DateRange? a = null;
        DateRange? b = null;
        (a == b).Should().BeTrue();
    }

    [Test]
    public void EqualityOperator_OneNull_ReturnsFalse()
    {
        var a = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        DateRange? b = null;
        (a == b).Should().BeFalse();
        (b == a).Should().BeFalse();
    }

    [Test]
    public void InequalityOperator_DifferentRanges_ReturnsTrue()
    {
        var a = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        var b = new DateRange(new DateTime(2023, 1, 1), new DateTime(2023, 6, 1));
        (a != b).Should().BeTrue();
    }

    [Test]
    public void InequalityOperator_EqualRanges_ReturnsFalse()
    {
        var a = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        var b = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        (a != b).Should().BeFalse();
    }

    [Test]
    public void Equals_NonDateRangeObject_ReturnsFalse()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        range.Equals("not a range").Should().BeFalse();
        range.Equals(null).Should().BeFalse();
    }

    [Test]
    public void GetHashCode_EqualRanges_ReturnsSameHash()
    {
        var a = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        var b = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Test]
    public void GetHashCode_ConsistentAcrossMultipleCalls()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        var hash1 = range.GetHashCode();
        var hash2 = range.GetHashCode();
        hash1.Should().Be(hash2);
    }

    /// <summary>
    /// A simple reversible converter that round-trips DateTime? <-> string via ISO 8601.
    /// </summary>
    private sealed class DateTimeConverter : IReversibleConverter<DateTime?, string?>
    {
        public string? Convert(DateTime? input) => input?.ToString("yyyy-MM-dd");

        public DateTime? ConvertBack(string? input) =>
            string.IsNullOrEmpty(input) ? null : DateTime.Parse(input);
    }

    /// <summary>
    /// A converter whose ConvertBack always fails.
    /// </summary>
    private sealed class FailingConverter : IReversibleConverter<DateTime?, string?>
    {
        public string? Convert(DateTime? input) => null;

        public DateTime? ConvertBack(string? input) =>
            throw new ConversionException("ERR", ["bad"]);
    }

    /// <summary>
    /// A converter whose ConvertBack succeeds for null but fails for any non-null input.
    /// </summary>
    private sealed class PartiallyFailingConverter : IReversibleConverter<DateTime?, string?>
    {
        public string? Convert(DateTime? input) => null;

        public DateTime? ConvertBack(string? input) =>
            string.IsNullOrEmpty(input) ? null : throw new ConversionException("ERR", ["bad"]);
    }
}
