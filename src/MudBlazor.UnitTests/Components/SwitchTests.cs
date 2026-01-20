using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.Switch;
using MudBlazor.UnitTests.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class SwitchTest : BunitTest
    {
        [Test]
        public async Task Switch_KeyboardNavigation()
        {
            var comp = Context.Render<MudSwitch<bool>>();

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(true));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Delete", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(false));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(true));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(false));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "NumpadEnter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(true));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(false));

            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(true));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));
            await comp.InvokeAsync(() => comp.Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.Instance.ReadValue.Should().Be(true));
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
        public async Task SwitchColor(Color color, Color uncheckedcolor)
        {
            var comp = Context.Render<MudSwitch<bool>>(x => x.Add(c => c.Color, color).Add(b => b.UncheckedColor, uncheckedcolor));

            var box = comp.Instance;

            var checkboxClasses = comp.Find(".mud-button-root.mud-icon-button.mud-switch-base");
            // check initial state
            box.ReadValue.Should().Be(false);
            checkboxClasses.ClassList.Should().ContainInOrder(new[] { $"mud-{uncheckedcolor.ToStringFast(true)}-text", $"hover:mud-{uncheckedcolor.ToStringFast(true)}-hover" });

            // click and check if it has new color
            await comp.Find("input").ChangeAsync(true);
            box.ReadValue.Should().Be(true);
            checkboxClasses.ClassList.Should().ContainInOrder(new[] { $"mud-{color.ToStringFast(true)}-text", $"hover:mud-{color.ToStringFast(true)}-hover" });
        }

        [Test]
        public void SwitchDisabled()
        {
            var comp = Context.Render<SwitchWithLabelTest>();
            comp.FindAll("label.mud-switch")[3].ClassList.Should().Contain("mud-disabled"); // 4rd switch
        }

        [Test]
        public void SwitchLabelPlacement()
        {
            var comp = Context.Render<SwitchWithLabelTest>();

            comp.FindAll("label.mud-switch")[0].ClassList.Should().Contain("mud-input-content-placement-end"); // 1st switch: (default) Placement.End
            comp.FindAll("label.mud-switch")[2].ClassList.Should().Contain("mud-input-content-placement-start"); // 3rd switch: Placement.Start
        }

        [Test]
        public void SwitchLabel()
        {
            var value = new DisplayNameLabelClass();

            var comp = Context.Render<MudSwitch<bool>>(x => x.Add(f => f.For, () => value.Boolean));
            comp.Instance.Label.Should().Be("Boolean LabelAttribute"); //label should be set by the attribute

            var comp2 = Context.Render<MudSwitch<bool>>(x => x.Add(f => f.For, () => value.Boolean).Add(l => l.Label, "Label Parameter"));
            comp2.Instance.Label.Should().Be("Label Parameter"); //existing label should remain
        }

        /// <summary>
        /// Optional Switch should not have required attribute should be false.
        /// </summary>
        [Test]
        public void OptionalSwitch_Should_NotHaveRequiredAttribute()
        {
            var comp = Context.Render<MudSwitch<bool>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();
        }

        /// <summary>
        /// Required Switch should have the required attribute.
        /// </summary>
        [Test]
        public void RequiredSwitch_Should_HaveRequiredAttribute()
        {
            var comp = Context.Render<MudSwitch<bool>>(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
        }

        /// <summary>
        /// Required Switch attribute should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredSwitchAttribute_Should_BeDynamic()
        {
            var comp = Context.Render<MudSwitch<bool>>();

            comp.Find("input").HasAttribute("required").Should().BeFalse();

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.Find("input").HasAttribute("required").Should().BeTrue();
        }

        [Test]
        public void ReadOnlyDisabled_ShouldNot_Hover()
        {
            Context.Render<MudSwitch<bool>>(self => self.Add(x => x.ReadOnly, false)).Find("span.mud-button-root").ClassList.Should().Contain("hover:mud-default-hover");
            Context.Render<MudSwitch<bool>>(self => self.Add(x => x.ReadOnly, true)).Find("span.mud-button-root").ClassList.Should().NotContain("hover:mud-default-hover");
            Context.Render<MudSwitch<bool>>(self => self.Add(x => x.ReadOnly, true).Add(x => x.Disabled, false)).Find("span.mud-button-root").ClassList.Should().NotContain("hover:mud-default-hover");
            Context.Render<MudSwitch<bool>>(self => self.Add(x => x.Disabled, false)).Find("span.mud-button-root").ClassList.Should().Contain("hover:mud-default-hover");
            Context.Render<MudSwitch<bool>>(self => self.Add(x => x.Disabled, true).Add(x => x.ReadOnly, false)).Find("span.mud-button-root").ClassList.Should().NotContain("hover:mud-default-hover");
            Context.Render<MudSwitch<bool>>(self => self.Add(x => x.Disabled, true).Add(x => x.ReadOnly, true)).Find("span.mud-button-root").ClassList.Should().NotContain("hover:mud-default-hover");
        }
    }
}
