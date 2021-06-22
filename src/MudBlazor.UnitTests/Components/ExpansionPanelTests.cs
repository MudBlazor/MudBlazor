using System.Collections.Generic;
using System.Linq;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ExpansionPanelTests
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
        /// Expansion panel must expand and collapse in the right order
        /// Here we are open the first, then the third and then the second
        /// </summary>
        [Test]
        public void MudExpansionPanel_Respects_Collapsing_Order()
        {
            var comp = ctx.RenderComponent<ExpansionPanelExpansions>();
            //the order in which the panels are going to be clicked
            //First, the first; then, the third, and then the second
            var sequence = new List<int> { 0, 2, 1 };
            foreach (var item in sequence)
            {
                var header = comp.FindAll(".mud-expand-panel-header")[item];
                header.Click();

                var panels = comp.FindAll(".mud-expand-panel").ToList();

                //just the panel that was clicked has the expanded class
                panels[item].OuterHtml.Should().Contain("mud-panel-expanded");
                foreach (var other in sequence.Where(it => it != item))
                {
                    //the other panels haven't the class expanded
                    panels[other].OuterHtml.Should().NotContain("mud-panel-expanded");
                }
            }

        }


        /// <summary>
        /// MultiExpansion panel should not collapse other panels
        /// </summary>
        [Test]
        public void MudExpansionPanel_MultiExpansion_Doesnt_Collapse_Others()
        {
            var comp = ctx.RenderComponent<ExpansionPanelMultiExpansion>();

            //click in the three headers 
            foreach (var header in comp.FindAll(".mud-expand-panel-header"))
            {
                header.Click();
            }

            //the three panels must be expanded
            var panels = comp.FindAll(".mud-panel-expanded").ToList();
            panels.Count.Should().Be(3);

        }




    }
}



