
#pragma warning disable CS1998 // async without await

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.UnitTests.TestComponents;
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
            options.ThrowOnDuplicateProvider.Should().Be(true);
        }

        [Test]
        [Obsolete]
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
        [Obsolete]
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
        [Obsolete]
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

        [Test(Description = "Remove in v7")]
        [Obsolete]
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
        [Obsolete]
        public async Task MudPopoverHandler_UpdateFragmentAsync()
        {
            var mock = Mock.Of<IJSRuntime>();

            var updateCounter = 0;

            RenderFragment initialRenderFragment = _ => { };

            RenderFragment newRenderFragment = _ => { };

            void Updater() => updateCounter++;

            var handler = new MudPopoverHandler(initialRenderFragment, mock, Updater);

            var comp = Context.RenderComponent<MudBadge>(p =>
            {
                p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
                p.Add(x => x.Tag, "my tag");

            });

            await handler.UpdateFragmentAsync(newRenderFragment, comp.Instance, "my-extra-class", "my-extra-style:2px", true);

            handler.Id.Should().NotBe(default(Guid));
            handler.UserAttributes.Should().BeEquivalentTo(new Dictionary<string, object> { { "myprop1", "myValue1" } });
            handler.Class.Should().Be("my-extra-class");
            handler.Style.Should().Be("my-extra-style:2px");
            handler.Tag.Should().Be("my tag");
            handler.Fragment.Should().BeSameAs(newRenderFragment);
            handler.IsConnected.Should().BeFalse();
            handler.ShowContent.Should().BeTrue();

            updateCounter.Should().Be(1);
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverHandler_DetachAndUpdateFragmentAsync()
        {
            var mock = Mock.Of<IJSRuntime>();

            var updateCounter = 0;

            RenderFragment initialRenderFragment = _ => { };

            RenderFragment newRenderFragment = _ => { };

            void Updater() => updateCounter++;

            var handler = new MudPopoverHandler(initialRenderFragment, mock, Updater);

            var comp = Context.RenderComponent<MudBadge>(p =>
            {
                p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
                p.Add(x => x.Tag, "my tag");

            });

            await handler.Detach();
            await handler.UpdateFragmentAsync(newRenderFragment, comp.Instance, "my-new-extra-class", "my-new-extra-style:2px", true);

            updateCounter.Should().Be(0);
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverHandler_DetachAndUpdateFragmentConcurrent_UpdateFragmentDoesNotRunInTheSameTimeAsDetach()
        {
            var connectTcs = new TaskCompletionSource<IJSVoidResult>();

            var mock = new Mock<IJSRuntime>();
            var handler = new MudPopoverHandler((tree) => { }, mock.Object, () => { });

            mock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.connect", It.IsAny<CancellationToken>(), It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == handler.Id)))
                .ReturnsAsync(Mock.Of<IJSVoidResult>())
                .Verifiable();

            mock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.disconnect", It.IsAny<CancellationToken>(), It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == handler.Id)))
                .Returns(new ValueTask<IJSVoidResult>(connectTcs.Task))
                .Verifiable();


            var comp = Context.RenderComponent<MudBadge>(p =>
            {
                p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
                p.Add(x => x.Tag, "my tag");

            });

            RenderFragment newRenderFragement = (tree) => { };
            await handler.Initialize();

            _ = handler.Detach();
            var task2 = handler.UpdateFragmentAsync(newRenderFragement, comp.Instance, "my-new-extra-class", "my-new-extra-style:2px", true);

            var completedTask = await Task.WhenAny(Task.Delay(50), task2);

            completedTask.Should().NotBe(task2);

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverHandler_DetachAndUpdateFragmentConcurrent_UpdateFragmentAsyncShouldRunAfterDetach()
        {
            var connectTcs = new TaskCompletionSource<IJSVoidResult>();

            var mock = new Mock<IJSRuntime>();
            var handler = new MudPopoverHandler(_ => { }, mock.Object, () => { });

            mock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.connect", It.IsAny<CancellationToken>(), It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == handler.Id)))
                .ReturnsAsync(Mock.Of<IJSVoidResult>())
                .Verifiable();

            mock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.disconnect", It.IsAny<CancellationToken>(), It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == handler.Id)))
                .Returns(new ValueTask<IJSVoidResult>(connectTcs.Task))
                .Verifiable();


            var comp = Context.RenderComponent<MudBadge>(p =>
            {
                p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
                p.Add(x => x.Tag, "my tag");

            });

            RenderFragment newRenderFragement = (tree) => { };
            await handler.Initialize();

            var task1 = handler.Detach();
            var task2 = handler.UpdateFragmentAsync(newRenderFragement, comp.Instance, "my-new-extra-class", "my-new-extra-style:2px", true);
            connectTcs.SetResult(Mock.Of<IJSVoidResult>());

            await Task.WhenAll(task1, task2);

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        [Test(Description = "Remove in v7")]
        [Obsolete]
        public void MudPopoverHandler_UpdaterInvokationTest()
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

            for (var i = 0; i < 4; i++)
            {
                handler.UpdateFragment(newRenderFragement, comp.Instance, "my-extra-class", "my-extra-style:2px", i % 2 == 0);
            }
            updateCounter.Should().Be(4);

            handler.UpdateFragment(newRenderFragement, comp.Instance, "my-new-extra-class", "my-new-extra-style:2px", true);

            updateCounter.Should().Be(5);

            handler.Class.Should().Be("my-new-extra-class");
            handler.Style.Should().Be("my-new-extra-style:2px");
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverHandler_UpdaterInvocationAsync()
        {
            var mock = Mock.Of<IJSRuntime>();

            var updateCounter = 0;

            RenderFragment initialRenderFragment = _ => { };

            RenderFragment newRenderFragment = _ => { };

            void Updater() => updateCounter++;

            var handler = new MudPopoverHandler(initialRenderFragment, mock, Updater);

            var comp = Context.RenderComponent<MudBadge>(p =>
            {
                p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
                p.Add(x => x.Tag, "my tag");

            });

            for (var i = 0; i < 4; i++)
            {
                await handler.UpdateFragmentAsync(newRenderFragment, comp.Instance, "my-extra-class", "my-extra-style:2px", i % 2 == 0);
            }
            updateCounter.Should().Be(4);

            await handler.UpdateFragmentAsync(newRenderFragment, comp.Instance, "my-new-extra-class", "my-new-extra-style:2px", true);

            updateCounter.Should().Be(5);

            handler.Class.Should().Be("my-new-extra-class");
            handler.Style.Should().Be("my-new-extra-style:2px");
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverHandler_InitializeAndDetach()
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
        [Obsolete]
        public async Task MudPopoverHandler_InitializeAndDetach_DetachThrowsTaskCanceledException()
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
        [Obsolete]
        public async Task MudPopoverHandler_InitializeAndDetach_DetachThrowsNotTaskCanceledException()
        {
            var handlerId = Guid.NewGuid();

            RenderFragment renderFragement = (tree) => { };
            var mock = new Mock<IJSRuntime>();
            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
                "mudPopover.connect",
                It.IsAny<CancellationToken>(),
                It.Is<object[]>(x => x.Length == 1 && (Guid)x[0] == handlerId))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
                "mudPopover.disconnect",
                It.IsAny<CancellationToken>(),
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
        [Obsolete]
        public async Task MudPopoverHandler_InitializeAndDetachConcurrent_DetachDoesNotRunAtSameTimeAsInitialize()
        {
            var connectTcs = new TaskCompletionSource<IJSVoidResult>();

            var mock = new Mock<IJSRuntime>();
            var handler = new MudPopoverHandler(_ => { }, mock.Object, () => { });

            mock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.connect", It.IsAny<CancellationToken>(), It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == handler.Id)))
                .Returns(new ValueTask<IJSVoidResult>(connectTcs.Task))
                .Verifiable();

            _ = handler.Initialize();
            var task2 = handler.Detach();

            var completedTask = await Task.WhenAny(Task.Delay(50), task2);

            completedTask.Should().NotBe(task2);

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverHandler_InitializeAndDetachConcurrent_DetachRunsAfterInitialize()
        {
            var connectTcs = new TaskCompletionSource<IJSVoidResult>();

            var mock = new Mock<IJSRuntime>();
            var handler = new MudPopoverHandler(_ => { }, mock.Object, () => { });

            mock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.connect", It.IsAny<CancellationToken>(), It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == handler.Id)))
                .Returns(new ValueTask<IJSVoidResult>(connectTcs.Task))
                .Verifiable();

            mock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.disconnect", It.IsAny<CancellationToken>(), It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == handler.Id)))
                .ReturnsAsync(Mock.Of<IJSVoidResult>())
                .Verifiable();

            var task1 = handler.Initialize();
            var task2 = handler.Detach();

            connectTcs.SetResult(Mock.Of<IJSVoidResult>());

            await Task.WhenAll(task1, task2);

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverHandler_DetachCalledBeforeInitialize_NoInteropShouldOccur()
        {
            var mock = new Mock<IJSRuntime>();

            var handler = new MudPopoverHandler(_ => { }, mock.Object, () => { });

            await handler.Detach();
            handler.IsConnected.Should().BeFalse();

            await handler.Initialize();
            handler.IsConnected.Should().BeFalse();

            mock.VerifyNoOtherCalls();
        }

        [Test]
        [Obsolete]
        public void MudPopoverService_Constructor_NoJsInterop()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new MudPopoverService(null));
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverService_Constructor_NullOption()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initialize"
               , It.IsAny<CancellationToken>(),
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
        [Obsolete]
        public async Task MudPopoverService_Initialize_Catch_JSDisconnectedException()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initialize",
               It.IsAny<CancellationToken>(),
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0))).ThrowsAsync(new JSDisconnectedException("JSDisconnectedException")).Verifiable();
            {
                var service = new MudPopoverService(mock.Object);
                await service.InitializeIfNeeded();
            }
            mock.Verify();
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverService_Initialize_Catch_TaskCanceledException()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initialize",
               It.IsAny<CancellationToken>(),
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0))).ThrowsAsync(new TaskCanceledException()).Verifiable();
            {
                var service = new MudPopoverService(mock.Object);
                await service.InitializeIfNeeded();
            }
            mock.Verify();
        }

        [Test]
        [Obsolete]
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
               "mudPopover.initialize",
               It.IsAny<CancellationToken>(),
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "my-custom-class" && (int)x[1] == 12))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            var service = new MudPopoverService(mock.Object, optionMock.Object);

            await service.InitializeIfNeeded();

            mock.Verify();
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverService_CallInitializeOnlyOnce()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initialize",
               It.IsAny<CancellationToken>(),
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0))).ReturnsAsync(Mock.Of<IJSVoidResult>(), TimeSpan.FromMilliseconds(300)).Verifiable();

            var tasks = new Task[5];
            var service = new MudPopoverService(mock.Object);

            for (var i = 0; i < 5; i++)
            {
                tasks[i] = Task.Run(async () => await service.InitializeIfNeeded());
            }

            Task.WaitAll(tasks);

            mock.Verify(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initialize",
               It.IsAny<CancellationToken>(),
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0)), Times.Once());
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverService_OnlyDisposeIfConnected()
        {
            var service = new MudPopoverService(Mock.Of<IJSRuntime>(MockBehavior.Strict));
            await service.DisposeAsync();
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverService_DisposeAsync()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initialize",
               It.IsAny<CancellationToken>(),
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
            "mudPopover.dispose",
            It.IsAny<CancellationToken>(),
            It.Is<object[]>(x => x.Length == 0))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            var service = new MudPopoverService(mock.Object);
            await service.InitializeIfNeeded();

            await service.DisposeAsync();

            mock.Verify();
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverService_DisposeAsync_WithTaskCancelException()
        {
            var mock = new Mock<IJSRuntime>();

            mock.Setup(x =>
           x.InvokeAsync<IJSVoidResult>(
               "mudPopover.initialize",
               It.IsAny<CancellationToken>(),
               It.Is<object[]>(x => x.Length == 2 && (string)x[0] == "mudblazor-main-content" && (int)x[1] == 0))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            mock.Setup(x =>
            x.InvokeAsync<IJSVoidResult>(
            "mudPopover.dispose",
            It.IsAny<CancellationToken>(),
            It.Is<object[]>(x => x.Length == 0))).ThrowsAsync(new TaskCanceledException()).Verifiable();

            var service = new MudPopoverService(mock.Object);
            await service.InitializeIfNeeded();

            //dispose shouldn't throw an exception in task a TaskCanceledException happend
            await service.DisposeAsync();

            mock.Verify();
        }

        [Test(Description = "Remove in v7")]
        [Obsolete]
        public void MudPopoverService_RegisterAndUseHandler()
        {
            var service = new MudPopoverService(Mock.Of<IJSRuntime>(MockBehavior.Strict));

            var fragmentChangedCounter = 0;

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
            // counter doesn't change because UpdateFragment now only re-renders the updated fragment, without raising the FragmentsChanged event
            fragmentChangedCounter.Should().Be(1);
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverService_RegisterAndUseHandlerAsync()
        {
            var service = new MudPopoverService(Mock.Of<IJSRuntime>(MockBehavior.Strict));

            var fragmentChangedCounter = 0;

            service.FragmentsChanged += (_, _) =>
            {
                fragmentChangedCounter++;
            };

            RenderFragment fragment = _ => { };

            RenderFragment changedFragment = _ => { };

            var handler = service.Register(fragment);

            handler.Should().NotBeNull();
            fragmentChangedCounter.Should().Be(1);

            var comp = Context.RenderComponent<MudBadge>(p =>
            {
                p.Add(x => x.UserAttributes, new Dictionary<string, object> { { "myprop1", "myValue1" } });
                p.Add(x => x.Tag, "my tag");

            });
            await handler.UpdateFragmentAsync(changedFragment, comp.Instance, "my-class", "my-style", true);
            // counter doesn't change because UpdateFragment now only re-renders the updated fragment, without raising the FragmentsChanged event
            fragmentChangedCounter.Should().Be(1);
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverService_Unregister_NullFragment()
        {
            var service = new MudPopoverService(Mock.Of<IJSRuntime>(MockBehavior.Strict));

            var result = await service.Unregister(null);
            result.Should().BeFalse();
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverService_Unregister_HandlerNotFound()
        {
            var service = new MudPopoverService(Mock.Of<IJSRuntime>(MockBehavior.Strict));

            var handler = new MudPopoverHandler((tree) => { }, Mock.Of<IJSRuntime>(MockBehavior.Strict), () => { });
            var result = await service.Unregister(handler);

            result.Should().BeFalse();
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverService_Unregister_NotConnected()
        {
            var service = new MudPopoverService(Mock.Of<IJSRuntime>(MockBehavior.Strict));

            RenderFragment fragment = (builder) => { };

            var handler = service.Register(fragment);
            var result = await service.Unregister(handler);

            result.Should().BeTrue();
        }

        [Test]
        [Obsolete]
        public async Task MudPopoverService_Unregister()
        {
            var handlerId = Guid.NewGuid();
            var mock = new Mock<IJSRuntime>();
            mock.Setup(jsRuntime =>
                    jsRuntime.InvokeAsync<IJSVoidResult>(
                        "mudPopover.connect",
                        It.IsAny<CancellationToken>(),
                        It.Is<object[]>(x => x.Length == 1 && (Guid)x[0] == handlerId)))
                .ReturnsAsync(Mock.Of<IJSVoidResult>)
                .Verifiable();

            mock.Setup(jsRuntime =>
                    jsRuntime.InvokeAsync<IJSVoidResult>(
                        "mudPopover.disconnect",
                        It.IsAny<CancellationToken>(),
                        It.Is<object[]>(x => x.Length == 1 && (Guid)x[0] == handlerId)))
                .ReturnsAsync(Mock.Of<IJSVoidResult>)
                .Verifiable();


            var service = new MudPopoverService(mock.Object);

            await service.InitializeIfNeeded();

            RenderFragment fragment = (builder) => { };

            var handler = service.Register(fragment);
            handlerId = handler.Id;

            await handler.Initialize();

            var fragmentChangedCounter = 0;

            service.FragmentsChanged += (_, _) =>
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
            popover.DropShadow.Should().BeTrue();
            popover.Elevation.Should().Be(8);
            popover.Square.Should().BeFalse();
            popover.Open.Should().BeFalse();
            popover.Fixed.Should().BeFalse();
            popover.AnchorOrigin.Should().Be(Origin.TopLeft);
            popover.TransformOrigin.Should().Be(Origin.TopLeft);
            popover.RelativeWidth.Should().BeFalse();
            popover.OverflowBehavior.Should().Be(OverflowBehavior.FlipOnOpen);
            popover.Duration.Should().Be(251);
        }

        [Test]
        public async Task MudPopover_OpenAndClose()
        {
            var comp = Context.RenderComponent<PopoverTest>();

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

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.GetAttribute("style").Split(';', StringSplitOptions.RemoveEmptyEntries).Should().Contain(new[] { "max-height:100px", "my-custom-style:3px" });
        }

        [Test]
        public void MudPopover_Property_TransitionDuration()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(x => x.Duration, 100));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.GetAttribute("style").Split(';', StringSplitOptions.RemoveEmptyEntries).Should().Contain(new[] { "transition-duration:100ms" });
        }

        [Test]
        public void MudPopover_Property_Fixed()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.Fixed, true));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", "mud-popover-fixed", "my-custom-class" });
        }

        [Test]
        public void MudPopover_Property_RelativeWidth()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.RelativeWidth, true));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", "mud-popover-relative-width", "my-custom-class" });
        }

        [Test]
        public void MudPopover_Property_Paper()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.Paper, true));

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

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", $"mud-popover-anchor-{expectedClass}", "my-custom-class" });
        }

        [Test]
        [TestCase(OverflowBehavior.FlipNever, "flip-never")]
        [TestCase(OverflowBehavior.FlipOnOpen, "flip-onopen")]
        [TestCase(OverflowBehavior.FlipAlways, "flip-always")]
        public void MudPopover_Property_OverflowBehavior(OverflowBehavior overflowBehavior, string expectedClass)
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.OverflowBehavior, overflowBehavior));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().Contain(new[] { "mud-popover-open", $"mud-popover-overflow-{expectedClass}", "my-custom-class" });
        }

        [Test]
        public void MudPopover_Property_DropShadow_False_NoElevation()
        {
            var comp = Context.RenderComponent<PopoverPropertyTest>(p => p.Add(
                x => x.DropShadow, false));

            var popoverElement = comp.Find(".test-popover-content").ParentElement;

            popoverElement.ClassList.Should().NotContainMatch("mud-elevation-*");
        }

        [Test]
        public async Task MudPopover_WithDynamicContent()
        {
            var comp = Context.RenderComponent<PopoverComplexContent>();

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
            provider.Enabled.Should().BeTrue();
        }

        [Test]
        public void MudPopoverProvider_RenderElementsBasedOnEnableState()
        {
            var comp = Context.RenderComponent<PopoverProviderTest>(p => p.Add(x => x.ProviderEnabled, true));
            comp.Find("#my-content").TextContent.Should().Be("Popover content");

            for (var i = 0; i < 3; i++)
            {
                comp.SetParametersAndRender(p => p.Add(x => x.ProviderEnabled, false));
                Assert.Throws<ElementNotFoundException>(() => comp.Find("#my-content"));

                comp.SetParametersAndRender(p => p.Add(x => x.ProviderEnabled, true));
                comp.Find("#my-content").TextContent.Should().Be("Popover content");
            }
        }

        [Test]
        public void MudPopoverProvider_NoRenderWhenEnabledIsFalse()
        {
            var comp = Context.RenderComponent<PopoverProviderTest>(p => p.Add(x => x.ProviderEnabled, false));
            Assert.Throws<ElementNotFoundException>(() => comp.Find("#my-content"));
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task MudPopoverProvider_ThrowOnDuplicate(bool ThrowOnDuplicateProvider)
        {
            var options = new PopoverOptions
            {
                ThrowOnDuplicateProvider = ThrowOnDuplicateProvider
            };

            Context.Services.Configure<PopoverOptions>(x => x.ThrowOnDuplicateProvider = ThrowOnDuplicateProvider);
            Context.JSInterop.Setup<int>("mudpopoverHelper.countProviders").SetResult(ThrowOnDuplicateProvider ? 2 : 1);

            if (ThrowOnDuplicateProvider)
            {
                var ex = Assert.Throws<InvalidOperationException>(() => Context.RenderComponent<PopoverDuplicationTest>());
                ex.Message.Should().StartWith("Duplicate MudPopoverProvider detected");
            }
            else
            {
                var comp = Context.RenderComponent<PopoverDuplicationTest>();
                await comp.Instance.Open();
                await comp.Instance.Close();
            }
        }
    }
}
