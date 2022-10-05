﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public class ChipTests : BunitTest
    {
        /// <summary>
        /// Clicks on the chip and tests if the OnClick event works
        /// </summary>
        [Test]
        public void Chip_OnClick_Test()
        {
            var comp = Context.RenderComponent<ChipOnClickTest>();
            // print the generated html
            //Console.WriteLine(comp.Markup);
            
            // chip should have mud-clickable and mud-ripple classes
            var chip = comp.Find("div.mud-chip");
            chip.ClassName.Should().Contain("mud-clickable");
            chip.ClassName.Should().Contain("mud-ripple");
            
            // click on chip
            chip.Click();

            var expectedEvent = comp.Find("#chip-click-test-expected-value");
            expectedEvent.InnerHtml.Should().Be("OnClick");
        }

        /// <summary>
        /// Clicks on the close button and tests if the OnClose event works
        /// </summary>
        [Test]
        public void Chip_OnClose_Test()
        {
            var comp = Context.RenderComponent<ChipOnClickTest>();
            // print the generated html
            //Console.WriteLine(comp.Markup);
            
            // chip should have mud-clickable and mud-ripple classes
            var chip = comp.Find("div.mud-chip");
            chip.ClassName.Should().Contain("mud-clickable");
            chip.ClassName.Should().Contain("mud-ripple");
            
            // click on close button
            comp.Find("button.mud-chip-close-button").Click();

            var expectedEvent = comp.Find("#chip-click-test-expected-value");
            expectedEvent.InnerHtml.Should().Be("OnClose");
        }

        [Test]
        public async Task Chip_Link_Test()
        {
            var comp = Context.RenderComponent<ChipLinkTest>();
            var chip = comp.FindComponent<MudChip>();
            
            await comp.InvokeAsync(() => chip.Instance.ForceRerender());
            await comp.InvokeAsync(() => chip.Instance.OnClickHandler(new MouseEventArgs()));

            comp.WaitForAssertion(() => comp.Find("#chip-click-test-expected-value").InnerHtml.Should().Be(""));
#pragma warning disable BL0005
            await comp.InvokeAsync(() => chip.Instance.Target = "_blank");
            await comp.InvokeAsync(() => chip.Instance.OnClickHandler(new MouseEventArgs()));

            comp.WaitForAssertion(() => comp.Find("#chip-click-test-expected-value").InnerHtml.Should().Be(""));
        }
    }
}
