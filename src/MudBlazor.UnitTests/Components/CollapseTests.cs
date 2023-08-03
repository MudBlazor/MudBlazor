// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
using static MudBlazor.MudCollapse;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class Collapseests : BunitTest
    {
        [Test]
        public async Task Collapse_Test1()
        {
            //TO DO for %100 coverage we need js test
            var comp = Context.RenderComponent<MudCollapse>(Parameter("MaxHeight", 1600));

            _ = comp.Instance._state = CollapseState.Exiting;
            await comp.InvokeAsync(() => comp.Instance.AnimationEndAsync());
            comp.WaitForAssertion(() => comp.Instance._height.Should().Be(0));

            //MaxHeight acceptes minus value?
            _ = comp.Instance._state = CollapseState.Entering;
#pragma warning disable BL0005
            await comp.InvokeAsync(() => comp.Instance.MaxHeight = -1);
            await comp.InvokeAsync(() => comp.Instance.AnimationEndAsync());
            await comp.InvokeAsync(() => comp.Instance.UpdateHeight());
            comp.WaitForAssertion(() => comp.Instance._height.Should().Be(-1));
        }
    }
}
