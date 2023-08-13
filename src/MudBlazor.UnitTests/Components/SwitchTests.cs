﻿using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Docs.Examples;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents.Switch;
using MudBlazor.UnitTests.Utilities;
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

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Checked.Should().Be(false));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Checked.Should().Be(true));

            comp.SetParam("Disabled", true);
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Checked.Should().Be(true));
        }

        [Test]
        [TestCase(Color.Default, Color.Primary)]
        [TestCase(Color.Primary, Color.Secondary)]
        [TestCase(Color.Secondary, Color.Info)]
        [TestCase(Color.Tertiary, Color.Success)]
        [TestCase(Color.Info, Color.Warning)]
        [TestCase(Color.Success, Color.Error)]
        [TestCase(Color.Warning, Color.Dark)]
        [TestCase(Color.Error, Color.Primary)]
        [TestCase(Color.Dark, Color.Primary)]
        public void SwitchColorTest(Color color, Color uncheckedcolor)
        {
            var comp = Context.RenderComponent<MudSwitch<bool>>(x => x.Add(c => c.Color, color).Add(b => b.UnCheckedColor, uncheckedcolor));

            var box = comp.Instance;
            var input = comp.Find("input");

            var checkboxClasses = comp.Find(".mud-button-root.mud-icon-button.mud-switch-base");
            // check initial state
            box.Checked.Should().Be(false);
            checkboxClasses.ClassList.Should().ContainInOrder(new[] { $"mud-{uncheckedcolor.ToDescriptionString()}-text", $"hover:mud-{uncheckedcolor.ToDescriptionString()}-hover" });

            // click and check if it has new color
            input.Change(true);
            box.Checked.Should().Be(true);
            checkboxClasses.ClassList.Should().ContainInOrder(new[] { $"mud-{color.ToDescriptionString()}-text", $"hover:mud-{color.ToDescriptionString()}-hover" });
        }

        [Test]
        public void SwitchDisabledTest()
        {
            var comp = Context.RenderComponent<SwitchWithLabelExample>();
            var switches = comp.FindAll("label.mud-switch");
            switches[3].ClassList.Should().Contain("mud-disabled"); // 4rd switch
        }

        [Test]
        public void SwitchLabelPositionTest()
        {
            var comp = Context.RenderComponent<SwitchWithLabelExample>();
            var switches = comp.FindAll("label.mud-switch");
    
            switches[0].ClassList.Should().NotContain("flex-row-reverse"); // 1st switch: (default) LabelPosition.End
            switches[2].ClassList.Should().Contain("flex-row-reverse"); // 3rd switch: LabelPosition.Start
        }

        [Test]
        public void SwitchLabelTest()
        {
            var value = new DisplayNameLabelClass();

            var comp = Context.RenderComponent<MudSwitch<bool>>(x => x.Add(f => f.For, () => value.Boolean));
            comp.Instance.Label.Should().Be("Boolean LabelAttribute"); //label should be set by the attribute

            var comp2 = Context.RenderComponent<MudSwitch<bool>>(x => x.Add(f => f.For, () => value.Boolean).Add(l => l.Label, "Label Parameter"));
            comp2.Instance.Label.Should().Be("Label Parameter"); //existing label should remain
        }

        [Test]
        public void SwitchLabelTextSizeTest()
        {
            var comp = Context.RenderComponent<MudSwitchTest>();
            var switches = comp.FindAll("label.mud-switch", true);
            var inputs = comp.FindAll("input");
            switches[3].Children[1].ClassList.Should().Contain("mud-switch-label-medium"); //4th switch doesn't have size set, it should be at default values
            switches[3].Children[0].ClassList.Should().Contain("mud-switch-span-medium");

            switches[4].Children[1].ClassList.Should().Contain("mud-switch-label-small"); //5th switch is a small switch with corresponding label text size
            switches[4].Children[0].ClassList.Should().Contain("mud-switch-span-small");

            switches[5].Children[1].ClassList.Should().Contain("mud-switch-label-medium"); //6th switch is a medium switch with corresponding label text size
            switches[5].Children[0].ClassList.Should().Contain("mud-switch-span-medium");

            switches[6].Children[1].ClassList.Should().Contain("mud-switch-label-large"); //7th switch is a large switch with corresponding label text size
            switches[6].Children[0].ClassList.Should().Contain("mud-switch-span-large");

            switches[7].Children[1].ClassList.Should().Contain("mud-switch-label-small"); //8th switch is a small switch that changes to large when unchecked
            switches[7].Children[0].ClassList.Should().Contain("mud-switch-span-small");

            // 8th switch Size is tied to the Label_Switch2 bool, if it's false, it should become large
            inputs[7].Change(false);
            switches[7].Children[1].ClassList.Should().Contain("mud-switch-label-large");
            switches[7].Children[0].ClassList.Should().Contain("mud-switch-span-large");
            


        }
    }
}
