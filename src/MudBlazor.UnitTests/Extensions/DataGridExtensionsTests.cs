// Copyright (c) MudBlazor 2025
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{
    [TestFixture]
    public class DataGridExtensionsTests
    {
        private class Person
        {
            public string Name { get; set; } = null!;
            public int Age { get; set; }
        }

        private static SortDefinition<Person> ByAge(bool descending = false)
        {
            return new SortDefinition<Person>("Age", descending, 0, p => p.Age);
        }

        private static SortDefinition<Person> ByName(bool descending = false)
        {
            return new SortDefinition<Person>("Name", descending, 0, p => p.Name);
        }

        [Test]
        public void OrderBy_ICollection_EmptySource_ReturnsEmpty()
        {
            var sortDefs = new List<SortDefinition<Person>> { ByAge() } as ICollection<SortDefinition<Person>>;
            var result = Array.Empty<Person>().OrderBySortDefinitions(sortDefs);
            result.Should().BeEmpty();
        }

        [Test]
        public void OrderBy_ICollection_NoDefinitions_ReturnsOriginalOrder()
        {
            var p1 = new Person { Name = "A", Age = 2 };
            var p2 = new Person { Name = "B", Age = 1 };
            var source = new[] { p1, p2 };
            ICollection<SortDefinition<Person>> sortDefs = new List<SortDefinition<Person>>();
            var result = source.OrderBySortDefinitions(sortDefs);
            result.Should().Equal(source);
        }

        [Test]
        public void OrderBy_ICollection_SingleAscending_SortsByAge()
        {
            var p1 = new Person { Name = "X", Age = 40 };
            var p2 = new Person { Name = "Y", Age = 20 };
            var p3 = new Person { Name = "Z", Age = 30 };
            var source = new[] { p1, p2, p3 };
            ICollection<SortDefinition<Person>> sortDefs = new List<SortDefinition<Person>> { ByAge() };
            var result = source.OrderBySortDefinitions(sortDefs).ToList();
            result.Select(x => x.Age).Should().ContainInOrder(20, 30, 40);
        }

        [Test]
        public void OrderBy_IReadOnlyCollection_SingleDescending_SortsByAgeDesc()
        {
            var p1 = new Person { Name = "X", Age = 40 };
            var p2 = new Person { Name = "Y", Age = 20 };
            var p3 = new Person { Name = "Z", Age = 30 };
            var source = new[] { p1, p2, p3 };
            IReadOnlyCollection<SortDefinition<Person>> sortDefs = new List<SortDefinition<Person>> { ByAge(true) };
            var result = source.OrderBySortDefinitions(sortDefs).ToList();
            result.Select(x => x.Age).Should().ContainInOrder(40, 30, 20);
        }

        [Test]
        public void OrderBy_ICollection_MultipleColumns_AgeThenName()
        {
            var p1 = new Person { Name = "B", Age = 20 };
            var p2 = new Person { Name = "A", Age = 20 };
            var p3 = new Person { Name = "C", Age = 10 };
            var source = new[] { p1, p2, p3 };
            ICollection<SortDefinition<Person>> sortDefs = new List<SortDefinition<Person>>
            {
                ByAge(),
                ByName()
            };
            var result = source.OrderBySortDefinitions(sortDefs).ToList();
            // Age 10 first, then age 20 sorted by Name A,B
            result.Select(x => (x.Age, x.Name))
                  .Should().ContainInOrder((10, "C"), (20, "A"), (20, "B"));
        }

        [Test]
        public void OrderBy_ICollection_MultipleColumns_ThenByDescendingOnSecondDefinition()
        {
            // This forces the 'else' branch + ThenByDescending
            var p1 = new Person { Name = "A", Age = 10 };
            var p2 = new Person { Name = "B", Age = 10 };
            var p3 = new Person { Name = "C", Age = 5 };
            var source = new[] { p1, p2, p3 };
            ICollection<SortDefinition<Person>> sortDefs = new List<SortDefinition<Person>>
            {
                ByAge(),         // first => OrderBy
                ByName(true)     // second => ThenByDescending
            };
            var result = source.OrderBySortDefinitions(sortDefs).ToList();
            // Age 5 first, then among Age 10 names in descending order B, A
            result.Select(x => x.Name)
                  .Should().ContainInOrder("C", "B", "A");
        }

        [Test]
        public void OrderBy_GridState_UsesItsSortDefinitions()
        {
            var p1 = new Person { Name = "Alpha", Age = 5 };
            var p2 = new Person { Name = "Beta", Age = 1 };
            var source = new[] { p1, p2 };
            var state = new GridState<Person> { SortDefinitions = new List<SortDefinition<Person>> { ByName(true) } };
            var result = source.OrderBySortDefinitions(state).ToList();
            result.Select(x => x.Name).Should().ContainInOrder("Beta", "Alpha");
        }

        [Test]
        public void OrderBy_GridStateVirtualize_UsesItsSortDefinitions()
        {
            var p1 = new Person { Name = "Alpha", Age = 5 };
            var p2 = new Person { Name = "Beta", Age = 1 };
            var source = new[] { p1, p2 };
            var vstate = new GridStateVirtualize<Person> { SortDefinitions = new List<SortDefinition<Person>> { ByName() } };
            var result = source.OrderBySortDefinitions(vstate).ToList();
            result.Select(x => x.Name).Should().ContainInOrder("Alpha", "Beta");
        }
    }
}
