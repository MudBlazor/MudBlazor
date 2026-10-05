using System.Globalization;
using System.Runtime.CompilerServices;
using AwesomeAssertions;
using MudBlazor.Utilities.Comparer;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Comparer;

[TestFixture]
public class ReferenceCultureComparerTests
{
    [Test]
    public void Equals_UsesReferenceEquality()
    {
        var comparer = ReferenceCultureComparer.Default;
        var first = new CultureInfo("en-US");
        var second = new CultureInfo("en-US");

        comparer.Equals(first, first).Should().BeTrue();
        comparer.Equals(first, second).Should().BeFalse();
        comparer.Equals(null, null).Should().BeTrue();
    }

    [Test]
    public void Equals_NullAndNonNull_AreNotEqual()
    {
        var comparer = ReferenceCultureComparer.Default;
        var culture = new CultureInfo("en-US");

        comparer.Equals(null, culture).Should().BeFalse();
        comparer.Equals(culture, null).Should().BeFalse();
    }

    [Test]
    public void GetHashCode_UsesReferenceBasedHashCode()
    {
        var comparer = ReferenceCultureComparer.Default;
        var culture = new CultureInfo("fr-FR");

        comparer.GetHashCode(culture).Should().Be(RuntimeHelpers.GetHashCode(culture));
    }

    [Test]
    public void Dictionary_WithReferenceComparerTreatsEquivalentCulturesAsDifferentKeys()
    {
        var first = new CultureInfo("de-DE");
        var second = new CultureInfo("de-DE");
        var dictionary = new Dictionary<CultureInfo, string>(ReferenceCultureComparer.Default)
        {
            [first] = "first"
        };

        dictionary.ContainsKey(first).Should().BeTrue();
        dictionary.ContainsKey(second).Should().BeFalse();
    }
}
