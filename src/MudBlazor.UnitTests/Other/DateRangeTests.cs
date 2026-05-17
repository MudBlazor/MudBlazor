// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Utilities.Exceptions;
using NUnit.Framework;

#nullable enable

namespace MudBlazor.UnitTests.Other;

[TestFixture]
public class DateRangeTests
{
    // A simple reversible converter that round-trips DateTime? <-> string via ISO 8601.
    private sealed class DateTimeConverter : IReversibleConverter<DateTime?, string?>
    {
        public string? Convert(DateTime? input) => input?.ToString("yyyy-MM-dd");

        public DateTime? ConvertBack(string? input) =>
            string.IsNullOrEmpty(input) ? null : DateTime.Parse(input);
    }

    // A converter whose ConvertBack always fails.
    private sealed class FailingConverter : IReversibleConverter<DateTime?, string?>
    {
        public string? Convert(DateTime? input) => null;

        public DateTime? ConvertBack(string? input) =>
            throw new ConversionException("ERR", ["bad"]);
    }

    // A converter whose ConvertBack succeeds for null but fails for any non-null input.
    private sealed class PartiallyFailingConverter : IReversibleConverter<DateTime?, string?>
    {
        public string? Convert(DateTime? input) => null;

        public DateTime? ConvertBack(string? input) =>
            string.IsNullOrEmpty(input) ? null : throw new ConversionException("ERR", ["bad"]);
    }

    // ── ToString(converter) ───────────────────────────────────────────────────

    [Test]
    public void ToString_NullStart_ReturnsEmptyString()
    {
        var range = new DateRange(null, new DateTime(2024, 6, 1));
        var result = range.ToString(new DateTimeConverter());
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ToString_NullEnd_ReturnsEmptyString()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), null);
        var result = range.ToString(new DateTimeConverter());
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ToString_BothDatesSet_ReturnsParts()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        var result = range.ToString(new DateTimeConverter());
        Assert.That(result, Does.Contain("2024-01-01").And.Contain("2024-06-01"));
    }

    // ── ToIsoDateString() ─────────────────────────────────────────────────────

    [Test]
    public void ToIsoDateString_NullStart_ReturnsEmptyString()
    {
        var range = new DateRange(null, new DateTime(2024, 6, 1));
        Assert.That(range.ToIsoDateString(), Is.Empty);
    }

    [Test]
    public void ToIsoDateString_NullEnd_ReturnsEmptyString()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), null);
        Assert.That(range.ToIsoDateString(), Is.Empty);
    }

    // ── TryParse(string, converter) ───────────────────────────────────────────

    [Test]
    public void TryParse_InvalidRangeFormat_ReturnsFalse()
    {
        var ok = DateRange.TryParse("not-a-range", new DateTimeConverter(), out var result);
        Assert.That(ok, Is.False);
        Assert.That(result, Is.Null);
    }

    [Test]
    public void TryParse_NullValue_ReturnsFalse()
    {
        var ok = DateRange.TryParse((string?)null, new DateTimeConverter(), out var result);
        Assert.That(ok, Is.False);
        Assert.That(result, Is.Null);
    }

    // ── TryParse(start, end, converter) ──────────────────────────────────────

    [Test]
    public void TryParse_EndParseFailure_ReturnsFalse()
    {
        var ok = DateRange.TryParse("2024-01-01", "not-a-date", new FailingConverter(), out var result);
        Assert.That(ok, Is.False);
        Assert.That(result, Is.Null);
    }

    [Test]
    public void TryParse_StartParseFailure_ReturnsFalse()
    {
        // End succeeds (null → null), start throws.
        var ok = DateRange.TryParse("not-a-date", null, new PartiallyFailingConverter(), out var result);
        Assert.That(ok, Is.False);
        Assert.That(result, Is.Null);
    }

    [Test]
    public void TryParse_BothValid_ReturnsTrue()
    {
        var ok = DateRange.TryParse("2024-01-01", "2024-06-01", new DateTimeConverter(), out var result);
        Assert.That(ok, Is.True);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Start, Is.EqualTo(new DateTime(2024, 1, 1)));
        Assert.That(result.End, Is.EqualTo(new DateTime(2024, 6, 1)));
    }

    // ── Equality operators ────────────────────────────────────────────────────

    [Test]
    public void EqualityOperator_BothNull_ReturnsTrue()
    {
        DateRange? a = null;
        DateRange? b = null;
        Assert.That(a == b, Is.True);
    }

    [Test]
    public void EqualityOperator_OneNull_ReturnsFalse()
    {
        var a = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        DateRange? b = null;
        Assert.That(a == b, Is.False);
        Assert.That(b == a, Is.False);
    }

    [Test]
    public void InequalityOperator_DifferentRanges_ReturnsTrue()
    {
        var a = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        var b = new DateRange(new DateTime(2023, 1, 1), new DateTime(2023, 6, 1));
        Assert.That(a != b, Is.True);
    }

    [Test]
    public void InequalityOperator_EqualRanges_ReturnsFalse()
    {
        var a = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        var b = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        Assert.That(a != b, Is.False);
    }

    [Test]
    public void Equals_NonDateRangeObject_ReturnsFalse()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        Assert.That(range.Equals("not a range"), Is.False);
        Assert.That(range.Equals(null), Is.False);
    }

    // ── GetHashCode ───────────────────────────────────────────────────────────

    [Test]
    public void GetHashCode_EqualRanges_ReturnsSameHash()
    {
        var a = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        var b = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
    }

    [Test]
    public void GetHashCode_ConsistentAcrossMultipleCalls()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 6, 1));
        var hash1 = range.GetHashCode();
        var hash2 = range.GetHashCode();
        Assert.That(hash1, Is.EqualTo(hash2));
    }
}
