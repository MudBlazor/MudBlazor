﻿
#pragma warning disable CS1998 // async without await

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class BadgeTests : BunitTest
    {
        [Test]
        public async Task Badge_Renders_Using_Default_Values()
        {
            var comp = Context.RenderComponent<MudBadge>();
            comp.FindAll("span").Should().HaveCount(3, "Default behavior of badge is to render 3 spans");

            await comp.InvokeAsync(() => comp.Instance.HandleBadgeClick(new MouseEventArgs()));
        }

        [Test]
        public async Task Badge_Renders_When_VisibleIsTrue()
        {
            var comp = Context.RenderComponent<MudBadge>();
            comp.SetParam("Visible", true);
            comp.FindAll("span").Should().HaveCount(3, "Visible badge renders 3 spans");
        }

        [Test]
        public async Task Badge_Does_Not_Render_When_VisibleIsFalse()
        {
            var comp = Context.RenderComponent<MudBadge>();
            comp.SetParam("Visible", false);
            comp.FindAll("span").Should().HaveCount(1, "Hidden badge renders 1 span");
        }

        [Test]
        public async Task BadgeTest_Click()
        {
            var comp = Context.RenderComponent<BadgeClickTest>();
            MudBadge badge() => comp.FindComponent<MudBadge>().Instance;
            MudNumericField<int> numeric() => comp.FindComponent<MudNumericField<int>>().Instance;
            comp.WaitForAssertion(() => numeric().Value.Should().Be(0));
            await comp.InvokeAsync(() => badge().HandleBadgeClick(new MouseEventArgs()));
            comp.WaitForAssertion(() => numeric().Value.Should().Be(1));
        }
    }
}
