
using System.Collections.Generic;
using FluentAssertions;
using MudBlazor.Docs.Extensions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class HashSetExtensionsTests
    {
        [Test]
        public void IsEqualToTest()
        {
            HashSet<int> h = null;
            h.IsEqualTo(null).Should().BeTrue();
            h.IsEqualTo(new HashSet<int>()).Should().BeFalse();
            h.IsEqualTo(new HashSet<int>() { 1, 17, 77 }).Should().BeFalse();
            new HashSet<int>().IsEqualTo(null).Should().BeFalse();
            new HashSet<int> { 1 }.IsEqualTo(null).Should().BeFalse();
            new HashSet<int> { 1 }.IsEqualTo(new HashSet<int>()).Should().BeFalse();
            new HashSet<int> { 1 }.IsEqualTo(new HashSet<int> { 1 }).Should().BeTrue();
            new HashSet<int> { }.IsEqualTo(new HashSet<int> { }).Should().BeTrue();
            new HashSet<int> { 1, 17, 77 }.IsEqualTo(new HashSet<int> { 77, 1, 17 }).Should().BeTrue();
            new HashSet<int> { 1, 17, 77 }.IsEqualTo(new HashSet<int> { 77, 0, 17 }).Should().BeFalse();
        }

    }
}
