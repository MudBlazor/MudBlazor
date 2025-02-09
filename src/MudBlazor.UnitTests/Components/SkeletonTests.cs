using Bunit;
using FluentAssertions;
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

        [Test]
        public void SkeletonHeightIsAppliedAfterInitialRendering()
        {
            var comp = Context.RenderComponent<MudSkeleton>(p => p.Add(x => x.Height, "100px"));

            var skeleton = comp.Instance;
            var span = comp.Find("span");

            var skeletonClasses = comp.Find(".mud-skeleton");
            // check initial state
            span.Attributes.GetNamedItem("style")?.Value.Should().Be("height:100px;");
            skeleton.Height.Should().Be("100px");
            skeletonClasses.ClassList.Should().ContainInOrder(new[] { "mud-skeleton", "mud-skeleton-text", "mud-skeleton-pulse" });

            // add new style and check if it was applied
            comp.SetParametersAndRender(p => p.Add(x => x.Height, "50px"));
            span = comp.Find("span");
            span.Attributes.GetNamedItem("style")?.Value.Should().Be("height:50px;");
        }

        [Test]
        public void SkeletonWidthIsAppliedAfterInitialRendering()
        {
            var comp = Context.RenderComponent<MudSkeleton>(p => p.Add(x => x.Width, "300px"));

            var skeleton = comp.Instance;
            var span = comp.Find("span");

            var skeletonClasses = comp.Find(".mud-skeleton");
            // check initial state
            span.Attributes.GetNamedItem("style")?.Value.Should().Be("width:300px;");
            skeleton.Width.Should().Be("300px");
            skeletonClasses.ClassList.Should().ContainInOrder(new[] { "mud-skeleton", "mud-skeleton-text", "mud-skeleton-pulse" });

            // add new style and check if it was applied
            comp.SetParametersAndRender(p => p.Add(x => x.Width, "500px"));
            span = comp.Find("span");
            span.Attributes.GetNamedItem("style")?.Value.Should().Be("width:500px;");
        }
    }
}
