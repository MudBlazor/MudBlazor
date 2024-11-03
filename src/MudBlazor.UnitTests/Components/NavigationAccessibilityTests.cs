// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Linq;
using Bunit;
using FluentAssertions;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class NavigationAccessibilityTests : BunitTest
{
    /// <summary>
    /// Active NavGroups should be focusable, i.e. have a tabindex of 0.
    /// </summary>
    [Test]
    public void ActiveNavGroups_Should_BeFocusable()
    {
        var comp = Context.RenderComponent<NavigationAccessibilityTest>();
        var navGroups = comp
            .FindComponents<MudNavGroup>()
            .Where(navGroup => navGroup.Instance.Disabled is false)
            .ToList();

        navGroups.Should().HaveCount(2);
        navGroups
            .Should()
            .AllSatisfy(navGroup =>
                navGroup
                    .Find("button")
                    .GetAttribute("tabindex")
                    .Should()
                    .Be("0"));
    }

    /// <summary>
    /// Disabled NavGroups should not be focusable, i.e. have a tabindex of -1.
    /// </summary>
    [Test]
    public void DisabledNavGroups_Should_NotBeFocusable()
    {
        var comp = Context.RenderComponent<NavigationAccessibilityTest>();
        var navGroups = comp
            .FindComponents<MudNavGroup>()
            .Where(navGroup => navGroup.Instance.Disabled)
            .ToList();

        navGroups.Should().HaveCount(1);
        navGroups
            .Should()
            .AllSatisfy(navGroup =>
                navGroup
                    .Find("button")
                    .GetAttribute("tabindex")
                    .Should()
                    .Be("-1"));
    }

    /// <summary>
    /// Active NavLinks (inside of active NavGroups) should be focusable, i.e. have a tabindex of 0.
    /// </summary>
    [Test]
    public void ActiveNavLinkInActiveNavGroup_Should_BeFocusable()
    {
        var comp = Context.RenderComponent<NavigationAccessibilityTest>();

        comp.FindAll("#second-level-navgroup a.mud-nav-link:not(.mud-nav-link-disabled)").Should().HaveCount(1);
        comp.FindAll("#second-level-navgroup a.mud-nav-link:not(.mud-nav-link-disabled)")
            .Should()
            .AllSatisfy(navLink =>
                navLink
                    .GetAttribute("tabindex")
                    .Should()
                    .Be("0"));
    }

    /// <summary>
    /// Disabled NavLinks (inside of active NavGroups) should not be focusable, i.e. have a tabindex of -1.
    /// </summary>
    [Test]
    public void DisabledNavLinksInActiveNavGroup_Should_NotBeFocusable()
    {
        var comp = Context.RenderComponent<NavigationAccessibilityTest>();

        comp.FindAll("#second-level-navgroup a.mud-nav-link.mud-nav-link-disabled").Should().HaveCount(2);
        comp.FindAll("#second-level-navgroup a.mud-nav-link.mud-nav-link-disabled")
            .Should()
            .AllSatisfy(navLink =>
                navLink
                    .GetAttribute("tabindex")
                    .Should()
                    .Be("-1"));
    }

    /// <summary>
    /// Nothing should be focusable within a disabled NavGroup other than the NavGroup expand button.
    /// </summary>
    [Test]
    public void EverythingWithinDisabledNavGroups_Should_NotBeFocusable()
    {
        var comp = Context.RenderComponent<NavigationAccessibilityTest>(parameters =>
            parameters.Add(p => p.TopLevelDisabled, true));

        comp.FindAll("[tabindex]").Should().HaveCountGreaterThan(0).And.AllSatisfy(node => node.GetAttribute("tabindex").Should().Be("-1"));
    }

    /// <summary>
    /// MudCollapse components within collapsed NavGroups should be aria-hidden.
    /// </summary>
    [Test]
    public void MudCollapseWithinCollapsedNavGroup_Should_BeAriaHidden()
    {
        var comp = Context.RenderComponent<NavigationAccessibilityTest>(parameters =>
            parameters.Add(p => p.TopLevelExpanded, false));

        comp.FindAll(".mud-collapse-container").Should().HaveCountGreaterThan(0).And.AllSatisfy(node => node.GetAttribute("aria-hidden").Should().Be("true"));
    }

    /// <summary>
    /// MudCollapse components within expanded NavGroups should not be aria-hidden.
    /// </summary>
    [Test]
    public void MudCollapseWithinExpandedNavGroup_Should_NotBeAriaHidden()
    {
        var comp = Context.RenderComponent<NavigationAccessibilityTest>();

        comp.FindAll("#second-level-navgroup .mud-collapse-container").Should().HaveCountGreaterThan(0).And.AllSatisfy(node => node.GetAttribute("aria-hidden").Should().Be("false"));
    }

    /// <summary>
    /// NavGroup buttons should have aria-expanded set to true when expanded and false when collapsed.
    /// </summary>
    [Test]
    public void NavGroupButtons_Should_HaveCorrectAriaExpandedValue()
    {
        var comp = Context.RenderComponent<NavigationAccessibilityTest>();
        var navGroups = comp.FindComponents<MudNavGroup>();

        navGroups
            .Should()
            .AllSatisfy(navGroup =>
                navGroup
                    .Find("button")
                    .GetAttribute("aria-expanded")
                    .Should()
                    .Be(navGroup.Instance.GetState(x => x.Expanded).ToString().ToLowerInvariant()));
    }

    /// <summary>
    /// NavGroup buttons should have aria-controls set to the id of a NavMenu.
    /// </summary>
    [Test]
    public void NavGroupButtons_Should_HaveValidAriaControlsValue_And_NavMenus_Should_HaveAnId()
    {
        var comp = Context.RenderComponent<NavigationAccessibilityTest>();
        var navGroups = comp.FindComponents<MudNavGroup>();
        var navMenus = comp.FindComponents<MudNavMenu>();
        var ariaControlsIds = navGroups
            .Select(navGroup =>
                navGroup
                    .Find("button")
                    .GetAttribute("aria-controls"));
        var navMenuIds = navMenus
            .Select(
                navMenu => navMenu
                    .Find(".mud-navmenu")
                    .GetAttribute("id"));

        ariaControlsIds.Should().BeEquivalentTo(navMenuIds);
    }

    /// <summary>
    /// NavGroup buttons should have an aria-label.
    /// </summary>
    [Test]
    public void NavGroupButtons_Should_HaveAriaLabel()
    {
        var comp = Context.RenderComponent<NavigationAccessibilityTest>();
        var navGroups = comp.FindComponents<MudNavGroup>();

        navGroups
            .Should()
            .AllSatisfy(navGroup =>
                navGroup
                    .Find("button")
                    .GetAttribute("aria-label")
                    .Should()
                    .NotBeNullOrEmpty());
    }

    /// <summary>
    /// NavGroups should have an aria-label when a title is provided.
    /// </summary>
    [Test]
    public void NavGroups_Should_HaveAriaLabel_WhenTitleIsProvided()
    {
        var expectedTitle = "expected title";
        var comp = Context.RenderComponent<NavigationAccessibilityTest>(parameters =>
            parameters.Add(p => p.SecondLevelTitle, expectedTitle));

        comp.Find("#second-level-navgroup")
            .GetAttribute("aria-label")
            .Should()
            .Be(expectedTitle);
    }

    /// <summary>
    /// NavGroup buttons' aria-label should contain the title of the NavGroup when a title is provided.
    /// </summary>
    [Test]
    public void NavGroupButtonsAriaLabel_Should_ContainTitle_WhenTitleIsProvided()
    {
        var expectedTitle = "expected title";
        var comp = Context.RenderComponent<NavigationAccessibilityTest>(parameters =>
            parameters.Add(p => p.SecondLevelTitle, expectedTitle));

        comp.Find("#second-level-navgroup > button")
            .GetAttribute("aria-label")
            .Should()
            .Contain(expectedTitle);
    }
}
