// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities;

#nullable enable
[TestFixture]
public class NullableObjectTests
{
    [Test]
    public void Constructor_ShouldSetItemAndIsNull()
    {
        // Arrange & Act
        var obj = new NullableObject<string>("test");
        var nullObj = new NullableObject<string>(null);

        // Assert
        obj.Item.Should().Be("test");
        obj.IsNull.Should().BeFalse();
        nullObj.Item.Should().BeNull();
        nullObj.IsNull.Should().BeTrue();
    }

    [Test]
    public void ToString_ShouldReturnItemStringOrNull()
    {
        // Arrange
        var obj = new NullableObject<string>("test");
        var nullObj = new NullableObject<string>(null);

        // Act & Assert
        obj.ToString().Should().Be("test");
        nullObj.ToString().Should().Be("NULL");
    }

    [Test]
    public void Equals_ShouldReturnTrueForEqualObjects()
    {
        // Arrange
        var obj1 = new NullableObject<string>("test");
        var obj2 = new NullableObject<string>("test");
        var nullObj1 = new NullableObject<string>(null);
        var nullObj2 = new NullableObject<string>(null);

        // Act & Assert
        obj1.Equals(obj2).Should().BeTrue();
        nullObj1.Equals(nullObj2).Should().BeTrue();
    }

    [Test]
    public void Equals_ShouldReturnFalseForDifferentObjects()
    {
        // Arrange
        var obj1 = new NullableObject<string>("test1");
        var obj2 = new NullableObject<string>("test2");
        var obj = new NullableObject<string>("test");
        var nullObj = new NullableObject<string>(null);

        // Act & Assert
        obj1.Equals(obj2).Should().BeFalse();
        obj.Equals(nullObj).Should().BeFalse();
    }

    [Test]
    public void GetHashCode_ShouldReturnSameHashCodeForEqualObjects()
    {
        // Arrange
        var obj1 = new NullableObject<string>("test");
        var obj2 = new NullableObject<string>("test");
        var nullObj1 = new NullableObject<string>(null);
        var nullObj2 = new NullableObject<string>(null);

        // Act & Assert
        obj1.GetHashCode().Should().Be(obj2.GetHashCode());
        nullObj1.GetHashCode().Should().Be(nullObj2.GetHashCode());
    }

    [Test]
    public void ImplicitConversion_ShouldConvertToAndFromNullableObject()
    {
        // Arrange & Act
        NullableObject<string?> obj = "test";
        string? item = obj;
        NullableObject<string?> nullObj = null;
        string? nullItem = nullObj;

        // Assert
        obj.Item.Should().Be("test");
        obj.IsNull.Should().BeFalse();
        item.Should().Be("test");
        nullObj.Item.Should().BeNull();
        nullObj.IsNull.Should().BeTrue();
        nullItem.Should().BeNull();
    }

    [Test]
    public void Null_ShouldReturnNullObject()
    {
        // Arrange & Act
        var nullObj = NullableObject<string>.Null;

        // Assert
        nullObj.Item.Should().BeNull();
        nullObj.IsNull.Should().BeTrue();
    }

    [Test]
    public void OperatorEquals_ShouldReturnTrueForEqualObjects()
    {
        // Arrange
        var obj1 = new NullableObject<string>("test");
        var obj2 = new NullableObject<string>("test");
        var nullObj1 = new NullableObject<string>(null);
        var nullObj2 = new NullableObject<string>(null);

        // Act & Assert
        (obj1 == obj2).Should().BeTrue();
        (nullObj1 == nullObj2).Should().BeTrue();
    }

    [Test]
    public void OperatorNotEquals_ShouldReturnTrueForDifferentObjects()
    {
        // Arrange
        var obj1 = new NullableObject<string>("test1");
        var obj2 = new NullableObject<string>("test2");
        var obj = new NullableObject<string>("test");
        var nullObj = new NullableObject<string>(null);

        // Act & Assert
        (obj1 != obj2).Should().BeTrue();
        (obj != nullObj).Should().BeTrue();
    }
}
