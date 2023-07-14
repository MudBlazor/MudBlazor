// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using System;
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
            Assert.True(property1.IsBodyMemberExpression);
            Assert.True(property2.IsBodyMemberExpression);
            Assert.AreEqual("Name", property1.ToString());
            Assert.AreEqual("Manager.Name", property2.ToString());
            Assert.AreEqual("Name", property1.GetPath());
            Assert.AreEqual("Manager.Name", property2.GetPath());
            Assert.AreEqual("Name", property1.GetLastMemberName());
            Assert.AreEqual("Name", property2.GetLastMemberName());
            Assert.AreEqual(1, property1.GetMembers().Count);
            Assert.AreEqual(2, property2.GetMembers().Count);
        }

        [Test]
        public void PropertyPathTests_Visit_Invalid_Expression()
        {
            // Arrange
            Expression<Func<Employee, string>> exp = x => new Employee() + "";

            // Act
            var property = PropertyPath.Visit(exp);

            // Assert
            Assert.False(property.IsBodyMemberExpression);
            Assert.AreEqual("", property.ToString());
            Assert.AreEqual("", property.GetPath());
            Assert.AreEqual("", property.GetLastMemberName());
            Assert.AreEqual(0, property.GetMembers().Count);
        }
    }
}
