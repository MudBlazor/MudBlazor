// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DateRangeTests
    {
        private static DefaultConverter<T> Converter<T>() => new()
        {
            Culture = () => CultureInfo.InvariantCulture,
            Format = () => "yyyy-MM-dd"
        };

        private static DateTime D(int year, int month, int day) => new(year, month, day, 0, 0, 0, DateTimeKind.Unspecified);

        [Test]
        public void DefaultConstructor_LeavesStartAndEndNull()
        {
            var range = new DateRange<DateTime?>();
            range.Start.Should().BeNull();
            range.End.Should().BeNull();
        }

        [Test]
        public void Constructor_AssignsStartAndEnd()
        {
            var range = new DateRange<DateTime?>(D(2024, 1, 1), D(2024, 1, 5));
            range.Start.Should().Be(D(2024, 1, 1));
            range.End.Should().Be(D(2024, 1, 5));
        }

        [Test]
        public void ToString_WithConverter_ReturnsEmptyWhenStartOrEndIsNull()
        {
            var converter = Converter<DateTime?>();
            new DateRange<DateTime?>(null, D(2024, 1, 5)).ToString(converter).Should().Be(string.Empty);
            new DateRange<DateTime?>(D(2024, 1, 1), null).ToString(converter).Should().Be(string.Empty);
            new DateRange<DateTime?>(null, null).ToString(converter).Should().Be(string.Empty);
        }

        [Test]
        public void ToString_WithConverter_JoinsConvertedParts()
        {
            var range = new DateRange<DateTime?>(D(2024, 1, 1), D(2024, 1, 5));
            range.ToString(Converter<DateTime?>()).Should().Be("[2024-01-01;2024-01-05]");
        }

        [Test]
        public void ToIsoDateString_DateTime_FormatsRange()
        {
            var range = new DateRange<DateTime?>(D(2024, 3, 4), D(2024, 3, 9));
            range.ToIsoDateString().Should().Be("[2024-03-04;2024-03-09]");
        }

        [Test]
        public void ToIsoDateString_DateOnly_FormatsRange()
        {
            var range = new DateRange<DateOnly?>(new DateOnly(2024, 3, 4), new DateOnly(2024, 3, 9));
            range.ToIsoDateString().Should().Be("[2024-03-04;2024-03-09]");
        }

        [Test]
        public void ToIsoDateString_DateTimeOffset_FormatsRange()
        {
            var range = new DateRange<DateTimeOffset?>(
                new DateTimeOffset(2024, 3, 4, 0, 0, 0, TimeSpan.FromHours(2)),
                new DateTimeOffset(2024, 3, 9, 0, 0, 0, TimeSpan.FromHours(2)));
            range.ToIsoDateString().Should().Be("[2024-03-04;2024-03-09]");
        }

        [Test]
        public void ToIsoDateString_ReturnsEmptyWhenStartOrEndIsNull()
        {
            new DateRange<DateTime?>(null, D(2024, 1, 5)).ToIsoDateString().Should().Be(string.Empty);
            new DateRange<DateTime?>(D(2024, 1, 1), null).ToIsoDateString().Should().Be(string.Empty);
        }

        [Test]
        public void ToIsoDateString_UnsupportedT_Throws()
        {
            var range = new DateRange<int?>(1, 2);
            var act = () => range.ToIsoDateString();
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void TryParse_Single_ReturnsFalseWhenSplitFails()
        {
            DateRange<DateTime?>.TryParse("not-a-range", Converter<DateTime?>(), out var range).Should().BeFalse();
            range.Should().BeNull();
        }

        [Test]
        public void TryParse_Single_ParsesValidRange()
        {
            DateRange<DateTime?>.TryParse("[2024-01-01;2024-01-05]", Converter<DateTime?>(), out var range).Should().BeTrue();
            range.Start.Should().Be(D(2024, 1, 1));
            range.End.Should().Be(D(2024, 1, 5));
        }

        [Test]
        public void TryParse_StartEnd_ReturnsFalseWhenEndInvalid()
        {
            DateRange<DateTime?>.TryParse("2024-01-01", "not-a-date", Converter<DateTime?>(), out var range).Should().BeFalse();
            range.Should().BeNull();
        }

        [Test]
        public void TryParse_StartEnd_ReturnsFalseWhenStartInvalid()
        {
            DateRange<DateTime?>.TryParse("not-a-date", "2024-01-05", Converter<DateTime?>(), out var range).Should().BeFalse();
            range.Should().BeNull();
        }

        [Test]
        public void TryParse_StartEnd_ParsesValidValues()
        {
            DateRange<DateTime?>.TryParse("2024-01-01", "2024-01-05", Converter<DateTime?>(), out var range).Should().BeTrue();
            range.Start.Should().Be(D(2024, 1, 1));
            range.End.Should().Be(D(2024, 1, 5));
        }

        [Test]
        public void Equals_ReturnsTrueForSameValues()
        {
            var a = new DateRange<DateTime?>(D(2024, 1, 1), D(2024, 1, 5));
            var b = new DateRange<DateTime?>(D(2024, 1, 1), D(2024, 1, 5));

            a.Equals(b).Should().BeTrue();
            a.Equals((object)b).Should().BeTrue();
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Test]
        public void Equals_ReturnsFalseForDifferentValues()
        {
            var a = new DateRange<DateTime?>(D(2024, 1, 1), D(2024, 1, 5));
            var c = new DateRange<DateTime?>(D(2024, 1, 1), D(2024, 1, 6));

            a.Equals(c).Should().BeFalse();
            a.Equals((object)"foo").Should().BeFalse();
            a.Equals(null).Should().BeFalse();
        }

        [Test]
        public void EqualityOperators_HandleNullAndReferenceEquality()
        {
            var a = new DateRange<DateTime?>(D(2024, 1, 1), D(2024, 1, 5));
            var b = new DateRange<DateTime?>(D(2024, 1, 1), D(2024, 1, 5));
            var c = new DateRange<DateTime?>(D(2024, 1, 1), D(2024, 1, 6));

            (a == b).Should().BeTrue();
            (a != c).Should().BeTrue();
            (a == null).Should().BeFalse();
            (null == a).Should().BeFalse();
            ((DateRange<DateTime?>)null == null).Should().BeTrue();
        }
    }
}
