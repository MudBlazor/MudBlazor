// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Linq;
using Bunit;
using FluentAssertions;
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
        var navLinksInsideActiveNavGroups = comp
            .FindAll("#second-level-navgroup a.mud-nav-link:not(.mud-nav-link-disabled)");

        navLinksInsideActiveNavGroups.Should().HaveCount(1);
        navLinksInsideActiveNavGroups
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
        var navLinksInsideActiveNavGroups = comp
            .FindAll("#second-level-navgroup a.mud-nav-link.mud-nav-link-disabled");

        navLinksInsideActiveNavGroups.Should().HaveCount(2);
        navLinksInsideActiveNavGroups
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
        var elementsWithTabIndex = comp.FindAll("[tabindex]");

        elementsWithTabIndex.Should().HaveCountGreaterThan(0).And.AllSatisfy(node => node.GetAttribute("tabindex").Should().Be("-1"));
    }
}
