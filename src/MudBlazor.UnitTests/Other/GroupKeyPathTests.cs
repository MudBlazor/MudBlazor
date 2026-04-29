using System.Threading.Tasks;
// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#nullable enable

namespace MudBlazor.UnitTests.Other
{
    public class GroupKeyPathTests
    {
        [Test]
        public async Task Equals_SameReference_ReturnsTrue()
        {
            var keys = new GroupKeyPath(["A", "B", 1, null]);
            await Assert.That(keys.Equals(keys)).IsTrue();
        }

        [Test]
        public async Task Equals_IdenticalContent_ReturnsTrue()
        {
            var keys1 = new GroupKeyPath(["A", 1, null]);
            var keys2 = new GroupKeyPath(["A", 1, null]);
            await Assert.That(keys1.Equals(keys2)).IsTrue();
            await Assert.That(keys2.Equals(keys1)).IsTrue();
            await Assert.That(keys1.GetHashCode()).IsEqualTo(keys2.GetHashCode());
        }

        [Test]
        public async Task Equals_DifferentCounts_ReturnsFalse()
        {
            var keys1 = new GroupKeyPath(["A", 1]);
            var keys2 = new GroupKeyPath(["A", 1, null]);
            await Assert.That(keys1.Equals(keys2)).IsFalse();
            await Assert.That(keys2.Equals(keys1)).IsFalse();
        }

        [Test]
        public async Task Equals_DifferentElements_ReturnsFalse()
        {
            var keys1 = new GroupKeyPath(["A", 1, null]);
            var keys2 = new GroupKeyPath(["B", 1, null]);
            await Assert.That(keys1.Equals(keys2)).IsFalse();
            await Assert.That(keys2.Equals(keys1)).IsFalse();
        }

        [Test]
        public async Task Equals_DifferentOrder_ReturnsFalse()
        {
            var keys1 = new GroupKeyPath(["A", 2]);
            var keys2 = new GroupKeyPath([2, "A"]);
            await Assert.That(keys1.Equals(keys2)).IsFalse();
            await Assert.That(keys2.Equals(keys1)).IsFalse();
        }

        [Test]
        public async Task Equals_ComparedToOtherType_ReturnsFalse()
        {
            var keys = new GroupKeyPath(["A", 1]);
            await Assert.That(keys.Equals("not a keys collection")).IsFalse();
            await Assert.That(keys.Equals(null)).IsFalse();
        }

        [Test]
        public async Task GetHashCode_EqualObjects_ReturnsSameHash()
        {
            var keys1 = new GroupKeyPath(["A", 1, null]);
            var keys2 = new GroupKeyPath(["A", 1, null]);
            await Assert.That(keys1.GetHashCode()).IsEqualTo(keys2.GetHashCode());
        }

        [Test]
        public async Task GetHashCode_DifferentObjects_ReturnsDifferentHash()
        {
            var keys1 = new GroupKeyPath(["A", 1, null]);
            var keys2 = new GroupKeyPath(["A", 2, null]);
            await Assert.That(keys1.GetHashCode()).IsNotEqualTo(keys2.GetHashCode());
        }
    }
}