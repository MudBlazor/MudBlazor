// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using MudBlazor.UnitTests.TestComponents.ToolBar;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ToolBarTests : BunitTest
    {
        [Test]
        public void ToolBarWrapContent()
        {
            var component = Context.Render<ToolBarWrapContentTest>();
            var mudToolBar = component.Find(".mud-toolbar");

            mudToolBar.ClassList.Should().Contain("mud-toolbar-wrap-content");
        }

        /// <summary>
        /// ToolBar's WrapContent should be false by default
        /// </summary>
        [Test]
        public void ToolBar_WrapContent_ShouldBeFalseByDefault()
        {
            var comp = Context.Render<MudToolBar>();
            comp.Instance.WrapContent.Should().Be(false);
        }
    }
}
