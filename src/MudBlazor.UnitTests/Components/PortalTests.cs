#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.Interop;
using MudBlazor.Services;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class PortalTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        /// <summary>
        /// MudElement renders first an anchor and then a button
        /// </summary>
        [Test]
        public void Basic_Example_of_a_Portaled_Menu()
        {
            var comp = ctx.RenderComponent<PortalMenuTest>();
            var portalprovider = comp.Find("#mud-portal-container");
            //the portal provider is empty;
            var itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("0");

            //no portal is created yet
            comp.FindAll(".portal-anchor").Should().HaveCount(0);
            comp.FindAll(".portal").Should().HaveCount(0);

            //after click the button of the menu, the portal is created
            comp.Find("button").Click();
            comp.Find(".portal-anchor");
            comp.Find(".portal");

            //it has 2 popovers, one is what you see, the other is used to make the calculations to
            //know if it fits the viewport or it needs to be moved inside
            comp.FindAll(".mud-popover").Should().HaveCount(2);

            //The portal provider now has 1 menu inside
            itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("1");

            //should have 3 items
            comp.FindAll(".portal-anchor .mud-list-item").Should().HaveCount(3);

            //clicking in one of them, the popover dissapears
            comp.FindAll(".portal-anchor .mud-list-item")[0].Click();
            comp.FindAll(".mud-popover").Should().HaveCount(0);
        }

        /// <summary>
        /// MudElement renders first an anchor and then a button
        /// </summary>
        [Test]
        public void Basic_Example_of_a_Portaled_Select()
        {
            var comp = ctx.RenderComponent<PortalSelectTest>();
            var portalprovider = comp.Find("#mud-portal-container");
            //the portal provider is empty;
            var itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("0");

            //no portal is created yet
            comp.FindAll(".portal-anchor").Should().HaveCount(0);
            comp.FindAll(".portal").Should().HaveCount(0);

            //after click the button of the menu, the portal is created
            comp.Find("input").Click();
            comp.Find(".portal-anchor");
            comp.Find(".portal");

            //it has 2 popovers, one is what you see, the other is used to make the calculations to
            //know if it fits the viewport or it needs to be moved inside
            comp.FindAll(".mud-popover").Should().HaveCount(2);

            //The portal provider now has 1 menu inside
            itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("1");

            //should have 3 items
            comp.FindAll(".portal-anchor .mud-list-item").Should().HaveCount(4);

            //clicking in one of them, the popover dissapears
            comp.FindAll(".portal-anchor .mud-list-item")[0].Click();
            comp.FindAll(".mud-popover").Should().HaveCount(0);

            //The portal provider now has 0 items
            itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("0");

            //now, the select is closed, but if we set IsPreRendered to true, then
            //the portaled item is already present
            var isPrerrender = Parameter(nameof(MudSelect<string>.IsPrerrendered), true);
            comp.SetParametersAndRender(isPrerrender);

            //The portal provider now has 1 select inside, because is prerrendered
            itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("1");

            //has the 2 popovers, one to show, another to check position
            comp.FindAll(".mud-popover").Should().HaveCount(2);

            //and the list items
            comp.FindAll(".portal-anchor .mud-list-item").Should().HaveCount(4);
        }

        /// <summary>
        /// MudElement renders first an anchor and then a button
        /// </summary>
        [Test]
        public void Basic_Example_of_a_Portaled_Autocomplete()
        {
            var comp = ctx.RenderComponent<PortalAutocompleteTest>();
            var portalprovider = comp.Find("#mud-portal-container");
            //the portal provider is empty;
            var itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("0");

            //no portal is created yet
            comp.FindAll(".portal-anchor").Should().HaveCount(0);
            comp.FindAll(".portal").Should().HaveCount(0);

            //after click the button of the menu, the portal is created
            comp.Find("input").Input("Ala");
            comp.WaitForAssertion(() => comp.Find(".portal-anchor"));
            comp.Find(".portal-anchor");
            comp.Find(".portal");

            //it has 2 popovers, one is what you see, the other is used to make the calculations to
            //know if it fits the viewport or it needs to be moved inside
            comp.FindAll(".mud-popover").Should().HaveCount(2);

            //The portal provider now has 1 menu inside
            itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("1");

            //should have 3 items
            comp.FindAll(".portal-anchor .mud-list-item").Should().HaveCount(3);

            //clicking in one of them, the popover dissapears
            comp.FindAll(".portal-anchor .mud-list-item")[0].Click();
            comp.FindAll(".mud-popover").Should().HaveCount(0);

            //The portal provider now has 0 items
            itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("0");
        }

        /// <summary>
        /// MudElement renders first an anchor and then a button
        /// </summary>
        [Test]
        public async Task Basic_Example_of_a_Portaled_Tooltip()
        {
            var comp = ctx.RenderComponent<PortalTooltipTest>();
            var portalprovider = comp.Find("#mud-portal-container");
            //the portal provider is empty;
            var itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("0");

            //no portal is created yet
            comp.FindAll(".portal-anchor").Should().HaveCount(0);
            comp.FindAll(".portal").Should().HaveCount(0);

            //can't continue testing because Bunit doesn't implement onmouseenter or onmouseleave
            //and onmouseover doesn't trigger onmouseenter
            //TODO when Bunit implements custom events
        }

        private BoundingClientRect _anchorRect = new BoundingClientRect();
        private BoundingClientRect _fragmentRect = new BoundingClientRect();
        public BoundingClientRect FragmentRect
        {
            get
            {
                _fragmentRect.Left = _anchorRect.Left;
                _fragmentRect.Top = _anchorRect.Top;
                _fragmentRect.Width = _anchorRect.Width;
                _fragmentRect.Height = 300;
                _fragmentRect.WindowHeight = _anchorRect.WindowHeight;
                _fragmentRect.WindowWidth = _anchorRect.WindowWidth;
                return _fragmentRect;
            }
        }

        /// <summary>
        /// In the repositioning of outside of the viewport portal items,
        /// we only correct the anchor, that is the element the fragment is anchored
        /// </summary>
        [Test]
        public void Portal_Repositioning_Correct_Boundaries()
        {
            //configure objects

            var portalItem = new PortalItem();
            portalItem.AnchorRect = _anchorRect;
            portalItem.FragmentRect = FragmentRect;

            //set values to anchorrect
            _anchorRect.Left = 0;
            _anchorRect.Top = 700;
            _anchorRect.Width = 100;
            _anchorRect.Height = 60;
            _anchorRect.WindowHeight = 800;
            _anchorRect.WindowWidth = 800;
            _anchorRect.IsOutsideBottom.Should().BeFalse();

            //keep copy
            var oldTop = _anchorRect.Top;

            //set values to fragment rect
            portalItem.FragmentRect = FragmentRect;

            //IS OUTSIDE BOTTOM
            FragmentRect.IsOutsideBottom.Should().BeTrue();

            //correct the position
            Repositioning.CorrectAnchorBoundaries(portalItem);

            //the position was corrected (moved upside)
            oldTop.Should().BeGreaterThan(_anchorRect.Top);
            (_anchorRect.Top + FragmentRect.Height).Should().BeLessThan(_anchorRect.WindowHeight);

            FragmentRect.IsOutsideBottom.Should().BeFalse();

            //IS OUTSIDE LEFT

            _anchorRect.Left = -10;
            portalItem.FragmentRect = FragmentRect;

            var oldLeft = _anchorRect.Left;

            FragmentRect.IsOutsideLeft.Should().BeTrue();

            //correct the position
            Repositioning.CorrectAnchorBoundaries(portalItem);

            //the position was corrected, moved to the right
            (oldLeft < _anchorRect.Left).Should().BeTrue();
            (_anchorRect.Left - FragmentRect.Width).Should().BeGreaterOrEqualTo(0);

            FragmentRect.IsOutsideLeft.Should().BeFalse();

            //IS OUTSIDE RIGHT
            _anchorRect.Left = _anchorRect.WindowWidth - 1;
            portalItem.FragmentRect = FragmentRect;

            _anchorRect.IsOutsideRight.Should().BeTrue();
            oldLeft = _anchorRect.Left;

            //correct the position
            Repositioning.CorrectAnchorBoundaries(portalItem);

            //was pulled to the left
            (oldLeft > _anchorRect.Left).Should().BeTrue();
            (_anchorRect.Left + FragmentRect.Width).Should().BeLessOrEqualTo(_anchorRect.WindowWidth);
            FragmentRect.IsOutsideRight.Should().BeFalse();

            //IS OUTSIDE TOP
            _anchorRect.Top = -10;
            portalItem.FragmentRect = FragmentRect;

            _anchorRect.IsOutsideTop.Should().BeTrue();
            oldTop = _anchorRect.Top;

            //correct the position
            Repositioning.CorrectAnchorBoundaries(portalItem);

            //was pulled down
            (oldTop < _anchorRect.Top).Should().BeTrue();
            FragmentRect.IsOutsideTop.Should().BeFalse();
        }
    }
}
