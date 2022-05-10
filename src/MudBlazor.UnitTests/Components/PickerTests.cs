// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.Picker;
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
            

            await comp.InvokeAsync(() => picker.Instance.SelectAsync());
            await comp.InvokeAsync(() => picker.Instance.SelectRangeAsync(0, 0));
#pragma warning disable BL0005
            await comp.InvokeAsync(() => picker.Instance.Disabled = true);
            await comp.InvokeAsync(() => picker.Instance.HandleKeyDown(new KeyboardEventArgs()));
        }
    }
}
