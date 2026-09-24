using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.Button;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class FabMenuTests : BunitTest
{
    /// <summary>
    /// The menu starts closed with all of its items rendered.
    /// </summary>
    [Test]
    public void StartsClosedWithItemsRendered()
    {
        var comp = Context.Render<FabMenuTest>();

        comp.Find(".mud-fab-menu").ClassList.Should().NotContain("mud-fab-menu-open");
        comp.FindAll(".mud-fab-menu-item").Should().HaveCount(3);
    }

    /// <summary>
    /// Clicking the menu button opens the menu and raises OnClick, and clicking an item closes it.
    /// </summary>
    [Test]
    public async Task ButtonClickOpensAndItemClickCloses()
    {
        var clicks = 0;
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.OpenOnMouseHover, false)
            .Add(p => p.OnClick, () => clicks++)
            .AddChildContent<MudFabMenuItem>());

        await comp.Find(".mud-fab-menu-button").ClickAsync();
        IsOpen(comp).Should().BeTrue();
        clicks.Should().Be(1);

        await comp.Find(".mud-fab-menu-item").ClickAsync();
        IsOpen(comp).Should().BeFalse();
    }

    /// <summary>
    /// With CloseOnMenuItemClicked off, the menu stays open after an item is clicked.
    /// </summary>
    [Test]
    public async Task ItemClickKeepsMenuOpenWhenCloseOnMenuItemClickedIsOff()
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.OpenOnMouseHover, false)
            .Add(p => p.CloseOnMenuItemClicked, false)
            .AddChildContent<MudFabMenuItem>());

        await comp.Find(".mud-fab-menu-button").ClickAsync();
        await comp.Find(".mud-fab-menu-item").ClickAsync();

        IsOpen(comp).Should().BeTrue();
    }

    /// <summary>
    /// A disabled menu does not open when its button is clicked.
    /// </summary>
    [Test]
    public async Task DisabledMenuDoesNotOpenOnClick()
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.OpenOnMouseHover, false)
            .Add(p => p.Disabled, true));

        await comp.Find(".mud-fab-menu-button").ClickAsync();

        IsOpen(comp).Should().BeFalse();
    }

    /// <summary>
    /// Hovering a disabled menu does not open it.
    /// </summary>
    [Test]
    public async Task DisabledMenuDoesNotOpenOnHover()
    {
        var changes = new List<bool>();
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.OpenChanged, open => changes.Add(open)));

        await comp.Find(".mud-fab-menu-container").MouseEnterAsync(new MouseEventArgs());

        IsOpen(comp).Should().BeFalse();
        changes.Should().BeEmpty();
    }

    /// <summary>
    /// Hovering a menu disabled through a cascaded ParentDisabled value does not open it.
    /// </summary>
    [Test]
    public async Task ParentDisabledMenuDoesNotOpenOnHover()
    {
        var changes = new List<bool>();
        var comp = Context.Render<CascadingValue<bool>>(parameters => parameters
            .Add(p => p.Name, "ParentDisabled")
            .Add(p => p.Value, true)
            .AddChildContent<MudFabMenu>(menu => menu
                .Add(p => p.OpenChanged, open => changes.Add(open))));

        await comp.Find(".mud-fab-menu-container").MouseEnterAsync(new MouseEventArgs());

        IsOpen(comp).Should().BeFalse();
        changes.Should().BeEmpty();
    }

    /// <summary>
    /// An open menu closes when it becomes disabled, because its button can no longer toggle it.
    /// </summary>
    [Test]
    public async Task OpenMenuClosesWhenDisabled()
    {
        var changes = new List<bool>();
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.StartIcon, Icons.Material.Filled.Settings)
            .Add(p => p.OpenChanged, open => changes.Add(open)));

        await comp.Find(".mud-fab-menu-container").MouseEnterAsync(new MouseEventArgs());
        IsOpen(comp).Should().BeTrue();

        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Disabled, true));

        IsOpen(comp).Should().BeFalse();
        comp.FindComponent<MudFab>().Instance.StartIcon.Should().Be(Icons.Material.Filled.Settings);
        changes.Should().Equal(true, false);
    }

    /// <summary>
    /// A menu rendered both open and disabled starts closed and reports the close through OpenChanged.
    /// </summary>
    [Test]
    public void OpenAndDisabledMenuRendersClosed()
    {
        var changes = new List<bool>();
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Open, true)
            .Add(p => p.Disabled, true)
            .Add(p => p.StartIcon, Icons.Material.Filled.Settings)
            .Add(p => p.OpenChanged, open => changes.Add(open)));

        IsOpen(comp).Should().BeFalse();
        comp.FindComponent<MudFab>().Instance.StartIcon.Should().Be(Icons.Material.Filled.Settings);
        changes.Should().Equal(false);
    }

    /// <summary>
    /// Open renders the menu open, and user toggles are reported through OpenChanged.
    /// </summary>
    [Test]
    public async Task OpenIsTwoWayBindable()
    {
        var changes = new List<bool>();
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Open, true)
            .Add(p => p.OpenOnMouseHover, false)
            .Add(p => p.OpenChanged, open => changes.Add(open)));

        IsOpen(comp).Should().BeTrue();

        await comp.Find(".mud-fab-menu-button").ClickAsync();

        IsOpen(comp).Should().BeFalse();
        changes.Should().Equal(false);
    }

    /// <summary>
    /// While open, the menu button swaps its icons for the close icon unless UseCloseIcon is off, and restores them when closed.
    /// </summary>
    [TestCase(true, Icons.Material.Outlined.Add)]
    [TestCase(false, Icons.Material.Filled.Settings)]
    public async Task OpenMenuShowsCloseIcon(bool useCloseIcon, string expectedOpenIcon)
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.OpenOnMouseHover, false)
            .Add(p => p.UseCloseIcon, useCloseIcon)
            .Add(p => p.StartIcon, Icons.Material.Filled.Settings));

        await comp.Find(".mud-fab-menu-button").ClickAsync();
        comp.FindComponent<MudFab>().Instance.StartIcon.Should().Be(expectedOpenIcon);

        await comp.Find(".mud-fab-menu-button").ClickAsync();
        comp.FindComponent<MudFab>().Instance.StartIcon.Should().Be(Icons.Material.Filled.Settings);
    }

    /// <summary>
    /// With OpenOnMouseHover, entering the menu opens it, clicking an item closes it, and leaving closes it.
    /// </summary>
    [Test]
    public async Task HoverOpensAndLeaveCloses()
    {
        var comp = Context.Render<FabMenuTest>(parameters => parameters.Add(p => p.OpenOnMouseHover, true));

        await comp.Find(".mud-fab-menu-container").MouseEnterAsync(new MouseEventArgs());
        IsOpen(comp).Should().BeTrue();

        await comp.FindAll(".mud-fab-menu-item")[0].ClickAsync();
        IsOpen(comp).Should().BeFalse();

        await comp.Find(".mud-fab-menu-container").MouseEnterAsync(new MouseEventArgs());
        IsOpen(comp).Should().BeTrue();

        await comp.Find(".mud-fab-menu-container").MouseLeaveAsync(new MouseEventArgs());
        IsOpen(comp).Should().BeFalse();
    }

    /// <summary>
    /// With OpenOnMouseHover off, hovering neither opens nor closes the menu.
    /// </summary>
    [Test]
    public async Task HoverIsIgnoredWhenOpenOnMouseHoverIsOff()
    {
        var comp = Context.Render<FabMenuTest>(parameters => parameters.Add(p => p.OpenOnMouseHover, false));

        await comp.Find(".mud-fab-menu-container").MouseEnterAsync(new MouseEventArgs());
        IsOpen(comp).Should().BeFalse();

        await comp.Find(".mud-fab-menu-button").ClickAsync();
        await comp.Find(".mud-fab-menu-container").MouseLeaveAsync(new MouseEventArgs());
        IsOpen(comp).Should().BeTrue();
    }

    /// <summary>
    /// A tap fires touchstart, then an emulated mouseenter, then click.
    /// The emulated mouseenter must not open the menu, or the click that follows would close it again.
    /// </summary>
    [Test]
    public async Task TapOpensMenuDespiteEmulatedMouseEnter()
    {
        var comp = Context.Render<FabMenuTest>(parameters => parameters.Add(p => p.OpenOnMouseHover, true));

        await comp.Find(".mud-fab-menu-container").TouchStartAsync(new TouchEventArgs());
        await comp.Find(".mud-fab-menu-container").MouseEnterAsync(new MouseEventArgs());
        IsOpen(comp).Should().BeFalse();

        await comp.Find(".mud-fab-menu-button").ClickAsync();
        IsOpen(comp).Should().BeTrue();
    }

    /// <summary>
    /// Direction sets the class that lays the items out on that side of the button.
    /// </summary>
    [TestCase(Direction.Top, "mud-fab-menu-direction-top")]
    [TestCase(Direction.Bottom, "mud-fab-menu-direction-bottom")]
    [TestCase(Direction.Left, "mud-fab-menu-direction-left")]
    [TestCase(Direction.Right, "mud-fab-menu-direction-right")]
    [TestCase(Direction.Start, "mud-fab-menu-direction-start")]
    [TestCase(Direction.End, "mud-fab-menu-direction-end")]
    public void DirectionSetsClass(Direction direction, string expectedClass)
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Direction, direction));

        comp.Find(".mud-fab-menu").ClassList.Should().Contain(expectedClass);
    }

    /// <summary>
    /// A fixed menu is pinned to the viewport corner or edge named by Anchor.
    /// </summary>
    [TestCase(Origin.TopLeft, "mud-fab-anchor-top-left")]
    [TestCase(Origin.TopCenter, "mud-fab-anchor-top-center")]
    [TestCase(Origin.TopRight, "mud-fab-anchor-top-right")]
    [TestCase(Origin.CenterLeft, "mud-fab-anchor-center-left")]
    [TestCase(Origin.CenterCenter, "mud-fab-anchor-center-center")]
    [TestCase(Origin.CenterRight, "mud-fab-anchor-center-right")]
    [TestCase(Origin.BottomLeft, "mud-fab-anchor-bottom-left")]
    [TestCase(Origin.BottomCenter, "mud-fab-anchor-bottom-center")]
    [TestCase(Origin.BottomRight, "mud-fab-anchor-bottom-right")]
    public void FixedMenuSetsAnchorClass(Origin anchor, string expectedClass)
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Fixed, true)
            .Add(p => p.Anchor, anchor));

        comp.Find(".mud-fab-menu-container").ClassList.Should().Contain(["fixed", expectedClass]);
    }

    /// <summary>
    /// A menu that is not fixed ignores Anchor and stays in the document flow.
    /// </summary>
    [Test]
    public void NonFixedMenuIgnoresAnchor()
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Anchor, Origin.TopLeft));

        comp.Find(".mud-fab-menu-container").ClassList.Should().NotContain(["fixed", "mud-fab-anchor-top-left"]);
    }

    /// <summary>
    /// The menu forwards its Variant to the button that opens it.
    /// </summary>
    [TestCase(Variant.Filled, "mud-fab-filled")]
    [TestCase(Variant.Outlined, "mud-fab-outlined")]
    [TestCase(Variant.Text, "mud-fab-text")]
    public void VariantAppliesToMenuButton(Variant variant, string expectedClass)
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Variant, variant)
            .Add(p => p.StartIcon, Icons.Material.Filled.Add));

        comp.Find(".mud-fab-menu-button").ClassList.Should().Contain(expectedClass);
    }

    /// <summary>
    /// An item without its own Variant inherits the menu's Variant.
    /// </summary>
    [TestCase(Variant.Filled, "mud-fab-filled")]
    [TestCase(Variant.Outlined, "mud-fab-outlined")]
    [TestCase(Variant.Text, "mud-fab-text")]
    public void ItemInheritsMenuVariant(Variant menuVariant, string expectedClass)
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Variant, menuVariant)
            .AddChildContent<MudFabMenuItem>(item => item
                .Add(p => p.StartIcon, Icons.Material.Filled.Edit)));

        ItemVariantClasses(comp).Should().Equal(expectedClass);
    }

    /// <summary>
    /// An item's own Variant wins over the menu's, including when it is set to the default Filled.
    /// </summary>
    [TestCase(Variant.Outlined, Variant.Filled, "mud-fab-filled")]
    [TestCase(Variant.Text, Variant.Outlined, "mud-fab-outlined")]
    [TestCase(Variant.Filled, Variant.Text, "mud-fab-text")]
    public void ItemVariantOverridesMenuVariant(Variant menuVariant, Variant itemVariant, string expectedClass)
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Variant, menuVariant)
            .AddChildContent<MudFabMenuItem>(item => item
                .Add(p => p.Variant, itemVariant)
                .Add(p => p.StartIcon, Icons.Material.Filled.Edit)));

        ItemVariantClasses(comp).Should().Equal(expectedClass);
    }

    /// <summary>
    /// An item rendered outside a menu falls back to the filled variant.
    /// </summary>
    [Test]
    public void ItemOutsideMenuIsFilled()
    {
        var comp = Context.Render<MudFabMenuItem>(parameters => parameters
            .Add(p => p.StartIcon, Icons.Material.Filled.Edit));

        ItemVariantClasses(comp).Should().Equal("mud-fab-filled");
    }

    /// <summary>
    /// An item with Href renders a link with the menu item role and its link attributes, including the default rel for a new tab.
    /// </summary>
    [TestCase("_blank", null, "noopener")]
    [TestCase("_self", "nofollow", "nofollow")]
    public void ItemWithHrefRendersLink(string target, string rel, string expectedRel)
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .AddChildContent<MudFabMenuItem>(item => item
                .Add(p => p.Href, "https://example.com")
                .Add(p => p.Target, target)
                .Add(p => p.Rel, rel)
                .Add(p => p.Label, "Link")));

        var item = comp.Find(".mud-fab-menu-item");
        item.TagName.Should().Be("A");
        item.GetAttribute("role").Should().Be("menuitem");
        item.GetAttribute("href").Should().Be("https://example.com");
        item.GetAttribute("target").Should().Be(target);
        item.GetAttribute("rel").Should().Be(expectedRel);
    }

    /// <summary>
    /// Clearing the menu's Href turns its button back from a link into a button.
    /// </summary>
    [Test]
    public async Task ClearingHrefRendersMenuButtonAgain()
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.StartIcon, Icons.Material.Filled.Add)
            .Add(p => p.Href, "/docs"));

        comp.Find(".mud-fab-menu-button").TagName.Should().Be("A");

        await comp.SetParametersAndRenderAsync(parameters => parameters
            .Add(p => p.Href, (string)null));

        var button = comp.Find(".mud-fab-menu-button");
        button.TagName.Should().Be("BUTTON");
        button.HasAttribute("href").Should().BeFalse();
    }

    /// <summary>
    /// Enabling a disabled menu with Href restores its button as a link.
    /// </summary>
    [Test]
    public async Task EnablingDisabledMenuRestoresLink()
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.StartIcon, Icons.Material.Filled.Add)
            .Add(p => p.Href, "/docs")
            .Add(p => p.Disabled, true));

        comp.Find(".mud-fab-menu-button").TagName.Should().Be("BUTTON");

        await comp.SetParametersAndRenderAsync(parameters => parameters
            .Add(p => p.Disabled, false));

        var button = comp.Find(".mud-fab-menu-button");
        button.TagName.Should().Be("A");
        button.GetAttribute("href").Should().Be("/docs");
    }

    /// <summary>
    /// An item click runs the item's OnClick and reaches handlers outside the menu only when the menu's ClickPropagation is set.
    /// </summary>
    [TestCase(false, 0)]
    [TestCase(true, 1)]
    public async Task ItemClickPropagatesOnlyWhenAllowed(bool clickPropagation, int expectedContainerClicks)
    {
        var comp = Context.Render<FabMenuItemClickPropagationTest>(parameters => parameters.Add(p => p.ClickPropagation, clickPropagation));

        await comp.Find(".mud-fab-menu-item").ClickAsync();

        comp.Instance.ItemClickedCount.Should().Be(1);
        comp.Instance.ContainerClickedCount.Should().Be(expectedContainerClicks);
    }

    /// <summary>
    /// Returns whether the menu is showing its items.
    /// </summary>
    private static bool IsOpen<TComponent>(IRenderedComponent<TComponent> comp) where TComponent : IComponent
    {
        return comp.Find(".mud-fab-menu").ClassList.Contains("mud-fab-menu-open");
    }

    /// <summary>
    /// Returns the variant classes on the first menu item, so a test can check that exactly one variant applies.
    /// </summary>
    private static IEnumerable<string> ItemVariantClasses<TComponent>(IRenderedComponent<TComponent> comp) where TComponent : IComponent
    {
        return comp.Find(".mud-fab-menu-item").ClassList.Where(c => c is "mud-fab-filled" or "mud-fab-outlined" or "mud-fab-text").ToList();
    }
}
