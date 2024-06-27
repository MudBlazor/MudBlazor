using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Docs.Examples;
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
            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(true));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(false));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(true));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(false));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(true));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(false));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(true));

            comp.SetParam("Disabled", true);
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.Instance.Value.Should().Be(true));
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
            var comp = Context.RenderComponent<MudSwitch<bool>>(x => x.Add(c => c.Color, color).Add(b => b.UncheckedColor, uncheckedcolor));

            var box = comp.Instance;

            var checkboxClasses = comp.Find(".mud-button-root.mud-icon-button.mud-switch-base");
            // check initial state
            box.Value.Should().Be(false);
            checkboxClasses.ClassList.Should().ContainInOrder(new[] { $"mud-{uncheckedcolor.ToDescriptionString()}-text", $"hover:mud-{uncheckedcolor.ToDescriptionString()}-hover" });

            // click and check if it has new color
            comp.Find("input").Change(true);
            box.Value.Should().Be(true);
            checkboxClasses.ClassList.Should().ContainInOrder(new[] { $"mud-{color.ToDescriptionString()}-text", $"hover:mud-{color.ToDescriptionString()}-hover" });
        }

        [Test]
        public void SwitchDisabledTest()
        {
            var comp = Context.RenderComponent<SwitchWithLabelExample>();
            comp.FindAll("label.mud-switch")[3].ClassList.Should().Contain("mud-disabled"); // 4rd switch
        }

        [Test]
        public void SwitchLabelPositionTest()
        {
            var comp = Context.RenderComponent<SwitchWithLabelExample>();

            comp.FindAll("label.mud-switch")[0].ClassList.Should().NotContain("flex-row-reverse"); // 1st switch: (default) LabelPosition.End
            comp.FindAll("label.mud-switch")[2].ClassList.Should().Contain("flex-row-reverse"); // 3rd switch: LabelPosition.Start
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

            comp.FindAll("label.mud-switch", true)[3].Children[1].ClassList.Should().Contain("mud-switch-label-medium"); //4th switch doesn't have size set, it should be at default values
            comp.FindAll("label.mud-switch", true)[3].Children[0].ClassList.Should().Contain("mud-switch-span-medium");

            comp.FindAll("label.mud-switch", true)[4].Children[1].ClassList.Should().Contain("mud-switch-label-small"); //5th switch is a small switch with corresponding label text size
            comp.FindAll("label.mud-switch", true)[4].Children[0].ClassList.Should().Contain("mud-switch-span-small");

            comp.FindAll("label.mud-switch", true)[5].Children[1].ClassList.Should().Contain("mud-switch-label-medium"); //6th switch is a medium switch with corresponding label text size
            comp.FindAll("label.mud-switch", true)[5].Children[0].ClassList.Should().Contain("mud-switch-span-medium");

            comp.FindAll("label.mud-switch", true)[6].Children[1].ClassList.Should().Contain("mud-switch-label-large"); //7th switch is a large switch with corresponding label text size
            comp.FindAll("label.mud-switch", true)[6].Children[0].ClassList.Should().Contain("mud-switch-span-large");

            comp.FindAll("label.mud-switch", true)[7].Children[1].ClassList.Should().Contain("mud-switch-label-small"); //8th switch is a small switch that changes to large when unchecked
            comp.FindAll("label.mud-switch", true)[7].Children[0].ClassList.Should().Contain("mud-switch-span-small");

            // 8th switch Size is tied to the Label_Switch2 bool, if it's false, it should become large
            comp.FindAll("input")[7].Change(false);
            comp.FindAll("label.mud-switch", true)[7].Children[1].ClassList.Should().Contain("mud-switch-label-large");
            comp.FindAll("label.mud-switch", true)[7].Children[0].ClassList.Should().Contain("mud-switch-span-large");
        }

        /// <summary>
        /// Optional Switch should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalSwitch_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.RenderComponent<MudSwitch<bool>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");
        }

        /// <summary>
        /// Required Switch should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredSwitch_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.RenderComponent<MudSwitch<bool>>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }

        /// <summary>
        /// Required and aria-required Switch attributes should be dynamic.
        /// </summary>
        [Test]
        public void RequiredAndAriaRequiredSwitchAttributes_Should_BeDynamic()
        {
            var comp = Context.RenderComponent<MudSwitch<bool>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
            comp.Find("input").GetAttribute("aria-required").Should().Be("false");

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
            comp.Find("input").GetAttribute("aria-required").Should().Be("true");
        }


        [Test]
        public void ReadOnlyDisabled_ShouldNot_Hover()
        {
            Context.RenderComponent<MudSwitch<bool>>(self => self.Add(x => x.ReadOnly, false)).Find("span.mud-button-root").ClassList.Should().Contain("hover:mud-default-hover");
            Context.RenderComponent<MudSwitch<bool>>(self => self.Add(x => x.ReadOnly, true)).Find("span.mud-button-root").ClassList.Should().NotContain("hover:mud-default-hover");
            Context.RenderComponent<MudSwitch<bool>>(self => self.Add(x => x.ReadOnly, true).Add(x => x.Disabled, false)).Find("span.mud-button-root").ClassList.Should().NotContain("hover:mud-default-hover");
            Context.RenderComponent<MudSwitch<bool>>(self => self.Add(x => x.Disabled, false)).Find("span.mud-button-root").ClassList.Should().Contain("hover:mud-default-hover");
            Context.RenderComponent<MudSwitch<bool>>(self => self.Add(x => x.Disabled, true).Add(x => x.ReadOnly, false)).Find("span.mud-button-root").ClassList.Should().NotContain("hover:mud-default-hover");
            Context.RenderComponent<MudSwitch<bool>>(self => self.Add(x => x.Disabled, true).Add(x => x.ReadOnly, true)).Find("span.mud-button-root").ClassList.Should().NotContain("hover:mud-default-hover");
        }

        [Test]
        public void Switch_inputs_checked_value_should_match_parameter_value()
        {
            var comp = Context.RenderComponent<MudSwitch<bool>>();

            // change value using parameter
            comp.Instance.Value.Should().BeFalse();
            SwitchInput().HasAttribute("checked").Should().BeFalse();
            comp.SetParam(x => x.Value, true);
            comp.Instance.Value.Should().BeTrue();
            SwitchInput().HasAttribute("checked").Should().BeTrue();
            comp.SetParam(x => x.Value, false);
            comp.Instance.Value.Should().BeFalse();
            SwitchInput().HasAttribute("checked").Should().BeFalse();

            // change value using input
            SwitchInput().Change(true);
            comp.Instance.Value.Should().BeTrue();
            SwitchInput().HasAttribute("checked").Should().BeTrue();
            SwitchInput().Change(false);
            comp.Instance.Value.Should().BeFalse();
            SwitchInput().HasAttribute("checked").Should().BeFalse();
            return;

            IElement SwitchInput()
                => comp.Find("input");
        }

        [Test]
        public void Should_pass_value_binding_tests()
        {
            var comp = Context.RenderComponent<SwitchValueBindingTest>();

            // SwitchWithNoValueOrCallback should maintain correct internal state
            SwitchWithNoValueOrCallback().Change(true);
            SwitchWithNoValueOrCallback().HasAttribute("checked").Should().BeTrue();
            comp.Instance.LatestFormFieldValue.Should().BeTrue();
            SwitchWithNoValueOrCallback().Change(false);
            SwitchWithNoValueOrCallback().HasAttribute("checked").Should().BeFalse();
            comp.Instance.LatestFormFieldValue.Should().BeFalse();

            // SwitchWithValueButNoCallback should maintain correct internal state but not update the one-way bound value
            SwitchWithValueButNoCallback().Change(true);
            SwitchWithValueButNoCallback().HasAttribute("checked").Should().BeTrue();
            comp.Instance.LatestFormFieldValue.Should().BeTrue();
            comp.Instance.OneWayBoundValue.Should().BeFalse();
            SwitchWithValueButNoCallback().Change(false);
            SwitchWithValueButNoCallback().HasAttribute("checked").Should().BeFalse();
            comp.Instance.LatestFormFieldValue.Should().BeFalse();
            comp.Instance.OneWayBoundValue.Should().BeFalse();

            // SwitchWithNoValueButWithCallback should not change (empty callback prevents changes)
            SwitchWithNoValueButWithCallback().Change(true);
            SwitchWithNoValueButWithCallback().HasAttribute("checked").Should().BeFalse();
            comp.Instance.LatestFormFieldValue.Should().BeFalse();
            SwitchWithNoValueButWithCallback().Change(false);
            SwitchWithNoValueButWithCallback().HasAttribute("checked").Should().BeFalse();
            comp.Instance.LatestFormFieldValue.Should().BeFalse();

            // SwitchWithValueAndCallback should maintain the correct internal and external state
            SwitchWithValueAndCallback().Change(true);
            ConfirmDialogButton().Click();
            SwitchWithValueAndCallback().HasAttribute("checked").Should().BeTrue();
            comp.Instance.LatestFormFieldValue.Should().BeTrue();
            comp.Instance.OneWayBoundValueWithCallback.Should().BeTrue();
            SwitchWithValueAndCallback().Change(false);
            ConfirmDialogButton().Click();
            SwitchWithValueAndCallback().HasAttribute("checked").Should().BeFalse();
            comp.Instance.LatestFormFieldValue.Should().BeFalse();
            comp.Instance.OneWayBoundValueWithCallback.Should().BeFalse();

            // SwitchWithTwoWayBinding should maintain the correct internal and external state
            SwitchWithTwoWayBinding().Change(true);
            SwitchWithTwoWayBinding().HasAttribute("checked").Should().BeTrue();
            comp.Instance.LatestFormFieldValue.Should().BeTrue();
            comp.Instance.TwoWayBoundValue.Should().BeTrue();
            SwitchWithTwoWayBinding().Change(false);
            SwitchWithTwoWayBinding().HasAttribute("checked").Should().BeFalse();
            comp.Instance.LatestFormFieldValue.Should().BeFalse();
            comp.Instance.TwoWayBoundValue.Should().BeFalse();

            return;
            IElement SwitchWithNoValueOrCallback() => comp.Find("#no-value-and-no-callback");
            IElement SwitchWithValueButNoCallback() => comp.Find("#value-but-no-callback");
            IElement SwitchWithNoValueButWithCallback() => comp.Find("#no-value-but-callback");
            IElement SwitchWithValueAndCallback() => comp.Find("#value-and-callback");
            IElement SwitchWithTwoWayBinding() => comp.Find("#two-way-bound");
            IElement ConfirmDialogButton() => comp.Find("#submit-button");
        }
    }
}
