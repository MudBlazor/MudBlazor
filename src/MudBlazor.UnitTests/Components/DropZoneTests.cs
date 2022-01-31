
using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DropZoneTests : BunitTest
    {

        [Test]
        public void DropContainer_Defaults()
        {
            var container = new MudDropContainer<object>();

            container.ApplyDropClassesOnDragStarted.Should().BeFalse();
            container.CanDrop.Should().BeNull();
            container.CanDropClass.Should().BeNullOrEmpty();
            container.DisabledClass.Should().Be("disabled");
            container.DraggingClass.Should().BeNullOrEmpty();
            container.ItemDraggingClass.Should().BeNullOrEmpty();
            container.ItemIsDisbaled.Should().BeNull();
            container.Items.Should().BeNull();
            container.ItemsSelector.Should().BeNull();
            container.NoDropClass.Should().BeNullOrEmpty();
        }

        [Test]
        public void GenerelView()
        {
            var comp = Context.RenderComponent<DropzoneBasicTest>();

            var container = comp.Find(".mud-drop-container");

            container.ClassList.Should().BeEquivalentTo(new[] { "mud-drop-container", "d-flex" });

            container.Children.Should().HaveCount(2);

            var firstDropZone = container.Children[0];
            firstDropZone.ClassList.Should().Contain("first-drop-zone");
            firstDropZone.Children.Should().HaveCount(2);

            firstDropZone.Children[0].TextContent.Should().Be("Drop Zone 1");
            firstDropZone.Children[1].TextContent.Should().Be("First Item");

            var secondDropZone = container.Children[1];
            secondDropZone.ClassList.Should().Contain("second-drop-zone");
            secondDropZone.Children.Should().HaveCount(3);

            secondDropZone.Children[0].TextContent.Should().Be("Drop Zone 2");
            secondDropZone.Children[1].TextContent.Should().Be("Second Item");
            secondDropZone.Children[2].TextContent.Should().Be("Third Item");

        }
    }
}
