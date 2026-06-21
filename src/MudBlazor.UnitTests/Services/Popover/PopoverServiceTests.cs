// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
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
    private static PopoverService CreateService(IJSRuntime jsRuntime, PopoverOptions? options = null, FakeTimeProvider? timeProvider = null)
    {
        timeProvider ??= new FakeTimeProvider();

        return options is null
            ? new PopoverService(NullLogger<PopoverService>.Instance, jsRuntime, timeProvider)
            : new PopoverService(NullLogger<PopoverService>.Instance, jsRuntime, timeProvider, new OptionsWrapper<PopoverOptions>(options));
    }

    private static PopoverServiceMock CreateMockService(IJSRuntime jsRuntime, PopoverServiceMock.IPopoverTimerMock popoverTimer, FakeTimeProvider? timeProvider = null)
    {
        timeProvider ??= new FakeTimeProvider();

        return new PopoverServiceMock(NullLogger<PopoverService>.Instance, jsRuntime, timeProvider, popoverTimer);
    }

    [Test]
    public void ActivePopovers_ShouldBeEmpty_AtInitialization()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = CreateService(jsRuntimeMock);

        // Assert
        service.ActivePopovers.Should().BeEmpty();
    }

    [Test]
    public void IsInitialized_ShouldBeFalse_AtInitialization()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = CreateService(jsRuntimeMock);

        // Assert
        service.IsInitialized.Should().BeFalse();
    }

    [Test]
    public async Task IsInitialized_ShouldNotConnectAutomaticallyAfterCreatePopover()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var options = new PopoverOptions { CheckForPopoverProvider = false };
        var service = CreateService(jsRuntimeMock, options);

        // Assert
        service.IsInitialized.Should().BeFalse();

        // Act
        await service.CreatePopoverAsync(popover);

        // Assert
        service.IsInitialized.Should().BeFalse();
    }

    [Test]
    public async Task IsInitialized_ShouldConnectAutomaticallyAfterDestroyPopover()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = CreateService(jsRuntimeMock);

        // Assert
        service.IsInitialized.Should().BeFalse();

        // Act
        await service.DestroyPopoverAsync(popover);

        // Assert
        service.IsInitialized.Should().BeTrue();
    }

    [Test]
    public async Task IsInitialized_ShouldConnectAutomaticallyAfterUpdatePopover()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = CreateService(jsRuntimeMock);

        // Assert
        service.IsInitialized.Should().BeFalse();

        // Act
        await service.UpdatePopoverAsync(popover);

        // Assert
        service.IsInitialized.Should().BeTrue();
    }

    [Test]
    public async Task IsInitialized_ShouldNotConnectAutomaticallyAfterCountProviders()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = CreateService(jsRuntimeMock);

        // Assert
        service.IsInitialized.Should().BeFalse();

        // Act
        await service.GetProviderCountAsync();

        // Assert
        service.IsInitialized.Should().BeFalse();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task CreatePopoverAsync_CheckForPopoverProvider(bool checkForPopoverProvider)
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var options = new PopoverOptions { CheckForPopoverProvider = checkForPopoverProvider };
        var service = CreateService(jsRuntimeMock, options);

        // Act
        var create = () => service.CreatePopoverAsync(popover);

        // Assert
        if (checkForPopoverProvider)
        {
            await create.Should().ThrowAsync<InvalidOperationException>();
        }
        else
        {
            await create.Should().NotThrowAsync<InvalidOperationException>();
        }
    }

    [Test]
    public void Unsubscribe_ShouldThrowWheNullObserver()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = CreateService(jsRuntimeMock);

        // Act
        var unsubscribe = () => service.Unsubscribe(null!);

        // Assert
        unsubscribe.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Subscribe_ShouldThrowWheNullObserver()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = CreateService(jsRuntimeMock);

        // Act
        var subscribe = () => service.Subscribe(null!);

        // Assert
        subscribe.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public async Task CreatePopoverAsync_ShouldThrowWheNullPopover()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = CreateService(jsRuntimeMock);

        // Act
        var createPopover = () => service.CreatePopoverAsync(null!);

        // Assert
        await createPopover.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task CreatePopoverAsync_ShouldAddStateAndNotifyObservers()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = CreateService(jsRuntimeMock);
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
    public async Task CreatePopoverAsync_ShouldUseInjectedTimeProviderForActivationDate()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var timeProvider = new FakeTimeProvider();
        timeProvider.SetUtcNow(new DateTime(2024, 4, 5, 6, 7, 8, DateTimeKind.Utc));
        var popover = new PopoverMock { Open = true };
        var options = new PopoverOptions { CheckForPopoverProvider = false };
        var service = CreateService(jsRuntimeMock, options, timeProvider);

        // Act
        await service.CreatePopoverAsync(popover);

        // Assert
        service.ActivePopovers.Single().ActivationDate.Should().Be(timeProvider.GetLocalNow().DateTime);
    }

    [Test]
    public async Task CreatePopoverAsync_ShouldNotAddDuplicateWhenCalledTwiceWithSameId()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var options = new PopoverOptions { CheckForPopoverProvider = false };
        var service = CreateService(jsRuntimeMock, options);

        // Act
        await service.CreatePopoverAsync(popover);
        await service.CreatePopoverAsync(popover);

        // Assert
        service.ActivePopovers.Should().ContainSingle().Which.Id.Should().Be(popover.Id);
    }

    [Test]
    public async Task UpdatePopoverAsync_ShouldThrowWheNullPopover()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = CreateService(jsRuntimeMock);

        // Act
        var updatePopover = () => service.UpdatePopoverAsync(null!);

        // Assert
        await updatePopover.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task UpdatePopoverAsync_ShouldNotUpdateWhenNotCreated()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = CreateService(jsRuntimeMock);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        // Act
        var result = await service.UpdatePopoverAsync(popover);

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
        var service = CreateService(jsRuntimeMock);
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
        var service = CreateService(jsRuntimeMock);
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
    public async Task UpdatePopoverAsync_ShouldNotUpdateStateWhenDetached()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = CreateService(jsRuntimeMock);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        // Act
        await service.CreatePopoverAsync(popover);
        //Get reference before destroyed
        var updatedState = service.ActivePopovers.FirstOrDefault(x => x.Id == popover.Id);
        if (updatedState is MudPopoverHolder internalHolder)
        {
            internalHolder.IsDetached = true;
        }

        var isUpdated = await service.UpdatePopoverAsync(popover);

        // Assert
        isUpdated.Should().BeFalse();
    }

    [Test]
    public async Task DestroyPopoverAsync_ShouldThrowWheNullPopover()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = CreateService(jsRuntimeMock);

        // Act
        var destroyPopover = () => service.DestroyPopoverAsync(null!);

        // Assert
        await destroyPopover.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task DestroyPopoverAsync_ShouldNotDestroyWhenNotCreated()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = CreateService(jsRuntimeMock);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        // Act
        var result = await service.DestroyPopoverAsync(popover);

        // Assert
        result.Should().BeFalse();
        observer.PopoverNotifications.Should().BeEmpty();
    }

    [Test]
    public async Task DestroyPopoverAsync_ShouldRemoveStateAndNotifyObservers()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = CreateService(jsRuntimeMock);
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
        var options = new PopoverOptions { CheckForPopoverProvider = false };
        var service = CreateService(jsRuntimeMock, options);

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
        var service = CreateService(jsRuntimeMock);
        var observerMock = new Mock<IPopoverObserver>();
        service.Subscribe(observerMock.Object);

        observerMock
            .Setup(h => h.PopoverCollectionUpdatedNotificationAsync(
                It.IsAny<PopoverHolderContainer>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback<PopoverHolderContainer, CancellationToken>((container, _) => containerNotificationList.Add(container));

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
        var options = new PopoverOptions { CheckForPopoverProvider = false };
        var service = CreateService(jsRuntimeMock, options);

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
        var options = new PopoverOptions { CheckForPopoverProvider = false };
        var service = CreateService(jsRuntimeMock, options);

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
    [CancelAfter(5000)]
    public async Task CreatePopoverAsync_UpdatePopoverAsync_DestroyPopoverAsync_ShouldInvokeJS()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var popover = new PopoverMock();
        var popoverTimerMock = new Mock<PopoverServiceMock.IPopoverTimerMock>();
        var batchCompletion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var timeProvider = new FakeTimeProvider();
        var service = CreateMockService(jsRuntimeMock.Object, popoverTimerMock.Object, timeProvider);
        var observer = new PopoverObserverMock();
        service.Subscribe(observer);

        popoverTimerMock
            .Setup(h => h.OnBatchTimerElapsedAfterAsync(
                It.IsAny<IReadOnlyCollection<MudPopoverHolder>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => batchCompletion.TrySetResult());

        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.initialize", It.IsAny<CancellationToken>(),
                It.Is<object[]>(y => y.Length == 3)))
            .ReturnsAsync(Mock.Of<IJSVoidResult>())
            .Verifiable();

        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.connect", It.IsAny<CancellationToken>(),
                It.Is<object[]>(y => y.Length == 1 && (Guid)y[0] == popover.Id)))
            .ReturnsAsync(Mock.Of<IJSVoidResult>())
            .Verifiable();

        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudPopover.disconnect", It.IsAny<CancellationToken>(),
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
        timeProvider.Advance(service.PopoverOptions.QueueDelay);
        await batchCompletion.Task;

        // Assert
        jsRuntimeMock.Verify();
        jsRuntimeMock.VerifyNoOtherCalls();
    }

    [Test]
    [CancelAfter(5000)]
    public async Task DisposeAsync_ShouldCancelDetachRange()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var popoverTimerMock = new Mock<PopoverServiceMock.IPopoverTimerMock>();
        var beforeBatchCompletion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var afterBatchCompletion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var timeProvider = new FakeTimeProvider();
        var service = CreateMockService(jsRuntimeMock.Object, popoverTimerMock.Object, timeProvider);
        var observer = new PopoverObserverMock();
        var popovers = new[] { new PopoverMock(), new PopoverMock(), new PopoverMock(), new PopoverMock() };
        service.Subscribe(observer);

        popoverTimerMock
            .Setup(h => h.OnBatchTimerElapsedBeforeAsync(
                It.IsAny<IReadOnlyCollection<MudPopoverHolder>>(),
                It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                // Call dispose immediately before the DetachRangeAsync about to fire.
                beforeBatchCompletion.TrySetResult();
                await service.DisposeAsync();
            });

        popoverTimerMock
            .Setup(h => h.OnBatchTimerElapsedAfterAsync(
                It.IsAny<IReadOnlyCollection<MudPopoverHolder>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => afterBatchCompletion.TrySetResult());

        // Act
        foreach (var popover in popovers)
        {
            await service.CreatePopoverAsync(popover);
        }

        foreach (var popover in popovers)
        {
            // Necessary to make them connect to check if "mudPopover.disconnect" was invoked otherwise will be skipped.
            await service.UpdatePopoverAsync(popover);
        }

        foreach (var popover in popovers)
        {
            await service.DestroyPopoverAsync(popover);
        }

        timeProvider.Advance(service.PopoverOptions.QueueDelay);
        await beforeBatchCompletion.Task;
        await afterBatchCompletion.Task;

        // Assert
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudPopover.disconnect", It.IsAny<CancellationToken>(), It.IsAny<object[]>()), Times.Never);
    }

    [Test]
    public async Task DisposeAsync_ShouldClearActivePopovers()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = CreateService(jsRuntimeMock);
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
        var service = CreateService(jsRuntimeMock);
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

    [Test]
    public async Task DisposeAsync_ShouldNotAcceptObservers()
    {
        // Arrange
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = CreateService(jsRuntimeMock);

        // Act
        await service.DisposeAsync();
        service.Subscribe(new PopoverObserverMock());

        // Assert
        service.ObserversCount.Should().Be(0);
    }
    [Test]
    public async Task DisposeAsync_ShouldNotCreateOrUpdateWhenDisposed()
    {
        // Arrange
        var popoverOperations = new List<PopoverHolderOperation>();
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var service = CreateService(jsRuntimeMock);
        var popover = new PopoverMock();
        var observerMock = new Mock<IPopoverObserver>();
        service.Subscribe(observerMock.Object);

        observerMock
            .Setup(h => h.PopoverCollectionUpdatedNotificationAsync(
                It.IsAny<PopoverHolderContainer>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback<PopoverHolderContainer, CancellationToken>((container, token) =>
            {
                popoverOperations.Add(container.Operation);
            });

        // Act
        await service.DisposeAsync();
        await service.CreatePopoverAsync(popover);
        await service.UpdatePopoverAsync(popover);

        // Assert
        popoverOperations.Should().BeEquivalentTo(new[] { PopoverHolderOperation.Remove });
    }

    [Test]
    public async Task DisposeAsync_PopoverCollectionUpdatedNotificationAsync_IsCancellationRequested()
    {
        //Arrange
        var isCancellationRequested = false;
        var jsRuntimeMock = Mock.Of<IJSRuntime>();
        var popover = new PopoverMock();
        var service = CreateService(jsRuntimeMock);
        var observerMock = new Mock<IPopoverObserver>();
        service.Subscribe(observerMock.Object);

        observerMock
            .Setup(h => h.PopoverCollectionUpdatedNotificationAsync(
                It.IsAny<PopoverHolderContainer>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback<PopoverHolderContainer, CancellationToken>((container, token) =>
            {
                isCancellationRequested = token.IsCancellationRequested;
            });

        // Act
        await service.CreatePopoverAsync(popover);

        // Assert
        isCancellationRequested.Should().BeFalse();

        // Act
        await service.DisposeAsync();

        // Assert
        isCancellationRequested.Should().BeTrue();
    }
}
