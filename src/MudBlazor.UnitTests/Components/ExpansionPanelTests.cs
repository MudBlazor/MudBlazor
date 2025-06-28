using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.ExpansionPanel;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ExpansionPanelTests : BunitTest
    {

        [OneTimeSetUp]
        public void Init()
        {
            AssertionOptions.FormattingOptions.MaxDepth = 100;
            AssertionOptions.FormattingOptions.MaxLines = 5000;
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            AssertionOptions.FormattingOptions.MaxDepth = 5;
            AssertionOptions.FormattingOptions.MaxLines = 100;
        }

        /// <summary>
        /// Expansion panel must expand and collapse in the right order
        /// Here we are open the first, then the third and then the second
        /// </summary>
        [Test]
        public async Task MudExpansionPanel_Respects_Collapsing_Order()
        {
            var comp = Context.RenderComponent<ExpansionPanelExpansionsTest>();
            //the order in which the panels are going to be clicked
            //First, the first; then, the third, and then the second
            var sequence = new List<int> { 0, 2, 1 };
            foreach (var item in sequence)
            {
                await comp.InvokeAsync(() => comp.FindAll(".mud-expand-panel-header")[item].Click());

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
        public async Task MudExpansionPanel_MultiExpansion_Doesnt_Collapse_Others()
        {
            var comp = Context.RenderComponent<ExpansionPanelMultiExpansionTest>();

            //click in the three headers
            foreach (var header in comp.FindAll(".mud-expand-panel-header"))
            {
                await comp.InvokeAsync(() => header.Click());
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
        public async Task MudExpansionPanel_IsInitiallyExpanded_Works_With_Multi_Expanded()
        {
            var comp = Context.RenderComponent<ExpansionPanelStartExpandedMultipleTest>();

            // three panels is expanded initially
            var panels = comp.FindAll(".mud-panel-expanded").ToList();
            panels.Count.Should().Be(3);

            //click in the three headers
            foreach (var header in comp.FindAll(".mud-expand-panel-header"))
            {
                await comp.InvokeAsync(() => header.Click());
            }

            //we could close them all
            panels = comp.FindAll(".mud-panel-expanded").ToList();
            panels.Count.Should().Be(0);
        }

        [Test]
        public async Task MudExpansionPanel_Other()
        {
            var comp = Context.RenderComponent<ExpansionPanelStartExpandedTest>();
            var panel = comp.FindComponent<MudExpansionPanel>();
            panel.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.Disabled, true));

            comp.WaitForAssertion(() => comp.Instance.Panel1Expanded.Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Panel2Expanded.Should().BeTrue());
            comp.WaitForAssertion(() => comp.Instance.Panel3Expanded.Should().BeFalse());
            await comp.InvokeAsync(panel.Instance.ToggleExpansionAsync);
            comp.WaitForAssertion(() => comp.Instance.Panel1Expanded.Should().BeFalse()); // ToggleExpansionAsync checks for Disabled, so nothing happens
            comp.WaitForAssertion(() => comp.Instance.Panel2Expanded.Should().BeTrue());
            comp.WaitForAssertion(() => comp.Instance.Panel3Expanded.Should().BeFalse());
            await comp.InvokeAsync(panel.Instance.ExpandAsync);
            comp.WaitForAssertion(() => comp.Instance.Panel1Expanded.Should().BeTrue()); // ExpandAsync ignores Disabled
            comp.WaitForAssertion(() => comp.Instance.Panel2Expanded.Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Panel3Expanded.Should().BeFalse());
            await comp.InvokeAsync(panel.Instance.CollapseAsync);
            comp.WaitForAssertion(() => comp.Instance.Panel1Expanded.Should().BeFalse()); // ExpandAsync ignores Disabled
            comp.WaitForAssertion(() => comp.Instance.Panel2Expanded.Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Panel3Expanded.Should().BeFalse());
        }

        /// <summary>
        /// Tests that ExpandAll method expands all panels.
        /// </summary>
        [Test]
        public async Task MudExpansionPanel_ExpandAllAync()
        {
            var panels = Context.RenderComponent<MudExpansionPanels>();
            var panel1 = new MudExpansionPanel();
            var panel2 = new MudExpansionPanel();
            var panel3 = new MudExpansionPanel();
            await panels.Instance.AddPanelAsync(panel1);
            await panels.Instance.AddPanelAsync(panel2);
            await panels.Instance.AddPanelAsync(panel3);
            // We check _expandedState because we do not modify Expanded directly, therefore the parameter doesn't change.
            // For parameter to change you need to bind panels Expansion it via razor syntax, so that parent would update it.
            panels.Instance.Panels.Count.Should().Be(3);
            panel1._expandedState.Value.Should().BeFalse();
            panel2._expandedState.Value.Should().BeFalse();
            panel3._expandedState.Value.Should().BeFalse();
            await panels.Instance.ExpandAllAsync();
            panel1._expandedState.Value.Should().BeTrue();
            panel2._expandedState.Value.Should().BeTrue();
            panel3._expandedState.Value.Should().BeTrue();
        }

        /// <summary>
        /// Tests that CollapseAll method collapses all panels.
        /// </summary>
        [Test]
        public async Task MudExpansionPanel_CollapseAllAsync()
        {
            var panels = Context.RenderComponent<MudExpansionPanels>();
            var panel1 = new MudExpansionPanel();
            var panel2 = new MudExpansionPanel();
            var panel3 = new MudExpansionPanel();
            await panels.Instance.AddPanelAsync(panel1);
            await panels.Instance.AddPanelAsync(panel2);
            await panels.Instance.AddPanelAsync(panel3);
            await panel1.ExpandAsync();
            await panel2.ExpandAsync();
            await panel3.ExpandAsync();
            // We check _expandedState because we do not modify Expanded directly, therefore the parameter doesn't change.
            // For parameter to change you need to bind panels Expansion it via razor syntax, so that parent would update it.
            panels.Instance.Panels.Count.Should().Be(3);
            panel1._expandedState.Value.Should().BeTrue();
            panel2._expandedState.Value.Should().BeTrue();
            panel3._expandedState.Value.Should().BeTrue();
            await panels.Instance.CollapseAllAsync();
            panel1._expandedState.Value.Should().BeFalse();
            panel2._expandedState.Value.Should().BeFalse();
            panel3._expandedState.Value.Should().BeFalse();
        }

        /// <summary>
        /// Tests that CollapseAllExcept method collapses all panels except one.
        /// </summary>
        [Test]
        public async Task MudExpansionPanel_CollapseAllExceptAsync()
        {
            var panels = Context.RenderComponent<MudExpansionPanels>();
            var panel1 = new MudExpansionPanel();
            var panel2 = new MudExpansionPanel();
            var panel3 = new MudExpansionPanel();
            await panels.Instance.AddPanelAsync(panel1);
            await panels.Instance.AddPanelAsync(panel2);
            await panels.Instance.AddPanelAsync(panel3);
            await panel1.ExpandAsync();
            await panel2.ExpandAsync();
            await panel3.ExpandAsync();
            // We check _expandedState because we do not modify Expanded directly, therefore the parameter doesn't change.
            // For parameter to change you need to bind panels Expansion it via razor syntax, so that parent would update it.
            panels.Instance.Panels.Count.Should().Be(3);
            panel1._expandedState.Value.Should().BeTrue();
            panel2._expandedState.Value.Should().BeTrue();
            panel3._expandedState.Value.Should().BeTrue();
            await panels.Instance.CollapseAllExceptAsync(panel2);
            panel1._expandedState.Value.Should().BeFalse();
            panel2._expandedState.Value.Should().BeTrue();
            panel3._expandedState.Value.Should().BeFalse();
        }

        /// <summary>
        /// Test for https://github.com/MudBlazor/MudBlazor/issues/8429
        /// </summary>
        [Test]
        public async Task MudExpansionPanel_TwoWayBinding()
        {
            var comp = Context.RenderComponent<ExpansionPanelTwoWayBIndingTest>();

            comp.WaitForAssertion(() => comp.Instance.Expansion[0].Should().BeTrue());
            comp.WaitForAssertion(() => comp.Instance.Expansion[1].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[2].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[3].Should().BeFalse());

            await comp.InvokeAsync(comp.Instance.ToggleExpansion2);

            comp.WaitForAssertion(() => comp.Instance.Expansion[0].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[1].Should().BeTrue());
            comp.WaitForAssertion(() => comp.Instance.Expansion[2].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[3].Should().BeFalse());

            await comp.InvokeAsync(comp.Instance.ToggleExpansion3);

            comp.WaitForAssertion(() => comp.Instance.Expansion[0].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[1].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[2].Should().BeTrue());
            comp.WaitForAssertion(() => comp.Instance.Expansion[3].Should().BeFalse());

            await comp.InvokeAsync(comp.Instance.ToggleExpansion4);

            comp.WaitForAssertion(() => comp.Instance.Expansion[0].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[1].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[2].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[3].Should().BeTrue());

            await comp.InvokeAsync(comp.Instance.ToggleExpansion3);

            comp.WaitForAssertion(() => comp.Instance.Expansion[0].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[1].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[2].Should().BeTrue());
            comp.WaitForAssertion(() => comp.Instance.Expansion[3].Should().BeFalse());

            await comp.InvokeAsync(comp.Instance.ToggleExpansion2);

            comp.WaitForAssertion(() => comp.Instance.Expansion[0].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[1].Should().BeTrue());
            comp.WaitForAssertion(() => comp.Instance.Expansion[2].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[3].Should().BeFalse());

            await comp.InvokeAsync(comp.Instance.ToggleExpansion1);

            comp.WaitForAssertion(() => comp.Instance.Expansion[0].Should().BeTrue());
            comp.WaitForAssertion(() => comp.Instance.Expansion[1].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[2].Should().BeFalse());
            comp.WaitForAssertion(() => comp.Instance.Expansion[3].Should().BeFalse());
        }

        /// <summary>
        /// Tests that the panel toggles expansion state when the Enter and Space keys are pressed.
        /// </summary>
        [Test]
        public async Task MudExpansionPanel_Handles_KeyDown_For_Toggle()
        {
            var comp = Context.RenderComponent<ExpansionPanelKeyboardTest>();
            var header = comp.Find(".mud-expand-panel-header");

            comp.Markup.Should().NotContain("mud-panel-expanded");
            await header.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Enter" });
            comp.Markup.Should().Contain("mud-panel-expanded");
            await header.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = " " });
            comp.Markup.Should().NotContain("mud-panel-expanded");
        }

        /// <summary>
        /// Tests that buttons and other interactive content within a collapsed expansion panel 
        /// are properly hidden from accessibility tools and keyboard navigation by being placed 
        /// in a container with the hidden attribute.
        /// </summary>
        [Test]
        public void MudExpansionPanel_Button_IsInHiddenContainer_When_Panel_Collapsed()
        {
            var comp = Context.RenderComponent<ExpansionPanelKeyboardTest>();
            var button = comp.Find("button");
            var hiddenParent = button
                .GetAncestors()
                .OfType<IElement>()
                .FirstOrDefault(e => e.HasAttribute("hidden"));

            hiddenParent.Should().NotBeNull("button should not be accessible when the panel is collapsed");
        }

        /// <summary>
        /// Tests that HandleKeyDownAsync ignores key presses when the panel is disabled.
        /// </summary>
        [Test]
        public async Task MudExpansionPanel_HandleKeyDownAsync_IgnoresKeys_When_Disabled()
        {
            var comp = Context.RenderComponent<MudExpansionPanel>(parameters => parameters
                .Add(p => p.Disabled, true)
                .Add(p => p.Text, "Disabled Panel"));

            var header = comp.Find(".mud-expand-panel-header");
            comp.Markup.Should().NotContain("mud-panel-expanded");

            // Try to expand with Enter key - should be ignored
            await header.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Enter" });
            comp.Markup.Should().NotContain("mud-panel-expanded");

            // Try to expand with Space key - should be ignored
            await header.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = " " });
            comp.Markup.Should().NotContain("mud-panel-expanded");
        }

        /// <summary>
        /// Tests that content is rendered even when collapsed when KeepContentAlive is true.
        /// </summary>
        [Test]
        public void MudExpansionPanel_KeepContentAlive_True_RendersContentWhenCollapsed()
        {
            var comp = Context.RenderComponent<MudExpansionPanel>(parameters => parameters
                .Add(p => p.Text, "Test Panel")
                .Add(p => p.KeepContentAlive, true)
                .Add(p => p.ChildContent, builder => builder.AddMarkupContent(0, "<button>Test Button</button>")));

            comp.Markup.Should().NotContain("mud-panel-expanded");
            comp.Find("button").Should().NotBeNull();

            var contentDiv = comp.Find(".mud-expand-panel-content");
            contentDiv.HasAttribute("hidden").Should().BeTrue();
        }

        /// <summary>
        /// Tests that content is not rendered when collapsed when KeepContentAlive is false.
        /// </summary>
        [Test]
        public void MudExpansionPanel_KeepContentAlive_False_DoesNotRenderContentWhenCollapsed()
        {
            var comp = Context.RenderComponent<MudExpansionPanel>(parameters => parameters
                .Add(p => p.Text, "Test Panel")
                .Add(p => p.KeepContentAlive, false)
                .Add(p => p.ChildContent, builder => builder.AddMarkupContent(0, "<button>Test Button</button>")));

            comp.Markup.Should().NotContain("mud-panel-expanded");
            comp.FindAll("button").Should().BeEmpty();
        }
    }
}
