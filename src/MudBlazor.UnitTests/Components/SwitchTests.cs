
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
        public async Task SwitchTest_CheckIcon()
        {
            var comp = Context.RenderComponent<MudSwitch<bool>>();
            Console.WriteLine(comp.Markup);

            comp.SetParam("ThumbIcon", Icons.Material.Filled.Done);
            comp.SetParam("ThumbIconOff", Icons.Material.Filled.Close);

            var iconElement = comp.FindComponent<MudIcon>().Instance;

            await comp.InvokeAsync(() => comp.Instance.GetThumbIcon());
            comp.WaitForAssertion(() => iconElement.Icon.Should().Be(Icons.Material.Filled.Close));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => iconElement.Icon.Should().Be(Icons.Material.Filled.Done));

            comp.SetParam("ThumbIconOff", null);
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.WaitForAssertion(() => iconElement.Icon.Should().Be(Icons.Material.Filled.Done));

            comp.SetParam("ThumbIcon", null);
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.ThumbIcon.Should().Be(null));
        }

        [Test]
        public async Task SwitchTest_KeyboardNavigation()
        {
            var comp = Context.RenderComponent<MudSwitch<bool>>();
            Console.WriteLine(comp.Markup);

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Checked.Should().Be(true));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
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
    }
}
