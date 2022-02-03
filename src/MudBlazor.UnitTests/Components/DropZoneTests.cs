
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
            container.ItemIsDisabled.Should().BeNull();
            container.Items.Should().BeNull();
            container.ItemsSelector.Should().BeNull();
            container.NoDropClass.Should().BeNullOrEmpty();
        }

        [Test]
        public void DropZone_Defaults()
        {
            var container = new MudDropZone<object>();

            container.ApplyDropClassesOnDragStarted.Should().BeNull();
            container.CanDrop.Should().BeNull();
            container.CanDropClass.Should().BeNullOrEmpty();
            container.DisabledClass.Should().BeNullOrEmpty();
            container.DraggingClass.Should().BeNullOrEmpty();
            container.ItemDraggingClass.Should().BeNullOrEmpty();
            container.ItemIsDisabled.Should().BeNull();
            container.ItemsSelector.Should().BeNull();
            container.NoDropClass.Should().BeNullOrEmpty();
        }

        [Test]
        public void DropItem_Defaults()
        {
            var container = new MudDynamicDropItem<object>();

            container.Disabled.Should().BeFalse();
            container.DisabledClass.Should().BeNullOrEmpty();
            container.DraggingClass.Should().BeNullOrEmpty();
            container.ZoneIdentifier.Should().BeNullOrEmpty();
            container.Item.Should().BeNull();
        }

        [Test]
        public void DropZone_DisposeWork()
        {
            var container = new MudDropZone<object>();
            container.Dispose();
        }

        [Test]
        public void DropZone_GenerelView()
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
            Mock<IJSRuntime> _jsruntimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            Context.Services.AddSingleton(typeof(IJSRuntime), _jsruntimeMock.Object);

            _jsruntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudDragAndDrop.initDropZone", It.Is<object[]>(y => y.Length == 1 && Guid.Parse(y[0].ToString()) != Guid.Empty)))
                .ReturnsAsync(Mock.Of<IJSVoidResult>(), TimeSpan.FromMilliseconds(200)).Verifiable();

            var comp = Context.RenderComponent<DropzoneBasicTest>();

            _jsruntimeMock.Verify();
        }

        [Test]
        public void DropZone_DropzoneOverrideContainerRendered()
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

            var secondDropZone = container.Children[1];
            var firstDropItem = firstDropZone.Children[1];

            firstDropZone.ClassList.Should().NotContain("mud-drop-zone-drag-block");
            secondDropZone.ClassList.Should().NotContain("mud-drop-zone-drag-block");

            await firstDropItem.DragStartAsync(new DragEventArgs());

            firstDropZone = comp.Find(".first-drop-zone");
            firstDropZone.ClassList.Should().NotContain("mud-drop-zone-drag-block");

            secondDropZone = comp.Find(".second-drop-zone");
            secondDropZone.ClassList.Should().Contain("mud-drop-zone-drag-block");

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
        }

        [Test]
        public async Task DropZone_DragAndDropDraggingClass_DragCancelled()
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
    }
}
