using System;
using System.Linq;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Docs.Examples;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class SkeletonTests : BunitTest
    {
        [Test]
        public void SkeletonStyleIsAppliedAfterInitialRendering()
        {
            var comp = Context.RenderComponent<MudSkeleton>();

            var skeleton = comp.Instance;
            var span = comp.Find("span");

            var skeletonClasses = comp.Find(".mud-skeleton");
            // check initial state
            span.Attributes.GetNamedItem("style")?.Value.Should().BeNullOrEmpty();
            skeleton.Style.Should().BeNullOrEmpty();
            skeletonClasses.ClassList.Should().ContainInOrder(new[] { "mud-skeleton", "mud-skeleton-text", "mud-skeleton-pulse" });

            // add new style and check if it was applied
            comp.SetParametersAndRender(p => p.Add(x => x.Style, "background:blue"));
            span = comp.Find("span");
            span.Attributes.GetNamedItem("style")?.Value.Should().Be("background:blue;");
        }
    }
}
