// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.Picker;
using MudBlazor.UnitTests.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class PickerTests : BunitTest
    {
        [Test]
        public async Task PickerTest_Fundamentals()
        {
            var comp = Context.RenderComponent<SimplePickerTest>();
            var picker = comp.FindComponent<MudPicker<DateTime?>>();

            await comp.InvokeAsync(async () => await picker.Instance.SelectAsync());
            await comp.InvokeAsync(async () => await picker.Instance.SelectRangeAsync(0, 0));
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            await comp.InvokeAsync(() => picker.Instance.Disabled = true);
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
            await comp.InvokeAsync(() => picker.Instance.OnHandleKeyDownAsync(new KeyboardEventArgs()));
        }

        [Test]
        public void PickerLabelTest()
        {
            var value = new DisplayNameLabelClass();

            var comp = Context.RenderComponent<MudPicker<DateTime?>>(x => x.Add(f => f.For, () => value.Date));
            comp.Instance.Label.Should().Be("Date LabelAttribute"); //label should be set by the attribute

            var comp2 = Context.RenderComponent<MudPicker<DateTime?>>(x => x.Add(f => f.For, () => value.Date).Add(l => l.Label, "Label Parameter"));
            comp2.Instance.Label.Should().Be("Label Parameter"); //existing label should remain
        }

        [Test]
        public void PickerHasImmediateText()
        {
            var comp = Context.RenderComponent<MudPicker<DateTime?>>(ComponentParameter.CreateParameter("ImmediateText", true));
            comp.Instance.ImmediateText.Should().Be(true);
        }
    }
}
