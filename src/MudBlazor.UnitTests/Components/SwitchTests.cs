
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
    public class SwitchTest : BunitTest
    {
        [Test]
        public async Task SwitchTest_KeyboardNavigation()
        {
            var comp = Context.RenderComponent<MudSwitch<bool>>();
            //Console.WriteLine(comp.Markup);

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Checked.Should().Be(true));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Checked.Should().Be(false));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Checked.Should().Be(true));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Checked.Should().Be(false));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Checked.Should().Be(true));

            comp.SetParam("Disabled", true);
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Checked.Should().Be(true));
        }

        [Test]
        public void SwitchTest_ReverseLabel()
        {
            var comp = Context.RenderComponent<MudSwitch<bool>>();
            //Console.WriteLine(comp.Markup);

            comp.Find(".mud-switch").ClassList.Should().NotContain("flex-row-reverse", "Reverse should not be applied");

            comp.SetParametersAndRender(ComponentParameter.CreateParameter("ReverseLabel", true));
            comp.Find(".mud-switch").ClassList.Should().Contain("flex-row-reverse", "Reverse should be applied");
        }
    }
}
