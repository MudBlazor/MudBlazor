// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Moq;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.PointerEvents;

[TestFixture]
public class PointerEventsNoneObserverTests
{
    [Test]
    public void Constructor_WhenCalled_DoesNotInvokePointerObservers()
    {
        // Arrange
        var pointerDownMock = new Mock<IPointerDownObserver>();
        pointerDownMock
            .Setup(x => x.NotifyOnPointerDownAsync(It.IsAny<EventArgs>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var pointerUpMock = new Mock<IPointerUpObserver>();
        pointerUpMock
            .Setup(x => x.NotifyOnPointerUpAsync(It.IsAny<EventArgs>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var observer = new PointerEventsNoneObserver("observer1", pointerDownMock.Object, pointerUpMock.Object);

        // Assert
        pointerDownMock.Verify(x => x.NotifyOnPointerDownAsync(It.IsAny<EventArgs>()), Times.Never);
        pointerUpMock.Verify(x => x.NotifyOnPointerUpAsync(It.IsAny<EventArgs>()), Times.Never);
    }

    [Test]
    public async Task NotifyOnPointerDownAsync_WhenCalled_InvokesPointerDownObserver()
    {
        // Arrange
        var pointerDownMock = new Mock<IPointerDownObserver>();
        pointerDownMock
            .Setup(x => x.NotifyOnPointerDownAsync(It.IsAny<EventArgs>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        IPointerEventsNoneObserver observer = new PointerEventsNoneObserver("observer1", pointerDownMock.Object, null);

        // Act
        await observer.NotifyOnPointerDownAsync(EventArgs.Empty);

        // Assert
        pointerDownMock.Verify(x => x.NotifyOnPointerDownAsync(It.IsAny<EventArgs>()), Times.Once);
    }

    [Test]
    public async Task NotifyOnPointerDownAsync_WhenCalled_DoesNotInvokePointerUpObserver()
    {
        // Arrange
        var pointerUpMock = new Mock<IPointerUpObserver>();
        pointerUpMock
            .Setup(x => x.NotifyOnPointerUpAsync(It.IsAny<EventArgs>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        IPointerEventsNoneObserver observer = new PointerEventsNoneObserver("observer1", null, pointerUpMock.Object);

        // Act
        await observer.NotifyOnPointerDownAsync(EventArgs.Empty);

        // Assert
        pointerUpMock.Verify(x => x.NotifyOnPointerUpAsync(It.IsAny<EventArgs>()), Times.Never);
    }

    [Test]
    public async Task NotifyOnPointerUpAsync_WhenCalled_InvokesPointerUpObserver()
    {
        // Arrange
        var pointerUpMock = new Mock<IPointerUpObserver>();
        pointerUpMock
            .Setup(x => x.NotifyOnPointerUpAsync(It.IsAny<EventArgs>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        IPointerEventsNoneObserver observer = new PointerEventsNoneObserver("observer1", null, pointerUpMock.Object);

        // Act
        await observer.NotifyOnPointerUpAsync(EventArgs.Empty);

        // Assert
        pointerUpMock.Verify(x => x.NotifyOnPointerUpAsync(It.IsAny<EventArgs>()), Times.Once);
    }

    [Test]
    public async Task NotifyOnPointerUpAsync_WhenCalled_DoesNotInvokePointerDownObserver()
    {
        // Arrange
        var pointerDownMock = new Mock<IPointerDownObserver>();
        pointerDownMock
            .Setup(x => x.NotifyOnPointerDownAsync(It.IsAny<EventArgs>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        IPointerEventsNoneObserver observer = new PointerEventsNoneObserver("observer1", pointerDownMock.Object, null);

        // Act
        await observer.NotifyOnPointerUpAsync(EventArgs.Empty);

        // Assert
        pointerDownMock.Verify(x => x.NotifyOnPointerDownAsync(It.IsAny<EventArgs>()), Times.Never);
    }

    [Test]
    public async Task NotifyOnPointerDownAndUpAsync_WhenCalled_InvokesBothObservers()
    {
        // Arrange
        var pointerDownMock = new Mock<IPointerDownObserver>();
        pointerDownMock
            .Setup(x => x.NotifyOnPointerDownAsync(It.IsAny<EventArgs>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var pointerUpMock = new Mock<IPointerUpObserver>();
        pointerUpMock
            .Setup(x => x.NotifyOnPointerUpAsync(It.IsAny<EventArgs>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        IPointerEventsNoneObserver observer = new PointerEventsNoneObserver("observer1", pointerDownMock.Object, pointerUpMock.Object);

        // Act
        await observer.NotifyOnPointerDownAsync(EventArgs.Empty);
        await observer.NotifyOnPointerUpAsync(EventArgs.Empty);

        // Assert
        pointerDownMock.Verify(x => x.NotifyOnPointerDownAsync(It.IsAny<EventArgs>()), Times.Once);
        pointerUpMock.Verify(x => x.NotifyOnPointerUpAsync(It.IsAny<EventArgs>()), Times.Once);
    }
}
