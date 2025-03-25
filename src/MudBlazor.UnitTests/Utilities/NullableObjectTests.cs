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
        var nullObj = NullableObject<string>.Null;
        NullableObject<string> defaultObj = default;
        NullableObject<int> defaultStructObj = default;
        var nullStructObject = NullableObject<int>.Null;
        NullableObject<int?> defaultNullableStructObj = default;

        // Assert
        obj.Item.Should().Be("test");
        obj.IsNull.Should().BeFalse();
        nullObj.Item.Should().BeNull();
        nullObj.IsNull.Should().BeTrue();
        defaultObj.Item.Should().BeNull();
        defaultObj.IsNull.Should().BeTrue();
        defaultStructObj.Item.Should().Be(0);
        defaultStructObj.IsNull.Should().BeFalse();
        nullStructObject.Item.Should().Be(0, because: "Structs cannot be null");
        nullStructObject.IsNull.Should().BeFalse(because: "Structs cannot be null");
        defaultNullableStructObj.Item.Should().BeNull();
        defaultNullableStructObj.IsNull.Should().BeTrue();
    }

    [Test]
    public void ToString_ShouldReturnItemStringOrNull()
    {
        // Arrange
        var obj = new NullableObject<string>("test");
        var nullObj = NullableObject<string>.Null;

        // Act & Assert
        obj.ToString().Should().Be("test");
        nullObj.ToString().Should().Be("NULL");
    }

    [Test]
    public void Equals_WithSameNullableObject_ShouldReturnTrue()
    {
        // Arrange
        var obj1 = new NullableObject<string>("test");
        var obj2 = new NullableObject<string>("test");

        // Act
        var result = obj1.Equals(obj2);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Equals_WithDifferentNullableObject_ShouldReturnFalse()
    {
        // Arrange
        var obj1 = new NullableObject<string>("test1");
        var obj2 = new NullableObject<string>("test2");
        var obj3 = NullableObject<string>.Null;

        // Act
        var result1 = obj1.Equals(obj2);
        var result2 = obj1.Equals(obj3);

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [Test]
    public void EqualsT_WithSameItem_ShouldReturnTrue()
    {
        // Arrange
        var obj = new NullableObject<string>("test");
        const string Item = "test";

        // Act
        var result = obj.Equals(Item);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void EqualsT_WithDifferentItem_ShouldReturnFalse()
    {
        // Arrange
        var obj = new NullableObject<string>("test1");
        const string Item = "test2";

        // Act
        var result = obj.Equals(Item);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void EqualsT_WithNullItem_ShouldReturnTrue()
    {
        // Arrange
        var obj = NullableObject<string>.Null;
        string? item = null;

        // Act
        var result = obj.Equals(item);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void EqualsT_WithNullableAndNonNullObject_ShouldReturnFalse()
    {
        // Arrange
        var nullableObj = NullableObject<object>.Null;
        var obj = new object();

        // Act
        var result = nullableObj.Equals(obj);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Equals_WithNullObject_ShouldReturnTrue()
    {
        // Arrange
        var obj1 = new NullableObject<string>(null);
        var obj2 = NullableObject<string>.Null;

        // Act
        var result = obj1.Equals(obj2);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void EqualsObject_WithNonNullableDifferentTypeObject_ShouldReturnFalse()
    {
        // Arrange
        var obj = new NullableObject<string>("test");
        var nonNullableObj = new object();

        // Act
        var result = obj.Equals(nonNullableObj);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void EqualsObject_WithSameNullableObject_ShouldReturnTrue()
    {
        // Arrange
        var obj = new NullableObject<string>("test");
        object nonNullableObj = "test";

        // Act
        var result = obj.Equals(nonNullableObj);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void EqualsObject_WithNonNullableAndNullableObject_ShouldReturnFalse()
    {
        // Arrange
        object obj = new NullableObject<string>("test");
        object nullableObj = NullableObject<string>.Null;

        // Act
        var result1 = nullableObj.Equals(obj);
        var result2 = obj.Equals(nullableObj);

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [Test]
    public void EqualsObject_WithSameNonNullableObject_ShouldReturnTrue()
    {
        // Arrange
        object obj1 = new NullableObject<string>("test");
        object obj2 = new NullableObject<string>("test");

        // Act
        var result = obj1.Equals(obj2);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void EqualsObject_WithNonNullableAndNullObject_ShouldReturnFalse()
    {
        // Arrange
        object obj = new NullableObject<string>("test");
        object? nullObj = null;

        // Act
        var result = obj.Equals(nullObj);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void EqualsObject_WithNullableAndNonNullObject_ShouldReturnFalse()
    {
        // Arrange
        object obj = NullableObject<string>.Null;
        var nullObj = new object();

        // Act
        var result = obj.Equals(nullObj);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetHashCode_ShouldReturnSameHashCodeForEqualObjects()
    {
        // Arrange
        var obj1 = new NullableObject<string>("test");
        var obj2 = new NullableObject<string>("test");
        var nullObj1 = NullableObject<string>.Null;
        var nullObj2 = NullableObject<string>.Null;

        // Act & Assert
        obj1.GetHashCode().Should().Be(obj2.GetHashCode());
        nullObj1.GetHashCode().Should().Be(nullObj2.GetHashCode());
    }

    [Test]
    public void ImplicitConversion_ShouldConvertToAndFromNullableObject()
    {
        // Arrange & Act
        NullableObject<string> obj = "test";
        string? item = obj;
        NullableObject<string> nullObj = null;
        string? nullItem = nullObj;
        NullableObject<int> structObj = 5;
        int structItem = structObj;
        int? structNullItem = structObj;

        // Assert
        obj.Item.Should().Be("test");
        obj.IsNull.Should().BeFalse();
        item.Should().Be("test");
        nullObj.Item.Should().BeNull();
        nullObj.IsNull.Should().BeTrue();
        nullItem.Should().BeNull();
        structObj.Item.Should().Be(5);
        structObj.IsNull.Should().BeFalse();
        structItem.Should().Be(5);
        structNullItem.Should().Be(5);
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
        var obj3 = obj1;
        var obj4 = obj2;
        var nullObj1 = NullableObject<string>.Null;
        var nullObj2 = NullableObject<string>.Null;

        // Act & Assert
        (obj1 == obj2).Should().BeTrue();
        (obj2 == obj1).Should().BeTrue();
        (obj1 == obj3).Should().BeTrue();
        (obj3 == obj1).Should().BeTrue();
        (obj1 == obj4).Should().BeTrue();
        (obj4 == obj1).Should().BeTrue();
        (obj3 == obj4).Should().BeTrue();
        (obj4 == obj3).Should().BeTrue();
        (nullObj1 == nullObj2).Should().BeTrue();
        (nullObj2 == nullObj1).Should().BeTrue();
    }

    [Test]
    public void OperatorNotEquals_ShouldReturnTrueForDifferentObjects()
    {
        // Arrange
        var obj1 = new NullableObject<string>("test1");
        var obj2 = new NullableObject<string>("test2");
        var obj = new NullableObject<string>("test");
        var nullObj = NullableObject<string>.Null;

        // Act & Assert
        (obj1 != obj2).Should().BeTrue();
        (obj != nullObj).Should().BeTrue();
    }
}
