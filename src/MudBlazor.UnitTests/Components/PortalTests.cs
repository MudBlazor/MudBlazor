#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.Interop;
using MudBlazor.Services;
using NUnit.Framework;
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
            //there is already 1 portal;
            var itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("1");
            comp.FindAll(".portal-anchor").Should().HaveCount(1);
            comp.FindAll(".portal").Should().HaveCount(1);

            //after click the button of the menu, the menu is open
            comp.Find("button").Click();

            //it has 2 popovers, one is what you see, the other is used to make the calculations to
            //know if it fits the viewport or it needs to be moved inside
            comp.FindAll(".mud-popover-open").Should().HaveCount(2);

            //should have 3 items
            comp.FindAll(".portal-anchor .mud-list-item").Should().HaveCount(3);

            //clicking in one of them, the popover closes
            comp.FindAll(".portal-anchor .mud-list-item")[0].Click();
            comp.FindAll(".mud-popover-open").Should().HaveCount(0);
        }

        /// <summary>
        /// MudElement renders first an anchor and then a button
        /// </summary>
        [Test]
        public void Basic_Example_of_a_Portaled_Select()
        {
            var comp = ctx.RenderComponent<PortalSelectTest>();
            var portalprovider = comp.Find("#mud-portal-container");
            //the portal provider has already 1 element;
            var itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("1");
            comp.FindAll(".portal-anchor").Should().HaveCount(1);
            comp.FindAll(".portal").Should().HaveCount(1);

            //after click the button of the select, the select is opened
            comp.Find("input").Click();

            //it has 2 popovers, one is what you see, the other is used to make the calculations to
            //know if it fits the viewport or it needs to be moved inside
            comp.FindAll(".mud-popover-open").Should().HaveCount(2);

            //The portal provider still has 1 menu inside
            itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("1");

            //should have 4 items
            comp.FindAll(".portal-anchor .mud-list-item").Should().HaveCount(4);

            //clicking in one of them, the popover closes
            comp.FindAll(".portal-anchor .mud-list-item")[0].Click();
            comp.FindAll(".mud-popover-open").Should().HaveCount(0);
        }

        /// <summary>
        /// MudElement renders first an anchor and then a button
        /// </summary>
        [Test]
        public void Basic_Example_of_a_Portaled_Autocomplete()
        {
            var comp = ctx.RenderComponent<PortalAutocompleteTest>();
            var portalprovider = comp.Find("#mud-portal-container");

            //The portal provider now has 1 menu inside
            var itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("1");

            // portal is created
            comp.FindAll(".portal-anchor").Should().HaveCount(1);
            comp.FindAll(".portal").Should().HaveCount(1);

            //it has 2 popovers, one is what you see, the other is used to make the calculations to
            //know if it fits the viewport or it needs to be moved inside
            comp.FindAll(".mud-popover").Should().HaveCount(2);

            //after click the button of the menu, the portal is created
            comp.Find("input").Input("Ala");
            //should have 3 items
            comp.WaitForAssertion(() => comp.FindAll(".portal-anchor .mud-list-item").Should().HaveCount(3));
            comp.FindAll(".mud-popover-open").Should().HaveCount(2);
            //clicking in one of them, the popover closes
            comp.FindAll(".portal-anchor .mud-list-item")[0].Click();
            comp.FindAll(".mud-popover-open").Should().HaveCount(0);

            //still, the portal remains
            itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("1");
        }

        /// <summary>
        /// MudElement renders first an anchor and then a button
        /// </summary>
        [Test]
        public async Task Basic_Example_of_a_Portaled_Tooltip()
        {
            var comp = ctx.RenderComponent<PortalTooltipTest>();
            var portalprovider = comp.Find("#mud-portal-container");
            //the portal provider has 1 item;
            var itemsNumber = portalprovider.GetAttribute("data-items");
            itemsNumber.Should().Be("1");
            comp.FindAll(".portal-anchor").Should().HaveCount(1);
            comp.FindAll(".portal").Should().HaveCount(1);

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
            oldTop.Should().BeGreaterThan(portalItem.AnchorRect.Top);
            (portalItem.AnchorRect.Top + FragmentRect.Height).Should().BeLessThan(portalItem.AnchorRect.WindowHeight);
            _anchorRect = portalItem.AnchorRect;
            FragmentRect.IsOutsideBottom.Should().BeFalse();

            //IS OUTSIDE LEFT

            _anchorRect.Left = -10;
            portalItem.FragmentRect = FragmentRect;

            var oldLeft = _anchorRect.Left;

            FragmentRect.IsOutsideLeft.Should().BeTrue();

            //correct the position
            Repositioning.CorrectAnchorBoundaries(portalItem);
            _anchorRect = portalItem.AnchorRect;
            //the position was corrected, moved to the right
            (oldLeft < _anchorRect.Left).Should().BeTrue();
            (_anchorRect.Left).Should().Be(0);

            FragmentRect.IsOutsideLeft.Should().BeFalse();

            //IS OUTSIDE RIGHT
            _anchorRect.Left = _anchorRect.WindowWidth - 1;
            portalItem.FragmentRect = FragmentRect;

            _anchorRect.IsOutsideRight.Should().BeTrue();
            oldLeft = _anchorRect.Left;

            //correct the position
            Repositioning.CorrectAnchorBoundaries(portalItem);
            _anchorRect = portalItem.AnchorRect;
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
            _anchorRect = portalItem.AnchorRect;
            //was pulled down
            (oldTop < _anchorRect.Top).Should().BeTrue();
            FragmentRect.IsOutsideTop.Should().BeFalse();
        }
    }
}
