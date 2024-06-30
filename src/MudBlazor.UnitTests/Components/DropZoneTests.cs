
using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
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
            container.ItemDisabled.Should().BeNull();
            container.Items.Should().BeEmpty();
            container.ItemsSelector.Should().BeNull();
            container.NoDropClass.Should().BeNullOrEmpty();
        }

        [Test]
        public void DropZone_Defaults()
        {
            var zone = new MudDropZone<object>();

            zone.ApplyDropClassesOnDragStarted.Should().BeNull();
            zone.CanDrop.Should().BeNull();
            zone.CanDropClass.Should().BeNullOrEmpty();
            zone.DisabledClass.Should().BeNullOrEmpty();
            zone.DraggingClass.Should().BeNullOrEmpty();
            zone.ItemDraggingClass.Should().BeNullOrEmpty();
            zone.ItemDisabled.Should().BeNull();
            zone.ItemsSelector.Should().BeNull();
            zone.NoDropClass.Should().BeNullOrEmpty();
            zone.OnlyZone.Should().BeFalse();
            zone.AllowReorder.Should().BeFalse();
        }

        [Test]
        public void DropItem_Defaults()
        {
            var item = new MudDynamicDropItem<object>();

            item.Disabled.Should().BeFalse();
            item.DisabledClass.Should().BeNullOrEmpty();
            item.DraggingClass.Should().BeNullOrEmpty();
            item.ZoneIdentifier.Should().BeNullOrEmpty();
            item.Item.Should().BeNull();
            item.HideContent.Should().BeFalse();
        }

        [Test]
        public void DropZone_DisposeWork()
        {
            var container = new MudDropZone<object>();
            container.Dispose();
        }

        [Test]
        public void DropZone_GeneralView()
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

            var items = comp.FindAll(".mud-drop-item");
            items.Count(x => x.GetAttribute("draggable") == "true").Should().Be(3);
        }

        [Test]
        public void DropZone_TestJsCalls()
        {
            var jsRuntimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            Context.Services.AddSingleton(typeof(IJSRuntime), jsRuntimeMock.Object);

            jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudDragAndDrop.initDropZone", It.Is<object[]>(y => y.Length == 1 && Guid.Parse(y[0].ToString()) != Guid.Empty)))
                .ReturnsAsync(Mock.Of<IJSVoidResult>(), TimeSpan.FromMilliseconds(200)).Verifiable();

            var comp = Context.RenderComponent<DropzoneBasicTest>();

            jsRuntimeMock.Verify();
        }

        [Test]
        public void DropZone_DropZoneOverrideContainerRendered()
        {
            var comp = Context.RenderComponent<DropzoneCustomItemSelectorTest>();

            var container = comp.Find(".mud-drop-container");

            container.ClassList.Should().BeEquivalentTo(new[] { "mud-drop-container", "d-flex" });

            container.Children.Should().HaveCount(2);

            var firstDropZone = container.Children[0];

            firstDropZone.Children[0].TextContent.Should().Be("Drop Zone 1");
            firstDropZone.Children[1].TextContent.Should().Be("First Item by Drop Zone 1");

            var secondDropZone = container.Children[1];

            secondDropZone.Children[0].TextContent.Should().Be("Drop Zone 2");
            secondDropZone.Children[1].TextContent.Should().Be("Second Item");
            secondDropZone.Children[2].TextContent.Should().Be("Third Item");
        }

        [Test]
        public async Task DropZone_SimpleDragAndDrop()
        {
            var comp = Context.RenderComponent<DropzoneBasicTest>();

            var container = comp.Find(".mud-drop-container");
            container.Children.Should().HaveCount(2);

            var firstDropZone = container.Children[0];
            firstDropZone.Children.Should().HaveCount(2);

            var firstDropItem = firstDropZone.Children[1];

            firstDropItem.TextContent.Should().Be("First Item");
            await firstDropItem.DragStartAsync(new DragEventArgs());

            var secondDropZone = comp.Find(".second-drop-zone");

            await secondDropZone.DropAsync(new DragEventArgs());

            //updating the container reference to reflect changes
            container = comp.Find(".mud-drop-container");

            secondDropZone = container.Children[1];

            secondDropZone.Children.Should().HaveCount(4);

            secondDropZone.Children[0].TextContent.Should().Be("Drop Zone 2");
            secondDropZone.Children[1].TextContent.Should().Be("First Item");
            secondDropZone.Children[2].TextContent.Should().Be("Second Item");
            secondDropZone.Children[3].TextContent.Should().Be("Third Item");

            secondDropZone.Children[1].DragEnd();

            firstDropZone = comp.Find(".first-drop-zone");
            secondDropZone = comp.Find(".second-drop-zone");
            firstDropZone.ClassList.Should().NotContain("mud-drop-zone-drag-block");
            secondDropZone.ClassList.Should().NotContain("mud-drop-zone-drag-block");

            comp.Instance.IndexHistory.Distinct().Should().ContainSingle().And.Contain(-1);
        }

        [Test]
        public async Task DropZone_DragAndDropDraggingClass_DragCanceled()
        {
            var comp = Context.RenderComponent<DropzoneBasicTest>();

            var container = comp.Find(".mud-drop-container");
            container.Children.Should().HaveCount(2);

            var firstDropZone = container.Children[0];
            firstDropZone.Children.Should().HaveCount(2);
            var firstDropItem = firstDropZone.Children[1];

            firstDropZone.ClassList.Should().NotContain("my-special-dragging-class");
            firstDropItem.ClassList.Should().NotContain("my-special-item-dragging-class");
            await firstDropItem.DragStartAsync(new DragEventArgs());

            //reload DOM references
            container = comp.Find(".mud-drop-container");
            firstDropZone = container.Children[0];
            firstDropItem = firstDropZone.Children[1];

            firstDropZone.ClassList.Should().Contain("my-special-dragging-class");
            firstDropItem.ClassList.Should().Contain("my-special-item-dragging-class");

            //just cancel the transaction

            await firstDropItem.DragEndAsync(new DragEventArgs());

            //reload DOM references
            container = comp.Find(".mud-drop-container");
            firstDropZone = container.Children[0];
            firstDropItem = firstDropZone.Children[1];

            firstDropZone.ClassList.Should().NotContain("my-special-dragging-class");
            firstDropItem.ClassList.Should().NotContain("my-special-item-dragging-class");

            var secondDropZone = container.Children[1];
            var secondDropItem = secondDropZone.Children[1];

            secondDropZone = container.Children[1];

            secondDropZone.ClassList.Should().NotContain("my-special-drop-zone-dragging-class");
            secondDropItem.ClassList.Should().NotContain("my-special-item-drop-zone-dragging-class");

            await secondDropItem.DragStartAsync(new DragEventArgs());

            //reload DOM references
            container = comp.Find(".mud-drop-container");
            secondDropZone = container.Children[1];
            secondDropItem = secondDropZone.Children[1];

            secondDropZone.ClassList.Should().Contain("my-special-drop-zone-dragging-class");
            secondDropItem.ClassList.Should().Contain("my-special-item-drop-zone-dragging-class");

            await secondDropItem.DragEndAsync(new DragEventArgs());

            //reload DOM references
            container = comp.Find(".mud-drop-container");
            secondDropZone = container.Children[1];
            secondDropItem = secondDropZone.Children[1];

            secondDropZone.ClassList.Should().NotContain("my-special-drop-zone-dragging-class");
            secondDropItem.ClassList.Should().NotContain("my-special-item-drop-zone-dragging-class");
        }

        [Test]
        public async Task DropZone_DragAndDropDraggingClass_DragFinished()
        {
            var comp = Context.RenderComponent<DropzoneBasicTest>();

            var container = comp.Find(".mud-drop-container");
            container.Children.Should().HaveCount(2);

            var firstDropZone = container.Children[0];
            var secondDropZone = container.Children[1];

            firstDropZone.Children.Should().HaveCount(2);
            var firstDropItem = firstDropZone.Children[1];

            firstDropZone.ClassList.Should().NotContain("my-special-dragging-class");
            firstDropItem.ClassList.Should().NotContain("my-special-item-dragging-class");
            await firstDropItem.DragStartAsync(new DragEventArgs());

            //reload DOM references
            container = comp.Find(".mud-drop-container");
            firstDropZone = container.Children[0];
            firstDropItem = firstDropZone.Children[1];

            firstDropZone.ClassList.Should().Contain("my-special-dragging-class");
            firstDropItem.ClassList.Should().Contain("my-special-item-dragging-class");

            //drop item in second zone
            await secondDropZone.DropAsync(new DragEventArgs());

            //reload DOM references
            container = comp.Find(".mud-drop-container");
            firstDropZone = container.Children[0];
            firstDropItem = container.Children[1].Children[1];

            firstDropZone.ClassList.Should().NotContain("my-special-dragging-class");
            firstDropItem.ClassList.Should().NotContain("my-special-item-dragging-class");
        }

        [Test]
        public async Task DropZone_DragAndDropDraggingClass_DragFinished_DropNotAllowed()
        {
            var comp = Context.RenderComponent<DropzoneDraggingTestCantDropSecondZoneTest>();

            var container = comp.Find(".mud-drop-container");
            container.Children.Should().HaveCount(2);

            var firstDropZone = container.Children[0];
            var secondDropZone = container.Children[1];

            firstDropZone.Children.Should().HaveCount(2);
            var firstDropItem = firstDropZone.Children[1];

            firstDropZone.ClassList.Should().NotContain("my-special-dragging-class");
            firstDropItem.ClassList.Should().NotContain("my-special-item-dragging-class");
            await firstDropItem.DragStartAsync(new DragEventArgs());

            //reload DOM references
            container = comp.Find(".mud-drop-container");
            firstDropZone = container.Children[0];
            firstDropItem = firstDropZone.Children[1];

            firstDropZone.ClassList.Should().Contain("my-special-dragging-class");
            firstDropItem.ClassList.Should().Contain("my-special-item-dragging-class");

            //drop item in second zone
            await secondDropZone.DropAsync(new DragEventArgs());

            //reload DOM references
            container = comp.Find(".mud-drop-container");
            firstDropZone = container.Children[0];
            firstDropItem = container.Children[1].Children[1];

            firstDropZone.ClassList.Should().NotContain("my-special-dragging-class");
            firstDropItem.ClassList.Should().NotContain("my-special-item-dragging-class");
        }

        [Test]
        public async Task DropZone_DropItem_DragEnterNotTrackedItem()
        {
            var comp = Context.RenderComponent<DropzoneBasicTest>();

            var tempContainer = comp.Find(".mud-drop-container");
            tempContainer.Children.Should().HaveCount(2);

            var firstDropZone = tempContainer.Children[0];
            var firstDropItem = firstDropZone.Children[1];

            await firstDropItem.DragEnterAsync(new DragEventArgs());

            tempContainer.Children.Should().HaveCount(2);
            tempContainer.Children[0].Children.Should().HaveCount(2);
            tempContainer.Children[1].Children.Should().HaveCount(3);
        }

        [Test]
        public async Task DropZone_DropNotTrackedItem()
        {
            var comp = Context.RenderComponent<DropzoneBasicTest>();
            {
                var tempContainer = comp.Find(".mud-drop-container");
                tempContainer.Children.Should().HaveCount(2);

                var firstDropZone = tempContainer.Children[0];
                await firstDropZone.DropAsync(new DragEventArgs());
            }

            var container = comp.Find(".mud-drop-container");
            container.Children.Should().HaveCount(2);
            container.Children[0].Children.Should().HaveCount(2);
            container.Children[1].Children.Should().HaveCount(3);
        }

        [Test]
        public async Task DropZone_CheckDropClasses_NotApplyOnDragStarted()
        {
            var comp = Context.RenderComponent<DropzoneCanDropTest>();

            var firstDropZone = comp.Find(".first-drop-zone");
            var secondDropZone = comp.Find(".second-drop-zone");
            var thirdDropZone = comp.Find(".third-drop-zone");

            var dragItem = firstDropZone.Children[1];

            //start dragging
            await dragItem.DragStartAsync(new DragEventArgs());

            //enter sourced drop zone, no class applied
            await firstDropZone.DragEnterAsync(new DragEventArgs());
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            await firstDropZone.DragLeaveAsync(new DragEventArgs());

            //enter second drop zone
            await secondDropZone.DragEnterAsync(new DragEventArgs());
            secondDropZone.ClassList.Should().Contain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //leave second drop zone
            await secondDropZone.DragLeaveAsync(new DragEventArgs());
            secondDropZone.ClassList.Should().NotContain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //enter third drop zone
            await thirdDropZone.DragEnterAsync(new DragEventArgs());
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().Contain("no-drop-class-from-container");

            //leave third drop zone
            await thirdDropZone.DragLeaveAsync(new DragEventArgs());
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            //reenter sourced drop zone, no class applied
            await firstDropZone.DragEnterAsync(new DragEventArgs());
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");
        }

        [Test]
        public async Task DropZone_CheckDropClasses_TransactionNotStarted()
        {
            var comp = Context.RenderComponent<DropzoneCanDropTest>();

            var firstDropZone = comp.Find(".first-drop-zone");
            var secondDropZone = comp.Find(".second-drop-zone");
            var thirdDropZone = comp.Find(".third-drop-zone");

            //enter sourced drop zone, no class applied
            await firstDropZone.DragEnterAsync(new DragEventArgs());
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            await firstDropZone.DragLeaveAsync(new DragEventArgs());

            //enter second drop zone
            await secondDropZone.DragEnterAsync(new DragEventArgs());
            secondDropZone.ClassList.Should().NotContain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //leave second drop zone
            await secondDropZone.DragLeaveAsync(new DragEventArgs());
            secondDropZone.ClassList.Should().NotContain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //enter third drop zone
            await thirdDropZone.DragEnterAsync(new DragEventArgs());
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            //leave third drop zone
            await thirdDropZone.DragLeaveAsync(new DragEventArgs());
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().NotContain("no-drop-class-from-container");
        }

        [Test]
        public async Task DropZone_CheckDropClasses_ApplyOnDrag_OnlySecondZone()
        {
            var comp = Context.RenderComponent<DropzoneCanDropTest>(
                p => p.Add(x => x.SecondColumnAppliesClassesOnDragStarted, true));

            var firstDropZone = comp.Find(".first-drop-zone");
            var secondDropZone = comp.Find(".second-drop-zone");
            var thirdDropZone = comp.Find(".third-drop-zone");

            var dragItem = firstDropZone.Children[1];

            //start dragging
            await dragItem.DragStartAsync(new DragEventArgs());

            //second drop zone should still have classes applied
            secondDropZone.ClassList.Should().Contain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //enter sourced drop zone, no class applied
            await firstDropZone.DragEnterAsync(new DragEventArgs());
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            await firstDropZone.DragLeaveAsync(new DragEventArgs());

            //second drop zone should still have classes applied
            secondDropZone.ClassList.Should().Contain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //enter second drop zone, not changes expected
            await secondDropZone.DragEnterAsync(new DragEventArgs());
            secondDropZone.ClassList.Should().Contain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //leave second drop zone, should still have classes applied
            await secondDropZone.DragLeaveAsync(new DragEventArgs());
            secondDropZone.ClassList.Should().Contain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //enter third drop zone
            await thirdDropZone.DragEnterAsync(new DragEventArgs());
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().Contain("no-drop-class-from-container");

            //second drop zone should still have classes applied
            secondDropZone.ClassList.Should().Contain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //leave third drop zone
            await thirdDropZone.DragLeaveAsync(new DragEventArgs());
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            //second drop zone should still have classes applied
            secondDropZone.ClassList.Should().Contain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");
        }

        [Test]
        public async Task DropZone_CheckDropClasses_ApplyClassesOnDragStarted()
        {
            var comp = Context.RenderComponent<DropzoneCanDropTest>(p =>
            {
                p.Add(x => x.SecondColumnAppliesClassesOnDragStarted, false);
                p.Add(x => x.ApplyDropClassesOnDragStarted, true);
            });

            var firstDropZone = comp.Find(".first-drop-zone");
            var secondDropZone = comp.Find(".second-drop-zone");
            var thirdDropZone = comp.Find(".third-drop-zone");

            var dragItem = firstDropZone.Children[1];

            //start dragging
            await dragItem.DragStartAsync(new DragEventArgs());

            //first zone, as source, should not have styles applied
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            //second zone, should not have styles because it is explicit set to false
            secondDropZone.ClassList.Should().NotContain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //third zone, should have classes applied
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().Contain("no-drop-class-from-container");

            //start dragging around

            // first zone, no changes
            await firstDropZone.DragEnterAsync(new DragEventArgs());

            //first zone, as source, should not have styles applied
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            //second zone, should not have styles because it is explicit set to false
            secondDropZone.ClassList.Should().NotContain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //third zone, should have classes applied
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().Contain("no-drop-class-from-container");

            await firstDropZone.DragLeaveAsync(new DragEventArgs());

            //first zone, as source, should not have styles applied
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            //second zone, should not have styles because it is explicit set to false
            secondDropZone.ClassList.Should().NotContain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //third zone, should have classes applied
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().Contain("no-drop-class-from-container");


            //enter second drop zone
            await secondDropZone.DragEnterAsync(new DragEventArgs());

            //first zone, as source, should not have styles applied
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            //second should now have styles applied
            secondDropZone.ClassList.Should().Contain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //third zone, should have classes applied
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().Contain("no-drop-class-from-container");

            //leave second drop zone
            await secondDropZone.DragLeaveAsync(new DragEventArgs());

            //first zone, as source, should not have styles applied
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            //second zone, should not have classes applied anymore
            secondDropZone.ClassList.Should().NotContain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //third zone, should have classes applied
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().Contain("no-drop-class-from-container");

            //enter third drop zone, no changes expected
            await thirdDropZone.DragEnterAsync(new DragEventArgs());

            //first zone, as source, should not have styles applied
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            //second zone, should not have classes applied anymore
            secondDropZone.ClassList.Should().NotContain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //third zone, should have classes applied
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().Contain("no-drop-class-from-container");

            //leave third drop zone, no changes expected
            await thirdDropZone.DragLeaveAsync(new DragEventArgs());

            //first zone, as source, should not have styles applied
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            //second zone, should not have classes applied anymore
            secondDropZone.ClassList.Should().NotContain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //third zone, should have classes applied
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().Contain("no-drop-class-from-container");
        }

        [Test]
        public async Task DropZone_CheckDropClasses_ApplyClassesOnDragStarted_DragFinished()
        {
            var comp = Context.RenderComponent<DropzoneCanDropTest>(p =>
            {
                p.Add(x => x.SecondColumnAppliesClassesOnDragStarted, false);
                p.Add(x => x.ApplyDropClassesOnDragStarted, true);
            });

            var firstDropZone = comp.Find(".first-drop-zone");
            var secondDropZone = comp.Find(".second-drop-zone");
            var thirdDropZone = comp.Find(".third-drop-zone");

            var dragItem = firstDropZone.Children[1];

            //start dragging
            await dragItem.DragStartAsync(new DragEventArgs());

            //enter second drop zone
            await secondDropZone.DragEnterAsync(new DragEventArgs());

            //drop to second drop zone
            await secondDropZone.DropAsync(new DragEventArgs());

            //first zone, as source, should not have classes applied
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            //second zone, should not have styles because it is explicit set to false
            secondDropZone.ClassList.Should().NotContain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //third zone, should not have classes applied after drop
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().NotContain("no-drop-class-from-container");
        }

        [Test]
        public async Task DropZone_CheckDropClasses_ApplyClassesOnDragStarted_DragCanceled()
        {
            var comp = Context.RenderComponent<DropzoneCanDropTest>(p =>
            {
                p.Add(x => x.SecondColumnAppliesClassesOnDragStarted, false);
                p.Add(x => x.ApplyDropClassesOnDragStarted, true);
            });

            var firstDropZone = comp.Find(".first-drop-zone");
            var secondDropZone = comp.Find(".second-drop-zone");
            var thirdDropZone = comp.Find(".third-drop-zone");

            var dragItem = firstDropZone.Children[1];

            //start dragging
            await dragItem.DragStartAsync(new DragEventArgs());

            //cancel drag transaction
            await dragItem.DragEndAsync(new DragEventArgs());

            //first zone, as source, should not have classes applied
            firstDropZone.ClassList.Should().NotContain("can-drop-from-container");
            firstDropZone.ClassList.Should().NotContain("no-drop-class-from-container");

            //second zone, should not have styles because it is explicit set to false
            secondDropZone.ClassList.Should().NotContain("can-drop-from-zone");
            secondDropZone.ClassList.Should().NotContain("no-drop-class-from-zone");

            //third zone, should not have classes applied after cancellation
            thirdDropZone.ClassList.Should().NotContain("can-drop-from-container");
            thirdDropZone.ClassList.Should().NotContain("no-drop-class-from-container");
        }

        [Test]
        public void DropZone_CheckDropClasses_DisabledItems()
        {
            var comp = Context.RenderComponent<DropzoneDisableTest>();

            var firstDropZone = comp.Find(".first-drop-zone");
            var secondDropZone = comp.Find(".second-drop-zone");

            firstDropZone.Children.Should().HaveCount(3);
            secondDropZone.Children.Should().HaveCount(3);

            var firstDropItem = firstDropZone.Children[1];
            var fourthDropItem = firstDropZone.Children[2];

            var secondDropItem = secondDropZone.Children[1];
            var thirdDropItem = secondDropZone.Children[2];

            //first item should be enabled
            firstDropItem.ClassList.Should().NotContain("my-zone-based-custom-disabled");
            firstDropItem.ClassList.Should().NotContain("my-custom-disabled-class-from-container");
            firstDropItem.GetAttribute("draggable").Should().Be("true");

            //the fourth item should be disabled with zone class applied
            fourthDropItem.ClassList.Should().Contain("my-zone-based-custom-disabled");
            fourthDropItem.ClassList.Should().NotContain("my-custom-disabled-class-from-container");
            fourthDropItem.GetAttribute("draggable").Should().Be("false");

            //the second item should be enabled
            secondDropItem.ClassList.Should().NotContain("my-zone-based-custom-disabled");
            secondDropItem.ClassList.Should().NotContain("my-custom-disabled-class-from-container");
            secondDropItem.GetAttribute("draggable").Should().Be("true");

            //the third item should be disabled and the container class should be applied
            thirdDropItem.ClassList.Should().NotContain("my-zone-based-custom-disabled");
            thirdDropItem.ClassList.Should().Contain("my-custom-disabled-class-from-container");
            thirdDropItem.GetAttribute("draggable").Should().Be("false");

        }

        [Test]
        public async Task DropZone_DynamicItemsChanges()
        {
            var comp = Context.RenderComponent<DropzoneDynamicItemCollectionTest>();

            var firstDropZone = comp.Find(".first-drop-zone");
            var secondDropZone = comp.Find(".second-drop-zone");

            firstDropZone.Children.Should().HaveCount(2);
            secondDropZone.Children.Should().HaveCount(1);

            comp.Instance.AddItem();

            //no new items, since no update
            secondDropZone.Children.Should().HaveCount(1);

            //invoking update
            await comp.Instance.InvokeRefresh();

            //items should have been rendered
            secondDropZone.Children.Should().HaveCount(2);

            //removing item
            comp.Instance.RemoveLastItem();

            //not updated yet
            secondDropZone.Children.Should().HaveCount(2);

            //invoking update
            await comp.Instance.InvokeRefresh();

            //items should have been rendered
            secondDropZone.Children.Should().HaveCount(1);
        }

        [Test]
        public void DropZone_OnlyZone()
        {
            var comp = Context.RenderComponent<DropzoneVisbilityTest>();

            var container = comp.Find(".mud-drop-container");
            container.Children.Should().HaveCount(2);

            var firstDropZone = container.Children[0];
            firstDropZone.Children.Should().ContainSingle();
            firstDropZone.Children[0].TextContent.Should().Be("Drop Zone 1");

            var secondDropZone = container.Children[1];
            secondDropZone.Children.Should().HaveCount(3);

            secondDropZone.Children[0].TextContent.Should().Be("Drop Zone 2");
            secondDropZone.Children[1].TextContent.Should().Be("Second Item");
            secondDropZone.Children[2].TextContent.Should().Be("Third Item");

            comp.SetParametersAndRender(x => x.Add(p => p.HideItemsInFirstDropZone, false));

            container = comp.Find(".mud-drop-container");
            firstDropZone = container.Children[0];
            firstDropZone.Children.Should().HaveCount(2);
            firstDropZone.Children[0].TextContent.Should().Be("Drop Zone 1");
            firstDropZone.Children[1].TextContent.Should().Be("First Item");
        }

        [Test]
        public async Task DropZone_Reorder_PlaceIntoEmptyZone()
        {
            var comp = Context.RenderComponent<DropzoneReorderTest>();

            comp.Find(".mud-drop-container");
            var firstDropZone = comp.Find(".dropzone-1");
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");
            firstDropZone.Children[3].TextContent.Should().Be("Item 2");
            firstDropZone.Children[4].TextContent.Should().Be("Item 3");
            firstDropZone.Children[5].TextContent.Should().Be("Item 4");

            var firstDropItem = firstDropZone.Children[2];

            await firstDropItem.DragStartAsync(new DragEventArgs());

            var thirdDropZone = comp.Find(".dropzone-3");
            thirdDropZone.Children.Should().ContainSingle();
            thirdDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });

            await thirdDropZone.DragEnterAsync(new DragEventArgs());

            thirdDropZone.Children.Should().ContainSingle();
            thirdDropZone.Children[0].ClassList.Should().Contain("mud-dropitem-placeholder").And.NotContain("d-none");

            await thirdDropZone.DropAsync(new DragEventArgs());

            thirdDropZone.Children.Should().HaveCount(3);
            thirdDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            thirdDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            thirdDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            thirdDropZone.Children[2].TextContent.Should().Be("Item 1");

            comp.Instance.IndexHistory.Distinct().Should().ContainSingle().And.Contain(0);
        }

        [Test]
        public async Task DropZone_Reorder_MoveWithinContainer_Down()
        {
            var comp = Context.RenderComponent<DropzoneReorderTest>();

            comp.Find(".mud-drop-container");
            var firstDropZone = comp.Find(".dropzone-1");
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");

            var secondDropItem = firstDropZone.Children[3];
            secondDropItem.TextContent.Should().Be("Item 2");
            await secondDropItem.DragStartAsync(new DragEventArgs());

            var thirdDropItem = firstDropZone.Children[3];
            thirdDropItem.TextContent.Should().Be("Item 3");
            await thirdDropItem.DragEnterAsync(new DragEventArgs());

            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[4].ClassList.Should().Contain("mud-dropitem-placeholder").And.NotContain("d-none");

            await firstDropZone.DropAsync(new DragEventArgs());
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");
            firstDropZone.Children[3].TextContent.Should().Be("Item 3");
            firstDropZone.Children[4].TextContent.Should().Be("Item 2");
            firstDropZone.Children[5].TextContent.Should().Be("Item 4");

            comp.Instance.IndexHistory.Distinct().Should().ContainSingle().And.Contain(2);
        }

        [Test]
        public async Task DropZone_Reorder_MoveWithinContainer_Up()
        {
            var comp = Context.RenderComponent<DropzoneReorderTest>();

            comp.Find(".mud-drop-container");
            var firstDropZone = comp.Find(".dropzone-1");
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");

            var thirdDropItem = firstDropZone.Children[4];
            thirdDropItem.TextContent.Should().Be("Item 3");
            await thirdDropItem.DragStartAsync(new DragEventArgs());

            var firstDropItem = firstDropZone.Children[1];
            firstDropItem.TextContent.Should().Be("Item 1");
            await firstDropItem.DragEnterAsync(new DragEventArgs());

            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[2].ClassList.Should().Contain("mud-dropitem-placeholder").And.NotContain("d-none");

            await firstDropZone.DropAsync(new DragEventArgs());
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");
            firstDropZone.Children[3].TextContent.Should().Be("Item 3");
            firstDropZone.Children[4].TextContent.Should().Be("Item 2");
            firstDropZone.Children[5].TextContent.Should().Be("Item 4");

            comp.Instance.IndexHistory.Distinct().Should().ContainSingle().And.Contain(1);

        }

        [Test]
        public async Task DropZone_Reorder_MoveWithinContainer_ToBottom()
        {
            var comp = Context.RenderComponent<DropzoneReorderTest>();

            comp.Find(".mud-drop-container");
            var firstDropZone = comp.Find(".dropzone-1");
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");

            var secondDropItem = firstDropZone.Children[3];
            secondDropItem.TextContent.Should().Be("Item 2");
            await secondDropItem.DragStartAsync(new DragEventArgs());

            var lastDropItem = firstDropZone.Children[4];
            lastDropItem.TextContent.Should().Be("Item 4");
            await lastDropItem.DragEnterAsync(new DragEventArgs());

            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[5].ClassList.Should().Contain("mud-dropitem-placeholder").And.NotContain("d-none");

            await firstDropZone.DropAsync(new DragEventArgs());
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");
            firstDropZone.Children[3].TextContent.Should().Be("Item 3");
            firstDropZone.Children[4].TextContent.Should().Be("Item 4");
            firstDropZone.Children[5].TextContent.Should().Be("Item 2");

            comp.Instance.IndexHistory.Distinct().Should().ContainSingle().And.Contain(3);

        }

        [Test]
        public async Task DropZone_Reorder_MoveWithinContainer_Top()
        {
            var comp = Context.RenderComponent<DropzoneReorderTest>();

            comp.Find(".mud-drop-container");
            var firstDropZone = comp.Find(".dropzone-1");
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");

            var thirdDropItem = firstDropZone.Children[4];
            thirdDropItem.TextContent.Should().Be("Item 3");
            await thirdDropItem.DragStartAsync(new DragEventArgs());

            var firstDropItem = firstDropZone.Children[0];
            firstDropItem.TextContent.Should().BeEmpty();
            await firstDropItem.DragEnterAsync(new DragEventArgs());

            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain("mud-dropitem-placeholder").And.NotContain("d-none");

            await firstDropZone.DropAsync(new DragEventArgs());
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 3");
            firstDropZone.Children[3].TextContent.Should().Be("Item 1");
            firstDropZone.Children[4].TextContent.Should().Be("Item 2");
            firstDropZone.Children[5].TextContent.Should().Be("Item 4");

            comp.Instance.IndexHistory.Distinct().Should().ContainSingle().And.Contain(0);
        }

        [Test]
        public async Task DropZone_Reorder_MoveWithinContainer_NoChange()
        {
            var comp = Context.RenderComponent<DropzoneReorderTest>();

            comp.Find(".mud-drop-container");
            var firstDropZone = comp.Find(".dropzone-1");
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");

            var secondDropItem = firstDropZone.Children[3];
            secondDropItem.TextContent.Should().Be("Item 2");
            await secondDropItem.DragStartAsync(new DragEventArgs());

            await firstDropZone.DropAsync(new DragEventArgs());
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");
            firstDropZone.Children[3].TextContent.Should().Be("Item 2");
            firstDropZone.Children[4].TextContent.Should().Be("Item 3");
            firstDropZone.Children[5].TextContent.Should().Be("Item 4");

            comp.Instance.IndexHistory.Distinct().Should().ContainSingle().And.Contain(1);
        }

        [Test]
        public async Task DropZone_Reorder_MoveBetweenZones()
        {
            var comp = Context.RenderComponent<DropzoneReorderTest>();

            comp.Find(".mud-drop-container");
            var firstDropZone = comp.Find(".dropzone-1");
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");

            var secondDropZone = comp.Find(".dropzone-2");

            var secondDropItemInFirstZone = firstDropZone.Children[3];
            secondDropItemInFirstZone.TextContent.Should().Be("Item 2");
            await secondDropItemInFirstZone.DragStartAsync(new DragEventArgs());

            await secondDropZone.DragEnterAsync(new DragEventArgs());

            var firstItemInSecondDropZone = secondDropZone.Children[3];
            firstItemInSecondDropZone.TextContent.Should().Be("Item 6");
            await firstItemInSecondDropZone.DragEnterAsync(new DragEventArgs());

            secondDropZone.Children.Should().HaveCount(4);
            secondDropZone.Children[3].ClassList.Should().Contain("mud-dropitem-placeholder").And.NotContain("d-none");

            await secondDropZone.DropAsync(new DragEventArgs());
            firstDropZone.Children.Should().HaveCount(5);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");
            firstDropZone.Children[3].TextContent.Should().Be("Item 3");
            firstDropZone.Children[4].TextContent.Should().Be("Item 4");

            secondDropZone.Children.Should().HaveCount(5);
            secondDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            secondDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            secondDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            secondDropZone.Children[2].TextContent.Should().Be("Item 5");
            secondDropZone.Children[3].TextContent.Should().Be("Item 6");
            secondDropZone.Children[4].TextContent.Should().Be("Item 2");

            await secondDropZone.Children[3].DragStartAsync(new DragEventArgs());
            await firstDropZone.DragEnterAsync(new DragEventArgs());
            await firstDropZone.Children[3].DragEnterAsync(new DragEventArgs());
            await firstDropZone.DropAsync(new DragEventArgs());

            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");
            firstDropZone.Children[3].TextContent.Should().Be("Item 3");
            firstDropZone.Children[4].TextContent.Should().Be("Item 6");
            firstDropZone.Children[5].TextContent.Should().Be("Item 4");

            secondDropZone.Children.Should().HaveCount(4);
            secondDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            secondDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            secondDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            secondDropZone.Children[2].TextContent.Should().Be("Item 5");
            secondDropZone.Children[3].TextContent.Should().Be("Item 2");

            await firstDropZone.Children[4].DragStartAsync(new DragEventArgs());
            await secondDropZone.DragEnterAsync(new DragEventArgs());
            await secondDropZone.Children[3].DragEnterAsync(new DragEventArgs());
            await secondDropZone.DropAsync(new DragEventArgs());

            firstDropZone.Children.Should().HaveCount(5);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");
            firstDropZone.Children[3].TextContent.Should().Be("Item 3");
            firstDropZone.Children[4].TextContent.Should().Be("Item 4");

            secondDropZone.Children.Should().HaveCount(5);
            secondDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            secondDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            secondDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            secondDropZone.Children[2].TextContent.Should().Be("Item 5");
            secondDropZone.Children[3].TextContent.Should().Be("Item 2");
            secondDropZone.Children[4].TextContent.Should().Be("Item 6");

            await secondDropZone.Children[3].DragStartAsync(new DragEventArgs());
            await firstDropZone.DragEnterAsync(new DragEventArgs());
            await firstDropZone.Children[2].DragEnterAsync(new DragEventArgs());
            await firstDropZone.DropAsync(new DragEventArgs());

            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");
            firstDropZone.Children[3].TextContent.Should().Be("Item 2");
            firstDropZone.Children[4].TextContent.Should().Be("Item 3");
            firstDropZone.Children[5].TextContent.Should().Be("Item 4");

            secondDropZone.Children.Should().HaveCount(4);
            secondDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            secondDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            secondDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            secondDropZone.Children[2].TextContent.Should().Be("Item 5");
            secondDropZone.Children[3].TextContent.Should().Be("Item 6");

            comp.Instance.IndexHistory.Should().ContainInOrder(
                new[] { 2, 2, 2, 1 });

        }

        [Test]
        public async Task DropZone_Item_ClassSelector()
        {
            var comp = Context.RenderComponent<DropzoneItemClassSelectorTest>();

            comp.Find(".mud-drop-container");
            var firstDropZone = comp.Find(".dropzone-1");
            var secondDropItemInFirstZone = firstDropZone.Children[3];
            secondDropItemInFirstZone.TextContent.Should().Be("Item 2");
            secondDropItemInFirstZone.ClassList.Should().Contain("mud-theme-primary");

            var secondDropZone = comp.Find(".dropzone-2");

            await secondDropItemInFirstZone.DragStartAsync(new DragEventArgs());

            await secondDropZone.DragEnterAsync(new DragEventArgs());

            var firstItemInSecondDropZone = secondDropZone.Children[3];
            await firstItemInSecondDropZone.DragEnterAsync(new DragEventArgs());

            await secondDropZone.DropAsync(new DragEventArgs());
            secondDropZone.Children.Should().HaveCount(5);
            secondDropZone.Children[4].TextContent.Should().Be("Item 2");
            secondDropZone.Children[4].ClassList.Should().Contain("mud-theme-secondary");
        }

        [Test]
        public async Task DropZone_Item_OnItemPicked()
        {
            var comp = Context.RenderComponent<DropzoneItemOnItemPickedTest>();

            comp.Find(".mud-drop-container");
            var firstDropZone = comp.Find(".dropzone-1");
            var secondDropItemInFirstZone = firstDropZone.Children[3];
            secondDropItemInFirstZone.TextContent.Should().Be("Item 2");
            await secondDropItemInFirstZone.DragStartAsync(new DragEventArgs());

            var secondDropZone = comp.Find(".dropzone-2");
            await secondDropZone.DragEnterAsync(new DragEventArgs());

            var firstItemInSecondDropZone = secondDropZone.Children[3];
            await firstItemInSecondDropZone.DragEnterAsync(new DragEventArgs());

            var message = comp.FindAll(".mud-typography")
                .Select(c => c.TextContent)
                .Where(t => t.Contains("Draging Started")).FirstOrDefault();
            message.Should().Be("Draging Started for [Item 2]");
        }

        [Test]
        public async Task DropZone_Reorder_NoPreviewOnSameItem()
        {
            var comp = Context.RenderComponent<DropzoneReorderTest>();

            comp.Find(".mud-drop-container");
            var firstDropZone = comp.Find(".dropzone-1");
            firstDropZone.Children.Should().HaveCount(6);
            firstDropZone.Children[0].ClassList.Should().Contain(new[] { "d-none", "mud-dropitem-placeholder" });
            firstDropZone.Children[1].ClassList.Should().Contain("mud-drop-item-preview-start");
            firstDropZone.Children[1].GetAttribute("draggable").Should().Be("false");
            firstDropZone.Children[2].TextContent.Should().Be("Item 1");

            var thirdDropItem = firstDropZone.Children[4];
            thirdDropItem.TextContent.Should().Be("Item 3");
            await thirdDropItem.DragStartAsync(new DragEventArgs());

            firstDropZone.Children.Should().HaveCount(5);
            firstDropZone.Children[0].TextContent.Should().BeNullOrEmpty();
            firstDropZone.Children[1].TextContent.Should().Be("Item 1");
            firstDropZone.Children[2].TextContent.Should().Be("Item 2");
            firstDropZone.Children[3].TextContent.Should().Be("Item 3");
            firstDropZone.Children[4].TextContent.Should().Be("Item 4");

            await firstDropZone.Children[3].DragEnterAsync(new DragEventArgs());

            firstDropZone.Children.Should().HaveCount(5);
            firstDropZone.Children[0].TextContent.Should().BeNullOrEmpty();
            firstDropZone.Children[1].TextContent.Should().Be("Item 1");
            firstDropZone.Children[2].TextContent.Should().Be("Item 2");
            firstDropZone.Children[3].TextContent.Should().Be("Item 3");
            firstDropZone.Children[4].TextContent.Should().Be("Item 4");

            await firstDropZone.Children[2].DragEnterAsync(new DragEventArgs());

            firstDropZone.Children.Should().HaveCount(5);
            firstDropZone.Children[0].TextContent.Should().BeNullOrEmpty();
            firstDropZone.Children[1].TextContent.Should().Be("Item 1");
            firstDropZone.Children[2].TextContent.Should().Be("Item 2");
            firstDropZone.Children[3].TextContent.Should().Be("Item 3");
            firstDropZone.Children[4].TextContent.Should().Be("Item 4");
        }

        [Test]
        public async Task DropZone_DragFinished_DropNotAllowed_KeepOrder()
        {
            var comp = Context.RenderComponent<DropzoneDraggingTestCantDropSecondZoneTest>();

            var container = comp.Find(".mud-drop-container");
            container.Children.Should().HaveCount(2);

            var firstDropZone = container.Children[0];
            var secondDropZone = container.Children[1];

            secondDropZone.Children.Should().HaveCount(3);
            var secondDropItem = secondDropZone.Children[1];
            var secondDropItemText = secondDropItem.Children[0].TextContent;

            secondDropItemText.Should().Be("Second Item");

            await secondDropItem.DragStartAsync(new DragEventArgs());

            //drop item in first zone
            await firstDropZone.DropAsync(new DragEventArgs());

            //reload DOM references
            container = comp.Find(".mud-drop-container");
            secondDropZone = container.Children[1];
            secondDropItem = secondDropZone.Children[1];
            secondDropItemText = secondDropItem.Children[0].TextContent;

            secondDropItemText.Should().Be("Second Item");
        }


        [Test]
        public async Task DropZone_IsOriginTest()
        {
            var comp = Context.RenderComponent<DropzoneBasicTest>();

            var container = comp.Find(".mud-drop-container");
            container.Children.Should().HaveCount(2);

            var firstDropZone = container.Children[0];
            firstDropZone.Children.Should().HaveCount(2);

            var firstDropItem = firstDropZone.Children[1];

            firstDropItem.TextContent.Should().Be("First Item");
            await firstDropItem.DragStartAsync(new DragEventArgs());

            var containerComponent = comp.FindComponent<MudDropContainer<TestComponents.DropzoneBasicTest.SimpleDropItem>>();
            containerComponent.Instance.IsOrigin(0, "Column 1").Should().Be(true);
#pragma warning disable CS0618 // Type or member is obsolete
            containerComponent.Instance.IsOrign(0, "Column 1").Should().Be(true);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Test]
        public async Task DropZone_GetTransactionOrignZoneIdentifierTest()
        {
            var comp = Context.RenderComponent<DropzoneBasicTest>();

            var container = comp.Find(".mud-drop-container");
            container.Children.Should().HaveCount(2);

            var firstDropZone = container.Children[0];
            firstDropZone.Children.Should().HaveCount(2);

            var firstDropItem = firstDropZone.Children[1];

            firstDropItem.TextContent.Should().Be("First Item");
            await firstDropItem.DragStartAsync(new DragEventArgs());

            var containerComponent = comp.FindComponent<MudDropContainer<TestComponents.DropzoneBasicTest.SimpleDropItem>>();
            containerComponent.Instance.GetTransactionOrignZoneIdentifier().Should().Be("Column 1");
#pragma warning disable CS0618 // Type or member is obsolete
            containerComponent.Instance.GetTransactionOrignZoneIdentiifer().Should().Be("Column 1");
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
