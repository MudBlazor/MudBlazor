// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities;

[TestFixture]
public class IdentifierTests
{
    [Test]
    public void Create_WithPrefix_ShouldReturnIdentifierWithPrefix()
    {
        // Arrange
        const string Prefix = "prefix";

        // Act
        var result = Identifier.Create(Prefix);

        // Assert
        result.Should().StartWith(Prefix);
        result.Length.Should().Be(Prefix.Length + 8);
    }

    [Test]
    public void Create_WithoutPrefix_ShouldReturnIdentifierWithRandomPrefix()
    {
        // Act
        var result = Identifier.Create();

        // Assert
        result.Length.Should().Be(9);
        // First character should be a letter (a-z)
        char firstChar = result[0];
        (firstChar >= 'a' && firstChar <= 'z').Should().BeTrue("first character should be a lowercase letter");
    }

    [Test]
    public void Create_WithoutPrefix_ShouldGenerateUniqueIdentifiers()
    {
        // Act
        var results = new HashSet<string>();
        for (int i = 0; i < 1000; i++)
        {
            results.Add(Identifier.Create());
        }

        // Assert - all 1000 should be unique
        results.Count.Should().Be(1000);
    }

    [Test]
    public void Create_WithoutPrefix_FirstCharacterShouldVary()
    {
        // Act - generate multiple identifiers and collect first characters
        var firstChars = new HashSet<char>();
        for (int i = 0; i < 100; i++)
        {
            var id = Identifier.Create();
            firstChars.Add(id[0]);
        }

        // Assert - should have multiple different first characters (high probability)
        firstChars.Count.Should().BeGreaterThan(5, "first character should vary randomly");
    }

    [Test]
    public void Create_WithPrefix_ShouldHandleEmptyPrefix()
    {
        // Act
        var result = Identifier.Create("");

        // Assert
        result.Length.Should().Be(8);
    }

    [Test]
    public void Create_WithPrefix_ShouldHandleLongPrefix()
    {
        // Arrange
        const string LongPrefix = "verylongprefixwithmanychars";

        // Act
        var result = Identifier.Create(LongPrefix);

        // Assert
        result.Should().StartWith(LongPrefix);
        result.Length.Should().Be(LongPrefix.Length + 8);
    }
}
