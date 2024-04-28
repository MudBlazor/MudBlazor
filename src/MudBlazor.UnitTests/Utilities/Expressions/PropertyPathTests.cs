// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using FluentAssertions;
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
        }
        // ReSharper restore ClassNeverInstantiated.Local

        [Test]
        public void PropertyPathTests_Visit_Valid_Test()
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
    }
}
