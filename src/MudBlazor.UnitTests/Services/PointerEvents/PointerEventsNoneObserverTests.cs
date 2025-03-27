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
    public async Task NotifyOnPointerDownAsync_WhenCalled_InvokesUnderlyingObserver()
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
            .Verifiable(); ;

        IPointerEventsNoneObserver observer = new PointerEventsNoneObserver("observer1", pointerDownMock.Object, pointerUpMock.Object);

        // Act
        await observer.NotifyOnPointerDownAsync(EventArgs.Empty);

        // Assert
        pointerDownMock.Verify(x => x.NotifyOnPointerDownAsync(It.IsAny<EventArgs>()), Times.Once);
        pointerUpMock.Verify(x => x.NotifyOnPointerUpAsync(It.IsAny<EventArgs>()), Times.Never);
    }

    [Test]
    public async Task NotifyOnPointerUpAsync_WhenCalled_InvokesUnderlyingObserver()
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
            .Verifiable(); ;
        IPointerEventsNoneObserver observer = new PointerEventsNoneObserver("observer1", pointerDownMock.Object, pointerUpMock.Object);

        // Act
        await observer.NotifyOnPointerUpAsync(EventArgs.Empty);

        // Assert
        pointerDownMock.Verify(x => x.NotifyOnPointerDownAsync(It.IsAny<EventArgs>()), Times.Never);
        pointerUpMock.Verify(x => x.NotifyOnPointerUpAsync(It.IsAny<EventArgs>()), Times.Once);
    }
}
