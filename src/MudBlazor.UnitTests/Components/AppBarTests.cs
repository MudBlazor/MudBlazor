// Copyright (c) MudBlazor 2023
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using MudBlazor.UnitTests.TestComponents.AppBar;
using NUnit.Framework;

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
            var comp = Context.Render<MudAppBar>(parameters => parameters.Add(x => x.ToolBarClass, "test-class"));

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
            var bar = Context.Render<MudAppBar>();
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
            var bar = Context.Render<MudAppBar>(parameters => parameters.Add(x => x.Bottom, false));
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
            var bar = Context.Render<MudAppBar>(parameters => parameters.Add(x => x.Bottom, true));
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
            var comp = Context.Render<MudAppBar>();
            comp.FindComponent<MudToolBar>().Instance.WrapContent.Should().Be(false);
        }

        [Test]
        public async Task AppBarWithContextualSetTrueAsync()
        {
            var comp = Context.Render<ContextualAppBarTest>();
            var bar = comp.FindComponent<MudAppBar>();

            bar.Markup.Should().Contain("regular-app-bar").And.Contain("mud-theme-primary");

            await comp.Find(".mud-switch-input").ChangeAsync(true);

            bar.Markup.Should().Contain("contextual-app-bar").And.Contain("mud-theme-tertiary");
        }

        [Test]
        public async Task AppBarWithContextualSetFalseAsync()
        {
            var comp = Context.Render<ContextualAppBarTest>(parameters => parameters.Add(x => x.IsContextual, false));
            var bar = comp.FindComponent<MudAppBar>();

            bar.Markup.Should().Contain("regular-app-bar").And.Contain("mud-theme-primary");

            await comp.Find(".mud-switch-input").ChangeAsync(true);

            bar.Markup.Should().Contain("regular-app-bar").And.Contain("mud-theme-primary");
        }
    }
}
