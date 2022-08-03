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

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class AlertTests : BunitTest
    {
        [Test]
        public async Task AlertTest()
        {
            var comp = Context.RenderComponent<MudAlert>(Parameter("Icon", Icons.Custom.Brands.MudBlazor));

            await comp.InvokeAsync(() => comp.Instance.OnCloseIconClickAsync());
            comp.WaitForAssertion(() => comp.Instance.Icon.Should().Be("<path d=\"M7.38,4.24c-.84,.11-2.17-.89-2.3-1.86s1-1.53,1.83-1.65,1.82,.19,1.94,1.16-.64,2.23-1.48,2.35Z\"/><path d=\"M22.61,8.61c-.47-1.24-1.13-2.41-1.96-3.45-.56-.7-1.2-1.34-1.91-1.88-.82-.63-1.73-1.2-2.73-1.44-4.54-1.12-4.18,2.13-6.51,3.12-2.33,.99-5.1,.07-6.59,2.11-.62,.85-1.24,1.76-1.63,2.75-.36,.91-.5,1.91-.43,2.88,.13,1.9,1.04,3.96,2.62,5.08,.56,.39,1.48,.89,2.12,.42,.31-.23,.5-.6,.64-.95,.41-1.02,.51-2.17,.58-3.25s-.14-4.6,.07-5.22c.16-.48,.56-.83,1.05-.96,.6-.17,1.37,.02,1.81,.47,.19,.19,.35,.43,.47,.7,.12,.27,.26,.61,.41,1.03l.9,2.52c.08,.21,.16,.43,.25,.65,.09,.22,.18,.43,.28,.61,.1,.18,.21,.33,.32,.45,.11,.12,.23,.17,.34,.17,.1,0,.19-.05,.29-.15,.1-.1,.19-.24,.28-.41,.09-.17,.18-.36,.28-.59s.18-.46,.27-.71l.97-2.62c.14-.37,.26-.69,.38-.96,.12-.27,.26-.49,.42-.67,.16-.17,.35-.3,.57-.39,.22-.09,.51-.13,.86-.13,.27,0,.5,.03,.7,.08s.36,.16,.49,.31c.23,.29,.28,.74,.33,1.1,.11,.76,.03,1.55,.03,2.31v4.39c0,.38,.02,.76,0,1.13-.02,.32-.01,.65-.17,.95-.14,.27-.41,.48-.71,.52-.39,.05-.7-.25-.85-.59-.07-.16-.11-.32-.14-.48-.02-.16-.04-.29-.04-.38,0,0,.16-4.3-.54-4.21-.14,.02-.47,.8-.56,.99-.49,.97-1.73,3.71-1.97,4.06-.19,.28-.46,.58-.81,.64-.24,.05-.48-.04-.67-.2-.47-.38-.68-1.08-.9-1.63-.18-.45-1.36-5-2.23-5.12-.12-.02-.2,.2-.25,.59-.05,.4-.25,7.35,1.72,9.31,2.81,2.79,7.86,2.55,11.62-3.17,1.83-2.79,1.66-6.81,.54-9.77Z\"/>"));
        }

        [Test]
        public void AlertTest2()
        {
            var comp = Context.RenderComponent<MudAlert>();
            try
            {
                comp.SetParam("Severity", "abc");
            }
            catch (Exception ex)
            {
                ex.Message.Should().Be("Unable to set property 'Severity' on object of type 'MudBlazor.MudAlert'. The error was: Unable to cast object of type 'System.String' to type 'MudBlazor.Severity'.");
            }
        }

        [Test]
        public async Task AlertTest_Click()
        {
            var comp = Context.RenderComponent<AlertClickTest>();
            var alert = comp.FindComponent<MudAlert>();
            var numeric = comp.FindComponent<MudNumericField<int>>();
            comp.WaitForAssertion(() => numeric.Instance.Value.Should().Be(0));
            await comp.InvokeAsync(() => alert.Instance.OnCloseIconClickAsync());
            comp.WaitForAssertion(() => numeric.Instance.Value.Should().Be(1));
        }

    }
}
