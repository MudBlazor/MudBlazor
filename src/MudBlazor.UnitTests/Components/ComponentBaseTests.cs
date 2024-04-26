// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests
{
    [TestFixture]
    public class ComponentBaseTests
    {
        [Test]
        public void MatchTypes()
        {
            new MudAvatar().MatchTypes(typeof(MudAvatar), typeof(MudButton)).Should().Be(true);
            new MudAvatar().MatchTypes(typeof(MudButton)).Should().Be(false);
            new MudList<string>().MatchTypes(typeof(MudList<>), typeof(MudListItem<>)).Should().Be(true);
            new MudList<int>().MatchTypes(typeof(MudList<>), typeof(MudListItem<>)).Should().Be(true);
        }
    }
}
