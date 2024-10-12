// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

#nullable enable
[TestFixture]
public class TableExtensionsTests
{
    private readonly IReadOnlyList<TestItem> _testData =
    [
        new TestItem("B"),
        new TestItem("A"),
        new TestItem("C")
    ];

    [Test]
    public void OrderByDirection_ShouldSortAscending_ForIEnumerable()
    {
        // Arrange
        var direction = SortDirection.Ascending;
        string KeySelector(TestItem item) => item.Name;

        // Act
        var result = _testData.OrderByDirection(direction, KeySelector).ToList();

        // Assert
        result.Should().BeInAscendingOrder(item => item.Name);
    }

    [Test]
    public void OrderByDirection_ShouldSortDescending_ForIEnumerable()
    {
        // Arrange
        var direction = SortDirection.Descending;
        string KeySelector(TestItem item) => item.Name;

        // Act
        var result = _testData.OrderByDirection(direction, KeySelector).ToList();

        // Assert
        result.Should().BeInDescendingOrder(item => item.Name);
    }

    [Test]
    public void OrderByDirection_ShouldSortAscending_ForIQueryable()
    {
        // Arrange
        var direction = SortDirection.Ascending;
        Expression<Func<TestItem, string>> keySelector = item => item.Name;
        var queryableData = _testData.AsQueryable();

        // Act
        var result = queryableData.OrderByDirection(direction, keySelector).ToList();

        // Assert
        result.Should().BeInAscendingOrder(item => item.Name);
    }

    [Test]
    public void OrderByDirection_ShouldSortDescending_ForIQueryable()
    {
        // Arrange
        var direction = SortDirection.Descending;
        Expression<Func<TestItem, string>> keySelector = item => item.Name;
        var queryableData = _testData.AsQueryable();

        // Act
        var result = queryableData.OrderByDirection(direction, keySelector).ToList();

        // Assert
        result.Should().BeInDescendingOrder(item => item.Name);
    }

    private class TestItem(string name)
    {
        public string Name { get; } = name;
    }
}
