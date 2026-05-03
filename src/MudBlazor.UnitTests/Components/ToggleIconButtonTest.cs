using AwesomeAssertions;
using Bunit;
using MudBlazor.UnitTests.TestComponents.ToggleIconButton;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ToggleIconButtonTest : BunitTest
    {
        [Test]
        public void DefaultState()
        {
            var comp = Context.Render<MudToggleIconButton>();
            comp.Instance.Toggled.Should().BeFalse();
            comp.Instance.Icon.Should().BeNull();
            comp.Instance.ToggledIcon.Should().BeNull();
            comp.Instance.Color.Should().Be(Color.Default);
            comp.Instance.ToggledColor.Should().BeNull();
            comp.Instance.Size.Should().Be(Size.Medium);
            comp.Instance.ToggledSize.Should().BeNull();
            comp.Instance.Variant.Should().Be(Variant.Text);
            comp.Instance.ToggledVariant.Should().BeNull();
            comp.Instance.Edge.Should().Be(Edge.False);
            comp.Instance.Ripple.Should().BeTrue();
            comp.Instance.DropShadow.Should().BeTrue();
            comp.Instance.Disabled.Should().BeFalse();
            comp.Instance.ClickPropagation.Should().BeFalse();
        }

        [Test]
        public async Task ShouldToggleOnClickAsync()
        {
            var boundValue = false;
            var comp = Context.Render<MudToggleIconButton>(parameters => parameters
                .Add(p => p.Toggled, boundValue)
                .Add(p => p.ToggledChanged, (toggleValue) => boundValue = toggleValue)
            );

            await comp.Find("button").ClickAsync();
            boundValue.Should().BeTrue();

            await comp.Find("button").ClickAsync();
            boundValue.Should().BeFalse();
        }

        [Test]
        public async Task ShouldSetAriaPressedAttributeAsync()
        {
            var comp = Context.Render<MudToggleIconButton>();

            comp.Find("button").GetAttribute("aria-pressed").Should().Be("false");

            await comp.Find("button").ClickAsync();
            comp.Find("button").GetAttribute("aria-pressed").Should().Be("true");

            await comp.Find("button").ClickAsync();
            comp.Find("button").GetAttribute("aria-pressed").Should().Be("false");
        }

        [Test]
        public async Task ShouldSynchronizeStateWithOtherComponentAsync()
        {
            var comp = Context.Render<ToggleIconButtonTest1>();
            // select elements needed for the test
            var group = comp.FindComponents<MudToggleIconButton>();
            var comp1 = group[0];
            var comp2 = group[1];
            // check initial state
            comp1.Instance.Toggled.Should().BeFalse();
            comp2.Instance.Toggled.Should().BeFalse();
            // click first button
            await comp1.Find("button").ClickAsync();
            // make sure both buttons state changed
            comp1.Instance.Toggled.Should().BeTrue();
            comp2.Instance.Toggled.Should().BeTrue();
        }

        [Test]
        public async Task Disabled_ShouldPreventInteractionAsync()
        {
            var comp = Context.Render<MudToggleIconButton>(parameters => parameters
                .Add(p => p.Disabled, true)
            );

            // Verify that the button is disabled
            var button = comp.Find("button");
            button.HasAttribute("disabled").Should().BeTrue();

            // Try to toggle the button
            await button.ClickAsync();

            // Verify that the toggled state has not changed
            comp.Instance.Toggled.Should().BeFalse();
        }

        [TestCase("icon-default", "icon-toggled", "icon-default", "icon-toggled")]
        [TestCase("icon-default", null, "icon-default", "icon-default")]
        public async Task GetIcon_ShouldReturnCorrectValueAsync(string icon, string toggledIcon, string expectedIcon, string expectedToggledIcon)
        {
            var comp = Context.Render<MudToggleIconButton>(parameters => parameters
                .Add(p => p.Icon, icon)
                .Add(p => p.ToggledIcon, toggledIcon)
            );

            // Check initial state
            comp.Instance.GetIcon().Should().Be(expectedIcon);

            // Toggle state
            await comp.Find("button").ClickAsync();

            // Check toggled state
            comp.Instance.GetIcon().Should().Be(expectedToggledIcon);
        }

        [TestCase(Size.Small, Size.Large, Size.Small, Size.Large)]
        [TestCase(Size.Small, null, Size.Small, Size.Small)]
        public async Task GetSize_ShouldReturnCorrectValueAsync(Size size, Size? toggledSize, Size expectedSize, Size expectedToggledSize)
        {
            var comp = Context.Render<MudToggleIconButton>(parameters => parameters
                .Add(p => p.Size, size)
                .Add(p => p.ToggledSize, toggledSize)
            );

            // Check initial state
            comp.Instance.GetSize().Should().Be(expectedSize);

            // Toggle state
            await comp.Find("button").ClickAsync();

            // Check toggled state
            comp.Instance.GetSize().Should().Be(expectedToggledSize);
        }

        [TestCase(Color.Tertiary, Color.Secondary, Color.Tertiary, Color.Secondary)]
        [TestCase(Color.Tertiary, null, Color.Tertiary, Color.Tertiary)]
        public async Task GetColor_ShouldReturnCorrectValueAsync(Color color, Color? toggledColor, Color expectedColor, Color expectedToggledColor)
        {
            var comp = Context.Render<MudToggleIconButton>(parameters => parameters
                .Add(p => p.Color, color)
                .Add(p => p.ToggledColor, toggledColor)
            );

            // Check initial state
            comp.Instance.GetColor().Should().Be(expectedColor);

            // Toggle state
            await comp.Find("button").ClickAsync();

            // Check toggled state
            comp.Instance.GetColor().Should().Be(expectedToggledColor);
        }

        [TestCase(Variant.Outlined, Variant.Filled, Variant.Outlined, Variant.Filled)]
        [TestCase(Variant.Outlined, null, Variant.Outlined, Variant.Outlined)]
        public async Task GetVariant_ShouldReturnCorrectValueAsync(Variant variant, Variant? toggledVariant, Variant expectedVariant, Variant expectedToggledVariant)
        {
            var comp = Context.Render<MudToggleIconButton>(parameters => parameters
                .Add(p => p.Variant, variant)
                .Add(p => p.ToggledVariant, toggledVariant)
            );

            // Check initial state
            comp.Instance.GetVariant().Should().Be(expectedVariant);

            // Toggle state
            await comp.Find("button").ClickAsync();

            // Check toggled state
            comp.Instance.GetVariant().Should().Be(expectedToggledVariant);
        }
    }
}
