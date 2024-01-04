// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.Interop;
using MudBlazor.UnitTests.Services.Popover.Mocks;
using MudBlazor.Utilities.Background.Batch;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.Popover;

#nullable enable
[TestFixture]
public class PopoverServiceTests
{
    [Test]
    public void ActivePopovers_ShouldBeEmpty_AtInitialization()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Assert
        service.ActivePopovers.Should().BeEmpty();
    }

    [Test]
    public void IsInitialized_ShouldBeFalse_AtInitialization()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Assert
        service.IsInitialized.Should().BeFalse();
    }

    [Test]
    public async Task IsInitialized_ShouldNotConnectAutomaticallyAfterCreatePopoverAsync()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Assert
        service.IsInitialized.Should().BeFalse();

        // Act
        await service.CreatePopoverAsync(popover);

        // Assert
        service.IsInitialized.Should().BeFalse();
    }

    [Test]
    public async Task IsInitialized_ShouldConnectAutomaticallyAfterDestroyPopoverAsync()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Assert
        service.IsInitialized.Should().BeFalse();

        // Act
        await service.DestroyPopoverAsync(popover);

        // Assert
        service.IsInitialized.Should().BeTrue();
    }

    [Test]
    public async Task IsInitialized_ShouldConnectAutomaticallyAfterUpdatePopoverAsync()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Assert
        service.IsInitialized.Should().BeFalse();

        // Act
        await service.UpdatePopoverAsync(popover);

        // Assert
        service.IsInitialized.Should().BeTrue();
    }

    [Test]
    public async Task IsInitialized_ShouldConnectAutomaticallyAfterCountProvidersAsync()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Assert
        service.IsInitialized.Should().BeFalse();

        // Act
        await service.GetProviderCountAsync();

        // Assert
        service.IsInitialized.Should().BeTrue();
    }

    [Test]
    public async Task CreatePopoverAsync_ShouldAddStateAndNotifyObservers()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        // Act
        await service.CreatePopoverAsync(popover);

        // Assert
        var activePopovers = service.ActivePopovers.Select(x => x.Id).ToList();
        observer.PopoverNotifications.Count.Should().Be(1);
        observer.PopoverNotifications.Should().Contain(popover.Id);
        activePopovers.Should().Contain(popover.Id);
    }

    [Test]
    public async Task UpdatePopoverAsync_ShouldNotUpdateWhenNotCreated()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        // Act
        var result = await service.UpdatePopoverAsync(popover);

        // Assert
        result.Should().BeFalse();
        observer.PopoverNotifications.Should().BeEmpty();
    }

    [Test]
    public async Task UpdatePopoverAsync_ShouldNotDestroyWhenNotCreated()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        // Act
        var result = await service.DestroyPopoverAsync(popover);

        // Assert
        result.Should().BeFalse();
        observer.PopoverNotifications.Should().BeEmpty();
    }

    [Test]
    public async Task UpdatePopoverAsync_ShouldUpdateState()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        // Act
        await service.CreatePopoverAsync(popover);

        RenderFragment newRenderFragment = _ => { };
        popover.ChildContent = newRenderFragment;
        popover.Open = true;
        popover.PopoverClass = "popoverClass";
        popover.PopoverStyles = "popoverStyle";
        popover.Tag = "my-tag";
        popover.UserAttributes = new Dictionary<string, object?>
        {
            { "key1", "value1" },
            { "key2", false }
        };

        var updatedState = service.ActivePopovers.FirstOrDefault(x => x.Id == popover.Id);

        //Assert before update
        updatedState.Should().NotBeNull();
        updatedState!.ShowContent.Should().BeFalse();
        updatedState.Class.Should().BeEmpty();
        updatedState.Style.Should().BeEmpty();
        updatedState.Tag.Should().BeNull();
        updatedState.UserAttributes.Should().BeEmpty();
        updatedState.Fragment.Should().BeNull();

        //Act
        var isUpdated = await service.UpdatePopoverAsync(popover);

        // Assert after update
        isUpdated.Should().BeTrue();
        updatedState.ShowContent.Should().Be(popover.Open);
        updatedState.Class.Should().Be(popover.PopoverClass);
        updatedState.Style.Should().Be(popover.PopoverStyles);
        updatedState.Tag.Should().Be(popover.Tag);
        updatedState.UserAttributes.Should().BeSameAs(popover.UserAttributes);
        updatedState.Fragment.Should().Be(newRenderFragment);

        //Assert
        //two notifications from CreatePopoverAsync and UpdatePopoverAsync
        observer.PopoverNotifications.Count.Should().Be(2);
        observer.PopoverNotifications.Should().Contain(popover.Id);
    }

    [Test]
    public async Task UpdatePopoverAsync_ShouldNotUpdateStateWhenDestroyed()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        // Act
        await service.CreatePopoverAsync(popover);
        //Get reference before destroyed
        var updatedState = service.ActivePopovers.FirstOrDefault(x => x.Id == popover.Id);
        var isDestroyed = await service.DestroyPopoverAsync(popover);
        popover.Open = true;
        popover.PopoverClass = "popoverClass";
        popover.PopoverStyles = "popoverStyle";
        popover.Tag = "my-tag";
        popover.UserAttributes = new Dictionary<string, object?>
        {
            { "key1", "value1" },
            { "key2", false }
        };

        var isUpdated = await service.UpdatePopoverAsync(popover);

        // Assert
        updatedState.Should().NotBeNull();
        isDestroyed.Should().BeTrue();
        isUpdated.Should().BeFalse();
        updatedState!.ShowContent.Should().BeFalse();
        updatedState.Class.Should().BeEmpty();
        updatedState.Style.Should().BeEmpty();
        updatedState.Tag.Should().BeNull();
        updatedState.UserAttributes.Should().BeEmpty();
        //two notifications from CreatePopoverAsync and DestroyPopover, UpdatePopoverAsync shouldn't fire notification since destroyed
        observer.PopoverNotifications.Count.Should().Be(2);
        observer.PopoverNotifications.Should().Contain(popover.Id);
    }

    [Test]
    public async Task DestroyPopoverAsync_ShouldRemoveStateAndNotifyObservers()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        // Act
        await service.CreatePopoverAsync(popover);
        var isDestroyed = await service.DestroyPopoverAsync(popover);

        // Assert
        isDestroyed.Should().BeTrue();
        service.ActivePopovers.Should().BeEmpty();
        //two notifications from CreatePopoverAsync and DestroyPopover
        observer.PopoverNotifications.Count.Should().Be(2);
        observer.PopoverNotifications.Should().Contain(popover.Id);
    }

    [Test]
    public async Task DestroyPopoverAsync_ShouldQueueForDisconnect()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popoverOne = new PopoverMock();
        var popoverTwo = new PopoverMock();
        var popoverThree = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Act
        await service.CreatePopoverAsync(popoverOne);
        await service.CreatePopoverAsync(popoverTwo);
        await service.CreatePopoverAsync(popoverThree);
        var isDestroyedOne = await service.DestroyPopoverAsync(popoverOne);
        var isDestroyedTwo = await service.DestroyPopoverAsync(popoverTwo);
        var isDestroyedThree = await service.DestroyPopoverAsync(popoverThree);

        // Assert
        service.QueueCount.Should().Be(3);
        isDestroyedOne.Should().BeTrue();
        isDestroyedTwo.Should().BeTrue();
        isDestroyedThree.Should().BeTrue();
    }

    [Test]
    public async Task CreatePopoverAsync_UpdatePopoverAsync_DestroyPopoverAsync_ShouldNotifyContainerWithCorrespondingOperation()
    {
        //Arrange
        var containerNotificationList = new List<PopoverHolderContainer>();
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);
        var observerMock = new Mock<IPopoverObserver>();
        service.Subscribe(observerMock.Object);

        observerMock
            .Setup(h => h.PopoverCollectionUpdatedNotificationAsync(
                It.IsAny<PopoverHolderContainer>()))
            .Returns(Task.CompletedTask)
            .Callback<PopoverHolderContainer>(containerNotificationList.Add);

        // Act
        await service.CreatePopoverAsync(popover);
        await service.UpdatePopoverAsync(popover);
        await service.DestroyPopoverAsync(popover);

        // Assert
        var firstNotification = containerNotificationList.ElementAt(0);
        var secondNotification = containerNotificationList.ElementAt(1);
        var thirdNotification = containerNotificationList.ElementAt(2);
        containerNotificationList.Count.Should().Be(3);
        firstNotification.Operation.Should().Be(PopoverHolderOperation.Create);
        secondNotification.Operation.Should().Be(PopoverHolderOperation.Update);
        thirdNotification.Operation.Should().Be(PopoverHolderOperation.Remove);
    }

    [Test]
    public async Task MudPopoverState_ShouldIsConnected()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Act
        await service.CreatePopoverAsync(popover);
        var updatedState = service.ActivePopovers.FirstOrDefault(x => x.Id == popover.Id);

        //Assert
        updatedState.Should().NotBeNull();
        updatedState!.IsConnected.Should().BeFalse();
        updatedState.IsDetached.Should().BeFalse();

        // Act
        var isUpdated = await service.UpdatePopoverAsync(popover);

        //Assert
        isUpdated.Should().BeTrue();
        updatedState.IsConnected.Should().BeTrue();
        updatedState.IsDetached.Should().BeFalse();
    }

    [Test]
    public async Task MudPopoverState_ShouldIsDetached()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Act
        await service.CreatePopoverAsync(popover);
        var updatedState = service.ActivePopovers.FirstOrDefault(x => x.Id == popover.Id);

        //Assert
        updatedState.Should().NotBeNull();
        updatedState!.IsConnected.Should().BeFalse();
        updatedState.IsDetached.Should().BeFalse();

        // Act
        var isDestroyed = await service.DestroyPopoverAsync(popover);

        //Assert
        isDestroyed.Should().BeTrue();
        updatedState.IsConnected.Should().BeFalse();
        updatedState.IsDetached.Should().BeTrue();
    }

    [Test]
    public async Task CreatePopoverAsync_UpdatePopoverAsync_DestroyPopoverAsync_ShouldInvokeJS()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var popover = new PopoverMock();
        var popoverTimerMock = new Mock<PopoverServiceMock.IPopoverTimerMock>();
        var signalEvent = new ManualResetEventSlim(false);
        var service = new PopoverServiceMock(NullLogger<PopoverService>.Instance, jsRuntimeMock.Object, popoverTimerMock.Object);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        popoverTimerMock
            .Setup(h => h.OnBatchTimerElapsedAfterAsync(
                It.IsAny<IReadOnlyCollection<MudPopoverHolder>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(signalEvent.Set);

        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.initialize",
                It.Is<object[]>(y => y.Length == 2)))
            .ReturnsAsync(Mock.Of<IJSVoidResult>())
            .Verifiable();

        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.connect",
                It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == popover.Id)))
            .ReturnsAsync(Mock.Of<IJSVoidResult>())
            .Verifiable();

        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.disconnect",
                It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == popover.Id)))
            .Returns(new ValueTask<IJSVoidResult>())
            .Verifiable();

        // Act
        await service.CreatePopoverAsync(popover);
        popover.ChildContent = _ => { };
        popover.PopoverClass = "my-new-extra-class";
        popover.PopoverStyles = "my-new-extra-style:2px";
        popover.Open = true;
        await service.UpdatePopoverAsync(popover);
        await service.DestroyPopoverAsync(popover);
        var signalEventWaitTime = service.PopoverOptions.QueueDelay.Add(TimeSpan.FromMinutes(2));
        signalEvent.Wait(signalEventWaitTime);

        // Assert
        jsRuntimeMock.Verify();
        jsRuntimeMock.VerifyNoOtherCalls();
    }

    [Test]
    [Ignore(reason:
        $"Not used anymore and replace by {nameof(DisposeAsync_ShouldClearActivePopovers)}," +
        $"because the {nameof(PopoverService.DisposeAsync)} doesn't trigger a guaranteed {nameof(IBatchTimerHandler<MudPopoverHolder>.OnBatchTimerElapsedAsync)} to disconnect popover," +
        $"since the {nameof(PopoverJsInterop.Dispose)} does it.")]
    public async Task DisposeAsync_ShouldClearActivePopoversAndFireOnBatchTimerElapsedAsync()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popoverTimerMock = new Mock<PopoverServiceMock.IPopoverTimerMock>();
        var signalEvent = new ManualResetEventSlim(false);
        var service = new PopoverServiceMock(NullLogger<PopoverService>.Instance, jsRuntimeMock, popoverTimerMock.Object);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        popoverTimerMock
            .Setup(h => h.OnBatchTimerElapsedAfterAsync(
                It.IsAny<IReadOnlyCollection<MudPopoverHolder>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(signalEvent.Set);

        await service.CreatePopoverAsync(new PopoverMock());
        await service.CreatePopoverAsync(new PopoverMock());

        // Act
        await service.DisposeAsync();
        // Wait for the event to be signaled, consider test failed if we didn't receive signal in period + 2 minutes
        var signalEventWaitTime = service.PopoverOptions.QueueDelay.Add(TimeSpan.FromMinutes(2));
        var eventSignaled = signalEvent.Wait(signalEventWaitTime);

        // Assert
        eventSignaled.Should().BeTrue();
        service.ActivePopovers.Should().BeEmpty();
        popoverTimerMock.Verify(
            h => h.OnBatchTimerElapsedAfterAsync(
                It.Is<IReadOnlyCollection<MudPopoverHolder>>(items => items.Count == 2),
                It.IsAny<CancellationToken>()),
            Times.AtLeastOnce,
            "The periodic handler method was not called.");
    }

    [Test]
    public async Task DisposeAsync_ShouldClearActivePopovers()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        await service.CreatePopoverAsync(new PopoverMock());
        await service.CreatePopoverAsync(new PopoverMock());

        // Act
        await service.DisposeAsync();

        // Assert
        service.QueueCount.Should().Be(0);
        service.ActivePopovers.Should().BeEmpty();
    }

    [Test]
    public async Task DisposeAsync_ShouldClearAllObservers()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);
        var popover = new PopoverMock();
        service.Subscribe(new PopoverObserverMock());
        service.Subscribe(new PopoverObserverMock());
        service.Subscribe(new PopoverObserverMock());
        service.Subscribe(new PopoverObserverMock());
        service.Subscribe(new PopoverObserverMock());
        var beforeObserversCount = service.ObserversCount;

        // Act
        await service.CreatePopoverAsync(popover);
        await service.DisposeAsync();
        var afterObserversCount = service.ObserversCount;

        // Assert
        beforeObserversCount.Should().Be(5);
        afterObserversCount.Should().Be(0);
    }
}
