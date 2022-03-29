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
            comp.WaitForAssertion(() => comp.Instance.Icon.Should().Be("<path d=\"M0.09,12.69V4.44l7.14,4.12v2.75L2.47,8.56v5.5L0.09,12.69z M7.23,8.56l7.14-4.12v8.24l-4.76,2.75l-2.38-1.37l4.76-2.75V8.56l-4.76,2.75V8.56z M7.23,14.06v2.75l4.76,2.75v-2.75L7.23,14.06z M11.99,19.56l7.14-4.12V4.45l-2.38,1.37v8.24l-4.76,2.75V19.56z M21.62,11.18l-2.38,1.37l2.27,1.32l2.38-1.37L21.62,11.18z M19.13,15.44l-2.38,1.37l4.76,2.75v-2.75L19.13,15.44z M19.13,4.45l-2.38,1.37l4.76,2.75l2.38-1.37L19.13,4.45z M23.89,7.19l-2.38,1.37v10.99l2.38-1.37V7.19z\"/>"));
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
