// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    public class SortingAssistentTests
    {

        public class ItemsWithOrder
        {
            public string Name { get; }
            public int Prio { get; set; }

            public ItemsWithOrder(string name, int priority)
            {
                Name = name;
                Prio = priority;
            }
        }

        [Test]
        public void UpdateOrder_MoveUp()
        {
            var items = GenerateList();

            //move item item-5 to index 0
            var dropInfo = new MudItemDropInfo<ItemsWithOrder>(items[4], "something", 0);
            items.UpdateOrder(dropInfo, x => x.Prio);

            var expectedOrders = new[] { 1, 2, 3, 4, 0, 5, 6, 7, 8, 9 };
            var actualOrders = items.Select(x => x.Prio).ToList();

            actualOrders.Should().ContainInOrder(expectedOrders);
        }

        [Test]
        public void UpdateOrder_MoveUp_Neighbor()
        {
            var items = GenerateList();

            //move item item-5 to index 0
            var dropInfo = new MudItemDropInfo<ItemsWithOrder>(items[3], "something", 4);
            items.UpdateOrder(dropInfo, x => x.Prio);

            var expectedOrders = new[] { 0, 1, 2, 4, 3, 5, 6, 7, 8, 9 };
            var actualOrders = items.Select(x => x.Prio).ToList();

            actualOrders.Should().ContainInOrder(expectedOrders);
        }

        [Test]
        public void UpdateOrder_MoveUp_EdgeCase()
        {
            var items = GenerateList();

            //move item item-5 to index 0
            var dropInfo = new MudItemDropInfo<ItemsWithOrder>(items[9], "something", 0);
            items.UpdateOrder(dropInfo, x => x.Prio);

            var expectedOrders = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            var actualOrders = items.Select(x => x.Prio).ToList();

            actualOrders.Should().ContainInOrder(expectedOrders);
        }

        [Test]
        public void UpdateOrder_MoveDown()
        {
            var items = GenerateList();

            //move item item-5 to index 0
            var dropInfo = new MudItemDropInfo<ItemsWithOrder>(items[2], "something", 5);
            items.UpdateOrder(dropInfo, x => x.Prio);

            var expectedOrders = new[] { 0, 1, 5, 2, 3, 4, 6, 7, 8, 9 };
            var actualOrders = items.Select(x => x.Prio).ToList();

            actualOrders.Should().ContainInOrder(expectedOrders);
        }

        [Test]
        public void UpdateOrder_MoveDown_Neighbor()
        {
            var items = GenerateList();

            //move item item-5 to index 0
            var dropInfo = new MudItemDropInfo<ItemsWithOrder>(items[4], "something", 3);
            items.UpdateOrder(dropInfo, x => x.Prio);

            var expectedOrders = new[] { 0, 1, 2, 4, 3, 5, 6, 7, 8, 9 };
            var actualOrders = items.Select(x => x.Prio).ToList();

            actualOrders.Should().ContainInOrder(expectedOrders);
        }

        [Test]
        public void UpdateOrder_MoveDown_EdgeCase()
        {
            var items = GenerateList();

            //move item item-5 to index 0
            var dropInfo = new MudItemDropInfo<ItemsWithOrder>(items[0], "something", 9);
            items.UpdateOrder(dropInfo, x => x.Prio);

            var expectedOrders = new[] { 9, 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            var actualOrders = items.Select(x => x.Prio).ToList();

            actualOrders.Should().ContainInOrder(expectedOrders);
        }

        [Test]
        public void UpdateOrder_MoveDown_SameItem()
        {
            var items = GenerateList();

            var dropInfo = new MudItemDropInfo<ItemsWithOrder>(items[3], "something", 3);
            items.UpdateOrder(dropInfo, x => x.Prio);

            var expectedOrders = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var actualOrders = items.Select(x => x.Prio).ToList();

            actualOrders.Should().ContainInOrder(expectedOrders);
        }

        private static List<ItemsWithOrder> GenerateList()
        {
            return new()
            {
                new ItemsWithOrder("Item-1", 0),
                new ItemsWithOrder("Item-2", 2),
                new ItemsWithOrder("Item-3", 3),
                new ItemsWithOrder("Item-4", 4),
                new ItemsWithOrder("Item-5", 5),
                new ItemsWithOrder("Item-6", 6),
                new ItemsWithOrder("Item-7", 7),
                new ItemsWithOrder("Item-8", 8),
                new ItemsWithOrder("Item-9", 9),
                new ItemsWithOrder("Item-10", 10),
            };
        }
    }
}
