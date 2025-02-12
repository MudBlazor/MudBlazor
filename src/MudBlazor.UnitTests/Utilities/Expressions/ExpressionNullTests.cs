// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using FluentAssertions;
using MudBlazor.Utilities.Expressions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Expressions;

#nullable enable
[TestFixture]
public class ExpressionNullTests
{
    [Test]
    public void AddNullChecks_ShouldAddNullChecksToMemberExpression()
    {
        // Arrange
        Expression<Func<TestClass, string?>> expression = x => x.Property;
        var instance = new TestClass { Property = "Test" };

        // Act
        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        // Assert
        value.Should().Be("Test");
    }

    [Test]
    public void AddNullChecks_ShouldAddNullChecksToMethodCallExpression()
    {
        // Arrange
        Expression<Func<TestClass, string?>> expression = x => x.GetProperty();
        var instance = new TestClass { Property = "Test" };

        // Act
        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        // Assert
        value.Should().Be("Test");
    }

    [Test]
    public void AddNullChecks_ShouldHandleNonNullableValueType()
    {
        // Arrange
        Expression<Func<TestClass, int>> expression = x => x.ValueTypeProperty;
        var instance = new TestClass { ValueTypeProperty = 42 };

        // Act
        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        // Assert
        value.Should().Be(42);
    }

    [Test]
    public void AddNullChecks_ShouldHandleNullableValueType()
    {
        // Arrange
        Expression<Func<TestClass, int?>> expression = x => x.NullableValueTypeProperty;
        var instance = new TestClass { NullableValueTypeProperty = 42 };

        // Act
        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        // Assert
        value.Should().Be(42);
    }

    [Test]
    public void AddNullChecks_ShouldHandleNestedMemberExpressions()
    {
        // Arrange
        Expression<Func<TestClass, string?>> expression = x => x.Nested!.Property;
        var instance = new TestClass { Nested = new NestedClass { Property = "NestedTest" } };

        // Act
        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        // Assert
        value.Should().Be("NestedTest");
    }

    [Test]
    public void AddNullChecks_ShouldHandleNestedMethodCallExpressions()
    {
        // Arrange
        Expression<Func<TestClass, string?>> expression = x => x.Nested!.GetProperty();
        var instance = new TestClass { Nested = new NestedClass { Property = "NestedTest" } };

        // Act
        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        // Assert
        value.Should().Be("NestedTest");
    }

    [Test]
    public void AddNullChecks_ShouldReturnDefaultForNullNestedMember()
    {
        // Arrange
        Expression<Func<TestClass, string?>> expression = x => x.Nested!.Property;
        var instance = new TestClass { Nested = null };

        // Act
        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        // Assert
        value.Should().BeNull();
    }

    [Test]
    public void AddNullChecks_ShouldReturnDefaultForNullNestedMethodCall()
    {
        // Arrange
        Expression<Func<TestClass, string?>> expression = x => x.Nested!.GetProperty();
        var instance = new TestClass { Nested = null };

        // Act
        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        // Assert
        value.Should().BeNull();
    }

    private class TestClass
    {
        public string? Property { get; set; }

        public int ValueTypeProperty { get; set; }

        public int? NullableValueTypeProperty { get; set; }

        public NestedClass? Nested { get; set; }

        public string? GetProperty() => Property;
    }

    private class NestedClass
    {
        public string? Property { get; set; }

        public string? GetProperty() => Property;
    }
}
