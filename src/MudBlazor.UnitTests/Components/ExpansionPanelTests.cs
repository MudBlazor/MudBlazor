using System.Collections.Generic;
using System.Linq;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ExpansionPanelTests : BunitTest
    {
        /// <summary>
        /// Expansion panel must expand and collapse in the right order
        /// Here we are open the first, then the third and then the second
        /// </summary>
        [Test]
        public void MudExpansionPanel_Respects_Collapsing_Order()
        {
            var comp = Context.RenderComponent<ExpansionPanelExpansionsTest>();
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
        /// Multiple expanded expansion panels should not enter an infinite loop 
        /// when MultiExpansionPanel is false
        /// </summary>
        [Test]
        public void MudExpansionPanel_Without_MultiExpansion_Doesnt_Crash_With_Multiple_Expanded_Tabs()
        {
            var comp = Context.RenderComponent<ExpansionPanelExpandedMultipleWithoutMultipleExpansionSetTest>();

            //click in the three headers
            //foreach (var header in comp.FindAll(".mud-expand-panel-header"))
            //{
            //    header.Click();
            //}

            //Only one panel should be expanded
            var allPanels = comp.FindAll(".mud-expand-panel").ToList();

            var expandedPanels = comp.FindAll(".mud-panel-expanded").ToList();
            expandedPanels.Count.Should().Be(1);
            expandedPanels.First().Should().Be(allPanels.First());
        }

        /// <summary>
        /// MultiExpansion panel should not collapse other panels
        /// </summary>
        [Test]
        public void MudExpansionPanel_MultiExpansion_Doesnt_Collapse_Others()
        {
            var comp = Context.RenderComponent<ExpansionPanelMultiExpansionTest>();

            //click in the three headers
            foreach (var header in comp.FindAll(".mud-expand-panel-header"))
            {
                header.Click();
            }

            //the three panels must be expanded
            var panels = comp.FindAll(".mud-panel-expanded").ToList();
            panels.Count.Should().Be(3);
        }

        /// <summary>
        /// Start expanded should expand panel
        /// </summary>
        [Test]
        public void MudExpansionPanel_IsInitiallyExpanded_Expands()
        {
            var comp = Context.RenderComponent<ExpansionPanelStartExpandedTest>();

            // one panel is expanded initially
            var panels = comp.FindAll(".mud-panel-expanded").ToList();
            panels.Count.Should().Be(1);

            var header = comp.FindAll(".panel-two > .mud-expand-panel-header").First();
            header.Click();

            //we could close the panel
            panels = comp.FindAll(".mud-panel-expanded").ToList();
            panels.Count.Should().Be(0);
        }

        /// <summary>
        /// Start expanded should work with multi expansion
        /// </summary>
        [Test]
        public void MudExpansionPanel_IsInitiallyExpanded_Works_With_Multi_Expanded()
        {
            var comp = Context.RenderComponent<ExpansionPanelStartExpandedMultipleTest>();

            // three panels is expanded initially
            var panels = comp.FindAll(".mud-panel-expanded").ToList();
            panels.Count.Should().Be(3);

            //click in the three headers
            foreach (var header in comp.FindAll(".mud-expand-panel-header"))
            {
                header.Click();
            }

            //we could close them all
            panels = comp.FindAll(".mud-panel-expanded").ToList();
            panels.Count.Should().Be(0);
        }
    }
}
