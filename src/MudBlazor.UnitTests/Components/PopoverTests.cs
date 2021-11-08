﻿
#pragma warning disable CS1998 // async without await

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.UnitTests.TestComponents.Popover;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class PopoverTests : BunitTest
    {
        [Test]
        public void PopoverOptions_Defaults()
        {
            var options = new PopoverOptions();

            options.ContainerClass.Should().Be("mudblazor-main-content");
            options.FlipMargin.Should().Be(0);
        }

        [Test]
        public void MudPopoverHandler_Constructor()
        {
            RenderFragment renderFragement = (tree) => { };
            var mock = Mock.Of<IJSRuntime>();
            Action updater = () => { };

            var handler = new MudPopoverHandler(renderFragement, mock, updater);

            handler.Id.Should().NotBe(default(Guid));
            handler.UserAttributes.Should().BeEmpty();
            handler.Class.Should().BeNull();
            handler.Tag.Should().BeNull();
            handler.Fragment.Should().BeSameAs(renderFragement);
            handler.IsConnected.Should().BeFalse();
            handler.ShowContent.Should().BeFalse();
        }

        [Test]
        public void MudPopoverHandler_PreventNullValuesInConstructor()
        {
            RenderFragment renderFragement = (tree) => { };
            var mock = Mock.Of<IJSRuntime>();
            Action updater = () => { };

            Assert.Throws<ArgumentNullException>(() => new MudPopoverHandler(null, mock, updater));
            Assert.Throws<ArgumentNullException>(() => new MudPopoverHandler(renderFragement, null, updater));
            Assert.Throws<ArgumentNullException>(() => new MudPopoverHandler(renderFragement, mock, null));
        }

        [Test]
        public void MudPopoverHandler_SetComponentBaseParameters()
        {
            RenderFragment renderFragement = (tree) => { };
            var mock = Mock.Of<IJSRuntime>();
            Action updater = () => { };

            var handler = new MudPopoverHandler(renderFragement, mock, updater);

            var comp = Context.RenderComponent<MudBadge>(p =>
            {
                p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
                p.Add(x => x.Tag, "my tag");

            });

            handler.SetComponentBaseParameters(comp.Instance, "my-extra-class", "my-extra-style:2px", true);

            handler.Id.Should().NotBe(default(Guid));
            handler.UserAttributes.Should().BeEquivalentTo(new Dictionary<string, object> { { "myprop1", "myValue1" } });
            handler.Class.Should().Be("my-extra-class");
            handler.Tag.Should().Be("my tag");
            handler.Fragment.Should().BeSameAs(renderFragement);
            handler.IsConnected.Should().BeFalse();
            handler.ShowContent.Should().BeTrue();
        }

        [Test]
        public void MudPopoverHandler_UpdateFragment()
        {
            RenderFragment initialRenderFragement = (tree) => { };
            var mock = Mock.Of<IJSRuntime>();

            var updateCounter = 0;
            Action updater = () => { updateCounter++; };

            var handler = new MudPopoverHandler(initialRenderFragement, mock, updater);

            var comp = Context.RenderComponent<MudBadge>(p =>
            {
                p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
                p.Add(x => x.Tag, "my tag");

            });

            RenderFragment newRenderFragement = (tree) => { };

            handler.UpdateFragment(newRenderFragement, comp.Instance, "my-extra-class", "my-extra-style:2px", true);

            handler.Id.Should().NotBe(default(Guid));
            handler.UserAttributes.Should().BeEquivalentTo(new Dictionary<string, object> { { "myprop1", "myValue1" } });
            handler.Class.Should().Be("my-extra-class");
            handler.Style.Should().Be("my-extra-style:2px");
            handler.Tag.Should().Be("my tag");
            handler.Fragment.Should().BeSameAs(newRenderFragement);
            handler.IsConnected.Should().BeFalse();
            handler.ShowContent.Should().BeTrue();

            updateCounter.Should().Be(1);
        }

        [Test]
        public void MudPopoverHandler_DontUpdateWhenLockIsEnabled()
        {
            RenderFragment initialRenderFragement = (tree) => { };
            var mock = Mock.Of<IJSRuntime>();

            var updateCounter = 0;
            Action updater = () => { updateCounter++; };

            var handler = new MudPopoverHandler(initialRenderFragement, mock, updater);

            var comp = Context.RenderComponent<MudBadge>(p =>
            {
                p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
                p.Add(x => x.Tag, "my tag");

            });

            RenderFragment newRenderFragement = (tree) => { };

            for (int i = 0; i < 4; i++)
            {
                handler.UpdateFragment(newRenderFragement, comp.Instance, "my-extra-class", "my-extra-style:2px", i % 2 == 0);
            }

            updateCounter.Should().Be(1);
        }

        [Test]
        public void MudPopoverHandler_UpdateAndLockCycle()
        {
            RenderFragment initialRenderFragement = (tree) => { };
            var mock = Mock.Of<IJSRuntime>();

            var updateCounter = 0;
            Action updater = () => { updateCounter++; };

            var handler = new MudPopoverHandler(initialRenderFragement, mock, updater);

            var comp = Context.RenderComponent<MudBadge>(p =>
            {
                p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
                p.Add(x => x.Tag, "my tag");

            });

            RenderFragment newRenderFragement = (tree) => { };

            for (int i = 0; i < 4; i++)
            {
                handler.UpdateFragment(newRenderFragement, comp.Instance, "my-extra-class", "my-extra-style:2px", i % 2 == 0);
            }
            updateCounter.Should().Be(1);

            handler.Release();

            handler.UpdateFragment(newRenderFragement, comp.Instance, "my-new-extra-class", "my-new-extra-style:2px", true);

            updateCounter.Should().Be(2);

            handler.Class.Should().Be("my-new-extra-class");
            handler.Style.Should().Be("my-new-extra-style:2px");
        }

        [Test]
        public async Task MudPopoverHandler_InitizeAndDetach()
        {
            var handlerId = Guid.NewGuid();

            RenderFragment renderFragement = (tree) => { };
            var mock = new Mock<IJSRuntime>();
            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
                "mudPopover.connect",
                It.Is<object[]>(x => x.Length == 1 && (Guid)x[0] == handlerId))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
                "mudPopover.disconnect",
                It.Is<object[]>(x => x.Length == 1 && (Guid)x[0] == handlerId))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            var updateCounter = 0;
            Action updater = () => { updateCounter++; };

            var handler = new MudPopoverHandler(renderFragement, mock.Object, updater);
            handlerId = handler.Id;

            handler.IsConnected.Should().BeFalse();
            await handler.Initialize();

            handler.IsConnected.Should().BeTrue();

            await handler.Detach();
            handler.IsConnected.Should().BeFalse();
        }

        [Test]
        public async Task MudPopoverHandler_InitizeAndDetach_DetachThrowsTaskCancelledException()
        {
            var handlerId = Guid.NewGuid();

            RenderFragment renderFragement = (tree) => { };
            var mock = new Mock<IJSRuntime>();
            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
                "mudPopover.connect",
                It.Is<object[]>(x => x.Length == 1 && (Guid)x[0] == handlerId))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
                "mudPopover.disconnect",
                It.Is<object[]>(x => x.Length == 1 && (Guid)x[0] == handlerId))).ThrowsAsync(new TaskCanceledException()).Verifiable();

            var updateCounter = 0;
            Action updater = () => { updateCounter++; };

            var handler = new MudPopoverHandler(renderFragement, mock.Object, updater);
            handlerId = handler.Id;

            await handler.Initialize();
            //task canceled exception shoudn't result in an exception
            await handler.Detach();
            handler.IsConnected.Should().BeFalse();
        }

        [Test]
        public async Task MudPopoverHandler_InitizeAndDetach_DetachThrowsNotTaskCancelledException()
        {
            var handlerId = Guid.NewGuid();

            RenderFragment renderFragement = (tree) => { };
            var mock = new Mock<IJSRuntime>();
            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
                "mudPopover.connect",
                It.Is<object[]>(x => x.Length == 1 && (Guid)x[0] == handlerId))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
                "mudPopover.disconnect",
                It.Is<object[]>(x => x.Length == 1 && (Guid)x[0] == handlerId))).ThrowsAsync(new InvalidOperationException()).Verifiable();

            var updateCounter = 0;
            Action updater = () => { updateCounter++; };

            var handler = new MudPopoverHandler(renderFragement, mock.Object, updater);
            handlerId = handler.Id;

            await handler.Initialize();
            //exception of the js interop should result in an exception
            Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.Detach());

            //despite the exception the handler should be disconnected
            handler.IsConnected.Should().BeFalse();
        }

        [Test]
        public void MudPopoverService_Constructor_NoJsInterop()
        {
            Assert.Throws<ArgumentNullException>(() => new MudPopoverService(null));
        }

        [Test]
        public async Task MudPopoverService_Constructor_NullOption()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initilize",
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            {
                var service = new MudPopoverService(mock.Object, null);
                await service.InitializeIfNeeded();
            }
            {
                var service = new MudPopoverService(mock.Object);
                await service.InitializeIfNeeded();
            }
            mock.Verify();
        }

        [Test]
        public async Task MudPopoverService_Initialize_Catch_JSDisconnectedException()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initilize",
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0))).ThrowsAsync(new JSDisconnectedException("JSDisconnectedException")).Verifiable();
            {
                var service = new MudPopoverService(mock.Object);
                await service.InitializeIfNeeded();
            }
            mock.Verify();
        }

        [Test]
        public async Task MudPopoverService_Initialize_Catch_TaskCancelledException()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initilize",
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0))).ThrowsAsync(new TaskCanceledException()).Verifiable();
            {
                var service = new MudPopoverService(mock.Object);
                await service.InitializeIfNeeded();
            }
            mock.Verify();
        }

        [Test]
        public async Task MudPopoverService_Constructor_OptionWithCustomClass()
        {
            var mock = new Mock<IJSRuntime>();

            var option = new PopoverOptions
            {
                ContainerClass = "my-custom-class",
                FlipMargin = 12,
            };

            var optionMock = new Mock<IOptions<PopoverOptions>>();
            optionMock.SetupGet(x => x.Value).Returns(option);

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initilize",
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "my-custom-class" && (int)x[1] == 12))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            var service = new MudPopoverService(mock.Object, optionMock.Object);

            await service.InitializeIfNeeded();

            mock.Verify();
        }

        [Test]
        public async Task MudPopoverService_CallInitializeOnlyOnce()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initilize",
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0))).ReturnsAsync(Mock.Of<IJSVoidResult>(), TimeSpan.FromMilliseconds(300)).Verifiable();

            Task[] tasks = new Task[5];
            var service = new MudPopoverService(mock.Object);

            for (int i = 0; i < 5; i++)
            {
                tasks[i] = Task.Run(async () => await service.InitializeIfNeeded());
            }

            Task.WaitAll(tasks);

            mock.Verify(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initilize",
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0)), Times.Once());
        }

        [Test]
        public async Task MudPopoverService_OnlyDisposeIfConnected()
        {
            var service = new MudPopoverService(Mock.Of<IJSRuntime>(MockBehavior.Strict));
            await service.DisposeAsync();
        }

        [Test]
        public async Task MudPopoverService_DisposeAsync()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initilize",
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
            "mudPopover.dispose",
            It.Is<object[]>(x => x.Length == 0))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            var service = new MudPopoverService(mock.Object);
            await service.InitializeIfNeeded();

            await service.DisposeAsync();

            mock.Verify();
        }

        [Test]
        public async Task MudPopoverService_DisposeAsync_WithTaskCancelException()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initilize",
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
            "mudPopover.dispose",
            It.Is<object[]>(x => x.Length == 0))).ThrowsAsync(new TaskCanceledException()).Verifiable();

            var service = new MudPopoverService(mock.Object);
            await service.InitializeIfNeeded();

            //dispose shouldn't throw an exception in task a TaskCanceledException happend
            await service.DisposeAsync();

            mock.Verify();
        }

        [Test]
        public async Task MudPopoverService_DisposeAsync_ThrowsExceptionIfNotTaskCancelException()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initilize",
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
            "mudPopover.dispose",
            It.Is<object[]>(x => x.Length == 0))).ThrowsAsync(new InvalidOperationException()).Verifiable();

            var service = new MudPopoverService(mock.Object);
            await service.InitializeIfNeeded();

            //any other exception (despite TaskCancelException, should result in an exception
            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DisposeAsync());

            mock.Verify();
        }

        [Test]
        public void MudPopoverService_RegisterAndUseHandler()
        {
            var service = new MudPopoverService(Mock.Of<IJSRuntime>(MockBehavior.Strict));

            int fragmentChangedCounter = 0;

            service.FragmentsChanged += (e, args) =>
            {
                fragmentChangedCounter++;
            };

            RenderFragment fragment = (builder) => { };

            var handler = service.Register(fragment);

            handler.Should().NotBeNull();
            fragmentChangedCounter.Should().Be(1);

            RenderFragment changedFragment = (builder) => { };

            var comp = Context.RenderComponent<MudBadge>(p =>
            {
                p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
                p.Add(x => x.Tag, "my tag");

            });

            handler.UpdateFragment(changedFragment, comp.Instance, "my-class", "my-style", true);
            fragmentChangedCounter.Should().Be(2);
        }

        [Test]
        public async Task MudPopoverService_Unregister_NullFragment()
        {
            var service = new MudPopoverService(Mock.Of<IJSRuntime>(MockBehavior.Strict));

            var result = await service.Unregister(null);
            result.Should().BeFalse();
        }

        [Test]
        public async Task MudPopoverService_Unregister_HandlerNotFound()
        {
            var service = new MudPopoverService(Mock.Of<IJSRuntime>(MockBehavior.Strict));

            var handler = new MudPopoverHandler((tree) => { }, Mock.Of<IJSRuntime>(MockBehavior.Strict), () => { });
            var result = await service.Unregister(handler);

            result.Should().BeFalse();
        }

        [Test]
        public async Task MudPopoverService_Unregister_NotConnected()
        {
            var service = new MudPopoverService(Mock.Of<IJSRuntime>(MockBehavior.Strict));

            RenderFragment fragment = (builder) => { };

            var handler = service.Register(fragment);
            var result = await service.Unregister(handler);

            result.Should().BeFalse();
        }

        [Test]
        public async Task MudPopoverService_Unregister()
        {
            var handlerId = Guid.NewGuid();
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.connect",
               It.Is<object[]>(x => x.Length == 1 && (Guid)x[0] == handlerId))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
           "mudPopover.disconnect",
           It.Is<object[]>(x => x.Length == 1 && (Guid)x[0] == handlerId))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();


            var service = new MudPopoverService(mock.Object);

            await service.InitializeIfNeeded();

            RenderFragment fragment = (builder) => { };

            var handler = service.Register(fragment);
            handlerId = handler.Id;

            await handler.Initialize();

            int fragmentChangedCounter = 0;

            service.FragmentsChanged += (e, args) =>
            {
                fragmentChangedCounter++;
            };

            var result = await service.Unregister(handler);

            result.Should().BeTrue();
            fragmentChangedCounter.Should().Be(1);

            var secondResult = await service.Unregister(handler);
            secondResult.Should().BeFalse();

            mock.Verify();
        }

        [Test]
        public void MudPopover_DefaultValues()
        {
            var popover = new MudPopover();

            popover.MaxHeight.Should().BeNull();
            popover.Paper.Should().BeTrue();
            popover.Elevation.Should().Be(8);
            popover.Square.Should().BeFalse();
            popover.Open.Should().BeFalse();
            popover.Fixed.Should().BeFalse();
            popover.AnchorOrigin.Should().Be(Origin.TopLeft);
            popover.TransformOrigin.Should().Be(Origin.TopLeft);
            popover.RelativeWidth.Should().BeFalse();
            popover.OverflowBehavior.Should().Be(OverflowBehavior.FilpOnOpen);
            popover.Duration.Should().Be(251);
        }

        [Test]
        public async Task MudPopover_OpenAndClose()
        {
            var comp = Context.RenderComponent<PopoverTest>();
            Console.WriteLine(comp.Markup);

            //popup is close, so only the popover-content should be there
            var provider = comp.Find(".mud-popover-provider");
            provider.Children.Should().ContainSingle();

            var popoverNode = comp.Find(".popoverparent").Children[1];
            popoverNode.Id.Should().StartWith("popover-");

            var handlerId = Guid.Parse(popoverNode.Id.Substring(8));
            handlerId.Should().NotBe(Guid.Empty);

            provider.Children.Should().ContainSingle();
            provider.FirstElementChild.Id.Should().Be($"popovercontent-{handlerId}");
            provider.FirstElementChild.Children.Should().BeEmpty();

            //open the popover, content should be visible
            await comp.Instance.Open();

            provider.FirstElementChild.Children.Should().ContainSingle();
            provider.FirstElementChild.Children[0].TextContent.Should().Be("Popover content");

            //close the popover, not content should be visible again
            await comp.Instance.Close();
            provider.FirstElementChild.Children.Should().BeEmpty();
        }

        [Test]
        public void MudPopover_Property_MaxHeight()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(x => x.MaxHeight, 100));

            Console.WriteLine(comp.Markup);

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.GetAttribute("style").Split(';', StringSplitOptions.RemoveEmptyEntries).Should().Contain(new[] { "max-height:100px", "my-custom-style:3px" });
        }

        [Test]
        public void MudPopover_Property_TransitionDuration()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(x => x.Duration, 100));


            Console.WriteLine(comp.Markup);

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.GetAttribute("style").Split(';', StringSplitOptions.RemoveEmptyEntries).Should().Contain(new[] { "transition-duration:100ms" });
        }

        [Test]
        public void MudPopover_Property_Fixed()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.Fixed, true));

            Console.WriteLine(comp.Markup);

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", "mud-popover-fixed", "my-custom-class" });
        }

        [Test]
        public void MudPopover_Property_RelativeWidth()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.RelativeWidth, true));

            Console.WriteLine(comp.Markup);

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", "mud-popover-relative-width", "my-custom-class" });
        }

        [Test]
        public void MudPopover_Property_Paper()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.Paper, true));

            Console.WriteLine(comp.Markup);

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", "mud-paper", "mud-elevation-8", "my-custom-class" });
        }

        [Test]
        public void MudPopover_Property_PaperAndSqaure()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p =>
            {
                p.Add(x => x.Paper, true);
                p.Add(x => x.Square, true);

            });

            Console.WriteLine(comp.Markup);

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", "mud-paper-square", "mud-elevation-8", "my-custom-class" });
        }

        [Test]
        public void MudPopover_Property_Elevation()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p =>
            {
                p.Add(x => x.Paper, true);
                p.Add(x => x.Elevation, 10);

            });

            Console.WriteLine(comp.Markup);

            var popoverElement = comp.Find(".test-popover-content").ParentElement;
            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", "mud-paper", "mud-elevation-10", "my-custom-class" });
        }

        [Test]
        [TestCase(Origin.BottomCenter, "bottom-center")]
        [TestCase(Origin.BottomLeft, "bottom-left")]
        [TestCase(Origin.BottomRight, "bottom-right")]
        [TestCase(Origin.CenterCenter, "center-center")]
        [TestCase(Origin.CenterLeft, "center-left")]
        [TestCase(Origin.CenterRight, "center-right")]
        [TestCase(Origin.TopCenter, "top-center")]
        [TestCase(Origin.TopLeft, "top-left")]
        [TestCase(Origin.TopRight, "top-right")]
        public void MudPopover_Property_TransformOrigin(Origin transformOrigin, string expectedClass)
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.TransformOrigin, transformOrigin));

            Console.WriteLine(comp.Markup);

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", $"mud-popover-{expectedClass}", "my-custom-class" });
        }

        [Test]
        [TestCase(Origin.BottomCenter, "bottom-center")]
        [TestCase(Origin.BottomLeft, "bottom-left")]
        [TestCase(Origin.BottomRight, "bottom-right")]
        [TestCase(Origin.CenterCenter, "center-center")]
        [TestCase(Origin.CenterLeft, "center-left")]
        [TestCase(Origin.CenterRight, "center-right")]
        [TestCase(Origin.TopCenter, "top-center")]
        [TestCase(Origin.TopLeft, "top-left")]
        [TestCase(Origin.TopRight, "top-right")]
        public void MudPopover_Property_AnchorOrigin(Origin anchorOrigin, string expectedClass)
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.AnchorOrigin, anchorOrigin));

            Console.WriteLine(comp.Markup);

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", $"mud-popover-anchor-{expectedClass}", "my-custom-class" });
        }

        [Test]
        [TestCase(OverflowBehavior.FlipNever, "flip-never")]
        [TestCase(OverflowBehavior.FilpOnOpen, "flip-onopen")]
        [TestCase(OverflowBehavior.FlipAlways, "flip-always")]
        public void MudPopover_Property_OverflowBehavior(OverflowBehavior overflowBehavior, string expectedClass)
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.OverflowBehavior, overflowBehavior));

            Console.WriteLine(comp.Markup);

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", $"mud-popover-overflow-{expectedClass}", "my-custom-class" });
        }

        [Test]
        public async Task MudPopover_WithDynamicContent()
        {
            var comp = Context.RenderComponent<PopoverComplexContent>();

            Console.WriteLine(comp.Markup);

            var dynamicContentElement = comp.Find(".dynamic-content");
            dynamicContentElement.ChildNodes.Should().BeEmpty();

            await comp.Instance.AddRow();

            dynamicContentElement.ChildNodes.Should().ContainSingle();
            dynamicContentElement.ChildNodes[0].TextContent.Should().Be("Popover content 0");

            await comp.Instance.AddRow();
            dynamicContentElement.ChildNodes.Should().HaveCount(2);
            dynamicContentElement.ChildNodes[0].TextContent.Should().Be("Popover content 0");
            dynamicContentElement.ChildNodes[1].TextContent.Should().Be("Popover content 1");

            await comp.Instance.AddRow();
            dynamicContentElement.ChildNodes.Should().HaveCount(3);
            dynamicContentElement.ChildNodes[0].TextContent.Should().Be("Popover content 0");
            dynamicContentElement.ChildNodes[1].TextContent.Should().Be("Popover content 1");
            dynamicContentElement.ChildNodes[2].TextContent.Should().Be("Popover content 2");
        }

        [Test]
        public void MudPopoverProvider_DefaultValue()
        {
            var provider = new MudPopoverProvider();
            provider.IsEnabled.Should().BeTrue();
        }

        [Test]
        public void MudPopoverProvider_RenderElementsBasedOnEnableState()
        {
            var comp = Context.RenderComponent<PopoverProviderTest>(p => p.Add(x => x.ProviderIsEnabled, true));

            Console.WriteLine(comp.Markup);
            comp.Find("#my-content").TextContent.Should().Be("Popover content");

            for (int i = 0; i < 3; i++)
            {
                comp.SetParametersAndRender(p => p.Add(x => x.ProviderIsEnabled, false));
                Assert.Throws<ElementNotFoundException>(() => comp.Find("#my-content"));

                comp.SetParametersAndRender(p => p.Add(x => x.ProviderIsEnabled, true));
                comp.Find("#my-content").TextContent.Should().Be("Popover content");
            }
        }

        [Test]
        public void MudPopoverProvider_NoRenderWhenIsEnabledIsFalse()
        {
            var comp = Context.RenderComponent<PopoverProviderTest>(p => p.Add(x => x.ProviderIsEnabled, false));

            Console.WriteLine(comp.Markup);
            Assert.Throws<ElementNotFoundException>(() => comp.Find("#my-content"));
        }
    }
}
