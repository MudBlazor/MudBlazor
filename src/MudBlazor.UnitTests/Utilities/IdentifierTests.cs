// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
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
    public void Create_WithoutPrefix_ShouldReturnIdentifierWithDefaultPrefix()
    {
        // Act
        var result = Identifier.Create();

        // Assert
        result.Should().StartWith("a");
        result.Length.Should().Be(9);
    }
}
