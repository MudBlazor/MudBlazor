// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using AwesomeAssertions;
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

    [Test]
    public void AddNullChecks_ShouldReturnDefaultWhenMidChainMemberIsNull()
    {
        // Deep chain where the intermediate (Nested.Deep) is null; every level must be guarded, not just the last.
        Expression<Func<TestClass, string?>> expression = x => x.Nested!.Deep!.Property;
        var instance = new TestClass { Nested = new NestedClass { Deep = null } };

        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        value.Should().BeNull();
    }

    [Test]
    public void AddNullChecks_ShouldResolveFullDeepChainWhenNothingIsNull()
    {
        Expression<Func<TestClass, string?>> expression = x => x.Nested!.Deep!.Property;
        var instance = new TestClass { Nested = new NestedClass { Deep = new DeepNestedClass { Property = "Deep" } } };

        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        value.Should().Be("Deep");
    }

    [Test]
    public void AddNullChecks_ShouldReturnZeroForNonNullableValueTypeBehindNullMember()
    {
        // The leaf is a non-nullable value type, the intermediate is null: must yield default(int) instead of throwing.
        Expression<Func<TestClass, int>> expression = x => x.Nested!.DeepValue;
        var instance = new TestClass { Nested = null };

        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        value.Should().Be(0);
    }

    [Test]
    public void AddNullChecks_ShouldReturnNullForNullableValueTypeWhenValueIsNull()
    {
        Expression<Func<TestClass, int?>> expression = x => x.NullableValueTypeProperty;
        var instance = new TestClass { NullableValueTypeProperty = null };

        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        value.Should().BeNull();
    }

    [Test]
    public void AddNullChecks_ShouldPreserveMethodCallArguments()
    {
        // Arguments on the guarded call must be carried over unchanged.
        Expression<Func<TestClass, string?>> expression = x => x.Echo("hello");
        var instance = new TestClass();

        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        value.Should().Be("hello");
    }

    [Test]
    public void AddNullChecks_ShouldReturnSameLambdaParameter()
    {
        // The transformed lambda must reuse the original parameter so the body stays bound.
        Expression<Func<TestClass, string?>> expression = x => x.Property;

        var result = ExpressionNull.AddNullChecks(expression);

        result.Parameters[0].Should().BeSameAs(expression.Parameters[0]);
    }

    [Test]
    public void AddNullChecks_ShouldPassThroughParameterOnlyBody()
    {
        // An identity body has no member/method to guard and is returned unchanged.
        Expression<Func<TestClass, TestClass>> expression = x => x;
        var instance = new TestClass { Property = "Self" };

        var result = ExpressionNull.AddNullChecks(expression);
        var compiled = result.Compile();
        var value = compiled(instance);

        result.Body.Should().BeSameAs(expression.Body);
        value.Should().BeSameAs(instance);
    }

    private class TestClass
    {
        public string? Property { get; set; }

        public int ValueTypeProperty { get; set; }

        public int? NullableValueTypeProperty { get; set; }

        public NestedClass? Nested { get; set; }

        public string? GetProperty() => Property;

        public string? Echo(string value) => value;
    }

    private class NestedClass
    {
        public string? Property { get; set; }

        public DeepNestedClass? Deep { get; set; }

        public int DeepValue { get; set; }

        public string? GetProperty() => Property;
    }

    private class DeepNestedClass
    {
        public string? Property { get; set; }
    }
}
