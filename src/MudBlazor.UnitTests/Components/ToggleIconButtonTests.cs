using AwesomeAssertions;
using Bunit;
using MudBlazor.UnitTests.TestComponents.ToggleIconButton;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class ToggleIconButtonTests : BunitTest
{
    /// <summary>
    /// Each click flips the toggled state and reports it through ToggledChanged.
    /// </summary>
    [Test]
    public async Task ClickTogglesAndRaisesToggledChanged()
    {
        var changes = new List<bool>();
        var comp = Context.Render<MudToggleIconButton>(parameters => parameters
            .Add(p => p.ToggledChanged, toggled => changes.Add(toggled)));

        await comp.Find("button").ClickAsync();
        await comp.Find("button").ClickAsync();

        changes.Should().Equal(true, false);
    }

    /// <summary>
    /// aria-pressed follows the toggled state so assistive technology announces it as a toggle button.
    /// </summary>
    [Test]
    public async Task AriaPressedFollowsToggledState()
    {
        var comp = Context.Render<MudToggleIconButton>();
        comp.Find("button").GetAttribute("aria-pressed").Should().Be("false");

        await comp.Find("button").ClickAsync();
        comp.Find("button").GetAttribute("aria-pressed").Should().Be("true");

        await comp.Find("button").ClickAsync();
        comp.Find("button").GetAttribute("aria-pressed").Should().Be("false");
    }

    /// <summary>
    /// A Toggled value supplied by the parent renders the toggled state without any click.
    /// </summary>
    [Test]
    public async Task ToggledParameterRendersToggledState()
    {
        var comp = Context.Render<MudToggleIconButton>(parameters => parameters
            .Add(p => p.Icon, Icons.Material.Filled.AlarmOff)
            .Add(p => p.ToggledIcon, Icons.Material.Filled.AlarmOn));

        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Toggled, true));

        comp.Find("button").GetAttribute("aria-pressed").Should().Be("true");
        comp.FindComponent<MudIconButton>().Instance.Icon.Should().Be(Icons.Material.Filled.AlarmOn);
    }

    /// <summary>
    /// Two buttons bound to the same value stay in sync when one of them is clicked.
    /// </summary>
    [Test]
    public async Task BoundButtonsStayInSync()
    {
        var comp = Context.Render<ToggleIconButtonTest1>();

        await comp.FindAll("button")[0].ClickAsync();

        comp.FindAll("button").Select(b => b.GetAttribute("aria-pressed")).Should().Equal("true", "true");
    }

    /// <summary>
    /// A disabled button ignores clicks and programmatic toggles and never raises ToggledChanged.
    /// </summary>
    [Test]
    public async Task DisabledIgnoresToggles()
    {
        var changes = new List<bool>();
        var comp = Context.Render<MudToggleIconButton>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ToggledChanged, toggled => changes.Add(toggled)));

        comp.Find("button").HasAttribute("disabled").Should().BeTrue();
        await comp.Find("button").ClickAsync();
        await comp.InvokeAsync(() => comp.Instance.SetToggledAsync(true));

        changes.Should().BeEmpty();
        comp.Find("button").GetAttribute("aria-pressed").Should().Be("false");
    }

    /// <summary>
    /// Once toggled, the button switches to the toggled icon, color, size, and variant.
    /// </summary>
    [Test]
    public async Task ToggledAppearanceReplacesAppearance()
    {
        var comp = Context.Render<MudToggleIconButton>(parameters => parameters
            .Add(p => p.Icon, Icons.Material.Filled.AlarmOff)
            .Add(p => p.Color, Color.Error)
            .Add(p => p.Size, Size.Small)
            .Add(p => p.Variant, Variant.Outlined)
            .Add(p => p.ToggledIcon, Icons.Material.Filled.AlarmOn)
            .Add(p => p.ToggledColor, Color.Success)
            .Add(p => p.ToggledSize, Size.Large)
            .Add(p => p.ToggledVariant, Variant.Filled));

        AssertRenderedAppearance(comp, Icons.Material.Filled.AlarmOff, Color.Error, Size.Small, Variant.Outlined);

        await comp.Find("button").ClickAsync();

        AssertRenderedAppearance(comp, Icons.Material.Filled.AlarmOn, Color.Success, Size.Large, Variant.Filled);
    }

    /// <summary>
    /// Toggled appearance parameters that are not set fall back to the untoggled ones.
    /// </summary>
    [Test]
    public async Task UnsetToggledAppearanceFallsBack()
    {
        var comp = Context.Render<MudToggleIconButton>(parameters => parameters
            .Add(p => p.Icon, Icons.Material.Filled.AlarmOff)
            .Add(p => p.Color, Color.Error)
            .Add(p => p.Size, Size.Small)
            .Add(p => p.Variant, Variant.Outlined));

        await comp.Find("button").ClickAsync();

        comp.Find("button").GetAttribute("aria-pressed").Should().Be("true");
        AssertRenderedAppearance(comp, Icons.Material.Filled.AlarmOff, Color.Error, Size.Small, Variant.Outlined);
    }

    /// <summary>
    /// Class, Style, and unmatched attributes such as aria-label reach the rendered button.
    /// </summary>
    [Test]
    public void ForwardsClassStyleAndAttributesToButton()
    {
        var comp = Context.Render<MudToggleIconButton>(parameters => parameters
            .Add(p => p.Class, "my-toggle")
            .Add(p => p.Style, "color:blue")
            .AddUnmatched("aria-label", "Alarm"));

        var button = comp.Find("button");
        button.ClassList.Should().Contain("my-toggle");
        button.GetAttribute("style").Should().Be("color:blue");
        button.GetAttribute("aria-label").Should().Be("Alarm");
    }

    /// <summary>
    /// Asserts the icon, color, size, and variant the toggle button hands to its inner icon button.
    /// </summary>
    private static void AssertRenderedAppearance(IRenderedComponent<MudToggleIconButton> comp, string icon, Color color, Size size, Variant variant)
    {
        var iconButton = comp.FindComponent<MudIconButton>().Instance;
        iconButton.Icon.Should().Be(icon);
        iconButton.Color.Should().Be(color);
        iconButton.Size.Should().Be(size);
        iconButton.Variant.Should().Be(variant);
    }
}
