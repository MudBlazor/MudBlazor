// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using AwesomeAssertions;
using MudBlazor.Utilities.Expressions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Expressions
{
    [TestFixture]
    public class PropertyPathTests
    {
        // ReSharper disable ClassNeverInstantiated.Local
        private class Employee
        {
            public string Name => string.Empty;

            public Manager Manager { get; } = new();
        }

        private class Manager
        {
            public string Name => string.Empty;

            // Value-typed member: boxing it to object forces the compiler to emit a Convert node.
            public int Level => 0;

            // No initializer: only the property metadata is read via expression trees,
            // never a runtime value, so a self-referential default would just stack-overflow.
            public Manager Boss { get; }
        }
        // ReSharper restore ClassNeverInstantiated.Local

        [Test]
        public void PropertyPathTests_Visit_Valid()
        {
            // Arrange
            Expression<Func<Employee, string>> exp1 = x => x.Name;
            Expression<Func<Employee, string>> exp2 = x => x.Manager.Name;

            // Act
            var property1 = PropertyPath.Visit(exp1);
            var property2 = PropertyPath.Visit(exp2);

            // Assert
            property1.IsBodyMemberExpression.Should().BeTrue();
            property2.IsBodyMemberExpression.Should().BeTrue();
            property1.ToString().Should().Be("Name");
            property2.ToString().Should().Be("Manager.Name");
            property1.GetPath().Should().Be("Name");
            property2.GetPath().Should().Be("Manager.Name");
            property1.GetLastMemberName().Should().Be("Name");
            property2.GetLastMemberName().Should().Be("Name");
            property1.GetMembers().Count.Should().Be(1);
            property2.GetMembers().Count.Should().Be(2);
        }

        [Test]
        public void PropertyPathTests_Visit_Invalid_Expression()
        {
            // Arrange
            Expression<Func<Employee, string>> exp = x => new Employee() + "";

            // Act
            var property = PropertyPath.Visit(exp);

            // Assert
            property.IsBodyMemberExpression.Should().BeFalse();
            property.ToString().Should().Be("");
            property.GetPath().Should().Be("");
            property.GetLastMemberName().Should().Be("");
            property.GetMembers().Count.Should().Be(0);
        }

        [Test]
        public void PropertyPathTests_Visit_DeepChain_PreservesSourceToLeafOrder()
        {
            // Arrange
            Expression<Func<Employee, string>> exp = x => x.Manager.Boss.Name;

            // Act
            var property = PropertyPath.Visit(exp);

            // Assert: VisitMember inserts at index 0 while walking outermost->innermost,
            // so members must end up in source-to-leaf order.
            property.IsBodyMemberExpression.Should().BeTrue();
            property.GetMembers().Select(m => m.Name).Should().Equal("Manager", "Boss", "Name");
            property.GetPath().Should().Be("Manager.Boss.Name");
            property.GetLastMemberName().Should().Be("Name");
        }

        [Test]
        public void PropertyPathTests_Visit_ConvertBody_NotBodyMemberButStillVisitsMembers()
        {
            // Arrange: boxing a value type to object wraps the member in a Convert node,
            // so the body is a UnaryExpression, not a MemberExpression, even though it
            // resolves to one. (A reference-type-to-object conversion would NOT add a
            // Convert node, leaving the body a MemberExpression.)
            Expression<Func<Employee, object>> exp = x => x.Manager.Level;

            // Act
            var property = PropertyPath.Visit(exp);

            // Assert: the flag reflects the body node type (Convert, not Member), but the
            // visitor still descends into the wrapped MemberExpression chain.
            property.IsBodyMemberExpression.Should().BeFalse();
            property.GetPath().Should().Be("Manager.Level");
            property.GetLastMemberName().Should().Be("Level");
            property.GetMembers().Count.Should().Be(2);
        }
    }
}
