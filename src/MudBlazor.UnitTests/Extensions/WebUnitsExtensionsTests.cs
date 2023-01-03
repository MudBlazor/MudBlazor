// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{
    [TestFixture]
    public class WebUnitsExtensionsTests
    {
        [Test]
        public void All_ToPxMethods_Work()
        {
            0.0.ToPx().Should().Be("0px");
            3.3333.ToPx().Should().Be("3.33px");
            (-3.3333).ToPx().Should().Be("-3.33px");
            ((double?)3.3333).ToPx().Should().Be("3.33px");
            ((double?)null).ToPx().Should().Be(string.Empty);

            0.ToPx().Should().Be("0px");
            3.ToPx().Should().Be("3px");
            (-3).ToPx().Should().Be("-3px");
            ((int?)3).ToPx().Should().Be("3px");
            ((int?)null).ToPx().Should().Be(string.Empty);

            0L.ToPx().Should().Be("0px");
            3L.ToPx().Should().Be("3px");
            (-3L).ToPx().Should().Be("-3px");
            ((long?)3L).ToPx().Should().Be("3px");
            ((long?)null).ToPx().Should().Be(string.Empty);
        }
    }
}
