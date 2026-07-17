// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

#nullable enable

namespace MudBlazor.UnitTests.Other
{
    [TestFixture]
    public class GroupKeyPathTests
    {
        [Test]
        public void Equals_SameReference_ReturnsTrue()
        {
            var keys = new GroupKeyPath(["A", "B", 1, null]);
            Assert.That(keys.Equals(keys), Is.True);
        }

        [Test]
        public void Equals_IdenticalContent_ReturnsTrue()
        {
            var keys1 = new GroupKeyPath(["A", 1, null]);
            var keys2 = new GroupKeyPath(["A", 1, null]);
            Assert.That(keys1.Equals(keys2), Is.True);
            Assert.That(keys2.Equals(keys1), Is.True);
            Assert.That(keys1.GetHashCode(), Is.EqualTo(keys2.GetHashCode()));
        }

        [Test]
        public void Equals_DifferentCounts_ReturnsFalse()
        {
            var keys1 = new GroupKeyPath(["A", 1]);
            var keys2 = new GroupKeyPath(["A", 1, null]);
            Assert.That(keys1.Equals(keys2), Is.False);
            Assert.That(keys2.Equals(keys1), Is.False);
        }

        [Test]
        public void Equals_DifferentElements_ReturnsFalse()
        {
            var keys1 = new GroupKeyPath(["A", 1, null]);
            var keys2 = new GroupKeyPath(["B", 1, null]);
            Assert.That(keys1.Equals(keys2), Is.False);
            Assert.That(keys2.Equals(keys1), Is.False);
        }

        [Test]
        public void Equals_DifferentOrder_ReturnsFalse()
        {
            var keys1 = new GroupKeyPath(["A", 2]);
            var keys2 = new GroupKeyPath([2, "A"]);
            Assert.That(keys1.Equals(keys2), Is.False);
            Assert.That(keys2.Equals(keys1), Is.False);
        }

        [Test]
        public void Equals_ComparedToOtherType_ReturnsFalse()
        {
            var keys = new GroupKeyPath(["A", 1]);
            Assert.That(keys.Equals("not a keys collection"), Is.False);
            Assert.That(keys.Equals(null), Is.False);
        }

        [Test]
        public void GetHashCode_DifferentObjects_ReturnsDifferentHash()
        {
            var keys1 = new GroupKeyPath(["A", 1, null]);
            var keys2 = new GroupKeyPath(["A", 2, null]);
            Assert.That(keys1.GetHashCode(), Is.Not.EqualTo(keys2.GetHashCode()));
        }

        [Test]
        public void Equals_BothEmpty_ReturnsTrue()
        {
            // MudDataGrid creates root group definitions with an empty path, so empty paths must compare equal.
            var keys1 = new GroupKeyPath([]);
            var keys2 = new GroupKeyPath([]);
            Assert.That(keys1.Equals(keys2), Is.True);
            Assert.That(keys1.GetHashCode(), Is.EqualTo(keys2.GetHashCode()));
        }

        [Test]
        public void UsedAsDictionaryKey_EqualContentDistinctInstance_FindsValue()
        {
            // Equal-content paths from distinct instances must collide correctly so hash-based lookups succeed.
            var dictionary = new Dictionary<GroupKeyPath, bool>
            {
                [new GroupKeyPath(["A", 1, null])] = true
            };
            var lookup = new GroupKeyPath(["A", 1, null]);
            Assert.That(dictionary.TryGetValue(lookup, out var value), Is.True);
            Assert.That(value, Is.True);
            Assert.That(dictionary.ContainsKey(new GroupKeyPath(["A", 2, null])), Is.False);
        }
    }
}
