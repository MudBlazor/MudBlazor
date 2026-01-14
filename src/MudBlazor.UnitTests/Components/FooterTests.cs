// Copyright (c) MudBlazor 2026
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class FooterTests : BunitTest
    {
        [Test]
        public void FooterRendersFooterTagByDefault()
        {
            var footer = Context.Render<MudFooter>();

            footer.Markup
                .Should()
                .StartWith("<footer")
                .And
                .Contain("mud-footer");
        }

        [Test]
        public void FooterFixedAddsFixedClass()
        {
            var footer = Context.Render<MudFooter>(parameters => parameters.Add(x => x.Fixed, true));

            footer.Markup
                .Should()
                .Contain("mud-footer-fixed");
        }

        [Test]
        public void FooterStickyAddsStickyClass()
        {
            var footer = Context.Render<MudFooter>(parameters => parameters.Add(x => x.Sticky, true));

            footer.Markup
                .Should()
                .Contain("mud-footer-sticky");
        }

        [Test]
        public void FooterThrowsWhenFixedAndStickyAreBothTrue()
        {
            Action action = () => Context.Render<MudFooter>(parameters => parameters
                .Add(x => x.Fixed, true)
                .Add(x => x.Sticky, true));

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("*Fixed*Sticky*");
        }
    }
}
