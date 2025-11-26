// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents.ToolBar;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ToolBarTests : BunitTest
    {
        [Test]
        public void ToolBarWrapContentTest()
        {
            var component = Context.RenderComponent<ToolBarWrapContentTest>();
            var mudToolBar = component.Find(".mud-toolbar");

            mudToolBar.ClassList.Should().Contain("mud-toolbar-wrap-content");
        }

        /// <summary>
        /// ToolBar's WrapContent should be false by default
        /// </summary>
        [Test]
        public void ToolBar_WrapContent_ShouldBeFalseByDefault()
        {
            var comp = Context.RenderComponent<MudToolBar>();
            comp.Instance.WrapContent.Should().Be(false);
        }

    }
}
