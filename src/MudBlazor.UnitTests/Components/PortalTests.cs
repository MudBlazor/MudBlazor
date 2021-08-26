
#pragma warning disable CS1998 // async without await

using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.Interop;
using NUnit.Framework;
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class PortalTests : BunitTest
    {
        /// <summary>
        /// MudElement renders first an anchor and then a button
        /// </summary>
        [Test]
        public void Basic_Example_of_a_Portaled_Menu()
        {
            var comp = Context.RenderComponent<PortalMenuTest>();
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
            var comp = Context.RenderComponent<PortalSelectTest>();
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
            var comp = Context.RenderComponent<PortalAutocompleteTest>();
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
            comp.WaitForAssertion(() => comp.FindAll(".mud-popover-open").Should().HaveCount(0));

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
            var comp = Context.RenderComponent<PortalTooltipTest>();
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

        private BoundingClientRect _anchorRect = new();
        private BoundingClientRect _fragmentRect = new();
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
    }
}
