// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities;

#nullable enable
[TestFixture]
public class RangeUtilityTests
{
    [Test]
    public void Join_BothEmpty_ReturnsEmpty()
    {
        RangeUtility.Join(null, null).Should().BeEmpty();
        RangeUtility.Join(string.Empty, null).Should().BeEmpty();
        RangeUtility.Join(null, string.Empty).Should().BeEmpty();
    }

    [Test]
    public void Join_Parts_ReturnsCanonicalString()
    {
        RangeUtility.Join("1", "2").Should().Be("[1;2]");
        RangeUtility.Join(string.Empty, "5").Should().Be("[;5]");
        RangeUtility.Join("7", string.Empty).Should().Be("[7;]");
    }

    [Test]
    public void Split_InvalidInputs_ReturnsFalseAndOutputsEmptyParts()
    {
        RangeUtility.Split(null, out var s1, out var e1).Should().BeFalse();
        s1.Should().BeEmpty();
        e1.Should().BeEmpty();

        RangeUtility.Split(string.Empty, out var s2, out var e2).Should().BeFalse();
        s2.Should().BeEmpty();
        e2.Should().BeEmpty();

        RangeUtility.Split("no-brackets", out var s3, out var e3).Should().BeFalse();
        s3.Should().BeEmpty();
        e3.Should().BeEmpty();

        RangeUtility.Split("[missingsemicolon]", out var s4, out var e4).Should().BeFalse();
        s4.Should().BeEmpty();
        e4.Should().BeEmpty();
    }

    [Test]
    public void Split_ValidInputs_ExtractsParts()
    {
        RangeUtility.Split("[1;2]", out var s, out var e).Should().BeTrue();
        s.Should().Be("1");
        e.Should().Be("2");

        RangeUtility.Split("[;]", out var s2, out var e2).Should().BeTrue();
        s2.Should().BeEmpty();
        e2.Should().BeEmpty();

        RangeUtility.Split("[;5]", out var s3, out var e3).Should().BeTrue();
        s3.Should().BeEmpty();
        e3.Should().Be("5");

        RangeUtility.Split("[7;]", out var s4, out var e4).Should().BeTrue();
        s4.Should().Be("7");
        e4.Should().BeEmpty();
    }
}
