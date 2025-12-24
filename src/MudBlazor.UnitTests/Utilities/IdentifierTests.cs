// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities;

#nullable enable
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
        var firstChar = result[0];
        (firstChar is >= 'a' and <= 'z').Should().BeTrue("first character should be a lowercase letter");
    }

    [Test]
    public void Create_WithoutPrefix_ShouldGenerateUniqueIdentifiers()
    {
        // Arrange
        const int Count = 1000;
        var results = new HashSet<string>();
        var duplicates = 0;

        // Act
        for (var i = 0; i < Count; i++)
        {
            if (!results.Add(Identifier.Create()))
            {
                duplicates++;
            }
        }

        // Assert - allow tiny chance of collision
        duplicates.Should().BeInRange(0, 2, "collisions are extremely unlikely but theoretically possible");

        // Optionally also assert that most of the identifiers are unique
        results.Count.Should().BeGreaterThan(Count - 3, "almost all identifiers should be unique");
    }

    [Test]
    public void Create_WithoutPrefix_FirstCharacterShouldVary()
    {
        // Act - generate multiple identifiers and collect first characters
        var firstChars = new HashSet<char>();
        for (var i = 0; i < 100; i++)
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
