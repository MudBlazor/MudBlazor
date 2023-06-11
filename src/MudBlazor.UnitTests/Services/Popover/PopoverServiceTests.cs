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
using MudBlazor.Examples.Data.Models;
using MudBlazor.UnitTests.Services.Popover.Mocks;
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
        Assert.IsEmpty(service.ActivePopovers);
    }

    [Test]
    public void IsInitialized_ShouldBeFalse_AtInitialization()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Assert
        Assert.IsFalse(service.IsInitialized);
    }

    [Test]
    public async Task IsInitialized_ShouldConnectAutomaticallyAfterCreatePopoverAsync()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Assert
        Assert.IsFalse(service.IsInitialized);

        // Act
        await service.CreatePopoverAsync(popover);

        // Assert
        Assert.IsTrue(service.IsInitialized);
    }

    [Test]
    public async Task IsInitialized_ShouldConnectAutomaticallyAfterDestroyPopoverAsync()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Assert
        Assert.IsFalse(service.IsInitialized);

        // Act
        await service.DestroyPopoverAsync(popover);

        // Assert
        Assert.IsTrue(service.IsInitialized);
    }

    [Test]
    public async Task IsInitialized_ShouldConnectAutomaticallyAfterUpdatePopoverAsync()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Assert
        Assert.IsFalse(service.IsInitialized);

        // Act
        await service.UpdatePopoverAsync(popover);

        // Assert
        Assert.IsTrue(service.IsInitialized);
    }

    [Test]
    public async Task IsInitialized_ShouldConnectAutomaticallyAfterCountProvidersAsync()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = new PopoverService(NullLogger<PopoverService>.Instance, jsRuntimeMock);

        // Assert
        Assert.IsFalse(service.IsInitialized);

        // Act
        await service.GetProviderCountAsync();

        // Assert
        Assert.IsTrue(service.IsInitialized);
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
        Assert.AreEqual(1, observer.PopoverNotifications.Count);
        Assert.Contains(popover.Id, observer.PopoverNotifications);
        Assert.Contains(popover.Id, activePopovers);
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
        Assert.IsFalse(result);
        Assert.IsEmpty(observer.PopoverNotifications);
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
        Assert.IsFalse(result);
        Assert.IsEmpty(observer.PopoverNotifications);
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
        popover.UserAttributes = new Dictionary<string, object>
        {
            { "key1", "value1" },
            { "key2", false }
        };

        var updatedState = service.ActivePopovers.FirstOrDefault(x => x.Id == popover.Id);

        //Assert before update
        Assert.NotNull(updatedState);
        Assert.IsFalse(updatedState!.ShowContent);
        Assert.IsEmpty(updatedState.Class);
        Assert.IsEmpty(updatedState.Style);
        Assert.IsNull(updatedState.Tag);
        Assert.IsEmpty(updatedState.UserAttributes);
        Assert.IsNull(updatedState.Fragment);

        //Act
        var isUpdated = await service.UpdatePopoverAsync(popover);

        // Assert after update
        Assert.IsTrue(isUpdated);
        Assert.AreEqual(popover.Open, updatedState.ShowContent);
        Assert.AreEqual(popover.PopoverClass, updatedState.Class);
        Assert.AreEqual(popover.PopoverStyles, updatedState.Style);
        Assert.AreEqual(popover.Tag, updatedState.Tag);
        Assert.AreEqual(popover.UserAttributes, updatedState.UserAttributes);
        Assert.AreEqual(newRenderFragment, updatedState.Fragment);

        //Assert
        //two notifications from CreatePopoverAsync and UpdatePopoverAsync
        Assert.AreEqual(2, observer.PopoverNotifications.Count);
        Assert.Contains(popover.Id, observer.PopoverNotifications);
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
        popover.UserAttributes = new Dictionary<string, object>
        {
            { "key1", "value1" },
            { "key2", false }
        };

        var isUpdated = await service.UpdatePopoverAsync(popover);

        // Assert
        Assert.NotNull(updatedState);
        Assert.IsTrue(isDestroyed);
        Assert.IsFalse(isUpdated);
        Assert.IsFalse(updatedState!.ShowContent);
        Assert.IsEmpty(updatedState.Class);
        Assert.IsEmpty(updatedState.Style);
        Assert.IsNull(updatedState.Tag);
        Assert.IsEmpty(updatedState.UserAttributes);
        //two notifications from CreatePopoverAsync and DestroyPopover, UpdatePopoverAsync shouldn't fire notification since destroyed
        Assert.AreEqual(2, observer.PopoverNotifications.Count);
        Assert.Contains(popover.Id, observer.PopoverNotifications);
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
        Assert.True(isDestroyed);
        Assert.IsEmpty(service.ActivePopovers);
        //two notifications from CreatePopoverAsync and DestroyPopover
        Assert.AreEqual(2, observer.PopoverNotifications.Count);
        Assert.Contains(popover.Id, observer.PopoverNotifications);
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
        Assert.AreEqual(3, containerNotificationList.Count);
        Assert.AreEqual(PopoverHolderOperation.Create, firstNotification.Operation);
        Assert.AreEqual(PopoverHolderOperation.Update, secondNotification.Operation);
        Assert.AreEqual(PopoverHolderOperation.Remove, thirdNotification.Operation);
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
        Assert.NotNull(updatedState);
        Assert.IsFalse(updatedState!.IsConnected);
        Assert.IsFalse(updatedState.IsDetached);

        // Act
        var isUpdated = await service.UpdatePopoverAsync(popover);

        //Assert
        Assert.IsTrue(isUpdated);
        Assert.IsTrue(updatedState.IsConnected);
        Assert.IsFalse(updatedState.IsDetached);
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
        Assert.NotNull(updatedState);
        Assert.IsFalse(updatedState!.IsConnected);
        Assert.IsFalse(updatedState.IsDetached);

        // Act
        var isDestroyed = await service.DestroyPopoverAsync(popover);

        //Assert
        Assert.IsTrue(isDestroyed);
        Assert.IsFalse(updatedState.IsConnected);
        Assert.IsTrue(updatedState.IsDetached);
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
    public async Task DisposeAsync_ShouldClearActivePopovers()
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
        Assert.IsTrue(eventSignaled);
        Assert.IsEmpty(service.ActivePopovers);
        popoverTimerMock.Verify(
            h => h.OnBatchTimerElapsedAfterAsync(
                It.Is<IReadOnlyCollection<MudPopoverHolder>>(items => items.Count == 2),
                It.IsAny<CancellationToken>()),
            Times.AtLeastOnce,
            "The periodic handler method was not called.");
    }
}
