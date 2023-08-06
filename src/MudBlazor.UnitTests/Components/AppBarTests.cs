// Copyright (c) MudBlazor 2023
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class AppBarTests : BunitTest
    {
        /// <summary>
        /// AppBar with modified Toolbar class
        /// </summary>
        [Test]
        public void AppBarWithModifiedToolBarClass()
        {
            var comp = Context.RenderComponent<MudAppBar>(Parameter(nameof(MudAppBar.ToolBarClass), "test-class"));

            // Find the Toolbar inside the AppBar
            comp.Find("div").ToMarkup()
                .Should()
                .Contain("test-class");
        }

        /// <summary>
        /// AppBar with <c>Bottom</c> not set.
        /// </summary>
        [Test]
        public void AppBarWithBottomUnset()
        {
            var bar = Context.RenderComponent<MudAppBar>();
            bar.Markup
               .Should()
               .StartWith("<header")
               .And
               .Contain("mud-appbar-fixed-top");
        }

        /// <summary>
        /// AppBar with <c>Bottom</c> set to <see langword="false" />.
        /// </summary>
        [Test]
        public void AppBarWithBottomSetFalse()
        {
            var bar = Context.RenderComponent<MudAppBar>(Parameter(nameof(MudAppBar.Bottom), false));
            bar.Markup
               .Should()
               .StartWith("<header")
               .And
               .Contain("mud-appbar-fixed-top");
        }

        /// <summary>
        /// AppBar with <c>Bottom</c> set to <see langword="true" />.
        /// </summary>
        [Test]
        public void AppBarWithBottomSetTrue()
        {
            var bar = Context.RenderComponent<MudAppBar>(Parameter(nameof(MudAppBar.Bottom), true));
            bar.Markup
               .Should()
               .StartWith("<footer")
               .And
               .Contain("mud-appbar-fixed-bottom");
        }

        /// <summary>
        /// AppBar must not set WrapContent true by default as this is not backwards compatible
        /// </summary>
        [Test]
        public void AppBar_WrapContent_ShouldBeFalseByDefault()
        {
            var comp = Context.RenderComponent<MudAppBar>();
            comp.FindComponent<MudToolBar>().Instance.WrapContent.Should().Be(false);
        }

    }
}
